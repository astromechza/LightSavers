float4x4 World;
float4x4 View;
float4x4 Projection;
float3 CameraPosition;

float4 AmbientLightColour;


Texture2D CurrentTexture;
sampler2D TextureSampler = sampler_state
{
	Texture = <CurrentTexture>;
	MinFilter = linear;
	MagFilter = linear;
	MipFilter = linear;
};


// The incoming structure is VertexPositionNormalTexture.
struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL;
	float2 TextureCoord : TEXCOORD0;
};

// The output structure contains normalised interpolated versions 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 Normal : TEXCOORD0;
	float2 TextureCoord : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	// transform coordinate into world transform (Matrix.Indentity)
	float4 worldPosition = mul(input.Position, World);
	// transform worldposition into position in view
    float4 viewPosition = mul(worldPosition, View);
	// project position onto screen
    output.Position = mul(viewPosition, Projection);
	// transform normal by world matrix
	float3 normal = normalize(mul(input.Normal, World));
    output.Normal = normal;

	output.TextureCoord = input.TextureCoord;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 textureColor = tex2D(TextureSampler, input.TextureCoord);
    return textureColor;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
