// Decompiled with JetBrains decompiler
// Type: Rust.Ai.HTN.ScientistJunkpile.ScientistJunkpileContext
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Apex.Ai.HTN;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile
{
  [Serializable]
  public class ScientistJunkpileContext : BaseNpcContext, IDisposable
  {
    [ReadOnly]
    public List<CoverPoint> CoverPoints = new List<CoverPoint>();
    [ReadOnly]
    [SerializeField]
    public bool _isWorldStateDirty;
    [SerializeField]
    private byte[] _worldState;
    [ReadOnly]
    [SerializeField]
    private byte[] _previousWorldState;
    [ReadOnly]
    [SerializeField]
    private int _decompositionScore;
    [SerializeField]
    [ReadOnly]
    private List<PrimitiveTaskSelector> _debugPlan;
    private static Stack<WorldStateInfo>[] _worldStateChanges;
    public ScientistJunkpileContext.WorldStateChangedEvent OnWorldStateChangedEvent;
    [ReadOnly]
    public bool HasVisitedLastKnownEnemyPlayerLocation;
    [ReadOnly]
    public HTNPlayer Body;
    [ReadOnly]
    public ScientistJunkpileDomain Domain;
    [ReadOnly]
    public ScientistJunkpileMemory Memory;
    [ReadOnly]
    public AiLocationManager Location;
    [ReadOnly]
    public JunkPile Junkpile;
    [Header("Cover")]
    [ReadOnly]
    public CoverPointVolume CoverVolume;
    [ReadOnly]
    public CoverPoint BestFlankCover;
    [ReadOnly]
    public CoverPoint BestAdvanceCover;
    [ReadOnly]
    public CoverPoint BestRetreatCover;
    [ReadOnly]
    public CoverPoint ReservedCoverPoint;
    [ReadOnly]
    public float ReservedCoverTime;

    public override PlanResultType PlanResult { get; set; }

    public override PlanStateType PlanState { get; set; }

    public override Stack<PrimitiveTaskSelector> HtnPlan { get; set; } = new Stack<PrimitiveTaskSelector>();

    public override Dictionary<Guid, Stack<IEffect>> AppliedEffects { get; set; } = new Dictionary<Guid, Stack<IEffect>>(20);

    public override Dictionary<Guid, Stack<IEffect>> AppliedExpectedEffects { get; set; } = new Dictionary<Guid, Stack<IEffect>>(20);

    public override bool IsWorldStateDirty
    {
      get
      {
        return this._isWorldStateDirty;
      }
      set
      {
        this._isWorldStateDirty = value;
      }
    }

    public override byte[] WorldState
    {
      get
      {
        return this._worldState;
      }
    }

    public override byte[] PreviousWorldState
    {
      get
      {
        return this._previousWorldState;
      }
    }

    public override Stack<WorldStateInfo>[] WorldStateChanges
    {
      get
      {
        return ScientistJunkpileContext._worldStateChanges;
      }
    }

    public override int DecompositionScore
    {
      get
      {
        return this._decompositionScore;
      }
      set
      {
        this._decompositionScore = value;
      }
    }

    public override PrimitiveTaskSelector CurrentTask { get; set; }

    public override List<PrimitiveTaskSelector> DebugPlan
    {
      get
      {
        return this._debugPlan;
      }
    }

    public override NpcOrientation OrientationType { get; set; }

    public override List<NpcPlayerInfo> PlayersInRange { get; } = new List<NpcPlayerInfo>();

    public override List<NpcPlayerInfo> EnemyPlayersInRange { get; } = new List<NpcPlayerInfo>();

    public override List<NpcPlayerInfo> EnemyPlayersInLineOfSight { get; } = new List<NpcPlayerInfo>();

    public override List<NpcPlayerInfo> EnemyPlayersAudible { get; } = new List<NpcPlayerInfo>();

    public override List<NpcPlayerInfo> PlayersOutsideDetectionRange { get; } = new List<NpcPlayerInfo>();

    public override NpcPlayerInfo PrimaryEnemyPlayerInLineOfSight { get; set; }

    public override NpcPlayerInfo PrimaryEnemyPlayerAudible { get; set; }

    public override NpcPlayerInfo GetPrimaryEnemyPlayerTarget()
    {
      if (Object.op_Inequality((Object) this.PrimaryEnemyPlayerInLineOfSight.Player, (Object) null))
        return this.PrimaryEnemyPlayerInLineOfSight;
      return new NpcPlayerInfo();
    }

    public override bool HasPrimaryEnemyPlayerTarget()
    {
      return Object.op_Inequality((Object) this.GetPrimaryEnemyPlayerTarget().Player, (Object) null);
    }

    public override Vector3 GetDirectionToPrimaryEnemyPlayerTargetBody()
    {
      NpcPlayerInfo enemyPlayerTarget = this.GetPrimaryEnemyPlayerTarget();
      if (!Object.op_Inequality((Object) enemyPlayerTarget.Player, (Object) null))
        return ((Component) this.Body).get_transform().get_forward();
      Vector3 vector3_1 = Vector3.get_zero();
      if (enemyPlayerTarget.Player.IsDucked())
        vector3_1 = PlayerEyes.DuckOffset;
      if (enemyPlayerTarget.Player.IsSleeping())
        vector3_1 = Vector3.get_down();
      Vector3 target = Vector3.op_Addition(enemyPlayerTarget.Player.CenterPoint(), vector3_1);
      Vector3 origin = this.Body.CenterPoint();
      Vector3 vector3_2 = Vector3.op_Subtraction(target, origin);
      return this.Domain.ModifyFirearmAim(((Vector3) ref vector3_2).get_normalized(), target, origin, 1f);
    }

    public override Vector3 GetDirectionToAnimal()
    {
      AnimalInfo primaryKnownAnimal = this.Memory.PrimaryKnownAnimal;
      if (!Object.op_Inequality((Object) primaryKnownAnimal.Animal, (Object) null))
        return ((Component) this.Body).get_transform().get_forward();
      Vector3 zero = Vector3.get_zero();
      Vector3 target = Vector3.op_Addition(primaryKnownAnimal.Animal.CenterPoint(), zero);
      Vector3 origin = this.Body.CenterPoint();
      Vector3 vector3 = Vector3.op_Subtraction(target, origin);
      return this.Domain.ModifyFirearmAim(((Vector3) ref vector3).get_normalized(), target, origin, 1f);
    }

    public override Vector3 GetDirectionToPrimaryEnemyPlayerTargetHead()
    {
      NpcPlayerInfo enemyPlayerTarget = this.GetPrimaryEnemyPlayerTarget();
      if (!Object.op_Inequality((Object) enemyPlayerTarget.Player, (Object) null))
        return ((Component) this.Body).get_transform().get_forward();
      Vector3 position = enemyPlayerTarget.Player.eyes.position;
      Vector3 origin = this.Body.CenterPoint();
      Vector3 vector3 = Vector3.op_Subtraction(position, origin);
      return this.Domain.ModifyFirearmAim(((Vector3) ref vector3).get_normalized(), position, origin, 1f);
    }

    public override Vector3 GetDirectionToMemoryOfPrimaryEnemyPlayerTarget()
    {
      BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = this.Memory.PrimaryKnownEnemyPlayer;
      if (!Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null))
        return ((Component) this.Body).get_transform().get_forward();
      Vector3 vector3_1 = this.Body.CenterPoint();
      float num = (float) (vector3_1.y - ((Component) this.Body).get_transform().get_position().y);
      Vector3 vector3_2 = Vector3.op_Subtraction(Vector3.op_Addition(knownEnemyPlayer.LastKnownPosition, Vector3.op_Multiply(((Component) knownEnemyPlayer.PlayerInfo.Player).get_transform().get_up(), num)), vector3_1);
      if ((double) ((Vector3) ref vector3_2).get_sqrMagnitude() < 2.0)
        return knownEnemyPlayer.LastKnownHeading;
      return ((Vector3) ref vector3_2).get_normalized();
    }

    public override Vector3 GetDirectionLookAround()
    {
      BaseNpcMemory.EnemyPlayerInfo knownEnemyPlayer = this.Memory.PrimaryKnownEnemyPlayer;
      if (!Object.op_Inequality((Object) knownEnemyPlayer.PlayerInfo.Player, (Object) null))
        return ((Component) this.Body).get_transform().get_forward();
      Vector3 vector3 = Vector3.op_Subtraction(knownEnemyPlayer.PlayerInfo.Player.CenterPoint(), this.Body.CenterPoint());
      return ((Vector3) ref vector3).get_normalized();
    }

    public override Vector3 GetDirectionLastAttackedDir()
    {
      if (Object.op_Inequality((Object) this.Body.lastAttacker, (Object) null))
        return this.Body.LastAttackedDir;
      return ((Component) this.Body).get_transform().get_forward();
    }

    public override Vector3 GetDirectionAudibleTarget()
    {
      NpcPlayerInfo npcPlayerInfo1 = new NpcPlayerInfo();
      foreach (NpcPlayerInfo npcPlayerInfo2 in this.EnemyPlayersAudible)
      {
        if ((double) npcPlayerInfo2.AudibleScore > (double) npcPlayerInfo1.AudibleScore)
          npcPlayerInfo1 = npcPlayerInfo2;
      }
      if (!Object.op_Inequality((Object) npcPlayerInfo1.Player, (Object) null))
        return ((Component) this.Body).get_transform().get_forward();
      Vector3 target = npcPlayerInfo1.Player.CenterPoint();
      Vector3 origin = this.Body.CenterPoint();
      Vector3 vector3 = Vector3.op_Subtraction(target, origin);
      return this.Domain.ModifyFirearmAim(((Vector3) ref vector3).get_normalized(), target, origin, 1f);
    }

    public override List<AnimalInfo> AnimalsInRange { get; } = new List<AnimalInfo>();

    public override Vector3 BodyPosition
    {
      get
      {
        return ((Component) this.Body).get_transform().get_position();
      }
    }

    public override BaseNpcMemory BaseMemory
    {
      get
      {
        return (BaseNpcMemory) this.Memory;
      }
    }

    public CoverPoint ClosestCover
    {
      get
      {
        if (this.CoverPoints.Count <= 0)
          return (CoverPoint) null;
        return this.CoverPoints[0];
      }
    }

    public void ReserveCoverPoint(CoverPoint coverPoint)
    {
      if (coverPoint != null && !coverPoint.IsValidFor((BaseEntity) this.Body))
        return;
      if (this.ReservedCoverPoint != null && this.ReservedCoverPoint != coverPoint)
        this.ReservedCoverPoint.ReservedFor = (BaseEntity) null;
      this.ReservedCoverPoint = coverPoint;
      if (this.ReservedCoverPoint == null)
        return;
      this.ReservedCoverPoint.ReservedFor = (BaseEntity) this.Body;
      this.ReservedCoverTime = Time.get_time();
    }

    public override void StartDomainDecomposition()
    {
    }

    public override void ResetState()
    {
      base.ResetState();
      this.Memory?.ResetState();
      this.IsWorldStateDirty = false;
      this.PlanState = (PlanStateType) 0;
      this.PlanResult = (PlanResultType) 0;
      this.HtnPlan?.Clear();
      this.AppliedEffects?.Clear();
      this.AppliedExpectedEffects?.Clear();
      this.DecompositionScore = int.MaxValue;
      this.CurrentTask = (PrimitiveTaskSelector) null;
      this.HasVisitedLastKnownEnemyPlayerLocation = false;
      this.OrientationType = NpcOrientation.Heading;
      this.PlayersInRange?.Clear();
      this.EnemyPlayersInRange?.Clear();
      this.EnemyPlayersAudible?.Clear();
      this.EnemyPlayersInLineOfSight?.Clear();
      this.PrimaryEnemyPlayerAudible = new NpcPlayerInfo();
      this.PrimaryEnemyPlayerInLineOfSight = new NpcPlayerInfo();
      this.Location = (AiLocationManager) null;
      this.CoverPoints?.Clear();
      if (this._worldState == null)
        return;
      for (int index = 0; index < this._worldState.Length; ++index)
      {
        this._worldState[index] = (byte) 0;
        this._previousWorldState[index] = (byte) 0;
      }
    }

    public ScientistJunkpileContext(HTNPlayer body, ScientistJunkpileDomain domain)
    {
      int length = System.Enum.GetValues(typeof (Facts)).Length;
      if (this._worldState == null || this._worldState.Length != length)
      {
        this._worldState = new byte[length];
        this._previousWorldState = new byte[length];
        if (ScientistJunkpileContext._worldStateChanges == null)
        {
          ScientistJunkpileContext._worldStateChanges = new Stack<WorldStateInfo>[length];
          for (int index = 0; index < length; ++index)
            ScientistJunkpileContext._worldStateChanges[index] = new Stack<WorldStateInfo>(5);
        }
      }
      this._decompositionScore = int.MaxValue;
      this.Body = body;
      this.Domain = domain;
      this.PlanState = (PlanStateType) 0;
      if (this.Memory == null || this.Memory.ScientistJunkpileContext != this)
        this.Memory = new ScientistJunkpileMemory(this);
      if (!Object.op_Equality((Object) this.Location, (Object) null))
        return;
      float num = float.MaxValue;
      foreach (AiLocationManager manager in AiLocationManager.Managers)
      {
        Vector3 vector3 = Vector3.op_Subtraction(((Component) manager).get_transform().get_position(), ((Component) this.Body).get_transform().get_position());
        float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
        if ((double) sqrMagnitude < (double) num)
        {
          num = sqrMagnitude;
          this.Location = manager;
        }
      }
      if (!Object.op_Inequality((Object) this.Location, (Object) null))
        return;
      this.Junkpile = (JunkPile) ((Component) this.Location).GetComponent<JunkPile>();
    }

    public void Dispose()
    {
    }

    public bool IsBodyAlive()
    {
      if (Object.op_Inequality((Object) this.Body, (Object) null) && Object.op_Inequality((Object) ((Component) this.Body).get_transform(), (Object) null) && (!this.Body.IsDestroyed && !this.Body.IsDead()))
        return !this.Body.IsWounded();
      return false;
    }

    public void IncrementFact(
      Facts fact,
      int value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, (int) this.GetFact(fact) + value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void IncrementFact(
      Facts fact,
      byte value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, (int) this.GetFact(fact) + (int) value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void SetFact(
      Facts fact,
      EnemyRange value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, (byte) value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void SetFact(
      Facts fact,
      CoverState value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, (byte) value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void SetFact(
      Facts fact,
      CoverTactic value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, (byte) value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void SetFact(
      Facts fact,
      AmmoState value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, (byte) value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void SetFact(
      Facts fact,
      HealthState value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, (byte) value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void SetFact(
      Facts fact,
      ItemType value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, (byte) value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void SetFact(
      Facts fact,
      FireTactic value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, (byte) value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void SetFact(
      Facts fact,
      FirearmOrders value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, (byte) value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void SetFact(
      Facts fact,
      bool value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, value ? (byte) 1 : (byte) 0, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void SetFact(
      Facts fact,
      int value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact(fact, (byte) value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public void SetFact(
      Facts fact,
      byte value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      int index = (int) fact;
      if (checkValueDiff && (int) this._worldState[index] == (int) value)
        return;
      if (setAsDirty)
        this.IsWorldStateDirty = true;
      this._previousWorldState[index] = this._worldState[index];
      this._worldState[index] = value;
      if (!invokeChangedEvent)
        return;
      ScientistJunkpileContext.WorldStateChangedEvent stateChangedEvent = this.OnWorldStateChangedEvent;
      if (stateChangedEvent == null)
        return;
      stateChangedEvent(this, fact, this._previousWorldState[index], value);
    }

    public byte GetFact(Facts fact)
    {
      return this._worldState[(int) fact];
    }

    public byte GetPreviousFact(Facts fact)
    {
      return this._previousWorldState[(int) fact];
    }

    public override void SetFact(
      byte fact,
      byte value,
      bool invokeChangedEvent = true,
      bool setAsDirty = true,
      bool checkValueDiff = true)
    {
      this.SetFact((Facts) fact, value, invokeChangedEvent, setAsDirty, checkValueDiff);
    }

    public override byte GetFact(byte fact)
    {
      return this.GetFact((Facts) fact);
    }

    public bool IsFact(Facts fact)
    {
      return this.GetFact(fact) > (byte) 0;
    }

    public void PushFactChangeDuringPlanning(Facts fact, CoverState value, bool temporary)
    {
      this.PushFactChangeDuringPlanning((byte) fact, (byte) value, temporary);
    }

    public void PushFactChangeDuringPlanning(Facts fact, ItemType value, bool temporary)
    {
      this.PushFactChangeDuringPlanning((byte) fact, (byte) value, temporary);
    }

    public void PushFactChangeDuringPlanning(Facts fact, FirearmOrders value, bool temporary)
    {
      this.PushFactChangeDuringPlanning((byte) fact, (byte) value, temporary);
    }

    public void PushFactChangeDuringPlanning(Facts fact, HealthState value, bool temporary)
    {
      this.PushFactChangeDuringPlanning((byte) fact, (byte) value, temporary);
    }

    public void PushFactChangeDuringPlanning(Facts fact, bool value, bool temporary)
    {
      this.PushFactChangeDuringPlanning((byte) fact, value ? (byte) 1 : (byte) 0, temporary);
    }

    public void PushFactChangeDuringPlanning(Facts fact, int value, bool temporary)
    {
      this.PushFactChangeDuringPlanning((byte) fact, (byte) value, temporary);
    }

    public void PushFactChangeDuringPlanning(Facts fact, byte value, bool temporary)
    {
      this.PushFactChangeDuringPlanning((byte) fact, value, temporary);
    }

    public void PushFactChangeDuringPlanning(byte fact, byte value, bool temporary)
    {
      int index = (int) fact;
      Stack<WorldStateInfo> worldStateChange = ScientistJunkpileContext._worldStateChanges[index];
      WorldStateInfo worldStateInfo1 = (WorldStateInfo) null;
      worldStateInfo1.Value = (__Null) (int) value;
      worldStateInfo1.Temporary = (__Null) (temporary ? 1 : 0);
      WorldStateInfo worldStateInfo2 = worldStateInfo1;
      worldStateChange.Push(worldStateInfo2);
    }

    public void PopFactChangeDuringPlanning(Facts fact)
    {
      this.PopFactChangeDuringPlanning((byte) fact);
    }

    public void PopFactChangeDuringPlanning(byte fact)
    {
      int index = (int) fact;
      if (ScientistJunkpileContext._worldStateChanges[index].Count <= 0)
        return;
      ScientistJunkpileContext._worldStateChanges[index].Pop();
    }

    public byte PeekFactChangeDuringPlanning(Facts fact)
    {
      return this.PeekFactChangeDuringPlanning((byte) fact);
    }

    public byte PeekFactChangeDuringPlanning(byte fact)
    {
      int index = (int) fact;
      if (ScientistJunkpileContext._worldStateChanges[index].Count > 0)
        return (byte) ScientistJunkpileContext._worldStateChanges[index].Peek().Value;
      return 0;
    }

    public byte GetWorldState(Facts fact)
    {
      return this.GetWorldState((byte) fact);
    }

    public delegate void WorldStateChangedEvent(
      ScientistJunkpileContext context,
      Facts fact,
      byte oldValue,
      byte newValue);
  }
}
