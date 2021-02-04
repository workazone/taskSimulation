Shader "Custom/Horizont"
{
    Properties
    {
        _MainColor ("Main color", Color) = (0.49, 0., 0.66, 1.)
        _Values ("Values", Vector) = (1, 1, 0, 0)
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _MainColor;
            float4 _UVTrans;
            float4 _Values;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_UV(v.uv, _UVTrans);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float gradient = smoothstep(_Values.x, _Values.y, i.uv.y);
                gradient -= smoothstep(_Values.z, _Values.w, i.uv.y);
                return float4(_MainColor.rgb, gradient * _MainColor.a);
            }
            ENDCG
        }
    }
}