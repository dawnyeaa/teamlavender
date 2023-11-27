using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailHelper : MonoBehaviour {
  private Material mat;
  private TrailRenderer trail;

  void Start() {
    trail = GetComponent<TrailRenderer>();
    mat = trail.material;
  }

  void Update() {
    mat.SetFloat("_length", GetTrailLength(trail));
  }

  public static float GetTrailLength(TrailRenderer trailRenderer) {
    // You store the count since the created array might be bigger than the actual pointCount

    // Note: apparently the API is wrong?! There it says starting from version 2018.1 the parameter is "out"
    // if you get an error for the "out" anyway or if you use older versions instead use
    var points = new Vector3[trailRenderer.positionCount]; 
    var count = trailRenderer.GetPositions(points);
    // var count = trailRenderer.GetPositions(out var points);

    // If there are not at least 2 points .. well there is nothing to measure
    if(count < 2) return 0f;

    var length = 0f;

    // Store the first position 
    var start = points[0];

    // Iterate through the rest of positions
    for(var i = 1; i < count; i++) {
      // get the current position
      var end = points[i];
      // Add the distance to the last position
      // basically the same as writing
      //length += (end - start).magnitude;
      length += Vector3.Distance(start, end);
      // update the start position for the next iteration
      start = end;
    }
    return length;
  }
}
