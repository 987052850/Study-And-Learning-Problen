Shader "Learning/HW/UI/TransparentFollowMouse"
{
	Properties
	{
		_MainTex("Texture" , 2D) = "white"{}
		_MousePos("Mouse Position" , vector) = (0,0,0,0)
		_Range("Range" , range(0.001,1)) = 0.1
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"

		}
		LOD 100

		Pass
		{
			blend SrcAlpha OneMinusSrcAlpha
			ZTest off


			CGPROGRAM
			#pragma vertex vert;
			#pragma fragment frag;
			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex: POSITION;
				float2 uv: TEXCOORD0;
			};
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MousePos;
			float _Range;
			// 定义缓冲区
			StructuredBuffer<float2> _MousePositions;

			// 缓冲区大小
			#define BufferSize 30
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv , _MainTex);
				return o;
			}
			fixed4 frag(v2f o):SV_Target
			{
				fixed4 outColor = tex2D(_MainTex,o.uv);
				// 计算屏幕宽高比例校正系数
				float aspectRatio = _ScreenParams.x / _ScreenParams.y;

				//// 校正后的 UV 差值
				//float2 temp = o.uv - _MousePos.xy;
				//temp.x *= aspectRatio; // 按宽高比缩放 X 坐标
				//float length2 = dot(temp , temp);
				//if (length2 <= _Range*_Range)
				//{
				//	outColor.a = length2 / (_Range * _Range);
				//}
				float a = 1;
				float b = 2;
				// 遍历缓冲区鼠标位置
				for (int index = 0; index < BufferSize; index++)
				{
					float2 mousePos = _MousePositions[index];
					float2 temp = o.uv - mousePos.xy;
					temp.x *= aspectRatio; // 按宽高比缩放 X 坐标
					// 判断 UV 是否在鼠标附近
					float length2 = dot(temp, temp);
					if (length2 <= _Range * _Range)
					{
						//除非直接将a设置为0，否则会出现很奇怪的效果 ， 所以放弃直接return，会遍历全部，对比得到a值最小的
						//float a = length2 / (_Range * _Range);
						//outColor.a = pow(a , 3);
						//outColor.a = 0;
						//return outColor;

						b = length2 / (_Range * _Range);
						a = a < b ? a : b;
					}
				}
				outColor.a = a;
				return outColor;
			}
			ENDCG
		}
	}
}