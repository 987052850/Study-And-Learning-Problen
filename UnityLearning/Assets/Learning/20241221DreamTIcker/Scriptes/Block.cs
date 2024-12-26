using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TEN.GLOBAL.ENUM;

namespace TEN.LEARNING.DREAMTICKER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/22 11:12:39 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class Block : MonoBehaviour
	{
        public Vector3 UpperCenter => transform.position + Vector3.up * 0.5f;
        public Vector2Int ProjectedXY;
        public float ViewSpaceUpperCenterZ;
        public BlockProjectedShapes ProjectedShapes;
        public List<Block> AdjBlocks = new List<Block>();
    }
}
