Shader "Hidden/OilPaintPostProcess"
{
    Properties
    {
        _Radius ("Size of the Strokes", Range(0, 10)) = 3
    }

    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float4 _MainTex_ST;
    float4 _MainTex_TexelSize;
    Texture2D _CameraDepthTexture; SamplerState sampler_CameraDepthTexture;
    float4 _CameraDepthTexture_ST;

    uniform int _Radius;
    uniform half _Distance;
    uniform half _Thickness;

    struct region {
        int x1, y1, x2, y2;
    };

    struct vertInput
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct Varyings {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD0;
        float2 texcoord : TEXCOORD0;
        float2 texcoordStereo : TEXCOORD5;
    };

    Varyings vert(vertInput v) {
        // Varyings o;
        // o.vertex = float4(v.vertex.xy, 0.0, 1.0);
        // o.uv = v.uv;
        // float2 vertToUV = TransformTriangleVertexToUV(v.vertex.xy);
        // #if UNITY_UV_STARTS_AT_TOP
        //     vertToUV = vertToUV * float2(1.0, -1.0) + float2(0.0, 1.0);
        // #endif
        // o.texcoord = UnityStereoScreenSpaceUVAdjust(vertToUV, _MainTex_ST);
        // o.texcoordStereo = TransformStereoScreenSpaceTex(vertToUV, 1.0);
        // return o;
        Varyings o;
        o.vertex = float4(v.vertex.xy, 0.0, 1.0);
        o.texcoord = (v.vertex.xy + 1.0) * 0.5;

        #if UNITY_UV_STARTS_AT_TOP
            o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
        #endif

        o.texcoordStereo = TransformStereoScreenSpaceTex(o.texcoord, 1.0);

        return o;
    }

    float4 frag (Varyings i) : SV_Target
    {
        float2 uv = i.texcoordStereo;
        float n = float((_Radius + 1) * (_Radius + 1));
        float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        float4 normalColor = col;
        float3 m[4];
        float3 s[4];

        for (int k = 0; k < 4; ++k) {
            m[k] = float3(0, 0, 0);
            s[k] = float3(0, 0, 0);
        }

        region R[4] = {
            {-_Radius, -_Radius,       0,       0},
            {       0, -_Radius, _Radius,       0},
            {       0,        0, _Radius, _Radius},
            {-_Radius,        0,       0, _Radius}
        };

        for (int k = 0; k < 4; ++k) {
            for (int j = R[k].y1; j <= R[k].y2; ++j) {
                for (int i = R[k].x1; i <= R[k].x2; ++i) {
                    float3 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(i * _MainTex_TexelSize.x, j * _MainTex_TexelSize.y)).rgb;
                    m[k] += c;
                    s[k] += c * c;
                }
            }
        }

        float min = 1e+2;
        float s2;
        for (int k = 0; k < 4; ++k) {
            m[k] /= n;
            s[k] = abs(s[k] / n - m[k] * m[k]);

            s2 = s[k].r + s[k].g + s[k].b;
            if (s2 < min) {
                min = s2;
                col.rgb = m[k].rgb;
            }
        }

        // depth
        float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo);
        depth = 1.0 - depth;
        // depth = Linear01Depth(depth);
        // depth = pow(depth, 2.5);

        // float4 finalColor = float4(1,1,1,0);
        float front = lerp(_Distance - _Thickness, _Distance, depth);

        // if ( edge )
        float4 finalColor = lerp(normalColor, col, front);
        // finalColor.a = 0.0;
        // return depth;
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
