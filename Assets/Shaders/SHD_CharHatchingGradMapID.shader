Shader "Character/HatchingTexGradMapID" {
  Properties {
    _BaseMap ("Main Tex", 2D) = "white" {}

    _GradientMapColor0 ("Gradient Map Color 0", Color) = (0, 0, 0, 1)
    _GradientMapColor1 ("Gradient Map Color 1", Color) = (1, 1, 1, 1)
    _GradientBalance ("Gradient Balance", Float) = 0
    _GradientContrast ("Gradient Contrast", Float) = 1

    _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 1)
    _ShadowHardness ("Shadow Hardness", Range(0, 1)) = 0.9
    _ShadowSize ("Shadow Size", Range(0, 1)) = 0.6
    _ShadowFillAmount ("Shadow Fill Amount", Range(0, 1)) = 0.3

    _HatchingTex ("Hatching Texture", 2D) = "black" {}
    _CurvatureTex ("Curvature Texture", 2D) = "white" {}

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

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
		CBUFFER_START(UnityPerMaterial)
		float4 _BaseMap_ST;
		float4 _BaseColor;
		float _Cutoff;
		CBUFFER_END
    ENDHLSL

    Pass {
      Name "ForwardLit"
      Tags { "LightMode" = "UniversalForward" }

      HLSLPROGRAM
      #pragma prefer_hlslcc gles
      #pragma exclude_renderers d3d11_9x
      #pragma target 2.0

      #pragma vertex vert
      #pragma fragment frag

      CBUFFER_START(UnityPerMaterial)
      half4 _GradientMapColor0;
      half4 _GradientMapColor1;
      float _GradientBalance;
      float _GradientContrast;

      half4 _ShadowColor;
      float _ShadowHardness;
      float _ShadowSize;
      float _ShadowFillAmount;
      
      TEXTURE2D(_BaseMap);
      SAMPLER(sampler_BaseMap);

      TEXTURE2D(_HatchingTex);
      float4 _HatchingTex_ST;
      SAMPLER(sampler_HatchingTex);
      
      TEXTURE2D(_CurvatureTex);
      float4 _CurvatureTex_ST;
      SAMPLER(sampler_CurvatureTex);
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

        float gradientT = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv).r;
        gradientT = saturate(((gradientT-(0.5-(1/_GradientContrast)*0.5))*_GradientContrast)+_GradientBalance);
        float3 mainColor = lerp(_GradientMapColor0, _GradientMapColor1, gradientT);
        float lightDot = (dot(_LightDirection, i.normalWS)*0.5)+0.5;
        float shadowSoftnessOffset = (1-_ShadowHardness)/2;
        float lightMask = smoothstep(_ShadowSize-shadowSoftnessOffset, _ShadowSize+shadowSoftnessOffset, lightDot);

        float curvTex = SAMPLE_TEXTURE2D(_CurvatureTex, sampler_CurvatureTex, i.uv).a;

        float hatching = SAMPLE_TEXTURE2D(_HatchingTex, sampler_HatchingTex, TRANSFORM_TEX(i.uv, _HatchingTex)).a;
        float fadedHatching = step(((1-(curvTex*0.9))*(1-(curvTex*0.9)))+lightMask, hatching);
        fadedHatching += (1-lightMask)*_ShadowFillAmount;

        color.rgb = lerp(mainColor.rgb, _ShadowColor.rgb, _ShadowColor.a*fadedHatching);
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
