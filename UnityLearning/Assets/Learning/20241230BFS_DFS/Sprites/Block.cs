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
	public class Block : MonoBehaviour, System.IComparable
    {
        /// <summary>
        /// 总代价
        /// </summary>
        [Header("A Stat")]
        public float F = 0;
        /// <summary>
        /// 当前block到起点的代价
        /// </summary>
        public float G = 0;
        /// <summary>
        /// 当前block到终点的代价
        /// </summary>
        public float H = 0;

        private TMPro.TextMeshProUGUI _text;

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
            Transform textParent = GameObject.Find("AS Canvas").transform;
            _text = Instantiate((Resources.Load("Prefab/UI/ASText") as GameObject) , textParent).GetComponent<TMPro.TextMeshProUGUI>();
            _text.text = "";
            int x = (int)transform.position.x;
            int y = (int)transform.position.z;
            int uiX = -450 + (x * 90);
            int uiY = -450 + (y * 90);
            _text.rectTransform.anchoredPosition = new Vector2(uiX , uiY);

        }

        public void RestText()
        {
            _text.text = $"G = {G.ToString("0.0")}\nH = {H.ToString("0.0")}\nF = {F.ToString("0.0")}";
        }
        public void InitText()
        {
            _text.text = "";
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

        public float GetMannhattanDis(Block pIn_Other)
        {
            return Mathf.Abs(transform.position.x - pIn_Other.transform.position.x) +
                          Mathf.Abs(transform.position.z - pIn_Other.transform.position.z);
        }

        public float GetEuclideanDis(Block pIn_Other)
        {
            return Vector3.Distance(transform.position , pIn_Other.transform.position);
        }

        public int CompareTo(object obj)
        {
            if (!(obj is Block))
            {
                return 0;
            }
            return F.CompareTo((obj as Block).F);
        }
    }
}
