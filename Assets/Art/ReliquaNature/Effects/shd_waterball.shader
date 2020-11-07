Shader "Unlit/Waterball"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "bump" {}
        _MainColor("Main Color", Color) = (1, 1, 1, 1)
        _SecondColor("Second Color", Color) = (1, 1, 1, 1)
        _DisplacementStrength ("Displacement Strength", float) = 1.0
        _RimPower ("Rim Power", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        // ZWrite Off
        // Cull Off
        // Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.6
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            fixed4 _MainColor;
            fixed4 _SecondColor;
            float _DisplacementStrength;
            float _RimPower;

            v2f vert (appdata v)
            {
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // water deform
                float2 NCoord1 = (v.uv+float2(0.4,-0.2)*_Time.x*_DisplacementStrength);
                float3 N1 = UnpackNormal(tex2Dlod(_NoiseTex, float4(NCoord1, 1.0, 1.0)))*0.5;

                v.vertex.x += cos((v.vertex.x + _Time.y * 0.5) * 2.0) * 0.06 * _DisplacementStrength;
                v.vertex.y += cos((v.vertex.y + _Time.y * 0.5) * 3.0) * 0.06 * _DisplacementStrength;

                // o.vertex += float4(worldPosOffset, 1.0);
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.viewDir = normalize(UnityWorldSpaceViewDir(v.vertex));
                o.normal = worldNormal;
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half rim = saturate(pow(1.0 - saturate(dot(normalize(i.viewDir), i.normal)), _RimPower));
                fixed4 col = tex2D(_MainTex, i.uv) * lerp(_MainColor, _SecondColor, rim);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    Fallback "Unlit"
}
