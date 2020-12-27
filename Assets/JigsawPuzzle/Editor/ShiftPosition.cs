using System;
using System.Collections.Generic;

namespace JigsawPuzzle
{
    [ShareScript]
    public static class ShiftPosition
    {
        /* field */
        private static Dictionary<ShiftPositionPropensity, Func<Point, Point, IEnumerable<Point>>> EnumItMethods =
            new Dictionary<ShiftPositionPropensity, Func<Point, Point, IEnumerable<Point>>>()
            {
                { ShiftPositionPropensity.LineByLine,    LineByLine },
                { ShiftPositionPropensity.Tween,         Tween },
                { ShiftPositionPropensity.Interval4,     Interval4 },
                { ShiftPositionPropensity.Interval12,    Interval12 },
                { ShiftPositionPropensity.Random6,       Random6 },
                { ShiftPositionPropensity.Random32,      Random32 },
            };

        /* func */
        /// <summary>
        /// 使用指定的枚举倾向性，基于前景后景数据枚举范围内的偏移量
        /// </summary>
        /// <param name="fontgroundSize">前景尺寸</param>
        /// <param name="backgroundSize">后景尺寸</param>
        /// <param name="propensity">枚举倾向性</param>
        /// <returns>偏移量</returns>
        public static IEnumerable<Point> EnumIt(
            Point fontgroundSize,
            Point backgroundSize,
            ShiftPositionPropensity propensity)
        {
            if (EnumItMethods.TryGetValue(propensity, out Func<Point, Point, IEnumerable<Point>> method))
                return method(fontgroundSize, backgroundSize);
            else
                throw new NotImplementedException();
        }

        /// <summary>
        /// 逐行枚举偏移量
        /// <para>先移动x, 再移动y, 包含边界值</para>
        /// </summary>
        /// <param name="fontgroundSize">前景尺寸</param>
        /// <param name="backgroundSize">后景尺寸</param>
        /// <returns>偏移量</returns>
        private static IEnumerable<Point> LineByLine(Point fontgroundSize, Point backgroundSize)
        {
            for (int shiftY = 0; shiftY <= backgroundSize.Y - fontgroundSize.Y; shiftY++)
                for (int shiftX = 0; shiftX <= backgroundSize.X - fontgroundSize.X; shiftX++)
                    yield return new Point(shiftX, shiftY);
        }
        /// <summary>
        /// 逐行枚举补间值
        /// <para>先移动x, 再移动y, 包含边界值</para>
        /// <para>首个和最后一个坐标一定是边界值, 但不保证枚举顺序</para>
        /// </summary>
        /// <param name="one">边界位置1</param>
        /// <param name="another">边界位置2</param>
        /// <returns>补间值</returns>
        private static IEnumerable<Point> Tween(Point one, Point another)
        {
            int startShiftY, endShiftY;
            if (one.Y > another.Y)
            {
                startShiftY = another.Y;
                endShiftY = one.Y;
            }
            else
            {
                startShiftY = one.Y;
                endShiftY = another.Y;
            }
            int startShiftX, endShiftX;
            if (one.X > another.X)
            {
                startShiftX = another.X;
                endShiftX = one.X;
            }
            else
            {
                startShiftX = one.X;
                endShiftX = another.X;
            }
            for (int shiftY = startShiftY; shiftY <= endShiftY; shiftY++)
                for (int shiftX = startShiftX; shiftX <= endShiftX; shiftX++)
                    yield return new Point(shiftX, shiftY);
        }
        /// <summary>
        /// 4间隔枚举偏移量
        /// <para>先移动x, 再移动y, 包含边界值</para>
        /// </summary>
        /// <param name="fontgroundSize">前景尺寸</param>
        /// <param name="backgroundSize">后景尺寸</param>
        /// <returns>偏移量</returns>
        private static IEnumerable<Point> Interval4(Point fontgroundSize, Point backgroundSize)
        {
            for (int shiftY = 0; shiftY <= backgroundSize.Y - fontgroundSize.Y; shiftY += 4)
                for (int shiftX = 0; shiftX <= backgroundSize.X - fontgroundSize.X; shiftX += 4)
                    yield return new Point(shiftX, shiftY);
        }
        /// <summary>
        /// 12间隔枚举偏移量
        /// <para>先移动x, 再移动y, 包含边界值</para>
        /// </summary>
        /// <param name="fontgroundSize">前景尺寸</param>
        /// <param name="backgroundSize">后景尺寸</param>
        /// <returns>偏移量</returns>
        private static IEnumerable<Point> Interval12(Point fontgroundSize, Point backgroundSize)
        {
            for (int shiftY = 0; shiftY <= backgroundSize.Y - fontgroundSize.Y; shiftY += 12)
                for (int shiftX = 0; shiftX <= backgroundSize.X - fontgroundSize.X; shiftX += 12)
                    yield return new Point(shiftX, shiftY);
        }
        /// <summary>
        /// 6次随机枚举偏移量
        /// <para>包含边界值</para>
        /// </summary>
        /// <param name="fontgroundSize">前景尺寸</param>
        /// <param name="backgroundSize">后景尺寸</param>
        /// <returns>偏移量</returns>
        private static IEnumerable<Point> Random6(Point fontgroundSize, Point backgroundSize)
        {
            Random random = new Random();
            int deltaX = backgroundSize.X - fontgroundSize.X + 1;
            int deltaY = backgroundSize.Y - fontgroundSize.Y + 1;
            for (int i = 0; i < 6; i++)
                yield return new Point(random.Next(0, deltaX), random.Next(0, deltaY));
        }
        /// <summary>
        /// 32次随机枚举偏移量
        /// <para>包含边界值</para>
        /// </summary>
        /// <param name="fontgroundSize">前景尺寸</param>
        /// <param name="backgroundSize">后景尺寸</param>
        /// <returns>偏移量</returns>
        private static IEnumerable<Point> Random32(Point fontgroundSize, Point backgroundSize)
        {
            Random random = new Random();
            int deltaX = backgroundSize.X - fontgroundSize.X + 1;
            int deltaY = backgroundSize.Y - fontgroundSize.Y + 1;
            for (int i = 0; i < 32; i++)
                yield return new Point(random.Next(0, deltaX), random.Next(0, deltaY));
        }

