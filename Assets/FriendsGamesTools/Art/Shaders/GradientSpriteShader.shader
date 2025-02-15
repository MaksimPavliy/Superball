﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "HCShaders/GradientSpriteShader"
{
	Properties{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	_Color("Left Color", Color) = (1,1,1,1)
		_Color2("Right Color", Color) = (1,1,1,1)
		_Scale("Scale", Float) = 1
	}
		SubShader{
		Tags{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
	}
		LOD 100

		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
				  ZTest on
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		fixed4 _Color;
	fixed4 _Color2;
	fixed  _Scale;

	struct v2f {
		float4 pos : SV_POSITION;
		fixed4 col : COLOR;
	};

	v2f vert(appdata_full v)
	{

		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.col = lerp(_Color,_Color2, v.texcoord.x);
		return o;
	}
	float4 frag(v2f i) : COLOR{
		float4 c = i.col;
		return c;
	}


		ENDCG
	}
	}

}