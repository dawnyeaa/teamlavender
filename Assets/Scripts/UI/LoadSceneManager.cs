using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour {
  public GameObject LoadingScreen;
  public Image LoadingBarFill;
  public AudioSource MenuMusic;

  public void LoadScene(int scene) {
    MenuMusic.Stop();

    LoadingScreen.SetActive(true);
    
    StartCoroutine(LoadSceneAsync(scene));
  }

  IEnumerator LoadSceneAsync(int scene) {
    AsyncOperation op = SceneManager.LoadSceneAsync(scene);

    while (!op.isDone) {
      float progress = Mathf.Clamp01(op.progress / 0.9f);

      LoadingBarFill.fillAmount = progress;

      yield return null;
    }
  }
}
