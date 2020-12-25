using System;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
{
    [Serializable]
    public class JigsawPuzzleInfoData
    {
        /* field */
        [Tooltip("拼图信息文件创建时间")]
        public string CreationTime;
        [Tooltip("拼图信息文件修改时间")]
        public string LastWriteTime;
        [Tooltip("详细的精灵图片信息(不要修改内容)")]
        public SpriteInfo[] SpriteInfos;

        /* ctor */
        public JigsawPuzzleInfoData()
        {

        }

        /* func */
        public void UpdateTime()
        {
            if (string.IsNullOrWhiteSpace(CreationTime))
                CreationTime = DateTime.Now.ToString();
            LastWriteTime = DateTime.Now.ToString();
        }
    }
}