﻿// credits to https://www.shadertoy.com/view/tdlfD7

Shader "Custom/Border"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            #define PI 3.1425

            float3 remap(float2 p, float2 s, float d)
            {                
	            float2 q = abs(p);
                float n = 0.;
                
                //if point closer to left or right side
                if(q.x-s.x>q.y-s.y){
                    //how far is y coord up edge?
    	            n = q.y/s.y;//0->1
                    n = sign(p.x)>0.?sign(p.y)*n*.125+.125:sign(p.y)*-n*.125+.625;
                }
                else
                {
    	            n = q.x/s.x;
                    n= sign(p.y)>0.?sign(p.x)*-n*.125+.375:sign(p.x)*n*.125+.875;                    
                }
                
                //THIS LINE HERE CREATES THE ROUNDED CAP
                n+=sqrt(-(sin(d)*6.0)+.5);
                //n-=sqrt(-(d*d)+.05)*500.;               
                
                return frac(n*1.2-_Time.y/3.)*3.*frac(float3(.6,.2,.4));
            }


    float sdBox( in float2 p, float b)
    {
        float2 d = abs(p)-b;
        float c = length(max(d,0.0)) + min(max(d.x,d.y),0.0);
        return 1.-smoothstep(0.00,0.01,abs(c));
    }

    fixed4 frag (v2f i) : SV_Target
    {
	    float2 s = 0.1;    
        float w = 0.01;
        float d = sdBox(i.uv, 1.00);
        float3 mask = d*remap(i.uv,s,d/20.);
        
        float3 col = mask;
        return float4(col,1);

    }

       
            ENDCG
        }
    }
}