using System.Collections;
using System.Collections.Generic;
using CircularBuffer;
using UnityEngine;

public class DebugGraph : MonoBehaviour {
  public Dictionary<string, Queue<(float value, float time)>> dataStreams;
  public LineRenderer line;
  public int valueCount = 500;

  void Start() {
    dataStreams = new Dictionary<string, Queue<(float value, float time)>>();
  }

  public void AddToDataStream(string name, (float value, float time) point) {
    if (!dataStreams.ContainsKey(name)) {
      var newStream = new Queue<(float value, float time)>(valueCount);
      dataStreams.Add(name, newStream);
    }
    dataStreams[name].Enqueue(point);
  }
}
