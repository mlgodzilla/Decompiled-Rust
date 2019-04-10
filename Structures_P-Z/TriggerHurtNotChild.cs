// Decompiled with JetBrains decompiler
// Type: TriggerHurtNotChild
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System;
using System.Linq;
using UnityEngine;

public class TriggerHurtNotChild : TriggerBase
{
  public float DamagePerSecond = 1f;
  public float DamageTickRate = 4f;
  public DamageType damageType;
  public BaseEntity SourceEntity;

  internal override GameObject InterestedInObject(GameObject obj)
  {
    obj = base.InterestedInObject(obj);
    if (Object.op_Equality((Object) obj, (Object) null))
      return (GameObject) null;
    BaseEntity baseEntity = obj.ToBaseEntity();
    if (Object.op_Equality((Object) baseEntity, (Object) null))
      return (GameObject) null;
    if (baseEntity.isClient)
      return (GameObject) null;
    if (Object.op_Equality((Object) baseEntity.parentEntity.Get(true), (Object) this.SourceEntity))
      return (GameObject) null;
    return ((Component) baseEntity).get_gameObject();
  }

  internal override void OnObjects()
  {
    this.InvokeRepeating(new Action(this.OnTick), 0.0f, 1f / this.DamageTickRate);
  }

  internal override void OnEmpty()
  {
    this.CancelInvoke(new Action(this.OnTick));
  }

  public new void OnDisable()
  {
    this.CancelInvoke(new Action(this.OnTick));
    base.OnDisable();
  }

  private void OnTick()
  {
    BaseEntity baseEntity = ((Component) this).get_gameObject().ToBaseEntity();
    if (this.entityContents == null)
      return;
    foreach (BaseEntity ent in this.entityContents.ToArray<BaseEntity>())
    {
      if (ent.IsValid())
      {
        BaseCombatEntity baseCombatEntity = ent as BaseCombatEntity;
        if (!Object.op_Equality((Object) baseCombatEntity, (Object) null))
          baseCombatEntity.Hurt(this.DamagePerSecond * (1f / this.DamageTickRate), this.damageType, baseEntity, true);
      }
    }
  }
}
