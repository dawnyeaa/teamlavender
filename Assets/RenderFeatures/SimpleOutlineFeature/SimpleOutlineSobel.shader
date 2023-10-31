Shader "PostFX/SimpleOutlineSobel" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _BaseColor ("Color", Color) = (0.2, 0.2, 0.2, 1)

    _SobelThickness ("Sobel Thickness", Float) = 1
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
      
      TEXTURE2D(_MainTex);
      SAMPLER(sampler_MainTex);
      float4 _MainTex_TexelSize;
      
      TEXTURE2D(_Screen);
      SAMPLER(sampler_Screen);
      float4 _Screen_TexelSize;

      static float2 uvOffset[9] = {
        float2(0, 0),
        float2(-1, -1),
        float2(0, -1),
        float2(1, -1),
        float2(-1, 0),
        float2(1, 0),
        float2(-1, 1),
        float2(0, 1),
        float2(1, 1)
      };
      
      static float3 offsetIDs[9] = {
        float3(1, 1, 1),
        float3(1, 0, 0),
        float3(0, 1, 0),
        float3(0, 0, 1),
        float3(1, 1, 0),
        float3(0, 1, 1),
        float3(1, 0, 1),
        float3(1, 0.5, 0),
        float3(0, 1, 0.5),
      };

      struct VertexInput {
        float4 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
      };

      struct VertexOutput {
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
      };

      float intensity(in float4 color) {
        return sqrt((color.x*color.x) + (color.y*color.y) + (color.z*color.z));
      }

      float sobelish(float2 uv, float stepx, float stepy) {
        float current = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(0, 0)));
        float right   = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(stepx, 0)));
        float bottom  = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(0, stepy)));
        float bright  = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(stepx, stepy)));

        float x = current - bright;
        float y = right - bottom;
        float mag = sqrt((x*x) + (y*y));
        return mag;
      }

      float2 actuallySobel(float2 uv, float stepx, float stepy) {
        float tleft  = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(-stepx, -stepy)));
        float top    = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(0, -stepy)));
        float tright = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(stepx, -stepy)));
        float left   = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(-stepx, 0)));
        float right  = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(stepx, 0)));
        float bleft  = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(-stepx, stepy)));
        float bottom = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(0, stepy)));
        float bright = intensity(SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(stepx, stepy)));

        float x = (1*tleft + 1*bleft + (2*left)) - (1*tright + 1*bright + (2*right));
        float y = (1*tleft + 1*tright + (2*top)) - (1*bleft + 1*bright + (2*bottom));
        float mag = sqrt((x*x) + (y*y));
        return float2(mag, (atan2(y, x)/(2*PI))+0.5);
      }

      float normalSobel(float2 uv, float stepx, float stepy) {
        float3 tleft  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(-stepx, -stepy)).rgb;
        float3 top    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(0, -stepy)).rgb;
        float3 tright = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(stepx, -stepy)).rgb;
        float3 left   = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(-stepx, 0)).rgb;
        float3 right  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(stepx, 0)).rgb;
        float3 bleft  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(-stepx, stepy)).rgb;
        float3 bottom = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(0, stepy)).rgb;
        float3 bright = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + half2(stepx, stepy)).rgb;
        
        float3 center = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb;

        return abs(tleft - center) + abs(top - center) + abs(tright - center) + abs(left - center) +
               abs(right - center) + abs(bleft - center) + abs(bottom - center) + abs(bright - center);
      }

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

        o.positionCS = vertexInput.positionCS;
        o.uv = i.uv;
        return o;
      }

      half4 frag(VertexOutput i) : SV_TARGET {
        float depthSobel = step(actuallySobel(i.uv, _MainTex_TexelSize.x*_SobelThickness, _MainTex_TexelSize.y*_SobelThickness).r, 0.2);
        float normalSobelRes = step(normalSobel(i.uv, _MainTex_TexelSize.x*_SobelThickness, _MainTex_TexelSize.y*_SobelThickness), 0.1);
        half4 screen = SAMPLE_TEXTURE2D(_Screen, sampler_Screen, i.uv);
        half4 color = half4(0, 0, 0, 1);
        color.rgb = lerp(_BaseColor.rgb, screen.rgb, normalSobelRes*depthSobel);
        // color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).a;
        return color;
      }

      ENDHLSL
    }
  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
