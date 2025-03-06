using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEN.INTERVIEW
{
    /// <summary>
    ///项目 : TEN
    ///日期：2025/3/6 12:09:54 
    ///创建者：Michael Corleone
    ///类用途：
    /// </summary>
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;

    public class DistortionEffectController : MonoBehaviour
    {
        [Tooltip("请将场景中 PostProcessVolume 组件拖入此处")]
        public PostProcessVolume volume;

        private DistortionEffect effect;

        void Start()
        {
            if (volume == null)
            {
                volume = GetComponent<PostProcessVolume>();
            }

            // 从 Volume Profile 中获取自定义 DistortionEffect 效果
            if (volume.profile.TryGetSettings<DistortionEffect>(out effect))
            {
                // 初始关闭效果，扭曲强度为 0
                effect.enable.value = false;
                effect.intensity.value = 0f;
            }
            else
            {
                Debug.LogError("Volume Profile 中未找到 DistortionEffect 效果，请确认已添加！");
            }
        }

        void Update()
        {
            // 按空格键切换效果的开关
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetState();
            }

            // 按上箭头增加扭曲强度
            if (Input.GetKey(KeyCode.UpArrow))
            {
                Add();
            }
            // 按下箭头降低扭曲强度
            if (Input.GetKey(KeyCode.DownArrow))
            {
                Minus();
            }
        }
        public void SetState()
        {
            effect.enable.value = !effect.enable.value;
        }
        public void Add()
        {
            effect.intensity.value = Mathf.Clamp01(effect.intensity.value + Time.deltaTime);
        }
        public void Minus()
        {
            effect.intensity.value = Mathf.Clamp01(effect.intensity.value - Time.deltaTime);
        }
    }

}
