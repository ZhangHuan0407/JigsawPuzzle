using System;
using System.Collections.Generic;

namespace JigsawPuzzle
{
    /// <summary>
    /// 精灵图片色彩数据的容器，在整个运算任务中保证色彩数据只序列化一次
    /// </summary>
    [ShareScript]
    public class SpriteColorContainer
    {
        /* field */
        protected Dictionary<SpriteInfo, JPColor[,]> SpriteColorData;

        /* inter */
        public JPColor[,] this[SpriteInfo spriteInfo] => SpriteColorData[spriteInfo];

        /* ctor */
        public SpriteColorContainer()
        {
            SpriteColorData = new Dictionary<SpriteInfo, JPColor[,]>();
        }

        /* func */
        public void Add(SpriteInfo spriteInfo, byte[] binData)
        {
            if (binData is null)
                throw new ArgumentNullException(nameof(binData));

            if (binData.Length < spriteInfo.ColorDataEndPosition
                || spriteInfo.ColorDataEndPosition > int.MaxValue)
                throw new ArgumentException(nameof(binData));

            if (SpriteColorData.ContainsKey(spriteInfo))
                SpriteColorData.Remove(spriteInfo);
            JPColor[,] colorData = new JPColor[spriteInfo.Width, spriteInfo.Height];
            
            long startPosition = spriteInfo.ColorDataStartPosition;
            int startIndex = (int)startPosition;
            for (int y = 0; y < spriteInfo.Height; y++)
                for (int x = 0; x < spriteInfo.Width; x++)
                {
                    colorData[x, y] = new JPColor(binData, startIndex);
                    startIndex += JPColor.FileDataPreJPColor;
                }
            SpriteColorData.Add(spriteInfo, colorData);
        }
        /// <summary>
        /// 立即清除所有精灵图片的色彩数据
        /// </summary>
        public void Clear() => SpriteColorData.Clear();
    }
}