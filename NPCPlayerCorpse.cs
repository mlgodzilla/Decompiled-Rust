// Decompiled with JetBrains decompiler
// Type: NPCPlayerCorpse
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

public class NPCPlayerCorpse : PlayerCorpse
{
  private bool lootEnabled;

  public override float GetRemovalTime()
  {
    return 60f;
  }

  public override bool CanLoot()
  {
    return this.lootEnabled;
  }

  public void SetLootableIn(float when)
  {
    this.Invoke(new Action(this.EnableLooting), when);
  }

  public void EnableLooting()
  {
    this.lootEnabled = true;
  }
}
