Shader "Custom/SpriteRendererRenderTexture"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}   // SpriteRenderer's texture
        _RenderTex("Render Texture", 2D) = "white" {} // Actual texture
        _Color("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "CanUseSpriteAtlas" = "False"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _RenderTex;
            float4 _RenderTex_TexelSize;

            fixed4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_OUTPUT(v2f, o);

                o.pos = UnityObjectToClipPos(IN.vertex);
                o.uv = IN.texcoord;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_RenderTex, i.uv);
                return c * _Color;
            }
            ENDCG
        }
    }
}
