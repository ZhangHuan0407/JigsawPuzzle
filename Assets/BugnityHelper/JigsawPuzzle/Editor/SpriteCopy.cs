using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
{
    public class SpriteCopy
    {
        /* field */
        internal string ImageIdentified;

        private readonly Sprite m_Sprite;
        internal readonly Vector2Int TexturePosition;
        internal readonly Vector2Int ImageSize;
        /// <summary>
        /// 原始像素数据拷贝
        /// </summary>
        internal Vector4[,] Pixels;
        /// <summary>
        /// 1/4像素数据拷贝
        /// </summary>
        public Vector4[,] PixelsQuarter { get; protected set; }
        public Vector2 PreferredLocalPosition { get; protected set; }
        public bool GetPreferredPositiosn { get; protected set; }

        /* inter */

        /* ctor */
        public SpriteCopy(Sprite sprite)
        {
            if (sprite == null)
                throw new ArgumentNullException(nameof(sprite));
            m_Sprite = sprite;

            Rect rect = m_Sprite.rect;
            TexturePosition = new Vector2Int((int)rect.x, (int)rect.y);
            ImageSize = new Vector2Int((int)rect.width, (int)rect.height);
            Pixels = new Vector4[ImageSize.x, ImageSize.y];
            ImageIdentified = $"{AssetDatabase.GetAssetPath(sprite)}/{sprite.name}";
            GetPreferredPositiosn = false;
        }

        /* func */
        public void LoadPixel(JigsawPuzzleArguments arguments)
        {
            Texture2D copyTexture = arguments.GetTexture2DCopy(m_Sprite);
            for (int y = 0; y < ImageSize.y; y++)
                for (int x = 0; x < ImageSize.x; x++)
                {
                    int shiftX = TexturePosition.x;
                    int shiftY = TexturePosition.y;
                    Pixels[x, y] = copyTexture.GetPixel(x + shiftX, y + shiftY);
                }
        }
        public void CreateMinimap()
        {
            int width = (int)Mathf.Ceil(ImageSize.x / 4f);
            int height = (int)Mathf.Ceil(ImageSize.y / 4f);
            PixelsQuarter = new Vector4[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    PixelsQuarter[x, y] = Pixels[x * 4, y * 4];
                }
        }

        public void TryGetPreferredLocalPosition(JigsawPuzzleArguments arguments)
        {
            Vector4[,] effectPixelsQuarter = arguments.Effect.PixelsQuarter;


            Debug.LogWarning("Not Implemented Exception");


        }

        /* operator */
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}