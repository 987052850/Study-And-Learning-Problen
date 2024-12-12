using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TEN.GLOBAL.STRUCT;
using TEN.INSTANCE;

namespace TEN.MANAGER
{
	/// <summary>
	///项目 : TEN
	///日期：2024/12/11 22:01:12 
	///创建者：Michael Corleone
	///类用途：用于实例化一些项目中的预制体
	/// </summary>
	public class InstantiateManager : TEN.DESIGNMODEL.Singleton<InstantiateManager>
	{
        private PrefabList UIPrefab;
        private InstantiateManager()
        {
            UIPrefab = Resources.Load(@"ScriptObject/UIPrefabs") as PrefabList;
            Debug.Log(UIPrefab.CanvasPrefab.name);
        }
        public GameObject InstantiateCanvas(SInterface vIn_Interface)
        {
            GameObject Canvas = Object.Instantiate(UIPrefab.CanvasPrefab,vIn_Interface.Parent);
            Canvas.name = vIn_Interface.Name;
            CanvasInstance canvasInstance = Canvas.GetComponent<CanvasInstance>();
            canvasInstance.Init(vIn_Interface);
            return Canvas;
        }
        public GameObject InstantiateButton(SButtonData vIn_ButtonData)
        {
            GameObject button = Object.Instantiate(UIPrefab.ButtonPrefab, vIn_ButtonData.Parent);
            button.name = vIn_ButtonData.Name;
            ButtonInstance buttonInstance = button.GetComponent<ButtonInstance>();
            buttonInstance.Init(vIn_ButtonData);
            return button;
        }
	}
}
