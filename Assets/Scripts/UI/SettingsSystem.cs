using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsSystem : MonoBehaviour {
  [SerializeField] private Dictionary<string, bool> BoolDefaults = new Dictionary<string, bool>{
    ["Fullscreen"] = true,
    ["FrankMode"] = true
  };
  [SerializeField] private Dictionary<string, int> IntDefaults = new Dictionary<string, int>{
    ["MasterVolume"] = 10,
    ["MusicVolume"] = 10,
    ["SFXVolume"] = 10
  };
  [SerializeField] private AudioMixer audioMixer;
  [SerializeField] private PickupManager pickupManager;
  [SerializeField] private Vector2 masterVolumeRange = new(-80, 0);
  [SerializeField] private Vector2 SFXVolumeRange = new(-80, 0);
  [SerializeField] private Vector2 musicVolumeRange = new(-80, -10);
  [SerializeField] private Toggle fullscreenToggle;
  [SerializeField] private Toggle frankModeToggle;
  [SerializeField] private Slider masterVolumeSlider;
  [SerializeField] private Slider musicVolumeSlider;
  [SerializeField] private Slider sfxVolumeSlider;
  void Awake() {
    foreach (var boolDefault in BoolDefaults) {
      if (!PlayerPrefs.HasKey(boolDefault.Key)) {
        PlayerPrefs.SetInt(boolDefault.Key, boolDefault.Value ? 1 : 0);
      }
    }
    foreach (var intDefault in IntDefaults) {
      if (!PlayerPrefs.HasKey(intDefault.Key)) {
        PlayerPrefs.SetInt(intDefault.Key, intDefault.Value);
      }
    }
    SaveAndUpdateAll();
    // and the settings menu fields
    SetToggle(fullscreenToggle, PlayerPrefs.GetInt("Fullscreen") == 1);
    SetToggle(frankModeToggle, PlayerPrefs.GetInt("FrankMode") == 1);
    SetSlider(masterVolumeSlider, PlayerPrefs.GetInt("MasterVolume"));
    SetSlider(musicVolumeSlider, PlayerPrefs.GetInt("MusicVolume"));
    SetSlider(sfxVolumeSlider, PlayerPrefs.GetInt("SFXVolume"));
  }

  void Update() {
  }

  public void OnChangeMasterVolume(float value) {
    UpdateMasterVolume(false, value);
  }

  public void OnChangeMusicVolume(float value) {
    UpdateMusicVolume(false, value);
  }

  public void OnChangeSFXVolume(float value) {
    UpdateSFXVolume(false, value);
  }

  public void OnChangeFrankMode(bool value) {
    UpdateFrankMode(false, value);
  }

  public void OnChangeFullscreen(bool value) {
    WriteBoolWithoutUpdate("Fullscreen", value);
  }

  private void UpdateMasterVolume(bool fromDisk = true, float value = 0) {
    float masterVolume;
    if (fromDisk)
      masterVolume = PlayerPrefs.GetInt("MasterVolume");
    else {
      masterVolume = value;
      PlayerPrefs.SetInt("MasterVolume", (int)value);
    }
    masterVolume /= 10f;
    audioMixer?.SetFloat("MasterVolume", Mathf.Lerp(masterVolumeRange.x, masterVolumeRange.y, 1-Mathf.Pow(1-masterVolume, 3)));
  }

  private void UpdateMusicVolume(bool fromDisk = true, float value = 0) {
    float musicVolume;
    if (fromDisk)
      musicVolume = PlayerPrefs.GetInt("MusicVolume");
    else {
      musicVolume = value;
      PlayerPrefs.SetInt("MusicVolume", (int)value);
    }
    musicVolume /= 10f;
    audioMixer?.SetFloat("MusicVolume", Mathf.Lerp(musicVolumeRange.x, musicVolumeRange.y, 1-Mathf.Pow(1-musicVolume, 3)));
  }

  private void UpdateSFXVolume(bool fromDisk = true, float value = 0) {
    float sfxVolume;
    if (fromDisk)
      sfxVolume = PlayerPrefs.GetInt("SFXVolume");
    else {
      sfxVolume = value;
      PlayerPrefs.SetInt("SFXVolume", (int)value);
    }
    sfxVolume /= 10f;
    audioMixer?.SetFloat("SFXVolume", Mathf.Lerp(SFXVolumeRange.x, SFXVolumeRange.y, 1-Mathf.Pow(1-sfxVolume, 3)));
  }

  private void UpdateFullscreen(bool fromDisk = true, bool value = true) {
    bool fullscreen;
    if (fromDisk)
      fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
    else {
      fullscreen = value;
      PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }
    Screen.fullScreen = fullscreen;
  }

  private void UpdateFrankMode(bool fromDisk = true, bool value = true) {
    bool frankMode;
    if (fromDisk)
      frankMode = PlayerPrefs.GetInt("FrankMode") == 1;
    else {
      frankMode = value;
      PlayerPrefs.SetInt("FrankMode", value ? 1 : 0);
    }
    if (pickupManager)
      pickupManager.SetFrankMode(frankMode);
  }

  private void WriteBoolWithoutUpdate(string key, bool value) {
    PlayerPrefs.SetInt(key, value ? 1 : 0);
  }

  public void SaveAndUpdateAll() {
    PlayerPrefs.Save();
    UpdateMasterVolume();
    UpdateMusicVolume();
    UpdateSFXVolume();
    UpdateFullscreen();
    UpdateFrankMode();
  }

  void SetToggle(Toggle toggle, bool value) {
    if (toggle) toggle.isOn = value;
  }

  void SetSlider(Slider slider, int value) {
    if (slider) slider.value = value;
  }
}
