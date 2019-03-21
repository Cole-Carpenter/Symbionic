/*
Code Edited by Matthew Carlson on 3/1/2019

Moves offset y direction
*/
Shader "Custom/UV_MOVE_Y" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Color (RGB) Alpha (A)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Transparency("Transparency",Range(0,1)) = 1
		_Occlusion("Occlusion", Range(0,1)) = 1.0
		_Speed("Speed", Range(0, 360)) = 180
		_Speed2("Speed", Range(0, 50)) = 0.8
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "ForceNoShadowCasting" = "True" }
		LOD 200

		CGPROGRAM
#include "UnityCG.cginc"
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf StandardSpecular alpha 

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
	struct Input {
		float2 uv_MainTex;
	};
	
	half _Transparency;
	half _Occlusion;
	half _Glossiness;
	half _Metallic;
	fixed4 _Color;
	float _Speed;
	float _Speed2;

	fixed4 ColorSwirl(sampler2D tex,  Input I) {

		fixed offsetY = _Speed2 * _Time;
		fixed offsetX = 0 * _Time;
		fixed2 offsetUV = fixed2(offsetX, offsetY);

		fixed2 mainUV = I.uv_MainTex - offsetUV;

		float4 c = tex2D(tex, mainUV);

		return c;
	}
	void surf(Input IN, inout SurfaceOutputStandardSpecular o) {

		fixed4 c = ColorSwirl(_MainTex, IN) * _Color;
		fixed _Transparency_Offset = sin(_Time * _Speed)*.05;
		o.Albedo = c.rgb;
		o.Smoothness = _Glossiness;
		o.Occlusion = _Occlusion;
		o.Alpha = _Transparency + _Transparency_Offset;
	}
	ENDCG
	}
		FallBack "Diffuse"
}