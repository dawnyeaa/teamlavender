Shader "Character/CustomisableBoard" {
  Properties {
    _BaseMap ("Main Tex", 2D) = "white" {}
    _GradientTex ("Gradient Tex", 2D) = "white" {}
    _GradientMaskTex ("Gradient Mask Tex", 2D) = "white" {}

    _DeckTex ("Deck Texture", 2D) = "white" {}
    [Toggle] _UseDeckTex ("Use Deck Texture?", Float) = 0

    _BackupShadowColor ("No-Gradient Shadow Color", Color) = (0, 0, 0, 1)

    _ShadowValue ("Shadow Value", Float) = 1
    _ShadowHardness ("Shadow Hardness", Range(0, 1)) = 0.9
    _ShadowSize ("Shadow Size", Range(0, 1)) = 0.6
    
    _ShadowTexturedHardness ("Shadow Textured Hardness", Range(0, 1)) = 0.3
    _ShadowTexturedOffset ("Shadow Textured Offset", Range(0, 1)) = 0.5

    _WashContrast ("Wash Contrast", Float) = 1
    _WashBalance ("Wash Balance", Float) = 0

    _WashContrast2 ("Wash Contrast 2", Float) = 1
    _WashBalance2 ("Wash Balance 2", Float) = 0

    _UpAmbientValue ("Top Ambient Value", Range(0, 1)) = 1
    _DownAmbientValue ("Bottom Ambient Value", Range(0, 1)) = 0

    _AmbientValueContrast ("Ambient Value Contrast", Float) = 1
    _AmbientValueBalance ("Ambient Value Balance", Float) = 0

    _GradientMapShadowOffset ("Gradient Mapped Shadow Offset", Float) = 0
    _GradientMapShadowMultiplier ("Gradient Mapped Shadow Multiplier", Float) = 1

    _WashTex ("Wash Texture", 2D) = "black" {}

    _TruckGradientX ("Truck Gradient X", Range(0, 1)) = 0
    _WheelGradientX ("Wheel Gradient X", Range(0, 1)) = 0

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

    Cull Off

    Pass {
      Name "ForwardLit"
      Tags { "LightMode" = "UniversalForward" }

      HLSLPROGRAM
      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

      #pragma shader_feature SHOW_CUTOUT_MASK

      #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
      #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
      
      #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
      #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
      #pragma multi_compile_fragment _ _SHADOWS_SOFT
      #pragma multi_compile _ LIGHTMAP_ON
      #pragma multi_compile _ DIRLIGHTMAP_COMBINED
      #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
      #pragma multi_compile _ SHADOWS_SHADOWMASK
      #pragma multi_compile _ _SCREEN_SPACE_OCCLUSION

      #pragma prefer_hlslcc gles
      #pragma exclude_renderers d3d11_9x
      #pragma target 2.0

      #pragma vertex vert
      #pragma fragment frag

      CBUFFER_START(UnityPerMaterial)
      float4 _BaseMap_ST;
      float4 _BaseColor;
      half4 _BackupShadowColor;
      float _Cutoff;

      float _UseDeckTex;

      float _ShadowValue;
      float _ShadowHardness;
      float _ShadowSize;

      float _ShadowTexturedHardness;
      float _ShadowTexturedOffset;

      float _WashContrast;
      float _WashBalance;
      
      float _WashContrast2;
      float _WashBalance2;

      float _UpAmbientValue;
      float _DownAmbientValue;

      float _AmbientValueContrast;
      float _AmbientValueBalance;

      float _GradientMapShadowOffset;
      float _GradientMapShadowMultiplier;

      float _TruckGradientX;
      float _WheelGradientX;
      
      TEXTURE2D(_DeckTex);
      SAMPLER(sampler_DeckTex);

      TEXTURE2D(_WashTex);
      float4 _WashTex_ST;
      SAMPLER(sampler_WashTex);
      
      TEXTURE2D(_GradientTex);
      float4 _GradientTex_ST;
      SAMPLER(sampler_GradientTex);
      
      TEXTURE2D(_GradientMaskTex);
      float4 _GradientMaskTex_ST;
      SAMPLER(sampler_GradientMaskTex);
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
        float3 positionWS : TEXCOORD3;
      };
      
      float3 _LightDirection;

      float overlay(float a, float b) {
        return lerp(2*a*b, 1-2*(1-a)*(1-b), 1-step(a, 0.5));
      }

      float contrastBalance(float v, float constrast, float balance) {
        return ((v-0.5)*(1/constrast))+0.5+balance;
      }

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

        VertexNormalInputs normalInput = GetVertexNormalInputs(i.normalOS, i.tangentOS);

        o.positionCS = vertexInput.positionCS;
        o.positionOS = i.positionOS.xyz;
        o.normalWS = normalInput.normalWS;
        o.positionWS = vertexInput.positionWS;
        o.uv = i.uv;

        return o;
      }

      half4 frag(VertexOutput i) : SV_TARGET {
        half4 color;

        half4 mainTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, TRANSFORM_TEX(i.uv, _BaseMap));
        half2 gradientMask = SAMPLE_TEXTURE2D(_GradientMaskTex, sampler_GradientMaskTex, TRANSFORM_TEX(i.uv, _GradientMaskTex)).rg;
        half4 deckTex = SAMPLE_TEXTURE2D(_DeckTex, sampler_DeckTex, i.uv);
        half3 mainColor = pow(mainTex.rgb, 2.2);
        float gradientT = mainTex.r;

        mainColor = lerp(mainColor, deckTex, mainTex.a*_UseDeckTex);

        float lightDot = (dot(_LightDirection, i.normalWS)*0.5)+0.5;
        float shadowSoftnessOffset = (1-_ShadowHardness)/2;
        float lightMask = smoothstep(_ShadowSize-shadowSoftnessOffset, _ShadowSize+shadowSoftnessOffset, lightDot);
        float4 shadowAttenuation = GetMainLight(TransformWorldToShadowCoord(i.positionWS)).shadowAttenuation;
        lightMask *= shadowAttenuation.r;

        float wash = SAMPLE_TEXTURE2D(_WashTex, sampler_WashTex, TRANSFORM_TEX(i.uv, _WashTex)).r;

        float wa = 1-overlay(lightMask, saturate(contrastBalance(wash, _WashContrast, _WashBalance)));
        
        float ambientT = (((dot(i.normalWS, half3(0, 1, 0))*_AmbientValueContrast)+_AmbientValueBalance)*0.5)+0.5;
        float ambientValue = lerp(_DownAmbientValue, _UpAmbientValue, saturate(ambientT));

        float shading = saturate(smoothstep(_ShadowTexturedOffset-(1-_ShadowTexturedHardness), _ShadowTexturedOffset+(1-_ShadowTexturedHardness), wa)-contrastBalance(wash, _WashContrast2, _WashBalance2)) * _ShadowValue;

        float3 gradientTrucksColor = SAMPLE_TEXTURE2D(_GradientTex, sampler_GradientTex, TRANSFORM_TEX(float2(_TruckGradientX, gradientT), _GradientTex));
        float3 gradientWheelsColor = SAMPLE_TEXTURE2D(_GradientTex, sampler_GradientTex, TRANSFORM_TEX(float2(_WheelGradientX, gradientT), _GradientTex));

        float gradientMapShading = (((shading-((ambientValue*2)-1))-0.5)*_GradientMapShadowMultiplier)+0.5+_GradientMapShadowOffset;

        float3 gradientTrucksDarkestColor = SAMPLE_TEXTURE2D(_GradientTex, sampler_GradientTex, TRANSFORM_TEX(float2(_TruckGradientX, 0), _GradientTex));
        float3 gradientWheelsDarkestColor = SAMPLE_TEXTURE2D(_GradientTex, sampler_GradientTex, TRANSFORM_TEX(float2(_WheelGradientX, 0), _GradientTex));

        float gradientShadingValue = gradientT-gradientMapShading;

        float3 gradientTrucksDeepShadow = lerp(gradientTrucksDarkestColor*_BackupShadowColor.rgb, gradientTrucksDarkestColor, saturate(gradientShadingValue+1));
        float3 gradientWheelsDeepShadow = lerp(gradientWheelsDarkestColor*_BackupShadowColor.rgb, gradientWheelsDarkestColor, saturate(gradientShadingValue+1));

        float3 gradientTrucksShadedColor = lerp(gradientTrucksDeepShadow, SAMPLE_TEXTURE2D(_GradientTex, sampler_GradientTex, TRANSFORM_TEX(float2(_TruckGradientX, saturate(gradientShadingValue)), _GradientTex)), step(0, gradientShadingValue));
        float3 gradientWheelsShadedColor = lerp(gradientWheelsDeepShadow, SAMPLE_TEXTURE2D(_GradientTex, sampler_GradientTex, TRANSFORM_TEX(float2(_WheelGradientX, saturate(gradientShadingValue)), _GradientTex)), step(0, gradientShadingValue));
        
        float3 gradientShadedColor = lerp(gradientTrucksShadedColor, gradientWheelsShadedColor, gradientMask.g);

        float3 mainShadedColor = lerp(mainColor, mainColor*(_BackupShadowColor.rgb + ambientValue), shading);
        float3 maskedColor = lerp(mainShadedColor, gradientShadedColor, max(gradientMask.r, gradientMask.g));
        return half4(maskedColor, 1);
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
      half4 _IDColor;
      float _Cutoff;

      float _CutoutR;
      float _CutoutG;
      float _CutoutB;
      
      TEXTURE2D(_BodyCutoutTex);
      float4 _BodyCutoutTex_ST;
      SAMPLER(sampler_BodyCutoutTex);
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

        float3 mask = 1-SAMPLE_TEXTURE2D(_BodyCutoutTex, sampler_BodyCutoutTex, TRANSFORM_TEX(i.uv, _BodyCutoutTex));
        half3 thresholds = float3(_CutoutR, _CutoutG, _CutoutB);
        clip(mask - thresholds);

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
