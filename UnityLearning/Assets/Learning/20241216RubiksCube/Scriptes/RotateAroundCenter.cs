using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.UTILS
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/17 19:37:15 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class RotateAroundCenter
	{
        private GameObject[] _allEntity;
        private Vector3 _center;
        private Vector3 _axis;

        public void Reset(GameObject[] gameObjects , Vector3 vIn_Axis)
        {
            _allEntity = gameObjects;
            Vector3 add = Vector3.zero;
            foreach (var item in _allEntity)
            {
                add += item.transform.position;
            }
            _center = add / _allEntity.Length;
            _axis = vIn_Axis;
        }

        public void Rotate(float vIn_Degree)
        {
            foreach (var item in _allEntity)
            {
                item.transform.RotateAround(_center,_axis, vIn_Degree);
            }
        }
	}
}
