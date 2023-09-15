using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    //Public.
    public int volume;

    //Private.
    [SerializeField] MenuManager _menuManager;
    [SerializeField] SaveManager _saveManager;

    // Start is called before the first frame update
    void Start()
    {
         
        // PlayerData data = SaveSystem.LoadMoney();

        // if (data != null)
        // {
        //     _menuManager.volume = data.volume;
        //     _menuManager.SetVolume();
        //     Debug.Log("Volume set to: " + data.volume);
        // }
        // else
        // {
        //     _menuManager.volume = 20;
        // }
    }
    public void save()
    {
        volume = _menuManager.volume;
        SaveSystem.SaveMoney(_saveManager);
    }
}
