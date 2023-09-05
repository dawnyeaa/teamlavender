using UnityEngine;

using UnityEditor;

public class SkateboardRailState : SkateboardBaseState {
  public SkateboardRailState(SkateboardStateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
  }

  public override void Tick() {
    Debug.DrawLine(sm.Board.position, sm.GrindingRail.Position, Color.white);

    var a = Vector3.ProjectOnPlane(Physics.gravity, sm.GrindingRail.RailVector);
    var v = Vector3.ProjectOnPlane(sm.MainRB.velocity, sm.GrindingRail.RailVector);
    sm.MainRB.AddForce(-a, ForceMode.Acceleration);
    sm.MainRB.AddForce(-v/Time.fixedDeltaTime, ForceMode.Acceleration);
  }

  public override void Exit() {
  }
}