Shader "ImageEffect/ScreenSpaceWriteId" {
	SubShader {
		ZTest LEqual  Cull Off ZWrite On Fog { Mode Off }
		Pass {
			CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#include "UnityCG.cginc"
			
				float id;
			
				float4 frag(v2f_img IN) : COLOR 
				{
					return float4(id,0,0,1);
				}
			ENDCG
		}
	}
}
