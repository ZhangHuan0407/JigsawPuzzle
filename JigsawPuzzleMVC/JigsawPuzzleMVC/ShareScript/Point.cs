namespace JigsawPuzzle
{
    [ShareScript]
    public struct Point
    {
        /* field */
        public int X;
        public int Y;

        /* ctor */
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /* operator */
        public static Point operator +(Point left, Point right) => new Point(left.X + right.X, left.Y + right.Y);
        public static Point operator *(Point point, int ratio) => new Point(point.X * ratio, point.Y * ratio);
    }
}