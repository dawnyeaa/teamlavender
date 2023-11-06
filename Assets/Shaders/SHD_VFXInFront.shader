Shader "VFX/InFront" {
  Properties {
    _BaseMap ("Main Tex", 2D) = "white" {}

    _BaseColor ("Tint", Color) = (1, 1, 1, 1)
  }

  SubShader {
    Tags { "RenderType" = "Transparent"
           "RenderPipeline" = "UniversalPipeline"
           "Queue" = "Transparent"
           "UniversalMaterialType" = "Lit" }

    Stencil {
      Ref 55
      Comp Always
      Pass Replace
    }

    Blend SrcAlpha OneMinusSrcAlpha

    ZTest Always
    ZWrite Off
    Cull Off

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
      float4 _BaseColor;
      float _Cutoff;

      TEXTURE2D(_BaseMap);
      float4 _BaseMap_ST;
      SAMPLER(sampler_BaseMap);
      CBUFFER_END

      struct VertexInput {
        float4 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
        float4 color      : COLOR;
      };

      struct VertexOutput {
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
        float4 color      : COLOR;
      };

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

        o.positionCS = vertexInput.positionCS;
        o.uv = i.uv;
        o.color = i.color;

        return o;
      }

      half4 frag(VertexOutput i) : SV_TARGET {
        half4 color;

        float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv * _BaseMap_ST.xy + _BaseMap_ST.zw);

        color = tex * i.color * _BaseColor;

        return color;
      }

      ENDHLSL
    }

  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
