using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TEN.MANAGER;
using UnityEngine.Events;

namespace TEN.EVENTS
{
    /// <summary>
    ///项目 : TEN
    ///日期：2024/12/12 21:50:41 
    ///创建者：Michael Corleone
    ///类用途：
    /// </summary>
    public class ButtonEvent : TEN.DESIGNMODEL.Singleton<ButtonEvent>
    {
        public UnityAction GetEvent(string pIn_EventName , string pIn_Prameter)
        {
            switch (pIn_EventName)
            {
                case "LoadScene":
                    return () => { LoadingScene(pIn_Prameter); };
                default:
                    return () => { Debug.Log("Click me!shuang!"); };
            }
        }
        public void LoadingScene(string pIn_SceneName)
        {
            MSceneManager.Instance.LoadScene(pIn_SceneName);
        }
	}
}
