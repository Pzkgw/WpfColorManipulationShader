float Script : STANDARDSGLOBAL <
    string UIWidget = "none";
    string ScriptClass = "scene";
    string ScriptOrder = "postprocess";
    string ScriptOutput = "color";
    string Script = "Technique=Main;";
> = 0.8;
/*
#ifndef WPF
float GainR <
    string UIWidget = "slider";
    float UIMin = 0.0f;
    float UIMax = 2.0f;
    float UIStep = 0.1f;
    string UIName = "GainR";
> = 0.5f;
#else
float GainR : register(c0);
#endif

#ifndef WPF
float GainG <
    string UIWidget = "slider";
    float UIMin = 0.0f;
    float UIMax = 2.0f;
    float UIStep = 0.1f;
    string UIName = "GainG";
> = 0.5f;
#else
float GainG : register(c1);
#endif

#ifndef WPF
float GainB <
    string UIWidget = "slider";
    float UIMin = 0.0f;
    float UIMax = 2.0f;
    float UIStep = 0.1f;
    string UIName = "GainB";
> = 0.5f;
#else
float GainB : register(c2);
#endif

#ifndef WPF
float Saturation <
    string UIWidget = "slider";
    float UIMin = 0.0f;
    float UIMax = 2.0f;
    float UIStep = 0.1f;
    string UIName = "Saturation";
> = 0.5f;
#else
float Saturation : register(c3);
#endif

#ifndef WPF
float Brightness <
    string UIWidget = "slider";
    float UIMin = -300.0f/255.f;
    float UIMax = 300.0f/255.f;
    float UIStep = 0.1f;
    string UIName = "Brightness";
> = 0.5f;
#else
float Brightness : register(c4);
#endif

#ifndef WPF
float Contrast <
    string UIWidget = "slider";
    float UIMin = -1.0f;
    float UIMax = 1.0f;
    float UIStep = 0.1f;
    string UIName = "Contrast";
> = 0.5f;
#else
float Contrast : register(c5);
#endif

#ifndef WPF
float Gamma <
    string UIWidget = "slider";
    float UIMin = 0.0f;
    float UIMax = 2.0f;
    float UIStep = 0.1f;
    string UIName = "Gamma";
> = 0.5f;
#else
float Gamma : register(c6);
#endif

#ifndef WPF
float AverageR <
    string UIWidget = "slider";
    float UIMin = 0.0f;
    float UIMax = 2.0f;
    float UIStep = 0.1f;
    string UIName = "AverageR";
> = 0.5f;
#else
float AverageR : register(c7);
#endif

#ifndef WPF
float AverageG <
    string UIWidget = "slider";
    float UIMin = 0.0f;
    float UIMax = 2.0f;
    float UIStep = 0.1f;
    string UIName = "AverageG";
> = 0.5f;
#else
float AverageG : register(c8);
#endif

#ifndef WPF
float AverageB <
    string UIWidget = "slider";
    float UIMin = 0.0f;
    float UIMax = 2.0f;
    float UIStep = 0.1f;
    string UIName = "AverageB";
> = 0.5f;
#else
float AverageB : register(c9);
#endif
*/
texture implicitTexture : RENDERCOLORTARGET < 
    float2 ViewPortRatio = {1.0,1.0}; 
    int MipLevels = 1; 
    string Format = "A8R8G8B8";
    string UIWidget = "None"; 
>; 	

#ifndef WPF
sampler2D implicitSampler = sampler_state { 
    texture = implicitTexture; 
    AddressU = Clamp; 
    AddressV = Clamp; 
    MagFilter = Linear; 
    MipFilter = POINT; 
    MinFilter = LINEAR; 
    MagFilter = LINEAR; 
};
#else
sampler2D implicitSampler : register(s0);
#endif


texture ClrLut  <
	  string ResourceName = "";//Optional default file name
	  string UIName =  "ClrLut Texture";
	  string ResourceType = "2D";
	  >;

