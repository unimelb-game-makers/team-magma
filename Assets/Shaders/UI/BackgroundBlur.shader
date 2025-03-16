Shader "UI/BackgroundBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 10)) = 2.0
        _Alpha ("Alpha", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _BlurSize;
            float _Alpha;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv) * _Alpha;

                // Gaussian blur sample offsets
                float2 offset1 = float2(_BlurSize / _ScreenParams.x, 0);
                float2 offset2 = float2(0, _BlurSize / _ScreenParams.y);

                // Gather neighboring pixels for blur effect
                float4 blur = tex2D(_MainTex, i.uv) * 0.36;
                blur += tex2D(_MainTex, i.uv + offset1) * 0.16;
                blur += tex2D(_MainTex, i.uv - offset1) * 0.16;
                blur += tex2D(_MainTex, i.uv + offset2) * 0.16;
                blur += tex2D(_MainTex, i.uv - offset2) * 0.16;

                return float4(blur.rgb, col.a);
            }
            ENDCG
        }
    }
}
