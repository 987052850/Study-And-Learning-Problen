using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TEN.GLOBAL.ENUM;
using UnityEngine.Events;
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
        /// <summary>
        /// block的正上方的位置。
        /// </summary>
        public Vector3 UpperCenter => transform.position + Vector3.up * 0.5f;
        public Vector2Int ProjectedXY;
        public float ViewSpaceUpperCenterZ;
        public BlockProjectedShapes ProjectedShapes;
        public List<Block> AdjBlocks = new List<Block>();

        private MeshRenderer _renderer;
        public MeshRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = GetComponent<MeshRenderer>();
                }
                return _renderer;
            }
        }

        [SerializeField] private UnityEvent<Block> _onMouseDown = new UnityEvent<Block>();
        [SerializeField] private UnityEvent<Block> _onMouseUp = new UnityEvent<Block>();
        [SerializeField] private UnityEvent<Block> _onMouseDrag = new UnityEvent<Block>();
        [SerializeField] private UnityEvent<Block> _onMouseEnter = new UnityEvent<Block>();
        [SerializeField] private UnityEvent<Block> _onMouseExit = new UnityEvent<Block>();

        public event UnityAction<Block> MouseDownEvent
        {
            add => _onMouseDown.AddListener(value);
            remove => _onMouseDown.RemoveListener(value);
        }

        public event UnityAction<Block> MouseUpEvent
        {
            add => _onMouseUp.AddListener(value);
            remove => _onMouseUp.RemoveListener(value);
        }

        public event UnityAction<Block> MouseDragEvent
        {
            add => _onMouseDrag.AddListener(value);
            remove => _onMouseDrag.RemoveListener(value);
        }

        public event UnityAction<Block> MouseEnterEvent
        {
            add => _onMouseEnter.AddListener(value);
            remove => _onMouseEnter.RemoveListener(value);
        }

        public event UnityAction<Block> MouseExitEvent
        {
            add => _onMouseExit.AddListener(value);
            remove => _onMouseExit.RemoveListener(value);
        }

        private void OnMouseDown() => _onMouseDown.Invoke(this);

        private void OnMouseUp() => _onMouseUp.Invoke(this);

        private void OnMouseDrag() => _onMouseDrag.Invoke(this);

        private void OnMouseEnter() => _onMouseEnter.Invoke(this);

        private void OnMouseExit() => _onMouseExit.Invoke(this);

        private void OnDrawGizmos()
        {
            Color color = Gizmos.color;

            if (!Application.isPlaying)
            {
                Gizmos.color = Color.grey;
            }
            else if ((ProjectedShapes & BlockProjectedShapes.Walkable) == BlockProjectedShapes.Walkable)
            {
                Gizmos.color = Color.green;
            }
            else if ((ProjectedShapes & BlockProjectedShapes.Walkable) != 0)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                return;
            }

            Gizmos.DrawSphere(UpperCenter, 0.1f);
            Gizmos.color = Color.green;
            foreach (var adj in AdjBlocks)
            {
                Gizmos.DrawLine(UpperCenter, adj.UpperCenter);
            }

            Gizmos.color = color;
        }
    }
}
