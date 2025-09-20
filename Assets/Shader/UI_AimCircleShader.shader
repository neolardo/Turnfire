Shader "Unlit/UI_AimRingShader"
{
    Properties
    {
        _OuterColor("OuterColor", Color) = (0.8,0.8,0.8,1)
        _InnerColor("InnerColor", Color) = (0.8,0.8,0.8,1)
        _OuterThickness("OuterThickness", Float) = 0.02
        _InnerThickness("InnerThickness", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "CanvasShader"="True" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            Lighting Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _InnerColor;
            float4 _OuterColor;
            float _InnerThickness;
            float _OuterThickness;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 centeredUV = i.uv - 0.5;

                float dist = length(centeredUV);

                float radius = 0.5-_OuterThickness;

                float outerAlpha =  smoothstep(radius, radius + _OuterThickness, dist);
                float innerAlpha =  (1- smoothstep(radius - _InnerThickness, radius, dist));

                float alpha = (1- (outerAlpha + innerAlpha)) * (_InnerColor.a + _OuterColor.a)/2;
                float3 outMixColor = (1-step(radius + _OuterThickness/2, dist)) * smoothstep(radius-_InnerThickness/2, radius+_OuterThickness/2, dist) * _OuterColor.rgb;
                float3 inMixColor = step(radius - _InnerThickness/2, dist) * smoothstep(radius+_OuterThickness/2, radius-_InnerThickness/2, dist) * _InnerColor.rgb;
                float3 outColor = step(radius + _OuterThickness/2, dist) * _OuterColor.rgb;
                float3 inColor = (1-step(radius - _InnerThickness/2, dist)) * _InnerColor.rgb;
                float3 color = inColor + outColor + (inMixColor + outMixColor /2);
                return float4(color, alpha);
            }
            ENDCG
        }
    }
}
