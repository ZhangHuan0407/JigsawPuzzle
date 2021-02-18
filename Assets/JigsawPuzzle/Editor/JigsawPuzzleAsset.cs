using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle
{
    public class JigsawPuzzleAsset : ScriptableObject
    {
        /* const */
        public const string AssetDirectory = "Assets/Editor Default Resources/JigsawPuzzle/Task";

        /* field */
        [Tooltip("效果图")]
        public Sprite Effect;
        [Tooltip("图集内的零散图片")]
        public Sprite[] Sprites;
        [Tooltip("数据文件名称(不要修改内容)")]
        public string DataName;

        [Tooltip("Data")]
        public JigsawPuzzleInfoData InfoData;

        [HideInInspector]
        [NonSerialized]
        public Dictionary<string, SpriteInfo> SpriteMap;

        /* inter */
        public string AssetFullName => $"{AssetDirectory}/{DataName}.asset";
        public string InfoDataName => $"Task/{DataName}.json";
        public string InfoDataFullName => $"{AssetDirectory}/{DataName}.json";
        public string BinDataName => $"Task/{DataName}.bytes";
        public string BinDataFullName => $"{AssetDirectory}/{DataName}.bytes";

        public SpriteInfo this[string spriteName]
        {
            get 
            {
                if (!Check)
                    return null;
                if (SpriteMap is null)
                    SpriteMap = InfoData.ToDictionary();
                SpriteMap.TryGetValue(spriteName, out SpriteInfo result);
                return result;
            }
        }

        /* ctor */
        public JigsawPuzzleAsset()
        {
            Sprites = new Sprite[0];
            DataName = Guid.NewGuid().ToString();
        }

        /* inter */
        /// <summary>
        /// 检查当前用户提供的数据是否完整
        /// </summary>
        public bool Check => Effect && Sprites != null && Sprites.Length > 0;

        /* func */
        public void RemoveInvalidData()
        {
            List<Sprite> buffer = new List<Sprite>(Sprites.Length);
            foreach (Sprite sprite in Sprites)
            {
                if (sprite && sprite.texture)
                    buffer.Add(sprite);
            }
            Sprites = buffer.ToArray();
        }

        public void CreateDataFiles()
        {
            RemoveInvalidData();
            if (!Check)
            {
                Debug.LogError("Not pass check!");
                return;
            }

            InfoData = new JigsawPuzzleInfoData()
            {
                SpriteInfos = new SpriteInfo[Sprites.Length + 1],
                DataName = DataName,
            };
            SpriteInfo[] spriteInfos = InfoData.SpriteInfos;
            spriteInfos[0] = new SpriteInfo(Effect, true);
            for (int index = 0; index < Sprites.Length; index++)
                spriteInfos[index + 1] = new SpriteInfo(Sprites[index], false);

            using (SpriteColorBuilder spriteColorBuilder = new SpriteColorBuilder())
            {
                spriteColorBuilder.AppendSprite(Effect, spriteInfos[0]);
                for (int index = 1; index < spriteInfos.Length; index++)
                    spriteColorBuilder.AppendSprite(Sprites[index - 1], spriteInfos[index]);
                File.WriteAllBytes(BinDataFullName, spriteColorBuilder.ToArray());
            }

            InfoData.UpdateTime();

            string content = JsonUtility.ToJson(InfoData, true);
            File.WriteAllText(InfoDataFullName, content);
        }
        public void RemoveDataFiles() 
        {
            File.Delete(InfoDataFullName);
            File.Delete($"{InfoDataFullName}.meta");
            File.Delete(BinDataFullName);
            File.Delete($"{BinDataFullName}.meta");
        }

        public void SendToServer()
        {
            byte[] BinData = File.ReadAllBytes(BinDataFullName);
            JPTaskConnector connector = EditorWindow.GetWindow<JigsawPuzzleWindow>().Connector.Value;
            connector.PostFile(
                "Explorer", "UploadFiles",
                BinData, "File", $"{BinDataName}",
                (object obj) => Debug.Log($"Task/CreateNewData success. File : {BinDataName}\n{obj}"),
                (HttpResponseMessage message) => Debug.LogError($"Task/CreateNewData failed. File : {BinDataName}\n{message.Content.ReadAsStringAsync().Result}"));
            BinData = File.ReadAllBytes(InfoDataFullName);
            connector.PostFile(
                "Explorer", "UploadFiles",
                BinData, "File", $"{InfoDataName}",
                (object obj) => Debug.Log($"Task/CreateNewData success. File : {InfoDataName}\n{obj}"),
                (HttpResponseMessage message) => Debug.LogError($"Task/CreateNewData failed. File : {InfoDataName}\n{message.Content.ReadAsStringAsync().Result}"));
        }

        public void AddIntoWindow()
        {
            if (JigsawPuzzleWindow.JPAssetPool is null)
                JigsawPuzzleWindow.JPAssetPool = new Dictionary<Sprite, JigsawPuzzleAsset>();
            if (!JigsawPuzzleWindow.JPAssetPool.ContainsKey(Effect))
                JigsawPuzzleWindow.JPAssetPool.Add(Effect, this);
        }

        public static IEnumerable<string> GetAssetList()
        {
            foreach (string fileFullPath in Directory.GetFiles(AssetDirectory, "*.asset"))
            {
                FileInfo fileInfo = new FileInfo(fileFullPath);
                yield return JPFileMap.GetFileName(fileInfo.Name);
            }
        }

        public static JigsawPuzzleAsset GetNew()
        {
            Directory.CreateDirectory(AssetDirectory);
            JigsawPuzzleAsset instance = CreateInstance<JigsawPuzzleAsset>();
            AssetDatabase.CreateAsset(instance, instance.AssetFullName);
            return instance;
        }
        public static void OverrideInfoData(string name, byte[] binData)
        {
            Directory.CreateDirectory(AssetDirectory);
            if (binData is null)
                return;
            File.WriteAllBytes($"{AssetDirectory}/{name}.json", binData);
            JigsawPuzzleAsset asset = AssetDatabase.LoadAssetAtPath<JigsawPuzzleAsset>($"{AssetDirectory}/{name}.asset");
            JigsawPuzzleInfoData infoData = JsonFuck.FromJsonToObject<JigsawPuzzleInfoData>(Encoding.UTF8.GetString(binData));
            asset.InfoData = infoData;
            EditorUtility.SetDirty(asset);
        }
    }
}