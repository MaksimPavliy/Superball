Shader "Stylized/SimpleSky"
{
    Properties
    {
        [Header(Sky Gradient)]
        _SkyGradientTop ("Top", Color) = (0.172549, 0.5686274, 0.6941177, 1)
        _SkyGradientBottom ("Bottom", Color) = (0.764706, 0.8156863, 0.8509805)
        _SkyGradientExponent ("Exponent", float) = 2.5
        _HorizonHeight("Horizon Height",Range(-1,1))=0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Background"
            "Queue" = "Background"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
        
            float3 _SkyGradientTop;
            float3 _SkyGradientBottom;
            float _SkyGradientExponent;
            float _HorizonHeight;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPosition : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float maskHorizon = dot(normalize(i.worldPosition)+float3(0,_HorizonHeight,0), float3(0, 1, 0));
                float3 skyGradientColor = lerp(_SkyGradientTop, _SkyGradientBottom, pow(1 - saturate(maskHorizon), _SkyGradientExponent));
                return float4(skyGradientColor, 1);
            }
            ENDCG
        }
    }
}
