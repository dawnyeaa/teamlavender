using UnityEngine;

public class EmitterBundle : MonoBehaviour {
  public ParticleSystem[] emitters;

  void Start() {
    emitters = transform.GetComponentsInChildren<ParticleSystem>(true);
  }

  public void Play() {
    foreach (ParticleSystem emitter in emitters) {
      emitter.gameObject.SetActive(false);
      emitter.gameObject.SetActive(true);
    }
  }
}
