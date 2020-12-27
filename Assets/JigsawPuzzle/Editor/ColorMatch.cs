using System;
using System.Collections;
using System.Collections.Generic;

namespace JigsawPuzzle
{
    [ShareScript]
    public abstract class ColorMatch<Value, AverageValue> : IEnumerable<(Point, AverageValue)>
    {
        /* field */
        public JPColor[,] EffectSpriteColor { get; protected set; }
        public List<(Point, AverageValue)> PreferredPosition { get; protected set; }
        public ShiftPositionPropensity Propensity { get; protected set; }
        public JPColor[,] SpriteColor { get; protected set; }

        /* inter */
        public bool Check
        {
            get
            {
                if (EffectSpriteColor is null)
                    return false;
                else if (SpriteColor is null)
                    return false;
                else if (SpriteColor.GetLength(0) > EffectSpriteColor.GetLength(0))
                    return false;
                else if (SpriteColor.GetLength(1) > EffectSpriteColor.GetLength(1))
                    return false;
                return true;
            }
        }

        public Point EffectSpriteColorSize => new Point(EffectSpriteColor.GetLength(0), EffectSpriteColor.GetLength(1));
        public Point SpriteColorSize => new Point(SpriteColor.GetLength(0), SpriteColor.GetLength(1));

        /* ctor */
        protected ColorMatch()
        {
            PreferredPosition = new List<(Point, AverageValue)>();
        }

        /* func */
        public virtual void TryGetPreferredPosition()
        {
            PreferredPosition.Clear();
            Point spritetSize = SpriteColorSize;
            foreach (Point point in GetShiftPosition())
            {
                Value[,] valueMap = new Value[spritetSize.X, spritetSize.Y];
                for (int y = 0; y < spritetSize.Y; y++)
                    for (int x = 0; x < spritetSize.X; x++)
                    {
                        JPColor effectColor = EffectSpriteColor[point.X + x, point.Y + y];
                        JPColor spriteColor = SpriteColor[x, y];
                        GetDeltaValue(effectColor, spriteColor);
                    }

                if (ValueMapIsBetter(valueMap, out AverageValue averageValue))
                    PreferredPosition.Add((point, averageValue));
            }
        }

        public virtual IEnumerable<Point> GetShiftPosition()
        {
            if (!Check)
                throw new ArgumentException("Not pass check");
            return ShiftPosition.EnumIt(SpriteColorSize, EffectSpriteColorSize, Propensity);
        }
        public abstract Value GetDeltaValue(JPColor effectColor, JPColor spriteColor);
        public abstract bool ValueMapIsBetter(Value[,] valueMap, out AverageValue averageValue);

        /* IEnumerable */
        public IEnumerator<(Point, AverageValue)> GetEnumerator() => PreferredPosition.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}