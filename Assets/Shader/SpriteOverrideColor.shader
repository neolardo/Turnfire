Shader "Custom/SpriteOverrideColor"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Sprite"
                "CanUseSpriteAtlas" = "True"
            }

            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float4 color  : COLOR; 
                    float2 uv     : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float4 color  : COLOR; 
                    float2 uv     : TEXCOORD0;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    o.color = v.color;

                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 texColor = tex2D(_MainTex, i.uv);

                    float originalAlpha = texColor.a;

                    float t = i.color.a;

                    fixed3 finalRGB = lerp(texColor.rgb, i.color.rgb, t);
                    
                    return fixed4(finalRGB, originalAlpha);
                }
            ENDCG
        }
    }
}
