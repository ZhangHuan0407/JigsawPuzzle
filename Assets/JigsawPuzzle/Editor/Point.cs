namespace JigsawPuzzle
{
    /// <summary>
    /// 二维平面整型位置信息
    /// </summary>
    [ShareScript]
    public struct Point
    {
        /* const */
        public static readonly Point Zero = new Point();

        /* field */
        /// <summary>
        /// 整型分量
        /// </summary>
        public int X, Y;

        /* ctor */
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /* operator */
        public static Point operator +(Point left, Point right) => new Point(left.X + right.X, left.Y + right.Y);
        public static Point operator *(Point point, int ratio) => new Point(point.X * ratio, point.Y * ratio);

        public override string ToString() => $"{nameof(X)}:{X},{nameof(Y)}:{Y}";
    }
}