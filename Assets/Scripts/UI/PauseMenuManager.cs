using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour {
  [SerializeField] GameObject[] Menus;
  [SerializeField] int CurrentMenu;
  [SerializeField] ButtonSelectionHandler PauseMenuDefault, PauseReturnToMenuDefault, CommandMenuDefault;
  [SerializeField] Animator PauseViewfinderAnimator;
  [SerializeField] SkateboardStateMachine CharController;

  [SerializeField] UIExtraInput extraInput;
  [SerializeField] CinemachineInputProvider camInput;
  [SerializeField] GameObject gameplayUI;

  bool paused = false;

  void LateUpdate() {
    if (paused) Time.timeScale = 0;
  }

  public void ChangeMenu(int menu) {
    Menus[CurrentMenu].SetActive(false);
    CurrentMenu = menu;
    Menus[menu].SetActive(true);
    switch (menu) {
      case 0:
        PauseMenuDefault.ManualSelect();
        break;
      
      case 1:
        PauseReturnToMenuDefault.ManualSelect();
        break;

      case 2:
        CommandMenuDefault.ManualSelect();
        break;
    }
  }

  public void ChangeScene(string sceneName) {
    ResumeGame();
    SceneManager.LoadScene(sceneName);
  }

  public void PauseGame() {
    paused = true;
    camInput.enabled = false;
    gameplayUI.SetActive(false);
    PauseViewfinderAnimator.SetBool("enabled", true);
    PauseMenuDefault.ManualSelect();
    extraInput.OnUnpausePerformed += ResumeGame;
    ChangeMenu(0);
  }

  public void ResumeGame() {
    extraInput.OnUnpausePerformed -= ResumeGame;
    PauseViewfinderAnimator.SetBool("enabled", false);
    gameplayUI.SetActive(true);
    camInput.enabled = true;
    paused = false;
    Time.timeScale = 1;
    CharController.Unpause();
    ChangeMenu(0);
  }

  public void QuitGame() {
    Application.Quit();
  }
}