Shader "HarmonyQuest/ToonSurface"
{
    Properties {
        [Header(Texture Parameters)]
        [HDR] _Color ("Tint", Color) = (0, 0, 0, 1)
        _MainTex ("Base", 2D) = "white" {}
        _RampTex ("Ramp", 2D) = "white" {}
        _NumOfColors ("Number of Colors", float) = 5.0
    }

    SubShader {
        Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

        CGPROGRAM

        #pragma surface surf Stepped fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _RampTex;

        fixed4 _Color;

        float _NumOfColors;

        struct SurfaceOutput1
        {
            fixed3 Albedo;
            fixed3 Normal;
            fixed3 Emission;
            fixed Alpha;    // alpha for transparencies
        };

        // Will be called once per light
        float4 LightingStepped(SurfaceOutput1 s, float3 lightDir, half3 viewDir, float shadowAttenuation) {
            float4 color;
            color.rgb = s.Albedo * _LightColor0.rgb /* * shadow */;
            color.a = s.Alpha;
            return color;
        }

        float3 RGBToHSV( float3 RGB ){
            float4 k = float4(0.0, -1.0/3.0, 2.0/3.0, -1.0);
            float4 p = RGB.g < RGB.b ? float4(RGB.b, RGB.g, k.w, k.z) : float4(RGB.gb, k.xy);
            float4 q = RGB.r < p.x   ? float4(p.x, p.y, p.w, RGB.r) : float4(RGB.r, p.yzx);
            float d = q.x - min(q.w, q.y);
            float e = 1.0e-10;
            return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
        }

        float3 HSVToRGB( float3 HSV ){
            float4 k = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
            float3 p = abs(frac(HSV.xxx + k.xyz) * 6.0 - k.www);
            return HSV.z * lerp(k.xxx, clamp(p - k.xxx, 0.0, 1.0), HSV.y);
        }

        struct Input {
            float2 uv_MainTex;
            float3 worldRefl;
        };

        void surf (Input i, inout SurfaceOutput1 o) {
            fixed4 col = tex2D(_MainTex, i.uv_MainTex);
            half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.worldRefl);
            float3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);
            // Posterize
            float cutColor = 1.0 / _NumOfColors;

            float3 outColor = RGBToHSV(skyColor);
            float value = outColor.b;
            float2 target_c = cutColor * floor(outColor.gb / cutColor);

            outColor = HSVToRGB(float3(outColor.r, target_c));
            fixed4 rampCol = tex2D(_RampTex, 1.0 - value);
            col *= _Color;
            o.Albedo = rampCol.rgb;
            o.Alpha = rampCol.a;
        }
        ENDCG
    }

    FallBack "Standard"
}
