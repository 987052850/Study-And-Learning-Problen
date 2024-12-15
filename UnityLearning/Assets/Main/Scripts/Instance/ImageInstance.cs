using TEN.GLOBAL.STRUCT;
using UnityEngine;
using UnityEngine.UI;

namespace TEN.INSTANCE
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/15 11:21:18 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class ImageInstance : MonoBehaviour, TEN.INTERFACE.IInit, INTERFACE.IReset
    {
        private Image _image;
        private RectTransform _rectTransform;
        public void Init(SInterface vIn_InitData)
        {
            _image = GetComponent<Image>();
            _rectTransform = GetComponent<RectTransform>();
            Reset(vIn_InitData);
        }

        public void Reset(SInterface vIn_InitData)
        {
            SImageData sImageData = vIn_InitData as SImageData;
            GLOBAL.Global.GameobjectOpreate.SetRectTransform(_rectTransform, vIn_InitData.SBaseData);
            if (!string.IsNullOrEmpty(sImageData.BackroundImagePath))
            {
                _image.sprite = Resources.Load<Sprite>(sImageData.BackroundImagePath);
            }
            _image.color = sImageData.Color;
        }
    }
}
