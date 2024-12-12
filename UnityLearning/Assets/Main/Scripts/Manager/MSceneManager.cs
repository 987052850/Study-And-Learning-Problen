using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TEN.MANAGER
{
    /// <summary>
    ///项目 : TEN
    ///日期：2024/12/12 21:59:18 
    ///创建者：Michael Corleone
    ///类用途：
    /// </summary>
    public class MSceneManager : MonoBehaviour
    {
        public static MSceneManager Instance;
        private static readonly object _lock = new object();

        private string _sceneName;
        private bool _isLoading = false;
        private float _minTime = 2.0f;
        private float _factor = 0.2f;
        //private string[] _allScenesName;
        void Awake()
        {
            if (Instance) return;
            Instance = this;
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
            //Scene[] scenes = SceneManager.GetAllScenes();
            //_allScenesName = new string[scenes.Length];
            //for (int i = 0; i < scenes.Length; i++)
            //{
            //    _allScenesName[i] = scenes[i].name;
            //    Debug.Log(_allScenesName[i]);
            //}

        }
        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void SetLoadingState(bool vIn_State)
        {
            lock (_lock)
            {
                _isLoading = vIn_State;
            }
        }


        // 加载场景
        public void LoadScene(string sceneName)
        {
            if (_isLoading) return;
            //Debug.Log($"检测 {sceneName} ");
            //if (!DetectSceneName(sceneName)) return;
            SetLoadingState(true);
            _sceneName = sceneName;
            SceneManager.LoadScene("Loading");
        }

        // 场景加载完成后的回调方法
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!scene.name.Equals("Loading")) return;
            StartCoroutine(AsyncLoadingScene());
        }

        //private bool DetectSceneName(string sceneName)
        //{
        //    foreach (var item in _allScenesName)
        //    {
        //        if (sceneName == item)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        IEnumerator AsyncLoadingScene()
        {
            // 异步加载场景
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneName);
            asyncLoad.allowSceneActivation = false;
            float time = 0;
            float progress = 0;

            // 等待场景加载完成
            while (progress < 0.9999f)
            {
                progress = asyncLoad.progress * _factor + time / _minTime * (1 - _factor);
                LoadingSceneManager.Instance.Setprogress(progress);
                time += Time.deltaTime;
                yield return null;
            }
            asyncLoad.allowSceneActivation = true;
            _isLoading = false;
        }
    }
}
