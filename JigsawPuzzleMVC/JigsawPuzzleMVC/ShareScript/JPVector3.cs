namespace JigsawPuzzle
{
    /// <summary>
    /// 三维浮点数向量
    /// <para>与 Unity 不同，此向量为引用类型</para>
    /// </summary>
    [ShareScript]
    public class JPVector3
    {
        /* const */
        /// <summary>
        /// 三维原点
        /// </summary>
        public static readonly JPVector3 Zero = new JPVector3(0f, 0f, 0f);

        /* field */
        /// <summary>
        /// 三维浮点数分量
        /// </summary>
        public float X, Y, Z;

        /* inter */
        /// <summary>
        /// 三维浮点数向量的平方模量
        /// </summary>
        public float SqrMagnitude => X * X + Y * Y + Z * Z;

        /* ctor */
        /// <summary>
        /// 创建一个具有初始值的三维浮点数向量
        /// </summary>
        /// <param name="x">X 方向分量</param>
        /// <param name="y">Y 方向分量</param>
        /// <param name="z">Z 方向分量</param>
        public JPVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}