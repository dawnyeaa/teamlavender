using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SimpleOutlineDepthNormalsPass : ScriptableRenderPass {
  private readonly ProfilingSampler _profilingSampler;
  private FilteringSettings _filteringSettings;

  public Material _depthNormalMaterial;
  private readonly int _depthNormalId;
  private RenderTargetIdentifier _depthNormalRT;

  public SimpleOutlineDepthNormalsPass(string profilerTag, LayerMask layerMask, int depthNormalId) {
    // set up the profiler so it has a slot in there
    _profilingSampler = new ProfilingSampler(profilerTag);

    // set up that filter from the layer mask i mentioned earlier
    _filteringSettings = new FilteringSettings(null, layerMask);

    _depthNormalId = depthNormalId;

    renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
  }

  public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
    var depthNormalDataDesc = renderingData.cameraData.cameraTargetDescriptor;
    depthNormalDataDesc.colorFormat = RenderTextureFormat.ARGBFloat;
    cmd.GetTemporaryRT(_depthNormalId, depthNormalDataDesc);
    _depthNormalRT = new RenderTargetIdentifier(_depthNormalId);
    ConfigureTarget(_depthNormalRT, renderingData.cameraData.renderer.cameraDepthTarget);
    ConfigureClear(ClearFlag.Color, Color.clear);
  }

  public override void OnCameraCleanup(CommandBuffer cmd) {
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
    // some settings we need for drawing the draw renderers
    var drawingSettings = CreateDrawingSettings(new ShaderTagId("DepthNormals"), ref renderingData, SortingCriteria.CommonOpaque);
    drawingSettings.overrideMaterial = _depthNormalMaterial;
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