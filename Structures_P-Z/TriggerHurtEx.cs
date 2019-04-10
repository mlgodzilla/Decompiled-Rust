// Decompiled with JetBrains decompiler
// Type: TriggerHurtEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerHurtEx : TriggerBase, IServerComponent
{
  public float repeatRate = 0.1f;
  public bool damageEnabled = true;
  [Header("On Enter")]
  public List<DamageTypeEntry> damageOnEnter;
  public GameObjectRef effectOnEnter;
  public TriggerHurtEx.HurtType hurtTypeOnEnter;
  [Header("On Timer (damage per second)")]
  public List<DamageTypeEntry> damageOnTimer;
  public GameObjectRef effectOnTimer;
  public TriggerHurtEx.HurtType hurtTypeOnTimer;
  [Header("On Move (damage per meter)")]
  public List<DamageTypeEntry> damageOnMove;
  public GameObjectRef effectOnMove;
  public TriggerHurtEx.HurtType hurtTypeOnMove;
  [Header("On Leave")]
  public List<DamageTypeEntry> damageOnLeave;
  public GameObjectRef effectOnLeave;
  public TriggerHurtEx.HurtType hurtTypeOnLeave;
  internal Dictionary<BaseEntity, TriggerHurtEx.EntityTriggerInfo> entityInfo;
  internal List<BaseEntity> entityAddList;
  internal List<BaseEntity> entityLeaveList;

  internal override GameObject InterestedInObject(GameObject obj)
  {
    obj = base.InterestedInObject(obj);
    if (Object.op_Equality((Object) obj, (Object) null))
      return (GameObject) null;
    BaseEntity baseEntity = obj.ToBaseEntity();
    if (Object.op_Equality((Object) baseEntity, (Object) null))
      return (GameObject) null;
    if (!(baseEntity is BaseCombatEntity))
      return (GameObject) null;
    if (baseEntity.isClient)
      return (GameObject) null;
    return ((Component) baseEntity).get_gameObject();
  }

  internal void DoDamage(
    BaseEntity ent,
    TriggerHurtEx.HurtType type,
    List<DamageTypeEntry> damage,
    GameObjectRef effect,
    float multiply = 1f)
  {
    if (!this.damageEnabled)
      return;
    using (TimeWarning.New("TriggerHurtEx.DoDamage", 0.1f))
    {
      if (damage != null && damage.Count > 0)
      {
        BaseCombatEntity baseCombatEntity = ent as BaseCombatEntity;
        if (Object.op_Implicit((Object) baseCombatEntity))
        {
          HitInfo info = new HitInfo();
          info.damageTypes.Add(damage);
          info.damageTypes.ScaleAll(multiply);
          info.DoHitEffects = true;
          info.DidHit = true;
          info.Initiator = ((Component) this).get_gameObject().ToBaseEntity();
          info.PointStart = ((Component) this).get_transform().get_position();
          info.PointEnd = ((Component) baseCombatEntity).get_transform().get_position();
          if (type == TriggerHurtEx.HurtType.Simple)
            baseCombatEntity.Hurt(info);
          else
            baseCombatEntity.OnAttacked(info);
        }
      }
      if (!effect.isValid)
        return;
      Effect.server.Run(effect.resourcePath, ent, StringPool.closest, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    }
  }

  internal override void OnEntityEnter(BaseEntity ent)
  {
    base.OnEntityEnter(ent);
    if (Object.op_Equality((Object) ent, (Object) null))
      return;
    if (this.entityAddList == null)
      this.entityAddList = new List<BaseEntity>();
    this.entityAddList.Add(ent);
    this.Invoke(new Action(this.ProcessQueues), 0.1f);
  }

  internal override void OnEntityLeave(BaseEntity ent)
  {
    base.OnEntityLeave(ent);
    if (Object.op_Equality((Object) ent, (Object) null))
      return;
    if (this.entityLeaveList == null)
      this.entityLeaveList = new List<BaseEntity>();
    this.entityLeaveList.Add(ent);
    this.Invoke(new Action(this.ProcessQueues), 0.1f);
  }

  internal override void OnObjects()
  {
    this.InvokeRepeating(new Action(this.OnTick), this.repeatRate, this.repeatRate);
  }

  internal override void OnEmpty()
  {
    this.CancelInvoke(new Action(this.OnTick));
  }

  private void OnTick()
  {
    this.ProcessQueues();
    if (this.entityInfo == null)
      return;
    foreach (KeyValuePair<BaseEntity, TriggerHurtEx.EntityTriggerInfo> keyValuePair in this.entityInfo.ToArray<KeyValuePair<BaseEntity, TriggerHurtEx.EntityTriggerInfo>>())
    {
      if (keyValuePair.Key.IsValid())
      {
        Vector3 position = ((Component) keyValuePair.Key).get_transform().get_position();
        Vector3 vector3 = Vector3.op_Subtraction(position, keyValuePair.Value.lastPosition);
        float magnitude = ((Vector3) ref vector3).get_magnitude();
        if ((double) magnitude > 0.00999999977648258)
        {
          keyValuePair.Value.lastPosition = position;
          this.DoDamage(keyValuePair.Key, this.hurtTypeOnMove, this.damageOnMove, this.effectOnMove, magnitude);
        }
        this.DoDamage(keyValuePair.Key, this.hurtTypeOnTimer, this.damageOnTimer, this.effectOnTimer, this.repeatRate);
      }
    }
  }

  private void ProcessQueues()
  {
    if (this.entityAddList != null)
    {
      foreach (BaseEntity entityAdd in this.entityAddList)
      {
        if (entityAdd.IsValid())
        {
          this.DoDamage(entityAdd, this.hurtTypeOnEnter, this.damageOnEnter, this.effectOnEnter, 1f);
          if (this.entityInfo == null)
            this.entityInfo = new Dictionary<BaseEntity, TriggerHurtEx.EntityTriggerInfo>();
          if (!this.entityInfo.ContainsKey(entityAdd))
            this.entityInfo.Add(entityAdd, new TriggerHurtEx.EntityTriggerInfo()
            {
              lastPosition = ((Component) entityAdd).get_transform().get_position()
            });
        }
      }
      this.entityAddList = (List<BaseEntity>) null;
    }
    if (this.entityLeaveList == null)
      return;
    foreach (BaseEntity entityLeave in this.entityLeaveList)
    {
      if (entityLeave.IsValid())
      {
        this.DoDamage(entityLeave, this.hurtTypeOnLeave, this.damageOnLeave, this.effectOnLeave, 1f);
        if (this.entityInfo != null)
        {
          this.entityInfo.Remove(entityLeave);
          if (this.entityInfo.Count == 0)
            this.entityInfo = (Dictionary<BaseEntity, TriggerHurtEx.EntityTriggerInfo>) null;
        }
      }
    }
    this.entityLeaveList.Clear();
  }

  public enum HurtType
  {
    Simple,
    IncludeBleedingAndScreenShake,
  }

  public class EntityTriggerInfo
  {
    public Vector3 lastPosition;
  }
}
