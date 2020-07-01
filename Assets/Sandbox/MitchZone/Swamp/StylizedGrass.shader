Shader "Custom/StylizedGrass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FurMaskTex("Fur Mask", 2D) = "white" {}
        _SideFurTex("Side Fur", 2D) = "white" {}
        _FurTex("Fur Texture", 2D) = "white" {}
        _Distortation("Texture", 2D) = "white" {}
        _Size("Size of Fur", Range(0, 1)) = 0.5
        _FurLayers("Layers of Fur", Range(0,30)) = 0.5
        _Comb("Comb fur", Vector) = (0,-1,0,0)
        _CombStrength("Comb Strength", Range(0,1)) = 0.25
        _OuterMaskHeight("Outer Mask Height", float) = 1.0
        _OuterMaskStrength("Outer Mask Strength", float) = 1.0
        _WindStrength("Wind Strength", float) = 0.25
    }
    SubShader
    {

        Tags { "RenderType"="Opaque"  "LightMode" = "ForwardBase" }
        LOD 100

        //Mesh
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };


            struct v2g
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = mul(unity_WorldToObject, v.normal);;
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
            {
                float4 v0 = IN[0].vertex;
                float4 v1 = IN[1].vertex;
                float4 v2 = IN[2].vertex;

                g2f pIn;

                pIn.vertex = v0;
                pIn.uv = IN[0].uv;
                pIn.normal = IN[0].normal;
                triStream.Append(pIn);

                pIn.vertex = v1;
                pIn.uv = IN[0].uv;
                pIn.normal = IN[0].normal;
                triStream.Append(pIn);

                pIn.vertex = v2;
                pIn.uv = IN[0].uv;
                pIn.normal = IN[0].normal;
                triStream.Append(pIn);
                triStream.RestartStrip();

            }

            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                return col;
            }
            ENDCG
        }

        //Shells
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
        LOD 100
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2g
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float layer : TEXCOORD1;
                float color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _FurTex;
            sampler2D _FurMaskTex;
            sampler2D _Distortation;
            float4 _MainTex_ST;
            float _Size;
            float _FurLayers;
            float4 _Comb;
            float _CombStrength;
            float _OuterMaskHeight;
            float _OuterMaskStrength;
            float _WindStrength;

            float sdBox( in float2 p, in float2 b )
            {
                float2 d = abs(p)-b;
                return length(max(d,float2(0,0))) + min(max(d.x,d.y),0.0);
            }

            v2g vert(appdata v)
            {
                v2g o;

                o.vertex = v.vertex;
                o.normal = v.normal;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            [maxvertexcount(16 * 3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
            {
                float4 v0 = IN[0].vertex;
                float4 v1 = IN[1].vertex;
                float4 v2 = IN[2].vertex;

                float4 n0 = float4(IN[0].normal, 0);
                float4 n1 = float4(IN[1].normal, 0);
                float4 n2 = float4(IN[2].normal, 0);

                // float4x4 vp = UnityObjectToClipPos(unity_WorldToObject);

                float4 d0;
                float4 d1;
                float4 d2;

                g2f pIn;

                float4 displacement;
                float heightOffset;
                for (int i = 0; i < _FurLayers; i++) {

                    d0 = float4(lerp(n0, _Comb * _CombStrength + n0 * (1 - _CombStrength), i / _FurLayers).xyz, 0);
                    d1 = float4(lerp(n1, _Comb * _CombStrength + n1 * (1 - _CombStrength), i / _FurLayers).xyz, 0);
                    d2 = float4(lerp(n2, _Comb * _CombStrength + n2 * (1 - _CombStrength), i / _FurLayers).xyz, 0);

                    heightOffset = _Size * i / _FurLayers;

                    displacement = d0 * heightOffset;
                    pIn.vertex = UnityObjectToClipPos(v0 + displacement);
                    pIn.uv = IN[0].uv;
                    pIn.normal = n0;
                    pIn.layer = i;
                    pIn.color = float(1.0 - i / _FurLayers).xxx;
                    triStream.Append(pIn);

                    displacement = d1 * heightOffset;
                    pIn.vertex = UnityObjectToClipPos(v1 + displacement);
                    pIn.uv = IN[1].uv;
                    pIn.normal = n1;
                    pIn.layer = i;
                    pIn.color = float(1.0 - i / _FurLayers).xxx;
                    triStream.Append(pIn);

                    displacement = d2 * heightOffset;
                    pIn.vertex = UnityObjectToClipPos(v2 + displacement);
                    pIn.uv = IN[2].uv;
                    pIn.normal = n2;
                    pIn.layer = i;
                    pIn.color = float(1.0 - i / _FurLayers).xxx;
                    triStream.Append(pIn);

                    triStream.RestartStrip();
                }
            }

            fixed4 frag(g2f i) : SV_Target
            {
                // Wind
                float2 dis = tex2D(_Distortation,i.uv  *0.6+ _Time.xx*3.3);
                float displacementStrengh = 0.22* (((sin(_Time.y) + sin(_Time.y*0.5 + 1.051))/4.0) +0.5f);
                dis = dis * displacementStrengh * (1.0 - i.color.xx) * _WindStrength;

                // Edge mask
                float d = sdBox(i.uv + float2(-0.5, -0.5), float2(_OuterMaskHeight, _OuterMaskHeight)) * _OuterMaskStrength;
                // d = distance(i.uv, float2(0.5, 0.5)) - _OuterMaskHeight; // circle

                fixed4 col = tex2D(_MainTex, i.uv * 2.0 + dis.xy);
                fixed4 furPattern = tex2D(_FurTex, i.uv * 2.0 + dis.xy);
                fixed4 mask = tex2D(_FurMaskTex, i.uv * 2.0 + dis.xy);

                col.a = furPattern.r * mask.r - d;

                if (col.a <= .1 * i.layer.x) discard;

                return col;
            }

            ENDCG
        }


        // //Fins
        // Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
        // LOD 100
        // Cull Off
        // ZWrite Off
        // Blend SrcAlpha OneMinusSrcAlpha
        // Pass
        // {
        // CGPROGRAM
        // #pragma vertex vert
        // #pragma geometry geom
        // #pragma fragment frag


        // #include "UnityCG.cginc"

        // struct appdata
        // {
        // 	float4 vertex : POSITION;
        // 	float2 uv : TEXCOORD0;
        // 	float3 normal : NORMAL;
        // };


        // struct v2g
        // {
        // 	float2 uv : TEXCOORD0;
        // 	float4 vertex : SV_POSITION;
        // 	float3 normal : NORMAL;
        // };

        // struct g2f
        // {
        // 	float2 uv : TEXCOORD0;
        // 	float2 originalUv : TEXCOORD1;
        // 	float4 vertex : SV_POSITION;
        // };

        // sampler2D _MainTex;
        // sampler2D _SideFurTex;
        // sampler2D _FurMaskTex;
        // float4 _MainTex_ST;
        // float _Size;
        // float _FurLayers;
        // float4 _Comb;

        // v2g vert(appdata v)
        // {
        // 	v2g o;
        // 	o.vertex = v.vertex;
        // 	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        // 	o.normal = v.normal;
        // 	return o;
        // }

        // [maxvertexcount(4)]
        // void geom(line v2g IN[2], inout TriangleStream<g2f> triStream)
        // {
        // 	// float4x4 vp = UnityObjectToClipPos(unity_WorldToObject);

        // 	float3 eyeVec = normalize(((IN[0].vertex -_WorldSpaceCameraPos) + (IN[1].vertex - _WorldSpaceCameraPos)) / 2);
        // 	float4 lineNormal = float4(normalize((IN[0].normal + IN[1].normal) / 2), 0)  * 0.15;
        // 	float eyeDot = dot(lineNormal, eyeVec);

        // 	float3 newNormal = normalize(cross(IN[1].vertex- IN[0].vertex, lineNormal));
        // 	float maxOffset = 0.25f;

        // 	if (eyeDot < maxOffset && eyeDot > -maxOffset)
        // 	{

        // 		g2f pIn;

        // 		pIn.vertex = UnityObjectToClipPos(IN[1].vertex);
        // 		pIn.uv = float2(1, 0);
        // 		pIn.originalUv = IN[1].uv;
        // 		triStream.Append(pIn);

        // 		pIn.vertex = UnityObjectToClipPos(IN[1].vertex + lineNormal);
        // 		pIn.uv = float2(1, 1);
        // 		pIn.originalUv = IN[1].uv;
        // 		triStream.Append(pIn);

        // 		pIn.vertex = UnityObjectToClipPos(IN[0].vertex);
        // 		pIn.uv = float2(0, 0);
        // 		pIn.originalUv = IN[0].uv;
        // 		triStream.Append(pIn);

        // 		pIn.vertex = UnityObjectToClipPos(IN[0].vertex + lineNormal);
        // 		pIn.uv = float2(0, 1);
        // 		pIn.originalUv = IN[0].uv;
        // 		triStream.Append(pIn);

        // 		triStream.RestartStrip();
        // 	}

        // }

        // fixed4 frag(g2f i) : SV_Target
        // {
        // 	fixed4 originalCol = tex2D(_MainTex, i.originalUv);
        // 	fixed4 col = tex2D(_SideFurTex, i.uv);
        // 	fixed4 mask = tex2D(_FurMaskTex, i.originalUv);

        // 	if (mask.r <= .5) discard;

        // 	return originalCol * col;
        // }

        // ENDCG
        // }

    }
}
