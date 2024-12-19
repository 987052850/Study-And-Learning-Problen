using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TEN.GLOBAL.ENUM;

namespace TEN.INSTANCE
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/19 19:17:24 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class CubeIntance : MonoBehaviour
	{
        public ERubiksCubeInstanceState MState;
        BoxCollider _collider;
        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
        }
        public bool Detect()
        {
            if (MState == ERubiksCubeInstanceState.NORMAL)
            {
                return true;
            }
            Collider[] colliders = Physics.OverlapBox(transform.TransformPoint(_collider.center), Vector3.Scale(_collider.size, transform.lossyScale) / 2 , Quaternion.identity , 1 << 6);
            foreach (var item in colliders)
            {
                if (item.GetComponent<CubeIntance>().MState != MState)
                {
                    return false;
                }
            }
            return true;
        }

        //void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.red;
        //    //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        //        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        //        Gizmos.DrawWireCube(transform.TransformPoint(_collider.center), Vector3.Scale(_collider.size,transform.lossyScale) / 2);
        //}
    }
}
