using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckGizmo : MonoBehaviour {
  public float RayLength = 1;
  void OnDrawGizmos() {
    Gizmos.color = Color.blue;
    Gizmos.DrawRay(transform.position, transform.forward*RayLength);
    
    Gizmos.color = Color.red;
    Gizmos.DrawRay(transform.position, transform.right*RayLength);
    
    Gizmos.color = Color.green;
    Gizmos.DrawRay(transform.position, transform.up*RayLength);
  }
}
