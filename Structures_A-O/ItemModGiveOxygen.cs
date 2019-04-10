// Decompiled with JetBrains decompiler
// Type: ItemModGiveOxygen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class ItemModGiveOxygen : ItemMod
{
  public int amountToConsume = 1;
  public GameObjectRef inhaleEffect;
  public GameObjectRef exhaleEffect;
  public GameObjectRef bubblesEffect;
  private bool inhaled;

  public override void DoAction(Item item, BasePlayer player)
  {
    if (!item.hasCondition || (double) item.conditionNormalized == 0.0 || (Object.op_Equality((Object) player, (Object) null) || (double) player.WaterFactor() < 1.0) || (item.parent == null || item.parent != player.inventory.containerWear))
      return;
    Effect.server.Run(!this.inhaled ? this.inhaleEffect.resourcePath : this.exhaleEffect.resourcePath, (BaseEntity) player, StringPool.Get("jaw"), Vector3.get_zero(), Vector3.get_forward(), (Connection) null, false);
    this.inhaled = !this.inhaled;
    if (!this.inhaled && (double) WaterLevel.GetWaterDepth(player.eyes.position) > 3.0)
      Effect.server.Run(this.bubblesEffect.resourcePath, (BaseEntity) player, StringPool.Get("jaw"), Vector3.get_zero(), Vector3.get_forward(), (Connection) null, false);
    item.LoseCondition((float) this.amountToConsume);
    player.metabolism.oxygen.Add(1f);
  }
}
