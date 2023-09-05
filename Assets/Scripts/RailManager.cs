using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RailManager : MonoBehaviour {
  public List<Rail> Rails;
  public float ScanDistance = 10f;
  public float LockDistance = 0.5f;
  public float MinRailSpeed = 0.1f;
  public float RailLockAngleDegrees = 45f;
  private float cachedRailLockAngleDeg = 45f;
  private float minValidRailLockCos, maxValidRailLockCos;

  void Start() {
    Rails = FindObjectsOfType<Rail>().ToList();
  }

  void Awake() {
    CacheRailLockCos();
  }

  void OnValidate() {
    if (cachedRailLockAngleDeg != RailLockAngleDegrees) {
      CacheRailLockCos();
    }
  }

  private void CacheRailLockCos() {
    cachedRailLockAngleDeg = RailLockAngleDegrees;
    var midAngle = 45f;
    var minAngle = midAngle - (cachedRailLockAngleDeg/2f);
    var maxAngle = midAngle + (cachedRailLockAngleDeg/2f);
    minValidRailLockCos = Mathf.Cos(maxAngle);
    maxValidRailLockCos = Mathf.Cos(minAngle);
  }

  public Rail CheckRails(Vector3 boardPos, Vector3 velocity) {
    // conditions to validate a rail
    // 1. the rail center must be within the scan distance ✅
    // 2. the nearest point must be within lock distance ✅
    // 3. must be falling 🔙 implement in char
    // 4. pos must be within angle window ✅
    // 5. must have velocity along length of rail ✅
    Rail lockedRail = null;
    var nearestPoint = Vector3.zero;
    var bestDist = float.PositiveInfinity;
    foreach (Rail rail in Rails) {
      if (Vector3.Distance(boardPos, rail.Position) < ScanDistance) {
        Debug.Log("passed 1");
        if (Mathf.Abs(Vector3.Dot(velocity, rail.RailVector.normalized)) > MinRailSpeed) {
          Debug.Log("passed 2");
          var newPoint = rail.GetNearestPoint(boardPos);
          var railToBoard = boardPos - newPoint;
          var dist = railToBoard.magnitude;
          if (dist < LockDistance && dist < bestDist) {
            Debug.Log("passed 3");
            var flatRailToBoard = new Vector3(railToBoard.x, 0, railToBoard.z).normalized;
            if (Vector3.Dot(flatRailToBoard, rail.RailOutside) < 0) return null;
            Debug.Log("passed 4");
            var incomingAngleDot = Vector3.Dot(railToBoard.normalized, flatRailToBoard);
            if (Vector3.Dot(railToBoard.normalized, Vector3.up) > 0 && incomingAngleDot > minValidRailLockCos && incomingAngleDot < maxValidRailLockCos) {
              Debug.Log("passed 5");
              bestDist = dist;
              nearestPoint = newPoint;
              lockedRail = rail;
            }
          }
        }
      }
    }

    return lockedRail;
  }
}
