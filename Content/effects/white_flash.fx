//https://gist.github.com/Leland-Kwong/40edb57c87c0755eb7f5ba6f65d9c484


sampler2D _mainTex;
float2 sprite_size;
float4 outline_color;
float4 inside_color;


struct PixelInput {
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};

float4 SpritePixelShader(PixelInput p) : COLOR0
{
    float4 c = tex2D(_mainTex, p.TexCoord);
    float texelSizeX = 1.0 / sprite_size.x;
    float texelSizeY = 1.0 / sprite_size.y;

    
    float weight = 
        tex2D(_mainTex, float2(p.TexCoord.x + texelSizeX  , p.TexCoord.y)).a *
        tex2D(_mainTex, float2(p.TexCoord.x               , p.TexCoord.y - texelSizeY)).a *
        tex2D(_mainTex, float2(p.TexCoord.x - texelSizeX  , p.TexCoord.y)).a *
        tex2D(_mainTex, float2(p.TexCoord.x               , p.TexCoord.y + texelSizeY)).a;
    
    float4 val = lerp(outline_color, inside_color, ceil(weight));
    val.a = c.a;
    return val;
}

technique SpriteBatch
{
    pass
    {
        PixelShader = compile ps_3_0 SpritePixelShader();
    }
}