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
  private const int MAXBUFFERSIZE = 401;
  private const int MAXFALLOFF = 5;
  private const int POSCOUNT = 3;
  private const int DIRCOUNT = 3;
  private DebugFrame[] debugFrames;
  private int frameWriteIndex;
  private int frameReadIndex;
  private List<GameObject> pointGizmoPool;
  private List<GameObject> directionGizmoPool;
  public GameObject pointGizmo, dirGizmo;
  public float pointGizmoScale = 1, dirGizmoScale = 1;
  private int currentFrame = 1;
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
    debugFrames = new DebugFrame[MAXBUFFERSIZE];
    frameWriteIndex = 0;
    frameReadIndex = 0;
    int posGizmoPoolSize = (MAXFALLOFF*2+1)*POSCOUNT;
    pointGizmoPool = new List<GameObject>(posGizmoPoolSize);
    for (int i = 0; i < posGizmoPoolSize; ++i) {
      GameObject newGizmo = Instantiate(pointGizmo, transform);
      newGizmo.SetActive(false);
      pointGizmoPool.Add(newGizmo);
    }
    int dirGizmoPoolSize = (MAXFALLOFF*2+1)*DIRCOUNT;
    directionGizmoPool = new List<GameObject>(dirGizmoPoolSize);
    for (int i = 0; i < dirGizmoPoolSize; ++i) {
      GameObject newGizmo = Instantiate(dirGizmo, transform);
      newGizmo.SetActive(false);
      directionGizmoPool.Add(newGizmo);
    }
  }

  public void ShowTraceLines(bool show) {
    foreach (LineRenderer line in traceLines) {
      line.enabled = show;
    }
    ClearGizmos();
    currentFrame = GetBufferSize()-1;
  }

  public void ClearGizmos() {
    foreach(GameObject lPosGizmo in pointGizmoPool) {
      lPosGizmo.SetActive(false);
    }
    foreach(GameObject lDirGizmo in directionGizmoPool) {
      lDirGizmo.SetActive(false);
    }
  }

  public bool ArrangeTraceLines() {
    int bufferSize = GetBufferSize() - 1;
    Vector3[] COMs = new Vector3[bufferSize+1];
    Vector3[] POCs = new Vector3[bufferSize+1];
    Vector3[] predictedLandings = new Vector3[bufferSize+1];
    GradientColorKey[] COMcolors = new GradientColorKey[] { new GradientColorKey(centerOfMassColorDark, 0) };
    GradientColorKey[] POCcolors = new GradientColorKey[] { new GradientColorKey(pointOfContactColorDark, 0) };
    GradientColorKey[] predictedLandingcolors = new GradientColorKey[] { new GradientColorKey(predictedLandingPositionColorDark, 0) };
    GradientAlphaKey[] alphas = new GradientAlphaKey[] { new GradientAlphaKey(1, 0) };
    ClearGizmos();
    // iterate thru all frames
    for (int i = frameReadIndex, j = 0, k = 0; j < bufferSize+1; i = (i + 1) % MAXBUFFERSIZE, ++j) {
      // draw paths
      COMs[j] = debugFrames[i].centerOfMass;
      POCs[j] = debugFrames[i].pointOfContact;
      predictedLandings[j] = debugFrames[i].predictedLandingPosition;
      // now drawing gizmos
      if (j >= currentFrame-frameFalloff && j <= currentFrame+frameFalloff) {
        // draw center of mass
        GameObject comGizmo = pointGizmoPool[k*POSCOUNT];
        comGizmo.transform.position = debugFrames[i].centerOfMass;
        comGizmo.transform.rotation = pointGizmo.transform.rotation;
        comGizmo.transform.localScale = Vector3.one*pointGizmoScale;
        comGizmo.SetActive(true);
        comGizmo.GetComponent<DebugGizmoMatHandler>().SetColor(centerOfMassColorLight);
        // draw point of contact
        GameObject pocGizmo = pointGizmoPool[(k*POSCOUNT)+1];
        pocGizmo.transform.position = debugFrames[i].pointOfContact;
        pocGizmo.transform.rotation = pointGizmo.transform.rotation;
        pocGizmo.transform.localScale = Vector3.one*pointGizmoScale;
        pocGizmo.SetActive(true);
        pocGizmo.GetComponent<DebugGizmoMatHandler>().SetColor(pointOfContactColorLight);
        // draw predicted landing
        GameObject predLandingGizmo = pointGizmoPool[(k*POSCOUNT)+2];
        predLandingGizmo.transform.position = debugFrames[i].predictedLandingPosition;
        predLandingGizmo.transform.rotation = pointGizmo.transform.rotation;
        predLandingGizmo.transform.localScale = Vector3.one*pointGizmoScale;
        predLandingGizmo.SetActive(true);
        predLandingGizmo.GetComponent<DebugGizmoMatHandler>().SetColor(predictedLandingPositionColorLight);
        // draw down
        if (debugFrames[i].downVector.magnitude > 0) {
          GameObject downGizmo = directionGizmoPool[k*DIRCOUNT];
          downGizmo.transform.position = debugFrames[i].centerOfMass;
          downGizmo.transform.rotation = Quaternion.LookRotation(debugFrames[i].downVector, Vector3.up);
          downGizmo.transform.localScale = Vector3.one*dirGizmoScale;
          downGizmo.SetActive(true);
          downGizmo.GetComponent<DebugGizmoMatHandler>().SetColor(downVectorColorLight);
        }
        // draw damped down
        if (debugFrames[i].dampedDownVector.magnitude > 0) {
          GameObject dampedDownGizmo = directionGizmoPool[(k*DIRCOUNT)+1];
          dampedDownGizmo.transform.position = debugFrames[i].centerOfMass;
          dampedDownGizmo.transform.rotation = Quaternion.LookRotation(debugFrames[i].dampedDownVector, Vector3.up);
          dampedDownGizmo.transform.localScale = Vector3.one*dirGizmoScale;
          dampedDownGizmo.SetActive(true);
          dampedDownGizmo.GetComponent<DebugGizmoMatHandler>().SetColor(dampedDownVectorColorLight);
        }
        // draw ground normal
        if (debugFrames[i].contactNormal.magnitude > 0) {
          GameObject groundNormalGizmo = directionGizmoPool[(k*DIRCOUNT)+2];
          groundNormalGizmo.transform.position = debugFrames[i].pointOfContact;
          groundNormalGizmo.transform.rotation = Quaternion.LookRotation(debugFrames[i].contactNormal, Vector3.up);
          groundNormalGizmo.transform.localScale = Vector3.one*dirGizmoScale;
          groundNormalGizmo.SetActive(true);
          groundNormalGizmo.GetComponent<DebugGizmoMatHandler>().SetColor(contactNormalColorLight);
        }
        k++;
      }
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
    frameFalloff = Mathf.Min(frameFalloff+1, MAXFALLOFF);
  }

  public void DecreaseFrameWindow() {
    frameFalloff = Mathf.Max(frameFalloff-1, 0);
  }

  public DebugFrame[] SampleFrames(int offset, int length) {
    return debugFrames.SubArray(offset, length);
  }

  public bool PutFrame(DebugFrame frame) {
    bool result = true;
    if ((frameWriteIndex + 1) % MAXBUFFERSIZE == frameReadIndex) {
      // uh oh the buffers full
      frameReadIndex = (frameReadIndex + 1) % MAXBUFFERSIZE;
      result = false;
    }
    debugFrames[frameWriteIndex] = frame;
    frameWriteIndex = (frameWriteIndex + 1) % MAXBUFFERSIZE;
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

    frameReadIndex = (frameReadIndex + 1) % MAXBUFFERSIZE;
    return true;
  }

  private int NormalizedToBufferIndex(int normI) {
    return (normI + frameReadIndex) % MAXBUFFERSIZE;
  }

  private int BufferToNormalizedIndex(int buffI) {
    if (buffI < frameReadIndex) {
      buffI += MAXBUFFERSIZE;
    }
    return buffI - frameReadIndex;
  }
  
  private int GetBufferSize() {
    int diff = ((frameWriteIndex-frameReadIndex)+MAXBUFFERSIZE) % MAXBUFFERSIZE;
    return diff;
  }
}
