Shader "UI/BoardCustoCutout" {
  Properties {
    _BaseMap ("Main Tex", 2D) = "white" {}

    _MaskTexture ("Mask Texture", 2D) = "white" {}

    [Toggle(ID_USE_TEXTURE)] _IDUseTexture ("Get ID from texture?", Float) = 0
    _IDTex ("ID Texture", 2D) = "white" {}
    _IDColor ("ID Color", Color) = (0, 0, 0, 1)
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
      

      TEXTURE2D(_BaseMap);
      SAMPLER(sampler_BaseMap);
      float4 _BaseMap_TexelSize;

      TEXTURE2D(_MaskTexture);
      SAMPLER(sampler_MaskTexture);
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
        float mask = SAMPLE_TEXTURE2D(_MaskTexture, sampler_MaskTexture, i.uv).r;
        clip(mask - 0.5);

        float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);

        color.rgb = tex.rgb;
        color.a = 1;

        return color;
      }

      ENDHLSL
    }

    Pass {
      Name "ID"
      Tags { "LightMode" = "ID" }

      HLSLPROGRAM

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
      CBUFFER_START(UnityPerMaterial)
      half4 _IDColor;

      TEXTURE2D(_MaskTexture);
      SAMPLER(sampler_MaskTexture);
      CBUFFER_END

      #pragma vertex vert
      #pragma fragment frag

      #pragma shader_feature ID_USE_TEXTURE

      CBUFFER_START(UnityPerMaterial)      
      TEXTURE2D(_IDTex);
      SAMPLER(sampler_IDTex);
      CBUFFER_END

      struct VertexInput {
        float4 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
      };

      struct VertexOutput {
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
        float3 positionOS : TEXCOORD1;
      };

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

        o.positionCS = vertexInput.positionCS;
        o.uv = i.uv;
        o.positionOS = i.positionOS.xyz;

        return o;
      }

      void frag(VertexOutput i, out half4 ID : SV_TARGET0, out float3 OSPos : SV_TARGET1) {
        half4 color;

        float3 mask = SAMPLE_TEXTURE2D(_MaskTexture, sampler_MaskTexture, i.uv);
        clip(mask - 0.5);

        #ifdef ID_USE_TEXTURE
          float4 idTex = SAMPLE_TEXTURE2D(_IDTex, sampler_IDTex, i.uv);
          color.rgb = idTex.rgb;
          color.a = 1;
        #else
          color = _IDColor;
        #endif

        ID = color;
        OSPos = i.positionOS;
      }

      ENDHLSL
    }

  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
