using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class SimpleOutlineSobelPass : ScriptableRenderPass {

  ProfilingSampler _profilingSampler;
  public Material _simpleOutlineMaterial;
  private int _depthNormalId;
  private RenderTargetIdentifier _depthNormalRT;
  private static readonly int _tmpId1 = Shader.PropertyToID("tmpJFART1");
  RenderTargetIdentifier _tmpRT1;
  public SimpleOutlineSobelPass(string profilerTag, int depthNormalId) {
    _profilingSampler = new ProfilingSampler(profilerTag);
    _depthNormalId = depthNormalId;
    renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
  }

  public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
    _depthNormalRT = new RenderTargetIdentifier(_depthNormalId);

    RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
    // desc.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;
    cmd.GetTemporaryRT(_tmpId1, desc);

    _tmpRT1 = new RenderTargetIdentifier(_tmpId1);
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
    var cmd = CommandBufferPool.Get();

    using (new ProfilingScope(cmd, _profilingSampler)) {
      cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, _tmpRT1);
      cmd.SetGlobalTexture(Shader.PropertyToID("_Screen"), _tmpRT1);
      cmd.Blit(_depthNormalRT, renderingData.cameraData.renderer.cameraColorTarget, _simpleOutlineMaterial);
    }

    context.ExecuteCommandBuffer(cmd);
    cmd.Clear();

    CommandBufferPool.Release(cmd);
  }

  public override void OnCameraCleanup(CommandBuffer cmd) {
    cmd.ReleaseTemporaryRT(_depthNormalId);
  }
}