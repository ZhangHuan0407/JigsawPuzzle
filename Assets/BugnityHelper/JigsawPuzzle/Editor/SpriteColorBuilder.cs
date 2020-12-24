using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
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

            throw new NotImplementedException();

        }
        private Texture2D GetEnableTexture2D(Texture2D source)
        {
            if (!TextureCopy.TryGetValue(source, out Texture2D copy))
            {

                throw new NotImplementedException();
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