using Cinemachine;

public class CameraController : CinemachineComponentBase
{
    public override void MutateCameraState(ref CameraState curState, float deltaTime)
    {
        
    }

    public override bool IsValid { get; }
    public override CinemachineCore.Stage Stage { get; }
}
