// Decompiled with JetBrains decompiler
// Type: Locker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Locker : StorageContainer
{
  private int rowSize = 7;
  private int beltSize = 6;
  private int columnSize = 2;
  private Item[] clothingBuffer = new Item[7];
  public GameObjectRef equipSound;
  public bool equippingActive;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Locker.OnRpcMessage", 0.1f))
    {
      if (rpc == 1799659668U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Equip "));
          using (TimeWarning.New("RPC_Equip", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Equip", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Equip(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Equip");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool IsEquipping()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved1);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.inventory.canAcceptItem += new Func<Item, int, bool>(this.LockerItemFilter);
    this.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
  }

  public bool LockerItemFilter(Item item, int targetSlot)
  {
    return this.equippingActive;
  }

  public void ClearEquipping()
  {
    this.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void RPC_Equip(BaseEntity.RPCMessage msg)
  {
    int num1 = msg.read.Int32();
    switch (num1)
    {
      case 0:
      case 1:
      case 2:
        if (this.IsEquipping())
          break;
        BasePlayer player = msg.player;
        int num2 = this.rowSize * this.columnSize;
        int num3 = num1 * num2;
        this.equippingActive = true;
        bool flag = false;
        for (int slot1 = 0; slot1 < player.inventory.containerWear.capacity; ++slot1)
        {
          Item slot2 = player.inventory.containerWear.GetSlot(slot1);
          if (slot2 != null)
          {
            slot2.RemoveFromContainer();
            this.clothingBuffer[slot1] = slot2;
          }
        }
        for (int index = 0; index < this.rowSize; ++index)
        {
          int num4 = num3 + index;
          int iTargetPos = index;
          Item slot = this.inventory.GetSlot(num4);
          Item obj = this.clothingBuffer[index];
          if (slot != null)
          {
            flag = true;
            if (slot.info.category != ItemCategory.Attire || !slot.MoveToContainer(player.inventory.containerWear, iTargetPos, true))
              slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), (Quaternion) null);
          }
          if (obj != null)
          {
            flag = true;
            if (obj.info.category != ItemCategory.Attire || !obj.MoveToContainer(this.inventory, num4, true))
              obj.Drop(this.GetDropPosition(), this.GetDropVelocity(), (Quaternion) null);
          }
          this.clothingBuffer[index] = (Item) null;
        }
        for (int slot1 = 0; slot1 < this.beltSize; ++slot1)
        {
          int num4 = num3 + slot1 + this.rowSize;
          int iTargetPos = slot1;
          Item slot2 = this.inventory.GetSlot(num4);
          Item slot3 = player.inventory.containerBelt.GetSlot(slot1);
          slot3?.RemoveFromContainer();
          if (slot2 != null)
          {
            flag = true;
            if (!slot2.MoveToContainer(player.inventory.containerBelt, iTargetPos, true))
              slot2.Drop(this.GetDropPosition(), this.GetDropVelocity(), (Quaternion) null);
          }
          if (slot3 != null)
          {
            flag = true;
            if (!slot3.MoveToContainer(this.inventory, num4, true))
              slot3.Drop(this.GetDropPosition(), this.GetDropVelocity(), (Quaternion) null);
          }
        }
        this.equippingActive = false;
        if (!flag)
          break;
        Effect.server.Run(this.equipSound.resourcePath, (BaseEntity) player, StringPool.Get("spine3"), Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
        this.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
        this.Invoke(new Action(this.ClearEquipping), 1.5f);
        break;
    }
  }

  public static class LockerFlags
  {
    public const BaseEntity.Flags IsEquipping = BaseEntity.Flags.Reserved1;
  }
}
