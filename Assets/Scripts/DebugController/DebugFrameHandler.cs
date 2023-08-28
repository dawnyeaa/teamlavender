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
  private const int _maxBufferSize = 401;
  private DebugFrame[] debugFrames;
  private int frameWriteIndex;
  private int frameReadIndex;
  private int currentFrame = 1;
  [Range(0, 1)]
  public float darkAlpha, lightAlpha;
  [Space(10)]
  public Color centerOfMassColorDark, centerOfMassColorLight;
  [Space(10)]
  public Color pointOfContactColorDark, pointOfContactColorLight;
  [Space(10)]
  public Color predictedLandingPositionColorDark, predictedLandingPositionColorLight;
  [Space(10)]
  public Color downVectorColorDark, downVectorColorLight;
  [Space(10)]
  public Color dampedDownVectorColorDark, dampedDownVectorColorLight;
  [Space(10)]
  public Color contactNormalColorDark, contactNormalColorLight;
  [Space(10)]
  public int frameFalloff = 5;
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
    currentFrame = GetBufferSize()-1;
  }

  public bool ArrangeTraceLines() {
    int bufferSize = GetBufferSize() - 1;
    Vector3[] COMs = new Vector3[bufferSize+1];
    Vector3[] POCs = new Vector3[bufferSize+1];
    Vector3[] predictedLandings = new Vector3[bufferSize+1];
    (float val, float pos) startKey = (currentFrame-frameFalloff > 0 ? 0 : 1-(currentFrame/(float)frameFalloff), Mathf.Clamp01((currentFrame-frameFalloff)/(float)bufferSize));
    (float val, float pos) midKey = (1, currentFrame/(float)bufferSize);
    (float val, float pos) endKey = (currentFrame+frameFalloff < bufferSize ? 0 : 1-((bufferSize-currentFrame)/(float)frameFalloff), Mathf.Clamp01((currentFrame+frameFalloff)/(float)bufferSize));
    GradientColorKey[] COMcolors = new GradientColorKey[] {
      new GradientColorKey(Color.Lerp(centerOfMassColorDark, centerOfMassColorLight, startKey.val), startKey.pos),
      new GradientColorKey(Color.Lerp(centerOfMassColorDark, centerOfMassColorLight, midKey.val), midKey.pos),
      new GradientColorKey(Color.Lerp(centerOfMassColorDark, centerOfMassColorLight, endKey.val), endKey.pos-Mathf.Epsilon)
    };
    GradientColorKey[] POCcolors = new GradientColorKey[] {
      new GradientColorKey(Color.Lerp(pointOfContactColorDark, pointOfContactColorLight, startKey.val), startKey.pos),
      new GradientColorKey(Color.Lerp(pointOfContactColorDark, pointOfContactColorLight, midKey.val), midKey.pos),
      new GradientColorKey(Color.Lerp(pointOfContactColorDark, pointOfContactColorLight, endKey.val), endKey.pos-Mathf.Epsilon)
    };
    GradientColorKey[] predictedLandingcolors = new GradientColorKey[] {
      new GradientColorKey(Color.Lerp(predictedLandingPositionColorDark, predictedLandingPositionColorLight, startKey.val), startKey.pos),
      new GradientColorKey(Color.Lerp(predictedLandingPositionColorDark, predictedLandingPositionColorLight, midKey.val), midKey.pos),
      new GradientColorKey(Color.Lerp(predictedLandingPositionColorDark, predictedLandingPositionColorLight, endKey.val), endKey.pos-Mathf.Epsilon)
    };
    GradientAlphaKey[] alphas = new GradientAlphaKey[] {
      new GradientAlphaKey(Mathf.Lerp(darkAlpha, lightAlpha, startKey.val), startKey.pos),
      new GradientAlphaKey(Mathf.Lerp(darkAlpha, lightAlpha, midKey.val), midKey.pos),
      new GradientAlphaKey(Mathf.Lerp(darkAlpha, lightAlpha, endKey.val), endKey.pos-Mathf.Epsilon)
    };
    // iterate thru all frames
    for (int i = frameReadIndex, j = 0; j < bufferSize+1; i = (i + 1) % _maxBufferSize, ++j) {
      COMs[j] = debugFrames[i].centerOfMass;
      POCs[j] = debugFrames[i].pointOfContact;
      predictedLandings[j] = debugFrames[i].predictedLandingPosition;
    }
    traceLines[0].positionCount = GetBufferSize();
    traceLines[0].SetPositions(COMs);
    traceLines[1].positionCount = GetBufferSize();
    traceLines[1].SetPositions(POCs);
    traceLines[2].positionCount = GetBufferSize();
    traceLines[2].SetPositions(predictedLandings);
    Gradient grad = new();
    grad.SetKeys(COMcolors, alphas);
    traceLines[0].colorGradient = grad;
    grad.SetKeys(POCcolors, alphas);
    traceLines[1].colorGradient = grad;
    grad.SetKeys(predictedLandingcolors, alphas);
    traceLines[2].colorGradient = grad;
    return true;
  }

  public void SelectNextFrame() {
    currentFrame = Mathf.Min(currentFrame+1, GetBufferSize()-1);
  }

  public void SelectPrevFrame() {
    currentFrame = Mathf.Max(currentFrame-1, 0);
  }

  public void IncreaseFrameWindow() {
    frameFalloff++;
  }

  public void DecreaseFrameWindow() {
    frameFalloff = Mathf.Max(frameFalloff-1, 1);
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
