Shader "Custom/RimlitExplosion" 
{
	Properties
	{
		_FresnelExponent ("Fresnel power", Range (1, 32)) = 20
		_Color ("Fresnel color", Color) = (1, 1, 1, 1)
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	float 		_FresnelExponent;
	fixed4		_Color;
	ENDCG
	
	SubShader 
	{		
		Blend SrcAlpha One
		Tags { "Queue"="Transparent" }
		
		Pass
		{		
			CGPROGRAM
			struct v2f
			{
				float4 	pos : POSITION;
				float3	viewDir : TEXCOORD0;
				float3	normal : TEXCOORD1;
			};
			
			v2f vert (appdata_tan v) // Using appdata_tan here for tangents
			{
				v2f o;
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);				
				o.viewDir = ObjSpaceViewDir(v.vertex);
				o.normal = v.normal;
				
				return o;
			}

			fixed4 frag (v2f i) : COLOR0
			{
				// Fresnel term at pixel level for the best look. 
				// Fresnel term is simply 1 minus view dot n.
				float fresnel = 1 - dot(normalize(i.viewDir), normalize(i.normal));
				fresnel = pow(fresnel, _FresnelExponent);
				
				return (fixed4)fresnel * _Color;
			}
	
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
