public class ContinuousDataStepper {
  private float _continuousData;
  private float _steppedData;
  private float _timePerStep;
  private float _timeSinceStep;

  public ContinuousDataStepper(float initData, float stepRate) {
    _continuousData = initData;
    _steppedData = initData;
    _timePerStep = 1/stepRate;
    _timeSinceStep = 0;
  }

  public void SetStepRate(float stepRate) {
    _timePerStep = 1/stepRate;
  }

  public float Tick(float continuousData, float dt) {
    _continuousData = continuousData;

    _timeSinceStep += dt;
    if (_timeSinceStep >= _timePerStep) {
      _timeSinceStep -= _timePerStep;
      _steppedData = continuousData;
    }

    return _steppedData;
  }

  public float TickDelta(float continuousDelta, float dt) {
    return Tick(_continuousData + continuousDelta, dt);
  }

  public float GetStepped() {
    return _steppedData;
  }

  public float GetContinuous() {
    return _continuousData;
  }
}
