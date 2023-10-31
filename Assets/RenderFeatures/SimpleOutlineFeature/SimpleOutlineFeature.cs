using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ActionCode.Attributes;

public class SimpleOutlineFeature : ScriptableRendererFeature {
  private SimpleOutlineDepthNormalsPass _depthNormalsPass;
  private SimpleOutlineSobelPass _simpleOutlinePass;
  [SerializeField] LayerMask _depthNormalLayerMask;
  [SerializeField] Material _depthNormalMaterial;
  [SerializeField] Material _simpleOutlineMaterial;

  public override void Create() {
    int depthNormalId = Shader.PropertyToID("_OutlineDepthNormalRT");
    _depthNormalsPass = new SimpleOutlineDepthNormalsPass("Outline Depth Normals Pass", _depthNormalLayerMask, depthNormalId) {
      _depthNormalMaterial = _depthNormalMaterial
    };
    _simpleOutlinePass = new SimpleOutlineSobelPass("Simple Outline Sobel Pass", depthNormalId) {
      _simpleOutlineMaterial = _simpleOutlineMaterial
    };
  }

  public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
    if (renderingData.cameraData.isPreviewCamera) return;
    // Ignore feature for editor/inspector previews & asset thumbnails
    if (renderingData.cameraData.isSceneViewCamera) return;
    // Ignore feature for scene view
    renderer.EnqueuePass(_depthNormalsPass);
    renderer.EnqueuePass(_simpleOutlinePass);
  }
}
