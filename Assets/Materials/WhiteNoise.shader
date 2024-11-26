Shader "Custom/UIWhiteNoiseOverlay"
{
    Properties
    {
        _Alpha ("Alpha", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Alpha;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233) + _Time.y)) * 43758.5453);
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float noise = rand(i.uv * _ScreenParams.xy);
                return fixed4(noise, noise, noise, _Alpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}

