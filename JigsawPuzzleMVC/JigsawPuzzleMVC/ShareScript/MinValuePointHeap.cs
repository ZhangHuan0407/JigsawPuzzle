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
        public LinkedList<WeightedPoint> PositionList;
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
        public MinValuePointHeap(int capacity, float limitDefault)
        {
            if (capacity < 1)
                throw new ArgumentException(nameof(capacity));

            Capacity = capacity;
            Limit = limitDefault;
            PositionList = new LinkedList<WeightedPoint>();
        }

        /* func */
        /// <summary>
        /// 向当前的位置堆中添加最小值节点
        /// <para>如果超出容量，将丢弃最大值节点</para>
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="value">值</param>
        public virtual void AddMinItem(WeightedPoint weightedPoint)
        {
            if (weightedPoint is null)
                throw new ArgumentNullException(nameof(weightedPoint));

            LinkedListNode<WeightedPoint> node = PositionList.First;
            if (weightedPoint.Value >= Limit)
                return;
            else if (PositionList.Count == 0)
            {
                PositionList.AddFirst(weightedPoint);
                return;
            }
            else if (PositionList.Count >= Capacity
                && weightedPoint.Value > node.Value.Value)
                return;
            while (true)
            {
                if (weightedPoint.Value < node.Value.Value)
                {
                    if (node.Next is null)
                    {
                        PositionList.AddAfter(node, weightedPoint);
                        break;
                    }
                    else
                        node = node.Next;
                }
                else
                {
                    PositionList.AddBefore(node, weightedPoint);
                    break;
                }
            }
            if (PositionList.Count > Capacity)
                PositionList.RemoveFirst();
            Limit = weightedPoint.Value;
        }

        internal WeightedPoint[] ToArray()
        {
            WeightedPoint[] copy = new WeightedPoint[PositionList.Count];
            PositionList.CopyTo(copy, 0);
            return copy;
        }
    }
}