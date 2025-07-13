#pragma warning (disable : 4717) 
sampler2D image1 : register(s1);
sampler2D image2 : register(s2);
sampler2D image3 : register(s3);
float2 uImageSize1;
float uTime;
float4 uShaderSpecificData;
float3 uColor;
float3 uSecondaryColor;

float4 Flames(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords * 2.;
    uv -= 0.5;

    float4 noiseTex = tex2D(image1, uv * float2(1, 20) + float2(uTime,0) * (2 * step(uv.x, 0.5) + -1));

    noiseTex.a = noiseTex.r;

    float4 color1 = noiseTex;
    color1.rgba *= (1 / (distance(uv.x, float2(0.5, uv.x)) * 10));
    color1.rgb *= lerp(uSecondaryColor,uColor,uv.x);
    color1.rgba *= uShaderSpecificData.x;
    color1 *= 1 / length(uv.x - 0.5) * 0.5;
    return (floor(color1 * 5) / 5);
}
    
technique Technique1
{
    pass Flames
    {
        
        PixelShader = compile ps_2_0 Flames();
    }
}