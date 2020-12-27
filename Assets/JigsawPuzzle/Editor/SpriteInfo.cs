using System;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace JigsawPuzzle
{
    [ShareScript]
    [Serializable]
    public class SpriteInfo
    {
        /* const */

        /* field */
        #region RawData
        public bool IsEffect;
        public string SpriteFullPath;

        public int Width;
        public int Height;
        public int TexturePosX;
        public int TexturePosY;

        public long ColorDataStartPosition;
        #endregion RawData

        #region  StatisticalData
#if UNITY_EDITOR
        [HideInInspector]
        public int TotalNumberOfPossibilities;
        [HideInInspector]
        public ShiftPositionPropensity PretreatmentPropensity;
        [HideInInspector]
        public ShiftPositionPropensity Propensity;
        [HideInInspector]
        public PositionHeap PreferredPositiosn;
        [HideInInspector]
        public float MaxSqrMagnitude;
#else
        public int TotalNumberOfPossibilities;
        public ShiftPositionPropensity PretreatmentPropensity;
        public ShiftPositionPropensity Propensity;
        public PositionHeap PreferredPositiosn;
        public float MaxSqrMagnitude;
#endif
        #endregion

        /* inter */
        public long ColorDataLength => JPColor.FileDataPreJPColor * Width * Height;
        public int PixelCount => Width * Height;
        public long ColorDataEndPosition => ColorDataStartPosition + ColorDataLength;

        /* ctor */
#if UNITY_EDITOR
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
#endif
        public SpriteInfo() { }
    }
}