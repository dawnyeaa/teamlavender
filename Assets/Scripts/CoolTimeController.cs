using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolTimeController : MonoBehaviour {
  public float duration = 0.15f;
  public float transitionTime = 0.04f;
  public float coolTimeScale = 0.05f;
  private float playingTime = 0;
  private bool playing = false;
  private RendererFeatureDynamicProperties rfdp;

  void Start() {
    rfdp = Camera.main.GetComponent<RendererFeatureDynamicProperties>();
  }

  public void StartCoolTime() {
    if (!playing) {
      playingTime = 0;
      playing = true;
    }
  }

  void Update() {
    if (playingTime >= duration) {
      playing = false;
      SetTimeScale(0);
    }

    if (playing) {
      float t;
      if (playingTime < transitionTime) {
        t = Mathf.Clamp01(playingTime / transitionTime);
      }
      else if (playingTime >= duration-transitionTime) {
        t = Mathf.InverseLerp(duration, duration-transitionTime, playingTime);
      }
      else {
        t = 1;
      }
      SetTimeScale(Mathf.SmoothStep(0, 1, t));
      rfdp.StrokeThicknessFactor = t;

      playingTime += Time.unscaledDeltaTime;
    }
    rfdp.StrokesEnabled = playing;
    rfdp.DropShadowEnabled = playing;
  }

  private void SetTimeScale(float t) {
    Time.timeScale = Mathf.Lerp(1, coolTimeScale, Mathf.Clamp01(t));
    rfdp.StrokeFPS = (int)(6f / Time.timeScale);
  }
}
