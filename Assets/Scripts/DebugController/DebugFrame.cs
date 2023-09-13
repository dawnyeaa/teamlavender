using UnityEngine;

public class DebugFrame {
  // positions
  public Vector3 centerOfMass;
  public Vector3 pointOfContact;
  public Vector3 predictedLandingPosition;

  // directions
  public Vector3 facing;
  public Vector3 velocity;
  public Vector3 downVector;
  public Vector3 dampedDownVector;
  public Vector3 contactNormal;
}