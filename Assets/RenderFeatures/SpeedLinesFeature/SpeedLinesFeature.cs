using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpeedLinesFeature : ScriptableRendererFeature {
  #region Renderer Pass
  class SpeedLinesPass : ScriptableRenderPass {
    public Material speedLinesMat;
    ProfilingSampler _profilingSampler;
    int _tempRTId = Shader.PropertyToID("_TempRT");
    RenderTargetIdentifier _tempRT;
    public SpeedLinesPass(string profilerTag) {
    // set up the profiler so it has a slot in there
    _profilingSampler = new ProfilingSampler(profilerTag);

      renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
      var desc = renderingData.cameraData.cameraTargetDescriptor;
      cmd.GetTemporaryRT(_tempRTId, desc);
      _tempRT = new(_tempRTId);
      ConfigureTarget(_tempRT);
      ConfigureClear(ClearFlag.All, Color.clear);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
      var RFprops = renderingData.cameraData.camera.GetComponent<RendererFeatureDynamicProperties>();
      var amount = 0f;
      if (RFprops) {
        amount = RFprops.speedLinesAmount;
      }
      var cmd = CommandBufferPool.Get();
      // make sure its shown in its own profiler step
      using (new ProfilingScope(cmd, _profilingSampler)) {
        // do the rendering!!!
        cmd.Blit(renderingData.cameraData.renderer.cameraColorTarget, _tempRT);
        cmd.SetGlobalFloat("_amount", amount);
        cmd.Blit(_tempRT, renderingData.cameraData.renderer.cameraColorTarget, speedLinesMat, 0);
      }
      context.ExecuteCommandBuffer(cmd);
      cmd.Clear();
      CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd) {
      cmd.ReleaseTemporaryRT(_tempRTId);
    }
  }
  #endregion

  #region Renderer Feature

  SpeedLinesPass _speedLinesPass;
  bool _initialized;

  public Material speedLinesMat;

  public override void Create() {
    _speedLinesPass = new("Speed Lines Pass") {
      speedLinesMat = speedLinesMat
    };

    _initialized = true;
  }

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
    if (renderingData.cameraData.isPreviewCamera) return;
    // Ignore feature for editor/inspector previews & asset thumbnails
    if (renderingData.cameraData.isSceneViewCamera) return;
    // Ignore feature for scene view

    if (_initialized)
      renderer.EnqueuePass(_speedLinesPass);
  }

  #endregion
}
