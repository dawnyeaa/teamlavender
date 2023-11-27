using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StrokeyIDPass : ScriptableRenderPass {
  private ProfilingSampler _profilingSampler;
  // i think this is the variable thats gonna store our filter decided by the layer mask
  private FilteringSettings _filteringSettings;
  // this is the pass to find in shaders to run
  private static readonly ShaderTagId _shaderTag = new ShaderTagId("ID");
  // making an id for the render target we're gonna draw the id map to
  private int _renderTargetId;
  // an identifier SPECIFICALLY for the command buffer
  private RenderTargetIdentifier _renderTargetIdentifier;

  public Material _overrideMat;

  public StrokeyIDPass(string profilerTag, LayerMask layerMask, int renderTargetId) {
    // set up the profiler so it has a slot in there
    _profilingSampler = new ProfilingSampler(profilerTag);

    // set up that filter from the layer mask i mentioned earlier
    _filteringSettings = new FilteringSettings(null, layerMask);

    _renderTargetId = renderTargetId;

    // i get to choose when this pass happens!
    renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
  }

  public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
    RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
    blitTargetDescriptor.colorFormat = RenderTextureFormat.ARGB32;
    blitTargetDescriptor.depthBufferBits = 0;
    // blitTargetDescriptor.msaaSamples = 8;
    cmd.GetTemporaryRT(_renderTargetId, blitTargetDescriptor);
    _renderTargetIdentifier = new RenderTargetIdentifier(_renderTargetId);
    ConfigureTarget(_renderTargetIdentifier);
    ConfigureClear(ClearFlag.All, Color.clear);
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
    // var RFprops = renderingData.cameraData.camera.GetComponent<RendererFeatureDynamicProperties>();
    // if (RFprops) {
    //   if (!RFprops.StrokesEnabled) return;
    // }
    var camera = renderingData.cameraData.camera;
    // some settings we need for drawing the draw renderers
    var drawingSettings = CreateDrawingSettings(_shaderTag, ref renderingData, SortingCriteria.CommonOpaque);
    drawingSettings.overrideMaterial = _overrideMat;
    drawingSettings.overrideMaterialPassIndex = 1;
    drawingSettings.fallbackMaterial = _overrideMat;
    
    var cmd = CommandBufferPool.Get();

    cmd.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);

    context.ExecuteCommandBuffer(cmd);
    // context.SetupCameraProperties(camera);
    // make sure its shown in its own profiler step
    using (new ProfilingScope(cmd, _profilingSampler)) {
      // do the rendering!!!
      context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings);
    }
    cmd.Clear();
    CommandBufferPool.Release(cmd);      
  }
}