Shader "ImageEffect/AlphaEdge" {
	Properties {
		_MainTex ("Render Input", 2D) = "white" {}
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha, One One
		Pass {
			CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#include "UnityCG.cginc"
			
				sampler2D _MainTex;
				float2 stepSize;
			
				float4 frag(v2f_img IN) : COLOR {
					/*
						kernal
						1  1 1
						1 -8 1
						1  1 1
					*/
					
					float alpha =  -8 * tex2D (_MainTex, IN.uv).a;
					
					alpha += tex2D (_MainTex, IN.uv + float2(stepSize.x,0)).a;
					alpha += tex2D (_MainTex, IN.uv + float2(-stepSize.x,0)).a;
					alpha += tex2D (_MainTex, IN.uv + float2(0,stepSize.y)).a;
					alpha += tex2D (_MainTex, IN.uv + float2(0,-stepSize.y)).a;
					
					alpha += tex2D (_MainTex, IN.uv + float2(stepSize.x,stepSize.y)).a;
					alpha += tex2D (_MainTex, IN.uv + float2(stepSize.x,-stepSize.y)).a;
					alpha += tex2D (_MainTex, IN.uv + float2(-stepSize.x,stepSize.y)).a;
					alpha += tex2D (_MainTex, IN.uv + float2(-stepSize.x,-stepSize.y)).a;
					
					return float4(1,1,1,alpha);
				}
			ENDCG
		}
	}
}