        public static IEnumerable<Point> EnumItNearly(
            Point fontgroundSize, Point backgroundSize,
            Point position, int maxDistance = 1)
        {
            int minDeltaX, maxDeltaX;
            if (position.X - maxDistance > 0)
                minDeltaX = position.X - maxDistance;
            else
                minDeltaX = 0;
            if (backgroundSize.X - fontgroundSize.X > position.X + maxDistance)
                maxDeltaX = position.X + maxDistance;
            else
                maxDeltaX = backgroundSize.X - fontgroundSize.X;

            int minDeltaY, maxDeltaY;
            if (position.X - maxDistance > 0)
                minDeltaY = position.Y - maxDistance;
            else
                minDeltaY = 0;
            if (backgroundSize.Y - fontgroundSize.Y > position.Y + maxDistance)
                maxDeltaY = position.Y + maxDistance;
            else
                maxDeltaY = backgroundSize.Y - fontgroundSize.Y;

            // 我曾经写过一次螺旋线遍历，用的是四条斜线
            // 那次经历给我留下痛苦的回忆，所以我选择更费电的写法
            for (int shiftY = minDeltaY; shiftY <= maxDeltaY; shiftY++)
                for (int shiftX = minDeltaX; shiftX <= maxDeltaX; shiftX++)
                {
                    Point targetPosition = new Point(shiftX, shiftY);
                    if (NearlyButNotEqual(targetPosition))
                        yield return targetPosition;
                }

            bool NearlyButNotEqual(Point targetPosition)
            {
                int distanceX = targetPosition.X - position.X;
                int distanceY = targetPosition.Y - position.Y;
                distanceX = distanceX > 0 ? distanceX : -distanceX;
                distanceY = distanceY > 0 ? distanceY : -distanceY;
                int distance = distanceX + distanceY;
                if (distance > maxDistance)
                    return false;
                else if (distance == 0)
                    return false;
                else
                    return true;
            }
        }
    }
}