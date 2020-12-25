
namespace BugnityHelper.JigsawPuzzle
{
    public struct JPColor
    {
        /* const */
        public const int FileDataPreJPColor = 7;
        public const int MemoryDataPreJPColor = 28;

        /* field */
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
    }
}