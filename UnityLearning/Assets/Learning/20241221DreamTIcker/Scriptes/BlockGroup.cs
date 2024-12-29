using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TEN.GLOBAL.ENUM;

namespace TEN.LEARNING.DREAMTICKER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/22 12:49:51 
	///创建者：Michael Corleone
	///类用途：block集合
	/// </summary>
	public class BlockGroup : IEnumerable<Block>
    {

        private readonly List<Block> _blocks = new List<Block>();
        public bool IsWalkable
        {
            get
            {
                BlockProjectedShapes shapes = BlockProjectedShapes.None;

                foreach (var b in _blocks)
                {
                    shapes |= b.ProjectedShapes;

                    if ((shapes & BlockProjectedShapes.Walkable) == BlockProjectedShapes.Walkable)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        public void AddBlock(Block block)
        {
            _blocks.Add(block);
        }

        public void ClearAdjBlocks()
        {
            foreach (var block in _blocks)
            {
                block.AdjBlocks.Clear();
            }
        }

        public void AddAdjBlocks(BlockGroup adjBlocks)
        {
            foreach (var block in _blocks)
            {
                foreach (var adjBlock in adjBlocks._blocks)
                {
                    if ((adjBlock.ProjectedShapes & BlockProjectedShapes.Walkable) != 0)
                    {
                        block.AdjBlocks.Add(adjBlock);
                    }
                }
            }
        }
        public List<Block>.Enumerator GetEnumerator()
        {
            return _blocks.GetEnumerator();
        }

        IEnumerator<Block> IEnumerable<Block>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
