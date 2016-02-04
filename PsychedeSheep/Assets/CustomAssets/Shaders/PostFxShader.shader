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
				

				//UNITY_TRANSFER_DEPTH(i.depth);
				fixed4 col = tex2D(_MainTex, i.uv);
				float hueShift = (_SinTime[3] * _HueModSpeed);
				float depthShift = _SinTime[3];// *i.depth;
				d = d*0.5+0.5* Linear01Depth(d);
				//float4 hue = float4(col.rgb.r*abs(sin(hueShift)), col.rgb.g*abs(sin(hueShift + 120)), col.rgb.b*abs(sin(hueShift + 240)),1);
				/*
				float4 hue = float4(
					col.rgb.r*(fmod((hueShift*d) + col.rgb.g*(1 - hueShift), 1) ),
					col.rgb.g*(fmod(((hueShift+=0.333333)*d) + col.rgb.b*(1 - hueShift), 1)),
					col.rgb.b*(fmod(((hueShift += 0.333333)*d)+ col.rgb.r*(1 - hueShift), 1)),
					1);
				*/
				float4 hue = col;
				//col = hue;
				
				col.r =fmod(0.75*hue.rgb.r*(hueShift*d) + 0.25*hue.rgb.b*(1 - hueShift*d),1);
				col.g =fmod(0.75*hue.rgb.g*((hueShift +=0.333333)*d) + 0.25*hue.rgb.r*(1 - hueShift*d), 1);
				col.b =fmod(0.75*hue.rgb.b*((hueShift += 0.333333)*d) + 0.25*hue.rgb.g*(1 - hueShift*d),1);
				//col.r = col.r*
				//*/
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
