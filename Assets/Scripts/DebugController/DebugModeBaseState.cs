using UnityEngine;

public abstract class DebugModeBaseState : State {
  protected readonly DebugModeStateMachine sm;

  private Vector2 MatchForwards(Vector2 v, Vector2 forwards) {
    float angle = Vector2.SignedAngle(Vector2.up, forwards)*Mathf.Deg2Rad;
    float x = v.x*Mathf.Cos(angle)-v.y*Mathf.Sin(angle);
    float y = v.x*Mathf.Sin(angle)+v.y*Mathf.Cos(angle);
    return new Vector2(x, y);
  }

  protected DebugModeBaseState(DebugModeStateMachine stateMachine) {
    sm = stateMachine;
  }

  protected void MoveCharacter() {
    // horizontal
    Vector3 forward = Vector3.ProjectOnPlane(sm.MainCamera.forward, Vector3.up).normalized;
    Vector2 localForward = new(forward.x, forward.z);
    Vector2 accel = Vector2.zero;
    if (sm.Input.move.magnitude > 0.1f) {
      accel = MatchForwards(sm.Input.move, localForward) * sm.Acceleration;
    }
    Vector2 drag = sm.Velocity * -sm.Deceleration;
    sm.Velocity += (accel + drag) * Time.unscaledDeltaTime;
    sm.Velocity = sm.Velocity.normalized * Mathf.Min(sm.Velocity.magnitude, sm.Speed);

    // vertical
    float vertMove = sm.Input.vert * sm.VertSpeed;

    sm.transform.position += new Vector3(sm.Velocity.x, vertMove, sm.Velocity.y) * Time.unscaledDeltaTime;
  }

  protected void SelectNextFrame() {
    sm.DebugFrameHandler.SelectNextFrame();
  }

  protected void SelectPrevFrame() {
    sm.DebugFrameHandler.SelectPrevFrame();
  }

  protected void IncreaseFrameWindow() {
    sm.DebugFrameHandler.IncreaseFrameWindow();
  }

  protected void DecreaseFrameWindow() {
    sm.DebugFrameHandler.DecreaseFrameWindow();
  }

  protected void DisplayDebugInfo() {
    sm.DebugFrameHandler.ArrangeTraceLines();
  }
}