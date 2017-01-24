sampler2D inputSampler : register(S0);
float2 center : register(C0);
float4 firstColor : register(C1);
float4 secondColor : register(C2);
float4 thirdColor : register(C3);

//To compile
//"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x86\fxc.exe" /T ps_3_0 /Fo "D:\Programmation\VirtualTaluva\VirtualTaluva.Net\VirtualTaluva.Demo\Resources\AngularGradientEffect.ps"  "D:\Programmation\VirtualTaluva\VirtualTaluva.Net\VirtualTaluva.Demo\Resources\AngularGradientEffect.hlsl"

float4 main(float2 uv : TEXCOORD) : COLOR
{
    clip(tex2D(inputSampler, uv).a - 0.01);

    float4 colors[3] = { firstColor, secondColor, thirdColor };

    float2 pos = center - uv;
    float value = (atan2(pos.x, pos.y) + 3.141596) / (2.0 * 3.141596);

    float scaledValue = value * 3;
    float4 prevColor = colors[(int)scaledValue];
    float4 nextColor = colors[((int)scaledValue + 1) % 3];

    return lerp(prevColor, nextColor, scaledValue - (float)((int)scaledValue));
}