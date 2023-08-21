using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSensWrapper : MonoBehaviour {
  public delegate void HeadHitCallback();
  public HeadHitCallback callbacks;
  public float minZoneSize = 0.24f;
  public float maxZoneSize = 0.32f;
  public Transform vis;
  [ReadOnly] public float zoneSize = 0.24f;
  private float zoneSizeT = 0;
  private SphereCollider coll;
  private MeshRenderer mesh;

  public void Start() {
    coll = GetComponent<SphereCollider>();
    mesh = vis.GetComponent<MeshRenderer>();
  }

  public void FixedUpdate() {
    zoneSize = Mathf.Lerp(minZoneSize, maxZoneSize, zoneSizeT);
    coll.radius = zoneSize;
    vis.localScale = zoneSize*2f * Vector3.one;
  }

  public void SetT(float t) {
    zoneSizeT = t;
  }

  public void SetShow(bool show) {
    mesh.enabled = show;
  }

  public void AddCallback(HeadHitCallback callback) {
    callbacks += callback;
  }

  private void OnTriggerEnter(Collider other) {
    // Debug.Log("here");
    callbacks?.Invoke();
  }
}
