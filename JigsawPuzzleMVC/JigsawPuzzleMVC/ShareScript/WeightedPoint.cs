using System;
using System.Collections.Generic;

namespace JigsawPuzzle
{
    [ShareScript]
    public class WeightedPoint
    {
        /* field */
        public Point Position;
        public float Value;

        /* ctor */
        public WeightedPoint(Point position, float value)
        {
            Position = position;
            Value = value;
        }

        /* func */
        public static List<WeightedPoint> GroupWeightedPoints(IEnumerable<WeightedPoint> weightedPoints, int deltaX, int deltaY)
        {
            if (deltaX < 1 || deltaY < 1)
                throw new ArgumentException($"Why less than one?\n{nameof(deltaX)} : {deltaX}, {nameof(deltaY)} : {deltaY}");

            List<WeightedPoint> buffer = new List<WeightedPoint>();
            foreach (WeightedPoint oneWeightedPoint in weightedPoints)
            {
                Point onePoint = oneWeightedPoint.Position;
                foreach (WeightedPoint containedWeightPoint in buffer)
                {
                    if (Math.Abs(onePoint.X - containedWeightPoint.Position.X) < deltaX
                        && Math.Abs(onePoint.Y - containedWeightPoint.Position.Y) < deltaY)
                    {
                        if (oneWeightedPoint.Value < containedWeightPoint.Value)
                        {
                            buffer.Remove(containedWeightPoint);
                            buffer.Add(oneWeightedPoint);
                        }
                        goto NextCircle;
                    }
                }
                buffer.Add(oneWeightedPoint);
            NextCircle:
                ;
            }
            return buffer;
        }

        /* operator */
        public override string ToString()
        {
            return $"{nameof(Position)} : {Position}, {nameof(Value)} : {Value}";
        }
    }
}