using UnityEngine;

public abstract class StateMachine : MonoBehaviour {
  private State currentState;
  protected bool fixedUpdate = true;

  public void SwitchState(State state) {
    currentState?.Exit();
    currentState = state;
    currentState.Enter();
  }

  private void FixedUpdate() {
    if (fixedUpdate) {
      currentState?.Tick();
    }
  }

  private void Update() {
    if (!fixedUpdate) {
      currentState?.Tick();
    }
  }
  
  private void OnDrawGizmos()
  {
    currentState?.DrawGizmos(false);
  }
  
  private void OnDrawGizmosSelected()
  {
    currentState?.DrawGizmos(true);
  }
}