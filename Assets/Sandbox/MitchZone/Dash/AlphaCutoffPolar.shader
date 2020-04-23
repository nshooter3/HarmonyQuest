﻿Shader "Unlit/AlphaCutoffPolar"
{
    Properties
    {
        [HDR] _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("Particle Texture", 2D) = "white" {}
        _CutOff ("Alpha Cutoff", Range(0, 1)) = 0.25
        _PanningSpeed("Panning speed (XY main texture)", Vector) = (0,0,0,0)
    }

    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend SrcAlpha OneMinusSrcAlpha

        Cull Off Lighting On ZWrite Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;

            struct appdata_t {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                fixed4 color : COLOR;
                float4 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float age : TEXCOORD1;
                UNITY_FOG_COORDS(3)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _MainTex_ST;
            half2 _PanningSpeed;
            half _CutOff;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.age = v.uv.z;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 mappedUV = (i.uv * 2) - 1;
                float2 uv = float2(atan2(mappedUV.y, mappedUV.x) / UNITY_PI / 2.0 + 0.5, length(mappedUV));
                uv += _Time.y * _PanningSpeed.xy;
                fixed4 colA = tex2D(_MainTex, uv);
                float alpha = saturate(_CutOff * colA.r) - 0.25;
                fixed4 col = i.color + _TintColor;
                col.a = alpha;
                UNITY_APPLY_FOG(i.fogCoord, col);
                clip(alpha);
                return col;
            }

            ENDCG
        }
    }
}
