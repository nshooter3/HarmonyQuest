Shader "HarmonyQuest/StylizedWater"
{
    Properties
    {
        _WaterColor("Wate rColor", Color) = (1, 1, 1, .5)
        _WaterNearColor("Water Near Color", Color) = (1, 1, 1, .5)
        _WaterFarColor("Water Far Color", Color) = (1, 1, 1, .5)
        _MainTex ("Main Texture", 2D) = "white" {}
        _LUTTex ("Water Color LUT Texture", 2D) = "white" {}
        _Noise1Tex("Wave Noise 1", 2D) = "white" {}
        _Noise2Tex("Wave Noise 2", 2D) = "white" {}
        _Normal1Tex("Surface Normal Texture 1", 2D) = "bump" {}
        _Normal2Tex("Surface Normal Texture 2", 2D) = "bump" {}
        _Speed("Wave Speed", Range(0,1)) = 0.5
        _Amount("Wave Amount", Range(0,10)) = 0.5
        _Height("Wave Height", Range(0,1)) = 0.5
        _Foam("Foamline Thickness", Range(0,5)) = 0.5
        _DistanceScale("Distance Scale",float) = 35.0
        _NormalStrength("Normal Strength", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque"  "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass
        {
            "_BackgroundTexture"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 scrPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                // these three vectors will hold a 3x3 rotation matrix
                // that transforms from tangent to world space
                half3 tspace0 : TEXCOORD3; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD4; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD5; // tangent.z, bitangent.z, normal.z
                float4 grabPos : TEXCOORD6;
                float vertexDiff : TEXCOORD7;
            };

            float4 _WaterColor, _WaterNearColor, _WaterFarColor;
            uniform sampler2D _CameraDepthTexture;
            sampler2D _BackgroundTexture;
            sampler2D _MainTex, _Noise1Tex, _Noise2Tex, _Normal1Tex, _Normal2Tex, _LUTTex;
            float4 _MainTex_ST;
            float _Speed, _Amount, _Height, _Foam, _DistanceScale, _NormalStrength;

            v2f vert (appdata v)
            {
                v2f o;
                float4 noise1 = tex2Dlod(_Noise1Tex, float4(v.uv.xy, 0, 0));
                float4 noise2 = tex2Dlod(_Noise1Tex, float4(v.uv.xy, 0, 0));
                float4 tex = noise1 * 2.0 * noise2;
                float vertexExcludeOffset = v.vertex.y;
                v.vertex.y += sin(_Time.z * _Speed + (v.vertex.x * v.vertex.z * _Amount * tex)) * _Height;
                o.vertexDiff = vertexExcludeOffset - v.vertex.y;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.scrPos = ComputeScreenPos(o.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                // compute world space view direction
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 worldBitangent = cross(worldNormal, worldTangent) * tangentSign;
                o.tspace0 = half3(worldTangent.x, worldBitangent.x, worldNormal.x);
                o.tspace1 = half3(worldTangent.y, worldBitangent.y, worldNormal.y);
                o.tspace2 = half3(worldTangent.z, worldBitangent.z, worldNormal.z);
                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv + _Time.x * float2(1.0, 1.0));

                // Foam
                half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
                half foamLine = saturate(_Foam * (depth - i.scrPos.w)); // foam line by comparing depth and screenposition
                half foamLinePowered = pow(foamLine, 0.75);
                // Reflection
                // sample the normal map, and decode from the Unity encoding
                half3 normal1 = UnpackNormal(tex2D(_Normal1Tex, i.uv + _Time.x * float2(1.0, 1.0) * _Speed));
                half3 normal2 = UnpackNormal(tex2D(_Normal2Tex, i.uv + _Time.x * float2(0.5, -1.0)* _Speed));
                half3 tnormal = lerp(normal1, normal2, 0.5) * _NormalStrength;
                // transform normal from tangent to world space
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);
                half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                half3 worldRefl = reflect(-worldViewDir, worldNormal);
                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
                half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);
                // Distance Color
                float camDist = distance(i.worldPos, _WorldSpaceCameraPos) / _DistanceScale;
                half4 waterColor = tex2D(_LUTTex, float2(camDist, 0.5));
                // Near rim mask
                float _RimSize = 0.8;
                float rim = step(1.0 - _RimSize , 1.0 - saturate(dot(normalize(worldViewDir), worldNormal)));
                // Scene color
                float2 refrOffset = worldNormal.xz /* * _RefrDistortionAmount */;
                half4 sceneColor = tex2D(_BackgroundTexture, i.scrPos.xy / i.scrPos.w - refrOffset * foamLinePowered);
                sceneColor = lerp(sceneColor, half4(0,0,0,1), foamLine);
                // Final water color
                waterColor = lerp(_WaterNearColor * (waterColor  ) * foamLinePowered, waterColor * _WaterFarColor, 0.5) + sceneColor;
                // Specular sparkle
                float sparkle = pow(saturate(pow(saturate(dot(normal1.r, normal2.r)), 0.5) * 3.3), 3800) * step(i.vertexDiff * 10.0, -1.2);

                col = waterColor * _WaterColor;
                col.rgb *= skyColor.b;
                col.rgb += sparkle;
                col.a = saturate(foamLinePowered * _Foam);
                return col;
            }
            ENDCG
        }
    }
}
