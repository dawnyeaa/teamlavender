using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ActionCode.Attributes;

public class MotionBlurFeature : ScriptableRendererFeature {
  private MotionBlurDataPass _motionBlurDataPass;
  private MotionBlurBlurringPass _motionBlurBlurringPass;
  [SerializeField] LayerMask _motionBlurConeLayerMask;
  [SerializeField] Material _motionBlurDataMaterial;
  [SerializeField] Material _blurringMaterial;

  public override void Create() {
    int motionBlurDataId = Shader.PropertyToID("_MotionBlurDataRT");
    _motionBlurDataPass = new MotionBlurDataPass("Motion Blur Data Pass", _motionBlurConeLayerMask, motionBlurDataId) {
      _motionBlurConeMaterial = _motionBlurDataMaterial
    };
    _motionBlurBlurringPass = new MotionBlurBlurringPass("Motion Blur Blurring Pass", motionBlurDataId) {
      _blurringMaterial = _blurringMaterial
    };
  }

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
    if (renderingData.cameraData.isPreviewCamera) return;
    // Ignore feature for editor/inspector previews & asset thumbnails
    if (renderingData.cameraData.isSceneViewCamera) return;
    // Ignore feature for scene view
    renderer.EnqueuePass(_motionBlurDataPass);
    renderer.EnqueuePass(_motionBlurBlurringPass);
  }
}
