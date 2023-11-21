using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCustoDeck", menuName = "ScriptableObjects/CustoDeck", order = 2)]
public class CustoDeck : ScriptableObject {
  public string idName;
  public Texture2D deckTexture;
  public string deckTitle;
  public string deckArtist;
}
