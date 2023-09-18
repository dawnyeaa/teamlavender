using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour {
  public static SoundEffectsManager instance;

  [SerializeField] private AudioSource soundFXObject;

  void Awake() {
    instance ??= this;
  }

  public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume) {
    AudioSource audio = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

    audio.clip = audioClip;
    audio.volume = volume;
    audio.Play();

    var clipLength = audio.clip.length;

    Destroy(audio.gameObject, clipLength);
  }

  public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume) {
    var rand = Random.Range(0, audioClip.Length);

    AudioSource audio = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

    audio.clip = audioClip[rand];
    audio.volume = volume;
    audio.Play();

    var clipLength = audio.clip.length;

    Destroy(audio.gameObject, clipLength);
  }
}
