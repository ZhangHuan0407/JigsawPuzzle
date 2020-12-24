using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace BugnityHelper.JigsawPuzzle.UnitTest
{
    public class ShiftPosition_UnitTest
    {
        public static List<(Vector2Int, Vector2Int)> Size = new List<(Vector2Int, Vector2Int)>()
        {
            (new Vector2Int(5, 5), new Vector2Int(8, 8)),
            (new Vector2Int(5, 8), new Vector2Int(8, 8)),
            (new Vector2Int(8, 6), new Vector2Int(8, 8)),
            (new Vector2Int(7, 7), new Vector2Int(8, 8)),

            (new Vector2Int(1, 1), new Vector2Int(4, 4)),
            (new Vector2Int(1, 8), new Vector2Int(8, 8)),
            (new Vector2Int(8, 1), new Vector2Int(8, 8)),
            (new Vector2Int(8, 8), new Vector2Int(8, 8)),
        };

        [MenuItem("Unit Test/" + nameof(JigsawPuzzle) + "/" + nameof(ShiftPosition) + "/" + nameof(EnumPositionPropensity.LineByLine))]
        public static void ShiftPositionShouldLineByLine()
        {
            StringBuilder builder = new StringBuilder(200);
            foreach ((Vector2Int, Vector2Int) size in Size)
            {
                IEnumerable<Vector2Int> shiftPosition = ShiftPosition.EnumIt(size.Item1, size.Item2, EnumPositionPropensity.LineByLine);
                builder.AppendLine($"font : {size.Item1}, back : {size.Item2}");
                foreach (Vector2Int position in shiftPosition)
                    builder.AppendLine(position.ToString());
                Debug.Log(builder.ToString());
                builder.Clear();
            }
        }
        [MenuItem("Unit Test/" + nameof(JigsawPuzzle) + "/" + nameof(ShiftPosition) + "/" + nameof(EnumPositionPropensity.Tween))]
        public static void ShiftPositionShouldTween()
        {
            StringBuilder builder = new StringBuilder(200);
            foreach ((Vector2Int, Vector2Int) size in Size)
            {
                IEnumerable<Vector2Int> shiftPosition = ShiftPosition.EnumIt(size.Item1, size.Item2, EnumPositionPropensity.Tween);
                builder.AppendLine($"font : {size.Item1}, back : {size.Item2}");
                foreach (Vector2Int position in shiftPosition)
                    builder.AppendLine(position.ToString());
                Debug.Log(builder.ToString());
                builder.Clear();
            }
        }
        [MenuItem("Unit Test/" + nameof(JigsawPuzzle) + "/" + nameof(ShiftPosition) + "/" + nameof(EnumPositionPropensity.Interval4))]
        public static void ShiftPositionShouldInterval4()
        {
            StringBuilder builder = new StringBuilder(200);
            foreach ((Vector2Int, Vector2Int) size in Size)
            {
                Vector2Int fontgroundSize = size.Item1 * 10;
                Vector2Int backgroundSize = size.Item2 * 10;
                IEnumerable<Vector2Int> shiftPosition = ShiftPosition.EnumIt(fontgroundSize, backgroundSize, EnumPositionPropensity.Interval4);
                builder.AppendLine($"font : {fontgroundSize}, back : {backgroundSize}");
                foreach (Vector2Int position in shiftPosition)
                    builder.AppendLine(position.ToString());
                Debug.Log(builder.ToString());
                builder.Clear();
            }
        }
        [MenuItem("Unit Test/" + nameof(JigsawPuzzle) + "/" + nameof(ShiftPosition) + "/" + nameof(EnumPositionPropensity.Random6))]
        public static void ShiftPositionShouldRandom6()
        {
            StringBuilder builder = new StringBuilder(200);
            foreach ((Vector2Int, Vector2Int) size in Size)
            {
                Vector2Int fontgroundSize = size.Item1 * 10;
                Vector2Int backgroundSize = size.Item2 * 10;
                IEnumerable<Vector2Int> shiftPosition = ShiftPosition.EnumIt(fontgroundSize, backgroundSize, EnumPositionPropensity.Random6);
                builder.AppendLine($"font : {fontgroundSize}, back : {backgroundSize}");
                foreach (Vector2Int position in shiftPosition)
                    builder.AppendLine(position.ToString());
                Debug.Log(builder.ToString());
                builder.Clear();
            }
        }

        [MenuItem("Unit Test/" + nameof(JigsawPuzzle) + "/" + nameof(ShiftPosition) + "/" + nameof(ShiftPosition.EnumItNearly))]
        public static void ShiftPositionShouldEnumItNearly()
        {
            StringBuilder builder = new StringBuilder(200);
            Vector2Int fontgroundSize = new Vector2Int(5, 5);
            Vector2Int backgroundSize = new Vector2Int(Random.Range(5, 10), Random.Range(5, 10));
            Vector2Int randomPoint = new Vector2Int(
                Random.Range(0, backgroundSize.x - fontgroundSize.x),
                Random.Range(0, backgroundSize.y - fontgroundSize.y));
            int maxDistance = Random.Range(1, 5);
            IEnumerable<Vector2Int> shiftPosition = ShiftPosition.EnumItNearly(
                fontgroundSize, backgroundSize,
                randomPoint, maxDistance);
            builder.AppendLine($"font : {fontgroundSize}, back : {backgroundSize}, randomPoint : {randomPoint}, maxDistance : {maxDistance}");
            foreach (Vector2Int position in shiftPosition)
                builder.AppendLine(position.ToString());
            Debug.Log(builder.ToString());
            builder.Clear();
        }
    }
}