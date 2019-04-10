// Decompiled with JetBrains decompiler
// Type: RepairBench
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class RepairBench : StorageContainer
{
  public float maxConditionLostOnRepair = 0.2f;
  public GameObjectRef skinchangeEffect;
  private float nextSkinChangeTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("RepairBench.OnRpcMessage", 0.1f))
    {
      if (rpc == 1942825351U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ChangeSkin "));
        using (TimeWarning.New("ChangeSkin", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.ChangeSkin(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in ChangeSkin");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1178348163U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RepairItem "));
          using (TimeWarning.New("RepairItem", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RepairItem(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RepairItem");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public float GetRepairFraction(Item itemToRepair)
  {
    return (float) (1.0 - (double) itemToRepair.condition / (double) itemToRepair.maxCondition);
  }

  public float RepairCostFraction(Item itemToRepair)
  {
    return this.GetRepairFraction(itemToRepair) * 0.2f;
  }

  public void GetRepairCostList(ItemBlueprint bp, List<ItemAmount> allIngredients)
  {
    foreach (ItemAmount ingredient in bp.ingredients)
      allIngredients.Add(new ItemAmount(ingredient.itemDef, ingredient.amount));
    foreach (ItemAmount ingredient1 in bp.ingredients)
    {
      if (ingredient1.itemDef.category == ItemCategory.Component && Object.op_Inequality((Object) ingredient1.itemDef.Blueprint, (Object) null))
      {
        bool flag = false;
        ItemAmount ingredient2 = ingredient1.itemDef.Blueprint.ingredients[0];
        foreach (ItemAmount allIngredient in allIngredients)
        {
          if (Object.op_Equality((Object) allIngredient.itemDef, (Object) ingredient2.itemDef))
          {
            allIngredient.amount += ingredient2.amount * ingredient1.amount;
            flag = true;
            break;
          }
        }
        if (!flag)
          allIngredients.Add(new ItemAmount(ingredient2.itemDef, ingredient2.amount * ingredient1.amount));
      }
    }
  }

  public void debugprint(string toPrint)
  {
    if (Global.developer <= 0)
      return;
    Debug.LogWarning((object) toPrint);
  }

  [BaseEntity.RPC_Server]
  public void ChangeSkin(BaseEntity.RPCMessage msg)
  {
    if ((double) Time.get_realtimeSinceStartup() < (double) this.nextSkinChangeTime)
      return;
    BasePlayer player = msg.player;
    int num = msg.read.Int32();
    Item slot = this.inventory.GetSlot(0);
    if (slot == null)
      return;
    if (num != 0 && !player.blueprints.steamInventory.HasItem(num))
    {
      this.debugprint("RepairBench.ChangeSkin player does not have item :" + (object) num + ":");
    }
    else
    {
      ulong skin = ItemDefinition.FindSkin(slot.info.itemid, num);
      if ((long) skin == (long) slot.skin)
      {
        this.debugprint("RepairBench.ChangeSkin cannot apply same skin twice : " + (object) skin + ": " + (object) slot.skin);
      }
      else
      {
        this.nextSkinChangeTime = Time.get_realtimeSinceStartup() + 0.75f;
        slot.skin = skin;
        slot.MarkDirty();
        BaseEntity heldEntity = slot.GetHeldEntity();
        if (Object.op_Inequality((Object) heldEntity, (Object) null))
        {
          heldEntity.skinID = skin;
          heldEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
        }
        if (!this.skinchangeEffect.isValid)
          return;
        Effect.server.Run(this.skinchangeEffect.resourcePath, (BaseEntity) this, 0U, new Vector3(0.0f, 1.5f, 0.0f), Vector3.get_zero(), (Connection) null, false);
      }
    }
  }

  [BaseEntity.RPC_Server]
  public void RepairItem(BaseEntity.RPCMessage msg)
  {
    Item slot = this.inventory.GetSlot(0);
    if (slot == null)
      return;
    ItemDefinition info = slot.info;
    ItemBlueprint component = (ItemBlueprint) ((Component) info).GetComponent<ItemBlueprint>();
    if (!Object.op_Implicit((Object) component) || !info.condition.repairable || (double) slot.condition == (double) slot.maxCondition)
      return;
    BasePlayer player = msg.player;
    if ((player.blueprints.HasUnlocked(info) ? 1 : (!Object.op_Inequality((Object) info.Blueprint, (Object) null) ? 0 : (!info.Blueprint.isResearchable ? 1 : 0))) == 0)
      return;
    float num = this.RepairCostFraction(slot);
    bool flag = false;
    List<ItemAmount> list = (List<ItemAmount>) Pool.GetList<ItemAmount>();
    this.GetRepairCostList(component, list);
    foreach (ItemAmount itemAmount in list)
    {
      if (itemAmount.itemDef.category != ItemCategory.Component)
      {
        int amount = player.inventory.GetAmount(itemAmount.itemDef.itemid);
        if (Mathf.CeilToInt(itemAmount.amount * num) > amount)
        {
          flag = true;
          break;
        }
      }
    }
    if (flag)
    {
      // ISSUE: cast to a reference type
      Pool.FreeList<ItemAmount>((List<M0>&) ref list);
    }
    else
    {
      foreach (ItemAmount itemAmount in list)
      {
        if (itemAmount.itemDef.category != ItemCategory.Component)
        {
          int amount = Mathf.CeilToInt(itemAmount.amount * num);
          player.inventory.Take((List<Item>) null, itemAmount.itemid, amount);
        }
      }
      // ISSUE: cast to a reference type
      Pool.FreeList<ItemAmount>((List<M0>&) ref list);
      slot.DoRepair(this.maxConditionLostOnRepair);
      if (Global.developer > 0)
        Debug.Log((object) ("Item repaired! condition : " + (object) slot.condition + "/" + (object) slot.maxCondition));
      Effect.server.Run("assets/bundled/prefabs/fx/repairbench/itemrepair.prefab", (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    }
  }
}
