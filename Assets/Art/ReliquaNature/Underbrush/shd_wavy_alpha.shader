Shader "Unlit/WavyAlpha"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _MainColor("Main Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags {"Queue"="Transparent-1"}
        ZWrite On
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha
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
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD2;
                UNITY_FOG_COORDS(1)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _MainColor;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.x += cos((v.vertex.x + _Time.y * 0.5) * 2.0) * 0.06;
                // v.vertex.y += cos((v.vertex.y + _Time.y * 0.5) * 3.0) * 0.06;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;
                clip(col.a-0.5);
                // UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    Fallback "Standard"
}
