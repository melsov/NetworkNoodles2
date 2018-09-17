// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/RainbowRibbons"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SauceScale("_SauceScale", Float) = 20.0
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
			// make fog work
			//#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 norm : COLOR0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _SauceScale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex); // UnityObjectToClipPos(v.vertex);
				o.norm =UnityObjectToViewPos(v.vertex); 
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed2 offsetSecretSauce(float4 a) {
				float4 ref = abs(a);

				ref = fmod(ref, _SauceScale);
				ref = ref / _SauceScale;
				ref = floor(ref * 4);
				ref = ref / 4;
				return ref.xy;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture

				fixed2 co = offsetSecretSauce(i.vertex); // i.uv / 4;
				co = fixed2(co.x, i.uv.y);
				fixed4 col = tex2D(_MainTex, co); // i.uv);
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
