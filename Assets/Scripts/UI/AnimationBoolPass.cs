using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnimationBoolPass : MonoBehaviour {
  public Animator anim;
  public MenuManager mm;
  public EventSystem eventsys;
  public GameObject defaultSelect;

  public void SetTheBool(bool Bool) {
    anim?.SetBool("show", Bool);
    eventsys.SetSelectedGameObject(defaultSelect);
  }

  public void EnableMainMenu() {
    mm.EnableMenu();
  }
}
