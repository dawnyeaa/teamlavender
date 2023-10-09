using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPointHandler : MonoBehaviour {
  // different ways to get points
  // 1. airtime
  // 2. tricks
  // 3. grind time
  // 4. ???
  PointManager pointSystem;
  public Dictionary<string, int> trickPoints;

  public void CompleteTrick(string trickName) {
    // depending on the trick, add points corresponding to that trick
    if (trickPoints == null || !trickPoints.ContainsKey(trickName)) return;
    pointSystem.AddPoints(trickPoints[trickName]);
  }

  public void ValidateTricks() {
    pointSystem.Validate();
  }

  public void CompleteAndValidateTrick(string trickName) {
    CompleteTrick(trickName);
    ValidateTricks();
  }
}
