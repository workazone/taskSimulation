// modified version of https://www.shadertoy.com/view/WsSXzt

Shader "Custom/NeonCubes"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

      
            float4 frag (v2f i) : SV_Target
            {
                // circle wave
                float wave = 0.2 + sin(_Time.y - length(i.uv) * 20.) * 0.7;

                // cells
                float2 gv = i.uv * 300.0 ;
                gv = frac(gv) - 0.5;

                // hide to edges
                float h = 1.0 - length(i.uv);
                h *= h;

                // dots
                float m = smoothstep(0.1, 0.1 - 0.01, length(gv + 0.5));
                float3 col = float3(1. - wave, 0., 0.5) * m * h;

                // grid
                m += wave * length(gv);      
                col += float3(wave, 0., 0.5) * m * h;
                
                return  float4(col, 1.0);
            }
            ENDCG
        }
    }
}