using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int volume;

    public PlayerData(SaveManager _saveManager)
    {
        volume = _saveManager.volume;
    }
}
