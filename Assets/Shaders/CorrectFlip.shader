Shader "ImageEffect/CorrectFlip" {
	Properties {
		_MainTex ("Render Input", 2D) = "white" {}
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		Pass {
			CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#include "UnityCG.cginc"
			
				sampler2D _MainTex;
			
				float4 frag(v2f_img IN) : COLOR {
					#if UNITY_UV_STARTS_AT_TOP
					IN.uv.y = 1-IN.uv.y;
					#endif
				
					half4 c = tex2D (_MainTex, IN.uv);
					return c;
				}
			ENDCG
		}
	}
}
