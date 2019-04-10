// Decompiled with JetBrains decompiler
// Type: SupplyDrop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SupplyDrop : LootContainer
{
  public GameObjectRef parachutePrefab;
  public BaseEntity parachute;

  public override void ServerInit()
  {
    base.ServerInit();
    if (this.parachutePrefab.isValid)
      this.parachute = GameManager.server.CreateEntity(this.parachutePrefab.resourcePath, (Vector3) null, (Quaternion) null, true);
    if (Object.op_Implicit((Object) this.parachute))
    {
      this.parachute.SetParent((BaseEntity) this, "parachute_attach", false, false);
      this.parachute.Spawn();
    }
    this.isLootable = false;
    this.Invoke(new Action(this.MakeLootable), 300f);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.RemoveParachute();
  }

  public void RemoveParachute()
  {
    if (!Object.op_Implicit((Object) this.parachute))
      return;
    this.parachute.Kill(BaseNetworkable.DestroyMode.None);
    this.parachute = (BaseEntity) null;
  }

  public void MakeLootable()
  {
    this.isLootable = true;
  }

  private void OnCollisionEnter(Collision collision)
  {
    this.RemoveParachute();
    this.MakeLootable();
  }
}
