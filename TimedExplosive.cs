// Decompiled with JetBrains decompiler
// Type: TimedExplosive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Rust;
using Rust.Ai;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TimedExplosive : BaseEntity
{
  public float timerAmountMin = 10f;
  public float timerAmountMax = 20f;
  public float explosionRadius = 10f;
  public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();
  public float minExplosionRadius;
  public bool canStick;
  public bool onlyDamageParent;
  public GameObjectRef explosionEffect;
  public GameObjectRef stickEffect;
  public GameObjectRef bounceEffect;
  public bool explosionUsesForward;
  public bool waterCausesExplosion;
  [NonSerialized]
  private float lastBounceTime;

  public override void ServerInit()
  {
    this.lastBounceTime = Time.get_time();
    base.ServerInit();
    this.SetFuse(this.GetRandomTimerTime());
    this.ReceiveCollisionMessages(true);
    if (!this.waterCausesExplosion)
      return;
    this.InvokeRepeating(new Action(this.WaterCheck), 0.0f, 0.5f);
  }

  public void WaterCheck()
  {
    if (!this.waterCausesExplosion || (double) this.WaterFactor() < 0.5)
      return;
    this.Explode();
  }

  public virtual void SetFuse(float fuseLength)
  {
    if (!this.isServer)
      return;
    this.Invoke(new Action(this.Explode), fuseLength);
  }

  public virtual float GetRandomTimerTime()
  {
    return Random.Range(this.timerAmountMin, this.timerAmountMax);
  }

  public virtual void ProjectileImpact(RaycastHit info)
  {
    this.Explode();
  }

  public virtual void Explode()
  {
    ((Collider) ((Component) this).GetComponent<Collider>()).set_enabled(false);
    if (this.explosionEffect.isValid)
      Effect.server.Run(this.explosionEffect.resourcePath, this.PivotPoint(), this.explosionUsesForward ? ((Component) this).get_transform().get_forward() : Vector3.get_up(), (Connection) null, true);
    if (this.damageTypes.Count > 0)
    {
      if (this.onlyDamageParent)
      {
        DamageUtil.RadiusDamage(this.creatorEntity, this.LookupPrefab(), this.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 141568, true);
        BaseEntity parentEntity = this.GetParentEntity();
        BaseCombatEntity baseCombatEntity;
        for (baseCombatEntity = parentEntity as BaseCombatEntity; Object.op_Equality((Object) baseCombatEntity, (Object) null) && Object.op_Inequality((Object) parentEntity, (Object) null) && parentEntity.HasParent(); baseCombatEntity = parentEntity as BaseCombatEntity)
          parentEntity = parentEntity.GetParentEntity();
        if (Object.op_Implicit((Object) baseCombatEntity))
        {
          HitInfo info = new HitInfo();
          info.Initiator = this.creatorEntity;
          info.WeaponPrefab = this.LookupPrefab();
          info.damageTypes.Add(this.damageTypes);
          baseCombatEntity.Hurt(info);
        }
        if (Object.op_Inequality((Object) this.creatorEntity, (Object) null) && this.damageTypes != null)
        {
          float num = 0.0f;
          foreach (DamageTypeEntry damageType in this.damageTypes)
            num += damageType.amount;
          Sense.Stimulate(new Sensation()
          {
            Type = SensationType.Explosion,
            Position = ((Component) this.creatorEntity).get_transform().get_position(),
            Radius = this.explosionRadius * 17f,
            DamagePotential = num,
            InitiatorPlayer = this.creatorEntity as BasePlayer,
            Initiator = this.creatorEntity
          });
        }
      }
      else
      {
        DamageUtil.RadiusDamage(this.creatorEntity, this.LookupPrefab(), this.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 1075980544, true);
        if (Object.op_Inequality((Object) this.creatorEntity, (Object) null) && this.damageTypes != null)
        {
          float num = 0.0f;
          foreach (DamageTypeEntry damageType in this.damageTypes)
            num += damageType.amount;
          Sense.Stimulate(new Sensation()
          {
            Type = SensationType.Explosion,
            Position = ((Component) this.creatorEntity).get_transform().get_position(),
            Radius = this.explosionRadius * 17f,
            DamagePotential = num,
            InitiatorPlayer = this.creatorEntity as BasePlayer,
            Initiator = this.creatorEntity
          });
        }
      }
    }
    if (this.IsDestroyed)
      return;
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override void OnCollision(Collision collision, BaseEntity hitEntity)
  {
    if (this.canStick && !this.IsStuck())
    {
      bool flag = true;
      if (Object.op_Implicit((Object) hitEntity))
      {
        flag = this.CanStickTo(hitEntity);
        if (!flag)
        {
          Collider component = (Collider) ((Component) this).GetComponent<Collider>();
          if (Object.op_Inequality((Object) collision.get_collider(), (Object) null) && Object.op_Inequality((Object) component, (Object) null))
            Physics.IgnoreCollision(collision.get_collider(), component);
        }
      }
      if (flag)
        this.DoCollisionStick(collision, hitEntity);
    }
    this.DoBounceEffect();
  }

  public virtual bool CanStickTo(BaseEntity entity)
  {
    return Object.op_Equality((Object) ((Component) entity).GetComponent<DecorDeployable>(), (Object) null);
  }

  private void DoBounceEffect()
  {
    if (!this.bounceEffect.isValid || (double) Time.get_time() - (double) this.lastBounceTime < 0.200000002980232)
      return;
    Rigidbody component = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    if (Object.op_Implicit((Object) component))
    {
      Vector3 velocity = component.get_velocity();
      if ((double) ((Vector3) ref velocity).get_magnitude() < 1.0)
        return;
    }
    if (this.bounceEffect.isValid)
      Effect.server.Run(this.bounceEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, true);
    this.lastBounceTime = Time.get_time();
  }

  private void DoCollisionStick(Collision collision, BaseEntity ent)
  {
    this.DoStick(((ContactPoint) ref collision.get_contacts()[0]).get_point(), ((ContactPoint) ref collision.get_contacts()[0]).get_normal(), ent);
  }

  public virtual void SetMotionEnabled(bool wantsMotion)
  {
    Rigidbody component = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    if (!Object.op_Implicit((Object) component))
      return;
    component.set_useGravity(wantsMotion);
    component.set_isKinematic(!wantsMotion);
  }

  public bool IsStuck()
  {
    Rigidbody component1 = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    if (Object.op_Implicit((Object) component1) && !component1.get_isKinematic())
      return false;
    Collider component2 = (Collider) ((Component) this).GetComponent<Collider>();
    if (Object.op_Implicit((Object) component2) && component2.get_enabled())
      return false;
    return this.parentEntity.IsValid(true);
  }

  public void DoStick(Vector3 position, Vector3 normal, BaseEntity ent)
  {
    if (Object.op_Equality((Object) ent, (Object) null))
      return;
    if (ent is TimedExplosive)
    {
      if (!ent.HasParent())
        return;
      position = ((Component) ent).get_transform().get_position();
      ent = ent.parentEntity.Get(true);
    }
    this.SetMotionEnabled(false);
    this.SetCollisionEnabled(false);
    if (this.HasChild(ent))
      return;
    ((Component) this).get_transform().set_position(position);
    ((Component) this).get_transform().set_rotation(Quaternion.LookRotation(normal, ((Component) this).get_transform().get_up()));
    this.SetParent(ent, StringPool.closest, true, false);
    if (this.stickEffect.isValid)
      Effect.server.Run(this.stickEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, true);
    this.ReceiveCollisionMessages(false);
  }

  private void UnStick()
  {
    if (!Object.op_Implicit((Object) this.GetParentEntity()))
      return;
    this.SetParent((BaseEntity) null, true, true);
    this.SetMotionEnabled(true);
    this.SetCollisionEnabled(true);
    this.ReceiveCollisionMessages(true);
  }

  internal override void OnParentRemoved()
  {
    this.UnStick();
  }

  public virtual void SetCollisionEnabled(bool wantsCollision)
  {
    Collider component = (Collider) ((Component) this).GetComponent<Collider>();
    if (component.get_enabled() == wantsCollision)
      return;
    component.set_enabled(wantsCollision);
  }

  public override bool PhysicsDriven()
  {
    return true;
  }
}
