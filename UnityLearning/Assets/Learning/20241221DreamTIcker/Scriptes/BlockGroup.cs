using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.LEARNING.DREAMTICKER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/22 12:49:51 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class BlockGroup : MonoBehaviour
	{
        private readonly List<Block> _blocks = new List<Block>();

        public void AddBlock(Block block)
        {
            _blocks.Add(block);
        }
    }
}
