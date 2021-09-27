//https://gist.github.com/Leland-Kwong/40edb57c87c0755eb7f5ba6f65d9c484


sampler _mainTex;
float2 sprite_size;
float4 outline_color;
float4 inside_color;
uniform bool enabled;


float4 SpritePixelShader(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(_mainTex, texCoord);

    float texelSizeX = 1.0 / sprite_size.x;
    float texelSizeY = 1.0 / sprite_size.y;

    
    float weight = 
        tex2D(_mainTex, float2(texCoord.x + texelSizeX  , texCoord.y)).a *
        tex2D(_mainTex, float2(texCoord.x               , texCoord.y - texelSizeY)).a *
        tex2D(_mainTex, float2(texCoord.x - texelSizeX  , texCoord.y)).a *
        tex2D(_mainTex, float2(texCoord.x               , texCoord.y + texelSizeY)).a;
    
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