Shader "Unlit/UI_SoftCircleShader"
{
    Properties
    {
        _Color("Color", Color) = (0.8,0.8,0.8,1)
        _Thickness("Thickness", Float) = 0.02
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

            float4 _Color;
            float _Thickness;

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

                float radius = 0.5;

                float alpha = 1-(smoothstep(radius - _Thickness, radius, dist));

                return float4(_Color.rgb, alpha * _Color.a);
            }
            ENDCG
        }
    }
}
