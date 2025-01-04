using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.LEARNING.ALGORITHM
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/30 22:18:23 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class Block : MonoBehaviour
	{
        private Dictionary<int, Color> keyValuePairs = new Dictionary<int, Color>()
        {
            { 0,Color.white},
            { 1,Color.red},
            { 2,Color.green},
            { 3,Color.blue},
            { 4,Color.black},
        };
        public BlockGroup Neighbors
        {
            get;
            private set;
        } = new BlockGroup();
        private void Awake()
        {
            Neighbors.OnAddNeighbor += () => {
                GetComponent<MeshRenderer>().material.color = keyValuePairs[(int)Mathf.Clamp(Neighbors.Count, 0, 4)];
            };
        }


        public Block GetMinDisBlockToGoal(Block pIn_Goal ,HashSet<Block> pIn_Ignore)
        {
            float dis = float.MaxValue;
            Block _minDisBlock = null;
            foreach (var item in Neighbors)
            {
                float curDis = Vector3.Distance(item.transform.position , pIn_Goal.transform.position);
                if (curDis < dis && !pIn_Ignore.Contains(item))
                {
                    dis = curDis;
                    _minDisBlock = item;
                    pIn_Ignore.Add(item);
                }
            }
            return _minDisBlock;
        }

    }
}
