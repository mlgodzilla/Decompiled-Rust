// Decompiled with JetBrains decompiler
// Type: SpawnPointInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;

public class SpawnPointInstance : MonoBehaviour
{
  public SpawnGroup parentSpawnGroup;
  public BaseSpawnPoint parentSpawnPoint;

  public void Notify()
  {
    if (Object.op_Implicit((Object) this.parentSpawnGroup))
      this.parentSpawnGroup.ObjectSpawned(this);
    if (!Object.op_Implicit((Object) this.parentSpawnPoint))
      return;
    this.parentSpawnPoint.ObjectSpawned(this);
  }

  protected void OnDestroy()
  {
    if (Application.isQuitting != null)
      return;
    if (Object.op_Implicit((Object) this.parentSpawnGroup))
      this.parentSpawnGroup.ObjectRetired(this);
    if (!Object.op_Implicit((Object) this.parentSpawnPoint))
      return;
    this.parentSpawnPoint.ObjectRetired(this);
  }

  public SpawnPointInstance()
  {
    base.\u002Ector();
  }
}
