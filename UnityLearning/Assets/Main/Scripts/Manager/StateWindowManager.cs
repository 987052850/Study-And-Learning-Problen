using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace TEN.MANAGER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/15 11:24:31 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class StateWindowManager : MonoBehaviour
	{
        private TextMeshProUGUI _frameRate;
        private TextMeshProUGUI _drawCall;
        private TextMeshProUGUI _totalVertices;
        private TextMeshProUGUI _totalTriangles;
        private delegate void UpdateText(TextMeshProUGUI pIn_Text, string pIn_Value);
        private UpdateText UpdateFrameRate;
        private UpdateText UpdateDrawCall;
        private UpdateText UpdateTotalVertices;
        private UpdateText UpdateTotalTriangles;
        private float _timeSpan = 2;
        private float _curTime = 0;
        private void Awake()
        {
            if (transform.Find("FrameRate"))
            {
                _frameRate = transform.Find("FrameRate").GetComponent<TextMeshProUGUI>();
                if (_frameRate)
                {
                    UpdateFrameRate = UpdateTextF;
                }
            }
            if (transform.Find("DrallCall"))
            {
                _drawCall = transform.Find("DrallCall").GetComponent<TextMeshProUGUI>();
                if (_drawCall)
                {
                    UpdateDrawCall = UpdateTextF;
                }
            }
            if (transform.Find("TotalVertices"))
            {
                _totalVertices = transform.Find("TotalVertices").GetComponent<TextMeshProUGUI>();
                if (_totalVertices)
                {
                    UpdateTotalVertices = UpdateTextF;
                }
            }
            if (transform.Find("TotalTriangles"))
            {
                _totalTriangles = transform.Find("TotalTriangles").GetComponent<TextMeshProUGUI>();
                if (_totalTriangles)
                {
                    UpdateTotalTriangles = UpdateTextF;
                }
            }
        }

        private void Update()
        {
            _curTime += Time.deltaTime;
            if (_curTime >= _timeSpan)
            {
                _curTime = 0;
                int totalVertices = 0;
                int totalTriangles = 0;
                int drawCalls = 0;

                // 遍历所有渲染器
                foreach (MeshRenderer renderer in FindObjectsOfType<MeshRenderer>())
                {
                    MeshFilter filter = renderer.GetComponent<MeshFilter>();
                    if (filter && filter.sharedMesh)
                    {
                        Mesh mesh = filter.sharedMesh;
                        totalVertices += mesh.vertexCount;
                        totalTriangles += mesh.triangles.Length / 3; // 三角形个数
                        drawCalls++;
                    }
                }

                foreach (SkinnedMeshRenderer skinnedRenderer in FindObjectsOfType<SkinnedMeshRenderer>())
                {
                    if (skinnedRenderer.sharedMesh)
                    {
                        Mesh mesh = skinnedRenderer.sharedMesh;
                        totalVertices += mesh.vertexCount;
                        totalTriangles += mesh.triangles.Length / 3;
                        drawCalls++;
                    }
                }

                // 遍历所有 UI 渲染器
                foreach (CanvasRenderer canvasRenderer in FindObjectsOfType<CanvasRenderer>())
                {
                    if (canvasRenderer.gameObject.activeInHierarchy)
                    {
                        // CanvasRenderer 没有直接暴露顶点和三角形信息。
                        // 如果需要统计其数据，需调用 Canvas 动态生成的批次信息。
                        //totalVertices += canvasRenderer.GetMaterial().ma; // 粗略的顶点计数方式
                        drawCalls += canvasRenderer.materialCount; // 一个 CanvasRenderer 对应一个 UI 批次。
                    }
                }

                UpdateFrameRate?.Invoke(_frameRate, string.Format($"FPS: {(int)(1 / Time.deltaTime)}"));
                UpdateDrawCall?.Invoke(_drawCall , string.Format($"Draw Calls: {drawCalls}"));
                UpdateTotalVertices?.Invoke(_totalVertices, string.Format($"Vertices: {totalVertices}"));
                UpdateTotalTriangles?.Invoke(_totalTriangles, string.Format($"Triangles: {totalTriangles}"));
            }
        }
        public void SetTimeSpan(float vIn_TimeSpan)
        {
            _timeSpan = vIn_TimeSpan;
        }

        private void UpdateTextF(TextMeshProUGUI pIn_Text, string pIn_Value)
        {
            pIn_Text.text = pIn_Value;
        }
    }
}
