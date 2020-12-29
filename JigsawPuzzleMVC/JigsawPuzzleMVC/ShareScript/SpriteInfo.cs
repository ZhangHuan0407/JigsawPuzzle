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
        /// <summary>
        /// 精灵图片绑定的是否是一张效果图
        /// </summary>
        public bool IsEffect;
        /// <summary>
        /// 精灵图片资源定位路径
        /// </summary>
        public string SpriteFullPath;

        /// <summary>
        /// 精灵图片绑定图片的纹理宽度(px)
        /// </summary>
        public int Width;
        /// <summary>
        /// 精灵图片绑定图片的纹理高度(px)
        /// </summary>
        public int Height;
        /// <summary>
        /// 精灵图片绑定图片的横向纹理起始坐标(px)
        /// </summary>
        public int TexturePosX;
        /// <summary>
        /// 精灵图片绑定图片的纵向纹理起始坐标(px)
        /// </summary>
        public int TexturePosY;

        /// <summary>
        /// 精灵图片颜色数据在二进制数据中的起始位置
        /// </summary>
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
        [HideInInspector]
        public int AccurateDistance;
#else
        public int TotalNumberOfPossibilities;
        public ShiftPositionPropensity PretreatmentPropensity;
        public ShiftPositionPropensity Propensity;
        public PositionHeap PreferredPositiosn;
        public float MaxSqrMagnitude;
        public int AccurateDistance;
#endif
        #endregion

        /* inter */
        public long ColorDataLength => JPColor.FileDataPreJPColor * Width * Height;
        /// <summary>
        /// 精灵图片的像素数量
        /// </summary>
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