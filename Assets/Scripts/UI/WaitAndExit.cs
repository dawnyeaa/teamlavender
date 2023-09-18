using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WaitAndExit : StateMachineBehaviour {
  public float duration = 3;
  private float currentTime = 0;
  // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
  override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    currentTime = 0;
  }

  public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    if (currentTime >= duration) {
      animator.SetBool("show", false);
    }
    currentTime += Time.deltaTime;
  }
}
