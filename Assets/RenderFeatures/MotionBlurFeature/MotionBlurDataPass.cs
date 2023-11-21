using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MotionBlurDataPass : ScriptableRenderPass {
  private readonly ProfilingSampler _profilingSampler;
  private FilteringSettings _filteringSettings;

  public Material _motionBlurConeMaterial;
  private readonly int _motionBlurDataId;
  private RenderTargetIdentifier _motionBlurDataRT;

  public MotionBlurDataPass(string profilerTag, LayerMask layerMask, int motionBlurDataId) {
    // set up the profiler so it has a slot in there
    _profilingSampler = new ProfilingSampler(profilerTag);

    // set up that filter from the layer mask i mentioned earlier
    _filteringSettings = new FilteringSettings(null, layerMask);

    _motionBlurDataId = motionBlurDataId;

    renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
  }

  public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
    var motionBlurDataDesc = renderingData.cameraData.cameraTargetDescriptor;
    motionBlurDataDesc.colorFormat = RenderTextureFormat.ARGB32;
    cmd.GetTemporaryRT(_motionBlurDataId, motionBlurDataDesc);
    _motionBlurDataRT = new RenderTargetIdentifier(_motionBlurDataId);
    ConfigureTarget(_motionBlurDataRT, renderingData.cameraData.renderer.cameraDepthTarget);
    ConfigureClear(ClearFlag.Color, Color.clear);
  }

  public override void OnCameraCleanup(CommandBuffer cmd) {
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
    // some settings we need for drawing the draw renderers
    var drawingSettings = CreateDrawingSettings(new ShaderTagId("MotionBlurData"), ref renderingData, SortingCriteria.CommonOpaque);
    drawingSettings.overrideMaterial = _motionBlurConeMaterial;
    // drawingSettings.overrideMaterialPassIndex = 0;
    
    var cmd = CommandBufferPool.Get();
    // make sure its shown in its own profiler step
    using (new ProfilingScope(cmd, _profilingSampler)) {
      // do the rendering!!!
      context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
    }

    context.ExecuteCommandBuffer(cmd);
    cmd.Clear();
    CommandBufferPool.Release(cmd);
  }
}