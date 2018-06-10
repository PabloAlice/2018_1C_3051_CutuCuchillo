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

    float4x4 rotation;
    float3x1 origin;
    origin[0][0] = 0;
    origin[1][0] = 0;
    origin[2][0] = 0;
    float suma = distance(Input.Position.xyz, origin);

    rotation[0][0] = cos(time + suma);
    rotation[0][1] = sin(time + suma);
    rotation[0][2] = 0;
    rotation[0][3] = 0;

    rotation[1][0] = -sin(time + suma);
    rotation[1][1] = cos(time + suma);
    rotation[1][2] = 0;
    rotation[1][3] = 0;

    rotation[2][0] = 0;
    rotation[2][1] = 0;
    rotation[2][2] = 1;
    rotation[2][3] = 0;

    rotation[3][0] = 0;
    rotation[3][1] = 0;
    rotation[3][2] = 0;
    rotation[3][3] = 1;

    float4 rotationMat = mul(Input.Position, rotation);

    Output.Position = mul(rotationMat, matWorldViewProj);

    Output.Texcoord = Input.Texcoord;

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