using System;
using System.IO;
using System.Text.RegularExpressions;
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
        internal FileInfo InfoDataFile;
        internal FileInfo BinDataFile;

        internal JigsawPuzzleInfoData JPInfoData;
        internal SpriteColorContainer SpriteColor;

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
            foreach (SpriteInfo item in JPInfoData.SpriteInfos)
                SpriteColor.Add(item, binData);

            return true;
        }
        private void Pretreatment()
        {

        }
        private void SpeculatePreferredPosition()
        {

        }
        private void PredictLayout()
        {
        }
        private void FreeTemp()
        {
            SpriteColor.Clear();
            SpriteColor = null;
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