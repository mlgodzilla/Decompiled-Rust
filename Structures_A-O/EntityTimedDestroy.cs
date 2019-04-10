// Decompiled with JetBrains decompiler
// Type: EntityTimedDestroy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class EntityTimedDestroy : EntityComponent<BaseEntity>
{
  public float secondsTillDestroy = 1f;

  private void OnEnable()
  {
    this.Invoke(new Action(this.TimedDestroy), this.secondsTillDestroy);
  }

  private void TimedDestroy()
  {
    if (Object.op_Inequality((Object) this.baseEntity, (Object) null))
      this.baseEntity.Kill(BaseNetworkable.DestroyMode.None);
    else
      Debug.LogWarning((object) "EntityTimedDestroy failed, baseEntity was already null!");
  }
}
