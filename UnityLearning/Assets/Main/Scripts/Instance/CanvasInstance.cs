using System.Collections;
using System.Collections.Generic;
using TEN.GLOBAL.STRUCT;
using UnityEngine;
using UnityEngine.UI;

namespace TEN.INSTANCE
{
    //项目 : TEN
    //日期：2024/12/11 21:38:56 
    //创建者：Michael Corleone
    //类用途：
    public class CanvasInstance : MonoBehaviour, TEN.INTERFACE.InterfaceInit
    {
        private Image _background;
        public void Init(SInterface vIn_InitData)
        {
            _background = transform.GetComponentInChildren<Image>();
            Texture2D texture = Resources.Load<Texture2D>(vIn_InitData.ImagePath);
            _background.sprite = Sprite.Create(texture , new Rect(0,0, texture.width , texture.height) , Vector2.one / 2);
        }
    }
}
