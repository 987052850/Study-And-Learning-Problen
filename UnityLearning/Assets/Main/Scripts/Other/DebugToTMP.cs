using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/15 21:00:28 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class DebugToTMP : MonoBehaviour
	{
        public TMPro.TextMeshProUGUI text;
        public static DebugToTMP Instance;
        private void Awake()
        {
            if (Instance)
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(this);
            Instance = this;
        }
        public void Log(string s)
        {
            text.text = s;
        }
    }
}
