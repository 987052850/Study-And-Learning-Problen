Shader "Hidden/Custom/KamuiDistortionEffect"
{
	Properties
	{
		_Intensity("Intensity", Range(0, 1)) = 0.0
		// �������������ɵ���Ҳ������ Shader ���趨Ĭ��ֵ
		_SwirlCenter("Swirl Center", Vector) = (0.5, 0.5, 0, 0)
		_SwirlRadius("Swirl Radius", Range(0, 1)) = 0.5
		_SwirlAngle("Swirl Angle", Range(0, 10)) = 3.0
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" "Queue" = "Overlay" }
		Pass
		{
			Name "KamuiDistortion"
			ZTest Always Cull Off ZWrite Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// ����
			float _Intensity;
			float4 _SwirlCenter;  // ʹ�� xy ����
			float _SwirlRadius;
			float _SwirlAngle;

			sampler2D _MainTex;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			// ������Ļ UV ʵ������Ť��
			float2 ApplySwirl(float2 uv)
			{
				float2 delta = uv - _SwirlCenter.xy;
				float dist = length(delta);
				if (dist < _SwirlRadius)
				{
					// ����Խ����תԽ��
					float percent = (_SwirlRadius - dist) / _SwirlRadius;
					float theta = percent * _SwirlAngle * _Intensity;
					float s = sin(theta);
					float c = cos(theta);
					delta = float2(delta.x * c - delta.y * s, delta.x * s + delta.y * c);
				}
				return _SwirlCenter.xy + delta;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// �Ե�ǰ���� UV Ӧ��Ť��
				float2 uv = ApplySwirl(i.uv);
				fixed4 col = tex2D(_MainTex, uv);
				return col;
			}
			ENDHLSL
		}
	}
}
