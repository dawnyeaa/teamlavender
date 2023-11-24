using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DropShadowPass : ScriptableRenderPass {

  ProfilingSampler _profilingSampler;

  public Material _dropShadowMat;

  int _maskRTId;
  int _tmpId = Shader.PropertyToID("tmpRT");
  RenderTargetIdentifier _maskRT, _tmpRT;

  public DropShadowPass(string profilerTag, int maskRTId) {
    // set up the profiler so it has a slot in there
    _profilingSampler = new ProfilingSampler(profilerTag);

    _maskRTId = maskRTId;

    // i get to choose when this pass happens!
    renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
  }
  public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
    var desc = renderingData.cameraData.cameraTargetDescriptor;
    cmd.GetTemporaryRT(_tmpId, desc);

    _maskRT = new(_maskRTId);
    _tmpRT = new(_tmpId);
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
    var RFprops = renderingData.cameraData.camera.GetComponent<RendererFeatureDynamicProperties>();
    if (RFprops) {
      if (!RFprops.DropShadowEnabled) return;
    }
    var cmd = CommandBufferPool.Get();
    // make sure its shown in its own profiler step
    using (new ProfilingScope(cmd, _profilingSampler)) {
      cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, _tmpRT);
      cmd.SetGlobalTexture("_Screen", _tmpRT);
      cmd.Blit(_maskRT, renderingData.cameraData.renderer.cameraColorTarget, _dropShadowMat, 0);
    }

    context.ExecuteCommandBuffer(cmd);
    cmd.Clear();
    CommandBufferPool.Release(cmd);    
  }

    public override void OnCameraCleanup(CommandBuffer cmd) {
      cmd.ReleaseTemporaryRT(_tmpId);
      cmd.ReleaseTemporaryRT(_maskRTId);
      base.OnCameraCleanup(cmd);
    }
}
