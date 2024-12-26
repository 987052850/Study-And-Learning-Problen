using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.SETTING
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/21 12:14:35 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class ApplicationSetting : MonoBehaviour
	{
        public int MaxFrameRate = 120;
        private void Awake()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Application.targetFrameRate = MaxFrameRate;
            }
        }
    }
}
