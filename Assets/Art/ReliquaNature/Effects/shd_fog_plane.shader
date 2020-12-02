Shader "Unlit/FogPlane"
{
    Properties
    {
        _Tint("Fog Tint", Color) = (1, 1, 1, .5)
        _Strength("Fog Strength", Range(0,3)) = 0.5
        _Density("Fog Density", Range(0,3)) = 0.5
        _NoiseTex("Noise Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque"  "Queue" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

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
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 scrPos : TEXCOORD2;
            };

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            float4 _Tint;
            uniform sampler2D _CameraDepthTexture; //Depth Texture
            float _Strength;
            float _Density;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeScreenPos(o.vertex); // grab position on screen
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half noise = pow(tex2D(_NoiseTex, i.uv).g, 0.5);
                half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos))); // depth
                half4 fog = (noise * _Strength * pow(depth - i.scrPos.w, _Density)); // fog by comparing depth and screenposition
                half4 col = fog * _Tint; // add the color
                col = saturate(col); // clamp to prevent weird artifacts
                UNITY_APPLY_FOG(i.fogCoord, col); // comment out this line if you want this fog to override the fog in lighting settings
                return col;
            }
            ENDCG
        }
    }
}
