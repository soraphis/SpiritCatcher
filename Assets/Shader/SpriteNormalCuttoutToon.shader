Shader "CustomSprite/Sprite-Normal Cutout Toon Tex" {
	Properties{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_SecColor("Sec Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_SecTex("Sec (RGB)", 2D) = "black" {}
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_BumpMap("Normalmap", 2D) = "bump" {}
	}

		SubShader{
		Tags{ 
			"Queue" = "AlphaTest"// = "Transparent" // "Queue"="AlphaTest"
			"IgnoreProjector" = "True" 
			"RenderType" = "TransparentCutout" 
			"PreviewType" = "Plane"
			//"CanUseSpriteAtlas"="True"
		}
		ZWrite Off
		Cull off
		//LOD 200
		Lighting Off
		Fog{ Mode Off }

		CGPROGRAM
#pragma surface surf ToonRamp alphatest:_Cutoff vertex:vert

		sampler2D _Ramp;

	// custom lighting function that uses a texture ramp based
	// on angle between light direction and normal
#pragma lighting ToonRamp exclude_path:prepass
	inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
	{
#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = normalize(lightDir);
#endif

		half d = dot(s.Normal, lightDir)*0.5 + 0.5;
		half3 ramp = tex2D(_Ramp, float2(d,d)).rgb;

		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
		c.a = 0;
		return c;
	}


	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _SecTex;
	float4 _Color;
	float4 _SecColor;

	struct Input {
		float2 uv_MainTex : TEXCOORD0;
		float2 uv_SecTex;
		float2 uv_BumpMap;
		float3 viewDir;
	};

	void vert(inout appdata_full v, out Input o) {
		v.normal = float3(0,0,-1);
		v.tangent = float4(1, 0, 0, -1);

		UNITY_INITIALIZE_OUTPUT(Input, o);
	}

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = ((tex2D(_SecTex, IN.uv_SecTex) * _SecColor) + tex2D(_MainTex, IN.uv_MainTex))* _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		//if (IN.viewDir.z < 0)
		//{
		//	o.Normal.b = -o.Normal.b;
		//}
	}
	ENDCG

	}

		Fallback "Transparent/VertexLit"
}