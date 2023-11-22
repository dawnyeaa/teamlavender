using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTrigger : MonoBehaviour {
  public Transform train;
  public Transform character;
  public GameObject posTrainMesh, negTrainMesh;
  public KillZone trainKillZone;
  public Vector3 trainStartPos;
  private Vector3 trainHidePos;
  public Vector3 moveDirection = new(0, 0, -1);
  public float speed = 50f;
  public float ragdollSpeed = 50f;
  public float maxDistance = 200f;
  public Vector3 mirrorNormal = new(1, 0, 0);
  private bool trainGoing = false;
  private float distance = 0;
  void Start() {
    trainHidePos = train.position;
  }
  void Update() {
    if (trainGoing) {
      distance += speed * Time.deltaTime;
      train.position = trainStartPos + moveDirection * distance;
      trainGoing = distance < maxDistance;
    }
    else {
      train.position = trainHidePos;
    }
  }
  void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Player")) {
      // send in the train
      SendTrain();
    }
  }

  private void SendTrain() {
    trainGoing = true;
    train.position = trainStartPos;
    distance = 0;
    trainKillZone.velocity = moveDirection * ragdollSpeed;

    var playerRelativeToTrain = character.position - train.position;
    var positiveTrain = Vector3.Dot(playerRelativeToTrain, mirrorNormal) >= 0;
    posTrainMesh.SetActive(positiveTrain);
    negTrainMesh.SetActive(!positiveTrain);
  }
}
