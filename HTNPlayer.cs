// Decompiled with JetBrains decompiler
// Type: HTNPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Rust.Ai;
using Rust.Ai.HTN;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HTNPlayer : BasePlayer, IHTNAgent
{
  [Header("Hierarchical Task Network")]
  public HTNDomain _aiDomain;
  [Header("Ai Definition")]
  public BaseNpcDefinition _aiDefinition;
  private bool isDormant;
  private float lastInvokedTickTime;
  private int serverMaxProjectileID;

  public BaseNpcDefinition AiDefinition
  {
    get
    {
      return this._aiDefinition;
    }
  }

  public bool OnlyRotateAroundYAxis { get; set; }

  public HTNDomain AiDomain
  {
    get
    {
      return this._aiDomain;
    }
  }

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
    if (Object.op_Inequality((Object) this.AiDomain, (Object) null))
      this.AiDomain.Resume();
    if (!((BaseScriptableObject) this.AiDefinition != (BaseScriptableObject) null))
      return;
    this.AiDefinition.StartVoices(this);
  }

  public void Pause()
  {
    if (Object.op_Inequality((Object) this.AiDomain, (Object) null))
      this.AiDomain.Pause();
    if (!((BaseScriptableObject) this.AiDefinition != (BaseScriptableObject) null))
      return;
    this.AiDefinition.StopVoices(this);
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
    if (Object.op_Equality((Object) ((Component) this).get_transform(), (Object) null) || this.IsDestroyed || this.IsDead())
      return;
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
    if (!AI.move || Object.op_Equality((Object) this.AiDomain, (Object) null))
      return;
    Vector3 nextPosition = this.AiDomain.GetNextPosition(delta);
    if (!this._ValidateNextPosition(ref nextPosition))
      return;
    BaseEntity parentEntity = this.GetParentEntity();
    if (Object.op_Implicit((Object) parentEntity))
      ((Component) this).get_transform().set_localPosition(((Component) parentEntity).get_transform().InverseTransformPoint(nextPosition));
    else
      ((Component) this).get_transform().set_localPosition(nextPosition);
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
    Vector3 vector3_1;
    switch (this.AiDomain.NpcContext.OrientationType)
    {
      case NpcOrientation.Heading:
        vector3_1 = this.AiDomain.GetHeadingDirection();
        break;
      case NpcOrientation.PrimaryTargetBody:
        vector3_1 = this.AiDomain.NpcContext.GetDirectionToPrimaryEnemyPlayerTargetBody();
        break;
      case NpcOrientation.PrimaryTargetHead:
        vector3_1 = this.AiDomain.NpcContext.GetDirectionToPrimaryEnemyPlayerTargetHead();
        break;
      case NpcOrientation.LastKnownPrimaryTargetLocation:
        vector3_1 = this.AiDomain.NpcContext.GetDirectionToMemoryOfPrimaryEnemyPlayerTarget();
        break;
      case NpcOrientation.LookAround:
        vector3_1 = this.AiDomain.NpcContext.GetDirectionLookAround();
        break;
      case NpcOrientation.LastAttackedDirection:
        vector3_1 = this.AiDomain.NpcContext.GetDirectionLastAttackedDir();
        break;
      case NpcOrientation.AudibleTargetDirection:
        vector3_1 = this.AiDomain.NpcContext.GetDirectionAudibleTarget();
        break;
      case NpcOrientation.LookAtAnimal:
        vector3_1 = this.AiDomain.NpcContext.GetDirectionToAnimal();
        break;
      case NpcOrientation.Home:
        vector3_1 = this.AiDomain.GetHomeDirection();
        break;
      default:
        return;
    }
    if (Mathf.Approximately(((Vector3) ref vector3_1).get_sqrMagnitude(), 0.0f))
      return;
    BaseEntity parentEntity = this.GetParentEntity();
    if (Object.op_Implicit((Object) parentEntity))
    {
      Vector3 vector3_2 = ((Component) parentEntity).get_transform().InverseTransformDirection(vector3_1);
      Vector3 vector3_3;
      ((Vector3) ref vector3_3).\u002Ector((float) vector3_1.x, (float) vector3_2.y, (float) vector3_1.z);
      this.eyes.rotation = Quaternion.LookRotation(vector3_3, ((Component) parentEntity).get_transform().get_up());
      if (this.OnlyRotateAroundYAxis)
      {
        PlayerEyes eyes = this.eyes;
        Quaternion rotation = this.eyes.rotation;
        Quaternion quaternion = Quaternion.Euler(new Vector3(0.0f, (float) ((Quaternion) ref rotation).get_eulerAngles().y, 0.0f));
        eyes.rotation = quaternion;
      }
      this.ServerRotation = this.eyes.bodyRotation;
    }
    else
    {
      this.eyes.bodyRotation = Quaternion.LookRotation(vector3_1, ((Component) this).get_transform().get_up());
      if (this.OnlyRotateAroundYAxis)
      {
        PlayerEyes eyes = this.eyes;
        Quaternion bodyRotation = this.eyes.bodyRotation;
        Quaternion quaternion = Quaternion.Euler(new Vector3(0.0f, (float) ((Quaternion) ref bodyRotation).get_eulerAngles().y, 0.0f));
        eyes.bodyRotation = quaternion;
      }
      this.ServerRotation = this.eyes.bodyRotation;
    }
  }

  public int NewServerProjectileID()
  {
    return ++this.serverMaxProjectileID;
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
    BaseNpcDefinition aiDefinition = this.AiDefinition;
    if (aiDefinition == null)
      return 0.0f;
    return aiDefinition.Vitals.HP;
  }

  public override float StartMaxHealth()
  {
    BaseNpcDefinition aiDefinition = this.AiDefinition;
    if (aiDefinition == null)
      return 0.0f;
    return aiDefinition.Vitals.HP;
  }

  public override float MaxHealth()
  {
    BaseNpcDefinition aiDefinition = this.AiDefinition;
    if (aiDefinition == null)
      return 0.0f;
    return aiDefinition.Vitals.HP;
  }

  public override float MaxVelocity()
  {
    BaseNpcDefinition aiDefinition = this.AiDefinition;
    if (aiDefinition == null)
      return 0.0f;
    return aiDefinition.Movement.RunSpeed;
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
      BaseEntity parentEntity = this.GetParentEntity();
      if (Object.op_Inequality((Object) parentEntity, (Object) null))
        return Vector3.op_Addition(this.BodyPosition, Vector3.op_Multiply(((Component) parentEntity).get_transform().get_up(), (float) PlayerEyes.EyeOffset.y));
      return this.eyes.position;
    }
  }

  public Quaternion EyeRotation
  {
    get
    {
      return this.eyes.rotation;
    }
  }

  public override Quaternion GetNetworkRotation()
  {
    if (this.isServer)
      return this.eyes.bodyRotation;
    return Quaternion.get_identity();
  }

  protected override float PositionTickRate
  {
    get
    {
      return 0.05f;
    }
  }

  public override BaseNpc.AiStatistics.FamilyEnum Family
  {
    get
    {
      return this.AiDefinition.Info.ToFamily(this.AiDefinition.Info.Family);
    }
  }

  public override string Categorize()
  {
    return "npc";
  }

  public override bool ShouldDropActiveItem()
  {
    return false;
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
      if (!AiManager.ai_htn_use_agency_tick)
        this.InvokeRepeating(new Action(this.InvokedTick), 0.0f, 0.1f);
      this.AiDefinition?.Loadout(this);
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
    if (Object.op_Inequality((Object) info.InitiatorPlayer, (Object) null) && info.InitiatorPlayer.Family == this.Family)
      return;
    if (Object.op_Inequality((Object) this.AiDomain, (Object) null) && this.IsAlive())
      this.AiDomain.OnPreHurt(info);
    base.Hurt(info);
    if (!Object.op_Inequality((Object) this.AiDomain, (Object) null) || !this.IsAlive())
      return;
    this.AiDomain.OnHurt(info);
  }

  public override bool EligibleForWounding(HitInfo info)
  {
    return false;
  }

  public override void OnKilled(HitInfo info)
  {
    base.OnKilled(info);
    if (!((BaseScriptableObject) this.AiDefinition != (BaseScriptableObject) null))
      return;
    this.AiDefinition.StopVoices(this);
  }

  public override BaseCorpse CreateCorpse()
  {
    BaseCorpse corpse = this.AiDefinition.OnCreateCorpse(this);
    if (Object.op_Implicit((Object) corpse))
      return corpse;
    return base.CreateCorpse();
  }

  public override void OnSensation(Sensation sensation)
  {
    base.OnSensation(sensation);
    if (!Object.op_Inequality((Object) this.AiDomain, (Object) null) || !this.IsAlive())
      return;
    this.AiDomain.OnSensation(sensation);
  }

  public override Vector3 GetLocalVelocityServer()
  {
    return Vector3.op_Subtraction(this.estimatedVelocity, this.GetParentVelocity());
  }

  [SpecialName]
  Transform IHTNAgent.get_transform()
  {
    return ((Component) this).get_transform();
  }
}
