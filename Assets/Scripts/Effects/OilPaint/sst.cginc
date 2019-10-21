float4 sst(Texture2D img, SamplerState sampler_img, float uv)
{
    float2 d = 1.0 / _ScreenParams.xy;
    
    float3 c = SAMPLE_TEXTURE2D(img, sampler_img, uv).xyz;
    float3 u = (
           -1.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2(-d.x, -d.y)).xyz +
           -2.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2(-d.x,  0.0)).xyz + 
           -1.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2(-d.x,  d.y)).xyz +
           +1.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2( d.x, -d.y)).xyz +
           +2.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2( d.x,  0.0)).xyz + 
           +1.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2( d.x,  d.y)).xyz
           ) / 4.0;

    float3 v = (
           -1.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2(-d.x, -d.y)).xyz + 
           -2.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2( 0.0, -d.y)).xyz + 
           -1.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2( d.x, -d.y)).xyz +
           +1.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2(-d.x,  d.y)).xyz +
           +2.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2( 0.0,  d.y)).xyz + 
           +1.0 * SAMPLE_TEXTURE2D(img, sampler_img, uv + float2( d.x,  d.y)).xyz
           ) / 4.0;

    return float4(dot(u, u), dot(v, v), dot(u, v), 1.0);
}