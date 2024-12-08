Shader "Learning/HW/Graphic/GPUInstancing"
{
	Properties
	{
		_MainTex("Texture" , 2D) = "white"{}
		_Color("Color" , Color) = (1,1,1,1)
		_Lerp("Lerp" , range(0,1)) = 1
	}
		SubShader
	{
		Tags
		{

		}

		Pass
		{


			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//开启GPUInstancing,此时使用Shader对应材质的对象会在场景中消失很多
			//这是由于GPU在处理实例对象时没有为每个对象指定唯一的id
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;	
			fixed _Lerp;
			//fixed4 _Color;

			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(fixed4 , _Color)
			UNITY_INSTANCING_BUFFER_END(Props)

			struct appdata
			{
				//声明一个instance id
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 vertex : POSITION;
				half2 uv : TEXCOORD0;
			};
			struct v2f
			{
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 vertex : SV_POSITION;
				half2 uv : TEXCOORD0;
			};
			v2f vert(appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v,o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv,_MainTex);
				return o;
			}
			fixed4 frag(v2f o) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(o);
				fixed4 outColor = tex2D(_MainTex , o.uv	);
				return lerp(outColor , UNITY_ACCESS_INSTANCED_PROP(Props,_Color) , _Lerp);
			//return UNITY_ACCESS_INSTANCED_PROP(Props,_Color);
			}
			ENDCG
		}
	}
}