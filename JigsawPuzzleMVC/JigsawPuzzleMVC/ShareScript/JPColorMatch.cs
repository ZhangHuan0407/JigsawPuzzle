﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace JigsawPuzzle
{
    [ShareScript]
    public abstract class JPColorMatch<Value, AverageValue> : IEnumerable<WeightedPoint> where Value : new()
    {
        /* const */
        public const int ZipCount = 500;

        /* field */
        public Point CenterPosition { get; protected set; }
        public JPColor[,] EffectSpriteColor { get; protected set; }
        public List<WeightedPoint> PreferredPosition { get; protected set; }
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
        protected JPColorMatch()
        {
            PreferredPosition = new List<WeightedPoint>();
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
                    JPColor effectColor = EffectSpriteColor[point.X + selectPoint.X, point.Y + selectPoint.Y];
                    valueMap[selectPoint.X, selectPoint.Y] = GetDeltaValue(effectColor, spriteColor);
                }
                if (ValueMapIsBetter(valueMap, out float averageValue))
                    PreferredPosition.Add(new WeightedPoint(point, averageValue));
            }
#if DEBUG && MVC && !PARALLELMODE
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            foreach (WeightedPoint weightedPoint in PreferredPosition)
                builder.AppendLine($"{nameof(WeightedPoint)} : {weightedPoint}");
            Log.WriteData(null, $"ColorMatch.TryGetPreferredPosition, PreferredPosition : {PreferredPosition.Count}", builder.ToString());
#endif
        }
        protected virtual Point[] GetEffectiveArea()
        {
            List<Point> result = new List<Point>();
            IEnumerable<Point> enumPoints;
            int count = EffectSpriteColor.Length;
            if (count < ZipCount)
                enumPoints = ShiftPosition.EnumIt(SpriteColorSize, Point.One, ShiftPositionPropensity.LineByLine);
            else if (count < ZipCount * 3)
                enumPoints = ShiftPosition.EnumIt(SpriteColorSize, Point.One, ShiftPositionPropensity.Random512);
            else if (count < ZipCount * 6)
                enumPoints = ShiftPosition.EnumIt(SpriteColorSize, Point.One, ShiftPositionPropensity.Interval2);
            else
                enumPoints = ShiftPosition.EnumIt(SpriteColorSize, Point.One, ShiftPositionPropensity.Interval3);

            foreach (Point enumPoint in enumPoints)
            {
                JPColor enumPointColor = SpriteColor[enumPoint.X, enumPoint.Y];
                if (enumPointColor.A > 0.25)
                    result.Add(enumPoint);
            }
            return result.ToArray();
        }

        protected virtual IEnumerable<Point> GetShiftPosition() => ShiftPosition.EnumIt(EffectSpriteColorSize, SpriteColorSize, Propensity);
        protected abstract Value GetDeltaValue(JPColor effectColor, JPColor spriteColor);
        protected abstract bool ValueMapIsBetter(Value[,] valueMap, out float averageDeltaValue);

        public virtual void TryGetNearlyPreferredPosition(int distance)
        {
            Check();
            Point spriteSize = SpriteColorSize;
            HashSet<Point> points = new HashSet<Point>();
            foreach (WeightedPoint weightedPoint in PreferredPosition)
                foreach (Point point in ShiftPosition.EnumItNearly(EffectSpriteColorSize, spriteSize, weightedPoint.Position, distance))
                    points.Add(point);
            PreferredPosition.Clear();
            EffectiveArea = GetEffectiveArea();
            Value[,] valueMap = new Value[spriteSize.X, spriteSize.Y];

            foreach (Point point in points)
            {
                foreach (Point selectPoint in EffectiveArea)
                {
                    JPColor effectColor = EffectSpriteColor[point.X + selectPoint.X, point.Y + selectPoint.Y];
                    JPColor spriteColor = SpriteColor[selectPoint.X, selectPoint.Y];
                    valueMap[selectPoint.X, selectPoint.Y] = GetDeltaValue(effectColor, spriteColor);
                }

                if (ValueMapIsBetter(valueMap, out float averageValue))
                    PreferredPosition.Add(new WeightedPoint(point, averageValue));
            }
        }

        /* IEnumerable */
        public IEnumerator<WeightedPoint> GetEnumerator() => PreferredPosition.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}