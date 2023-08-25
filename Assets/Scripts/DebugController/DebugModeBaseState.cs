using UnityEngine;
using Unity.Mathematics;

using UnityEditor;

public abstract class DebugModeBaseState : State {
  protected readonly DebugModeStateMachine sm;

  protected DebugModeBaseState(DebugModeStateMachine stateMachine) {
    sm = stateMachine;
  }
}