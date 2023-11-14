using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPosHelper : MonoBehaviour {
  private ParticleSystem sys;
  private ParticleSystemRenderer rend;

  void Start() {
    sys = GetComponent<ParticleSystem>();
    rend = sys.GetComponent<ParticleSystemRenderer>();
  }

  void Update() {
    rend.material.SetVector("_pos", transform.position);
  }
}
