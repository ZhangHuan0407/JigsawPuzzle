using System;

namespace JigsawPuzzle
{
    /// <summary>
    /// 枚举坐标倾向性 选择不同的倾向性以使用不同的枚举策略
    /// </summary>
    [ShareScript]
    [Flags]
    public enum ShiftPositionPropensity
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
}