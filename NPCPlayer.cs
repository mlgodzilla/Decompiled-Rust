// Decompiled with JetBrains decompiler
// Type: NPCPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.AI;

public class NPCPlayer : BasePlayer
{
  public LayerMask movementMask = LayerMask.op_Implicit(429990145);
  public float damageScale = 1f;
  public Vector3 finalDestination;
  [NonSerialized]
  private float randomOffset;
  [NonSerialized]
  private Vector3 spawnPos;
  public PlayerInventoryProperties[] loadouts;
  public NavMeshAgent NavAgent;
  private bool _isDormant;
  protected float lastGunShotTime;
  private float triggerEndTime;
  protected float nextTriggerTime;
  private float lastThinkTime;
  private Vector3 lastPos;
  protected bool _traversingNavMeshLink;
  protected OffMeshLinkData _currentNavMeshLink;
  protected string _currentNavMeshLinkName;
  protected Quaternion _currentNavMeshLinkOrientation;
  protected Vector3 _currentNavMeshLinkEndPos;

  public override bool IsNpc
  {
    get
    {
      return true;
    }
  }

  public virtual bool IsDormant
  {
    get
    {
      return this._isDormant;
    }
    set
    {
      this._isDormant = value;
      int num = this._isDormant ? 1 : 0;
    }
  }

  protected override float PositionTickRate
  {
    get
    {
      return 0.1f;
    }
  }

  public virtual bool IsLoadBalanced()
  {
    return false;
  }

  public override void ServerInit()
  {
    if (this.isClient)
      return;
    this.spawnPos = this.GetPosition();
    this.randomOffset = Random.Range(0.0f, 1f);
    base.ServerInit();
    this.UpdateNetworkGroup();
    if (this.loadouts != null && this.loadouts.Length != 0)
      this.loadouts[Random.Range(0, this.loadouts.Length)].GiveToPlayer((BasePlayer) this);
    else
      Debug.LogWarningFormat("Loadout for NPC {0} was empty.", new object[1]
      {
        (object) ((Object) this).get_name()
      });
    if (!this.IsLoadBalanced())
    {
      this.InvokeRepeating(new Action(this.ServerThink_Internal), 0.0f, 0.1f);
      this.lastThinkTime = Time.get_time();
    }
    this.Invoke(new Action(this.EquipTest), 0.25f);
    this.finalDestination = ((Component) this).get_transform().get_position();
    this.AgencyUpdateRequired = false;
    this.IsOnOffmeshLinkAndReachedNewCoord = false;
    if (Object.op_Equality((Object) this.NavAgent, (Object) null))
      this.NavAgent = (NavMeshAgent) ((Component) this).GetComponent<NavMeshAgent>();
    if (!Object.op_Implicit((Object) this.NavAgent))
      return;
    this.NavAgent.set_updateRotation(false);
    this.NavAgent.set_updatePosition(false);
  }

  public void RandomMove()
  {
    Vector2 vector2 = Vector2.op_Multiply(Random.get_insideUnitCircle(), 8f);
    this.SetDestination(Vector3.op_Addition(this.spawnPos, new Vector3((float) vector2.x, 0.0f, (float) vector2.y)));
  }

  public virtual void SetDestination(Vector3 newDestination)
  {
    this.finalDestination = newDestination;
  }

  public AttackEntity GetAttackEntity()
  {
    return this.GetHeldEntity() as AttackEntity;
  }

  public BaseProjectile GetGun()
  {
    AttackEntity heldEntity = this.GetHeldEntity() as AttackEntity;
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return (BaseProjectile) null;
    BaseProjectile baseProjectile = heldEntity as BaseProjectile;
    if (Object.op_Implicit((Object) baseProjectile))
      return baseProjectile;
    return (BaseProjectile) null;
  }

  public virtual float AmmoFractionRemaining()
  {
    AttackEntity attackEntity = this.GetAttackEntity();
    if (Object.op_Implicit((Object) attackEntity))
      return attackEntity.AmmoFraction();
    return 0.0f;
  }

  public virtual bool IsReloading()
  {
    AttackEntity attackEntity = this.GetAttackEntity();
    if (!Object.op_Implicit((Object) attackEntity))
      return false;
    return attackEntity.ServerIsReloading();
  }

