using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.LEARNING
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/15 15:24:24 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class UpdateMousePos : MonoBehaviour
	{
        private Material _material;
        private ComputeBuffer mouseBuffer;
        private const int BufferSize = 30; // 存储最近 2 秒的数据 (假设每秒 60 帧)
        private Vector2[] mousePositions = new Vector2[BufferSize];
        private int currentIndex = 0;
        private void Awake()
        {
            _material = GetComponent<UnityEngine.UI.Image>().material;
            mouseBuffer = new ComputeBuffer(BufferSize, sizeof(float) * 2);
            _material.SetBuffer("_MousePositions", mouseBuffer);
        }

        private void Update()
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseUV = new Vector3(mousePosition.x / Screen.width, mousePosition.y / Screen.height, 0f);
            _material.SetVector("_MousePos", mouseUV);

            mousePositions[currentIndex] = mouseUV;
            currentIndex = (currentIndex + 1) % BufferSize; // 环形缓冲

            // 更新到 GPU
            mouseBuffer.SetData(mousePositions);
        }

        void OnDestroy()
        {
            // 释放缓冲区
            if (mouseBuffer != null)
            {
                mouseBuffer.Release();
            }
        }
    }
}
