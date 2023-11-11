Shader "General/UnlitInFront" {
  Properties {
    _BaseMap ("Main Tex", 2D) = "white" {}

    _MatrixPixel ("Matrix Pixel Tex", 2D) = "white" {}
    _Gain ("Gain", Float) = 3
  }

  SubShader {
    Tags { "RenderType" = "Opaque"
           "RenderPipeline" = "UniversalPipeline"
           "Queue" = "Geometry"
           "UniversalMaterialType" = "Lit" }

    Stencil {
      Ref 55
      Comp Always
      Pass Replace
    }

    Pass {
      Name "ForwardLit"
      Tags { "LightMode" = "UniversalForward" }

      HLSLPROGRAM
      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

      #pragma prefer_hlslcc gles
      #pragma exclude_renderers d3d11_9x
      #pragma target 2.0

      #pragma vertex vert
      #pragma fragment frag

      CBUFFER_START(UnityPerMaterial)
      float _Gain;

      TEXTURE2D(_BaseMap);
      SAMPLER(sampler_BaseMap);
      float4 _BaseMap_TexelSize;

      TEXTURE2D(_MatrixPixel);
      SAMPLER(sampler_MatrixPixel);
      CBUFFER_END

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

      half4 frag(VertexOutput i) : SV_TARGET {
        half4 color;

        float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
        float4 pixel = SAMPLE_TEXTURE2D(_MatrixPixel, sampler_MatrixPixel, i.uv * _BaseMap_TexelSize.zw);

        color.rgb = tex.rgb * pixel.rgb * _Gain;
        color.a = 1;

        return color;
      }

      ENDHLSL
    }

  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
