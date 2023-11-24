Shader "Example/ShaderWithColorID" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _ShadowColor ("Shadow Color", Color) = (1, 1, 1, 1)
    _ShadowHardness ("Shadow Hardness", Range(0, 1)) = 0.9
    _ShadowSize ("Shadow Size", Range(0, 1)) = 0.6
    _IDColor ("ID Color", Color) = (1, 1, 1, 1)
  }

  SubShader {
    Tags { "RenderType" = "Opaque"
           "RenderPipeline" = "UniversalPipeline"
           "Queue" = "Geometry"
           "UniversalMaterialType" = "Lit" }

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
      half4 _ShadowColor;
      float _ShadowHardness;
      float _ShadowSize;

      TEXTURE2D(_MainTex);
      SAMPLER(sampler_MainTex);
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
      };
      
      float3 _LightDirection;

      VertexOutput vert(VertexInput i) {
        VertexOutput o;

        VertexPositionInputs vertexInput = GetVertexPositionInputs(i.positionOS.xyz);

        VertexNormalInputs normalInput = GetVertexNormalInputs(i.normalOS, i.tangentOS);

        o.positionCS = vertexInput.positionCS;
        o.normalWS = normalInput.normalWS;
        o.uv = i.uv;

        return o;
      }

      half4 frag(VertexOutput i) : SV_TARGET {
        half4 color;
        float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        float lightDot = (dot(_LightDirection, i.normalWS)*0.5)+0.5;
        float shadowSoftnessOffset = (1-_ShadowHardness)/2;
        float lightMask = smoothstep(_ShadowSize-shadowSoftnessOffset, _ShadowSize+shadowSoftnessOffset, lightDot);

        color.rgb = lerp(lerp(baseTex.rgb, _ShadowColor.rgb, _ShadowColor.a), baseTex.rgb, lightMask);
        color.a = 1;
        return color;
      }

      ENDHLSL
    }

    Pass {
      Name "ID"
      Tags { "LightMode" = "ID" }

      ZTest Always

      HLSLPROGRAM

      #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

      #pragma vertex vert
      #pragma fragment frag
      
      CBUFFER_START(UnityPerMaterial)
        float4 _IDColor;

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
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

      half4 frag(VertexOutput i) : SV_TARGET{
        float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
        return half4(1, 1, 1, 1);
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
