// Decompiled with JetBrains decompiler
// Type: ItemModConsume
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Rust;
using UnityEngine;

[RequireComponent(typeof (ItemModConsumable))]
public class ItemModConsume : ItemMod
{
  public string eatGesture = "eat_2hand";
  public GameObjectRef consumeEffect;
  [Tooltip("Items that are given on consumption of this item")]
  public ItemAmountRandom[] product;
  public ItemModConsumable primaryConsumable;

  public virtual ItemModConsumable GetConsumable()
  {
    if (Object.op_Implicit((Object) this.primaryConsumable))
      return this.primaryConsumable;
    return (ItemModConsumable) ((Component) this).GetComponent<ItemModConsumable>();
  }

  public virtual GameObjectRef GetConsumeEffect()
  {
    return this.consumeEffect;
  }

  public override void DoAction(Item item, BasePlayer player)
  {
    if (item.amount < 1)
      return;
    GameObjectRef consumeEffect = this.GetConsumeEffect();
    if (consumeEffect.isValid)
    {
      Vector3 posLocal = player.IsDucked() ? new Vector3(0.0f, 1f, 0.0f) : new Vector3(0.0f, 2f, 0.0f);
      Effect.server.Run(consumeEffect.resourcePath, (BaseEntity) player, 0U, posLocal, Vector3.get_zero(), (Connection) null, false);
    }
    player.metabolism.MarkConsumption();
    ItemModConsumable consumable = this.GetConsumable();
    float num1 = (float) Mathf.Max(consumable.amountToConsume, 1);
    float num2 = Mathf.Min((float) item.amount, num1);
    float num3 = num2 / num1;
    float num4 = item.conditionNormalized;
    if ((double) consumable.conditionFractionToLose > 0.0)
      num4 = consumable.conditionFractionToLose;
    foreach (ItemModConsumable.ConsumableEffect effect in consumable.effects)
    {
      if ((double) player.healthFraction <= (double) effect.onlyIfHealthLessThan)
      {
        if (effect.type == MetabolismAttribute.Type.Health)
        {
          if ((double) effect.amount < 0.0)
            player.OnAttacked(new HitInfo((BaseEntity) player, (BaseEntity) player, DamageType.Generic, -effect.amount * num3 * num4, Vector3.op_Addition(((Component) player).get_transform().get_position(), Vector3.op_Multiply(((Component) player).get_transform().get_forward(), 1f))));
          else
            player.health += effect.amount * num3 * num4;
        }
        else
          player.metabolism.ApplyChange(effect.type, effect.amount * num3 * num4, effect.time * num3 * num4);
      }
    }
    if (this.product != null)
    {
      foreach (ItemAmountRandom itemAmountRandom in this.product)
      {
        int iAmount = Mathf.RoundToInt((float) itemAmountRandom.RandomAmount() * num4);
        if (iAmount > 0)
        {
          Item obj = ItemManager.Create(itemAmountRandom.itemDef, iAmount, 0UL);
          player.GiveItem(obj, BaseEntity.GiveItemReason.Generic);
        }
      }
    }
    if (string.IsNullOrEmpty(this.eatGesture))
      player.SignalBroadcast(BaseEntity.Signal.Gesture, this.eatGesture, (Connection) null);
    if ((double) consumable.conditionFractionToLose > 0.0)
      item.LoseCondition(consumable.conditionFractionToLose * item.maxCondition);
    else
      item.UseItem((int) num2);
  }

  public override bool CanDoAction(Item item, BasePlayer player)
  {
    return player.metabolism.CanConsume();
  }
}
