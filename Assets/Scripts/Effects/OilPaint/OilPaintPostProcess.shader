Shader "Hidden/OilPaintPostProcess"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    #define EXCLUDE_FAR_PLANE

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

    float4 _MainTex_TexelSize;
    float4x4 unity_CameraInvProjection;

    float4x4 _InverseView;
    float3 _Point;
    uniform int _Radius; // Size of the Strokes
    uniform half _Distance; // Distance from camera
    uniform half _Thickness; // How thick the falloff is

    struct region {
        int x1, y1, x2, y2;
    };

    struct vertInput
    {
        float4 vertex : POSITION;
    };

    struct Varyings {
        float4 vertex : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        float3 ray : TEXCOORD1;
        float2 texcoordStereo : TEXCOORD5;
    };

    float3 ComputeViewSpacePosition(Varyings input)
    {
        // Render settings
        float near = _ProjectionParams.y;
        float far = _ProjectionParams.z;
        float isOrtho = unity_OrthoParams.w; // 0: perspective, 1: orthographic

        // Z buffer sample
        float z = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, input.texcoord);

        // Far plane exclusion
        #if !defined(EXCLUDE_FAR_PLANE)
        float mask = 1;
        #elif defined(UNITY_REVERSED_Z)
        float mask = z > 0;
        #else
        float mask = z < 1;
        #endif

        // Perspective: view space position = ray * depth
        float3 vposPers = input.ray * Linear01Depth(z);

        // Orthographic: linear depth (with reverse-Z support)
        #if defined(UNITY_REVERSED_Z)
        float depthOrtho = -lerp(far, near, z);
        #else
        float depthOrtho = -lerp(near, far, z);
        #endif

        // Orthographic: view space position
        float3 vposOrtho = float3(input.ray.xy, depthOrtho);

        // Result: view space position
        return lerp(vposPers, vposOrtho, isOrtho) * mask;
    }

    //smooth version of step
    float aaStep(float compValue, float gradient){
        float halfChange = fwidth(gradient) / 2;
        //base the range of the inverse lerp on the change over one pixel
        float lowerEdge = compValue - halfChange;
        float upperEdge = compValue + halfChange;
        //do the inverse interpolation
        float stepped = (gradient - lowerEdge) / (upperEdge - lowerEdge);
        stepped = saturate(stepped);
        return stepped;
    }

    Varyings vert(vertInput v, uint vertexID : SV_VertexID) {
        // Varyings o;
        // o.vertex = float4(v.vertex.xy, 0.0, 1.0);
        // o.texcoord = (v.vertex.xy + 1.0) * 0.5;

        // #if UNITY_UV_STARTS_AT_TOP
        //     o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
        // #endif

        // o.texcoordStereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);

        // Render settings
        float far = _ProjectionParams.z;
        float2 orthoSize = unity_OrthoParams.xy;
        float isOrtho = unity_OrthoParams.w; // 0: perspective, 1: orthographic

        // Vertex ID -> clip space vertex position
        float x = (vertexID != 1) ? -1 : 3;
        float y = (vertexID == 2) ? -3 : 1;
        float3 vpos = float3(x, y, 1.0);

        // Perspective: view space vertex position of the far plane
        float3 rayPers = mul(unity_CameraInvProjection, vpos.xyzz * far).xyz;

        // Orthographic: view space vertex position
        float3 rayOrtho = float3(orthoSize * vpos.xy, 0);

        Varyings o;
        o.vertex = float4(vpos.x, -vpos.y, 1, 1);
        o.texcoord = (vpos.xy + 1) / 2;
        o.texcoordStereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);
        o.ray = lerp(rayPers, rayOrtho, isOrtho);
        return o;

        return o;
    }

    float4 frag (Varyings i) : SV_Target
    {
        float2 uv = i.texcoordStereo;
        float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        float4 normalColor = col;
        float3 m[4];
        float3 s[4];

        for (int k = 0; k < 4; ++k) {
            m[k] = float3(0, 0, 0);
            s[k] = float3(0, 0, 0);
        }

        float3 vpos = ComputeViewSpacePosition(i);
        float3 wpos = mul(_InverseView, float4(vpos, 1)).xyz;
        float dist = distance(wpos, _Point);
        float _ScanDistance = 1.0;
        float _ScanWidth = 5.0;
        float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo);
        float linearDepth = Linear01Depth(rawDepth);
        if (dist < _Distance && dist > _Distance - _Distance && linearDepth < 1)
        {
            float diff = 1 - (_Distance - dist) / (_Distance);
            // return diff;
            dist = lerp(dist, diff, _Thickness);
        }
        dist = smoothstep(0.0, 1.0, dist);
        // return aaStep(0.5, dist);
        // return dist;
        int radius = lerp(1, _Radius, dist);
        // return radius / _Radius;
        float n = float((radius + 1) * (radius + 1));
        region R[4] = {
            {-radius, -radius,       0,       0},
            {       0, -radius, radius,       0},
            {       0,        0, radius, radius},
            {-radius,        0,       0, radius}
        };

        for (k = 0; k < 4; ++k) {
            [loop]
            for (int j = R[k].y1; j <= R[k].y2; ++j) {
                [loop]
                for (int i = R[k].x1; i <= R[k].x2; ++i) {
                    float3 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(i * _MainTex_TexelSize.x, j * _MainTex_TexelSize.y)).rgb;
                    m[k] += c;
                    s[k] += c * c;
                }
            }
        }

        float min = 1e+2;
        float s2;
        for (k = 0; k < 4; ++k) {
            m[k] /= n;
            s[k] = abs(s[k] / n - m[k] * m[k]);

            s2 = s[k].r + s[k].g + s[k].b;
            if (s2 < min) {
                min = s2;
                col.rgb = m[k].rgb;
            }
        }

        // depth

    
        // float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo);
        // depth = 1.0 - depth;
        // return depth;
        // depth = Linear01Depth(depth);
        // depth = pow(depth, 2.5);

        // float4 finalColor = float4(1,1,1,0);
        // float front = lerp(_Distance - _Thickness, _Distance, depth);
        // return normalColor;
        float4 finalColor = lerp(normalColor, col, dist);
        // finalColor.a = 0.0;
        return finalColor;
    }
    ENDHLSL

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite On ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }
}
