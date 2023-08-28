using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
  public static T[] SubArray<T>(this T[] array, int offset, int length) {
    return new ArraySegment<T>(array, offset, length).ToArray();
  }
}

public class DebugFrameHandler : MonoBehaviour {
  private const int _maxBufferSize = 11;
  private DebugFrame[] debugFrames;
  private int frameWriteIndex;
  private int frameReadIndex;
  public int currentFrame;
  public int frameFalloff = 1;
  public LineRenderer[] traceLines;

  public void Awake() {
    debugFrames = new DebugFrame[_maxBufferSize];
    frameWriteIndex = 0;
    frameReadIndex = 0;
  }

  public void ShowTraceLines(bool show) {
    foreach (LineRenderer line in traceLines) {
      line.enabled = show;
    }
  }

  public bool ArrangeTraceLines() {
    Debug.Log("start buffer: " + frameReadIndex);
    Debug.Log(NormalizedToBufferIndex(7));
    int bufferSize = GetBufferSize();
    Vector3[] positions = new Vector3[bufferSize];
    float[] alphas = new float[bufferSize];
    // iterate thru all frames
    for (int i = frameReadIndex, j = 0; j < bufferSize; i = (i + 1) % _maxBufferSize, ++j) {
      positions[j] = debugFrames[i].centerOfMass;

    }
    traceLines[0].positionCount = GetBufferSize();
    traceLines[0].SetPositions(positions);
    Gradient grad = new Gradient();
    GradientColorKey colorKey0 = new GradientColorKey(Color.yellow, 0);
    GradientColorKey colorKey1 = new GradientColorKey(Color.yellow, 1);
    GradientAlphaKey alphaKey = new GradientAlphaKey(1, 1);
    grad.SetKeys(new GradientColorKey[] {colorKey0, colorKey1}, new GradientAlphaKey[] {alphaKey});
    traceLines[0].colorGradient = grad;
    return true;
  }

  public DebugFrame[] SampleFrames(int offset, int length) {
    return debugFrames.SubArray(offset, length);
  }

  public bool PutFrame(DebugFrame frame) {
    bool result = true;
    if ((frameWriteIndex + 1) % _maxBufferSize == frameReadIndex) {
      // uh oh the buffers full
      frameReadIndex = (frameReadIndex + 1) % _maxBufferSize;
      result = false;
    }
    debugFrames[frameWriteIndex] = frame;
    // needs to be before the new write index is assigned
    currentFrame = frameWriteIndex;
    frameWriteIndex = (frameWriteIndex + 1) % _maxBufferSize;
    return result;
  }

  public DebugFrame PeekFrame() {
    if (frameReadIndex == frameWriteIndex) {
      // buffers empty
      return null;
    }

    DebugFrame result = debugFrames[frameReadIndex];
    return result;
  }

  public DebugFrame PopFrame() {
    DebugFrame result = PeekFrame();
    RemoveFrame();
    return result;
  }

  public bool RemoveFrame() {
    if (frameReadIndex == frameWriteIndex) {
      // buffers empty
      return false;
    }

    frameReadIndex = (frameReadIndex + 1) % _maxBufferSize;
    return true;
  }

  private int NormalizedToBufferIndex(int normI) {
    return (normI + frameReadIndex) % _maxBufferSize;
  }

  private int BufferToNormalizedIndex(int buffI) {
    if (buffI < frameReadIndex) {
      buffI += _maxBufferSize;
    }
    return buffI - frameReadIndex;
  }
  
  private int GetBufferSize() {
    int diff = ((frameWriteIndex-frameReadIndex)+_maxBufferSize) % _maxBufferSize;
    return diff;
  }
}
