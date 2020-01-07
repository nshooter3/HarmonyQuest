Shader "Custom/VertexCrush" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Noise ("Noise (BW)", 2D) = "black" {}
        _NoiseResolution ("Noise Resolution", Float) = 1.0
        _MaskRadius("Mask Radius", float) = 1
        _MaskFalloff("Mask Falloff", float) = 1
        _Location ("Location", vector) = (0, 1, 0, 0)
        [HDR]_Emission("Emission", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" "DisableBatching" = "True"}
        LOD 200
        Cull off
 
        CGPROGRAM
        #pragma surface surf Standard addshadow vertex:vert
 
        #pragma target 3.0
 
        sampler2D _MainTex;
        sampler2D _Noise;
        float4 _Noise_ST;
 
        struct Input {
            float2 uv_MainTex;
            float2 uv_Normal;
            float mask;
        };
 
        fixed4 _Color;
        float _NoiseResolution;
        float _Progress;
        float _MaskRadius;
        fixed4 _Emission;
        float3 _Location;
        float _MaskFalloff;
        float4 _BallLocation;
        float4 _PlayerLocation;
 
        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float noise = tex2Dlod(_Noise, v.texcoord * _NoiseResolution).g;
            noise = saturate(noise * 2.0 - 1.0);
            float4 worldPos =  mul(unity_ObjectToWorld, v.vertex);
            float mask = saturate(pow(distance(worldPos, _BallLocation.xyz) / _MaskRadius, _MaskFalloff) /* - noise */);
            o.mask = mask;
            v.vertex.xyz += /* pow(2.0,  */lerp(_BallLocation - worldPos, float3(0,0,0), mask) * _Progress/* ) - 1.0 */;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = IN.mask;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}