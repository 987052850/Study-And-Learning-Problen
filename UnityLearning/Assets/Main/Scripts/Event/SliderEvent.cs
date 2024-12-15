using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TEN.EVENTS
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/14 21:08:20 
	///创建者：Michael Corleone
	///类用途：弃用，如果要动态修改某个对象身上某个组件的值的话，首先需要查询到这个对象，
    ///但是对象查找性能消耗太高。而如果将对象保存在SliderInstance中的话，那这个类就没有存在的意义了。
	/// </summary>
	public class SliderEvent : TEN.DESIGNMODEL.Singleton<ButtonEvent>
    {
        public UnityAction<float> GetEvent(GLOBAL.ENUM.ESliderMapType vIn_MapType , string pIn_EventName, string pIn_Prameter)
        {
            switch (vIn_MapType)
            {
                case GLOBAL.ENUM.ESliderMapType.SHADER:
                    return (float vIn_SliderValue) => { LoadingScene(pIn_Prameter); };
                default:
                    return (float vIn_SliderValue) => { Debug.Log($"drag me! shuang! {vIn_SliderValue}"); };
            }
        }
        public void LoadingScene(string pIn_SceneName)
        {
            //Debug.Log($"drag me! shuang! {vIn_SliderValue}");
        }
    }
}
