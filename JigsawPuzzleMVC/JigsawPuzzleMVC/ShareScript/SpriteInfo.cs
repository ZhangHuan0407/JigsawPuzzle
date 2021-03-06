﻿using System;

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

        public WeightedPoint[] PreferredPositions;
        #endregion RawData

        #region  StatisticalData
#if UNITY_EDITOR
        [NonSerialized]
        [HideInInspector]
        public int TotalNumberOfPossibilities;
        [NonSerialized]
        [HideInInspector]
        public ShiftPositionPropensity PretreatmentPropensity;
        [NonSerialized]
        [HideInInspector]
        public ShiftPositionPropensity Propensity;
        [NonSerialized]
        [HideInInspector]
        public MinValuePointHeap PreferredPosHeap;
        [NonSerialized]
        [HideInInspector]
        public float MaxSqrMagnitude;
        [NonSerialized]
        [HideInInspector]
        public int AccurateDistance;
#else
        [NonSerialized]
        public int TotalNumberOfPossibilities;
        [NonSerialized]
        public ShiftPositionPropensity PretreatmentPropensity;
        [NonSerialized]
        public ShiftPositionPropensity Propensity;
        [NonSerialized]
        public MinValuePointHeap PreferredPosHeap;
        [NonSerialized]
        public float MaxSqrMagnitude;
        [NonSerialized]
        public int AccurateDistance;
#endif
        #endregion

        /* inter */
        public long ColorDataLength => JPColor.PreJPColorDataSize * Width * Height;
        /// <summary>
        /// 精灵图片的像素数量
        /// </summary>
        public int PixelCount => Width * Height;
        public long ColorDataEndPosition => ColorDataStartPosition + ColorDataLength;

#if UNITY_EDITOR
        /// <summary>
        /// 尝试获取首选位置中的最佳点，
        /// <para>效果图左下角作为起始点，图像中心作为终点</para>
        /// </summary>
        public Vector2Int? BestOne
        {
            get
            {
                Vector2Int? bestPosition = null;
                if (PreferredPositions.Length > 0)
                {
                    WeightedPoint weightedPoint = PreferredPositions[PreferredPositions.Length - 1];
                    int shiftX = weightedPoint.Position.X + Width / 2;
                    int shiftY = weightedPoint.Position.Y + Height / 2;
                    bestPosition = new Vector2Int(shiftX, shiftY);
                }
                return bestPosition;
            }
        }
#endif

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