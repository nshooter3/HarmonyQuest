Shader "Unlit/LightShaft"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "bump" {}
        _GradientTex ("Gradient Texture", 2D) = "white" {}
        [HDR] _MainColor("Main Color", Color) = (1, 1, 1, 1)
        _NoiseSpeed ("Noise Strength", float) = 1.0
        _LightLength ("Length", float) = 0.25
        _LightStrength ("Strength", float) = 0.25
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True"}
        ZWrite Off
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
                float2 uv2 : TEXCOORD2;
            };

            struct v2f
            {
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            sampler2D _GradientTex;
            float4 _GradientTex_ST;
            fixed4 _MainColor;
            float _NoiseSpeed;
            half _LightLength;
            half _LightStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _GradientTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;
                fixed noise = tex2D(_NoiseTex, i.uv + float2(_NoiseSpeed * _Time.x, 0.)).g;
                fixed noise2 = tex2D(_NoiseTex, 2.0 * i.uv + float2(_NoiseSpeed * _Time.x * 0.5, 0.)).g;
                fixed4 gradient = pow(tex2D(_GradientTex, i.uv2) * _LightStrength, _LightLength);
                col.a = noise * noise2 * gradient;
                // UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    Fallback "Standard"
}
