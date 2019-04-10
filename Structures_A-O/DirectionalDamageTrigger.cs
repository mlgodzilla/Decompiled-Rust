// Decompiled with JetBrains decompiler
// Type: DirectionalDamageTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DirectionalDamageTrigger : TriggerBase
{
  public float repeatRate = 1f;
  public List<DamageTypeEntry> damageType;
  public GameObjectRef attackEffect;

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
    if (this.attackEffect.isValid)
      Effect.server.Run(this.attackEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    if (this.entityContents == null)
      return;
    foreach (BaseEntity ent in this.entityContents.ToArray<BaseEntity>())
    {
      if (ent.IsValid())
      {
        BaseCombatEntity baseCombatEntity = ent as BaseCombatEntity;
        if (!Object.op_Equality((Object) baseCombatEntity, (Object) null))
        {
          HitInfo info = new HitInfo();
          info.damageTypes.Add(this.damageType);
          info.DoHitEffects = true;
          info.DidHit = true;
          info.PointStart = ((Component) this).get_transform().get_position();
          info.PointEnd = ((Component) baseCombatEntity).get_transform().get_position();
          baseCombatEntity.Hurt(info);
        }
      }
    }
  }
}
