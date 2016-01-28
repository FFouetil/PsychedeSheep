Shader "Custom/TestSurfaceShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_GradTex ("Gradient", 2D) = "white" {}
	}

	SubShader {

      Tags { "RenderType" = "Opaque" "LightingMode" = "Vertex"}
      Lighting On

      Pass {
      		
			CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
			#pragma vertex vert
			#pragma fragment frag
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0


			sampler2D _GradTex;
			sampler2D _MainTex;

			struct v2f {
                float2 uv : TEXCOORD0;
                //float4 pos : SV_POSITION;
            };


			v2f vert(
		        float4 vertex : POSITION, // vertex position input
                float2 uv : TEXCOORD0,
                out float4 outpos : SV_POSITION )// first texture coordinate input)
                {
                	v2f o;
                	uv+=_Time*0.5;
                	o.uv=uv;
                	outpos = mul(UNITY_MATRIX_MVP, vertex);
                	return o;
                }

        	fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target 
        		{ return tex2D (_MainTex, i.uv);}

         	ENDCG
            }
          
//        CGPROGRAM
//		      #pragma surface surf Standard fullforwardshadows 
//		      #pragma target 3.0
//		      sampler2D _MainTex;
//		      struct Input {
//		          float2 uv_MainTex;
//		      };
//		      void surf (Input IN, inout SurfaceOutputStandard o) {
//		          o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
//		      }
//         ENDCG

		
	}
	FallBack "Diffuse"
}
