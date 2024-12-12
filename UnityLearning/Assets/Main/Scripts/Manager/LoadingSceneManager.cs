using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TEN.MANAGER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/12 22:29:12 
	///创建者：Michael Corleone
	///类用途：
	/// </summary>
	public class LoadingSceneManager : MonoBehaviour
	{
        public static LoadingSceneManager Instance;
        private Slider _progress;
        private void Awake()
        {
            if (Instance) return;
            Instance = this;
            _progress = GameObject.Find("Canvas/Slider").GetComponent<Slider>();
        }
        public void Setprogress(float progress)
        {
            _progress.value = progress;
        }
    }
}
