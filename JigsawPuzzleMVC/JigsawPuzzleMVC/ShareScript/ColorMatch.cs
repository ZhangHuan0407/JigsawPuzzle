﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace JigsawPuzzle
{
    [ShareScript]
    public abstract class ColorMatch<Value, AverageValue> : IEnumerable<(Point, AverageValue)>
    {
        /* field */
        public Point CenterPosition { get; protected set; }
        public JPColor[,] EffectSpriteColor { get; protected set; }
        public List<(Point, AverageValue)> PreferredPosition { get; protected set; }
        public ShiftPositionPropensity Propensity { get; protected set; }
        public JPColor[,] SpriteColor { get; protected set; }

        /* inter */
        public Point EffectSpriteColorSize => new Point(EffectSpriteColor.GetLength(0), EffectSpriteColor.GetLength(1));
        public Point SpriteColorSize => new Point(SpriteColor.GetLength(0), SpriteColor.GetLength(1));

        /* ctor */
        protected ColorMatch()
        {
            PreferredPosition = new List<(Point, AverageValue)>();
        }

        /* func */
        protected virtual void Check()
        {
            if (EffectSpriteColor is null)
                throw new ArgumentNullException(nameof(EffectSpriteColor));
            if (SpriteColor is null)
                throw new ArgumentNullException(nameof(SpriteColor));
            else if (SpriteColor.GetLength(0) > EffectSpriteColor.GetLength(0)
                || SpriteColor.GetLength(1) > EffectSpriteColor.GetLength(1))
                throw new ArgumentException($"{nameof(EffectSpriteColorSize)} : {EffectSpriteColorSize}, {nameof(SpriteColorSize)} : {SpriteColorSize}");
        }

        public virtual void TryGetPreferredPosition()
        {
            Check();
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
        protected virtual IEnumerable<Point> GetShiftPosition() => ShiftPosition.EnumIt(EffectSpriteColorSize, SpriteColorSize, Propensity);
        protected abstract Value GetDeltaValue(JPColor effectColor, JPColor spriteColor);
        protected abstract bool ValueMapIsBetter(Value[,] valueMap, out AverageValue averageValue);

        public virtual void TryGetNearlyPreferredPosition(Point position, int distance)
        {
            Check();
            PreferredPosition.Clear();
            Point spritetSize = SpriteColorSize;
            IEnumerable<Point> points = ShiftPosition.EnumItNearly(EffectSpriteColorSize, spritetSize, position, distance);
            foreach (Point point in points)
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

        public abstract (Point, AverageValue) BestOne();

        /* IEnumerable */
        public IEnumerator<(Point, AverageValue)> GetEnumerator() => PreferredPosition.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}