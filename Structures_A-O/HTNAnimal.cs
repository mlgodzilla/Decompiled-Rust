// Decompiled with JetBrains decompiler
// Type: HTNAnimal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Rust.Ai;
using Rust.Ai.HTN;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HTNAnimal : BaseCombatEntity, IHTNAgent
{
  [Header("Client Animation")]
  public Vector3 HipFudge = new Vector3(-90f, 0.0f, 90f);
  public bool UpdateWalkSpeed = true;
  public bool UpdateFacingDirection = true;
  public bool UpdateGroundNormal = true;
  public bool LaggyAss = true;
  public float MaxLaggyAssRotation = 70f;
  public float MaxWalkAnimSpeed = 25f;
  public Transform HipBone;
  public Transform LookBone;
  public Transform alignmentRoot;
  public bool LookAtTarget;
  [Header("Hierarchical Task Network")]
  public HTNDomain _aiDomain;
  [Header("Ai Definition")]
  public BaseNpcDefinition _aiDefinition;
  private bool isDormant;
  private float lastInvokedTickTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("HTNAnimal.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public BaseNpcDefinition AiDefinition
  {
    get
    {
      return this._aiDefinition;
    }
  }

  public HTNDomain AiDomain
  {
    get
    {
      return this._aiDomain;
    }
  }

  public Vector3 estimatedVelocity { get; set; }

  public bool IsDormant
  {
    get
    {
      return this.isDormant;
    }
    set
    {
      if (this.isDormant == value)
        return;
      this.isDormant = value;
      if (this.isDormant)
        this.Pause();
      else
        this.Resume();
    }
  }

  public void Resume()
  {
  }

  public void Pause()
  {
  }

  public BaseEntity MainTarget
  {
    get
    {
      if (this.AiDomain.NpcContext.OrientationType != NpcOrientation.LookAtAnimal)
        return (BaseEntity) this.AiDomain.NpcContext.PrimaryEnemyPlayerInLineOfSight.Player;
      return (BaseEntity) this.AiDomain.NpcContext.BaseMemory.PrimaryKnownAnimal.Animal;
    }
  }

  public void Tick()
  {
    this.InvokedTick();
  }

  private void InvokedTick()
  {
    float time = Time.get_time();
    float delta = time - this.lastInvokedTickTime;
    this.lastInvokedTickTime = Time.get_time();
    if (this.IsDormant)
      return;
    if (Object.op_Inequality((Object) this.AiDomain, (Object) null))
    {
      this.AiDomain.TickDestinationTracker();
      if (this.AiDomain.PlannerContext.get_IsWorldStateDirty() || this.AiDomain.PlannerContext.get_PlanState() == null)
        this.AiDomain.Think();
      this.AiDomain.Tick(Time.get_time());
    }
    this.TickMovement(delta);
    this.TickOrientation(delta, time);
  }

  private void TickMovement(float delta)
  {
    if (!AI.move || Object.op_Equality((Object) this.AiDomain, (Object) null) || (Object.op_Equality((Object) this.AiDomain.NavAgent, (Object) null) || !this.AiDomain.NavAgent.get_isOnNavMesh()))
      return;
    Vector3 moveToPosition = ((Component) this).get_transform().get_position();
    if (this.AiDomain.NavAgent.get_hasPath())
      moveToPosition = this.AiDomain.NavAgent.get_nextPosition();
    if (!this._ValidateNextPosition(ref moveToPosition))
      return;
    ((Component) this).get_transform().set_localPosition(((Component) this).get_transform().InverseTransformPoint(moveToPosition));
    ((Component) this).get_transform().set_hasChanged(true);
  }

  private bool _ValidateNextPosition(ref Vector3 moveToPosition)
  {
    if (ValidBounds.Test(moveToPosition) || !Object.op_Inequality((Object) ((Component) this).get_transform(), (Object) null) || this.IsDestroyed)
      return true;
    Debug.Log((object) string.Format("Invalid NavAgent Position: {0} {1} (destroying)", (object) this, (object) moveToPosition.ToString()));
    this.Kill(BaseNetworkable.DestroyMode.None);
    return false;
  }

  public void ForceOrientationTick()
  {
    this.TickOrientation(Time.get_deltaTime(), Time.get_time());
  }

  private void TickOrientation(float delta, float time)
  {
    if (Object.op_Equality((Object) this.AiDomain, (Object) null) || this.AiDomain.NpcContext == null)
      return;
    ((Component) this).get_transform().get_forward();
    Vector3 vector3;
    switch (this.AiDomain.NpcContext.OrientationType)
    {
      case NpcOrientation.Heading:
        vector3 = this.AiDomain.GetHeadingDirection();
        break;
      case NpcOrientation.PrimaryTargetBody:
        vector3 = this.AiDomain.NpcContext.GetDirectionToPrimaryEnemyPlayerTargetBody();
        break;
      case NpcOrientation.PrimaryTargetHead:
        vector3 = this.AiDomain.NpcContext.GetDirectionToPrimaryEnemyPlayerTargetHead();
        break;
      case NpcOrientation.LastKnownPrimaryTargetLocation:
        vector3 = this.AiDomain.NpcContext.GetDirectionToMemoryOfPrimaryEnemyPlayerTarget();
        break;
      case NpcOrientation.LookAround:
        vector3 = this.AiDomain.NpcContext.GetDirectionLookAround();
        break;
      case NpcOrientation.LastAttackedDirection:
        vector3 = this.AiDomain.NpcContext.GetDirectionLastAttackedDir();
        break;
      case NpcOrientation.AudibleTargetDirection:
        vector3 = this.AiDomain.NpcContext.GetDirectionAudibleTarget();
        break;
      case NpcOrientation.LookAtAnimal:
        vector3 = this.AiDomain.NpcContext.GetDirectionToAnimal();
        break;
      default:
        return;
    }
    if (Mathf.Approximately(((Vector3) ref vector3).get_sqrMagnitude(), 0.0f))
      return;
    this.ServerRotation = Quaternion.LookRotation(vector3, ((Component) this).get_transform().get_up());
  }

  public override bool IsNpc
  {
    get
    {
      return true;
    }
  }

  public override float StartHealth()
  {
    return this.AiDefinition.Vitals.HP;
  }

  public override float StartMaxHealth()
  {
    return this.AiDefinition.Vitals.HP;
  }

  public override float MaxHealth()
  {
    return this.AiDefinition.Vitals.HP;
  }

  public override float MaxVelocity()
  {
    return this.AiDefinition.Movement.RunSpeed;
  }

  public BaseEntity Body
  {
    get
    {
      return (BaseEntity) this;
    }
  }

  public Vector3 BodyPosition
  {
    get
    {
      return ((Component) this).get_transform().get_position();
    }
  }

  public Vector3 EyePosition
  {
    get
    {
      return this.CenterPoint();
    }
  }

  public Quaternion EyeRotation
  {
    get
    {
      return ((Component) this).get_transform().get_rotation();
    }
  }

  protected override float PositionTickRate
  {
    get
    {
      return 0.1f;
    }
  }

  public BaseNpc.AiStatistics.FamilyEnum Family
  {
    get
    {
      return this.AiDefinition.Info.ToFamily(this.AiDefinition.Info.Family);
    }
  }

  public override void ServerInit()
  {
    if (this.isClient)
      return;
    base.ServerInit();
    this.UpdateNetworkGroup();
    if (Object.op_Equality((Object) this.AiDomain, (Object) null))
    {
      Debug.LogError((object) (((Object) this).get_name() + " requires an AI domain to be set."));
      this.DieInstantly();
    }
    else
    {
      this.AiDomain.Initialize((BaseEntity) this);
      if (AiManager.ai_htn_use_agency_tick)
        return;
      this.InvokeRepeating(new Action(this.InvokedTick), 0.0f, 0.1f);
    }
  }

  public override void ResetState()
  {
    base.ResetState();
    if (!Object.op_Inequality((Object) this.AiDomain, (Object) null))
      return;
    this.AiDomain.ResetState();
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    this.AiDomain.Dispose();
  }

  public override void Hurt(HitInfo info)
  {
    base.Hurt(info);
    if (!Object.op_Inequality((Object) this.AiDomain, (Object) null) || !this.IsAlive())
      return;
    this.AiDomain.OnHurt(info);
  }

  public override void OnKilled(HitInfo info)
  {
    this.AiDefinition?.OnCreateCorpse(this);
    this.Invoke(new Action(((BaseNetworkable) this).KillMessage), 0.5f);
  }

  public override void OnSensation(Sensation sensation)
  {
    base.OnSensation(sensation);
    if (!Object.op_Inequality((Object) this.AiDomain, (Object) null) || !this.IsAlive())
      return;
    this.AiDomain.OnSensation(sensation);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
  }

  [SpecialName]
  Transform IHTNAgent.get_transform()
  {
    return ((Component) this).get_transform();
  }
}
