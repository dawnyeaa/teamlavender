using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int volume;
    public bool coinx2;
    public bool indestructible;
    public int coin;
    public int coinValue;

    public PlayerData(SaveManager _saveManager)
    {
        volume = _saveManager.volume;
    }
}
