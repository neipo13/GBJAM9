
sampler2D _mainTex;
float4x4 _ColorMatrix;


float4 frag(float2 texCoord : TEXCOORD0) : COLOR0
{
    //all colors are r=g=b so just grab one
    float c = tex2D(_mainTex, texCoord).r;
    //x * 3 floored will give 0, 1, 2, 3 which are indexes to our color matrix
    return _ColorMatrix[c * 3];
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 frag();
    }
}