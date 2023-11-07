Shader "PostFX/MotionBlurring" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _BlurResolution ("Blur Resolution", Int) = 20
  }

  SubShader {
    Tags { "RenderType" = "Opaque"
           "RenderPipeline" = "UniversalPipeline"
           "Queue" = "Geometry"
           "UniversalMaterialType" = "Lit" }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
    ENDHLSL

    Pass {
      Name "UniversalForward"
      Tags { "LightMode" = "UniversalForward" }

      HLSLPROGRAM

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

      #pragma vertex vert
      #pragma fragment frag
      
      TEXTURE2D(_MainTex);
      SAMPLER(sampler_MainTex);
      float4 _MainTex_TexelSize;
      
      TEXTURE2D(_Screen);
      SAMPLER(sampler_Screen);
      float4 _Screen_TexelSize;

      float _MaxBlurSize;
      int _BlurResolution;

      struct VertexInput {
        float4 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
      };

      struct VertexOutput {
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
      };

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

        o.positionCS = vertexInput.positionCS;
        o.uv = i.uv;

        return o;
      }

      half4 frag(VertexOutput input) : SV_TARGET {
        float4 blurData = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
        if (blurData.a > 0) {
          float blurDistance = _MaxBlurSize * blurData.z * blurData.z;
          
          float3 sum = float3(0, 0, 0);

          int skippedValues = 0;

          float2 offset = (blurData.xy*2-1) * blurDistance * _Screen_TexelSize.xy;

          float2 samplePos;
          float iF;
          float maskSample;
          float3 screenSample;

          [loop]
          for (int i = 0; i < _BlurResolution; ++i) {
            iF = ((float)i / _BlurResolution) * 2 - 1;

            samplePos = input.uv + iF * offset;
            maskSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, samplePos).a;
            if (maskSample > 0) {
              screenSample = SAMPLE_TEXTURE2D(_Screen, sampler_Screen, samplePos).xyz;
              sum += screenSample * maskSample;
            }
            else {
              skippedValues++;
            }
          }

          sum /= _BlurResolution-skippedValues;

          return float4(sum, 1);
        }
        return SAMPLE_TEXTURE2D(_Screen, sampler_Screen, input.uv);
      }

      ENDHLSL
    }

  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
