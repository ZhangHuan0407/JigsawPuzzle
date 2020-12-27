﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEngine;
#else
using Newtonsoft.Json;
#endif

namespace JigsawPuzzle
{
    [ShareScript]
    internal class JPTask
    {
        /* field */
        internal readonly FileInfo InfoDataFile;
        internal readonly FileInfo BinDataFile;

        internal JigsawPuzzleInfoData JPInfoData;
        internal SpriteColorContainer SpriteColor;
        internal SpriteInfo EffectSpriteInfo;
        internal List<SpriteInfo> SpritesInfo;

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

                Pretreatment();
                SpeculatePreferredPosition();
                PredictLayout();
                FreeTemp();
                WriteOutResult();
            }
            catch (Exception e)
            {
                // Log
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
            finally
            {
                SpriteColor?.Clear();
            }
        }
        private bool TryReloadDataAncCheck()
        {
            // check
            string infoData = File.ReadAllText(InfoDataFile.FullName);
#if UNITY_EDITOR
            JPInfoData = JsonUtility.FromJson<JigsawPuzzleInfoData>(infoData);
#else
            JPInfoData = JsonConvert.DeserializeObject<JigsawPuzzleInfoData>(infoData);
#endif
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
                    spriteInfo.PreferredPositiosn = new PositionHeap(10);
                }
                else if (times > 450000)
                {
                    spriteInfo.PretreatmentPropensity = ShiftPositionPropensity.Random64;
                    spriteInfo.Propensity = ShiftPositionPropensity.Interval9;
                    spriteInfo.AccurateDistance = 4;
                    spriteInfo.PreferredPositiosn = new PositionHeap(6);
                }
                else if (times > 100000)
                {
                    spriteInfo.PretreatmentPropensity = ShiftPositionPropensity.Random16;
                    spriteInfo.Propensity = ShiftPositionPropensity.Interval3;
                    spriteInfo.AccurateDistance = 1;
                    spriteInfo.PreferredPositiosn = new PositionHeap(4);
                }
                else
                {
                    spriteInfo.PretreatmentPropensity = ShiftPositionPropensity.None;
                    spriteInfo.Propensity = ShiftPositionPropensity.Interval3;
                    spriteInfo.AccurateDistance = 1;
                    spriteInfo.PreferredPositiosn = new PositionHeap(2);
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
        }
        [Obsolete("当前此方法不具有实际功能")]
        private void PredictLayout()
        {

        }
        private void FreeTemp()
        {
            SpriteColor.Clear();
            SpriteColor = null;
            EffectSpriteInfo = null;
            SpritesInfo.Clear();
            SpritesInfo = null;
        }
        private void WriteOutResult()
        {
            JPInfoData.UpdateTime();
#if UNITY_EDITOR
            string contents = JsonUtility.ToJson(JPInfoData);
#else
            string contents = JsonConvert.SerializeObject(JPInfoData);
#endif
            File.WriteAllText(InfoDataFile.FullName, contents);
        }
    }
}