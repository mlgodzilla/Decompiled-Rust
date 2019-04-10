// Decompiled with JetBrains decompiler
// Type: ItemModUnwrap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class ItemModUnwrap : ItemMod
{
  public int minTries = 1;
  public int maxTries = 1;
  public LootSpawn revealList;
  public GameObjectRef successEffect;

  public override void ServerCommand(Item item, string command, BasePlayer player)
  {
    if (!(command == "unwrap") || item.amount <= 0)
      return;
    item.UseItem(1);
    int num = Random.Range(this.minTries, this.maxTries + 1);
    for (int index = 0; index < num; ++index)
      this.revealList.SpawnIntoContainer(player.inventory.containerMain);
    if (!this.successEffect.isValid)
      return;
    Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, (Vector3) null, (Connection) null, false);
  }
}
