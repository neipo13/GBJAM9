
sampler2D _mainTex;

float time; // must be updated to the total elapsed time
float speed; // how fast to scroll this MFer

float4x4 view_projection;
float4x4 uv_transform;

struct VertexInput {
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};
struct PixelInput {
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};

PixelInput SpriteVertexShader(VertexInput v) {
    PixelInput output;

    output.Position = mul(v.Position, view_projection);
    output.Color = v.Color;
    output.TexCoord = mul(v.TexCoord, uv_transform);
    return output;
}

float4 SpritePixelShader(PixelInput p) : COLOR0
{
    // grab the location of the texture (needs ot be in linear wrap mode so < 0 and > 1 work)
    return tex2D(_mainTex, p.TexCoord + (time * speed));
}

technique SpriteBatch
{
    pass
    {
        VertexShader = compile vs_3_0 SpriteVertexShader();
        PixelShader = compile ps_3_0 SpritePixelShader();
    }
}