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
				//����
				//fixed value = (i.uv.x + i.uv.y) / 2;
				//fixed4 outCol = fixed4(value, value, value,1);
				//return outCol;
				//��ȡmaintex��Ӧuv�������ɫ	
				fixed4 outCol = tex2D(_MainTex , i.uv);
				float aspectRatio = (_ScreenParams.x / _ScreenParams.y) ;
				//ֻҪx����y��һ��ֵ������ֵ�ķ�Χ֮�ڣ�����alpha����Ϊ1
				//���ԣ���Ҫ�Ƚ�ת������һ�����uv��������ֵ�Ĺ�ϵ
				//���������ֵ��Ϊ1������Ҫ�������͸������
				//���Ƕ�������Ч�����ԣ�ֻҪ��x��y����һ��������ֵ֮�ڣ�����Ҫ�������͸������
				//��ʹ��1 - x * y
				/*
				��ʱ����ͼ�ĸ��ǻ����_R��ֵ����͸���������ǣ�����������������Բ��Ч��
				��Ҫ�ڽ���һ���жϣ�������ǰ��uv����������Ͻ�Բ�ε�λ���Ƿ�С�ڰ뾶
				������ڰ뾶����Ҫ��͸��������Ϊ0
				Ҳ����˵����Ҫ��uv��Բ��֮��ľ���������Ͻ�Բ�İ뾶ʱ��x*y*l��ֵӦ��Ϊ1
				������������ֻҪ������С�ڰ뾶ʱ����l��Ϊ0������������
				*/
				fixed alpha = 0;
				fixed2 halfUV = abs(i.uv - half2(0.5,0.5));
				fixed quezhi = abs(0.5 - _R);
				fixed stepX = step(quezhi , halfUV.x);
				fixed stepY = step(quezhi, halfUV.y);
				//���������Ҫ�����ţ������ܲ��Ѻ�
				//fixed stepL = step(_R , abs(length(fixed2(quezhi, quezhi) - halfUV)));
				//region ʹ�þ����ƽ���Ա�
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