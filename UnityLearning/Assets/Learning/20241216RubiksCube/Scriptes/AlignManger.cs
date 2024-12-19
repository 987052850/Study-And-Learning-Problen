using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TEN.GLOBAL.ENUM;

namespace TEN.MANAGER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/19 19:59:54 
	///创建者：Michael Corleone
	///类用途：用于检测是否对其
	/// </summary>
	public class AlignManger : MonoBehaviour
	{
        private List<INSTANCE.CubeIntance> _allDetect;
        private void Awake()
        {
            _allDetect = new List<INSTANCE.CubeIntance>();
            for (int i = 0; i < transform.childCount; i++)
            {
                _allDetect.Add(transform.GetChild(i).GetComponent<INSTANCE.CubeIntance>());
                MANAGER.RubikxCubeShaderManager.Instance.SetState(transform.GetChild(i).gameObject, ERubiksCubeInstanceState.TRANSPARENT_B);
            }
        }

        public bool Detected()
        {
            foreach (var item in _allDetect)
            {
                if (item.Detect() == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
