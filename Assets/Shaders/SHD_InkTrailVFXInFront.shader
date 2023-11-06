Shader "VFX/InkTrailInFront" {
  Properties {
    _NoiseTex ("Noise Texture", 2D) = "white" {}
    _PaperTex ("Paper Texture", 2D) = "white" {}

    _BaseColor ("Color", Color) = (1, 1, 1, 1)
    _EndChunk ("Trail End Roundness", Float) = 5.28

    _Threshold ("Threshold", Range(0, 1)) = 0.218
    _EdgeStrength ("Edge Strength", Float) = 0.3

    _BrushScale ("Brush Noise Scale", Float) = 1

    _NoiseStrength ("Noise Texture Strength", Range(0, 1)) = 0.4
    _PaperStrength ("Paper Texture Strength", Range(0, 1)) = 0.6
    _BrushStrength ("Brushstroke Strength", Range(0, 1)) = 0.49
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

    ZTest LEqual
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
      float _EndChunk;
      float _Threshold;
      float _EdgeStrength;

      float _BrushScale;

      float _NoiseStrength;
      float _PaperStrength;
      float _BrushStrength;
      
      float _length;

      TEXTURE2D(_NoiseTex);
      float4 _NoiseTex_ST;
      SAMPLER(sampler_NoiseTex);
      
      TEXTURE2D(_PaperTex);
      float4 _PaperTex_ST;
      SAMPLER(sampler_PaperTex);
      CBUFFER_END

      struct VertexInput {
        float4 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
        float4 color      : COLOR;
      };

      struct VertexOutput {
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
        float3 positionWS : TEXCOORD1;
        float4 color      : COLOR;
      };

      float overlay(float a, float b, float opacity) {
        float t = step(0.5, a);
        return lerp(a, lerp(2*a*b, 1 - 2*(1-a)*(1-b), t), opacity);
      }

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

        o.positionCS = vertexInput.positionCS;
        o.positionWS = vertexInput.positionWS;
        o.uv = i.uv;
        o.color = i.color;

        return o;
      }

      half4 frag(VertexOutput i) : SV_TARGET {
        half4 color;
        float2 worldPos = i.positionWS.xz;

        float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, worldPos * _NoiseTex_ST.xy + _NoiseTex_ST.zw).a;
        float bigNoise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, worldPos * 0.3 * _NoiseTex_ST.xy + _NoiseTex_ST.zw).a;
        float paper = SAMPLE_TEXTURE2D(_PaperTex, sampler_PaperTex, worldPos * _PaperTex_ST.xy + _PaperTex_ST.zw).r;
        float brush = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, i.uv * float2(0, _BrushScale)).a;

        float trailCenter = abs(2*i.uv.y-1);
        float edges = pow(trailCenter, 7.5);
        brush *= 1-edges;

        float dialledEdges = edges * _EdgeStrength;

        float combinedShape = overlay((bigNoise*dialledEdges)+((1-i.uv.x)*_Threshold), noise, _NoiseStrength);
        combinedShape = overlay(combinedShape, paper, _PaperStrength);
        combinedShape *= brush * _BrushStrength * 4;

        float steppedShape = smoothstep(0.06, 0.09, combinedShape);
        // color.rgb = (bigNoise*dialledEdges)+((1-i.uv.x)*_Threshold);
        // color.a = 1;

        float rounded = step(1, i.color.r*(1-i.uv.x)/pow(trailCenter, _EndChunk));

        color.rgb = _BaseColor.rgb;
        color.a = rounded * steppedShape * _BaseColor.a;

        clip(color.a - 0.5);

        return color;
      }

      ENDHLSL
    }

  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
