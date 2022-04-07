Shader "GI_OFFSET/OffsetCube"
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
			 zwrite off
             ztest gequal			 
			 cull front
          
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
				float4 screenUV : TEXCOORD0;
			    float3 ray : TEXCOORD1;
 				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _CameraDepthTexture;

			uniform float4 offsetDir;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenUV = ComputeScreenPos(o.vertex);
		        o.ray=UnityObjectToViewPos(v.vertex)*float3(-1,-1,1);
				return o;
			}
			float4 frag(v2f i) :SV_Target
			{
				i.ray=i.ray*(_ProjectionParams.z/i.ray.z);
				float2 uv=i.screenUV.xy/i.screenUV.w;


				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);

				// 要转换成线性的深度值 //
				depth = Linear01Depth (depth);
				
				float4 vpos = float4(i.ray * depth,1);
				float3 wpos = mul (unity_CameraToWorld, vpos).xyz;
				float3 opos = mul (unity_WorldToObject, float4(wpos,1)).xyz;
				clip (float3(0.5,0.5,0.5) - abs(opos.xyz));

				return   offsetDir;
					 
				}
		 
			ENDCG
		}
	}
}
