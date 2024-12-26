using UnityEngine;
using UnityEngine.Serialization;

namespace TEN.LEARNING.DREAMTICKER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/21 19:11:14 
	///创建者：Michael Corleone
	///类用途：用于设置镜子大小
	/// </summary>
	public class MirrorPlane : MonoBehaviour
	{
        [Header("Move")]
        [FormerlySerializedAs("MinX")] public float MoveMinX = 3;
        [FormerlySerializedAs("MaxX")] public float MoveMaxX = 7;
        [Header("Shape")]
        [Min(1)] public int Width = 7;
        [Min(1)] public int Height = 12;

        private void OnValidate()
        {
            transform.localScale = new Vector3(Width * 0.1f, Height * 0.1f, 1);
        }
        public Vector3 PlaneMaxPosition => transform.position;
    }
}
