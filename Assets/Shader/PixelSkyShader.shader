Shader "Unlit/PixelSkyShader"
{
    Properties
    {
        _WidthUnits  ("Width (Units)", Float) = 10
        _HeightUnits ("Height (Units)", Float) = 20
        _PixelsPerUnit ("Pixels Per Unit", Float) = 64

        _LayerCount ("Layer Count", Range(2,12)) = 8

        _CurveAmount ("Curve Strength (UV)", Float) = 0.02
        _CurveWidth  ("Curve Width (UV)", Float) = 0.3
        _CurveOriginX ("Curve Origin X (UV)", Range(-1,2)) = 0.5

        _BottomColor ("Bottom Layer Color", Color) = (0.411, 0.525, 0.827, 1)
        _TopColor    ("Top Layer Color", Color) = (0.494, 0.655, 0.863, 1)
    }

    SubShader
    {
        Tags { "Queue"="Background" }
        ZWrite Off
        Cull Off
        Lighting Off

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
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            float _WidthUnits;
            float _HeightUnits;
            float _PixelsPerUnit;
            float _LayerCount;

            float _CurveAmount;
            float _CurveWidth;
            float _CurveOriginX;

            fixed4 _BottomColor;
            fixed4 _TopColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float pixelCountX = _WidthUnits  * _PixelsPerUnit;
                float pixelCountY = _HeightUnits * _PixelsPerUnit;

                float2 pixelUV;
                pixelUV.x = floor(i.uv.x * pixelCountX) / pixelCountX;
                pixelUV.y = floor(i.uv.y * pixelCountY) / pixelCountY;

                float x = pixelUV.x - _CurveOriginX;
                float curve = (x * x) / (abs(x) + _CurveWidth);

                float heightFactor = pow(pixelUV.y, 1.7);
                float curvedY = pixelUV.y + curve * _CurveAmount * heightFactor;

                int layerCount = (int)_LayerCount;

                int layerIndex = clamp(
                    (int)floor(curvedY * layerCount),
                    0,
                    layerCount - 1
                );

                float t = layerCount > 1
                    ? layerIndex / (float)(layerCount - 1)
                    : 0;

                return lerp(_BottomColor, _TopColor, t);
            }
            ENDCG
        }
    }
}
