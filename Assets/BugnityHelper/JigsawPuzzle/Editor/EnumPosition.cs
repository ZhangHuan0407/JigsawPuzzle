using System;
using System.Collections.Generic;
using UnityEngine;

namespace BugnityHelper.JigsawPuzzle
{
    /// <summary>
    /// 枚举坐标倾向性 选择不同的倾向性以使用不同的枚举策略
    /// </summary>
    [Flags]
    public enum EnumPositionPropensity
    {
        /// <summary>
        /// 逐行枚举偏移量
        /// </summary>
        LineByLine = 0x0001,
        /// <summary>
        /// 逐行枚举补间值
        /// </summary>
        Tween = 0x0010,
        /// <summary>
        /// 4间隔枚举偏移量
        /// </summary>
        Interval4 = 0x0100,
        /// <summary>
        /// 12间隔枚举偏移量
        /// </summary>
        Interval12 = 0x0200,
        /// <summary>
        /// 6次随机枚举偏移量
        /// </summary>
        Random6 = 0x1000,
        /// <summary>
        /// 32次随机枚举偏移量
        /// </summary>
        Random32 = 0x2000,
    }

    public static class ShiftPosition
    {
        /* field */
        private static Dictionary<EnumPositionPropensity, Func<Vector2Int, Vector2Int, IEnumerable<Vector2Int>>> EnumItMethods =
            new Dictionary<EnumPositionPropensity, Func<Vector2Int, Vector2Int, IEnumerable<Vector2Int>>>()
            {
                { EnumPositionPropensity.LineByLine,    LineByLine },
                { EnumPositionPropensity.Tween,         Tween },
                { EnumPositionPropensity.Interval4,     Interval4 },
                { EnumPositionPropensity.Interval12,    Interval12 },
                { EnumPositionPropensity.Random6,       Random6 },
                { EnumPositionPropensity.Random32,      Random32 },
            };

