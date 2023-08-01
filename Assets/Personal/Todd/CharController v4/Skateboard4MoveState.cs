using UnityEngine;

public class Skateboard4MoveState : Skateboard4BaseState {
  private readonly int TurnHash = Animator.StringToHash("dir");

  public Skateboard4MoveState(Skateboard4StateMachine stateMachine) : base(stateMachine) {}

  public override void Enter() {
    sm.Input.OnPushPerformed += OnPush;
  }

  public override void Tick() {
    Vector3 footDirection = Vector3.down;
    sm.footRepresentation.localScale = new Vector3(sm.ProjectRadius, sm.ProjectRadius, sm.ProjectRadius) * 2f;
    RaycastHit hit;
    if (Physics.SphereCast(sm.transform.position, sm.ProjectRadius, footDirection, out hit, sm.ProjectLength, LayerMask.GetMask("Ground"))) {
      footDirection = -hit.normal;
      float length = hit.distance;
      float compression = sm.ProjectLength - length;
      sm.BoardRb.AddForce(-footDirection * (compression * sm.SpringConstant + Vector3.Dot(footDirection, sm.BoardRb.velocity) * sm.SpringDamping));
      sm.footRepresentation.localPosition = footDirection * (hit.distance);
      // sm.footRepresentation.position = hit.point;
    }
    else {
      sm.footRepresentation.localPosition = footDirection * sm.ProjectLength;
    }
    // sm.Board.SetFloat(TurnHash, sm.Turning);
  }

  public override void Exit() {
    sm.Input.OnPushPerformed -= OnPush;
  }
}