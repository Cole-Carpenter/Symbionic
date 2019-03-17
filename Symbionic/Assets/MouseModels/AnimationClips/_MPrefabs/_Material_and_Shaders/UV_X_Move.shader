/*
Code Edited by Matthew Carlson on 3/1/2019

Moves offset y direction
*/
Shader "Custom/UV_X_Move" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Speed("Speed", Range(0, 3)) = 0.5
		_Speed2("Speed", Range(0, 50)) = 0.8
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#include "UnityCG.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows 

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
	struct Input {
		float2 uv_MainTex;
	};

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;
	float _Speed;
	float _Speed2;

	fixed4 ColorSwirl(sampler2D tex, Input I) {

		fixed offsetX = _Speed2 * _Time;
		fixed offsetY = 0 * _Time;
		fixed2 offsetUV = fixed2(offsetX, offsetY);

		fixed2 mainUV = I.uv_MainTex - offsetUV;

		float4 c = tex2D(tex, mainUV);

		return c;
	}
	void surf(Input IN, inout SurfaceOutputStandard o) {

		fixed4 c = ColorSwirl(_MainTex, IN) * _Color;
		o.Albedo = c.rgb;
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}