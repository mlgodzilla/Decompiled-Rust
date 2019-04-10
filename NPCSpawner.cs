// Decompiled with JetBrains decompiler
// Type: NPCSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class NPCSpawner : SpawnGroup
{
  public MonumentNavMesh monumentNavMesh;
  public bool shouldFillOnSpawn;

  public override void SpawnInitial()
  {
    this.fillOnSpawn = this.shouldFillOnSpawn;
    if (this.WaitingForNavMesh())
      this.Invoke(new Action(this.LateSpawn), 10f);
    else
      base.SpawnInitial();
  }

  public bool WaitingForNavMesh()
  {
    if (Object.op_Inequality((Object) this.monumentNavMesh, (Object) null))
      return this.monumentNavMesh.IsBuilding;
    return false;
  }

  public void LateSpawn()
  {
    if (!this.WaitingForNavMesh())
    {
      this.SpawnInitial();
      Debug.Log((object) "Navmesh complete, spawning");
    }
    else
      this.Invoke(new Action(this.LateSpawn), 5f);
  }
}
