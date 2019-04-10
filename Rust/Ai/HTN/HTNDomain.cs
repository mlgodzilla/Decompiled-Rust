// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.HTNDomain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.AI.Components;
using Apex.AI.Core.HTN;
using Apex.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN
{
  public abstract class HTNDomain : MonoBehaviour, IHTNDomain, IContextProvider, IDisposable
  {
    [ReadOnly]
    public HTNDomain.MovementRule Movement;
    [ReadOnly]
    public float MovementRadius;
    private Vector3 _currentOffset;

    public float SqrMovementRadius
    {
      get
      {
        return this.MovementRadius * this.MovementRadius;
      }
    }

    public abstract BaseNpcContext NpcContext { get; }

    public abstract IHTNContext PlannerContext { get; }

    public abstract IUtilityAI PlannerAi { get; }

    public abstract IUtilityAIClient PlannerAiClient { get; }

    public abstract IAIContext GetContext(Guid aiId);

    public abstract NavMeshAgent NavAgent { get; }

    public abstract List<INpcSensor> Sensors { get; }

    public abstract List<INpcReasoner> Reasoners { get; }

    public byte[] WorldState
    {
      get
      {
        return this.PlannerContext.get_WorldState();
      }
    }

    public byte[] PreviousWorldState
    {
      get
      {
        return this.PlannerContext.get_PreviousWorldState();
      }
    }

    public Stack<PrimitiveTaskSelector> Plan
    {
      get
      {
        return this.PlannerContext.get_HtnPlan();
      }
    }

    public abstract void Initialize(BaseEntity body);

    public abstract void Dispose();

    public abstract void TickDestinationTracker();

    public abstract void Resume();

    public abstract void Pause();

    public abstract Vector3 GetNextPosition(float delta);

    public abstract void ForceProjectileOrientation();

    public void Think()
    {
      this.PlannerContext.set_IsWorldStateDirty(false);
      this.PlannerAiClient.Execute();
      if (this.PlannerContext.get_PlanResult() != 1 && this.PlannerContext.get_PlanResult() != 2 || this.PlannerContext.get_CurrentTask() == null)
        return;
      using (List<IOperator>.Enumerator enumerator = this.PlannerContext.get_CurrentTask().get_Operators().GetEnumerator())
      {
        while (enumerator.MoveNext())
          enumerator.Current?.Abort(this.PlannerContext, this.PlannerContext.get_CurrentTask());
      }
      this.PlannerContext.set_CurrentTask((PrimitiveTaskSelector) null);
    }

    public virtual void Tick(float time)
    {
      this.TickSensors(time);
      this.TickReasoners(time);
      this.TickPlan();
    }

    public virtual void ResetState()
    {
      this.NpcContext.ResetState();
    }

    public abstract Vector3 GetHeadingDirection();

    public abstract Vector3 GetHomeDirection();

    public Vector3 GetLookAroundDirection(float deltaTime)
    {
      return Vector3.op_Addition(this.GetHeadingDirection(), this._currentOffset);
    }

    public virtual void OnPreHurt(HitInfo info)
    {
    }

    public abstract void OnHurt(HitInfo info);

    public abstract void OnSensation(Sensation sensation);

    public abstract float SqrDistanceToSpawn();

    public abstract bool AllowedMovementDestination(Vector3 destination);

    public void TickPlan()
    {
      if (this.PlannerContext.get_PlanState() != 1)
        return;
      if (this.PlannerContext.get_CurrentTask() == null)
        this.PlannerContext.set_CurrentTask(this.PlannerContext.get_HtnPlan().Pop());
      else if (this.PlannerContext.get_CurrentTask().get_State() == 2)
      {
        if (this.PlannerContext.get_HtnPlan().Count > 0)
        {
          this.PlannerContext.set_CurrentTask(this.PlannerContext.get_HtnPlan().Pop());
        }
        else
        {
          this.CompletePlan();
          this.Think();
          return;
        }
      }
      if (!this.PlannerContext.get_CurrentTask().ValidatePreconditions(this.PlannerContext))
      {
        this.AbortPlan();
        this.Think();
      }
      else
      {
        if (this.PlannerContext.get_CurrentTask() == null)
          return;
        if (this.PlannerContext.get_CurrentTask().get_State() == null)
        {
          if ((double) TaskQualifier.TestPreconditions((ITask) this.PlannerContext.get_CurrentTask(), this.PlannerContext) <= 0.0)
          {
            this.PlannerContext.get_CurrentTask().set_State((PrimitiveTaskStateType) 3);
            this.AbortPlan();
            this.Think();
            return;
          }
          this.PlannerContext.get_CurrentTask().set_State((PrimitiveTaskStateType) 1);
          using (List<IOperator>.Enumerator enumerator = this.PlannerContext.get_CurrentTask().get_Operators().GetEnumerator())
          {
            while (enumerator.MoveNext())
              ((IAction) enumerator.Current).Execute((IAIContext) this.PlannerContext);
          }
        }
        int num = 0;
        using (List<IOperator>.Enumerator enumerator = this.PlannerContext.get_CurrentTask().get_Operators().GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            OperatorStateType operatorStateType = enumerator.Current.Tick(this.PlannerContext, this.PlannerContext.get_CurrentTask());
            if (operatorStateType == 3)
            {
              this.PlannerContext.get_CurrentTask().set_State((PrimitiveTaskStateType) 3);
              this.AbortPlan();
              this.Think();
              return;
            }
            if (operatorStateType == 2)
              ++num;
          }
        }
        if (num < this.PlannerContext.get_CurrentTask().get_Operators().Count)
          return;
        this.PlannerContext.get_CurrentTask().set_State((PrimitiveTaskStateType) 2);
      }
    }

    protected virtual void AbortPlan()
    {
      this.PlannerContext.get_HtnPlan().Clear();
      this.PlannerContext.set_PlanState((PlanStateType) 3);
      this.PlannerContext.set_DecompositionScore(int.MaxValue);
      this.PlannerContext.set_CurrentTask((PrimitiveTaskSelector) null);
    }

    protected virtual void CompletePlan()
    {
      this.PlannerContext.set_PlanState((PlanStateType) 2);
      this.PlannerContext.set_DecompositionScore(int.MaxValue);
      this.PlannerContext.set_CurrentTask((PrimitiveTaskSelector) null);
    }

    public void TickReasoners(float time)
    {
      for (int index = 0; index < this.Reasoners.Count; ++index)
      {
        INpcReasoner reasoner = this.Reasoners[index];
        float deltaTime = time - reasoner.LastTickTime;
        if (this.CanTickReasoner(deltaTime, reasoner))
        {
          this.TickReasoner(reasoner, deltaTime, time);
          reasoner.LastTickTime = time + Random.get_value() * 0.075f;
        }
      }
    }

    protected virtual bool CanTickReasoner(float deltaTime, INpcReasoner reasoner)
    {
      return (double) deltaTime >= (double) reasoner.TickFrequency;
    }

    protected abstract void TickReasoner(INpcReasoner reasoner, float deltaTime, float time);

    public void TickSensors(float time)
    {
      for (int index = 0; index < this.Sensors.Count; ++index)
      {
        INpcSensor sensor = this.Sensors[index];
        float deltaTime = time - sensor.LastTickTime;
        if (this.CanTickSensor(deltaTime, sensor))
        {
          this.TickSensor(sensor, deltaTime, time);
          sensor.LastTickTime = time + Random.get_value() * 0.075f;
        }
      }
    }

    protected virtual bool CanTickSensor(float deltaTime, INpcSensor sensor)
    {
      return (double) deltaTime >= (double) sensor.TickFrequency;
    }

    protected abstract void TickSensor(INpcSensor sensor, float deltaTime, float time);

    protected HTNDomain()
    {
      base.\u002Ector();
    }

    public enum MovementRule
    {
      NeverMove,
      RestrainedMove,
      FreeMove,
    }
  }
}
