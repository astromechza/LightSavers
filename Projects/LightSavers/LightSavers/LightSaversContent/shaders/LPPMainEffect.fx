//-----------------------------------------------------------------------------
// LPPMainEffect.fx
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//
// It uses some code from Nomal Mapping Sample found at
// http://create.msdn.com/en-US/education/catalog/sample/normal_mapping
// and also code from here
// http://aras-p.info/texts/CompactNormalStorage.html
//-----------------------------------------------------------------------------


//-----------------------------------------
// Parameters
//-----------------------------------------
float4x4 World;
float4x4 WorldView;
float4x4 View;
float4x4 Projection;
float4x4 WorldViewProjection;
float4x4 LightViewProj; //used when rendering to shadow map

float FarClip;
float2 LightBufferPixelSize;

//as we used a 0.1f scale when rendering to light buffer,
//revert it back here.
const static float LightBufferScaleInv = 10.0f;


#ifdef ALPHA_MASKED
float AlphaReference;
#endif

//-----------------------------------------
// Textures
//-----------------------------------------
texture DiffuseMap;
sampler diffuseMapSampler = sampler_state
{
	Texture = (DiffuseMap);
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture SpecularMap;
sampler specularMapSampler = sampler_state
{
	Texture = (SpecularMap);
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture NormalMap;
sampler normalMapSampler = sampler_state
{
	Texture = (NormalMap);
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture EmissiveMap;
sampler emissiveMapSampler = sampler_state
{
	Texture = (EmissiveMap);
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture LightBuffer;
sampler2D lightSampler = sampler_state
{
	Texture = <LightBuffer>;
	MipFilter = POINT;
	MagFilter = POINT;
	MinFilter = POINT;
	AddressU = Clamp;
	AddressV = Clamp;
};
//-------------------------------
// Helper functions
//-------------------------------
half2 EncodeNormal (half3 n)
{
	float kScale = 1.7777;
	float2 enc;
	enc = n.xy / (n.z+1);
	enc /= kScale;
	enc = enc*0.5+0.5;
	return enc;
}

float2 PostProjectionSpaceToScreenSpace(float4 pos)
{
	float2 screenPos = pos.xy / pos.w;
	return (0.5f * (float2(screenPos.x, -screenPos.y) + 1));
}

half3 NormalMapToSpaceNormal(half3 normalMap, float3 normal, float3 binormal, float3 tangent)
{
	normalMap = normalMap * 2 - 1;
	normalMap = half3(normal * normalMap.z + normalMap.x * tangent - normalMap.y * binormal);
	return normalMap;
}	
//-------------------------------
// Shaders
//-------------------------------

struct VertexShaderInput
{
    float4 Position  : POSITION0;
    float2 TexCoord  : TEXCOORD0;
    float3 Normal    : NORMAL0;    
    //float3 Binormal  : BINORMAL0;
    float4 Tangent   : TANGENT;
};


struct VertexShaderOutput
{
    float4 Position			: POSITION0;
    float2 TexCoord			: TEXCOORD0;
    float Depth				: TEXCOORD1;
	
    float3 Normal	: TEXCOORD2;
    float3 Tangent	: TEXCOORD3;
    float3 Binormal : TEXCOORD4; 
};

struct PixelShaderInput
{
    float4 Position			: POSITION0;
    float3 TexCoord			: TEXCOORD0;
    float Depth				: TEXCOORD1;
	
    float3 Normal	: TEXCOORD2;
    float3 Tangent	: TEXCOORD3;
    float3 Binormal : TEXCOORD4; 	
	
	//we need this to detect back bacing triangles
#ifdef ALPHA_MASKED	
	float Face : VFACE;
#endif
};
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float3 viewSpacePos = mul(input.Position, WorldView);
    output.Position = mul(input.Position, WorldViewProjection);
    output.TexCoord = input.TexCoord; //pass the texture coordinates further

	//we output our normals/tangents/binormals in viewspace
	output.Normal = normalize(mul(input.Normal,WorldView)); 
	output.Tangent =  normalize(mul(input.Tangent.xyz,WorldView)); 
	
	output.Binormal =  normalize(cross(output.Normal, output.Tangent)*input.Tangent.w);
		
	output.Depth = viewSpacePos.z; //pass depth
    return output;
}
//render to our 2 render targets, normal and depth 
struct PixelShaderOutput
{
    float4 Normal : COLOR0;
    float4 Depth : COLOR1;
};

PixelShaderOutput PixelShaderFunction(PixelShaderInput input)
{
	PixelShaderOutput output = (PixelShaderOutput)1;   

	//if we are using alpha mask, we need to read the diffuse map	
#ifdef ALPHA_MASKED
	half4 diffuseMap = tex2D(diffuseMapSampler, input.TexCoord);
	clip(diffuseMap.a - AlphaReference);	
#endif

	//read from our normal map
	half4 normalMap = tex2D(normalMapSampler, input.TexCoord);
	half3 normalViewSpace = NormalMapToSpaceNormal(normalMap.xyz, input.Normal, input.Binormal, input.Tangent);
    
	//if we are using alpha mask, we need to invert the normal if its a back face
#ifdef ALPHA_MASKED	
	normalViewSpace = normalViewSpace * sign(input.Face);
#endif

	output.Normal.rg =  EncodeNormal (normalize(normalViewSpace));	//our encoder output in RG channels
	output.Normal.b = normalMap.a;			//our specular power goes into B channel
	output.Normal.a = 1;					//not used
	output.Depth.x = -input.Depth/ FarClip;		//output Depth in linear space, [0..1]
	
	return output;
}



struct ReconstructVertexShaderInput
{
    float4 Position  : POSITION0;
    float2 TexCoord  : TEXCOORD0;
};


struct ReconstructVertexShaderOutput
{
    float4 Position			: POSITION0;
    float2 TexCoord			: TEXCOORD0;
	float4 TexCoordScreenSpace : TEXCOORD1;
};

ReconstructVertexShaderOutput ReconstructVertexShaderFunction(ReconstructVertexShaderInput input)
{
    ReconstructVertexShaderOutput output;
	
    output.Position = mul(input.Position, WorldViewProjection);
    output.TexCoord = input.TexCoord; //pass the texture coordinates further
	output.TexCoordScreenSpace = output.Position;
    return output;
}

float4 ReconstructPixelShaderFunction(ReconstructVertexShaderOutput input):COLOR0
{
	PixelShaderOutput output = (PixelShaderOutput)1;   
	// Find the screen space texture coordinate and offset it
	float2 screenPos = PostProjectionSpaceToScreenSpace(input.TexCoordScreenSpace) + LightBufferPixelSize;

	//read from our diffuse, specular and emissive maps
	half4 diffuseMap = tex2D(diffuseMapSampler, input.TexCoord);

	
#ifdef ALPHA_MASKED	
	clip(diffuseMap.a - AlphaReference);
#endif
	


	half3 emissiveMap = tex2D(emissiveMapSampler, input.TexCoord).rgb;
	half3 specularMap = tex2D(specularMapSampler, input.TexCoord).rgb;
	
	//read our light buffer texture. Remember to multiply by our magic constant explained on the blog
	float4 lightColor =  tex2D(lightSampler, screenPos) * LightBufferScaleInv;

	//our specular intensity is stored in alpha. We reconstruct the specular here, using a cheap and NOT accurate trick
	float3 specular = lightColor.rgb*lightColor.a;
	//return float4(lightColor.aaa,1);
	float4 finalColor = float4(diffuseMap*lightColor.rgb + specular*specularMap + emissiveMap,1);
	//add a small constant to avoid dark areas
	finalColor.rgb+= diffuseMap*0.1f;
	return finalColor;
}


struct ShadowMapVertexShaderInput
{
    float4 Position : POSITION0;	
	//if we have alpha mask, we need to use the tex coord
#ifdef ALPHA_MASKED	
    float2 TexCoord  : TEXCOORD0;
#endif

};

struct ShadowMapVertexShaderOutput
{
    float4 Position : POSITION0;
	float2 Depth : TEXCOORD0;
#ifdef ALPHA_MASKED	
    float2 TexCoord  : TEXCOORD1;
#endif
};



ShadowMapVertexShaderOutput OutputShadowVertexShaderFunction(ShadowMapVertexShaderInput input)
{
    ShadowMapVertexShaderOutput output = (ShadowMapVertexShaderOutput)0;
	
    float4 clipPos = mul(input.Position, mul(World, LightViewProj));
	//clamp to the near plane
	clipPos.z = max(clipPos.z,0);
	
	output.Position = clipPos;
	output.Depth = output.Position.zw;
	
#ifdef ALPHA_MASKED	
    output.TexCoord = input.TexCoord; //pass the texture coordinates further	
#endif
    return output;
}

float4 OutputShadowPixelShaderFunction(ShadowMapVertexShaderOutput input) : COLOR0
{
#ifdef ALPHA_MASKED	
	//read our diffuse
	half4 diffuseMap = tex2D(diffuseMapSampler, input.TexCoord);
	clip(diffuseMap.a - AlphaReference);
#endif

    float depth = input.Depth.x / input.Depth.y;	
    return float4(depth, 1, 1, 1); 
}

technique RenderToGBuffer
{
    pass RenderToGBufferPass
    {
	#ifdef ALPHA_MASKED	
		CullMode = None;
	#else
		CullMode = CCW;
	#endif

        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

technique ReconstructShading
{
	pass ReconstructShadingPass
    {
	#ifdef ALPHA_MASKED	
		CullMode = None;
	#else
		CullMode = CCW;
	#endif

        VertexShader = compile vs_3_0 ReconstructVertexShaderFunction();
        PixelShader = compile ps_3_0 ReconstructPixelShaderFunction();
    }
}

technique OutputShadow
{
	pass OutputShadowPass
	{		
	#ifdef ALPHA_MASKED	
		CullMode = None;
	#else
		CullMode = CCW;
	#endif

        VertexShader = compile vs_3_0 OutputShadowVertexShaderFunction();
        PixelShader = compile ps_3_0 OutputShadowPixelShaderFunction();
	}
}