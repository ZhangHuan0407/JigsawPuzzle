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
        /// 空标记
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// 逐行枚举偏移量
        /// </summary>
        LineByLine = 0x0001,
        /// <summary>
        /// 逐行枚举补间值
        /// </summary>
        Tween = 0x0010,
        /// <summary>
        /// 1间隔，2偏移量移量
        /// </summary>
        Interval2 = 0x0100,
        /// <summary>
        /// 2间隔，3偏移量枚举
        /// </summary>
        Interval3 = 0x0200,
        /// <summary>
        /// 16次随机枚举偏移量
        /// </summary>
        Random16 = 0x0400,
        /// <summary>
        /// 64次随机枚举偏移量
        /// </summary>
        Random256 = 0x0800,
        /// <summary>
        /// 512次随机枚举偏移量
        /// </summary>
        Random512 = 0x0F00,
    }
}