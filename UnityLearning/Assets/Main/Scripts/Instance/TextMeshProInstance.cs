using TEN.GLOBAL.STRUCT;
using UnityEngine;
using TMPro;
namespace TEN.INSTANCE
{
    /// <summary>
    ///项目 : TEN
    ///日期：2024/12/15 11:21:38 
    ///创建者：Michael Corleone
    ///类用途：
    /// </summary>
    public class TextMeshProInstance : MonoBehaviour, TEN.INTERFACE.IInit, INTERFACE.IReset
    {
        private TextMeshProUGUI _text;
        private RectTransform _rectTransform;
        public void Init(SInterface vIn_InitData)
        {
            _text = GetComponent<TextMeshProUGUI>();
            _rectTransform = GetComponent<RectTransform>();

            Reset(vIn_InitData);
        }

        public void Reset(SInterface vIn_InitData)
        {
            STextMeshProData textMehsProData = vIn_InitData as STextMeshProData;
            GLOBAL.Global.GameobjectOpreate.SetRectTransform(_rectTransform, textMehsProData.SBaseData);
            _text.color = textMehsProData.Color;
            _text.enableAutoSizing = textMehsProData.AutoSize;
            _text.fontSizeMin = textMehsProData.MinSize;
            _text.fontSizeMax = textMehsProData.MaxSize;
            _text.text = textMehsProData.Text;
        }
    }
}
