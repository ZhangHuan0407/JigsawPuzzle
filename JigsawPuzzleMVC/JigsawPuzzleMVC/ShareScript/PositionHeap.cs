using System;
using System.Collections.Generic;

namespace JigsawPuzzle
{
    [ShareScript]
    [Serializable]
    public class PositionHeap
    {
        /* field */
        public LinkedList<(Point, float)> PositionList;
        public readonly int Capacity;

        /* inter */
        public float Min { get; private set; }

        /* ctor */
        public PositionHeap(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentException(nameof(capacity));

            Capacity = capacity;
            PositionList = new LinkedList<(Point, float)>();
        }

        /* func */
        public void AddMinItem(Point position, float value)
        {
            LinkedListNode<(Point, float)> node = PositionList.First;
            if (PositionList.Count >= Capacity
                && value > node.Value.Item2)
                return;
            while (true)
            {
                if (value < node.Value.Item2)
                {
                    if (node.Next is null)
                    {
                        PositionList.AddAfter(node, (position, value));
                        break;
                    }
                    else
                        node = node.Next;
                }
                else
                {
                    PositionList.AddBefore(node, (position, value));
                    break;
                }
            }
            if (PositionList.Count > Capacity)
                PositionList.RemoveFirst();
        }

        internal (Point, float)[] ToArray()
        {
            (Point, float)[] copy = new (Point, float)[PositionList.Count];
            PositionList.CopyTo(copy, 0);
            return copy;
        }
    }
}