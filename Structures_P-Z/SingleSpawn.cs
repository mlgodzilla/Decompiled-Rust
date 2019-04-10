// Decompiled with JetBrains decompiler
// Type: SingleSpawn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

public class SingleSpawn : SpawnGroup
{
  public override bool WantsInitialSpawn()
  {
    return false;
  }

  public void FillDelay(float delay)
  {
    this.Invoke(new Action(((SpawnGroup) this).Fill), delay);
  }
}
