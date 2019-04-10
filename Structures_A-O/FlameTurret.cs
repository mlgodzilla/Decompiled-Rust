// Decompiled with JetBrains decompiler
// Type: FlameTurret
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FlameTurret : StorageContainer
{
  public float arc = 45f;
  public float triggeredDuration = 5f;
  public float flameRange = 7f;
  public float flameRadius = 4f;
  public float fuelPerSec = 1f;
  private int turnDir = 1;
  private float triggerCheckRate = 2f;
  public Transform upper;
  public Vector3 aimDir;
  public Transform eyeTransform;
  public List<DamageTypeEntry> damagePerSec;
  public GameObjectRef triggeredEffect;
  public GameObjectRef fireballPrefab;
  public GameObjectRef explosionEffect;
  public TargetTrigger trigger;
  private float nextFireballTime;
  private float lastServerThink;
  private float triggeredTime;
  private float nextTriggerCheckTime;
  private float pendingFuel;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("FlameTurret.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool IsTriggered()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved4);
  }

  public Vector3 GetEyePosition()
  {
    return this.eyeTransform.get_position();
  }

  public void Update()
  {
    if (!this.isServer)
      return;
    this.ServerThink();
  }

  public override bool CanPickup(BasePlayer player)
  {
    if (base.CanPickup(player))
      return !this.IsTriggered();
    return false;
  }

  public void SetTriggered(bool triggered)
  {
    if (triggered && this.HasFuel())
      this.triggeredTime = Time.get_realtimeSinceStartup();
    this.SetFlag(BaseEntity.Flags.Reserved4, triggered && this.HasFuel(), false, true);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRepeating(new Action(this.SendAimDir), 0.0f, 0.1f);
  }

  public void SendAimDir()
  {
    this.ClientRPC<Vector3>((Connection) null, "CLIENT_ReceiveAimDir", this.aimDir);
  }

  public float GetSpinSpeed()
  {
    return this.IsTriggered() ? 180f : 45f;
  }

  public override void OnAttacked(HitInfo info)
  {
    if (this.isClient)
      return;
    if (info.damageTypes.IsMeleeType())
      this.SetTriggered(true);
    base.OnAttacked(info);
  }

  public void MovementUpdate(float delta)
  {
    this.aimDir = Vector3.op_Addition(this.aimDir, Vector3.op_Multiply(new Vector3(0.0f, delta * this.GetSpinSpeed(), 0.0f), (float) this.turnDir));
    if (this.aimDir.y < (double) this.arc && this.aimDir.y > -(double) this.arc)
      return;
    this.turnDir *= -1;
    this.aimDir.y = (__Null) (double) Mathf.Clamp((float) this.aimDir.y, -this.arc, this.arc);
  }

  public void ServerThink()
  {
    float delta = Time.get_realtimeSinceStartup() - this.lastServerThink;
    if ((double) delta < 0.100000001490116)
      return;
    int num1 = this.IsTriggered() ? 1 : 0;
    this.lastServerThink = Time.get_realtimeSinceStartup();
    this.MovementUpdate(delta);
    if (this.IsTriggered() && ((double) Time.get_realtimeSinceStartup() - (double) this.triggeredTime > (double) this.triggeredDuration || !this.HasFuel()))
      this.SetTriggered(false);
    if (!this.IsTriggered() && this.HasFuel() && this.CheckTrigger())
    {
      this.SetTriggered(true);
      Effect.server.Run(this.triggeredEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    }
    int num2 = this.IsTriggered() ? 1 : 0;
    if (num1 != num2)
      this.SendNetworkUpdateImmediate(false);
    if (!this.IsTriggered())
      return;
    this.DoFlame(delta);
  }

  public bool CheckTrigger()
  {
    if ((double) Time.get_realtimeSinceStartup() < (double) this.nextTriggerCheckTime)
      return false;
    this.nextTriggerCheckTime = Time.get_realtimeSinceStartup() + 1f / this.triggerCheckRate;
    List<RaycastHit> list = (List<RaycastHit>) Pool.GetList<RaycastHit>();
    HashSet<BaseEntity> entityContents = this.trigger.entityContents;
    bool flag = false;
    if (entityContents != null)
    {
      foreach (Component component1 in entityContents)
      {
        BasePlayer component2 = (BasePlayer) component1.GetComponent<BasePlayer>();
        if (!component2.IsSleeping() && component2.IsAlive() && (!component2.IsBuildingAuthed() && ((Component) component2).get_transform().get_position().y <= this.GetEyePosition().y + 0.5))
        {
          list.Clear();
          Vector3 position = component2.eyes.position;
          Vector3 vector3 = Vector3.op_Subtraction(this.GetEyePosition(), component2.eyes.position);
          Vector3 normalized = ((Vector3) ref vector3).get_normalized();
          GamePhysics.TraceAll(new Ray(position, normalized), 0.0f, list, 9f, 1218519297, (QueryTriggerInteraction) 0);
          for (int index = 0; index < list.Count; ++index)
          {
            BaseEntity entity = list[index].GetEntity();
            if (Object.op_Inequality((Object) entity, (Object) null) && (Object.op_Equality((Object) entity, (Object) this) || entity.EqualNetID((BaseNetworkable) this)))
            {
              flag = true;
              break;
            }
            if (!Object.op_Inequality((Object) entity, (Object) null) || entity.ShouldBlockProjectiles())
              break;
          }
          if (flag)
            break;
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<RaycastHit>((List<M0>&) ref list);
    return flag;
  }

  public override void OnKilled(HitInfo info)
  {
    double num1 = (double) this.GetFuelAmount() / 500.0;
    DamageUtil.RadiusDamage((BaseEntity) this, this.LookupPrefab(), this.GetEyePosition(), 2f, 6f, this.damagePerSec, 133120, true);
    Effect.server.Run(this.explosionEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    int num2 = Mathf.CeilToInt(Mathf.Clamp((float) (num1 * 8.0), 1f, 8f));
    for (int index = 0; index < num2; ++index)
    {
      BaseEntity entity = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, ((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), true);
      if (Object.op_Implicit((Object) entity))
      {
        Vector3 onUnitSphere = Random.get_onUnitSphere();
        ((Component) entity).get_transform().set_position(Vector3.op_Addition(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 1.5f, 0.0f)), Vector3.op_Multiply(onUnitSphere, Random.Range(-1f, 1f))));
        entity.Spawn();
        entity.SetVelocity(Vector3.op_Multiply(onUnitSphere, (float) Random.Range(3, 10)));
      }
    }
    base.OnKilled(info);
  }

  public int GetFuelAmount()
  {
    Item slot = this.inventory.GetSlot(0);
    if (slot == null || slot.amount < 1)
      return 0;
    return slot.amount;
  }

  public bool HasFuel()
  {
    return this.GetFuelAmount() > 0;
  }

  public bool UseFuel(float seconds)
  {
    Item slot = this.inventory.GetSlot(0);
    if (slot == null || slot.amount < 1)
      return false;
    this.pendingFuel += seconds * this.fuelPerSec;
    if ((double) this.pendingFuel >= 1.0)
    {
      int amountToConsume = Mathf.FloorToInt(this.pendingFuel);
      slot.UseItem(amountToConsume);
      this.pendingFuel -= (float) amountToConsume;
    }
    return true;
  }

  public void DoFlame(float delta)
  {
    if (!this.UseFuel(delta))
      return;
    Ray ray;
    ((Ray) ref ray).\u002Ector(this.GetEyePosition(), ((Component) this).get_transform().TransformDirection(Quaternion.op_Multiply(Quaternion.Euler(this.aimDir), Vector3.get_forward())));
    Vector3 origin = ((Ray) ref ray).get_origin();
    RaycastHit raycastHit;
    bool flag = Physics.SphereCast(ray, 0.4f, ref raycastHit, this.flameRange, 1218652417);
    if (!flag)
      ((RaycastHit) ref raycastHit).set_point(Vector3.op_Addition(origin, Vector3.op_Multiply(((Ray) ref ray).get_direction(), this.flameRange)));
    float amount = this.damagePerSec[0].amount;
    this.damagePerSec[0].amount = amount * delta;
    DamageUtil.RadiusDamage((BaseEntity) this, this.LookupPrefab(), Vector3.op_Subtraction(((RaycastHit) ref raycastHit).get_point(), Vector3.op_Multiply(((Ray) ref ray).get_direction(), 0.1f)), this.flameRadius * 0.5f, this.flameRadius, this.damagePerSec, 2230272, true);
    DamageUtil.RadiusDamage((BaseEntity) this, this.LookupPrefab(), Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 1.25f, 0.0f)), 0.25f, 0.25f, this.damagePerSec, 133120, false);
    this.damagePerSec[0].amount = amount;
    if ((double) Time.get_realtimeSinceStartup() < (double) this.nextFireballTime)
      return;
    this.nextFireballTime = Time.get_realtimeSinceStartup() + Random.Range(1f, 2f);
    Vector3 vector3 = Random.Range(0, 10) <= 7 & flag ? ((RaycastHit) ref raycastHit).get_point() : Vector3.op_Addition(((Ray) ref ray).get_origin(), Vector3.op_Multiply(Vector3.op_Multiply(((Ray) ref ray).get_direction(), flag ? ((RaycastHit) ref raycastHit).get_distance() : this.flameRange), Random.Range(0.4f, 1f)));
    BaseEntity entity = GameManager.server.CreateEntity(this.fireballPrefab.resourcePath, Vector3.op_Subtraction(vector3, Vector3.op_Multiply(((Ray) ref ray).get_direction(), 0.25f)), (Quaternion) null, true);
    if (!Object.op_Implicit((Object) entity))
      return;
    entity.creatorEntity = (BaseEntity) this;
    entity.Spawn();
  }
}
