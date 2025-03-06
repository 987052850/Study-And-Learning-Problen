using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
namespace TEN.INTERVIEW
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;

    public class DistortionEffectRenderer : PostProcessEffectRenderer<DistortionEffect>
    {
        private Shader kamuiShader;
        private Material kamuiMaterial;

        public override void Init()
        {
            kamuiShader = Shader.Find("Hidden/Custom/KamuiDistortionEffect");
            if (kamuiShader != null)
            {
                kamuiMaterial = new Material(kamuiShader);
            }
            else
            {
                Debug.LogError("未找到 Kamui 扭曲 Shader：Hidden/Custom/KamuiDistortionEffect");
            }
        }

        public override void Render(PostProcessRenderContext context)
        {
            if (!settings.enable.value || settings.intensity.value <= 0f)
            {
                // 直接拷贝，不进行扭曲
                context.command.Blit(context.source, context.destination);
                return;
            }

            // 将扭曲强度传递给 Shader
            kamuiMaterial.SetFloat("_Intensity", settings.intensity.value);
            kamuiMaterial.SetVector("_SwirlCenter", new Vector2(Input.mousePosition.x / Screen.width , Input.mousePosition.y / Screen.height));
            kamuiMaterial.SetFloat("_SwirlRadius", settings.radius.value);
            kamuiMaterial.SetFloat("_SwirlAngle", settings.angle.value);
            // 这里也可以传入其他参数，比如扭曲中心、半径、最大旋转角度等（在 Shader 中定义默认值）
            context.command.Blit(context.source, context.destination, kamuiMaterial);
        }
    }

}
