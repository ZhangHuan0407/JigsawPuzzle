using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JigsawPuzzle
{
    [ShareScript]
    internal class JPTask
    {
        /* field */
        internal readonly FileInfo BinDataFile;
        internal readonly FileInfo InfoDataFile;

        internal SpriteInfo EffectSpriteInfo;
        internal JigsawPuzzleInfoData JPInfoData;
        internal SpriteColorContainer SpriteColorContainer;
        internal List<SpriteInfo> SpritesInfo;
        internal Stopwatch TaskStopwatch;
        internal StringBuilder Builder;

#if DEBUG && MVC
        public Analysis.Log Log;
#endif

        /* ctor */
        internal JPTask(FileInfo infoDataFile, FileInfo binDataFile)
        {
            InfoDataFile = infoDataFile ?? throw new ArgumentNullException(nameof(infoDataFile));
            BinDataFile = binDataFile ?? throw new ArgumentNullException(nameof(binDataFile));
        }

        /* func */
        internal void Start()
        {
            // 当前没有排队机制和内存评估机制，直接开始
            Task.Run(Execute);
        }

        internal void Execute()
        {
            try
            {
                InitParameter();
                TryReloadDataAncCheck();
                Pretreatment();
                SpeculatePreferredPosition();
                PredictLayout();
                FreeTemp();
                WriteOutResult();
            }
            catch (Exception e)
            {
                Builder?.AppendLine($"{nameof(DateTime)} : {DateTime.Now}")
                    .AppendLine(e.Message)
                    .AppendLine(e.StackTrace);
            }
            finally
            {
                SpriteColorContainer?.Clear();
                if (Builder != null)
                    File.WriteAllText($"{InfoDataFile.FullName}.log", Builder.ToString());
            }
        }
        private bool TryReloadDataAncCheck()
        {
            // check
            string infoData = File.ReadAllText(InfoDataFile.FullName);
            JPInfoData = JsonFuck.FromJsonToObject<JigsawPuzzleInfoData>(infoData);
            byte[] binData = File.ReadAllBytes(BinDataFile.FullName);

            if (binData.Length != JPInfoData.BinDataLength)
                throw new Exception($"binData.Length != JPInfoData.BinDataLength, {binData.Length}{JPInfoData.BinDataLength}");

            SpriteColorContainer = new SpriteColorContainer();
            EffectSpriteInfo = null;
            SpritesInfo = new List<SpriteInfo>();
            foreach (SpriteInfo spriteInfo in JPInfoData.SpriteInfos)
            {
                SpriteColorContainer.Add(spriteInfo, binData);
                if (spriteInfo.IsEffect)
                {
                    if (EffectSpriteInfo is null)
                        EffectSpriteInfo = spriteInfo;
                    else
                        throw new Exception("Effect 2.");
                }
                else
                    SpritesInfo.Add(spriteInfo);
            }
            return true;
        }
        private void InitParameter()
        {
#if DEBUG && MVC
            if (Log is null)
                throw new ArgumentNullException(nameof(Log));
#endif
            TaskStopwatch = Stopwatch.StartNew();
            Builder = new StringBuilder();
            Builder.AppendLine($"Finish {nameof(InitParameter)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
        }
        private void Pretreatment()
        {
            foreach (SpriteInfo spriteInfo in SpritesInfo)
            {
                spriteInfo.TotalNumberOfPossibilities = (EffectSpriteInfo.Width - spriteInfo.Width + 1) * (EffectSpriteInfo.Height - spriteInfo.Height + 1);

                spriteInfo.PretreatmentPropensity = ShiftPositionPropensity.Interval2;
                spriteInfo.Propensity = ShiftPositionPropensity.LineByLine;
                spriteInfo.AccurateDistance = 1;
                spriteInfo.PreferredPosHeap = new MinValuePointHeap(20, JPRGBAColorMatch.DefaultMaxSqrMagnitude);
                spriteInfo.MaxSqrMagnitude = JPRGBAColorMatch.DefaultMaxSqrMagnitude;
            }

            JPColor[,] effectSpriteColor = SpriteColorContainer[EffectSpriteInfo];
#if PARALLELMODE
            ParallelLoopResult result = Parallel.ForEach(SpritesInfo, (SpriteInfo spriteInfo) =>
            {
#else
            foreach (SpriteInfo spriteInfo in SpritesInfo)
            {
#endif
                if (spriteInfo.PretreatmentPropensity == ShiftPositionPropensity.None)
                    return;

                JPRGBAColorMatch match = new JPRGBAColorMatch(
                    effectSpriteColor,
                    SpriteColorContainer[spriteInfo],
                    spriteInfo.PretreatmentPropensity);
#if DEBUG && MVC
                match.Log = Log;
#endif
                match.TryGetPreferredPosition();
                foreach (WeightedPoint weightedPoint in match)
                    spriteInfo.PreferredPosHeap.AddMinItem(weightedPoint);
#if DEBUG && MVC && !PARALLELMODE
                StringBuilder builder = new StringBuilder();
                foreach (WeightedPoint weightedPoint in spriteInfo.PreferredPosHeap.ToArray())
                    builder.AppendLine($"{nameof(WeightedPoint)} : {weightedPoint}");
                Log.WriteData(null, "Pretreatment spriteInfo.PreferredPosHeap : ", builder.ToString());
#endif
#if PARALLELMODE
            });
#else
            }
#endif
            Builder.AppendLine($"Finish {nameof(Pretreatment)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
        }
        private void SpeculatePreferredPosition()
        {
            JPColor[,] effectSpriteColor = SpriteColorContainer[EffectSpriteInfo];
#if PARALLELMODE
            ParallelLoopResult result = Parallel.ForEach(SpritesInfo, (SpriteInfo spriteInfo) =>
            {
#else
            foreach (SpriteInfo spriteInfo in SpritesInfo)
            {
#endif
                if (spriteInfo.Propensity == ShiftPositionPropensity.None)
                    return;

                WeightedPoint[] preferredPositions = spriteInfo.PreferredPosHeap.ToArray();
                spriteInfo.PreferredPosHeap = new MinValuePointHeap(10, 1f);

                JPRGBAColorMatch accurateMatch = new JPRGBAColorMatch(
                    effectSpriteColor,
                    SpriteColorContainer[spriteInfo],
                    ShiftPositionPropensity.None,
                    preferredPositions);
#if DEBUG && MVC
                accurateMatch.Log = Log;
#endif
                accurateMatch.TryGetNearlyPreferredPosition(spriteInfo.AccurateDistance);
                // 对临近点进行聚类并排序入堆
                List<WeightedPoint> weightedPoints = WeightedPoint.GroupWeightedPoints(accurateMatch.PreferredPosition, spriteInfo.Width / 4, spriteInfo.Height / 4);

#if DEBUG && MVC
                Log.WriteData(null, $"accurateMatch.PreferredPosition : {accurateMatch.PreferredPosition.Count}, weightedPoints : {weightedPoints.Count}\n" +
                    $"PreferredPosition : {JsonFuck.FromObjectToJson(accurateMatch.PreferredPosition)}");
#endif
                foreach (WeightedPoint weightedPoint in weightedPoints)
                    spriteInfo.PreferredPosHeap.AddMinItem(weightedPoint);
                spriteInfo.PreferredPositions = spriteInfo.PreferredPosHeap.ToArray();
#if PARALLELMODE
            });
#else
            }
#endif
            Builder.AppendLine($"Finish {nameof(SpeculatePreferredPosition)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
        }
        [Obsolete("当前此方法不具有实际功能")]
        private void PredictLayout()
        {
            Builder.AppendLine($"Finish {nameof(PredictLayout)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
        }
        private void FreeTemp()
        {
            EffectSpriteInfo = null;
            SpriteColorContainer.Clear();
            SpriteColorContainer = null;
            SpritesInfo.Clear();
            SpritesInfo = null;
            GC.Collect();
            Builder.AppendLine($"Finish {nameof(FreeTemp)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
        }
        private void WriteOutResult()
        {
            JPInfoData.UpdateTime();
            string contents = JsonFuck.FromObjectToJson(JPInfoData);
            File.WriteAllText(InfoDataFile.FullName, contents);
            Builder.AppendLine($"Finish {nameof(WriteOutResult)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
#if DEBUG && MVC
            Log.WriteOut();
#endif
        }

        internal static void StartRemoteTask(JPTaskConnector connector, string[] filesName)
        {
            if (connector is null)
                throw new ArgumentNullException(nameof(connector));
            if (filesName is null)
                throw new ArgumentNullException(nameof(filesName));

            connector.PostForm("SelectFiles", "Explorer",
                new Dictionary<string, object>(){ { "File", filesName } },
                (object selectFilesMessage) => 
                {
#if UNITY_EDITOR
                    UnityEngine.Debug.Log(selectFilesMessage);
#endif
                    connector.Get("StartNew", "Task",
                        (object startNewMessage) =>
                        {
#if UNITY_EDITOR
                            UnityEngine.Debug.Log(startNewMessage);
#endif
                        });
                });
        }
    }
}