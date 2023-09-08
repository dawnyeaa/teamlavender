using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDController {
  public enum DerivativeMeasurement {
    Velocity,
    ErrorRateOfChange
  }
  public float proportionalGain;
  public float derivativeGain;
  public float errorLast, valueLast;
  public DerivativeMeasurement derivativeMeasurement;
  public bool derivativeInitialised;
  public float Update(float dt, float currentValue, float targetValue) {
    float error = targetValue - currentValue;

    float P = proportionalGain * error;

    float errorRateOfChange = (error - errorLast) / dt;
    errorLast = error;
    
    float valueRateOfChange = (currentValue - valueLast) / dt;
    valueLast = currentValue;

    float deriveMeasure = 0;

    if (derivativeInitialised) {
      if (derivativeMeasurement == DerivativeMeasurement.Velocity) {
        deriveMeasure = -valueRateOfChange;
      }
      else {
        deriveMeasure = errorRateOfChange;
      }
    }
    else {
      derivativeInitialised = true;
    }

    float D = derivativeGain * deriveMeasure;

    return P + D;
  }
}

public class PIDController3 {
  public float proportionalGain, derivativeGain;
  private PIDController pid1, pid2, pid3;

  public PIDController3() {
    pid1 = new();
    pid2 = new();
    pid3 = new();
  }

  public Vector3 Update(float dt, Vector3 currentValue, Vector3 targetValue) {
    Vector3 result = Vector3.zero;
    pid1.proportionalGain = proportionalGain;
    pid1.derivativeGain = derivativeGain;
    result.x = pid1.Update(dt, currentValue.x, targetValue.x);
    pid2.proportionalGain = proportionalGain;
    pid2.derivativeGain = derivativeGain;
    result.y = pid2.Update(dt, currentValue.y, targetValue.y);
    pid3.proportionalGain = proportionalGain;
    pid3.derivativeGain = derivativeGain;
    result.z = pid3.Update(dt, currentValue.z, targetValue.z);
    return result;
  }
}
