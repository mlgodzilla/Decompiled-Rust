// Decompiled with JetBrains decompiler
// Type: TimedUnlootableCrate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

public class TimedUnlootableCrate : LootContainer
{
  public bool unlootableOnSpawn = true;
  public float unlootableDuration = 300f;

  public override void ServerInit()
  {
    base.ServerInit();
    if (!this.unlootableOnSpawn)
      return;
    this.SetUnlootableFor(this.unlootableDuration);
  }

  public void SetUnlootableFor(float duration)
  {
    this.SetFlag(BaseEntity.Flags.OnFire, true, false, true);
    this.SetFlag(BaseEntity.Flags.Locked, true, false, true);
    this.unlootableDuration = duration;
    this.Invoke(new Action(this.MakeLootable), duration);
  }

  public void MakeLootable()
  {
    this.SetFlag(BaseEntity.Flags.OnFire, false, false, true);
    this.SetFlag(BaseEntity.Flags.Locked, false, false, true);
  }
}
