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
			// ���建����
			StructuredBuffer<float2> _MousePositions;

			// ��������С
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
				// ������Ļ��߱���У��ϵ��
				float aspectRatio = _ScreenParams.x / _ScreenParams.y;

				//// У����� UV ��ֵ
				//float2 temp = o.uv - _MousePos.xy;
				//temp.x *= aspectRatio; // ����߱����� X ����
				//float length2 = dot(temp , temp);
				//if (length2 <= _Range*_Range)
				//{
				//	outColor.a = length2 / (_Range * _Range);
				//}
				float a = 1;
				float b = 2;
				// �������������λ��
				for (int index = 0; index < BufferSize; index++)
				{
					float2 mousePos = _MousePositions[index];
					float2 temp = o.uv - mousePos.xy;
					temp.x *= aspectRatio; // ����߱����� X ����
					// �ж� UV �Ƿ�����긽��
					float length2 = dot(temp, temp);
					if (length2 <= _Range * _Range)
					{
						//����ֱ�ӽ�a����Ϊ0���������ֺ���ֵ�Ч�� �� ���Է���ֱ��return�������ȫ�����Աȵõ�aֵ��С��
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