using System.Collections;
using System.Collections.Generic;
using TEN.GLOBAL.STRUCT;
using UnityEngine;

namespace TEN
{
    /// <summary>
    ///项目 : TEN
    ///日期：2024/12/14 21:44:56 
    ///创建者：Michael Corleone
    ///类用途：
    /// </summary>
    public class SliderInstance : MonoBehaviour, TEN.INTERFACE.IInit, INTERFACE.IReset
    {
        private RectTransform _rectTransform;
        private UnityEngine.UI.Slider _slider;
        private SSliderData _sliderData;
        private float _step;
        private float _min;
        private GameObject _targetGameobject;
        private Material _targetMaterial;
        public void Init(SInterface vIn_InitData)
        {
            _rectTransform = gameObject.GetComponent<RectTransform>();
            _slider = gameObject.GetComponent<UnityEngine.UI.Slider>();
            Reset(vIn_InitData);
        }

        public void Reset(SInterface vIn_InitData)
        {
            _slider.onValueChanged.RemoveAllListeners();
            _sliderData = vIn_InitData as SSliderData;
            _step = _sliderData.MapRange.y - _sliderData.MapRange.x;
            _min = _sliderData.MapRange.x;
            GLOBAL.Global.GameobjectOpreate.SetRectTransform(_rectTransform, _sliderData.SBaseData);
            //_slider.onValueChanged.AddListener(TEN.EVENTS.SliderEvent.Instance.GetEvent(_sliderData.MapType, _sliderData.ObjectName, _sliderData.AttributeName));
            switch (_sliderData.MapType)
            {
                case GLOBAL.ENUM.ESliderMapType.SHADER_IMAGE:
                    {
                        _targetGameobject = GameObject.Find(_sliderData.ObjectName);
                        _targetMaterial = _targetGameobject.GetComponent<UnityEngine.UI.Image>().material;
                        _slider.onValueChanged.AddListener((float vIn_SliderValue)=> 
                        {
                            float mapValue = ValueMap(vIn_SliderValue);
                            _targetMaterial.SetFloat(_sliderData.AttributeName, mapValue);
                        });
                    }
                    break;
                case GLOBAL.ENUM.ESliderMapType.SHADER:
                    {
                        _targetGameobject = GameObject.Find(_sliderData.ObjectName);
                        Debug.Log($"{_targetGameobject == null}");
                        _targetMaterial = _targetGameobject.GetComponent<MeshRenderer>().sharedMaterial;
                        _slider.onValueChanged.AddListener((float vIn_SliderValue) =>
                        {
                            float mapValue = ValueMap(vIn_SliderValue);
                            _targetMaterial.SetFloat(_sliderData.AttributeName, mapValue);
                        });
                    }
                    break;
                default:
                    break;
            }

        }
        float ValueMap(float vIn_SliderValue)
        {
            return vIn_SliderValue * _step + _min;
        }
    }
}
