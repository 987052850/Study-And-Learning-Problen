using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TEN.GLOBAL.ENUM;

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
        private EScene _curScene = EScene.MAIN_SCENE;
        private EScene _preScene = EScene.MAIN_SCENE;
        private Stack<EScene> _sceneStack;
        private Dictionary<string, EScene> _nameMapEnum;
        private Dictionary<EScene, string> _sceneMapToPath;
        //private string[] _allScenesName;
        void Awake()
        {
            if (Instance) return;
            Instance = this;
            DontDestroyOnLoad(this);
            _sceneStack = new Stack<EScene>();
            //添加新场景修改内容 3/4
            _nameMapEnum = new Dictionary<string, EScene>()
            {
                { "Fork" , EScene.FORK_SCENE},
                { "IndividualExercises" , EScene.PRACTICE_SCENE},
                { "MainScene" , EScene.MAIN_SCENE},
                { "RoundedCorner" , EScene.ROUNDED_CORNER},
                { "GPUInstancing" , EScene.GPU_INSTANCING},
                { "TransparentFollowMouse" , EScene.TRANSPARENT_FOLLOW_MOUSE},
                { "RubiksCube" , EScene.FORK_RUBIKS_CUBE},
                { "StencilTest" , EScene.STENCIL_TEST},
            };
        
            _sceneMapToPath = new Dictionary<EScene, string>()
            {
                { EScene.MAIN_SCENE , Application.streamingAssetsPath + TEN.GLOBAL.Path.XMLCongigPath.MainSceneConfig},
                { EScene.FORK_SCENE , Application.streamingAssetsPath + TEN.GLOBAL.Path.XMLCongigPath.ForkSceneConfig},
                { EScene.PRACTICE_SCENE , Application.streamingAssetsPath + TEN.GLOBAL.Path.XMLCongigPath.SelfPracticeSceneConfig},
                { EScene.ROUNDED_CORNER , Application.streamingAssetsPath + TEN.GLOBAL.Path.XMLCongigPath.RoundedCornerSceneConfig},
                { EScene.GPU_INSTANCING , Application.streamingAssetsPath + TEN.GLOBAL.Path.XMLCongigPath.GPUInstancingSceneConfig},
                { EScene.TRANSPARENT_FOLLOW_MOUSE , Application.streamingAssetsPath + TEN.GLOBAL.Path.XMLCongigPath.TransparentFollowMouseSceneConfig},
                { EScene.FORK_RUBIKS_CUBE , Application.streamingAssetsPath + TEN.GLOBAL.Path.XMLCongigPath.RubiksCubeSceneConfig},
                { EScene.STENCIL_TEST , Application.streamingAssetsPath + TEN.GLOBAL.Path.XMLCongigPath.StencilTestSceneConfig},
           };
            //Scene[] scenes = SceneManager.GetAllScenes();
            //_allScenesName = new string[scenes.Length];
            //for (int i = 0; i < scenes.Length; i++)
            //{
            //    _allScenesName[i] = scenes[i].name;
            //    Debug.Log(_allScenesName[i]);
            //}

        }
        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
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

        public void OnLoadSceneClick(string sceneName)
        {
            if (LoadScene(sceneName))
            {
                _sceneStack.Push(_curScene);
                Debug.Log($"Fix Bug {_curScene}");
            }
        }

        // 加载场景
        private bool LoadScene(string sceneName)
        {
          
            if (_isLoading) return false;
            //Debug.Log($"检测 {sceneName} ");
            //if (!DetectSceneName(sceneName)) return;
            SetLoadingState(true);
            _sceneName = sceneName;
            Debug.Log(_sceneName);
            SceneManager.LoadScene("Loading");
            return true;
        }

        private string GetPathByName(string name)
        {
            EScene loadingScene = EScene.FORK_SCENE;
            if (_nameMapEnum.ContainsKey(name))
            {
                loadingScene = _nameMapEnum[name];
                SetCurrentScene(loadingScene);
                if (_sceneMapToPath.ContainsKey(loadingScene))
                {
                    return _sceneMapToPath[loadingScene];
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }
        private string GetSceneName(EScene vIn_Scene)
        {
            if (_nameMapEnum.ContainsValue(vIn_Scene))
            {
                foreach (var item in _nameMapEnum)
                {
                    if (item.Value == vIn_Scene)
                    {
                        return item.Key;
                    }
                }
                return "";
            }
            else
            {
                return "";
            }
        }
        private void SetCurrentScene(EScene vIn_CurScene)
        {
            _preScene = _curScene;
            _curScene = vIn_CurScene;
        }

        public void Back()
        {
            if (_sceneStack.Count <= 0)
            {
                return;
            }
            SetCurrentScene(_sceneStack.Pop());
            Debug.Log($"Fix Bug pop {_sceneStack.Count}");
            LoadScene(GetSceneName(_curScene));
        }

        // 场景加载完成后的回调方法
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals("Loading"))
            {
                StartCoroutine(AsyncLoadingScene());
            }
            else
            {
                Debug.Log(scene.name);
                ReadConfig.ConfigReader.ReadXML(GetPathByName(_sceneName));

                _isLoading = false;
            }
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
        }
    }
}
