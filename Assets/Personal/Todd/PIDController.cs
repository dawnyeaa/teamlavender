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
