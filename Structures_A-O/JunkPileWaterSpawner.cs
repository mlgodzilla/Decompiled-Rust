// Decompiled with JetBrains decompiler
// Type: JunkPileWaterSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class JunkPileWaterSpawner : SpawnGroup
{
  public BaseEntity attachToParent;

  protected override void PostSpawnProcess(BaseEntity entity, BaseSpawnPoint spawnPoint)
  {
    base.PostSpawnProcess(entity, spawnPoint);
    if (!Object.op_Inequality((Object) this.attachToParent, (Object) null))
      return;
    entity.SetParent(this.attachToParent, true, false);
  }
}
