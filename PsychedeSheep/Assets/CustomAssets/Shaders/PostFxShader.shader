Shader "Custom/PostFxShader"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_HueModSpeed("Hue Modulation Speed", Float) = 1.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
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
				//float2 depth : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.uv = v.uv;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				//UNITY_TRANSFER_DEPTH(o.depth);
				
				
				return o;
			}
			
			sampler2D _MainTex;
			float _HueModSpeed;
			sampler2D_float _CameraDepthTexture;

			fixed4 frag (v2f i) : SV_Target
			{
				//UNITY_OUTPUT_DEPTH(i.depth);
				float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv.xy);
				//d = Linear01Depth(d);

				//UNITY_TRANSFER_DEPTH(i.depth);
				fixed4 col = tex2D(_MainTex, i.uv);
				float hueShift = (_SinTime[3] * _HueModSpeed);
				float depthShift = _SinTime[3];// *i.depth;
				//float4 hue = float4(col.rgb.r*abs(sin(hueShift)), col.rgb.g*abs(sin(hueShift + 120)), col.rgb.b*abs(sin(hueShift + 240)),1);
				float4 hue = float4(
					col.rgb.r*hueShift+col.rgb.g*(1-hueShift),
					col.rgb.g*(fmod((hueShift+= depthShift),1) + col.rgb.b*(1 - hueShift)),
					col.rgb.b*(fmod((hueShift += depthShift), 1) + col.rgb.r*(1 - hueShift)),
					1);
				col.r *= d;
				col.g *= d;
				col.b *= d;
				//col.a = 1;
				//col = EncodeFloatRGBA(d);
				// just invert the colors
				//col = 1 - col;
				return col;
			}
			ENDCG


		}


	}


}
