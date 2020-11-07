Shader "HarmonyQuest/Environment/TreeLeaves"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Color 2", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Texture", 2D) = "black" {}
        _CloudSpeed ("Cloud Speed", float) = 1.0
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimScale ("Rim Scale", float) = 1.0
        _BaseColorStep ("Base Color Step", Range(0.0, 1.0)) = 0.165
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry"}
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD1;
                half lighting : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };

            fixed4 _Color1;
            fixed4 _Color2;
            fixed4 _RimColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            half _CloudSpeed;
            half _RimScale;
            half _BaseColorStep;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                float uv_scroll_speed = _Time.x * _CloudSpeed;
                float offset = tex2Dlod(_NoiseTex, float4(o.uv + float2(uv_scroll_speed, -uv_scroll_speed), 0.0, 0.0)).g;
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal * offset);
                o.normal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                o.color = v.color;
                o.lighting = max(0, dot(o.normal, _WorldSpaceLightPos0.xyz));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half NdotLChange = fwidth(i.lighting);
                half lighting = smoothstep(0, NdotLChange, i.lighting - _BaseColorStep);
                half rim = 1.0 - saturate(dot(normalize(i.viewDir), i.normal) * _RimScale);
                rim = smoothstep(_RimScale - 0.01, _RimScale + 0.01, rim);
                // rim *= i.lighting;
                fixed4 gradient = lerp(_Color2, _Color1, lighting);
                fixed4 col = /* i.color *  */gradient * tex2D(_MainTex, i.uv) + _RimColor * rim;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Standard"
}
