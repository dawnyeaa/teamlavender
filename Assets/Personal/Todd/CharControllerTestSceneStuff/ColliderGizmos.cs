using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ColliderGizmos : MonoBehaviour {
  List<SphereCollider> colliders;
  void OnDrawGizmos() {
    colliders = new List<SphereCollider>(GetComponents<SphereCollider>());
    foreach (SphereCollider collider in colliders) {
      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(transform.TransformPoint(collider.center), collider.radius);
    }
  }
}
