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

	// Animar posicion
    float X = Input.Position.x;
    float Y = Input.Position.y;
    float Z = Input.Position.z;
    Input.Position.y = Y * cos(time) + X * sin(time);
    Input.Position.x = X * cos(time) - Y * sin(time);

	//Proyectar posicion
    Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
    Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
    Output.Color = Input.Color;

    return (Output);
}

float frecuencia = 100;
//Pixel Shader
float4 ps_main(VS_OUTPUT Input) : COLOR0
{
    float u = Input.Texcoord.x;
    float v = Input.Texcoord.y;
    float r = distance(Input.Texcoord, float2(0.5, 0.5));
    float u2 = 0.5 + r * cos(time * 0.1);
    float v2 = 0.5 + r * sin(time * 0.1);
    float Y = Input.Texcoord.y / Input.RealPos.y;
    float dist = 1 / sqrt(pow(distance(Input.RealPos.x, 0), 2) + pow(distance(Input.RealPos.y, 0), 2));

    
    float4 Color = tex2D(diffuseMap, float2(u2, v2)) * (1 - frac(time)) * dist * 10; // * (max(0, abs(sin(time)) * Y)); //(abs(sin(time)) * Input.Texcoord.x * 5);
    return Color;
}

technique Portal
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}