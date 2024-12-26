using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.EVENTS
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/21 12:23:22 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class RubiksCubeEvent : DESIGNMODEL.Singleton<RubiksCubeEvent>
	{
        private TMPro.TextMeshProUGUI _completeText;
        private RubiksCubeEvent()
        {
            _completeText = GameObject.Find("Canvas/Complete").GetComponent< TMPro.TextMeshProUGUI>();
        }

        public void OnComplete()
        {
            _completeText.text = "COMPLETE";
        }
	}
}
