Shader "Custom/RenderFeature/StrokeQuads" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _Size ("size", float) = 1
    _Width ("width", float) = 1
    _Height ("height", float) = 1

    _RandSizeBounds ("random size bounds", Vector) = (0.8, 1.2, 0.8, 1.2)
  }

  SubShader {
    Pass {
      Name "StrokeQuadDraw"

      Cull Off

      ZTest Always
      ZWrite Off

      Stencil {
        Ref 55
        Comp NotEqual
      }

      Blend SrcAlpha OneMinusSrcAlpha

      HLSLPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma target 5.0

      #include "StrokePoint.hlsl"

      #define PI 3.1415926535

      struct VertexOutput {
        float4 positionCS : SV_POSITION;
        float2 uv         : TEXCOORD0;
        float4 color      : COLOR;
      };

      StructuredBuffer<StrokePoint> _quadPoints;

      Texture2D<float4> _MainTex;
      SamplerState sampler_MainTex;
      
      float _WidthRatio;
      float _ScreenSizeX;
      float _ScreenSizeY;
      float _Size;
      float _Width;
      float _Height;

      float4 _RandSizeBounds;

      static float2 uvByVertexID[6] = {
        float2(1.0, 0.0),
        float2(1.0, 1.0),
        float2(0.0, 0.0),
        float2(0.0, 0.0),
        float2(1.0, 1.0),
        float2(0.0, 1.0)
      };
      static float angleByVertexID[6] = {
        0.5 * PI,
        0.0 * PI,
        1.0 * PI,
        1.0 * PI,
        0.0 * PI,
        1.5 * PI
      };

      float2 PositionFromStrokePoint(StrokePoint p, int vertexID, float rand1, float rand2) {
        float rotation = angleByVertexID[vertexID];

        float s = sin(rotation);
        float c = cos(rotation);
        float2x2 rMatrix = float2x2(c, -s, s, c);
        rMatrix *= 0.5;
        rMatrix += 0.5;
        rMatrix = rMatrix * 2 - 1;
        float size = _Size * 0.12;
        float2 offset = mul(size.xx, rMatrix);
        offset.x *= _Width * lerp(_RandSizeBounds.x, _RandSizeBounds.y, rand1);
        offset.y *= _Height * lerp(_RandSizeBounds.z, _RandSizeBounds.w, rand2);
        
        float secrotation = (p.angle+0.25)*2*PI;
        s = sin(secrotation);
        c = cos(secrotation);
        rMatrix = float2x2(c, -s, s, c);
        offset = mul(offset, rMatrix);

        offset.x /= _WidthRatio;

        float2 middle = float2(
          (float(p.middle.x) / _ScreenSizeX) * 2.0 - 1.0,
          (1.0 - (float(p.middle.y) / _ScreenSizeY)) * 2.0 - 1.0
        );

        return middle + offset;
      }

      float rand2dTo1d(float2 value, float2 dotDir = float2(12.9898, 78.233)) {
        float2 smallValue = sin(value);
        float random = dot(smallValue, dotDir);
        random = frac(sin(random) * 143758.5453);
        return random;
      }

      VertexOutput vert(uint vertexID : SV_VERTEXID, uint instanceID : SV_INSTANCEID) {
        VertexOutput o;

        StrokePoint strokePoint = _quadPoints[instanceID];
        float rand = rand2dTo1d(strokePoint.mainPoint);
        float rand2 = rand2dTo1d(strokePoint.mainPoint+float2(478, 94));
        float rand3 = rand2dTo1d(strokePoint.mainPoint+float2(637, 83));
        float rand4 = rand2dTo1d(strokePoint.mainPoint+float2(57.221, 207.7713));
        float2 pos = PositionFromStrokePoint(strokePoint, vertexID, rand, rand2);

        o.positionCS = float4(pos.x, pos.y, 0.5, 1.0);
        o.uv = uvByVertexID[vertexID];
        float2 uvFlip = step(0.5, float2(rand3, rand4));
        o.uv = uvFlip + (o.uv * (uvFlip * -2 + 1));
        o.color = float4(rand, rand2, 0, 1);
        return o;
      }

      float4 frag(VertexOutput i) : SV_TARGET {
        float flipTile = floor(i.color.r*4);
        float4 tex = _MainTex.Sample(sampler_MainTex, i.uv*float2(1, 0.25)+(float2(0, 0.25*flipTile)));
        return tex;
      }
      ENDHLSL
    }
  }
}