        /* func */
        /// <summary>
        /// 使用指定的枚举倾向性，基于前景后景数据枚举范围内的偏移量
        /// </summary>
        /// <param name="fontgroundSize">前景尺寸</param>
        /// <param name="backgroundSize">后景尺寸</param>
        /// <param name="propensity">枚举倾向性</param>
        /// <returns>偏移量</returns>
        public static IEnumerable<Vector2Int> EnumIt(
            Vector2Int fontgroundSize,
            Vector2Int backgroundSize,
            EnumPositionPropensity propensity)
        {
            if (EnumItMethods.TryGetValue(propensity, out Func<Vector2Int, Vector2Int, IEnumerable<Vector2Int>> method))
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
        private static IEnumerable<Vector2Int> LineByLine(Vector2Int fontgroundSize, Vector2Int backgroundSize)
        {
            for (int shiftY = 0; shiftY <= backgroundSize.y - fontgroundSize.y; shiftY++)
                for (int shiftX = 0; shiftX <= backgroundSize.x - fontgroundSize.x; shiftX++)
                    yield return new Vector2Int(shiftX, shiftY);
        }
        /// <summary>
        /// 逐行枚举补间值
        /// <para>先移动x, 再移动y, 包含边界值</para>
        /// <para>首个和最后一个坐标一定是边界值, 但不保证枚举顺序</para>
        /// </summary>
        /// <param name="one">边界位置1</param>
        /// <param name="another">边界位置2</param>
        /// <returns>补间值</returns>
        private static IEnumerable<Vector2Int> Tween(Vector2Int one, Vector2Int another)
        {
            int startShiftY, endShiftY;
            if (one.y > another.y)
            {
                startShiftY = another.y;
                endShiftY = one.y;
            }
            else
            {
                startShiftY = one.y;
                endShiftY = another.y;
            }
            int startShiftX, endShiftX;
            if (one.x > another.x)
            {
                startShiftX = another.x;
                endShiftX = one.x;
            }
            else
            {
                startShiftX = one.x;
                endShiftX = another.x;
            }
            for (int shiftY = startShiftY; shiftY <= endShiftY; shiftY++)
                for (int shiftX = startShiftX; shiftX <= endShiftX; shiftX++)
                    yield return new Vector2Int(shiftX, shiftY);
        }
        /// <summary>
        /// 4间隔枚举偏移量
        /// <para>先移动x, 再移动y, 包含边界值</para>
        /// </summary>
        /// <param name="fontgroundSize">前景尺寸</param>
        /// <param name="backgroundSize">后景尺寸</param>
        /// <returns>偏移量</returns>
        private static IEnumerable<Vector2Int> Interval4(Vector2Int fontgroundSize, Vector2Int backgroundSize)
        {
            for (int shiftY = 0; shiftY <= backgroundSize.y - fontgroundSize.y; shiftY += 4)
                for (int shiftX = 0; shiftX <= backgroundSize.x - fontgroundSize.x; shiftX += 4)
                    yield return new Vector2Int(shiftX, shiftY);
        }
        /// <summary>
        /// 12间隔枚举偏移量
        /// <para>先移动x, 再移动y, 包含边界值</para>
        /// </summary>
        /// <param name="fontgroundSize">前景尺寸</param>
        /// <param name="backgroundSize">后景尺寸</param>
        /// <returns>偏移量</returns>
        private static IEnumerable<Vector2Int> Interval12(Vector2Int fontgroundSize, Vector2Int backgroundSize)
        {
            for (int shiftY = 0; shiftY <= backgroundSize.y - fontgroundSize.y; shiftY += 12)
                for (int shiftX = 0; shiftX <= backgroundSize.x - fontgroundSize.x; shiftX += 12)
                    yield return new Vector2Int(shiftX, shiftY);
        }
        /// <summary>
        /// 6次随机枚举偏移量
        /// <para>包含边界值</para>
        /// </summary>
        /// <param name="fontgroundSize">前景尺寸</param>
        /// <param name="backgroundSize">后景尺寸</param>
        /// <returns>偏移量</returns>
        private static IEnumerable<Vector2Int> Random6(Vector2Int fontgroundSize, Vector2Int backgroundSize)
        {
            System.Random random = new System.Random();
            int deltaX = backgroundSize.x - fontgroundSize.x + 1;
            int deltaY = backgroundSize.y - fontgroundSize.y + 1;
            for (int i = 0; i < 6; i++)
                yield return new Vector2Int(random.Next(0, deltaX), random.Next(0, deltaY));
        }
        /// <summary>
        /// 32次随机枚举偏移量
        /// <para>包含边界值</para>
        /// </summary>
        /// <param name="fontgroundSize">前景尺寸</param>
        /// <param name="backgroundSize">后景尺寸</param>
        /// <returns>偏移量</returns>
        private static IEnumerable<Vector2Int> Random32(Vector2Int fontgroundSize, Vector2Int backgroundSize)
        {
            System.Random random = new System.Random();
            int deltaX = backgroundSize.x - fontgroundSize.x + 1;
            int deltaY = backgroundSize.y - fontgroundSize.y + 1;
            for (int i = 0; i < 32; i++)
                yield return new Vector2Int(random.Next(0, deltaX), random.Next(0, deltaY));
        }

        public static IEnumerable<Vector2Int> EnumItNearly(
            Vector2Int fontgroundSize, Vector2Int backgroundSize,
            Vector2Int position, int maxDistance = 1)
        {
            int minDeltaX, maxDeltaX;
            if (position.x - maxDistance > 0)
                minDeltaX = position.x - maxDistance;
            else
                minDeltaX = 0;
            if (backgroundSize.x - fontgroundSize.x > position.x + maxDistance)
                maxDeltaX = position.x + maxDistance;
            else
                maxDeltaX = backgroundSize.x - fontgroundSize.x;

            int minDeltaY, maxDeltaY;
            if (position.x - maxDistance > 0)
                minDeltaY = position.y - maxDistance;
            else
                minDeltaY = 0;
            if (backgroundSize.y - fontgroundSize.y > position.y + maxDistance)
                maxDeltaY = position.y + maxDistance;
            else
                maxDeltaY = backgroundSize.y - fontgroundSize.y;

            // 我曾经写过一次螺旋线遍历，用的是四条斜线
            // 那次经历给我留下痛苦的回忆，所以我选择更费电的写法
            for (int shiftY = minDeltaY; shiftY <= maxDeltaY; shiftY++)
                for (int shiftX = minDeltaX; shiftX <= maxDeltaX; shiftX++)
                {
                    Vector2Int targetPosition = new Vector2Int(shiftX, shiftY);
                    if (NearlyButNotEqual(targetPosition))
                        yield return targetPosition;
                }

            bool NearlyButNotEqual(Vector2Int targetPosition)
            {
                int distanceX = targetPosition.x - position.x;
                int distanceY = targetPosition.y - position.y;
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