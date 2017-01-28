sampler2D inputSampler : register(S0);
float2 center : register(C0);
float4 firstColor : register(C1);
float4 secondColor : register(C2);
float4 thirdColor : register(C3);

//http://stackoverflow.com/questions/41798927/3-colors-anglegradient-in-wpf/
//To compile
//"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x86\fxc.exe" /T ps_3_0 /Fo "D:\Programmation\VirtualTaluva\VirtualTaluva.Net\VirtualTaluva.Demo\Resources\AngularGradientEffect.ps"  "D:\Programmation\VirtualTaluva\VirtualTaluva.Net\VirtualTaluva.Demo\Resources\AngularGradientEffect.hlsl"

float4 main(float2 uv : TEXCOORD) : COLOR
{
    // Put the three colors into an array.
    float4 colors[3] = { firstColor, secondColor, thirdColor };

    // Figure out where this pixel is in relation to the center point
    float2 pos = center - uv;

    // Compute the angle of this pixel relative to the center (in radians),
    // then divide by 2 pi to normalize the angle into a 0 to 1 range.
    // We are flipping the Y here so that 0 is at the top (instead of the bottom) and we
    // rotate clockwise. Could also flip X if we want to rotate counter-clockwise.
    float value = (atan2(pos.x, -pos.y) + 3.141596) / (2.0 * 3.141596);

    // Scale the angle based on the size of our array and determine which indices
    // we are currently between, wrapping around to 0 at the end.
    float scaledValue = value * 3;
    float4 prevColor = colors[(int)scaledValue];
    float4 nextColor = colors[((int)scaledValue + 1) % 3];

    // Figure out how far between the two colors we are
    float lerpValue = scaledValue - (float)((int)scaledValue);

    // Get the alpha of the incoming pixel from the sampler.
    float alpha = tex2D(inputSampler, uv).a;

    // Lerp between the colors. Multiply each color by its own alpha and the result by the
    // incoming alpha becuse WPF expects shaders to return premultiplied alpha pixel values.
    return float4(
        lerp(prevColor.rgb * prevColor.a, nextColor.rgb * nextColor.a, lerpValue) * alpha,
        lerp(prevColor.a, nextColor.a, lerpValue) * alpha);
}