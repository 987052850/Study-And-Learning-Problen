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
        public GameObject InstantiateScrollView(SScrollViewData vIn_ScrollViewData)
        {
            GameObject infiniteScrolling = Object.Instantiate(UIPrefab.InfiniteScrollingPrefab, vIn_ScrollViewData.Parent);
            infiniteScrolling.name = vIn_ScrollViewData.Name;
            InfiniteScrolling infiniteScrollingInstance = infiniteScrolling.GetComponent<InfiniteScrolling>();
            infiniteScrollingInstance.Init(vIn_ScrollViewData);
            return infiniteScrolling;
        }
        public GameObject InstantiateSlider(SSliderData vIn_SliderData)
        {
            GameObject slider = Object.Instantiate(UIPrefab.Slider, vIn_SliderData.Parent);
            slider.name = vIn_SliderData.Name;
            SliderInstance sliderInstance = slider.GetComponent<SliderInstance>();
            sliderInstance.Init(vIn_SliderData);
            return slider;
        }
        public GameObject InstantiateImage(SImageData vIn_ImageData)
        {
            GameObject slider = Object.Instantiate(UIPrefab.Image, vIn_ImageData.Parent);
            slider.name = vIn_ImageData.Name;
            ImageInstance imageInstance = slider.GetComponent<ImageInstance>();
            imageInstance.Init(vIn_ImageData);
            return slider;
        }
        public GameObject InstantiateTextMeshPro(STextMeshProData vIn_TextMeshProData)
        {
            GameObject slider = Object.Instantiate(UIPrefab.TextMeshPro, vIn_TextMeshProData.Parent);
            slider.name = vIn_TextMeshProData.Name;
            TextMeshProInstance TMPInstance = slider.GetComponent<TextMeshProInstance>();
            TMPInstance.Init(vIn_TextMeshProData);
            return slider;
        }
    }
}
