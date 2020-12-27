namespace JigsawPuzzle
{
    [ShareScript]
    public class JPVector3
    {
        /* field */
        public float X, Y, Z;

        /* inter */
        public float SqrMagnitude => X * X + Y * Y + Z * Z;

        /* ctor */
        public JPVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}