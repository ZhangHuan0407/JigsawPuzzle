using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using System.Text;

namespace BugnityHelper.JigsawPuzzle
{
    public class JigsawPuzzleAsset : ScriptableObject
    {
        /* const */
        public const string AssetDirectory = "Assets/Editor Default Resources/BugnityHelper/JigsawPuzzle/TaskAndData";

        /* field */
        [Tooltip("效果图")]
        public Sprite Effect;
        [Tooltip("图集内的零散图片")]
        public Sprite[] Sprites;
        [Tooltip("数据文件名称(不要修改内容)")]
        public string DataName;

        [Header("Data")]
        public string MigrationTime;
        [Tooltip("详细的精灵图片信息(不要修改内容)")]
        public SpriteInfo[] SpriteInfos;

        /* inter */
        public string AssetFullName => $"{AssetDirectory}/{DataName}.asset";
        public string InfoDataFullName => $"{AssetDirectory}/{DataName}.json";
        public string BinDataFullName => $"{AssetDirectory}/{DataName}.bytes";

        /* ctor */
        public JigsawPuzzleAsset()
        {
            Sprites = new Sprite[0];
            DataName = Guid.NewGuid().ToString();
            SpriteInfos = new SpriteInfo[0];
            MigrationTime = string.Empty;
        }

        /* inter */
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

            SpriteInfos = new SpriteInfo[Sprites.Length + 1];
            SpriteInfos[0] = new SpriteInfo(Effect, true);
            for (int index = 0; index < Sprites.Length; index++)
                SpriteInfos[index + 1] = new SpriteInfo(Sprites[index], false);

            using (SpriteColorBuilder spriteBuilder = new SpriteColorBuilder())
            {
                spriteBuilder.AppendSprite(Effect, SpriteInfos[0]);
                for (int index = 1; index < SpriteInfos.Length; index++)
                    spriteBuilder.AppendSprite(Sprites[index - 1], SpriteInfos[(int)index]);
                File.WriteAllBytes(BinDataFullName, spriteBuilder.ToArray());
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("{spriteInfo = [");
            foreach (SpriteInfo spriteInfo in SpriteInfos)
                stringBuilder.Append(EditorJsonUtility.ToJson(spriteInfo))
                    .AppendLine(",");
            stringBuilder.AppendLine("]}");
            File.WriteAllText(InfoDataFullName, stringBuilder.ToString());
            stringBuilder.Clear();

            MigrationTime = DateTime.Now.ToString();
            throw new NotImplementedException();
        }
        public void RemoveDataFiles() 
        {
            File.Delete(InfoDataFullName);
            File.Delete(BinDataFullName);
        }

        public static IEnumerable<string> GetAssetList() => Directory.GetFiles(AssetDirectory, "*.asset");
        public static JigsawPuzzleAsset GetNew()
        {
            JigsawPuzzleAsset instance = CreateInstance<JigsawPuzzleAsset>();
            AssetDatabase.CreateAsset(instance, instance.AssetFullName);
            return instance;
        }
    }
}