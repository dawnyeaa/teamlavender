using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour {
  public float HeightOffset = 1.5f;
  [SerializeField] private List<(Vector3 pos, Quaternion rot)> SpawnPoints;
  // Start is called before the first frame update
  void Start() {
    UpdateSpawnPoints();
  }

  private void UpdateSpawnPoints() {
    SpawnPoints = new();
    for (int i = 0; i < transform.childCount; ++i) {
      var child = transform.GetChild(i);
      SpawnPoints.Add(new(child.position, child.rotation));
    }
  }

  public List<(Vector3 pos, Quaternion rot)> GetSpawnPoints() {
    UpdateSpawnPoints();
    return SpawnPoints;
  }

  public (Vector3 pos, Quaternion rot) GetNearestSpawnPoint(Vector3 target) {
    UpdateSpawnPoints();
    (Vector3 pos, Quaternion rot) result = new(Vector3.zero, Quaternion.identity);
    float minDistance = float.PositiveInfinity;
    foreach (var spawn in SpawnPoints) {
      var currentDistance = Vector3.Distance(spawn.pos, target);
      if (currentDistance < minDistance) {
        result = spawn;
        minDistance = currentDistance;
      }
    }
    result.pos += HeightOffset * Vector3.up;
    return result;
  }
}
