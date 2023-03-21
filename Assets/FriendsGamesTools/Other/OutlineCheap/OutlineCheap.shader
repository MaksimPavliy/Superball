Shader "Mobile/OutlineCheap"
{
	Properties
	{
		_OutlineColor("Outline Color", Color) = (0, 1, 0, 1)
		_Outline("Outline", Float) = 0.1
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 150

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front
			ZWrite On

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				uniform half4 _OutlineColor;
				uniform half _Outline;
				struct vertexInput
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};
				struct vertexOutput
				{
					float4 pos : SV_POSITION;
				};
				vertexOutput vert(vertexInput v)
				{
					vertexOutput o;
					float3 vertex = v.vertex + v.normal * _Outline;
					o.pos = UnityObjectToClipPos(vertex);
					return o;
				}
				half4 frag(vertexOutput i) : COLOR
				{
					return _OutlineColor;
				}
			ENDCG
		}
	}
	Fallback "Mobile/VertexLit"
}
