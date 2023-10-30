Shader "Character/DeckWashTexID" {
  Properties {
    _BaseMap ("Main Tex", 2D) = "white" {}

    _DeckTex ("Deck Texture", 2D) = "white" {}
    [Toggle] _UseDeckTex ("Use Deck Texture?", Float) = 0

    _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
    _ShadowValue ("Shadow Value", Float) = 1
    _ShadowHardness ("Shadow Hardness", Range(0, 1)) = 0.9
    _ShadowSize ("Shadow Size", Range(0, 1)) = 0.6
    
    _ShadowTexturedHardness ("Shadow Textured Hardness", Range(0, 1)) = 0.3
    _ShadowTexturedOffset ("Shadow Textured Offset", Range(0, 1)) = 0.5

    _WashContrast ("Wash Contrast", Float) = 1
    _WashBalance ("Wash Balance", Float) = 0

    _WashPatternStrength ("Wash Pattern Visibility", Float) = 1
    _WashContrast2 ("Wash Contrast 2", Float) = 1
    _WashBalance2 ("Wash Balance 2", Float) = 0

    _WashTex ("Wash Texture", 2D) = "black" {}

    _IDTex ("ID Texture", 2D) = "white" {}
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
      float4 _BaseMap_ST;
      float4 _BaseColor;
      float _Cutoff;

      float _UseDeckTex;

      half4 _ShadowColor;
      float _ShadowValue;
      float _ShadowHardness;
      float _ShadowSize;

      float _ShadowTexturedHardness;
      float _ShadowTexturedOffset;

      float _WashContrast;
      float _WashBalance;
      
      float _WashContrast2;
      float _WashBalance2;

      float _WashPatternStrength;
      
      TEXTURE2D(_BaseMap);
      SAMPLER(sampler_BaseMap);
      
      TEXTURE2D(_DeckTex);
      SAMPLER(sampler_DeckTex);

      TEXTURE2D(_WashTex);
      float4 _WashTex_ST;
      SAMPLER(sampler_WashTex);
      CBUFFER_END

      struct VertexInput {
        float4 positionOS : POSITION;
        float2 uv         : TEXCOORD0;
        float3 normalOS   : NORMAL;
        float4 tangentOS  : TANGENT;
      };

      struct VertexOutput {
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
        float3 normalWS   : TEXCOORD1;
        float3 positionOS : TEXCOORD2;
      };
      
      float3 _LightDirection;

      float overlay(float a, float b) {
        return lerp(2*a*b, 1-2*(1-a)*(1-b), 1-step(a, 0.5));
      }

      float contrastBalance(float v, float constrast, float balance) {
        return ((v-(0.5-(1/constrast)*0.5))*constrast)+balance;
      }

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

        VertexNormalInputs normalInput = GetVertexNormalInputs(i.normalOS, i.tangentOS);

        o.positionCS = vertexInput.positionCS;
        o.positionOS = i.positionOS.xyz;
        o.normalWS = normalInput.normalWS;
        o.uv = i.uv;

        return o;
      }

      half4 frag(VertexOutput i) : SV_TARGET {
        half4 color;

        half4 mainTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, TRANSFORM_TEX(i.uv, _BaseMap));
        half4 deckTex = SAMPLE_TEXTURE2D(_DeckTex, sampler_DeckTex, i.uv);

        mainTex = lerp(mainTex, deckTex, mainTex.a*_UseDeckTex);

        float lightDot = (dot(_LightDirection, i.normalWS)*0.5)+0.5;
        float shadowSoftnessOffset = (1-_ShadowHardness)/2;
        float lightMask = smoothstep(_ShadowSize-shadowSoftnessOffset, _ShadowSize+shadowSoftnessOffset, lightDot);

        float wash = SAMPLE_TEXTURE2D(_WashTex, sampler_WashTex, TRANSFORM_TEX(i.uv, _WashTex)).r;

        float wa = 1-overlay(lightMask, saturate(contrastBalance(wash, _WashContrast, _WashBalance)));

        float3 mainColor = lerp(mainTex, _ShadowColor, saturate(smoothstep(_ShadowTexturedOffset-(1-_ShadowTexturedHardness), _ShadowTexturedOffset+(1-_ShadowTexturedHardness), wa)-contrastBalance(wash, _WashContrast2, _WashBalance2))*_ShadowColor.a);

        return half4(mainColor, 1);
      }

      ENDHLSL
    }

    Pass {
      Name "ID"
      Tags { "LightMode" = "ID" }

      HLSLPROGRAM

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
      CBUFFER_START(UnityPerMaterial)
      float4 _BaseMap_ST;
      float4 _BaseColor;
      float _Cutoff;
      CBUFFER_END

      #pragma vertex vert
      #pragma fragment frag

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
        float4 idTex = SAMPLE_TEXTURE2D(_IDTex, sampler_IDTex, i.uv);

        color.rgb = idTex.rgb;
        color.a = 1;
        ID = color;
        OSPos = i.positionOS;
      }

      ENDHLSL
    }

    Pass {
      Name "ShadowCaster"
      Tags { "LightMode" = "ShadowCaster" }

      ZWrite On
      ZTest LEqual
      ColorMask 0
      Cull Back

      HLSLPROGRAM
      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
      
      CBUFFER_START(UnityPerMaterial)
      float4 _BaseMap_ST;
      float4 _BaseColor;
      float _Cutoff;
      CBUFFER_END
      //--------------------------------------
      // GPU Instancing
      #pragma multi_compile_instancing
      #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

      #pragma vertex ShadowPassVertex
      #pragma fragment ShadowPassFragment

      #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

      #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
      ENDHLSL
    }

  }
  FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
