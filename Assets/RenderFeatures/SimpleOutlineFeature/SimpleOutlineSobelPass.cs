using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class SimpleOutlineSobelPass : ScriptableRenderPass {

  ProfilingSampler _profilingSampler;
  public Material _simpleOutlineMaterial;
  private int _depthNormalId;
  private RenderTargetIdentifier _depthNormalRT;
  private static readonly int _tmpId = Shader.PropertyToID("tmpScreenRT");
  RenderTargetIdentifier _tmpRT;

  RendererFeatureDynamicProperties _RFprops;
  public SimpleOutlineSobelPass(string profilerTag, int depthNormalId) {
    _profilingSampler = new ProfilingSampler(profilerTag);
    _depthNormalId = depthNormalId;
    renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
  }

  public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
    _depthNormalRT = new RenderTargetIdentifier(_depthNormalId);

    RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
    // desc.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;
    cmd.GetTemporaryRT(_tmpId, desc);

    _tmpRT = new RenderTargetIdentifier(_tmpId);
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
    var cmd = CommandBufferPool.Get();

    using (new ProfilingScope(cmd, _profilingSampler)) {
      cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, _tmpRT);
      cmd.SetGlobalTexture(Shader.PropertyToID("_Screen"), _tmpRT);

      _RFprops = renderingData.cameraData.camera.GetComponent<RendererFeatureDynamicProperties>();
      if (_RFprops) {
        cmd.SetGlobalFloat(Shader.PropertyToID("_CameraWarpingFactor"), _RFprops.LineWobbleCameraFactor);
      }
      cmd.Blit(_depthNormalRT, renderingData.cameraData.renderer.cameraColorTarget, _simpleOutlineMaterial);
    }

    context.ExecuteCommandBuffer(cmd);
    cmd.Clear();

    CommandBufferPool.Release(cmd);
  }

  public override void OnCameraCleanup(CommandBuffer cmd) {
    cmd.ReleaseTemporaryRT(_depthNormalId);
    cmd.ReleaseTemporaryRT(_tmpId);
  }
}