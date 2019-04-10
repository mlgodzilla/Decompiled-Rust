// Decompiled with JetBrains decompiler
// Type: EntityComponent`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class EntityComponent<T> : EntityComponentBase where T : BaseEntity
{
  [NonSerialized]
  private T _baseEntity;

  protected T baseEntity
  {
    get
    {
      if (Object.op_Equality((Object) (object) this._baseEntity, (Object) null))
        this.UpdateBaseEntity();
      return this._baseEntity;
    }
  }

  protected void UpdateBaseEntity()
  {
    if (!Object.op_Implicit((Object) this) || !Object.op_Implicit((Object) ((Component) this).get_gameObject()))
      return;
    this._baseEntity = ((Component) this).get_gameObject().ToBaseEntity() as T;
  }

  protected override BaseEntity GetBaseEntity()
  {
    return (BaseEntity) this.baseEntity;
  }
}
