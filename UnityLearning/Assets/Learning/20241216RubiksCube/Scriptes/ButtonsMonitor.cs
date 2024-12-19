using System.Collections;
using System.Collections.Generic;
using TEN.GLOBAL.STRUCT;
using UnityEngine;
using UnityEngine.UI;

namespace TEN.MANAGER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/17 19:09:49 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class ButtonsMonitor
	{
        private Button _curClick;
        private Button _preClicked;
        public bool Monitor = true;

        public void Reset()
        {
            _curClick = null;
            _preClicked = null;
        }
        public void OnChildClick(Button pIn_Button)
        {
            _curClick = pIn_Button;
            Monitor = _curClick != _preClicked;
            if (!Monitor)
            {
                Reset();
            }
            _preClicked = _curClick; ;
        }
    }
}
