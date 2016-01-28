Shader "Custom/ScrollSurfaceShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_ScrollSpeedX ("Scroll Speed X", Float) = 1.0
		_ScrollSpeedY ("Scroll Speed Y", Float) = 1.0
		_HueModSpeed ("Hue Modulation Speed", Float) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed _ScrollSpeedX, _ScrollSpeedY, _HueModSpeed;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			IN.uv_MainTex.x+=_Time*_ScrollSpeedX;
			IN.uv_MainTex.y+=_Time*_ScrollSpeedY;
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			float hueShift=_Time[1]*_HueModSpeed/1000*360;
			float4 hue = float4(c.rgb.r*abs(sin(hueShift)),c.rgb.g*abs(sin(hueShift+120)),c.rgb.b*abs(sin(hueShift+240)),1);

			o.Albedo = c.rgb*(hue*hue);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
