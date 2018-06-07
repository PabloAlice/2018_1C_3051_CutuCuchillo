// ---------------------------------------------------------
// Shader efecto congelado
// ---------------------------------------------------------

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

float screen_dx = 1024;
float screen_dy = 768;

//Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
    Texture = (texDiffuseMap);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

float time = 0;
float bluecolor = 0;

/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
    float4 Position : POSITION0;
    float2 Texcoord : TEXCOORD0;
    float2 RealPos : TEXCOORD1;
    float4 Color : COLOR0;
};

//Vertex Shader
VS_OUTPUT vs_main(VS_INPUT Input)
{
    VS_OUTPUT Output;

    Output.RealPos = Input.Position;
	//Proyectar posicion
    Output.Position = mul(Input.Position, matWorldViewProj);
   
	//Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
    Output.Color = Input.Color;

    return (Output);
}

VS_OUTPUT vs_main2(VS_INPUT Input)
{
    VS_OUTPUT Output;

    Output.RealPos = Input.Position;

	//Proyectar posicion
    Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;

	// Animar color
    Input.Color.r = 0.74;
	Input.Color.g = 0.84;
	Input.Color.b = 0.84;
	//Propago el color x vertice
    Output.Color = Input.Color;

    return (Output);
}

float frecuencia = 10;
//Pixel Shader
float4 ps_main(VS_OUTPUT Input) : COLOR0
{
    //float y = Input.Texcoord.y * screen_dy + sin(time * frecuencia);
    //Input.Texcoord.y = y / screen_dy;
    return tex2D(diffuseMap, Input.Texcoord);
}

//Pixel Shader
float4 ps_main2(float2 Texcoord : TEXCOORD0, float4 Color : COLOR0) : COLOR0
{
	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
    float4 fvBaseColor = tex2D(diffuseMap, Texcoord);
	// combino color y textura
	// en este ejemplo combino un 25% el color de la textura y un 75%el del vertice
    return 0.25 * fvBaseColor + 0.75 * Color;
}

// ------------------------------------------------------------------
technique Unfreeze
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}

technique Freeze
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main2();
        PixelShader = compile ps_3_0 ps_main2();
    }
}