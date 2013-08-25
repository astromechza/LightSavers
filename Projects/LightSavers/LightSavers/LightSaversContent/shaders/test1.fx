float4x4 World;
float4x4 View;
float4x4 Projection;
float3 CameraPosition;


// ========= LIGHTS ==========
float4 AmbientLightColour;

// = PointLight0
bool PointLight0Enabled;
float4 PointLight0Diffuse;
float3 PointLight0Position;
float4 PointLight0Attenuation;    

// = PointLight1
bool PointLight1Enabled;
float4 PointLight1Diffuse;
float3 PointLight1Position;
float4 PointLight1Attenuation;    

// = PointLight2
bool PointLight2Enabled;
float4 PointLight2Diffuse;
float3 PointLight2Position;
float4 PointLight2Attenuation; 

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
	float3 WorldPosition : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	// transform coordinate into world transform (Matrix.Indentity)
	float4 worldPosition = mul(input.Position, World);
	output.WorldPosition = worldPosition;
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


	float4 textureColour = tex2D(TextureSampler, input.TextureCoord);
	float4 ambientColour = AmbientLightColour;

	float4 pointLightContribution = float4(0,0,0,1.0);

	float lightIntensity;
	float3 lightpos;

	if (PointLight0Enabled)
	{
		float3 pointToLightDif = PointLight0Position - input.WorldPosition;
		float3 pointToLightDifNormal = normalize(pointToLightDif);

		float NDotL = dot(input.Normal, pointToLightDifNormal);
		if (NDotL > 0)
		{
			float LD = length(pointToLightDif);
			if(LD <= PointLight0Attenuation.x)
			{
				pointLightContribution += PointLight0Diffuse * NDotL * 1.0f/(PointLight0Attenuation.y + LD*PointLight0Attenuation.z + LD*LD*PointLight0Attenuation.w);
			}
		}
	}

	if (PointLight1Enabled)
	{
		float3 pointToLightDif = PointLight1Position - input.WorldPosition;
		float3 pointToLightDifNormal = normalize(pointToLightDif);

		float NDotL = dot(input.Normal, pointToLightDifNormal);
		if (NDotL > 0)
		{
			float LD = length(pointToLightDif);
			if(LD <= PointLight1Attenuation.x)
			{
				pointLightContribution += PointLight1Diffuse * NDotL * 1.0f/(PointLight1Attenuation.y + LD*PointLight1Attenuation.z + LD*LD*PointLight1Attenuation.w);
			}
		}
	}

	if (PointLight2Enabled)
	{
		float3 pointToLightDif = PointLight2Position - input.WorldPosition;
		float3 pointToLightDifNormal = normalize(pointToLightDif);

		float NDotL = dot(input.Normal, pointToLightDifNormal);
		if (NDotL > 0)
		{
			float LD = length(pointToLightDif);
			if(LD <= PointLight2Attenuation.x)
			{
				pointLightContribution += PointLight2Diffuse * NDotL * 1.0f/(PointLight2Attenuation.y + LD*PointLight2Attenuation.z + LD*LD*PointLight2Attenuation.w);
			}
		}
	}

	// add in ALL the light components
    return saturate(pointLightContribution + ambientColour) * textureColour;
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
