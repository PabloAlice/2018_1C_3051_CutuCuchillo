//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

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

float time;

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
    float4 Color : COLOR0;
};

VS_OUTPUT vs_main(VS_INPUT Input)
{
    VS_OUTPUT Output;

    /*
    float Y = Input.Position.y;
    float X = Input.Position.x;
    float1x1 origin = (0);
    float suma = time + distance(Input.Position.y, origin);
    Input.Position.y = Y * cos(suma) - X * sin(suma);
    Input.Position.x = X * cos(suma) + Y * sin(suma);
    */
    Output.Position = mul(Input.Position, matWorldViewProj);

    Output.Texcoord = Input.Texcoord + time;

    Output.Color = Input.Color;

    return (Output);
}

float4 ps_main(float2 Texcoord : TEXCOORD0, float4 Color : COLOR0) : COLOR0
{
    return tex2D(diffuseMap, Texcoord);
}

technique Portal
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}