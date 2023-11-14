Shader "VFX/InFrontCone" {
  Properties {
    _BaseMap ("Main Tex", 2D) = "white" {}
    _TopScale ("Top Scale", Float) = 0
    _BaseColor ("Tint", Color) = (1, 1, 1, 1)
    _Tiling ("Tiling", Float) = 2
    _FPS ("FPS", Int) = 6
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
      float _TopScale;
      float _Tiling;
      int _FPS;
      float3 _pos;
      float _Cutoff;

      TEXTURE2D(_BaseMap);
      float4 _BaseMap_ST;
      SAMPLER(sampler_BaseMap);
      CBUFFER_END

      struct VertexInput {
        float4 positionOS : POSITION;
        float4 uv         : TEXCOORD0;
        float4 color      : COLOR;
      };

      struct VertexOutput {
        float4 positionCS : SV_POSITION;
        float4 uv         : TEXCOORD0;
        float4 color      : COLOR;
      };

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        float3 osPos = i.positionOS.xyz - _pos;
        osPos.xz *= (_TopScale * i.uv.y) + 1;
        osPos += _pos;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(osPos);

        o.positionCS = vertexInput.positionCS;
        o.uv = i.uv;
        o.color = i.color;

        return o;
      }

      half4 frag(VertexOutput i) : SV_TARGET {
        half4 color;

        float2 uv = float2((floor((_Time.y+i.uv.w)*_FPS)*0.61)+(i.uv.x*_Tiling), saturate(i.uv.y+(1-i.uv.z)));

        color.rgb = i.color.rgb * _BaseColor.rgb;
        color.a = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv).r * i.color.a * _BaseColor.a;

        return color;
      }

      ENDHLSL
    }

  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
