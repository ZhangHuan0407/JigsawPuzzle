using System;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
{
    [Serializable]
    public class SpriteInfo
    {
        /* const */

        /* field */
        public bool IsEffect;
        public string SpriteFullPath;

        public int Width;
        public int Height;
        public int TexturePosX;
        public int TexturePosY;

        public long ColorDataStartPosition;

        /* inter */
        public long ColorDataLength => JPColor.FileDataPreJPColor * Width * Height;
        public int PixelCount => Width * Height;

        /* ctor */
        public SpriteInfo(Sprite sprite, bool isEffect)
        {
            if (sprite == null)
                throw new ArgumentNullException(nameof(sprite));
            if (sprite.texture == null)
                throw new ArgumentNullException(nameof(sprite.texture));
            IsEffect = isEffect;
            SpriteFullPath = $"{sprite.texture.name}/{sprite.name}";

            Width = (int)sprite.rect.width;
            Height = (int)sprite.rect.height;
            TexturePosX = (int)sprite.rect.x;
            TexturePosY = (int)sprite.rect.y;
        }
    }
}