﻿Shader "Bololens/EnergyPlasma" {
    Properties{
        _Color("Tint (RGB)", Color) = (1,1,1,1)
        _SurfaceTex("Noise (RGB)", 2D) = "white" {}
        _RampTex("Alpha Gradient (RGB)", 2D) = "white" {}
        _FadeIn("Fade In", Range(0.0, 1.0)) = 1.0
    }
        SubShader{
        ZWrite Off
        Tags{ "Queue" = "Transparent" }
        Blend One One
        Cull Off

        Pass{
        CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_fog_exp2
#include "UnityCG.cginc" 

        struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float2 uv2 : TEXCOORD1;
        float3 normal : TEXCOORD2;

        float4 opos : TEXCOORD3;
    };

    float _Offset;

    v2f vert(appdata_base v) {
        _Offset = _Time.x;

        v2f o;
        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

        float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
        v.texcoord.x = v.texcoord.x;
        v.texcoord.y = v.texcoord.y + _Offset;
        o.uv = TRANSFORM_UV(1);
        o.uv2 = float2(abs(dot(viewDir, v.normal)), 0.5);
        o.normal = v.normal;
        o.opos = v.vertex;
        return o;
    }

    uniform float4 _Color;
    uniform sampler2D _RampTex : register(s0);
    uniform sampler2D _SurfaceTex : register(s1);

    uniform float _FadeIn;

    half4 frag(v2f f) : COLOR
    {
        f.normal = normalize(f.normal);

        half4 ramp = tex2D(_RampTex, f.uv2) * _Color.a;

        half4 thisTex = tex2D(_SurfaceTex, f.uv) * ramp * _Color;


        return half4 (thisTex.r, thisTex.g, thisTex.b, ramp.r) * _FadeIn;
    }

        ENDCG

        SetTexture[_RampTex]{ combine texture }
        SetTexture[_SurfaceTex]{ combine texture }
    }
    }
        Fallback "Transparent/VertexLit"
}