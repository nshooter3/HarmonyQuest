Shader "HarmonyQuest/ToonEnvironment"
{
    Properties {
        [Header(Texture Parameters)]
        [HDR] _Color ("Tint", Color) = (0, 0, 0, 1)
        _MainTex ("Base", 2D) = "white" {}
        _ShadowTex ("Shade", 2D) = "black" {}

        [Header(Lighting Parameters)]
        _BaseColorStep ("Base Color Step", Range(0.0, 1.0)) = 0.165
        [HDR] _Emission ("Emission", color) = (0, 0, 0, 1)
        _EmissionTex ("Emission Map", 2D) = "black" {}
        _EmissionAmount ("Emission Amount", float) = 1.0

        [Header(Shadow Parameters)]
        [HDR] _ShadowColor ("Shadow Color", Color) = (0.7, 0.7, 0.7, 1)
        _ShadowBlendScale ("Shadow Burn Scale", Range(0, 5)) = 2.0
        [Enum(None,0,Darken,1)] _BlendMode ("Shadow Blend mode", Float) = 1

        [Toggle] _UseRim("Toggle Rim", float) = 0
        _RimScale ("Rim Scale", float) = 1.0
    }

    SubShader {
        Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

        CGPROGRAM

        #pragma surface surf Stepped fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _ShadowTex;
        sampler2D _EmissionTex;
        fixed4 _Color;
        half3 _Emission;

        float4 _RimColor;
        float3 _ShadowColor;

        float _BaseColorStep;
        float _BlendMode;
        float _ShadowBlendScale;
        half _RimScale;
        float _UseRim;
        half _EmissionAmount;

        struct SurfaceOutput1
        {
            fixed3 Albedo;
            fixed3 Shadow;
            fixed3 Normal;
            fixed3 Emission;
            fixed Alpha;    // alpha for transparencies
        };

        // Helper to mix two textures in their dark values
        fixed3 Darken(fixed3 s, fixed3 d)
        {
            return min(s, d);
        }

        // Will be called once per light
        float4 LightingStepped(SurfaceOutput1 s, float3 lightDir, half3 viewDir, float shadowAttenuation) {
            // N dot L: how much does the normal point towards the light?
            float towardsLight = dot(s.Normal, lightDir);
            // Halfway between the light and the camera
            float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
            float NdotH = dot(s.Normal, halfVector);
            // make the lighting a hard cut by getting the gradient/derivative of NdotL and stepping the light
            float towardsLightChange = fwidth(towardsLight);
            float lightIntensity = smoothstep(0, towardsLightChange, towardsLight - _BaseColorStep);

        #ifdef USING_DIRECTIONAL_LIGHT
            //for directional lights, get a hard cut in the middle of the shadow attenuation
            float attenuationChange = fwidth(shadowAttenuation) * 0.5;
            float shadow = smoothstep(0.5 - attenuationChange, 0.5 + attenuationChange, shadowAttenuation);
        #else
            //for other light types (point, spot), put the cutoff near black, so the falloff doesn't affect the range
            float attenuationChange = fwidth(shadowAttenuation);
            float shadow = smoothstep(0, attenuationChange, shadowAttenuation);
        #endif
            lightIntensity = lightIntensity * shadow;

            half rim = 0.0;
            if (_UseRim)
            {
                rim = 1.0 - saturate(dot(normalize(viewDir), s.Normal) * _RimScale);
            }

            // calculate shadow color using the shadow texture and mix light and shadow based on the light. Then tint it based on the light color
            float3 shadowColor = s.Shadow * _ShadowColor;
            float3 albedo = lerp(shadowColor,  s.Albedo, lightIntensity);
            float4 color;
            color.rgb = albedo * _LightColor0.rgb + rim;
            color.a = s.Alpha;
            return color;
        }

        struct Input {
            float2 uv_MainTex;
            float2 uv_ShadowTex;
        };

        void surf (Input i, inout SurfaceOutput1 o) {
            fixed4 col = tex2D(_MainTex, i.uv_MainTex);
            col *= _Color;
            o.Albedo = col.rgb;
            o.Alpha = col.a;
            fixed3 shadowColor = tex2D(_ShadowTex, i.uv_ShadowTex).rgb;
            fixed3 shadowBlended = Darken(col.rgb, shadowColor) * _ShadowBlendScale;
            o.Shadow = lerp(shadowBlended, shadowColor, step(_BlendMode, 0));
            fixed4 emission = tex2D(_EmissionTex, i.uv_MainTex);
            o.Emission = emission * _Emission * _EmissionAmount;
        }
        ENDCG
    }

    FallBack "Standard"
}
