using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JigsawPuzzle
{
    public class SpriteColorBuilder : IDisposable
    {
        /* field */
        private Dictionary<Texture2D, Texture2D> TextureCopy;
        private MemoryStream Stream;

        /* ctor */
        public SpriteColorBuilder()
        {
            TextureCopy = new Dictionary<Texture2D, Texture2D>();
            Stream = new MemoryStream();
        }

        /* inter */
        public byte[] ToArray() => Stream.ToArray();

        /* func */
        public void AppendSprite(Sprite sprite, SpriteInfo spriteInfo)
        {
            if (sprite == null)
                throw new ArgumentNullException(nameof(sprite));
            if (spriteInfo is null)
                throw new ArgumentNullException(nameof(spriteInfo));
            if (sprite.texture == null)
                throw new ArgumentNullException(nameof(sprite.texture));
            Texture2D texture2D = GetEnableTexture2D(sprite.texture);

            if (spriteInfo.ColorDataLength > int.MaxValue)
                throw new ArgumentOutOfRangeException($"{nameof(spriteInfo)}.{nameof(SpriteInfo.ColorDataLength)} : {spriteInfo.ColorDataLength} is greater than {int.MaxValue}");

            spriteInfo.ColorDataStartPosition = Stream.Position;
            int shiftX = spriteInfo.TexturePosX;
            int shiftY = spriteInfo.TexturePosY;

            byte[] buffer = new byte[spriteInfo.ColorDataLength];
            int startIndex = 0;
            for (int y = 0; y < spriteInfo.Height; y++)
                for (int x = 0; x < spriteInfo.Width; x++)
                {
                    Color color = texture2D.GetPixel(x + shiftX, y + shiftY);
                    Color.RGBToHSV(color, out float h, out float s, out float v);
                    buffer[startIndex++] = (byte)(color.r * 255);
                    buffer[startIndex++] = (byte)(color.g * 255);
                    buffer[startIndex++] = (byte)(color.b * 255);
                    buffer[startIndex++] = (byte)(color.a * 255);
                    buffer[startIndex++] = (byte)(h * 255);
                }
            Stream.Write(buffer, 0, buffer.Length);
        }
        private Texture2D GetEnableTexture2D(Texture2D source)
        {
            if (!TextureCopy.TryGetValue(source, out Texture2D copy))
            {
                copy = new Texture2D(source.width, source.height, source.format, false);
                byte[] buffer = source.GetRawTextureData();
                copy.LoadRawTextureData(buffer);
                TextureCopy.Add(source, copy);
            }
            return copy;
        }

        /* IDisposable */
        public void Dispose()
        {
            TextureCopy.Clear();
            TextureCopy = null;
            Stream.Dispose();
            Stream = null;
        }
    }
}