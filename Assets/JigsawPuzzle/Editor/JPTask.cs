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
        internal SpriteColorContainer SpriteColor;
        internal List<SpriteInfo> SpritesInfo;
        internal Stopwatch TaskStopwatch;
        internal StringBuilder Builder;

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
                // 修改为直接在函数内报错
                if (!TryReloadDataAncCheck())
                    throw new ArgumentException($"Throw Exception where invoke {nameof(JPTask)}.{nameof(TryReloadDataAncCheck)}");

                InitParameter();
                Pretreatment();
                SpeculatePreferredPosition();
                PredictLayout();
                FreeTemp();
                WriteOutResult();
            }
            catch (Exception e)
            {
                Builder.AppendLine($"{nameof(DateTime)} : {DateTime.Now}")
                    .AppendLine(e.Message)
                    .AppendLine(e.StackTrace);
            }
            finally
            {
                SpriteColor?.Clear();
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
                return false;

            SpriteColor = new SpriteColorContainer();
            EffectSpriteInfo = null;
            SpritesInfo = new List<SpriteInfo>();
            foreach (SpriteInfo spriteInfo in JPInfoData.SpriteInfos)
            {
                SpriteColor.Add(spriteInfo, binData);
                if (spriteInfo.IsEffect)
                {
                    if (EffectSpriteInfo is null)
                        EffectSpriteInfo = spriteInfo;
                    else
                        return false;
                }
                else
                    SpritesInfo.Add(spriteInfo);
            }
            return true;
        }
        private void InitParameter()
        {
            TaskStopwatch = Stopwatch.StartNew();
            Builder = new StringBuilder();
            Builder.AppendLine($"Finish {nameof(InitParameter)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
        }
        private void Pretreatment()
        {
            foreach (SpriteInfo spriteInfo in SpritesInfo)
            {
                spriteInfo.TotalNumberOfPossibilities = (EffectSpriteInfo.Width - spriteInfo.Width + 1) * (EffectSpriteInfo.Height - spriteInfo.Height + 1);

                int times = spriteInfo.TotalNumberOfPossibilities;
                if (times > 900000)
                {
                    spriteInfo.PretreatmentPropensity = ShiftPositionPropensity.Random256;
                    spriteInfo.Propensity = ShiftPositionPropensity.Interval9;
                    spriteInfo.AccurateDistance = 4;
                    spriteInfo.PreferredPositiosn = new MinValuePointHeap(10);
                }
                else if (times > 450000)
                {
                    spriteInfo.PretreatmentPropensity = ShiftPositionPropensity.Random64;
                    spriteInfo.Propensity = ShiftPositionPropensity.Interval9;
                    spriteInfo.AccurateDistance = 4;
                    spriteInfo.PreferredPositiosn = new MinValuePointHeap(6);
                }
                else if (times > 100000)
                {
                    spriteInfo.PretreatmentPropensity = ShiftPositionPropensity.Random16;
                    spriteInfo.Propensity = ShiftPositionPropensity.Interval3;
                    spriteInfo.AccurateDistance = 1;
                    spriteInfo.PreferredPositiosn = new MinValuePointHeap(4);
                }
                else
                {
                    spriteInfo.PretreatmentPropensity = ShiftPositionPropensity.None;
                    spriteInfo.Propensity = ShiftPositionPropensity.Interval3;
                    spriteInfo.AccurateDistance = 1;
                    spriteInfo.PreferredPositiosn = new MinValuePointHeap(2);
                }
            }

            JPColor[,] effectSpriteColor = SpriteColor[EffectSpriteInfo];
            foreach (SpriteInfo spriteInfo in SpritesInfo)
            {
                if (spriteInfo.PretreatmentPropensity == ShiftPositionPropensity.None)
                {
                    spriteInfo.MaxSqrMagnitude = JPRGBAColorMatch.DefaultMaxSqrMagnitude;
                    continue;
                }

                JPRGBAColorMatch match = new JPRGBAColorMatch(
                    effectSpriteColor,
                    SpriteColor[spriteInfo],
                    spriteInfo.PretreatmentPropensity,
                    JPRGBAColorMatch.DefaultMaxSqrMagnitude);
                match.TryGetPreferredPosition();
                Point preferredPosition = default;
                float maxSqrMagnitude = 1f;
                foreach ((Point, float) capture in match)
                    if (capture.Item2 < maxSqrMagnitude)
                    {
                        maxSqrMagnitude = capture.Item2;
                        preferredPosition = capture.Item1;
                    }
                spriteInfo.PreferredPositiosn.AddMinItem(preferredPosition, maxSqrMagnitude);
                spriteInfo.MaxSqrMagnitude = maxSqrMagnitude;
            }
            Builder.AppendLine($"Finish {nameof(Pretreatment)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
        }
        private void SpeculatePreferredPosition()
        {
            JPColor[,] effectSpriteColor = SpriteColor[EffectSpriteInfo];
            foreach (SpriteInfo spriteInfo in SpritesInfo)
            {
                if (spriteInfo.Propensity == ShiftPositionPropensity.None)
                    throw new ArgumentException($"spriteInfo.Propensity is {spriteInfo.Propensity}.");

                JPRGBAColorMatch match = new JPRGBAColorMatch(
                    effectSpriteColor,
                    SpriteColor[spriteInfo],
                    spriteInfo.Propensity,
                    spriteInfo.MaxSqrMagnitude);
                match.TryGetPreferredPosition();
                foreach ((Point, float) capture in match)
                    spriteInfo.PreferredPositiosn.AddMinItem(capture.Item1, capture.Item2);

                (Point, float)[] preferredPositiosn = spriteInfo.PreferredPositiosn.ToArray();
                foreach ((Point, float) position in preferredPositiosn)
                {
                    JPRGBAColorMatch accurateMatch = new JPRGBAColorMatch(
                    effectSpriteColor,
                    SpriteColor[spriteInfo],
                    ShiftPositionPropensity.None,
                    position.Item2);
                    accurateMatch.TryGetNearlyPreferredPosition(position.Item1, spriteInfo.AccurateDistance);
                    (Point, float) bestPosition = accurateMatch.BestOne();
                    spriteInfo.PreferredPositiosn.AddMinItem(bestPosition.Item1, bestPosition.Item2);
                }
            }
            Builder.AppendLine($"Finish {nameof(SpeculatePreferredPosition)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
        }
        [Obsolete("当前此方法不具有实际功能")]
        private void PredictLayout()
        {
            Builder.AppendLine($"Finish {nameof(PredictLayout)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
        }
        private void FreeTemp()
        {
            SpriteColor.Clear();
            SpriteColor = null;
            EffectSpriteInfo = null;
            SpritesInfo.Clear();
            SpritesInfo = null;
            Builder.AppendLine($"Finish {nameof(FreeTemp)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
        }
        private void WriteOutResult()
        {
            JPInfoData.UpdateTime();
            string contents = JsonFuck.FromObjectToJson(JPInfoData);
            File.WriteAllText(InfoDataFile.FullName, contents);
            Builder.AppendLine($"Finish {nameof(WriteOutResult)}, {nameof(TaskStopwatch)} : {TaskStopwatch.ElapsedMilliseconds} ms");
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
                    UnityEngine.Debug.Log(selectFilesMessage);
                    connector.Get("StartNew", "Task",
                        (object startNewMessage) =>
                        {
                            UnityEngine.Debug.Log(startNewMessage);
                        });
                });
        }
    }
}