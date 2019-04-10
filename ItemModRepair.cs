// Decompiled with JetBrains decompiler
// Type: ItemModRepair
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class ItemModRepair : ItemMod
{
  public float conditionLost = 0.05f;
  public GameObjectRef successEffect;
  public int workbenchLvlRequired;

  public bool HasCraftLevel(BasePlayer player = null)
  {
    if (Object.op_Inequality((Object) player, (Object) null) && player.isServer)
      return (double) player.currentCraftLevel >= (double) this.workbenchLvlRequired;
    return false;
  }

  public override void ServerCommand(Item item, string command, BasePlayer player)
  {
    if (!(command == "refill") || player.IsSwimming() || (!this.HasCraftLevel(player) || (double) item.conditionNormalized >= 1.0))
      return;
    item.DoRepair(this.conditionLost);
    if (!this.successEffect.isValid)
      return;
    Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, (Vector3) null, (Connection) null, false);
  }
}
