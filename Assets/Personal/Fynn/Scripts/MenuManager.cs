using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] menus; //mainMenu, settingsMenu, controlsMenu, characterMenu;
    [SerializeField] int currentMenu;
    [SerializeField] Button SettingMenu, MainMenu, ControlsMenu;
    [SerializeField] AudioMixer audioMixer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMenu(int menu)
    {
        menus[currentMenu].SetActive(false);
        currentMenu = menu;
        menus[menu].SetActive(true);
        switch (menu)
        {
            case 0:
                MainMenu.Select();
            break;
            
            case 1:
                ControlsMenu.Select();
            break;
            
            case 2:
                SettingMenu.Select();
            break;
        }
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void VolumeMixer(Slider volumeSlider)
    {
       //audioMixer. = volumeSlider.value
    }
}
