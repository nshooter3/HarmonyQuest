// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Cloud"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Texture", 2D) = "black" {}
        _CloudSpeed ("Cloud Speed", float) = 1.0
        _RimScale ("Rim Scale", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            half _CloudSpeed;
            half _RimScale;

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
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half rim = 1.0 - saturate(dot(normalize(i.viewDir), i.normal) * _RimScale);
                rim *= i.lighting;
                fixed4 col = i.color * tex2D(_MainTex, i.uv) + rim;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Standard"
}
