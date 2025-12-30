Shader "Custom/DrawLineFragment"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _LineColor ("Line Color", Color) = (1,0,0,1)
        _Thickness ("Thickness", Float) = 2
        _FadeOut ("Fade Out End", Float) = 0
        _PointCount ("Point Count", Int) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            #define MAX_POINTS 128

            sampler2D _MainTex;
            fixed4 _LineColor;
            float _Thickness;
            float _FadeOut;
            int _PointCount;
            fixed4 _Size;

            float4 _Points[MAX_POINTS]; // xy = pixel coords

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // snap to pixel grid
                float2 p = floor(i.uv * _Size.xy) + 0.5;

                if (_PointCount < 2)
                    return fixed4(0,0,0,0);

                float minDist = 1e9;
                float pathLength = 0;
                float closestDistAlong = 0;

                for (int i = 0; i < _PointCount - 1; i++)
                    pathLength += distance(_Points[i].xy, _Points[i+1].xy);

                float accum = 0;

                for (int j = 0; j < _PointCount - 1; j++)
                {
                    float2 a = _Points[j].xy;
                    float2 b = _Points[j+1].xy;

                    float2 ab = b - a;
                    float2 ap = p - a;

                    float t = saturate(dot(ap, ab) / dot(ab, ab));
                    float2 proj = a + ab * t;
                    float d = distance(p, proj);

                    if (d < minDist)
                    {
                        minDist = d;
                        closestDistAlong = accum + t * length(ab);
                    }

                    accum += length(ab);
                }

                // hard pixel cutoff
                if (round(minDist) > floor(_Thickness * 0.5))
                    return fixed4(0,0,0,0);

                float alpha = _LineColor.a;

                if (_FadeOut > 0.5 && pathLength > 0)
                    alpha *= 1.0 - (closestDistAlong / pathLength);

                return fixed4(_LineColor.rgb, alpha);
            }

            ENDCG
        }
    }
}
