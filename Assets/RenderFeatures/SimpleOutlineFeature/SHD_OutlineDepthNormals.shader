Shader "PostFX/OutlineDepthNormals" {
  Properties {
  }

  SubShader {
    Tags { "RenderType" = "Opaque"
           "RenderPipeline" = "UniversalPipeline"
           "Queue" = "Geometry"
           "UniversalMaterialType" = "Lit" }

    Cull Off

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
    ENDHLSL

    Pass {
      Name "DepthNormals"
      Tags { "LightMode" = "DepthNormals" }

      HLSLPROGRAM
      #pragma prefer_hlslcc gles
      #pragma exclude_renderers d3d11_9x
      #pragma target 2.0

      #pragma vertex vert
      #pragma fragment frag

      struct VertexInput {
        float4 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
        float3 normalOS   : NORMAL;
        float4 tangentOS  : TANGENT;
      };

      struct VertexOutput {
        float4 positionCS : SV_POSITION;
        float3 normalWS   : TEXCOORD0;
      };

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

        VertexNormalInputs normalInput = GetVertexNormalInputs(i.normalOS, i.tangentOS);

        o.positionCS = vertexInput.positionCS;
        o.normalWS = normalInput.normalWS;

        return o;
      }

      float4 frag(VertexOutput i) : SV_TARGET {
        float4 color = float4(1, 1, 1, 1);
        color.xyz = i.normalWS;
        return color;
      }

      ENDHLSL
    }

  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
