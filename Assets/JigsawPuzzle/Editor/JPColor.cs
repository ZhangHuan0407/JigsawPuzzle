namespace JigsawPuzzle
{
    /// <summary>
    /// 单个像素的颜色信息
    /// <para>与 Unity 不同，此数据为引用类型</para>
    /// </summary>
    [ShareScript]
    public class JPColor
    {
        /* const */
        public const int FileDataPreJPColor = 7;
        public const int MemoryDataPreJPColor = 28;

        /* field */
        /// <summary>
        /// 颜色浮点分量
        /// </summary>
        public float R, G, B, A, H, S, V;

        /* ctor */
        public JPColor(byte[] data, int startIndex)
        {
            R = data[startIndex++] / 255f;
            G = data[startIndex++] / 255f;
            B = data[startIndex++] / 255f;
            A = data[startIndex++] / 255f;
            H = data[startIndex++] / 255f;
            S = data[startIndex++] / 255f;
            V = data[startIndex++] / 255f;
        }

        /* inter */

        /* func */
        /// <summary>
        /// 求两个颜色的差值
        /// </summary>
        /// <returns>RGBA 视觉下的颜色差值</returns>
        public static JPVector3 RGBADelta(JPColor one, JPColor another)
        {
            float ratio = one.A * another.A;
            JPVector3 result;
            if (ratio > 0)
                result = new JPVector3(
                    (one.R - another.R) * ratio,
                    (one.G - another.G) * ratio,
                    (one.B - another.B) * ratio);
            else
                result = JPVector3.Zero;
            return result;
        }
    }
}