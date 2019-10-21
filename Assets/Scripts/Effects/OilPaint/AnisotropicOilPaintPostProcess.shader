Shader "Hidden/AnisotropicOilPaintPostProcess"
{
    HLSLINCLUDE

    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    #include "tfm.cginc"
    #include "sst.cginc"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float4 _MainTex_ST;
    float4 _MainTex_TexelSize;
    Texture2D _CameraDepthTexture; SamplerState sampler_CameraDepthTexture;
    float4 _CameraDepthTexture_ST;

    uniform sampler2D _K0123;

    uniform int _Radius;
    uniform half _Distance;
    uniform half _Thickness;
    uniform half _Alpha;
    uniform half _Q;

    const int N = 8;

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
        float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        float4 normalColor = col;
        float4 m[8];
        float3 s[8];
        int k = 0;

        UNITY_LOOP for (k = 0; k < N; ++k) {
            m[k] = float4(0.0, 0.0, 0.0, 0.0);
            s[k] = float3(0.0, 0.0, 0.0);
        }
        float4 sst1 = sst(_MainTex, sampler_MainTex, uv);
        return sst1;
        float4 t = tfm(_MainTex, sampler_MainTex, uv);
        return t;
        float a = _Radius * clamp((_Alpha + t.w) / _Alpha, 0.1, 2.0); 
        float b = _Radius * clamp(_Alpha / (_Alpha + t.w), 0.1, 2.0);

        float cos_phi = cos(t.z);
        float sin_phi = sin(t.z);

        float2x2 R = float2x2(cos_phi, -sin_phi, sin_phi, cos_phi);
        float2x2 S = float2x2(0.5/a, 0.0, 0.0, 0.5/b);
        float2x2 SR = S * R;

        int max_x = int(sqrt(a*a * cos_phi*cos_phi +
                            b*b * sin_phi*sin_phi));
        int max_y = int(sqrt(a*a * sin_phi*sin_phi +
                            b*b * cos_phi*cos_phi));

        float3 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb;
        float w = tex2D(_K0123, float2(0.5, 0.5)).x;
        for (k = 0; k < N; ++k) {
            m[k] += float4(c * w, w);
            s[k] += c * c * w;
        }

        UNITY_LOOP for (int j = 0; j <= max_y; ++j)  {
            UNITY_LOOP for (int i = -max_x; i <= max_x; ++i) {
                if ((j != 0) || (i > 0)) {
                    float2 v = mul(SR, float2(i, j));

                    if (dot(v,v) <= 0.25) {
                        float3 c0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(i,j)/_MainTex_TexelSize).rgb;
                        float3 c1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv - float2(i,j)/_MainTex_TexelSize).rgb;

                        float3 cc0 = c0 * c0;
                        float3 cc1 = c1 * c1;

                        float4 w0123 = tex2D(_K0123, float2(0.5, 0.5) + v);
                        UNITY_LOOP for (k = 0; k < 4; ++k) {
                            m[k] += float4(c0 * w0123[k], w0123[k]);
                            s[k] += cc0 * w0123[k];
                        }
                        UNITY_LOOP for (k = 4; k < 8; ++k) {
                            m[k] += float4(c1 * w0123[k-4], w0123[k-4]);
                            s[k] += cc1 * w0123[k-4];
                        }

                        float4 w4567 = tex2D(_K0123, float2(0.5, 0.5) - v);
                        UNITY_LOOP for (k = 4; k < 8; ++k) {
                            m[k] += float4(c0 * w4567[k-4], w4567[k-4]);
                            s[k] += cc0 * w4567[k-4];
                        }
                        UNITY_LOOP for (k = 0; k < 4; ++k) {
                            m[k] += float4(c1 * w4567[k], w4567[k]);
                            s[k] += cc1 * w4567[k];
                        }
                    }
                }
            }
        }

        float4 o = float4(0.0, 0.0, 0.0, 0.0);
        for (k = 0; k < N; ++k) {
            m[k].rgb /= m[k].w;
            s[k] = abs(s[k] / m[k].w - m[k].rgb * m[k].rgb);

            float sigma2 = s[k].r + s[k].g + s[k].b;
            float w = 1.0 / (1.0 + pow(255.0 * sigma2, 0.5 * _Q));

            o += float4(m[k].rgb * w, w);
        }

        return float4(o.rgb / o.w, 1.0);

        // depth
        float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoordStereo);
        depth = 1.0 - depth;
        // depth = Linear01Depth(depth);
        // depth = pow(depth, 2.5);

        float front = lerp(_Distance - _Thickness, _Distance, depth);

        float4 finalColor = lerp(normalColor, col, front);

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
