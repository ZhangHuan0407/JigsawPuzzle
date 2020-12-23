using System;
using System.Collections.Generic;
using System.Text;
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
        public Vector2 PreferredPosition { get; protected set; }
        public bool HavePreferredPositiosn { get; protected set; }

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
            HavePreferredPositiosn = false;
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
                    Color color = copyTexture.GetPixel(x + shiftX, y + shiftY);
                    Pixels[x, y] = new Vector4(color.r, color.g, color.b, color.a);
                }
        }
        public void CreateMinimap()
        {
            int width = (int)Mathf.Ceil(ImageSize.x / 4f);
            int height = (int)Mathf.Ceil(ImageSize.y / 4f);
            PixelsQuarter = new Vector4[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    PixelsQuarter[x, y] = Pixels[x * 4, y * 4];
        }

        public void TryGetPreferredPosition(JigsawPuzzleArguments arguments)
        {
            Vector4[,] effectPixelsQuarter = arguments.Effect.PixelsQuarter;
            RGBASpriteMatch match = new RGBASpriteMatch(effectPixelsQuarter, PixelsQuarter);
            _ = match.ExecuteAverageValue();
            if (match.PreferredPosition is Vector2Int position)
            {
                PreferredPosition = position * 4;
                HavePreferredPositiosn = true;
            }
            else
                HavePreferredPositiosn = false;
            Debug.Log($"{ImageIdentified}\n{nameof(TryGetPreferredPosition)} return => {HavePreferredPositiosn}.In {arguments.Stopwatch.ElapsedMilliseconds / 1000f:0.00}(s)\n");
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