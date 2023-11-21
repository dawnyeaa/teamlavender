using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MotionBlurBlurringPass : ScriptableRenderPass {
  private readonly ProfilingSampler _profilingSampler;

  public Material _blurringMaterial;
  private readonly int _motionBlurDataId;
  private RenderTargetIdentifier _motionBlurDataRT;
  private static readonly int _tmpId1 = Shader.PropertyToID("tmpJFART1");
  RenderTargetIdentifier _tmpRT1;

  RendererFeatureDynamicProperties _RFprops;

  public MotionBlurBlurringPass(string profilerTag, int motionBlurDataId) {
    // set up the profiler so it has a slot in there
    _profilingSampler = new ProfilingSampler(profilerTag);

    _motionBlurDataId = motionBlurDataId;

    renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
  }

  public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
    var desc = renderingData.cameraData.cameraTargetDescriptor;
    cmd.GetTemporaryRT(_tmpId1, desc);
    _tmpRT1 = new RenderTargetIdentifier(_tmpId1);
    _motionBlurDataRT = new RenderTargetIdentifier(_motionBlurDataId);
  }

  public override void OnCameraCleanup(CommandBuffer cmd) {
    cmd.ReleaseTemporaryRT(_motionBlurDataId);
    cmd.ReleaseTemporaryRT(_tmpId1);
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
    var cmd = CommandBufferPool.Get();
    // make sure its shown in its own profiler step
    using (new ProfilingScope(cmd, _profilingSampler)) {
      // do the rendering!!!
      cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, _tmpRT1);
      cmd.SetGlobalTexture(Shader.PropertyToID("_Screen"), _tmpRT1);

      _RFprops = renderingData.cameraData.camera.GetComponent<RendererFeatureDynamicProperties>();
      if (_RFprops)
        cmd.SetGlobalFloat(Shader.PropertyToID("_MaxBlurSize"), _RFprops.MotionBlurSize);
      cmd.Blit(_motionBlurDataRT, renderingData.cameraData.renderer.cameraColorTarget, _blurringMaterial);
    }

    context.ExecuteCommandBuffer(cmd);
    cmd.Clear();
    CommandBufferPool.Release(cmd);
  }
}