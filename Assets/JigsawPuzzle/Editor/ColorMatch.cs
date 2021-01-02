using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JigsawPuzzle
{
    [ShareScript]
    public abstract class ColorMatch<Value, AverageValue> : IEnumerable<(Point, AverageValue)> where Value : new()
    {
        /* const */
        public const int ZipCount = 500;

        /* field */
        public Point CenterPosition { get; protected set; }
        public JPColor[,] EffectSpriteColor { get; protected set; }
        public List<(Point, AverageValue)> PreferredPosition { get; protected set; }
        public ShiftPositionPropensity Propensity { get; protected set; }
        public JPColor[,] SpriteColor { get; protected set; }
        public Point[] EffectiveArea { get; protected set; }
#if DEBUG && MVC
        public Analysis.Log Log;
#endif

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
            Value[,] valueMap = new Value[spritetSize.X, spritetSize.Y];

            EffectiveArea = GetEffectiveArea();
            foreach (Point point in GetShiftPosition())
            {
                for (int index = 0; index < EffectiveArea.Length; index++)
                {
                    Point selectPoint = EffectiveArea[index];
                    JPColor spriteColor = SpriteColor[selectPoint.X, selectPoint.Y];
                    if (spriteColor.A == 0f)
                        valueMap[selectPoint.X, selectPoint.Y] = new Value();
                    else
                    {
                        JPColor effectColor = EffectSpriteColor[point.X + selectPoint.X, point.Y + selectPoint.Y];
                        valueMap[selectPoint.X, selectPoint.Y] = GetDeltaValue(effectColor, spriteColor);
                    }
                }

                if (ValueMapIsBetter(valueMap, out AverageValue averageValue))
                    PreferredPosition.Add((point, averageValue));
            }
#if DEBUG && MVC
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            foreach ((Point, AverageValue) point in PreferredPosition)
                builder.AppendLine($"{point.Item1} {point.Item2}");
            Log.WriteData(null, "ColorMatch.TryGetPreferredPosition, PreferredPosition : ", builder.ToString());
#endif
        }
        protected virtual Point[] GetEffectiveArea()
        {
            List<Point> result = new List<Point>();
            IEnumerable<Point> enumPoints;
            int count = EffectSpriteColor.Length;
            if (count < ZipCount)
                enumPoints = ShiftPosition.EnumIt(SpriteColorSize, Point.One, ShiftPositionPropensity.LineByLine).ToArray();
            else if (count < ZipCount * 4)
                enumPoints = ShiftPosition.EnumIt(SpriteColorSize, Point.One, ShiftPositionPropensity.Interval2).ToArray();
            else
                enumPoints = ShiftPosition.EnumIt(SpriteColorSize, Point.One, ShiftPositionPropensity.Interval3).ToArray();

            foreach (Point enumPoint in enumPoints)
            {
                JPColor enumPointColor = SpriteColor[enumPoint.X, enumPoint.Y];
                if (enumPointColor.A > 0)
                    result.Add(enumPoint);
            }
            return result.ToArray();
        }

        protected virtual IEnumerable<Point> GetShiftPosition() => ShiftPosition.EnumIt(EffectSpriteColorSize, SpriteColorSize, Propensity);
        protected abstract Value GetDeltaValue(JPColor effectColor, JPColor spriteColor);
        protected abstract bool ValueMapIsBetter(Value[,] valueMap, out AverageValue averageValue);

        public virtual void TryGetNearlyPreferredPosition(Point position, int distance)
        {
            Check();
            PreferredPosition.Clear();
            Point spriteSize = SpriteColorSize;
            IEnumerable<Point> points = ShiftPosition.EnumItNearly(EffectSpriteColorSize, spriteSize, position, distance);
            EffectiveArea = GetEffectiveArea();

            foreach (Point point in points)
            {
                Value[,] valueMap = new Value[spriteSize.X, spriteSize.Y];
                foreach (Point selectPoint in EffectiveArea)
                {
                    JPColor effectColor = EffectSpriteColor[point.X + selectPoint.X, point.Y + selectPoint.Y];
                    JPColor spriteColor = SpriteColor[selectPoint.X, selectPoint.Y];
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