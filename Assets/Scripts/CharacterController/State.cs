public abstract class State {
  public abstract void Enter();
  public abstract void Tick();
  public abstract void Exit();
  public virtual void DrawGizmos(bool selected) { }
  public virtual void LateUpdate() { }
}