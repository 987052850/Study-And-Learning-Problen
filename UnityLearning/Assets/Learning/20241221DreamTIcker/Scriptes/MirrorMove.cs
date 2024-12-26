using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.LEARNING.DREAMTICKER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/21 19:22:26 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
    [RequireComponent(typeof(MeshRenderer))]
	public class MirrorMove : MonoBehaviour
	{
        public MirrorPlane Plane;
        public float MoveSpeed = 0.05f;
        [Range(0, 1)]
        public float HighlightMix = 0.2f;

        private Vector2? _mousePos = null;
        private MeshRenderer _renderer;
        private readonly int _highlightMixPropID = Shader.PropertyToID("_HighlightMix");

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        private void OnMouseEnter()
        {
            _renderer.material.SetFloat(_highlightMixPropID, HighlightMix);
        }

        private void OnMouseExit()
        {
            _renderer.material.SetFloat(_highlightMixPropID, 0);
        }

        private void OnMouseDown()
        {
            if (!BlockManager.Instance.CanInteract)
            {
                return;
            }
            _mousePos = Input.mousePosition;
            BlockManager.Instance.DisableInteract();
        }
        private void OnMouseDrag()
        {
            if (_mousePos == null)
            {
                return;
            }
            float moveDis = Input.mousePosition.x - _mousePos.Value.x;
            float move = MoveSpeed * System.Math.Sign(moveDis);
            Vector3 pos = Plane.transform.localPosition;
            pos.x = Mathf.Clamp(pos.x + move, Plane.MoveMinX, Plane.MoveMaxX);
            Plane.transform.localPosition = pos;
            _mousePos = Input.mousePosition;
        }
        private void OnMouseUp()
        {
            if (_mousePos == null)
            {
                return;
            }
            Vector3 pos = Plane.transform.position;
            pos.x = Mathf.Clamp(Mathf.RoundToInt(pos.x + 0.5f) - 0.5f, Plane.MoveMinX, Plane.MoveMaxX);
            Plane.transform.localPosition = pos;

            _mousePos = null;
            BlockManager.Instance.EnableInteract();
            BlockManager.Instance.RebuildBlockGraph();
        }

    }
}
