Shader "StrokeyFeature/dropShadow" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _Offset ("Offset Distance", Float) = 20
    _Angle ("Angle", Float) = 45
    _Color ("Color", Color) = (1, 1, 1, 1)
  }

  SubShader {
    Tags { "RenderType" = "Opaque"
           "RenderPipeline" = "UniversalPipeline"
           "Queue" = "Geometry"
           "UniversalMaterialType" = "Lit" }
    ZWrite Off Cull Off

    Pass {
      Name "DropShadow"
      Tags { "LightMode" = "UniversalForward" }

      HLSLPROGRAM

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

      #pragma vertex vert
      #pragma fragment frag

      float _Offset;
      float _Angle;
      float4 _Color;
      
      TEXTURE2D(_MainTex);
      SAMPLER(sampler_MainTex);
      float4 _MainTex_TexelSize;
      
      TEXTURE2D(_Screen);
      SAMPLER(sampler_Screen);
      float4 _Screen_TexelSize;

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
        float mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).r;
        float angRadians = _Angle*2*PI/360.0;
        float2 offset = float2(cos(angRadians), sin(angRadians)) * _Offset;
        offset /= _ScreenParams.xy;
        float shadow = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + offset).r;
        float4 screen = SAMPLE_TEXTURE2D(_Screen, sampler_Screen, i.uv);
        return lerp(screen, _Color, saturate(shadow-mask));
      }
      ENDHLSL
    }
  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}