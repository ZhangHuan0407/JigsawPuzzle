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
        /* field */
#if UNITY_EDITOR
        [Tooltip("拼图信息文件创建时间")]
        public string CreationTime;
        [Tooltip("拼图信息文件修改时间")]
        public string LastWriteTime;
        [Tooltip("详细的精灵图片信息(不要修改内容)")]
        public SpriteInfo[] SpriteInfos;
#else
        public string CreationTime;
        public string LastWriteTime;
        public SpriteInfo[] SpriteInfos;
#endif

        /* ctor */
        public JigsawPuzzleInfoData()
        {

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
    }
}