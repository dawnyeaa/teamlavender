using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject[] menus; //mainMenu, settingsMenu, controlsMenu, characterMenu;
    [SerializeField] ButtonSelectionHandler[] menuDefaults;
    [SerializeField] int currentMenu;
    private EventSystem eventsys;

    void Awake() {
        eventsys = FindObjectOfType<EventSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void EnableMenu() 
    {
        ChangeMenu(0);
    }

    public void ChangeMenu(int menu)
    {
        menus[currentMenu].SetActive(false);
        currentMenu = menu;
        menus[menu].SetActive(true);
        menuDefaults[menu].ManualSelect();
    }

    public void SelectButton(Button button) {
        eventsys.SetSelectedGameObject(button.gameObject);
        button.Select();
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
