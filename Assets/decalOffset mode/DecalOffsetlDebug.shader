Shader "GI_OFFSET/DecalOffsetlDebug__"
{
	 
		Properties
		{
			_MainTex("Texture", 2D) = "white" {}
		}
			SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
				//	Name "DEFERRED"
			//Tags{ "LightMode" = "Deferred" }
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag


				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal : NORMAL;
					float3 color : COLOR;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;

					float4 vertex : SV_POSITION;
					float3 wpos : TEXCOORD1;
					float3 wnormal : TEXCOORD2;
					float3 color : COLOR;
				};

				sampler2D _MainTex;
				sampler3D volTex;
				sampler3D volOffsetTex;
				float4 _MainTex_ST;
				int VolDensity;
				int VolSize;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.wpos = mul(UNITY_MATRIX_M, v.vertex);
					o.color = v.color;
					o.wnormal = UnityObjectToWorldNormal(v.normal);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{   
				float3 offset = tex3D(volOffsetTex,(i.wpos ) / VolSize);
				float3 finalPos = i.wpos * VolDensity;
				float3 fracPos = frac(finalPos);;
		        if (offset.x > 0) finalPos.x = fracPos.x - 0.5f  > offset.x ? (int)finalPos.x + 1  : (int)finalPos.x  ;
				if (offset.y > 0) finalPos.y = fracPos.y - 0.5f  > offset.y ? (int)finalPos.y + 1  : (int)finalPos.y  ;
				if (offset.z > 0) finalPos.z = fracPos.z - 0.5f  > offset.z ? (int)finalPos.z + 1  : (int)finalPos.z  ;  
			     fixed4 col = tex3D(volTex, (finalPos +0.5  )/ VolDensity / VolSize);
				 return 1;
				  }
				  ENDCG
			  }
		}
	}
