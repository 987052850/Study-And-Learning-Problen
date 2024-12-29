using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.LEARNING.DREAMTICKER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/29 10:36:19 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TrailRenderer))]
	public class Trail : MonoBehaviour
	{
        public int FixedPositionY = 3;
        public float MoveTimePerBlock = 0.1f;
        private TrailRenderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<TrailRenderer>();
            _renderer.emitting = false;
        }

        public IEnumerator Move(Dictionary<Block, Block> next, Block start, Block end)
        {
            transform.position = GetConstrainedPositionAt(start);
            _renderer.Clear();
            yield return null;

            _renderer.emitting = true;

            while (start != end)
            {
                Block block = next[start];
                Vector3 startPos = GetConstrainedPositionAt(start);
                Vector3 endPos = GetConstrainedPositionAt(block);
                float time = 0;
                while (time < MoveTimePerBlock)
                {
                    time += Time.deltaTime;
                    float p = time / MoveTimePerBlock;
                    transform.position = Vector3.Lerp(startPos , endPos , p);
                    yield return null;
                }
                start = block;
            }
            yield return new WaitForSeconds(_renderer.time);
            _renderer.emitting = false;
        }

        private Vector3 GetConstrainedPositionAt(Block block)
        {
            Matrix4x4 viewMat = Camera.main.worldToCameraMatrix;
            Vector3 posVS = viewMat.MultiplyPoint(block.UpperCenter);

            //TODO 这里不懂
            Matrix4x4 matrix = new Matrix4x4(
                    viewMat.GetColumn(0),
                    viewMat.GetColumn(2),
                    new Vector4(0,0,-1,0),
                    new Vector4(0,0,0,1)
                );
            Vector4 p = matrix.inverse * new Vector4(
                    posVS.x - FixedPositionY * viewMat[0,1] - viewMat[0,3],
                    posVS.y - FixedPositionY * viewMat[1,1] - viewMat[1,3],
                    -FixedPositionY * viewMat[2,1] - viewMat[2,3],
                    0
                );
            return new Vector3(p.x, FixedPositionY, p.y);
        }

    }
}
