using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.DATASTRUCTURE
{
	/// <summary>
	///项目 : TEN
	///日期：2025/1/6 21:54:20 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class BinaryPile<T>
        where T : IComparable
	{
        private Dictionary<int, T> _allNodes = new Dictionary<int, T>();
        private int _length = 0;
        /// <summary>
        /// true 代表为最小二叉堆，即大爹的值最小
        /// </summary>
        private bool _isDescending;
        private int _sign;
        public int Count { get { return _length; } }


        public BinaryPile(bool vIn_IsDescending)
        {
            _isDescending = vIn_IsDescending;
            _sign = _isDescending ? -1 : 1;
        }

        public void Clear()
        {
            _length = 0;
        }

        public void Push(T vIn_Element)
        {
            _allNodes[_length] = vIn_Element;
            BubbleUp(_length);
            _length++;
        }
        public T Pop()
        {
            if (_length <= 0)
            {
                return default(T);
            }
            T temp = _allNodes[0];
            _length--;
            if (_length <= 0)
            {
                return temp;
            }

            _allNodes[0] = _allNodes[_length];
            BubbleDown();

            return temp;
        }

        private void Swap(int vIn_IndexA, int vIn_IndexB)
        {
            T temp = _allNodes[vIn_IndexA];
            _allNodes[vIn_IndexA] = _allNodes[vIn_IndexB];
            _allNodes[vIn_IndexB] = temp;
        }

        private void BubbleUp(int vIn_StartIndex)
        {
            int parentIndex = 0;
            while (vIn_StartIndex > 0)
            {
                parentIndex = (vIn_StartIndex - 1) / 2;
                if (_sign * _allNodes[vIn_StartIndex].CompareTo(_allNodes[parentIndex]) > 0)
                {
                    Swap(vIn_StartIndex, parentIndex);
                }
                else
                {
                    break;
                }
                vIn_StartIndex = parentIndex;
            }
        }

        private void BubbleDown()
        {
            int parentIndex = 0;
            int leftIndex = (2 * parentIndex) + 1;
            int rightIndex = leftIndex + 1;

            while (leftIndex < _length)
            {
                int minIndex = ((rightIndex < _length) && (_allNodes[leftIndex].CompareTo(_allNodes[rightIndex]) > 0)) ? rightIndex : leftIndex;
                if (_sign * _allNodes[minIndex].CompareTo(_allNodes[parentIndex]) > 0)
                {
                    Swap(minIndex, parentIndex);
                }
                else
                {
                    break;
                }
                parentIndex = minIndex;
                leftIndex = (2 * parentIndex) + 1;
                rightIndex = leftIndex + 1;
            }
        }
	}

    public class BinaryPileNode<T> : IComparable
        where T : IComparable
    {
        public T Value;

        public BinaryPileNode(T vIn_Value)
        {
            Value = vIn_Value;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is BinaryPileNode<T>))
            {
                return 0;
            }
            return Value.CompareTo((obj as BinaryPileNode<T>).Value);
        }
    }
}
