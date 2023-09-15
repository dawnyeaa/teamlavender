using UnityEngine;

public class WheelSpinParticleHandler : MonoBehaviour {
  public float seed = 0;
  public float scale = 1;
  private float chance = 0;
  private GameObject[] emitters;

  void Awake() {
    emitters = new GameObject[2];
    for (int i = 0; i < transform.childCount; ++i) {
      emitters[i] = transform.GetChild(i).gameObject;
    }
  }

  void Update() {
    var noise = Mathf.PerlinNoise(Time.time*scale, seed);
    SetActiveState(chance > Mathf.Clamp01(noise));
  }

  public void SetChance(float newchance) {
    chance = newchance;
  }

  private void SetActiveState(bool active) {
    foreach (GameObject emitter in emitters) {
      emitter.SetActive(active);
    }
  }
}
