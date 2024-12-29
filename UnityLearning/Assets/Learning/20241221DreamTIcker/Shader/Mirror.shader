Shader"Learning/HW/DreamTicker/Mirror"
{
	Properties
	{
		[MainColor] _BaseColor("Base Color" , Color) = (1,1,1,1)
		[MainTexture] _MainTex("Main Texture" , 2D) = "white" {}
		
		_HighlightColor("Highlight Color",Color) = (1,1,1,1)
		_HighlightMix("Highlight Mix",Range(0,1)) = 0
	}

	SubShader
	{
		Tags
		{
			"LightMode" = "Always"
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
		Pass
		{


			BlendOp Add
			Blend SrcAlpha OneMinusSrcAlpha

			ZWrite off
			Cull off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 position : POSITION;
				fixed2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 position : SV_POSITION;
				fixed2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _BaseColor;
			float4 _MainTex_ST;
			float4 _HighlightColor;
			float _HighlightMix;

			v2f vert(appdata v)
			{
				v2f o;
				o.position = UnityObjectToClipPos(v.position);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex , i.uv);
				return color * lerp(_BaseColor, _HighlightColor, _HighlightMix);
			}

			ENDCG

		}
	}
}