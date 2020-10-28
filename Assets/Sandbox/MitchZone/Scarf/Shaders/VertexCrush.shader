Shader "Custom/VertexCrush" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Noise ("Noise (BW)", 2D) = "black" {}
        _NoiseResolution ("Noise Resolution", Float) = 1.0
        _MaskRadius("Mask Radius", float) = 1
        _MaskFalloff("Mask Falloff", float) = 1
        _Location ("Location", vector) = (0, 1, 0, 0)
        _Progress2 ("Progress", float) = 0
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
        float _Progress2;
        float _MaskRadius;
        fixed4 _Emission;
        float3 _Location;
        float _MaskFalloff;
        float4 _BallLocation;
        float4 _BallRotation;
        float4 _PlayerRotation;

        // A given angle of rotation about a given axis
        float4 rotate_angle_axis(float angle, float3 axis)
        {
            float sn = sin(angle * 0.5);
            float cs = cos(angle * 0.5);
            return float4(axis * sn, cs);
        }
        // Quaternion multiplication
        // http://mathworld.wolfram.com/Quaternion.html
        float4 qmul(float4 q1, float4 q2)
        {
            return float4(
                q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz),
                q1.w * q2.w - dot(q1.xyz, q2.xyz)
            );
        }
        // Vector rotation with a quaternion
        // http://mathworld.wolfram.com/Quaternion.html
        float3 rotate_vector(float3 v, float4 r)
        {
            float4 r_c = r * float4(-1, -1, -1, 1);
            return qmul(r, qmul(float4(v, 0), r_c)).xyz;
        }

        void vert (inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input,o);
            float noise = tex2Dlod(_Noise, v.texcoord * _NoiseResolution).g;
            noise = saturate(noise * 2.0 - 1.0);
            float4 worldPos =  mul(unity_ObjectToWorld, v.vertex);
            // float angle = acos(dot(_BallForward, _PlayerLocation));
            float mask = saturate(pow(distance(worldPos, _BallLocation.xyz) / _MaskRadius, _MaskFalloff) /* - noise */);
            o.mask = mask;
            v.vertex.xyz += /* pow(2.0,  */lerp(rotate_vector(_BallLocation - worldPos, _PlayerRotation), float3(0,0,0), mask) * _Progress/* ) - 1.0 */;
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
