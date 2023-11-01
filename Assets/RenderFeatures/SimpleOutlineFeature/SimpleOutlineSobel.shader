Shader "PostFX/SimpleOutlineSobel" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _BaseColor ("Color", Color) = (0.2, 0.2, 0.2, 1)

    _SobelThickness ("Sobel Thickness", Float) = 1

    _DepthSensitivity ("Depth Sensitivity", Float) = 1
    _NormalSensitivity ("Normal Sensitivity", Float) = 1

    _WarpingNoise ("Warping Noise", 2D) = "white" {}
    _WarpingScale ("Warping Scale", Float) = 1
    _WarpingStrength ("Warping Strength", Float) = 1
    _ThicknessWarpingStrength ("Warping Thickness Strength", Float) = 1
    _WarpingUpdateRate ("Warping FPS", Int) = 12
    _WarpingUpdateOffset ("Warping Update Offset", Vector) = (487, 683, 0, 0)
    
    _CameraWarpingFactor ("Camera Warping Factor", Float) = 1
  }

  SubShader {
    Tags { "RenderType" = "Opaque"
           "RenderPipeline" = "UniversalPipeline"
           "Queue" = "Geometry"
           "UniversalMaterialType" = "Lit" }
    // ZWrite Off Cull Off

    Pass {
      Name "ForwardLit"
      Tags { "LightMode" = "UniversalForward" }

      HLSLPROGRAM

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

      #pragma vertex vert
      #pragma fragment frag

      float4 _BaseColor;
      float _SobelThickness;

      float _DepthSensitivity;
      float _NormalSensitivity;
      
      float _WarpingScale;
      float _WarpingStrength;
      float _ThicknessWarpingStrength;
      float _WarpingUpdateRate;
      float2 _WarpingUpdateOffset;

      float _CameraWarpingFactor;

      TEXTURE2D(_MainTex);
      SAMPLER(sampler_MainTex);
      float4 _MainTex_TexelSize;
      
      TEXTURE2D(_Screen);
      SAMPLER(sampler_Screen);
      
      TEXTURE2D(_WarpingNoise);
      SAMPLER(sampler_WarpingNoise);

      static float2 uvOffset[2] = {
        float2(1, 1),
        float2(1, -1)
      };

      struct VertexInput {
        float4 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
      };

      struct VertexOutput {
        float4 positionCS : SV_POSITION;
        float4 posScreen  : TEXCOORD0;
      };

      float getMaskedDifference(float a, float b, float maskA, float maskB) {
        return (a - b) * maskA * maskB;
      }

      float3 getMaskedDifference(float3 a, float3 b, float maskA, float maskB) {
        return (a - b) * maskA * maskB;
      }

      float depthSobel(float4 pos0, float4 pos1, float4 pos2, float4 pos3, float2 uv, float2 pixelStep) {
        float depth0 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + uvOffset[0] * pixelStep).r;
        float depth1 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv - uvOffset[0] * pixelStep).r;
        float depth2 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + uvOffset[1] * pixelStep).r;
        float depth3 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv - uvOffset[1] * pixelStep).r;

        float depthDifference0 = getMaskedDifference(depth1, depth0, pos1.a, pos0.a);
        float depthDifference1 = getMaskedDifference(depth3, depth2, pos3.a, pos2.a);
        float edgeDepth = sqrt(pow(depthDifference0, 2) + pow(depthDifference1, 2)) * 100;
        float depthThreshold = (1/_DepthSensitivity) * depth0;
        return step(depthThreshold, edgeDepth);
      }

      float normalSobel(float4 pos0, float4 pos1, float4 pos2, float4 pos3) {
        float3 normalDifference0 = getMaskedDifference(pos1.rgb, pos0.rgb, pos1.a, pos0.a);
        float3 normalDifference1 = getMaskedDifference(pos3.rgb, pos2.rgb, pos3.a, pos2.a);
        float edgeNormal = sqrt(dot(normalDifference0, normalDifference0) + dot(normalDifference1, normalDifference1));
        return step(1/_NormalSensitivity, edgeNormal);
      }

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

        o.positionCS = vertexInput.positionCS;
        o.posScreen = ComputeScreenPos(vertexInput.positionCS);
        return o;
      }

      half4 frag(VertexOutput i) : SV_TARGET {
        float warpUpdateSeconds = 1/_WarpingUpdateRate;
        float timeOffset = floor(_Time.y / warpUpdateSeconds);
        float2 warpNoise = SAMPLE_TEXTURE2D(_WarpingNoise, sampler_WarpingNoise, i.posScreen.xy*float2(_MainTex_TexelSize.y/_MainTex_TexelSize.x, 1)*_WarpingScale+timeOffset*_WarpingUpdateOffset).rg;
        float2 warpedScreenPos = i.posScreen.xy + (warpNoise*2-1) * _WarpingStrength * _CameraWarpingFactor;
        float thicky = abs(warpNoise.x - warpNoise.y) * _CameraWarpingFactor;

        float2 pixelStep = _MainTex_TexelSize.xy * _SobelThickness + thicky * _ThicknessWarpingStrength;
        float4 pos0 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, warpedScreenPos + uvOffset[0] * pixelStep);
        float4 pos1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, warpedScreenPos - uvOffset[0] * pixelStep);
        float4 pos2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, warpedScreenPos + uvOffset[1] * pixelStep);
        float4 pos3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, warpedScreenPos - uvOffset[1] * pixelStep);
        float mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.posScreen.xy).a;

        float depthSobelRes = depthSobel(pos0, pos1, pos2, pos3, warpedScreenPos, pixelStep);
        float normalSobelRes = normalSobel(pos0, pos1, pos2, pos3);
        half4 screen = SAMPLE_TEXTURE2D(_Screen, sampler_Screen, i.posScreen.xy);
        half4 color = half4(0, 0, 0, 1);
        float t = saturate(max(depthSobelRes, normalSobelRes));
        color.rgb = lerp(screen.rgb, _BaseColor.rgb, mask * t * _BaseColor.a);
        // color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).a;
        return color;
      }

      ENDHLSL
    }
  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
