// modified version of https://www.shadertoy.com/view/WsSXzt

Shader "Custom/Surface"
{
    Properties
    {
        _LinesColor ("Lines color", Color) = (0.49, 0., 0.66, 1.)
        _DotsColor ("Dots color", Color) = (0.49, 0., 0.66, 1.)
        _GridSize ("Grid Size", Float) = 10
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

            #define TRANSFORM_UV(tex,trans) (tex.xy * trans.xy + trans.zw)

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _LinesColor;
            float4 _DotsColor;
            float _GridSize;
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
                float4 color = 0.;

                float2 uvd = i.uv * 10000 / _GridSize;
                uvd = frac(uvd) - 0.5;

                // distance mask
                float dmask = 2. * length(i.uv - 0.5);
                dmask = 1. - dmask;
                dmask *= dmask;

                // wave mask
                float wmask = 0.7 + sin(_Time.z - length(i.uv) * 20.) * 0.7;

                // lines
                float lines = smoothstep(0.48, 0.5, abs(uvd.x));
                lines += smoothstep(0.48, 0.5, abs(uvd.y));
                lines += 0.5 * wmask * length(uvd.x);

                // dots
                uvd = ((0.5 + (uvd + 0.5)) % 1) - 0.5;
                float dots = smoothstep(0.1, 0.05, length(uvd));

                color += dmask * lerp(_DotsColor, _LinesColor, wmask) * (dots + lines * wmask);

                return color;
            }
            ENDCG
        }
    }
}