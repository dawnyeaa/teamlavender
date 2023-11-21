using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCustomiseSetObject : ScriptableObject {
  [ReadOnly] public float characterHue;
  [ReadOnly] public List<CustoSlotInfo> selections = new();
}
