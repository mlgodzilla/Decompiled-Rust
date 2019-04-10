// Decompiled with JetBrains decompiler
// Type: BaseAIBrain`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BaseAIBrain<T> : EntityComponent<T> where T : BaseEntity
{
  public BaseAIBrain<T>.BasicAIState[] AIStates;
  public const int AIStateIndex_UNSET = 0;
  public int _currentState;
  public Vector3 mainInterestPoint;

  public virtual bool ShouldThink()
  {
    return true;
  }

  public virtual void DoThink()
  {
  }

  public T GetEntity()
  {
    return this.baseEntity;
  }

  public void Awake()
  {
    this.InitializeAI();
  }

  public virtual void InitializeAI()
  {
  }

  public virtual void AddState(BaseAIBrain<T>.BasicAIState newState, int newIndex)
  {
    newState.SetIndex(newIndex);
    this.AIStates[newIndex] = newState;
    newState.brain = this;
    newState.Reset();
  }

  public BaseAIBrain<T>.BasicAIState GetCurrentState()
  {
    if (this.AIStates == null)
      return (BaseAIBrain<T>.BasicAIState) null;
    return this.AIStates[this._currentState];
  }

  public BaseAIBrain<T>.BasicAIState GetState(int index)
  {
    return this.AIStates[index];
  }

  public void SwitchToState(int newState)
  {
    BaseAIBrain<T>.BasicAIState currentState = this.GetCurrentState();
    BaseAIBrain<T>.BasicAIState state = this.GetState(newState);
    if (currentState != null)
    {
      if (currentState == state || !currentState.CanInterrupt())
        return;
      currentState.StateLeave();
    }
    this._currentState = newState;
    state.StateEnter();
  }

  public virtual void AIThink(float delta)
  {
    BaseAIBrain<T>.BasicAIState currentState = this.GetCurrentState();
    currentState?.StateThink(delta);
    if (currentState != null && !currentState.CanInterrupt())
      return;
    float num = 0.0f;
    int newState = 0;
    BaseAIBrain<T>.BasicAIState basicAiState = (BaseAIBrain<T>.BasicAIState) null;
    for (int index = 0; index < this.AIStates.Length; ++index)
    {
      BaseAIBrain<T>.BasicAIState aiState = this.AIStates[index];
      if (aiState != null)
      {
        float weight = aiState.GetWeight();
        if ((double) weight > (double) num)
        {
          num = weight;
          newState = index;
          basicAiState = aiState;
        }
      }
    }
    if (basicAiState == currentState)
      return;
    this.SwitchToState(newState);
  }

  public class BasicAIState
  {
    private int myIndex;
    public BaseAIBrain<T> brain;
    protected float _timeInState;
    protected float _lastStateExitTime;

    public virtual void StateEnter()
    {
      this._timeInState = 0.0f;
    }

    public virtual void StateThink(float delta)
    {
      this._timeInState += delta;
    }

    public virtual void StateLeave()
    {
      this._timeInState = 0.0f;
      this._lastStateExitTime = Time.get_time();
    }

    public virtual bool CanInterrupt()
    {
      return true;
    }

    public virtual float GetWeight()
    {
      return 0.0f;
    }

    public void SetIndex(int newIndex)
    {
      if (this.myIndex != 0)
        return;
      this.myIndex = newIndex;
    }

    public float TimeInState()
    {
      return this._timeInState;
    }

    public float TimeSinceState()
    {
      return Time.get_time() - this._lastStateExitTime;
    }

    public void Reset()
    {
      this._timeInState = 0.0f;
    }

    public bool IsInState()
    {
      if (Object.op_Inequality((Object) this.brain, (Object) null))
        return this.brain.GetCurrentState() == this;
      return false;
    }

    public virtual void DrawGizmos()
    {
    }

    public T GetEntity()
    {
      return this.brain.GetEntity();
    }
  }
}
