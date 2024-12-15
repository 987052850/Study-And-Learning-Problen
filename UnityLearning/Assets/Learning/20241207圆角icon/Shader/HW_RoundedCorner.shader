Shader "Learning/HW/UI/RoundedCorner"
{
	Properties
	{
		_MainTex("Texture" , 2D) = "white" {}
		_R("Radius" , range(0,0.5)) = 0
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
			ZWrite off

			CGPROGRAM
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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _R;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv , _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//渐变
				//fixed value = (i.uv.x + i.uv.y) / 2;
				//fixed4 outCol = fixed4(value, value, value,1);
				//return outCol;
				//获取maintex对应uv坐标的颜色	
				fixed4 outCol = tex2D(_MainTex , i.uv);
				float aspectRatio = (_ScreenParams.x / _ScreenParams.y) ;
				//只要x或者y有一个值落在阙值的范围之内，即将alpha设置为1
				//所以，需要比较转换到第一现象的uv坐标与阙值的关系
				//如果大于阙值则为1，即需要将其进行透明处理
				//但是对于期望效果而言，只要有x、y其中一个落在阙值之内，则不需要对其进行透明处理
				//故使用1 - x * y
				/*
				此时，贴图四个角会根据_R的值进行透明处理，但是，并不是最终期望的圆角效果
				需要在进行一次判断，即，当前的uv坐标距离右上角圆形的位置是否小于半径
				如果大于半径则需要将透明度设置为0
				也就是说，需要当uv与圆形之间的距离大于右上角圆的半径时，x*y*l的值应当为1
				根据上述需求，只要当距离小于半径时，将l置为0即可满足需求
				*/
				fixed alpha = 0;
				fixed2 halfUV = abs(i.uv - half2(0.5,0.5));
				fixed quezhi = abs(0.5 - _R);
				fixed stepX = step(quezhi , halfUV.x);
				fixed stepY = step(quezhi, halfUV.y);
				//计算距离需要开根号，对性能不友好
				//fixed stepL = step(_R , abs(length(fixed2(quezhi, quezhi) - halfUV)));
				//region 使用距离的平方对比
				fixed2 v = fixed2(quezhi, quezhi) - halfUV;
				fixed stepL = step(_R * _R, dot(v,v));
				//endregion
				alpha = 1- stepX * stepY * stepL;
				outCol.w = alpha;
				return outCol;
			}
			ENDCG
		}
	}
}