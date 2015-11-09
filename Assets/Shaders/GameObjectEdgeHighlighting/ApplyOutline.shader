Shader "ImageEffect/ApplyOutline" {
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
				sampler2D originalObjects;
				half4 outlineColor;
			
				float4 frag(v2f_img IN) : COLOR {
					half4 outline = tex2D (_MainTex, IN.uv);
					half4 original = tex2D (originalObjects, IN.uv);
					
					if(original.a != 0 || outline.a == 0)
						return half4(0,0,0,0);
					else
						return outlineColor;
				}
			ENDCG
		}
	}
}