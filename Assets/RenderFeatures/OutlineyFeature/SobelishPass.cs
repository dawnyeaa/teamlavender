using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelishPass : ScriptableRenderPass {
  private readonly ProfilingSampler _profilingSampler;
  // the material with our sobelish shader on it
  public Material _sobelishMaterial;
  public Material _osSobelBlurMaterial;
  // the render target that we're grabbing the id map from
  private readonly int _idMapId;
  private static int _osMapId;
  // making an id for the render target we're gonna draw the sobelish thing to
  private readonly int _renderTargetIdSobel;
  private readonly int _renderTargetIdSobelOS;
  // an identifier SPECIFICALLY for the command buffer
  private RenderTargetIdentifier _idMapIdentifier;
  private RenderTargetIdentifier[] _renderTargetIdentifiers;
  private RenderTargetIdentifier _depthBufTemp, _osMap;

  public SobelishPass(string profilerTag, int idMapId, int osMapId, int renderTargetIdSobel, int renderTargetIdSobelOS) {
    // set up the profiler so it has a slot in there
    _profilingSampler = new ProfilingSampler(profilerTag);
    _idMapId = idMapId;
    _osMapId = osMapId;
    _renderTargetIdSobel = renderTargetIdSobel;
    _renderTargetIdSobelOS = renderTargetIdSobelOS;

    // i get to choose when this pass happens!
    renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
  }

  public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
    RenderTextureDescriptor osSobelTexDesc = renderingData.cameraData.cameraTargetDescriptor;
    osSobelTexDesc.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;

    cmd.GetTemporaryRT(_renderTargetIdSobel, renderingData.cameraData.cameraTargetDescriptor);
    cmd.GetTemporaryRT(_renderTargetIdSobelOS, osSobelTexDesc);
    // cmd.GetTemporaryRT(Shader.PropertyToID("_depthBufTemp"), cameraTextureDescriptor);

    _renderTargetIdentifiers = new RenderTargetIdentifier[2];
    _renderTargetIdentifiers[0] = new RenderTargetIdentifier(_renderTargetIdSobel);
    _renderTargetIdentifiers[1] = new RenderTargetIdentifier(_renderTargetIdSobelOS);
    // _depthBufTemp = new RenderTargetIdentifier(Shader.PropertyToID("_depthBufTemp"));

    _osMap = new RenderTargetIdentifier(_osMapId);
    _idMapIdentifier = new RenderTargetIdentifier(_idMapId);
    ConfigureTarget(_renderTargetIdentifiers, renderingData.cameraData.renderer.cameraDepthTarget);
  }

  public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
    CommandBuffer cmd = CommandBufferPool.Get();
    using (new ProfilingScope(cmd, _profilingSampler)) {      
      // render the sobel (and sobel object space coordinates)
      cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
      cmd.SetGlobalTexture(Shader.PropertyToID("_MainTex"), _idMapIdentifier);
      cmd.SetGlobalTexture(Shader.PropertyToID("_OSTex"), _osMap);
      cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, _sobelishMaterial);
    }

    context.ExecuteCommandBuffer(cmd);
    cmd.Clear();

    CommandBufferPool.Release(cmd);
  }

  public override void OnCameraCleanup(CommandBuffer cmd) {
    cmd.ReleaseTemporaryRT(_idMapId);
    cmd.ReleaseTemporaryRT(_osMapId);
  }
}