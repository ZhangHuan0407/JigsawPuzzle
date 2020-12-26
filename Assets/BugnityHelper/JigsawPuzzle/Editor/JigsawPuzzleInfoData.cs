using System;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace JigsawPuzzle
{
    [ShareScripts]
    [Serializable]
    public class JigsawPuzzleInfoData
    {
        /* const */
        public const int SerializeVersion = 1;

        /* field */
#if UNITY_EDITOR
        [Tooltip("拼图信息文件创建时间")]
        public string CreationTime;
        [Tooltip("拼图信息文件修改时间")]
        public string LastWriteTime;
        [Tooltip("详细的精灵图片信息(不要修改内容)")]
        public SpriteInfo[] SpriteInfos;
        [HideInInspector]
        [SerializeField]
        private int Version;
#else
        public string CreationTime;
        public string LastWriteTime;
        public SpriteInfo[] SpriteInfos;
        public int Version;
#endif

        /* inter */
        public long BinDataLength
        {
            get => SpriteInfosBinDataLength();
        }

        /* ctor */
        public JigsawPuzzleInfoData()
        {
            Version = SerializeVersion;
        }

        /* func */
#if UNITY_EDITOR
        public void UpdateTime()
        {
            if (string.IsNullOrWhiteSpace(CreationTime))
                CreationTime = DateTime.Now.ToString();
            LastWriteTime = DateTime.Now.ToString();
        }
#endif

        public long SpriteInfosBinDataLength()
        {
            long length = 0;
            foreach (SpriteInfo spriteInfo in SpriteInfos)
                length += spriteInfo.ColorDataLength;
            return length;
        }
    }
}