using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour {
  public static SoundEffectsManager instance;

  [SerializeField] private AudioSource soundFXObject;
  private List<AudioSource> loopingSounds;
  private SortedList<int, int> availableSlots;

  void Awake() {
    instance ??= this;
    loopingSounds = new List<AudioSource>();
    availableSlots = new SortedList<int, int>();
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

  public int PlayLoopingSoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume) {
    AudioSource audio = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

    audio.clip = audioClip;
    audio.volume = volume;
    audio.loop = true;
    audio.Play();

    var newFXIndex = GetNextAvailableSlot();
    if (loopingSounds.Count > newFXIndex) {
      loopingSounds.RemoveAt(newFXIndex);
      loopingSounds.Insert(newFXIndex, audio);
    }
    else {
      loopingSounds.Add(audio);
    }

    return newFXIndex;
  }

  public void StopLoopingSoundFXClip(int index) {
    availableSlots.Add(index, index);
    Destroy(loopingSounds[index].gameObject);
  }

  private int GetNextAvailableSlot() {
    if (availableSlots.Count == 0) {
      return loopingSounds.Count;
    }
    var slot = availableSlots.Keys[0];
    availableSlots.RemoveAt(slot);
    return slot;
  }

  public void SetLoopingFXVolume(int index, float volume) {
    loopingSounds[index].volume = volume;
  }
}
