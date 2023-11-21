using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCustomiseSetObject : ScriptableObject {
  [ReadOnly] public float characterHue;
  [ReadOnly] public int characterGradientIndex;
  [ReadOnly] public int deckIndex;
  [ReadOnly] public List<CustoSlotInfo> selections = new();
}
