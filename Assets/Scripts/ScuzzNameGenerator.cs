using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScuzzNameGenerator : MonoBehaviour {
  private TextMeshProUGUI textBox;
  public ScuzzNames nameList;
  public float waveTimeFrequency = 1;
  public float waveIndexFrequency = 1;
  public float waveAmplitude = 1;
  public float shakeAmount = 1;
  public float shakeTurnFrequency = 1;
  public float shakeFrequency = 1;
  public float shakeIndexFrequency = 1;
  public float shakeSmoothing = 1;
  private int effectIndex = 0;
  private string regularNameKey = "isScuzzTitleTaste";

  private TMP_Text textMesh;
  private Mesh mesh;
  private Vector3[] vertices;
  private Vector3[] lastFrameOffsets;
  private float[] lastFrameAngles;
  private float[] lastFrameVelocities;


  void Start() {
    textBox = GetComponent<TextMeshProUGUI>();

    var isTaste = true;
    if (PlayerPrefs.HasKey(regularNameKey)) {
      isTaste = PlayerPrefs.GetInt(regularNameKey) == 1;
      isTaste = !isTaste;
    }
    PlayerPrefs.SetInt(regularNameKey, isTaste ? 1 : 0);

    if (isTaste) {
      textBox.text = nameList.names[0];
    }
    else {
      textBox.text = nameList.names[Random.Range(1, nameList.names.Count)];
    }

    textMesh = GetComponent<TMP_Text>();
    textMesh.ForceMeshUpdate();
    lastFrameOffsets = new Vector3[textMesh.textInfo.characterCount];
    lastFrameAngles = new float[textMesh.textInfo.characterCount];
    lastFrameVelocities = new float[textMesh.textInfo.characterCount];

    effectIndex = Random.Range(0, 3);
  }

  // void Update() {
  //   textMesh.ForceMeshUpdate();
  //   mesh = textMesh.mesh;
  //   vertices = mesh.vertices;

  //   for (int i = 0; i < textMesh.textInfo.characterCount; ++i) {
  //     TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
      
  //     if (c.character == ' ') continue;

  //     var index = c.vertexIndex;

  //     var offset = new Vector3(0, Mathf.Sin(Time.time*waveTimeFrequency+i*waveIndexFrequency)*waveAmplitude, 0);
  //     vertices[index] += offset;
  //     vertices[index + 1] += offset;
  //     vertices[index + 2] += offset;
  //     vertices[index + 3] += offset;
  //   }

  //   mesh.vertices = vertices;
  //   textMesh.canvasRenderer.SetMesh(mesh);
  // }

  // void Update() {
  //   textMesh.ForceMeshUpdate();
  //   mesh = textMesh.mesh;
  //   vertices = mesh.vertices;

  //   for (int i = 0; i < textMesh.textInfo.characterCount; ++i) {
  //     TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
      
  //     if (c.character == ' ') continue;

  //     var index = c.vertexIndex;

  //     Random.InitState(i+(int)(Time.time*shakeFrequency)*137);

  //     var velocity = lastFrameVelocities[i];

  //     Vector3 target = Random.insideUnitCircle.normalized * shakeAmount;
  //     Vector3 offset = Vector3.SmoothDamp(lastFrameOffsets[i], target, ref velocity, shakeSmoothing);
  //     vertices[index] += offset;
  //     vertices[index + 1] += offset;
  //     vertices[index + 2] += offset;
  //     vertices[index + 3] += offset;
  //     lastFrameOffsets[i] = offset;
  //     lastFrameVelocities[i] = velocity;
  //   }

  //   mesh.vertices = vertices;
  //   textMesh.canvasRenderer.SetMesh(mesh);
  // }

  void Update() {
    textMesh.ForceMeshUpdate();
    mesh = textMesh.mesh;
    vertices = mesh.vertices;

    for (int i = 0; i < textMesh.textInfo.characterCount; ++i) {
      TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];
      
      if (c.character == ' ') continue;

      var index = c.vertexIndex;

      Vector3 offset;

      switch (effectIndex) {
        case 1:
          offset = new Vector3(0, Mathf.Sin(Time.time*waveTimeFrequency+i*waveIndexFrequency)*waveAmplitude, 0);
          break;
        case 2:
          Random.InitState(i+(int)(Time.time*shakeTurnFrequency)*137);

          var velocity = lastFrameVelocities[i];
          var angle = Mathf.SmoothDampAngle(lastFrameAngles[i], Random.Range(0, 360), ref velocity, shakeSmoothing);
          offset = Quaternion.AngleAxis(angle, Vector3.forward) * new Vector3(0, Mathf.Sin(Time.time*shakeFrequency+i*shakeIndexFrequency)*shakeAmount);
          lastFrameAngles[i] = angle;
          lastFrameVelocities[i] = velocity;
          break;      
        default:
          offset = Vector3.zero;
          break;
      }

      vertices[index] += offset;
      vertices[index + 1] += offset;
      vertices[index + 2] += offset;
      vertices[index + 3] += offset;
    }

    mesh.vertices = vertices;
    textMesh.canvasRenderer.SetMesh(mesh);
  }
}
