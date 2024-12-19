Shader "Learning/HW/RubiksCube"
{
	Properties
	{
		_MainTex("Texture" , 2D) = "white"{}
		_Color("Color" , vector) = (0,0,0,0)
		_AlphaScale ("Alpha Scale" , Range(0,1)) = 1
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
		}
		LOD 100
		Pass
		{
			Tags
			{
				"LinghtMode" = "ForwardBase"
			}
			blend SrcAlpha OneMinusSrcAlpha
			ZTest off
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed4 _Color;
			fixed _AlphaScale;
			struct appdata
			{
				float4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};
			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv,_MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				
				return o;
			}
			fixed4 frag(v2f o) : SV_Target
			{
				fixed4 color;
				color = tex2D(_MainTex , o.uv);
				fixed3 worldNormal = normalize(o.worldNormal);
				fixed3 worldPos = normalize(o.worldPos);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				return fixed4(ambient * color  , color.a * _AlphaScale);
			}
			ENDCG
		}

		Pass
		{
			Tags
			{
				"LinghtMode" = "ForwardBase"
			}
			blend SrcAlpha OneMinusSrcAlpha
			ZTest off
			Cull Back

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed4 _Color;
			fixed _AlphaScale;
			struct appdata
			{
				float4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};
			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv,_MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}
			fixed4 frag(v2f o) : SV_Target
			{
				fixed4 color;
				color = tex2D(_MainTex , o.uv);
				fixed3 worldNormal = normalize(o.worldNormal);
				fixed3 worldPos = normalize(o.worldPos);
				fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				return fixed4(color.rgb  , color.a * _AlphaScale);
			}
			ENDCG
		}

	}
}