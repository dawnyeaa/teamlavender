using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckGizmo : MonoBehaviour {
  public float RayLength = 1;
  public float offset = 0;
  void OnDrawGizmos() {
    Gizmos.color = Color.blue;
    Gizmos.DrawRay(transform.position - transform.up*offset, transform.forward*RayLength);
    
    Gizmos.color = Color.red;
    Gizmos.DrawRay(transform.position - transform.up*offset, transform.right*RayLength);
    
    Gizmos.color = Color.green;
    Gizmos.DrawRay(transform.position - transform.up*offset, transform.up*RayLength);
  }
  void DrawMeGizmos() {
    Gizmos.color = Color.blue;
    Gizmos.DrawRay(transform.position - transform.up*offset, transform.forward*RayLength);
    
    Gizmos.color = Color.red;
    Gizmos.DrawRay(transform.position - transform.up*offset, transform.right*RayLength);
    
    Gizmos.color = Color.green;
    Gizmos.DrawRay(transform.position - transform.up*offset, transform.up*RayLength);
  }
}
