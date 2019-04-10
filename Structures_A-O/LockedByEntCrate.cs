// Decompiled with JetBrains decompiler
// Type: LockedByEntCrate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class LockedByEntCrate : LootContainer
{
  public GameObject lockingEnt;

  public void SetLockingEnt(GameObject ent)
  {
    this.CancelInvoke(new Action(this.Think));
    this.SetLocked(false);
    this.lockingEnt = ent;
    if (!Object.op_Inequality((Object) this.lockingEnt, (Object) null))
      return;
    this.InvokeRepeating(new Action(this.Think), Random.Range(0.0f, 1f), 1f);
    this.SetLocked(true);
  }

  public void SetLocked(bool isLocked)
  {
    this.SetFlag(BaseEntity.Flags.OnFire, isLocked, false, true);
    this.SetFlag(BaseEntity.Flags.Locked, isLocked, false, true);
  }

  public void Think()
  {
    if (!Object.op_Equality((Object) this.lockingEnt, (Object) null) || !this.IsLocked())
      return;
    this.SetLockingEnt((GameObject) null);
  }
}