  public virtual void AttemptReload()
  {
    AttackEntity attackEntity = this.GetAttackEntity();
    if (Object.op_Equality((Object) attackEntity, (Object) null) || !attackEntity.CanReload())
      return;
    attackEntity.ServerReload();
  }

  public virtual bool ShotTest()
  {
    AttackEntity heldEntity = this.GetHeldEntity() as AttackEntity;
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return false;
    BaseProjectile baseProjectile = heldEntity as BaseProjectile;
    if (Object.op_Implicit((Object) baseProjectile))
    {
      if (baseProjectile.primaryMagazine.contents <= 0)
      {
        baseProjectile.ServerReload();
        NPCPlayerApex npcPlayerApex = this as NPCPlayerApex;
        if (Object.op_Implicit((Object) npcPlayerApex) && npcPlayerApex.OnReload != null)
          npcPlayerApex.OnReload();
        return false;
      }
      if ((double) baseProjectile.NextAttackTime > (double) Time.get_time())
        return false;
    }
    if (!Mathf.Approximately(heldEntity.attackLengthMin, -1f))
    {
      if (this.IsInvoking(new Action(this.TriggerDown)) || (double) Time.get_time() < (double) this.nextTriggerTime)
        return true;
      this.InvokeRepeating(new Action(this.TriggerDown), 0.0f, 0.01f);
      this.triggerEndTime = Time.get_time() + Random.Range(heldEntity.attackLengthMin, heldEntity.attackLengthMax);
      this.TriggerDown();
      return true;
    }
    heldEntity.ServerUse(this.damageScale);
    this.lastGunShotTime = Time.get_time();
    return true;
  }

  public virtual float GetAimConeScale()
  {
    return 1f;
  }

  public void CancelBurst(float delay = 0.2f)
  {
    if ((double) this.triggerEndTime <= (double) Time.get_time() + (double) delay)
      return;
    this.triggerEndTime = Time.get_time() + delay;
  }

  public bool MeleeAttack()
  {
    AttackEntity heldEntity = this.GetHeldEntity() as AttackEntity;
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return false;
    BaseMelee baseMelee = heldEntity as BaseMelee;
    if (Object.op_Equality((Object) baseMelee, (Object) null))
      return false;
    baseMelee.ServerUse(this.damageScale);
    return true;
  }

  public virtual void TriggerDown()
  {
    AttackEntity heldEntity = this.GetHeldEntity() as AttackEntity;
    if (Object.op_Inequality((Object) heldEntity, (Object) null))
      heldEntity.ServerUse(this.damageScale);
    this.lastGunShotTime = Time.get_time();
    if ((double) Time.get_time() <= (double) this.triggerEndTime)
      return;
    this.CancelInvoke(new Action(this.TriggerDown));
    this.nextTriggerTime = Time.get_time() + (Object.op_Inequality((Object) heldEntity, (Object) null) ? heldEntity.attackSpacing : 1f);
  }

  public virtual void EquipWeapon()
  {
    Item slot = this.inventory.containerBelt.GetSlot(0);
    if (slot == null)
      return;
    this.UpdateActiveItem(this.inventory.containerBelt.GetSlot(0).uid);
    BaseEntity heldEntity = slot.GetHeldEntity();
    if (!Object.op_Inequality((Object) heldEntity, (Object) null))
      return;
    AttackEntity component = (AttackEntity) ((Component) heldEntity).GetComponent<AttackEntity>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.TopUpAmmo();
  }

  public void EquipTest()
  {
    this.EquipWeapon();
  }

  internal void ServerThink_Internal()
  {
    this.ServerThink(Time.get_time() - this.lastThinkTime);
    this.lastThinkTime = Time.get_time();
  }

  public virtual void ServerThink(float delta)
  {
    this.TickAi(delta);
  }

  public virtual void Resume()
  {
  }

  public virtual bool IsNavRunning()
  {
    return false;
  }

  public virtual bool IsOnNavMeshLink
  {
    get
    {
      if (this.IsNavRunning())
        return this.NavAgent.get_isOnOffMeshLink();
      return false;
    }
  }

