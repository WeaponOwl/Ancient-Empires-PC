sampler2D Texture;

struct PixelInput
{
	float2 TexCoord : TEXCOORD0;
	float4 Color :COLOR0;
};

struct PixelOutput
{
	float4 Color : COLOR0;
	float Depth : DEPTH;
};

float4 PixelShaderFunction(PixelInput input) : COLOR0
{
	float2 texcoord=input.TexCoord;
	float4 color=tex2D(Texture,texcoord);
	color=input.Color*color;
	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
