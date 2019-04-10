// Decompiled with JetBrains decompiler
// Type: DummySwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class DummySwitch : IOEntity
{
  public string listenString = "";
  public float duration = -1f;

  public override bool WantsPower()
  {
    return this.IsOn();
  }

  public override void ResetIOState()
  {
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    Debug.Log((object) "Resetting");
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    if (!this.IsOn())
      return 0;
    return this.GetCurrentEnergy();
  }

  public void SetOn(bool wantsOn)
  {
    this.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
    this.MarkDirty();
    if (!this.IsOn() || (double) this.duration == -1.0)
      return;
    this.Invoke(new Action(this.SetOff), this.duration);
  }

  public void SetOff()
  {
    this.SetOn(false);
  }

  public override void OnEntityMessage(BaseEntity from, string msg)
  {
    if (!(msg == this.listenString))
      return;
    if (this.IsOn())
      this.SetOn(false);
    this.SetOn(true);
  }
}
