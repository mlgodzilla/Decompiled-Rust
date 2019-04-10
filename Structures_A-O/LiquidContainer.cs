// Decompiled with JetBrains decompiler
// Type: LiquidContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class LiquidContainer : StorageContainer, ISplashable
{
  public ItemDefinition defaultLiquid;
  public int startingAmount;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("LiquidContainer.OnRpcMessage", 0.1f))
    {
      if (rpc == 2002733690U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SVDrink "));
          using (TimeWarning.New("SVDrink", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("SVDrink", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SVDrink(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SVDrink");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (this.startingAmount <= 0)
      return;
    this.inventory.AddItem(this.defaultLiquid, this.startingAmount);
  }

  protected void UpdateOnFlag()
  {
    this.SetFlag(BaseEntity.Flags.On, this.inventory.itemList.Count > 0 && this.inventory.itemList[0].amount > 0, false, true);
  }

  protected override void OnInventoryDirty()
  {
    this.UpdateOnFlag();
  }

  public virtual void OpenTap(float duration)
  {
    if (this.HasFlag(BaseEntity.Flags.Reserved5))
      return;
    this.SetFlag(BaseEntity.Flags.Reserved5, true, false, true);
    this.Invoke(new Action(this.ShutTap), duration);
    this.SendNetworkUpdateImmediate(false);
  }

  public virtual void ShutTap()
  {
    this.SetFlag(BaseEntity.Flags.Reserved5, false, false, true);
    this.SendNetworkUpdateImmediate(false);
  }

  public bool HasLiquidItem()
  {
    return this.GetLiquidItem() != null;
  }

  public Item GetLiquidItem()
  {
    if (this.inventory.itemList.Count != 0)
      return this.inventory.itemList[0];
    return (Item) null;
  }

  public bool wantsSplash(ItemDefinition splashType, int amount)
  {
    if (!this.HasLiquidItem())
      return true;
    if (Object.op_Equality((Object) this.GetLiquidItem().info, (Object) splashType))
      return this.GetLiquidItem().amount < this.maxStackSize;
    return false;
  }

  public int DoSplash(ItemDefinition splashType, int amount)
  {
    int iAmount;
    if (this.HasLiquidItem())
    {
      Item liquidItem = this.GetLiquidItem();
      int amount1 = liquidItem.amount;
      ItemDefinition template = WaterResource.Merge(splashType, liquidItem.info);
      if (Object.op_Inequality((Object) liquidItem.info, (Object) template))
      {
        liquidItem.Remove(0.0f);
        liquidItem = ItemManager.Create(template, amount1, 0UL);
        if (!liquidItem.MoveToContainer(this.inventory, -1, true))
        {
          liquidItem.Remove(0.0f);
          return 0;
        }
      }
      iAmount = Mathf.Min(this.maxStackSize - amount1, amount);
      liquidItem.amount += iAmount;
    }
    else
    {
      iAmount = Mathf.Min(amount, this.maxStackSize);
      Item obj = ItemManager.Create(splashType, iAmount, 0UL);
      if (obj != null && !obj.MoveToContainer(this.inventory, -1, true))
        obj.Remove(0.0f);
    }
    return iAmount;
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  public void SVDrink(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.metabolism.CanConsume())
      return;
    foreach (Item obj in this.inventory.itemList)
    {
      ItemModConsume component = (ItemModConsume) ((Component) obj.info).GetComponent<ItemModConsume>();
      if (!Object.op_Equality((Object) component, (Object) null) && component.CanDoAction(obj, rpc.player))
      {
        component.DoAction(obj, rpc.player);
        break;
      }
    }
  }
}
