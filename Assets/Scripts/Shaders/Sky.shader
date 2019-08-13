// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Sky"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _StarsTex ("Star Texture", 2D) = "white" {}
        _BaseNoise ("Base Noise", 2D) = "white" {}
        _Distort ("Distort Noise", 2D) = "white" {}
        _SecNoise ("Secondary Noise", 2D) = "white" {}
        _SunColor ("Sun Color", Color) = (1, 1, 0)
        _SunRadius ("Sun Radius", Range(0, 1)) = 0.1
        _MoonColor ("Moon Color", Color) = (0.8, 0.8, 0.1)
        _MoonRadius ("Moon Radius", Range(0, 1)) = 0.07
        _MoonOffset ("Moon Phase", Range(-1, 1)) = 0
        _DayBottomColor ("Day Bottom Color", Color) = (0, 0, 0)
        _DayTopColor ("Day Top Color", Color) = (0, 0, 0)
        _NightBottomColor ("Night Bottom Color", Color) = (0, 0, 0)
        _NightTopColor ("Night Top Color", Color) = (0, 0, 0)
        _HorizonColorDay ("Horizon Day Color", Color) = (0, 0, 0)
        _HorizonPower ("Power of the Horizon", Range(0, 4)) = 1.0
        _StarsSpeed ("Speed of Star Movement", Float) = 0.5
        _CloudColorDayEdge ("Cloud Color Day Edge", Color) = (0, 0, 0)
        _CloudColorDayMain ("Cloud Color Day Main", Color) = (0, 0, 0)
        _CloudCutoff ("Cloud Cutoff", Float) = 1.0
        _Fuzziness ("Fuzziness", Float) = 1.0
        _Scale ("Scale of clouds", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
                
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _StarsTex;
            sampler2D _BaseNoise;
            sampler2D _Distort;
            sampler2D _SecNoise;
            float4 _MainTex_ST;
            fixed4 _SunColor;
            fixed4 _MoonColor;
            fixed4 _DayBottomColor;
            fixed4 _DayTopColor;
            fixed4 _NightBottomColor;
            fixed4 _NightTopColor;
            fixed4 _HorizonColorDay;
            fixed4 _CloudColorDayEdge;
            fixed4 _CloudColorDayMain;
            half  _SunRadius;
            half  _MoonRadius;
            half  _MoonOffset;
            half _HorizonPower;
            half _Scale;
            half _StarsSpeed;
            half _CloudCutoff;
            half _Fuzziness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // Circle test to form the sun
                float sun = distance(i.uv.xyz, _WorldSpaceLightPos0);
                float sunDisc = ceil(1 - saturate(sun / _SunRadius));
                // Circle test to form the moon
                float moon = distance(i.uv.xyz, -_WorldSpaceLightPos0);
                float moonDisc = ceil(1 - saturate(moon / _MoonRadius));
                // Another circle for the world's shadow and offset it
                float crescentMoon = distance(float3(i.uv.x + _MoonOffset, i.uv.yz), -_WorldSpaceLightPos0);
                float crescentMoonDisc = ceil(1 - saturate(crescentMoon / _MoonRadius));
                // Subtract the two to create a crescent
                float newMoonDisc = moonDisc - crescentMoonDisc;
                // Sky color
                float3 gradientDay = lerp(_DayBottomColor, _DayTopColor, saturate(i.uv.y));
                float3 gradientNight = lerp(_NightBottomColor, _NightTopColor, saturate(i.uv.y));
                float3 skyGradients = lerp(gradientNight, gradientDay, saturate(_WorldSpaceLightPos0.y));
                // Horizon
                float horizon = pow(abs(i.uv.y), _HorizonPower);
                float horizonGlow = saturate((1 - horizon) * saturate(_WorldSpaceLightPos0.y + 90.0));
                float4 horizonGlowDay = horizonGlow * _HorizonColorDay;
                // Sky plane
                float norm = 1.0 / (i.worldPos.y * sqrt(2 * 3.14159)) * 2.71828;
                float2 skyUV = i.worldPos.xz * norm;
                // Stars
                float3 stars = tex2D(_StarsTex, skyUV - _Time.x * _StarsSpeed);
                stars *= pow(1 - saturate(_WorldSpaceLightPos0.y), 8.0);

                // Cloud noise
                float scrollSpeed = 1.0;
                float baseNoise = tex2D(_BaseNoise, (skyUV -  scrollSpeed) * _Scale);
                float noise1 = tex2D(_Distort, ((skyUV + baseNoise) - scrollSpeed) * _Scale);
                float noise2 = tex2D(_SecNoise, ((skyUV + noise1 )  - scrollSpeed) * _Scale);

                float finalNoise = saturate(noise1 * noise2) * saturate(i.worldPos.y);
                float clouds = saturate(smoothstep(_CloudCutoff, _CloudCutoff + _Fuzziness, finalNoise));
                float4 cloudsColored = lerp(_CloudColorDayEdge, _CloudColorDayMain , clouds) * clouds;
                return cloudsColored;
                return saturate(sunDisc * _SunColor + newMoonDisc * _MoonColor ) + fixed4(skyGradients, 1.0) + horizonGlowDay + fixed4(stars, 1.0);
            }
            ENDCG
        }
    }
}
