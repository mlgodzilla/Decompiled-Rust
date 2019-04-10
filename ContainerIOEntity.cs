// Decompiled with JetBrains decompiler
// Type: ContainerIOEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ContainerIOEntity : IOEntity
{
  public ItemContainer.ContentsType allowedContents = ItemContainer.ContentsType.Generic;
  public int maxStackSize = 1;
  public string lootPanelName = "generic";
  public bool isLootable = true;
  public ItemDefinition onlyAllowedItem;
  public int numSlots;
  public bool needsBuildingPrivilegeToUse;
  protected ItemContainer inventory;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("ContainerIOEntity.OnRpcMessage", 0.1f))
    {
      if (rpc == 331989034U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_OpenLoot "));
          using (TimeWarning.New("RPC_OpenLoot", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_OpenLoot", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_OpenLoot(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_OpenLoot");
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
    if (this.inventory == null)
    {
      this.CreateInventory(true);
      this.OnInventoryFirstCreated(this.inventory);
    }
    base.ServerInit();
  }

  public override void PreServerLoad()
  {
    base.PreServerLoad();
    this.CreateInventory(false);
  }

  public void CreateInventory(bool giveUID)
  {
    this.inventory = new ItemContainer();
    this.inventory.entityOwner = (BaseEntity) this;
    this.inventory.allowedContents = this.allowedContents == (ItemContainer.ContentsType) 0 ? ItemContainer.ContentsType.Generic : this.allowedContents;
    this.inventory.onlyAllowedItem = this.onlyAllowedItem;
    this.inventory.maxStackSize = this.maxStackSize;
    this.inventory.ServerInitialize((Item) null, this.numSlots);
    if (giveUID)
      this.inventory.GiveUID();
    this.inventory.onItemAddedRemoved = new Action<Item, bool>(this.OnItemAddedOrRemoved);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (!info.forDisk)
      return;
    if (this.inventory != null)
    {
      info.msg.storageBox = (__Null) Pool.Get<StorageBox>();
      ((StorageBox) info.msg.storageBox).contents = (__Null) this.inventory.Save();
    }
    else
      Debug.LogWarning((object) ("Storage container without inventory: " + ((object) this).ToString()));
  }

  public virtual void OnInventoryFirstCreated(ItemContainer container)
  {
  }

  public virtual void OnItemAddedOrRemoved(Item item, bool added)
  {
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  private void RPC_OpenLoot(BaseEntity.RPCMessage rpc)
  {
    if (this.inventory == null)
      return;
    BasePlayer player = rpc.player;
    if (!Object.op_Implicit((Object) player) || !player.CanInteract() || this.needsBuildingPrivilegeToUse && !player.CanBuild() || !player.inventory.loot.StartLootingEntity((BaseEntity) this, true))
      return;
    this.SetFlag(BaseEntity.Flags.Open, true, false, true);
    player.inventory.loot.AddContainer(this.inventory);
    player.inventory.loot.SendImmediate();
    player.ClientRPCPlayer<string>((Connection) null, player, "RPC_OpenLootPanel", this.lootPanelName);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public virtual void PlayerStoppedLooting(BasePlayer player)
  {
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (!info.fromDisk || info.msg.storageBox == null)
      return;
    if (this.inventory != null)
    {
      this.inventory.Load((ItemContainer) ((StorageBox) info.msg.storageBox).contents);
      this.inventory.capacity = this.numSlots;
    }
    else
      Debug.LogWarning((object) ("Storage container without inventory: " + ((object) this).ToString()));
  }
}
