Shader "HarmonyQuest/ToonSurface"
{
    Properties {
        [Header(Texture Parameters)]
        [HDR] _Color ("Tint", Color) = (0, 0, 0, 1)
        _MainTex ("Base", 2D) = "white" {}
        _ShadowTex ("Shade", 2D) = "black" {}

        [Header(Lighting Parameters)]
        _BaseColorStep ("Base Color Step", Range(0.0, 1.0)) = 0.165
        [HDR] _Emission ("Emission", color) = (0, 0, 0, 1)

        [Header(Shadow Parameters)]
        [HDR] _ShadowColor ("Shadow Color", Color) = (0.7, 0.7, 0.7, 1)
        _ShadowBlendScale ("Shadow Burn Scale", Range(0, 5)) = 2.0
        [Enum(None,0,Darken,1)] _BlendMode ("Shadow Blend mode", Float) = 1

        [Header(Rim Light Parameters)]
        [HDR] _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimAmount ("Rim Amount", Range(0, 1)) = 0.6
        _RimThreshold ("Rim Threshold", Range(0, 1)) = 0.6

        [Header(Outline Parameters)]
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline width", Range (.002, 2)) = 0.15
    }

    SubShader {
        Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
                UNITY_FOG_COORDS(0)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                float3 norm   = normalize(mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                float2 offset = TransformViewToProjection(norm.xy);
                o.vertex.xy += offset * o.vertex.z * _OutlineWidth;

                o.color = _OutlineColor;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_APPLY_FOG(i.fogCoord, i.color);
                return i.color;
            }
            ENDCG
        }

        CGPROGRAM

        #pragma surface surf Stepped fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _ShadowTex;
        fixed4 _Color;
        half3 _Emission;

        float4 _RimColor;
        float3 _ShadowColor;

        float _BaseColorStep;
        float _BlendMode;
        float _ShadowBlendScale;
        float _RimThreshold;
        float _RimAmount;

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
            
            // Rim light calculations that only appear on the lit side
            float4 rimDot = 1.0 - dot(viewDir, s.Normal);
            float rimIntensity = rimDot * pow(towardsLight * 0.5 + 0.5, _RimThreshold); // Remap NDotL to (0.0,1.0) to not have hilight rim in the shadow
            rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
            float4 rim = rimIntensity * _RimColor;

            // calculate shadow color using the shadow texture and mix light and shadow based on the light. Then tint it based on the light color
            float3 shadowColor = s.Shadow * _ShadowColor;
            float3 albedo = lerp(shadowColor,  (s.Albedo + rim.rgb), lightIntensity);
            float4 color;
            color.rgb = albedo * _LightColor0.rgb;
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
            o.Emission = _Emission;
        }
        ENDCG
    }

    FallBack "Standard"
}
