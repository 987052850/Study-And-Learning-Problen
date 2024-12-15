using System.Collections;
using System.Collections.Generic;
using TEN.GLOBAL.STRUCT;
using UnityEngine;
using UnityEngine.UI;
using TEN.EVENTS;

namespace TEN.INSTANCE
{
    /// <summary>
    /// 项目 : TEN
    ///日期：2024/12/11 21:41:46 
    ///创建者：Michael Corleone
    ///类用途：按钮实例类，相当与对Button的封装
    /// </summary>
    public class ButtonInstance : MonoBehaviour , TEN.INTERFACE.IInit , INTERFACE.IReset
    {
        private TMPro.TextMeshProUGUI _text;
        private Button _button;
        private Image _image;
        private RectTransform _rectTransform;

        public void Init(SInterface vIn_InitData)
        {
            _text = transform.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            _button = transform.GetComponent<Button>();
            _image = transform.GetComponent<Image>();
            _rectTransform = _button.GetComponent<RectTransform>();

            Reset(vIn_InitData);
        }

        public void Reset(SInterface vIn_InitData)
        {
            _button.onClick.RemoveAllListeners();
            SButtonData sButtonData = vIn_InitData as SButtonData;
            _text.text = sButtonData.Name;
            GLOBAL.Global.GameobjectOpreate.SetRectTransform(_rectTransform, sButtonData.SBaseData);
            _button.onClick.AddListener(TEN.EVENTS.ButtonEvent.Instance.GetEvent(sButtonData.EventName, sButtonData.EventParameter));
        }

        private void Awake()
        {

        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
