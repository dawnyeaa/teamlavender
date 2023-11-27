using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CoolTimeController : MonoBehaviour {
  private SkateboardStateMachine sm;
  public float duration = 0.15f;
  public float transitionTime = 0.04f;
  public float coolTimeScale = 0.05f;
  public float coolTimeAirDurationThreshold = 1.5f;
  public AnimationCurve chromAbb;
  public float chromAbbScale = 0.4f;
  private float playingTime = 0;
  private bool playing = false;
  private RendererFeatureDynamicProperties rfdp;
  public Volume fisheye;
  public DynamicCameraController camZoom;

  void Start() {
    rfdp = Camera.main.GetComponent<RendererFeatureDynamicProperties>();
    sm = GetComponent<SkateboardStateMachine>();
  }

  public void StartCoolTime(float timeToLand) {
    if (timeToLand > coolTimeAirDurationThreshold && !playing) {
      playingTime = 0;
      playing = true;
      camZoom.StartCoolTimeJump(timeToLand);
      sm.TryLandVFXTier(2);
    }
  }

  void Update() {
    if (playingTime >= duration) {
      playing = false;
      SetTimeScale(0);
      // fisheye.weight = 0;
      rfdp.ChromAbbIntensity = 0;
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
      // fisheye.weight = t;

      rfdp.ChromAbbIntensity = chromAbb.Evaluate(playingTime/duration) * chromAbbScale;

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
