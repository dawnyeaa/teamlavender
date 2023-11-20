using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustoDeckStorage : MonoBehaviour {
  public static CustoDeckStorage instance;
  public CustoDeck[] custoDecks;

  void Awake() {
    instance ??= this;
  }
}
