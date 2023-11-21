using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CharacterPointHandler : MonoBehaviour {
  // different ways to get points
  // 1. airtime
  // 2. tricks
  // 3. grind time
  // 4. manny time
  // 5. air turns
  // 6. ???
  PointManager pointSystem;
  [SerializeField] TextMeshProUGUI groundSpeedDisplay, slowSpeedDisplay, maxSpeedDisplay;
  [SerializeField] Image speedometerDisplay;
  public ComboDisplayManager comboDisplayManager;
  private Material speedometerDisplayMat;
  public int pointsPerHalfTurn = 10;
  public float turnTolerance = 0.1f;
  int lastHalfturns = 0;
  bool onGround = true;
  float groundSpeed = 0;
  float cwturnAmount = 0;
  float ccwturnAmount = 0;
  bool isGoofy = false;
  bool airborneGoofy = false;
  Vector3 lastUp = Vector3.up;
  Vector3 lastForward;
  public float groundSpeedSlowSpeed = 0.1f;
  public float groundSpeedSlowDuration = 1f;
  float slowDurationTimer = 0;

  void Start() {
    pointSystem = PointManager.instance;
    speedometerDisplayMat = new(speedometerDisplay.material);
    speedometerDisplay.material = speedometerDisplayMat;
    if (slowSpeedDisplay != null)
      slowSpeedDisplay.text = groundSpeedSlowSpeed.ToString("F");
    if (speedometerDisplayMat)
      speedometerDisplayMat.SetFloat("_slowSpeedThreshold", groundSpeedSlowSpeed);
    lastForward = transform.forward;
  }

  void Update() {

  }

  public void CompleteTrick(Combo trick) {
    // depending on the trick, add points corresponding to that trick
    pointSystem.AddPoints(trick._ComboTrickValue);
  }

  public void ValidateTricks() {
    pointSystem.Validate();
  }

  public void CompleteAndValidateTrick(Combo trick) {
    CompleteTrick(trick);
    ValidateTricks();
  }

  public void Die() {
    pointSystem.EndLine();
  }

  private void UpdateRotation() {
    var maxTurn = Mathf.Max(cwturnAmount, ccwturnAmount);
    if (maxTurn < (0.5f-turnTolerance)) return;

    var halfturns = Mathf.FloorToInt((maxTurn+turnTolerance)*2f);
    if (halfturns <= lastHalfturns) return;

    var fs = cwturnAmount > ccwturnAmount ^ airborneGoofy;

    // here we update the trick display to show the turn
    comboDisplayManager.SetTurnModifiers(halfturns, fs);
    comboDisplayManager.SetComboDisplay();

    lastHalfturns = halfturns;
  }

  private void ResolveRotation() {
    if (cwturnAmount < (0.5f-turnTolerance) && ccwturnAmount < (0.5f-turnTolerance)) {
      cwturnAmount = ccwturnAmount = 0;
      lastHalfturns = 0;
      return;
    }
    var fs = cwturnAmount > ccwturnAmount ^ airborneGoofy;
    var halfturns = Mathf.FloorToInt((Mathf.Max(cwturnAmount, ccwturnAmount)*2f)+turnTolerance);

    // pointSystem.AddPoints(halfturns * pointsPerHalfTurn);
    cwturnAmount = ccwturnAmount = 0;
    lastHalfturns = 0;
  }

  private void Landed() {
    ResolveRotation();
    // ValidateTricks();
  }

  private void Launched() {
    airborneGoofy = isGoofy;
  }

  public void SetMaxSpeed(float maxSpeed) {
    if (maxSpeedDisplay != null)
      maxSpeedDisplay.text = maxSpeed.ToString("F");
    if (speedometerDisplayMat)
      speedometerDisplayMat.SetFloat("_maxSpeed", maxSpeed);
  }

  public void SetSpeed(float speed) {
    groundSpeed = speed;
    if (groundSpeedDisplay != null)
      groundSpeedDisplay.text = groundSpeed.ToString("F");
    if (speedometerDisplayMat)
      speedometerDisplayMat.SetFloat("_currentSpeed", speed);
    if (groundSpeed < groundSpeedSlowSpeed) {
      if (slowDurationTimer < groundSpeedSlowDuration) {
        slowDurationTimer += Time.deltaTime;
      }
      else {
        slowDurationTimer = 0;
        pointSystem.EndLine();
      }
    }
    else {
      slowDurationTimer = 0;
    }
  }

  public void SetGrounded(bool isOnGround) {
    if (!onGround && isOnGround) Landed();
    if (onGround && !isOnGround) Launched();
    onGround = isOnGround;
  }

  public void SetOrientation(Vector3 up, Vector3 forward) {
    var forwardOldSpace = Quaternion.FromToRotation(up, lastUp) * forward;
    var angle = Vector3.SignedAngle(lastForward, forwardOldSpace, lastUp);
    if (angle > 0) {
      cwturnAmount += angle/360f;
    }
    else if (angle < 0) {
      ccwturnAmount -= angle/360f;
    }
    else {
      return;
    }
    lastForward = forward;
    lastUp = up;
    UpdateRotation();
  }

  public void SetGoofy(bool goofy) {
    isGoofy = goofy;
  }
}