#ifndef WPF
sampler2D ClrLutSampler = sampler_state { 
    texture = ClrLut; 
    AddressU = Clamp; 
    AddressV = Clamp; 
    MagFilter = Linear; 
    MipFilter = POINT; 
    MinFilter = LINEAR; 
    MagFilter = LINEAR; 
};
#else
sampler2D ClrLutSampler : register(s1);
#endif

#ifndef WPF
float Saturation <
    string UIWidget = "slider";
    float UIMin = 0.0f;
    float UIMax = 2.0f;
    float UIStep = 0.1f;
    string UIName = "Saturation";
> = 0.5f;
#else
float Saturation : register(c0);
#endif

texture zBuffer : RENDERDEPTHSTENCILTARGET < 
    float2 ViewPortRatio = {1.0,1.0}; 
    string Format = "D24S8"; 
    string UIWidget = "None"; 
>; 

struct VSOut {
    float4 Pos	: POSITION;
    float2 UV	: TEXCOORD0;
};

VSOut ShdColorEfectVS (
    float3 Position : POSITION, 
    float3 TexCoord : TEXCOORD0 ) {
    VSOut output;
    output.Pos 	= float4(Position, 1.0);
    output.UV 	= TexCoord.xy;
    return output;
}

/*
float4 ShdColorEfectPS(float2 uv : TEXCOORD0) : COLOR {
	
	float3 LuminanceWeights = float3(0.299,0.587,0.114);//required for gray calc
    float4 srcPixel = tex2D(implicitSampler, uv);//get input pixed form input texture
	
	srcPixel = pow(srcPixel, 1/Gamma);//Apply gamma
	srcPixel.rgb *= float3(GainR, GainG, GainB);//Apply RGB gain
	srcPixel.rgb += Brightness;//Apply brightness
  
    float	gray = dot(srcPixel.rgb, LuminanceWeights);//gray(luminance) calc
    float4	dstPixel = lerp(gray, srcPixel, Saturation);//Apply saturation using linear interpolation
	
    // --------------  Apply contrast ---------------
	//Subtract the mean from every color value,
	//causing the distribution to be shifted to the left,
	//with a new mean value of zero.	
    dstPixel.rgb -= float3(AverageR, AverageG, AverageB);
	
	//Multiply every color value by the same scale factor.
	dstPixel.rgb *= Contrast;
	
	//Add the original mean value
	dstPixel.rgb += float3(AverageR, AverageG, AverageB);	
	
	dstPixel.a = srcPixel.a;
	
    return dstPixel;
}*/


float4 ShdColorEfectPS(float2 uv : TEXCOORD0) : COLOR {
	
	float3  LuminanceWeights = float3(0.299,0.587,0.114);
	
    float4 srcPixel = tex2D(implicitSampler, uv);//get input pixed form input texture
	srcPixel.r = tex2D(ClrLutSampler, float2(srcPixel.r, 0)).r;
	srcPixel.g = tex2D(ClrLutSampler, float2(srcPixel.g, 0)).g;
	srcPixel.b = tex2D(ClrLutSampler, float2(srcPixel.b, 0)).b;
	
	
    float    luminance = dot(srcPixel.rgb, LuminanceWeights);
    float4   dstPixel = lerp(luminance, srcPixel, Saturation);
    dstPixel.a = srcPixel.a;
	
    return dstPixel;
}


float4 	clearColour 	= {0,0,0,0};
float 	clearDepth  	= 1.0;
technique Main < string Script =
    "RenderColorTarget0=implicitTexture;"
    "RenderDepthStencilTarget=zBuffer;"
    "ClearSetColor=clearColour;"
    "ClearSetDepth=clearDepth;"
	"Clear=Color;"
	"Clear=Depth;"
    "ScriptExternal=color;"
    "Pass=PostP0;";
> {
    pass PostP0 < string Script =
	"RenderColorTarget0=;"
	"RenderDepthStencilTarget=;"
	"Draw=Buffer;";
    > {
	VertexShader = compile vs_2_0 ShdColorEfectVS();
		ZEnable = false;
		ZWriteEnable = false;
		AlphaBlendEnable = false;
		CullMode = None;
	PixelShader = compile ps_2_0 ShdColorEfectPS();
    }
}
