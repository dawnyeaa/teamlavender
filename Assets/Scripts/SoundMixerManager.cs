using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour {
  [SerializeField] private AudioMixer mixer;

  public void SetMasterVolume(float level) {
    mixer.SetFloat("MasterVolume", level);
  }
  
  public void SetSFXVolume(float level) {
    mixer.SetFloat("SFXVolume", level);
  }
  
  public void SetMusicVolume(float level) {
    mixer.SetFloat("MusicVolume", level);
  }
}
