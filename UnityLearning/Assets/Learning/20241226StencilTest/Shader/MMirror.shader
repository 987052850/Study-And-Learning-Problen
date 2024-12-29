Shader "Learning/HW/StencilLearning/Mirror"
{
	Properties
	{
		 _MainTex("Main Texture" , 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
		Pass
		{
			stencil
			{
				Ref 2
				Comp Always
				Pass Replace
			}
			ColorMask 0
			ZWrite off
		}


		Pass
		{
			Name "Mirror Color"

			Tags
			{
				"LightMode" = "Always"
			}

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
			float4 _MainTex_ST;

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
				return fixed4(1,1,1,0.2);
			}

			ENDCG

		}
	}
}