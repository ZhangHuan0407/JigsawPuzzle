using System;
using System.Collections.Generic;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
{
    public abstract class SpriteMatch<Vector>
    {
        /* field */
        public Vector PerfectValue { get; protected set; }
        public Vector4[,] Background { get; protected set; }
        public Vector4[,] Fontground { get; protected set; }
        public Vector2Int? PreferredPosition;

        /* func */
        protected SpriteMatch(Vector4[,] background, Vector4[,] fontground)
        {
            Background = background ?? throw new ArgumentNullException(nameof(background));
            Fontground = fontground ?? throw new ArgumentNullException(nameof(fontground));

            if (Fontground.GetLength(0) > Background.GetLength(0))
                throw new ArgumentException($"Fontground.GetLengthLength(0) : {Fontground.GetLength(0)} is greater than Background.GetLengthLength(0) : {Background.GetLength(0)}");
            if (Fontground.GetLength(1) > Background.GetLength(1))
                throw new ArgumentException($"Fontground.GetLengthLength(1) : {Fontground.GetLength(1)} is greater than Background.GetLengthLength(1) : {Background.GetLength(1)}");
        }

        /* func */
        public virtual Vector ExecuteAverageValue(object limitMin)
        {
            Vector perfect;
            if (limitMin is Vector vector)
                perfect = vector;
            else
                perfect = default;

            foreach (Vector2Int position in FontgroundShiftPosition())
            {
                Vector average = SpriteDelta(position.x, position.y, out _);
                if (AIsBtterThanB(average, perfect))
                {
                    Debug.Log($"{average}, {perfect}, {position}");
                    perfect = average;
                    PreferredPosition = position;
                }
            }
            PerfectValue = perfect;
            return perfect;
        }
        public virtual IEnumerable<Vector2Int> FontgroundShiftPosition()
        {
            int backWidth = Background.GetLength(0);
            int backHeight = Background.GetLength(1);
            int fontWidth = Fontground.GetLength(0);
            int fontHeight = Fontground.GetLength(1);
            for (int shiftY = 0; shiftY < backHeight - fontHeight; shiftY++)
                for (int shiftX = 0; shiftX < backWidth - fontWidth; shiftX++)
                    yield return new Vector2Int(shiftX, shiftY);
        }
        protected virtual Vector SpriteDelta(int shiftX, int shiftY, out Vector[,] delta)
        {
            int width = Fontground.GetLength(0);
            int height = Fontground.GetLength(1);
            delta = new Vector[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    Vector4 fontColor = Fontground[x, y];
                    Vector4 backColor = Background[x + shiftX, y + shiftY];
                    delta[x, y] = PixelDelta(fontColor, backColor);
                }
            return GetAverage(delta);
        }
        protected abstract Vector PixelDelta(Vector4 fontColor, Vector4 backColor);
        protected abstract Vector GetAverage(Vector[,] delta);
        protected abstract bool AIsBtterThanB(Vector A, Vector B);
    }
}