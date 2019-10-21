// Anisotropy?
float4 tfm(Texture2D img, SamplerState sampler_img, float uv)
{
    float3 st = SAMPLE_TEXTURE2D(img, sampler_img, uv).xyz;
    // st = (st - 0.5) * 2.0;
    // st = sign(st)*st*st;

    float lambda1 = 0.5 * (st.y + st.x + sqrt(st.y*st.y - 2.0*st.x*st.y + st.x*st.x + 4.0*st.z*st.z));
    float lambda2 = 0.5 * (st.y + st.x - sqrt(st.y*st.y - 2.0*st.x*st.y + st.x*st.x + 4.0*st.z*st.z));

    float2 v = float2(lambda1 - st.x, -st.z);

    float2 t = lerp(normalize(v), float2(0.0, 1.0), step(length(v), 0.0)); // If length(v) > 0

    float phi = atan(t.y / t.x);

    float A = lerp((lambda1 - lambda2) / (lambda1 + lambda2), 0.0, step(lambda1 + lambda2, 0.0)); // If lambda1 + lambda2 > 0

    return float4(t, phi, A);
}