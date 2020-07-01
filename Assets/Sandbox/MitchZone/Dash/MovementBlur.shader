Shader "Custom/MovementBlur"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SimpleNoise ("Simple Noise (BW)", 2D) = "black" {}
        _NoiseResolution ("Noise Resolution", Float) = 1.0
        _NoiseMulti ("Blur Multi", Float) = 1.0
        _NoisePower ("Blur Power", Float) = 1.0
        _BlurDirection ("Blur Direction", Vector) = (0,0,0)
        _BlurScale ("Blur Scale", Range(0,1)) = 1.0
        _Tess ("Tessellation", Range(1,32)) = 4
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow tessellate:tessFixed

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _SimpleNoise;
        float4 _SimpleNoise_ST;

        struct Input
        {
            float2 uv_MainTex;
        };

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        fixed4 _Color;
        float3 _PlayerVelocity;

        float _NoiseResolution;
        float _NoiseMulti;
        float _NoisePower;
        float4 _BlurDirection;
        float _BlurScale; // (0,1)
        float _Tess;
        half _Glossiness;
        half _Metallic;

        float4 tessFixed()
        {
            return _Tess;
        }


        void vert (inout appdata_full v) {
            float2 uv = TRANSFORM_TEX(v.texcoord, _SimpleNoise);
            float3 noise = tex2Dlod(_SimpleNoise, float4(uv * _NoiseResolution, 0,0));
            noise = pow(noise, _NoisePower);

            float NdotBlur = dot(v.normal, _BlurDirection);
            NdotBlur = max(NdotBlur, 0.0);
            _BlurScale = length(_PlayerVelocity) / 36.0;
            float3 blurredNoise = noise * _BlurDirection * NdotBlur * _BlurScale;

            v.vertex.xyz += blurredNoise;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
