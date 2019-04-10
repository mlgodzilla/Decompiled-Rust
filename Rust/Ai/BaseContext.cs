// Decompiled with JetBrains decompiler
// Type: Rust.Ai.BaseContext
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class BaseContext : IAIContext
  {
    public List<BasePlayer> Players = new List<BasePlayer>();
    public List<BaseNpc> Npcs = new List<BaseNpc>();
    public List<BasePlayer> PlayersBehindUs = new List<BasePlayer>();
    public List<BaseNpc> NpcsBehindUs = new List<BaseNpc>();
    public List<TimedExplosive> DeployedExplosives = new List<TimedExplosive>(1);
    public Memory Memory;
    public BasePlayer ClosestPlayer;
    public BasePlayer EnemyPlayer;
    public BaseNpc EnemyNpc;
    public float LastTargetScore;
    public float LastEnemyPlayerScore;
    public float LastEnemyNpcScore;
    public float NextRoamTime;

    public BaseContext(IAIAgent agent)
    {
      this.AIAgent = agent;
      this.Entity = agent.Entity;
      this.sampledPositions = new List<Vector3>();
      this.Memory = new Memory();
    }

    public Vector3 lastSampledPosition { get; set; }

    public List<Vector3> sampledPositions { get; private set; }

    public IAIAgent AIAgent { get; private set; }

    public BaseCombatEntity Entity { get; private set; }

    public Vector3 Position
    {
      get
      {
        if (this.Entity.IsDestroyed || Object.op_Equality((Object) ((Component) this.Entity).get_transform(), (Object) null))
          return Vector3.get_zero();
        return this.Entity.ServerPosition;
      }
    }

    public Vector3 EnemyPosition
    {
      get
      {
        if (Object.op_Inequality((Object) this.EnemyPlayer, (Object) null))
          return this.EnemyPlayer.ServerPosition;
        if (!Object.op_Inequality((Object) this.EnemyNpc, (Object) null))
          return Vector3.get_zero();
        return this.EnemyNpc.ServerPosition;
      }
    }

    public byte GetFact(BaseNpc.Facts fact)
    {
      return this.AIAgent.GetFact(fact);
    }

    public void SetFact(BaseNpc.Facts fact, byte value)
    {
      this.AIAgent.SetFact(fact, value, true, true);
    }

    public byte GetFact(NPCPlayerApex.Facts fact)
    {
      return this.AIAgent.GetFact(fact);
    }

    public void SetFact(
      NPCPlayerApex.Facts fact,
      byte value,
      bool triggerCallback = true,
      bool onlyTriggerCallbackOnDiffValue = true)
    {
      this.AIAgent.SetFact(fact, value, triggerCallback, onlyTriggerCallbackOnDiffValue);
    }
  }
}
