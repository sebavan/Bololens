
Shader "Bololens/EnergyPlasmaRadarToDiffuse"
{
    Properties
    {
        _Color("Tint (RGB)", Color) = (0.1,0.9,1,1)

        _MainTex("Diffuse (RGB)", 2D) = "white" {}
        _SurfaceTex("Noise (RGB)", 2D) = "white" {}
        _FadeToBlack ("FadeToBlack", Range(0.0, 1.0)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
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
                float2 uv2 : TEXCOORD1;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float4 opos : TEXCOORD0;
                float3 onorm : TEXCOORD1;

                float2 uv : TEXCOORD2;
                float2 uv2 : TEXCOORD3;

                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            sampler2D _SurfaceTex;

            float _FadeToBlack;

            v2f vert(appdata v)
            {
                v2f output;

                output.opos = v.vertex;
                output.onorm = v.normal;
                output.onorm = mul(float4(output.onorm, 0.0), unity_WorldToObject).xyz;

                output.pos = mul(UNITY_MATRIX_MVP, v.vertex);

                output.uv = TRANSFORM_TEX(v.uv, _MainTex);
                output.uv2 = output.uv;
                output.uv2.y = v.uv.y + _Time.x;

                return output;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                i.onorm = normalize(i.onorm);
                fixed3 viewDir = _WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, i.opos).xyz;

                viewDir = normalize(viewDir);
                float rampUV = float2(0.2 + abs(dot(viewDir, i.onorm)), 0.5);

                float noise = tex2D(_SurfaceTex, i.uv2).g;
                float ramp = 1 - pow(rampUV, 5);

                const float PI2 = 6.283185;
                const float PI = 3.141592;
                float lineWidth = 0.1;
                float at2 = fmod(atan2(i.opos.x, i.opos.z) + PI2, PI2);
                float mT = fmod(_Time.a * 2, PI2);

                float coef = smoothstep(mT - lineWidth * 10, mT, at2) - smoothstep(mT, mT + lineWidth, at2);

                 fixed4 col = tex2D(_MainTex, i.uv);
                 float ratio = _FadeToBlack;

                 return saturate((ratio)* saturate(((coef + ramp) * noise)) * _Color + (1 - ratio) * col + saturate(ramp) * noise * _Color);
             }
             ENDCG
         }
    }
}
