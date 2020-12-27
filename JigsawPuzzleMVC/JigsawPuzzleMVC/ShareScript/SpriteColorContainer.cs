using System;
using System.Collections.Generic;

namespace JigsawPuzzle
{
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
        public void Clear() => SpriteColorData.Clear();
    }
}