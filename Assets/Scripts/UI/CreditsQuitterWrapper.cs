using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsQuitterWrapper : MonoBehaviour {
  public MenuManager menu;
  public void ExitCredits() {
    menu.ExitCredits(true);
  }
}
