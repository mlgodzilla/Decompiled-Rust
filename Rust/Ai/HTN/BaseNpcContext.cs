// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.BaseNpcContext
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.AI;
using Apex.Ai.HTN;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN
{
  public abstract class BaseNpcContext : IHTNContext, IAIContext
  {
    public static List<Item> InventoryLookupCache = new List<Item>(10);

    public abstract void StartDomainDecomposition();

    public abstract PlanResultType PlanResult { get; set; }

    public abstract PlanStateType PlanState { get; set; }

    public abstract Stack<PrimitiveTaskSelector> HtnPlan { get; set; }

    public abstract int DecompositionScore { get; set; }

    public abstract Dictionary<Guid, Stack<IEffect>> AppliedEffects { get; set; }

    public abstract Dictionary<Guid, Stack<IEffect>> AppliedExpectedEffects { get; set; }

    public abstract byte[] WorldState { get; }

    public abstract byte[] PreviousWorldState { get; }

    public abstract bool IsWorldStateDirty { get; set; }

    public abstract Stack<WorldStateInfo>[] WorldStateChanges { get; }

    public abstract List<PrimitiveTaskSelector> DebugPlan { get; }

    public abstract PrimitiveTaskSelector CurrentTask { get; set; }

    public abstract NpcOrientation OrientationType { get; set; }

    public abstract List<NpcPlayerInfo> PlayersInRange { get; }

    public abstract List<NpcPlayerInfo> EnemyPlayersInRange { get; }

    public abstract List<NpcPlayerInfo> EnemyPlayersInLineOfSight { get; }

    public abstract List<NpcPlayerInfo> EnemyPlayersAudible { get; }

    public abstract List<NpcPlayerInfo> PlayersOutsideDetectionRange { get; }

    public abstract NpcPlayerInfo PrimaryEnemyPlayerInLineOfSight { get; set; }

    public abstract NpcPlayerInfo PrimaryEnemyPlayerAudible { get; set; }

    public abstract NpcPlayerInfo GetPrimaryEnemyPlayerTarget();

    public abstract bool HasPrimaryEnemyPlayerTarget();

    public abstract Vector3 GetDirectionToPrimaryEnemyPlayerTargetBody();

    public abstract Vector3 GetDirectionToPrimaryEnemyPlayerTargetHead();

    public abstract Vector3 GetDirectionToMemoryOfPrimaryEnemyPlayerTarget();

    public abstract Vector3 GetDirectionLookAround();

    public abstract Vector3 GetDirectionLastAttackedDir();

    public abstract Vector3 GetDirectionAudibleTarget();

    public abstract Vector3 GetDirectionToAnimal();

    public abstract List<AnimalInfo> AnimalsInRange { get; }

    public abstract Vector3 BodyPosition { get; }

    public abstract BaseNpcMemory BaseMemory { get; }

    public abstract void SetFact(
      byte fact,
      byte value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true);

    public abstract byte GetFact(byte fact);

    public byte GetWorldState(byte fact)
    {
      int index = (int) fact;
      byte num = this.WorldState[index];
      if (this.WorldStateChanges[index].Count > 0)
        num = (byte) this.WorldStateChanges[index].Peek().Value;
      return num;
    }

    public virtual void ResetState()
    {
    }
  }
}
