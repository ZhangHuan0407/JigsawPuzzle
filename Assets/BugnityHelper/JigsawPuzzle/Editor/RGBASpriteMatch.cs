using System;
using System.Collections.Generic;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
{
    public class RGBASpriteMatch : SpriteMatch<Vector3>
    {
        /* field */
        public static Vector3 DefaultLimitMin = new Vector3(0.1f, 0.1f, 0.1f);

        /* ctor */
        public RGBASpriteMatch(Vector4[,] background, Vector4[,] fontground) : base(background, fontground)
        {
        }

        /* func */
        public override Vector3 ExecuteAverageValue(object limitMin = null)
        {
            if (limitMin is null)
                limitMin = DefaultLimitMin;
            return base.ExecuteAverageValue(limitMin);
        }

        protected override bool AIsBtterThanB(Vector3 A, Vector3 B) => A.magnitude < B.magnitude;
        protected override Vector3 GetAverage(Vector3[,] delta)
        {
            Vector3 buffer = new Vector3();
            foreach (Vector3 itemDelta in delta)
                buffer += itemDelta;
            return buffer / delta.Length;
        }

        protected override Vector3 PixelDelta(Vector4 fontColor, Vector4 backColor)
        {
            float transparent = fontColor.z;
            float deltaW = (fontColor.w - backColor.w) * transparent;
            float deltaX = (fontColor.x - backColor.x) * transparent;
            float deltaY = (fontColor.y - backColor.y) * transparent;
            Vector3 delta = new Vector3(
                deltaW < 0 ? deltaW : -deltaW,
                deltaX < 0 ? deltaX : -deltaX,
                deltaY < 0 ? deltaY : -deltaY);
            return delta;
        }
    }
}