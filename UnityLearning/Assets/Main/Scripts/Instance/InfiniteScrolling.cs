using System.Collections;
using System.Collections.Generic;
using TEN.GLOBAL.STRUCT;
using UnityEngine;
using UnityEngine.UI;

namespace TEN.INSTANCE
{
    /// <summary>
    ///项目 : TEN
    ///日期：2024/12/14 8:46:43 
    ///创建者：Michael Corleone
    ///类用途：无限滚动实例
    /// </summary>
    public class InfiniteScrolling : MonoBehaviour, TEN.INTERFACE.IInit
    {
        public int Space { get; set; } = 20;

        private Transform _content;
        private RectTransform _contentRect;
        private RectTransform _scrollView;
        private List<SButtonData> _dataOfContentChildren;
        private MANAGER.ObjectPool<RectTransform , SButtonData> _objectPool;
        //页面中的最多可显示的对象数量 + 3，一个用于动态调整位置，两个用于判断是否要动态调整第三个的位置
        private int _maxCountent;
        private Vector2 _contentChildSize;
        /// <summary>
        /// x 阙值位置；
        /// y 本次操作要移动的conten的子物体的节点；
        /// z y 对应的buttondata index；
        /// </summary>
        private Vector3 _upThreshold;
        private Vector3 _downThreshold;
        private ScrollRect _scrollRect;
        private Vector2 _preContentPos = Vector2.one;
        private Vector2 _curContentPos = Vector2.one;
        /// <summary>
        /// 注：这里初始化并没有完成，content还没有初始化。需要调用SetContent完成全部内容的初始化
        /// </summary>
        /// <param name="vIn_InitData"></param>
        public void Init(SInterface vIn_InitData)
        {
            _content = transform.Find("Viewport/Content");
            _contentRect = _content.gameObject.GetComponent<RectTransform>();
            _scrollView = GetComponent<RectTransform>();
            _dataOfContentChildren = new List<SButtonData>();
            _objectPool = new MANAGER.ObjectPool<RectTransform , SButtonData>();

            SScrollViewData sScrollViewData = (SScrollViewData)vIn_InitData;
            GLOBAL.Global.GameobjectOpreate.SetRectTransform(_scrollView, sScrollViewData.SBaseData);
            //_contentRect.sizeDelta = sScrollViewData.ContentSize;
            _scrollRect = _scrollView.GetComponent<ScrollRect>();
            _scrollRect.horizontal = sScrollViewData.Horizontal;
            _scrollRect.vertical = sScrollViewData.Vertical;
            _contentChildSize = sScrollViewData.ContentSize;
            _maxCountent = (int)System.Math.Ceiling((sScrollViewData.SBaseData.Size.y - _contentChildSize.y) / (_contentChildSize.y + Space));
            _maxCountent += 3;
            Debug.Log(_maxCountent);
        }
        public Transform GetContent()
        {
            return _content;
        }
        public void SetContent(List<SButtonData> vIn_List)
        {
            _scrollRect.onValueChanged.RemoveAllListeners();
            bool rightMove = vIn_List.Count > _maxCountent - 3;
            if (vIn_List.Count > _maxCountent)
            {
                _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            }
            RestContent();
            SetContentArea(vIn_List.Count, rightMove);
            _objectPool.OnCreate = TEN.MANAGER.InstantiateManager.Instance.InstantiateButton;
            int i = 0;
            foreach (var item in vIn_List)
            {
                item.SBaseData.Size = _contentChildSize;
                item.SBaseData.Pos = GetChildPos(i++);
                item.SBaseData.Achor = new Vector2(0.5f, 1f);
            }
            _objectPool.Init(_maxCountent , vIn_List);
            InitThreshold();
            _dataOfContentChildren = vIn_List;
        }
        private void RestContent()
        {
            _objectPool.Reset();
        }
        private void SetContentArea(int vIn_ContentChildrenCount , bool vIn_RightMove)
        {
            float offsetX = vIn_RightMove ? 15 : 0;
            if (vIn_ContentChildrenCount <= 0)
            {
                _contentRect.sizeDelta = new Vector2(offsetX, 0);
            }
            else
            {
                float height = 0;
                height = _contentChildSize.y + (vIn_ContentChildrenCount - 1) * (_contentChildSize.y + Space);
                _contentRect.sizeDelta = new Vector2(offsetX, height);
            }
        }
        private Vector2 GetChildPos(int vIn_Index)
        {
            if (vIn_Index<=0)
            {
                return new Vector2(0,-_contentChildSize.y/2);
            }
            float height = -(vIn_Index * (_contentChildSize.y + Space) + _contentChildSize.y/2);
            return new Vector2(0,height);
        }
        private void OnScrollValueChanged(Vector2 normalizedPos)
        {
            if (normalizedPos.y<=0 || normalizedPos.y >=1)
            {
                return;
            }
            _curContentPos = _contentRect.anchoredPosition;
            if (_curContentPos.y > _preContentPos.y)
            {
                DragUp();
            }
            else if (_curContentPos.y < _preContentPos.y)
            {
                DragDown();
            }

            _preContentPos = _curContentPos;
        }
        private void DragUp()
        {
            if (_curContentPos.y >= _upThreshold.x)
            {
                UpdateThreshold();
            }
        }
        private void DragDown()
        {
            if (_curContentPos.y + _scrollView.sizeDelta.y <= _downThreshold.x)
            {
                UpdateThreshold(false);
            }
        }
        private void InitThreshold()
        {
            _upThreshold = new Vector3(-GetChildPos(2).y - _contentChildSize.y / 2, 0 , _maxCountent);
            _downThreshold = new Vector3(-GetChildPos(_maxCountent - 3 - 1).y + _contentChildSize.y / 2, _maxCountent - 1 , -1);
            Debug.Log($"init data {_upThreshold}{_downThreshold}");
        }
        private int GetChildIndex(int vIn_CurIndex , int vIn_Max , bool vIn_IsNext = true)
        {
            int temp = vIn_IsNext ? 1 : -1;
            vIn_CurIndex += temp;

            if (vIn_CurIndex >= vIn_Max)
            {
                vIn_CurIndex = 0;
            } else if (vIn_CurIndex < 0)
            {
                vIn_CurIndex = vIn_Max - 1;
            }

            return vIn_CurIndex;
        }
        /// <summary>
        /// TODO 猪脑过载，先写死，后续再考虑如何优化
        /// </summary>
        /// <param name="vIn_DragUp"></param>
        private void UpdateThreshold(bool vIn_DragUp = true)
        {
            if (vIn_DragUp)
            {
                if ((int)_upThreshold.z >= _dataOfContentChildren.Count)
                {
                    return;
                }
                ButtonInstance buttonInstance = _content.GetChild((int)_upThreshold.y).GetComponent<ButtonInstance>();
                buttonInstance.Reset(_dataOfContentChildren[(int)_upThreshold.z]);

                _upThreshold.x += (_contentChildSize.y + Space);
                _upThreshold.y = GetChildIndex((int)_upThreshold.y, _maxCountent, true);
                _upThreshold.z = GetChildIndex((int)_upThreshold.z, _dataOfContentChildren.Count + 1, true);
                _downThreshold.x += (_contentChildSize.y + Space);
                _downThreshold.y = GetChildIndex((int)_downThreshold.y, _maxCountent, true);
                _downThreshold.z = GetChildIndex((int)_downThreshold.z, _dataOfContentChildren.Count, true);
            }
            else
            {
                ButtonInstance buttonInstance = _content.GetChild((int)_downThreshold.y).GetComponent<ButtonInstance>();
                buttonInstance.Reset(_dataOfContentChildren[(int)_downThreshold.z]);
                _downThreshold.x -= (_contentChildSize.y + Space);
                _downThreshold.y = GetChildIndex((int)_downThreshold.y, _maxCountent, false);
                _downThreshold.z = GetChildIndex((int)_downThreshold.z, _dataOfContentChildren.Count, false);
                _upThreshold.x -= (_contentChildSize.y + Space);
                _upThreshold.y = GetChildIndex((int)_upThreshold.y, _maxCountent, false);
                _upThreshold.z = GetChildIndex((int)_upThreshold.z, _dataOfContentChildren.Count, false);
            }
        }
    }
}
