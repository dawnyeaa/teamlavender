using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] menus; //mainMenu, settingsMenu, controlsMenu, characterMenu;
    [SerializeField] int currentMenu;
    int currentResolutionShowing;
    public int volume;
    [SerializeField] Button SettingMenu, MainMenu, ControlsMenu;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioListener audioListener;
    [SerializeField] Slider volumeSlider;
    [SerializeField] SaveManager saveManager;
    [SerializeField] Resolution[] resolutions;
    [SerializeField] TMP_Dropdown dropdown;
    List<string> resolutionsList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;

        dropdown.ClearOptions();
        currentResolutionShowing = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string ResolutionValue = resolutions[i].width + "x" + resolutions[i].height;
            resolutionsList.Add(ResolutionValue);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionShowing = i;
            }
        }
        dropdown.AddOptions(resolutionsList);
        dropdown.value = currentResolutionShowing;
        dropdown.RefreshShownValue();
        
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

    public void VolumeMixer()
    {
        AudioListener.volume = volumeSlider.value;
        volume = (int)volumeSlider.value;
        saveManager.save();
    }
    public void SetVolume()
    {
        volumeSlider.value = volume;
    }

    public void ChangeResolution(int Index)
    {
       Resolution resolution = resolutions[Index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
