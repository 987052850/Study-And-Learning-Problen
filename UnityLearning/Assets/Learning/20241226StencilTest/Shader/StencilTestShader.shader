Shader "Learning/HW/StencilLearning/Wall"
{
	Properties
	{
		_Color("Color" , Color) = (1,1,1,1)
		_MainTex("Texture" , 2D) = "white" {}
		_Specular("Specular",Color) = (1,1,1,1)
		_Gloss("Gloss",Range(0,256)) = 20

			[Space]
		_StencilRef("Ref" , float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)]	_StencilComp("Comp" , float) = 2
	}
		SubShader
		{
			stencil
			{
				Ref [_StencilRef]
				Comp [_StencilComp]
			}
			Tags
			{
				"Queue" = "Overlay"
			}
			Pass
			{
				Tags
				{
					"LightMode" = "ForwardBase"
				}
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "UnityLightingCommon.cginc"
				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _Specular;
				fixed _Gloss;

				struct a2v
				{
					float4 position : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};
				struct v2f
				{
					float4 position : SV_POSITION;
					float3 worldNormal : TEXCOORD0;
					float3 worldPos : TEXCOORD1;
					float2 uv : TEXCOOR2;
				};
				v2f vert(a2v v)
				{
					v2f o;
					o.position = UnityObjectToClipPos(v.position);
					o.worldNormal = UnityObjectToWorldNormal(v.normal);
					o.worldPos = mul(unity_ObjectToWorld, v.position);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}
				fixed4 frag(v2f i) : SV_Target
				{
					fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
					fixed3 worldNormal = normalize(i.worldNormal);
					fixed3 worldLight = normalize(_WorldSpaceLightPos0.xyz);
					fixed3 texResult = tex2D(_MainTex, i.uv) * _Color;
					fixed3 diffuse = _LightColor0 * texResult * saturate(dot(worldLight, worldNormal));
					fixed3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
					fixed3 halfDir = normalize(viewDir + worldLight);
					fixed3 specular = _LightColor0.rgb * _Specular * pow(saturate(dot(worldNormal,halfDir)), _Gloss);
					return fixed4(ambient + diffuse + specular, 1);
				}
			ENDCG
		}
	}
}