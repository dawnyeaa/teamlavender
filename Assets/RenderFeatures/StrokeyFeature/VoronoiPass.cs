using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VoronoiPass : ScriptableRenderPass {
  private ProfilingSampler _profilingSampler;
  public Mesh _voronoiMesh;
  public Material _voronoiMaterial;
  private float _pointDensity;
  private ComputeBuffer _poissonPointsBuffer;

  private int _voronoiRenderTargetId;

  private RenderTargetIdentifier _voronoiRenderTarget;
  private int _fps;

  public VoronoiPass(string profilerTag, int voronoiRenderTargetId, ComputeBuffer poissonPoints, float pointDensity) {
    _profilingSampler = new ProfilingSampler(profilerTag);

    _voronoiRenderTargetId = voronoiRenderTargetId;

    _poissonPointsBuffer = poissonPoints;

    _pointDensity = pointDensity;

    renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
  }

  public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
    RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
    desc.colorFormat = RenderTextureFormat.ARGBFloat;

    cmd.GetTemporaryRT(_voronoiRenderTargetId, desc);

    _voronoiRenderTarget = new RenderTargetIdentifier(_voronoiRenderTargetId);

    ConfigureTarget(_voronoiRenderTarget);
    ConfigureClear(ClearFlag.All, Color.clear);
  }

  public static void DrawFullScreen(CommandBuffer commandBuffer, Material material,
                                   MaterialPropertyBlock properties = null, int shaderPassId = 0) {
    commandBuffer.DrawProcedural(Matrix4x4.identity, material, shaderPassId, MeshTopology.Triangles, 3, 1, properties);
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
    var RFprops = renderingData.cameraData.camera.GetComponent<RendererFeatureDynamicProperties>();
    _fps = 3;
    if (RFprops) {
      if (!RFprops.StrokesEnabled) return;
      _fps = RFprops.StrokeFPS;
    }
    var cmd = CommandBufferPool.Get();

    using (new ProfilingScope(cmd, _profilingSampler)) {
      MaterialPropertyBlock properties = new();

      properties.SetInt("_PositionsSize", _poissonPointsBuffer.count);
      properties.SetBuffer("_Positions", _poissonPointsBuffer);
      properties.SetFloat("_PointDensity", _pointDensity);
      properties.SetInt("_FPS", _fps);
      cmd.DrawMesh(_voronoiMesh, Matrix4x4.identity, _voronoiMaterial, 0, 0, properties);
    }

    context.ExecuteCommandBuffer(cmd);
    cmd.Clear();

    CommandBufferPool.Release(cmd);
  }

  public override void OnCameraCleanup(CommandBuffer cmd) {
  }
}