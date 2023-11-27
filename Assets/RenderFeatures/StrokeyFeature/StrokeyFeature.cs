using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Runtime.InteropServices;

public class StrokeyFeature : ScriptableRendererFeature {
  private StrokeyIDPass _idPass;
  private SobelishAnglePass _sobelishPass;
  private VoronoiPass _voronoiPass;
  private StrokeQuadPass _strokeQuadPass;
  private DropShadowPass _dropShadowPass;

  [SerializeField] LayerMask _layerMask;
  [SerializeField] Material _sobelishMaterial;
  [SerializeField] Material _strokeQuadMaterial;
  [SerializeField] Material _sobelBlitMaterial;
  [SerializeField] Material _boxBlurMaterial;
  [SerializeField] Material _idOverrideMat;
  [SerializeField] Material _dropShadowMat;
  [SerializeField] Mesh _voronoiMesh;
  [SerializeField] Material _voronoiMaterial;
  // [SerializeField] ComputeShader _jfaComputeShader;
  [SerializeField] ComputeShader _strokeyQuadsComputeShader;
  [SerializeField] int _angleBlurSize = 3;
  [SerializeField] PoissonArrangementObject _poissonPoints;
  [SerializeField] int _pointScanSize;
  [SerializeField] Texture2D _strokeTexture;
  [SerializeField] float _strokeSize = 0.16f;
  [SerializeField] float _strokeWidth = 14.89f;
  [SerializeField] float _strokeHeight = 2.46f;
  [SerializeField] Vector2 _strokeRandomWidthBounds = new(0.2f, 1.1f);
  [SerializeField] Vector2 _strokeRandomHeightBounds = new(0.7f, 1.4f);

  [SerializeField] float _strokeDensity = 1;
  private ComputeBuffer _poissonPointsBuffer;
  private ComputeBuffer _quadPointsAppendBuffer;
  private ComputeBuffer _drawQuadsArgsBuffer;

  public override void Create() {
    int IDOutRT = Shader.PropertyToID("_IDPassRT");
    int SobelOutRT = Shader.PropertyToID("_sobelOutRT");
    int VoronoiOutRT = Shader.PropertyToID("_voronoiOutRT");

    if (_poissonPointsBuffer == null || !_poissonPointsBuffer.IsValid())
      _poissonPointsBuffer = new ComputeBuffer(_poissonPoints.points.Length, Marshal.SizeOf(typeof(Vector4)));
    _poissonPointsBuffer.SetData(_poissonPoints.points);
    

    if (_quadPointsAppendBuffer == null || !_quadPointsAppendBuffer.IsValid())
      _quadPointsAppendBuffer = new ComputeBuffer(_poissonPoints.points.Length, sizeof(uint)*2 + sizeof(float) + sizeof(uint)*2, ComputeBufferType.Append);

    if (_drawQuadsArgsBuffer == null || !_drawQuadsArgsBuffer.IsValid())
      _drawQuadsArgsBuffer = new ComputeBuffer(4, sizeof(uint), ComputeBufferType.IndirectArguments);
    _drawQuadsArgsBuffer.SetData(new uint[] {
      6, // vertices per instance
      0, // instance count
      0, // byte offset of first vertex
      0 // byte offset of first instance
    });

    _idPass = new StrokeyIDPass("Strokey ID Pass", _layerMask, IDOutRT) {
      _overrideMat = _idOverrideMat
    };
    _sobelishPass = new SobelishAnglePass("Sobelish Pass", IDOutRT, SobelOutRT, _angleBlurSize) {
      _sobelishMaterial = _sobelishMaterial,
      _boxBlurMaterial = _boxBlurMaterial
    };
    _voronoiPass = new VoronoiPass("Voronoi Pass", VoronoiOutRT, _poissonPointsBuffer, _strokeDensity) {
      _voronoiMesh = _voronoiMesh,
      _voronoiMaterial = _voronoiMaterial
    };
    _strokeQuadPass = new StrokeQuadPass(_strokeyQuadsComputeShader, 
                                        "Strokey Quads Pass", 
                                        SobelOutRT, 
                                        VoronoiOutRT, 
                                        _poissonPointsBuffer,
                                        _quadPointsAppendBuffer,
                                        _drawQuadsArgsBuffer, 
                                        _pointScanSize,
                                        _strokeTexture,
                                        _strokeDensity,
                                        _strokeSize,
                                        _strokeWidth,
                                        _strokeHeight,
                                        _strokeRandomWidthBounds,
                                        _strokeRandomHeightBounds) {
      _quadMaterial = _strokeQuadMaterial,
      _sobelBlitMat = _sobelBlitMaterial
    };
    _dropShadowPass = new DropShadowPass("Drop Shadow Pass", IDOutRT) {
      _dropShadowMat = _dropShadowMat
    };

  }

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
    if (renderingData.cameraData.isPreviewCamera) return;
    if (renderingData.cameraData.isSceneViewCamera) return;
    renderer.EnqueuePass(_idPass);
    renderer.EnqueuePass(_sobelishPass);
    renderer.EnqueuePass(_voronoiPass);
    renderer.EnqueuePass(_strokeQuadPass);
    renderer.EnqueuePass(_dropShadowPass);
  }

  protected override void Dispose(bool disposing) {
    // _voronoiPass.Dispose();
    // _strokeQuadPass.Dispose();
    _poissonPointsBuffer?.Release();
    _quadPointsAppendBuffer?.Release();
    _drawQuadsArgsBuffer?.Release();
    // _poissonPointsBuffer = null;
    // _quadPointsAppendBuffer = null;
    // _drawQuadsArgsBuffer = null;
    base.Dispose(disposing);
  }
}
