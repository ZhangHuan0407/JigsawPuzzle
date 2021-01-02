using System;
using System.Collections.Generic;

namespace JigsawPuzzle
{
    /// <summary>
    /// 最小值位置堆
    /// <para>向位置堆中加入超出容量的位置信息时，仅存储最有利的位置信息</para>
    /// </summary>
    [ShareScript]
    [Serializable]
    public class MinValuePointHeap
    {
        /* field */
        public LinkedList<(Point, float)> PositionList;
        /// <summary>
        /// 位置堆的存储容量
        /// </summary>
        public readonly int Capacity;

        /* inter */
        /// <summary>
        /// 最小值阈值
        /// <para>大于此值的任何添加项都不会进入堆中</para>
        /// </summary>
        public float Limit { get; private set; }

        /* ctor */
        public MinValuePointHeap(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentException(nameof(capacity));

            Capacity = capacity;
            PositionList = new LinkedList<(Point, float)>();
        }

        /* func */
        /// <summary>
        /// 向当前的位置堆中添加最小值节点
        /// <para>如果超出容量，将丢弃最大值节点</para>
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="value">值</param>
        public virtual void AddMinItem(Point position, float value)
        {
            LinkedListNode<(Point, float)> node = PositionList.First;
            if (value > Limit)
                return;
            else if (PositionList.Count == 0)
            {
                PositionList.AddFirst((position, value));
                return;
            }
            else if (PositionList.Count >= Capacity
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
        internal Point[] GetPoints()
        {
            Point[] copy = new Point[PositionList.Count];
            int index = 0;
            foreach ((Point, float) position in PositionList)
                copy[index++] = position.Item1;
            return copy;
        }
    }
}