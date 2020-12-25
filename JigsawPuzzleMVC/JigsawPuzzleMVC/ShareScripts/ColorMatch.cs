using System;
using System.Collections.Generic;

namespace JigsawPuzzle
{
    [ShareScripts]
    public abstract class ColorMatch<Value> where Value : new()
    {
        /* field */
        public JPColor[,] Background { get; protected set; }
        public JPColor[,] Fontground { get; protected set; }
        public List<(Point, Value)> PreferredList { get; protected set; }

        public ShiftPositionPropensity Propensity { get; protected set; }

        /* inter */
        public bool Check
        {
            get
            {
                if (Background is null)
                    return false;
                else if (Fontground is null)
                    return false;
                else if (Fontground.GetLength(0) > Background.GetLength(0))
                    return false;
                else if (Fontground.GetLength(1) > Background.GetLength(1))
                    return false;
                return true;
            }
        }

        public Point BackgroundSize => new Point(Background.GetLength(0), Background.GetLength(1));
        public Point FontgroundSize => new Point(Fontground.GetLength(0), Fontground.GetLength(1));

        /* ctor */
        protected ColorMatch()
        {
            PreferredList = new List<(Point, Value)>();
        }

        /* func */
        public virtual IEnumerable<Point> GetShiftPosition()
        {
            if (!Check)
                throw new ArgumentException("Not pass check");
            return ShiftPosition.EnumIt(FontgroundSize, BackgroundSize, Propensity);
        }
    }
}