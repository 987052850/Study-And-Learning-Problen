Shader "Learning/HW/DreamTicker/LitStencil"
{
	Properties
	{
		[MainColor] _BaseColor("Base Color",Color) = (1,1,1,1)
		[MainTexture] _MainTex("Base Map",2D) = "white"{}
		
		_HighlightColor("Highlight Color",Color) = (1,1,1,1)
		_HighlightMix("Highlight Mix",Range(0,1)) = 0

		[Header(Options)]
		[Enum(off,0,on,1)]_ZWriteMode("ZWriteMode",Float) = 1
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode",Float) = 2
		[Enum(UnityEngine.Rendering.CompareFunction)]_ZTestMode("ZTestMode",Float)=4
		[Enum(UnityEngine.Rendering.ColorWriteMask)]_ColorMask("ColorMask",Float)=15

		[Header(Blend)]
		[Enum(UnityEngine.Rendering.BlendOp)]_BlendOp("BlendOp",Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlend",Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("DstBlend",Float) = 0

		[Header(Stencil)]
		_StencilRef("Stencil Ref" , Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("Stencil Comp",Float) = 8
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilPass("Stencil Pass",Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilFail("Stencil Fail",Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
		}
		Pass
		{
			BlendOp [_BlendOp]
			Blend [_SrcBlend] [_DstBlend]

			ZWrite [_ZWriteMode]
			ZTest [_ZTestMode]
			Cull [_CullMode]
			ColorMask [_ColorMask]

			Stencil
			{
				Ref [_StencilRef]
				Comp [_StencilComp]
				Pass [_StencilPass]
				Fail [_StencilFail]
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct a2v
			{
				float4 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float2 uv : TEXCOORD0;
			};
			struct v2f
			{
				float4 positionHCS : SV_POSITION;
				float3 normalWS : TEXCOORD1;
				float2 uv : TEXCOORD0;
			};
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _BaseColor;
			float4 _HighlightColor;
			float _HighlightMix;
			v2f vert(a2v i)
			{
				v2f o;
				o.positionHCS = UnityObjectToClipPos(i.positionOS);
				o.normalWS = UnityObjectToWorldNormal(i.normalOS);
				o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				return o;
			}
			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex,i.uv);
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				float halfLambert = pow(0.5*dot(i.normalWS, lightDir) + 0.5, 2);
				color.rgb *= halfLambert;
				return color * lerp(_BaseColor,_HighlightColor,_HighlightMix);
			}
			ENDCG
		}
	}
}