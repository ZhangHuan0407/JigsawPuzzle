using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace JigsawPuzzle.UnitTest
{
    public class ShiftPosition_UnitTest
    {
        public static List<(Point, Point)> Size = new List<(Point, Point)>()
        {
            (new Point(5, 5), new Point(8, 8)),
            (new Point(5, 8), new Point(8, 8)),
            (new Point(8, 6), new Point(8, 8)),
            (new Point(7, 7), new Point(8, 8)),

            (new Point(1, 1), new Point(4, 4)),
            (new Point(1, 8), new Point(8, 8)),
            (new Point(8, 1), new Point(8, 8)),
            (new Point(8, 8), new Point(8, 8)),
        };

        [MenuItem("Unit Test/" + nameof(JigsawPuzzle) + "/" + nameof(ShiftPosition) + "/" + nameof(ShiftPositionPropensity.LineByLine))]
        public static void ShiftPositionShouldLineByLine()
        {
            StringBuilder builder = new StringBuilder(200);
            foreach ((Point, Point) size in Size)
            {
                IEnumerable<Point> shiftPosition = ShiftPosition.EnumIt(size.Item1, size.Item2, ShiftPositionPropensity.LineByLine);
                builder.AppendLine($"font : {size.Item1}, back : {size.Item2}");
                foreach (Point position in shiftPosition)
                    builder.AppendLine(position.ToString());
                Debug.Log(builder.ToString());
                builder.Clear();
            }
        }
        [MenuItem("Unit Test/" + nameof(JigsawPuzzle) + "/" + nameof(ShiftPosition) + "/" + nameof(ShiftPositionPropensity.Tween))]
        public static void ShiftPositionShouldTween()
        {
            StringBuilder builder = new StringBuilder(200);
            foreach ((Point, Point) size in Size)
            {
                IEnumerable<Point> shiftPosition = ShiftPosition.EnumIt(size.Item1, size.Item2, ShiftPositionPropensity.Tween);
                builder.AppendLine($"font : {size.Item1}, back : {size.Item2}");
                foreach (Point position in shiftPosition)
                    builder.AppendLine(position.ToString());
                Debug.Log(builder.ToString());
                builder.Clear();
            }
        }
        [MenuItem("Unit Test/" + nameof(JigsawPuzzle) + "/" + nameof(ShiftPosition) + "/" + nameof(ShiftPositionPropensity.Interval4))]
        public static void ShiftPositionShouldInterval4()
        {
            StringBuilder builder = new StringBuilder(200);
            foreach ((Point, Point) size in Size)
            {
                Point fontgroundSize = size.Item1 * 10;
                Point backgroundSize = size.Item2 * 10;
                IEnumerable<Point> shiftPosition = ShiftPosition.EnumIt(fontgroundSize, backgroundSize, ShiftPositionPropensity.Interval4);
                builder.AppendLine($"font : {fontgroundSize}, back : {backgroundSize}");
                foreach (Point position in shiftPosition)
                    builder.AppendLine(position.ToString());
                Debug.Log(builder.ToString());
                builder.Clear();
            }
        }
        [MenuItem("Unit Test/" + nameof(JigsawPuzzle) + "/" + nameof(ShiftPosition) + "/" + nameof(ShiftPositionPropensity.Random16))]
        public static void ShiftPositionShouldRandom16()
        {
            StringBuilder builder = new StringBuilder(200);
            foreach ((Point, Point) size in Size)
            {
                Point fontgroundSize = size.Item1 * 10;
                Point backgroundSize = size.Item2 * 10;
                IEnumerable<Point> shiftPosition = ShiftPosition.EnumIt(fontgroundSize, backgroundSize, ShiftPositionPropensity.Random16);
                builder.AppendLine($"font : {fontgroundSize}, back : {backgroundSize}");
                foreach (Point position in shiftPosition)
                    builder.AppendLine(position.ToString());
                Debug.Log(builder.ToString());
                builder.Clear();
            }
        }

        [MenuItem("Unit Test/" + nameof(JigsawPuzzle) + "/" + nameof(ShiftPosition) + "/" + nameof(ShiftPosition.EnumItNearly))]
        public static void ShiftPositionShouldEnumItNearly()
        {
            StringBuilder builder = new StringBuilder(200);
            Point fontgroundSize = new Point(5, 5);
            Point backgroundSize = new Point(Random.Range(5, 10), Random.Range(5, 10));
            Point randomPoint = new Point(
                Random.Range(0, backgroundSize.X - fontgroundSize.X),
                Random.Range(0, backgroundSize.Y - fontgroundSize.Y));
            int maxDistance = Random.Range(1, 5);
            IEnumerable<Point> shiftPosition = ShiftPosition.EnumItNearly(
                fontgroundSize, backgroundSize,
                randomPoint, maxDistance);
            builder.AppendLine($"font : {fontgroundSize}, back : {backgroundSize}, randomPoint : {randomPoint}, maxDistance : {maxDistance}");
            foreach (Point position in shiftPosition)
                builder.AppendLine(position.ToString());
            Debug.Log(builder.ToString());
            builder.Clear();
        }
    }
}