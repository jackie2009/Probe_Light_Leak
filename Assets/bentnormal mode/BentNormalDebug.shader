Shader "GI_OFFSET/BentNormalDebug"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
			float4 _MainTex_ST;
			int VolDensity;
			int VolSize;
			v2f vert (appdata v)
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
				float3 wnormal = normalize(i.wnormal);
				  wnormal = normalize(i.color);
				// sample the texture
				 fixed4 col = tex3D(volTex,i.wpos / VolSize + wnormal / VolDensity / VolSize);
			 
			 
				return col;
			}
			ENDCG
		}
	}
}
