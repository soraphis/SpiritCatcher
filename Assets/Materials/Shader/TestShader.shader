Shader "Unlit/TestShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		//_Color("Tint", Color) = (1,1,1,1)
		_AlphaColorKey("Alpha Color Key", Color) = (0,0,0,0)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 1
	}
	SubShader
	{
		Tags { 
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="TransparentCutOut"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas" = "True"
		}
		ZWrite Off
		LOD 200
		Cull Off
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
		Fog{ Mode Off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile DUMMY PIXELSNAP_ON
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _AlphaColorKey;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);

#ifdef PIXELSNAP_ON
				o.vertex = UnityPixelSnap(o.vertex);
#endif

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				//*
				float2 dropValues = float2(	floor(i.uv.y) * 1,
											floor(i.uv.x) * 1);

				fixed4 col = tex2D(_MainTex, i.uv.xy + dropValues);
				//*/
				//half4 col = tex2D(_MainTex, i.uv);
				

				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				/*
				if (_AlphaColorKey.a == 1 &&
					_AlphaColorKey.r == col.r &&
					_AlphaColorKey.g == col.g &&
					_AlphaColorKey.b == col.b)
				{
					col.a = 0;
				}*/

				return col;
			}
			ENDCG
		}
	}
			Fallback "Transparent/Cutout/VertexLit"
}