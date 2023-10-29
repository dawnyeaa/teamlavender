Shader "PostFX/MotionBlurCone" {
  Properties {
  }

  SubShader {
    Tags { "RenderType" = "Opaque"
           "RenderPipeline" = "UniversalPipeline"
           "Queue" = "Geometry"
           "UniversalMaterialType" = "Lit" }

    Stencil {
      Ref 55
      Comp NotEqual
      Pass Keep
      Fail Keep
    }

    ZTest Always
    ZWrite Off
    Cull Off

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
    ENDHLSL

    // Pass {
    //   Name "UniversalForward"
    //   Tags { "LightMode" = "UniversalForward" }

    //   HLSLPROGRAM

    //   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    //   #pragma vertex vert
    //   #pragma fragment frag

    //   struct VertexInput {
    //     float4 positionOS : POSITION;
    //     float2 uv         : TEXCOORD0;
    //   };

    //   struct VertexOutput {
    //     float4 positionCS : SV_POSITION;
    //     float2 uv         : TEXCOORD0;
    //   };

    //   VertexOutput vert(VertexInput i) {
    //     VertexOutput o;

    //     VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

    //     o.positionCS = vertexInput.positionCS;
    //     o.uv = i.uv;

    //     return o;
    //   }

    //   half4 frag(VertexOutput i) : SV_TARGET {
    //     discard;
    //     return half4(0, 0, 0, 0);
    //   }

    //   ENDHLSL
    // }

    Pass {
      Name "MotionBlurData"
      Tags { "LightMode" = "MotionBlurData" }

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
        float2 uv         : TEXCOORD0;
      };
      
      float3 _LightDirection;

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

        o.positionCS = vertexInput.positionCS;
        o.uv = i.uv;

        return o;
      }

      half4 frag(VertexOutput i) : SV_TARGET {
        half4 color = half4(1, 1, 1, 1);
        float2 centered = i.uv*2-1;
        color.xy = normalize(centered)*0.5+0.5;
        color.z = length(centered);
        return color;
      }

      ENDHLSL
    }

  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
