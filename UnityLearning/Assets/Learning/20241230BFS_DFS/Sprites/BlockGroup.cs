using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.LEARNING.ALGORITHM
{
    /// <summary>
    ///项目 : TEN
    ///日期：2024/12/30 22:17:00 
    ///创建者：Michael Corleone
    ///类用途：
    /// </summary>
    public class BlockGroup : IEnumerable<Block>
    {
        /// <summary>
        /// 存放邻居
        /// </summary>
        private HashSet<Block> _blocks = new HashSet<Block>();
        public event System.Action OnAddNeighbor;
        public uint Count { get; private set; } = 0;

        public void AddNeighbor(Block pIn_Neighbor)
        {
            if (!_blocks.Contains(pIn_Neighbor))
            {
                _blocks.Add(pIn_Neighbor);
                Count++;
            }
            else
            {
                GLOBAL.Global.MDebug.Log("重复的值");
            }
            OnAddNeighbor?.Invoke();
        }

        IEnumerator<Block> IEnumerable<Block>.GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }
    }
}
