/*Please do support www.bitshiftprogrammer.com by joining the facebook page : fb.com/BitshiftProgrammer
Legal Stuff:
This code is free to use no restrictions but attribution would be appreciated.
Any damage caused either partly or completly due to usage of this stuff is not my responsibility*/
Shader "BitShiftProductions/VoronoiMagic3"
{
	Properties
	{
        [HDR]_MainColor ("Main Color", Color) = (1,1,1,1)
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        [HDR]_SecondColor ("Second Color", Color) = (1,1,1,1)
		_UVX("_UVX", float) = 1
		_UVY("_UVY", float) = 1
		_Mult1("Mult1", float) = 1
		_Mult2("Mult2", float) = 1
		_Mult3("Mult3", float) = 1
		_Speed("Speed", float) = 1
	}
	SubShader
	{
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200
        Cull Off

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

            fixed4 _MainColor;
            fixed4 _Color;
            fixed4 _SecondColor;
		    float _UVX;
		    float _UVY;
		    float _Mult1;
		    float _Mult2;
		    float _Mult3;
		    float _Speed;

			float2 random2(float2 p)
			{
				return frac(sin(float2(dot(p,float2(117.12,341.7)),dot(p,float2(269.5,123.3))))*43458.5453);
			}

            fixed4 VoronoiColor(float2 uv, float multiplier)
            {
				fixed4 col = fixed4(0,0,0,1);
                uv.x *= _UVX * multiplier;
                uv.y *= _UVY * multiplier;
				//uv *= 6.0; //Scaling amount (larger number more cells can be seen)
				float2 iuv = floor(uv); //gets integer values no floating point
				float2 fuv = frac(uv); // gets only the fractional part
				float minDist = 1.0;  // minimun distance
				for (int y = -1; y <= 1; y++)
				{
					for (int x = -1; x <= 1; x++)
					{
						// Position of neighbour on the grid
						float2 neighbour = float2(float(x), float(y));
						// Random position from current + neighbour place in the grid
						float2 pointv = random2(iuv + neighbour);
						// Move the point with time
						pointv = 0.5 + 0.5*sin(_Time.z * 0.2 * _Speed + 6.2236*pointv);//each point moves in a certain way
																		// Vector between the pixel and the point
						float2 diff = neighbour + pointv - fuv;
						// Distance to the point
						float dist = length(diff);
						// Keep the closer distance
						minDist = min(minDist, dist);
					}
				}
    
				// Draw the min distance (distance field)
				//col.rgb += smoothstep( 0.6, 1, minDist * minDist * minDist  * minDist  * minDist  * minDist) ; // squared it to to make edges look sharper
				//col.rgb += smoothstep(0.1, 0.8, minDist * minDist);
				col.rgb += minDist * minDist * minDist  * minDist  * minDist  * minDist ; // squared it to to make edges look sharper
				return col;
            }

			fixed4 frag(v2f i) : SV_Target
			{
                fixed4 col1 = VoronoiColor(i.uv, _Mult1);
                fixed4 col2 = VoronoiColor(i.uv, _Mult2);
                fixed4 col3 = VoronoiColor(i.uv, _Mult3);
                fixed4 resultColor = _MainColor + _Color * col1  + col2 * _SecondColor + col3 * _SecondColor;
                resultColor.a = _MainColor.a;
				return resultColor;
			}
		ENDCG
		}
	}
}