  public virtual bool HasPath
  {
    get
    {
      if (this.IsNavRunning())
        return this.NavAgent.get_hasPath();
      return false;
    }
  }

  public virtual void TickAi(float delta)
  {
    this.MovementUpdate(delta);
  }

  public virtual void MovementUpdate(float delta)
  {
    if (this.isClient || !this.IsAlive() || this.IsWounded() || !this.isMounted && !this.IsNavRunning())
      return;
    if (this.IsDormant || !this.syncPosition)
    {
      if (!this.IsNavRunning())
        return;
      this.NavAgent.set_destination(this.ServerPosition);
    }
    else
    {
      Vector3 moveToPosition = ((Component) this).get_transform().get_position();
      if (this.IsOnNavMeshLink)
        this.HandleNavMeshLinkTraversal(delta, ref moveToPosition);
      else if (this.HasPath)
        moveToPosition = this.NavAgent.get_nextPosition();
      if (!this.ValidateNextPosition(ref moveToPosition))
        return;
      this.UpdateSpeed(delta);
      this.UpdatePositionAndRotation(moveToPosition);
    }
  }

  private bool ValidateNextPosition(ref Vector3 moveToPosition)
  {
    if (ValidBounds.Test(moveToPosition) || !Object.op_Inequality((Object) ((Component) this).get_transform(), (Object) null) || this.IsDestroyed)
      return true;
    Debug.Log((object) ("Invalid NavAgent Position: " + (object) this + " " + moveToPosition.ToString() + " (destroying)"));
    this.Kill(BaseNetworkable.DestroyMode.None);
    return false;
  }

  private void UpdateSpeed(float delta)
  {
    this.NavAgent.set_speed(Mathf.Lerp(this.NavAgent.get_speed(), this.DesiredMoveSpeed(), delta * 8f));
  }

  protected virtual void UpdatePositionAndRotation(Vector3 moveToPosition)
  {
    this.ServerPosition = moveToPosition;
    this.SetAimDirection(this.GetAimDirection());
  }

  public Vector3 GetPosition()
  {
    return ((Component) this).get_transform().get_position();
  }

  public virtual float DesiredMoveSpeed()
  {
    return this.GetSpeed(Mathf.Sin(Time.get_time() + this.randomOffset), 0.0f);
  }

  public override bool EligibleForWounding(HitInfo info)
  {
    return false;
  }

  public virtual Vector3 GetAimDirection()
  {
    if ((double) Vector3Ex.Distance2D(this.finalDestination, this.GetPosition()) < 1.0)
      return this.eyes.BodyForward();
    Vector3 vector3 = Vector3.op_Subtraction(this.finalDestination, this.GetPosition());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return new Vector3((float) normalized.x, 0.0f, (float) normalized.z);
  }

  public virtual void SetAimDirection(Vector3 newAim)
  {
    if (Vector3.op_Equality(newAim, Vector3.get_zero()))
      return;
    AttackEntity attackEntity = this.GetAttackEntity();
    if (Object.op_Implicit((Object) attackEntity))
      newAim = attackEntity.ModifyAIAim(newAim, 1f);
    this.eyes.rotation = Quaternion.LookRotation(newAim, Vector3.get_up());
    Quaternion rotation = this.eyes.rotation;
    this.viewAngles = ((Quaternion) ref rotation).get_eulerAngles();
    this.ServerRotation = this.eyes.rotation;
  }

  public bool AgencyUpdateRequired { get; set; }

  public bool IsOnOffmeshLinkAndReachedNewCoord { get; set; }

