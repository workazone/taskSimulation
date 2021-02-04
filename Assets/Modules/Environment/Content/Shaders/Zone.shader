Shader "Custom/Zone"
{
    Properties
    {
        _MainColor ("Main colot", Color) = (0.49, 0., 0.66, 1.)
        _UVTrans ("UV transform", Vector) = (1, 1, 0, 0)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define PI 3.1425
            #define TRANSFORM_UV(tex,trans) (tex.xy * trans.xy + trans.zw)

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

            float4 _MainColor;
            float4 _UVTrans;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_UV(v.uv, _UVTrans);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uvn = abs(i.uv);

                float mask = 0.5 + 0.3 * abs(sin(_Time.y));

                // внешние полосы
                float lines = step(0.977, uvn.x) * step(uvn.x, 1.0);
                lines += step(lines, 0.1) * step(0.977, uvn.y) * step(uvn.y, 1.0);
                lines *= 2.;

                // внутренний градиент
                lines += length(pow(uvn, 8 * mask));

                return _MainColor * lines * mask;
            }
            ENDCG
        }
    }
}