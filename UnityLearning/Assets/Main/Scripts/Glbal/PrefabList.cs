using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN
{
    /// <summary>
    ///项目 : TEN
    ///日期：2024/12/12 19:18:40 
    ///创建者：Michael Corleone
    ///类用途：
    /// </summary>
    [CreateAssetMenu(fileName = "NewPrefabList", menuName = "Custom Assets/Prefab List" , order = 1)]
    public class PrefabList : ScriptableObject
    {
        public GameObject CanvasPrefab;
        public GameObject ButtonPrefab;
    }

}
