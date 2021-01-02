using System;

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

        /* operator */
        public override string ToString()
        {
            return $"{nameof(Position)} : {Position}, {nameof(Value)} : {Value}";
        }
    }
}