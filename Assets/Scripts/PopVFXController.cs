using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopVFXController : MonoBehaviour {
  void OnEnable() {
    var emitters = GetComponentsInChildren<ParticleSystem>();
    var randomL = Random.value > 0.5f;
    if (emitters.Length == 2) {
      emitters[(randomL?0:1)].Emit(1);
    }
  }
}