  private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
  {
    if (!this._traversingNavMeshLink)
      this.HandleNavMeshLinkTraversalStart(delta);
    this.HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
    if (!this.IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
      return;
    this.CompleteNavMeshLink();
  }

  private bool HandleNavMeshLinkTraversalStart(float delta)
  {
    OffMeshLinkData currentOffMeshLinkData = this.NavAgent.get_currentOffMeshLinkData();
    if (!((OffMeshLinkData) ref currentOffMeshLinkData).get_valid() || !((OffMeshLinkData) ref currentOffMeshLinkData).get_activated())
      return false;
    Vector3 vector3 = Vector3.op_Subtraction(((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos(), ((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos());
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    normalized.y = (__Null) 0.0;
    Vector3 desiredVelocity = this.NavAgent.get_desiredVelocity();
    desiredVelocity.y = (__Null) 0.0;
    if ((double) Vector3.Dot(desiredVelocity, normalized) < 0.100000001490116)
    {
      this.CompleteNavMeshLink();
      return false;
    }
    this._currentNavMeshLink = currentOffMeshLinkData;
    this._currentNavMeshLinkName = ((OffMeshLinkData) ref currentOffMeshLinkData).get_linkType().ToString();
    vector3 = Vector3.op_Subtraction(this.ServerPosition, ((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos());
    double sqrMagnitude1 = (double) ((Vector3) ref vector3).get_sqrMagnitude();
    vector3 = Vector3.op_Subtraction(this.ServerPosition, ((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos());
    double sqrMagnitude2 = (double) ((Vector3) ref vector3).get_sqrMagnitude();
    if (sqrMagnitude1 > sqrMagnitude2)
    {
      this._currentNavMeshLinkEndPos = ((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos();
      this._currentNavMeshLinkOrientation = Quaternion.LookRotation(Vector3.op_Subtraction(Vector3.op_Addition(((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos(), Vector3.op_Multiply(Vector3.get_up(), (float) (((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos().y - ((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos().y))), ((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos()));
    }
    else
    {
      this._currentNavMeshLinkEndPos = ((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos();
      this._currentNavMeshLinkOrientation = Quaternion.LookRotation(Vector3.op_Subtraction(Vector3.op_Addition(((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos(), Vector3.op_Multiply(Vector3.get_up(), (float) (((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos().y - ((OffMeshLinkData) ref currentOffMeshLinkData).get_endPos().y))), ((OffMeshLinkData) ref currentOffMeshLinkData).get_startPos()));
    }
    this._traversingNavMeshLink = true;
    this.NavAgent.ActivateCurrentOffMeshLink(false);
    this.NavAgent.set_obstacleAvoidanceType((ObstacleAvoidanceType) 0);
    if (!(this._currentNavMeshLinkName == "OpenDoorLink") && !(this._currentNavMeshLinkName == "JumpRockLink"))
    {
      int num = this._currentNavMeshLinkName == "JumpFoundationLink" ? 1 : 0;
    }
    return true;
  }

  private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
  {
    if (this._currentNavMeshLinkName == "OpenDoorLink")
      moveToPosition = Vector3.MoveTowards(moveToPosition, this._currentNavMeshLinkEndPos, this.NavAgent.get_speed() * delta);
    else if (this._currentNavMeshLinkName == "JumpRockLink")
      moveToPosition = Vector3.MoveTowards(moveToPosition, this._currentNavMeshLinkEndPos, this.NavAgent.get_speed() * delta);
    else if (this._currentNavMeshLinkName == "JumpFoundationLink")
      moveToPosition = Vector3.MoveTowards(moveToPosition, this._currentNavMeshLinkEndPos, this.NavAgent.get_speed() * delta);
    else
      moveToPosition = Vector3.MoveTowards(moveToPosition, this._currentNavMeshLinkEndPos, this.NavAgent.get_speed() * delta);
  }

  private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
  {
    Vector3 vector3 = Vector3.op_Subtraction(moveToPosition, this._currentNavMeshLinkEndPos);
    if ((double) ((Vector3) ref vector3).get_sqrMagnitude() >= 0.00999999977648258)
      return false;
    moveToPosition = this._currentNavMeshLinkEndPos;
    this._traversingNavMeshLink = false;
    this._currentNavMeshLink = (OffMeshLinkData) null;
    this._currentNavMeshLinkName = string.Empty;
    this._currentNavMeshLinkOrientation = Quaternion.get_identity();
    this.CompleteNavMeshLink();
    return true;
  }

  private void CompleteNavMeshLink()
  {
    this.NavAgent.ActivateCurrentOffMeshLink(true);
    this.NavAgent.CompleteOffMeshLink();
    this.NavAgent.set_isStopped(false);
    this.NavAgent.set_obstacleAvoidanceType((ObstacleAvoidanceType) 4);
  }
}
