using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace TEN.INTERVIEW
{
    [System.Serializable]
    [PostProcess(typeof(DistortionEffectRenderer), PostProcessEvent.AfterStack, "Custom/DistortionEffect")]
    public sealed class DistortionEffect : PostProcessEffectSettings
    {
        // 控制效果是否生效（Volume Profile 中勾选 Override 后才生效）
        public BoolParameter enable = new BoolParameter { value = false };
        // 扭曲强度，范围 0 ~ 1
        [Range(0f, 1f), Tooltip("扭曲强度")]
        public FloatParameter intensity = new FloatParameter { value = 0f };
        [Range(0f, 1f), Tooltip("扭曲中心")]
        public Vector2Parameter center = new Vector2Parameter { value = { } };
        [Range(0f, 1f), Tooltip("扭曲强度")]
        public FloatParameter radius = new FloatParameter { value = 0f };
        [Range(0f, 10f), Tooltip("扭曲强度")]
        public FloatParameter angle = new FloatParameter { value = 0f };
    }
}
