// Decompiled with JetBrains decompiler
// Type: StorageContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class StorageContainer : DecayEntity
{
  public int inventorySlots = 6;
  public float dropChance = 0.75f;
  public bool isLootable = true;
  public bool isLockable = true;
  public string panelName = "generic";
  public Vector3 dropVelocity = Vector3.get_forward();
  public ItemCategory onlyAcceptCategory = ItemCategory.All;
  public ItemContainer.ContentsType allowedContents;
  public ItemDefinition allowedItem;
  public int maxStackSize;
  public bool needsBuildingPrivilegeToUse;
  public SoundDefinition openSound;
  public SoundDefinition closeSound;
  [Header("Item Dropping")]
  public Vector3 dropPosition;
  public bool onlyOneUser;
  [NonSerialized]
  public ItemContainer inventory;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("StorageContainer.OnRpcMessage", 0.1f))
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

  public void OnDrawGizmos()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(Color.get_yellow());
    Gizmos.DrawCube(this.dropPosition, Vector3.op_Multiply(Vector3.get_one(), 0.1f));
    Gizmos.set_color(Color.get_white());
    Gizmos.DrawRay(this.dropPosition, this.dropVelocity);
  }

  public void MoveAllInventoryItems(ItemContainer source, ItemContainer dest)
  {
    for (int slot = 0; slot < Mathf.Min(source.capacity, dest.capacity); ++slot)
      source.GetSlot(slot)?.MoveToContainer(dest, -1, true);
  }

  public virtual void ReceiveInventoryFromItem(Item item)
  {
    if (item.contents == null)
      return;
    this.MoveAllInventoryItems(item.contents, this.inventory);
  }

  public override bool CanPickup(BasePlayer player)
  {
    bool flag = Object.op_Inequality((Object) this.GetSlot(BaseEntity.Slot.Lock), (Object) null);
    if (this.isClient)
    {
      if (base.CanPickup(player))
        return !flag;
      return false;
    }
    if ((!this.pickup.requireEmptyInv || this.inventory == null || this.inventory.itemList.Count == 0) && base.CanPickup(player))
      return !flag;
    return false;
  }

  public override void OnPickedUp(Item createdItem, BasePlayer player)
  {
    base.OnPickedUp(createdItem, player);
    if (createdItem != null && createdItem.contents != null)
    {
      this.MoveAllInventoryItems(this.inventory, createdItem.contents);
    }
    else
    {
      for (int slot1 = 0; slot1 < this.inventory.capacity; ++slot1)
      {
        Item slot2 = this.inventory.GetSlot(slot1);
        if (slot2 != null)
        {
          slot2.RemoveFromContainer();
          player.GiveItem(slot2, BaseEntity.GiveItemReason.PickedUp);
        }
      }
    }
  }

  public override void ResetState()
  {
    base.ResetState();
    if (this.inventory != null)
      this.inventory.Clear();
    this.inventory = (ItemContainer) null;
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

  public virtual void OnInventoryFirstCreated(ItemContainer container)
  {
  }

  public virtual void OnItemAddedOrRemoved(Item item, bool added)
  {
  }

  public virtual bool ItemFilter(Item item, int targetSlot)
  {
    if (this.onlyAcceptCategory == ItemCategory.All)
      return true;
    return item.info.category == this.onlyAcceptCategory;
  }

  public void CreateInventory(bool giveUID)
  {
    this.inventory = new ItemContainer();
    this.inventory.entityOwner = (BaseEntity) this;
    this.inventory.allowedContents = this.allowedContents == (ItemContainer.ContentsType) 0 ? ItemContainer.ContentsType.Generic : this.allowedContents;
    this.inventory.onlyAllowedItem = this.allowedItem;
    this.inventory.maxStackSize = this.maxStackSize;
    this.inventory.ServerInitialize((Item) null, this.inventorySlots);
    if (giveUID)
      this.inventory.GiveUID();
    this.inventory.onDirty += new Action(this.OnInventoryDirty);
    this.inventory.onItemAddedRemoved = new Action<Item, bool>(this.OnItemAddedOrRemoved);
    if (this.onlyAcceptCategory == ItemCategory.All)
      return;
    this.inventory.canAcceptItem = new Func<Item, int, bool>(this.ItemFilter);
  }

  public override void PreServerLoad()
  {
    base.PreServerLoad();
    this.CreateInventory(false);
  }

  protected virtual void OnInventoryDirty()
  {
    this.InvalidateNetworkCache();
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    if (this.inventory != null && this.inventory.uid == 0U)
      this.inventory.GiveUID();
    this.SetFlag(BaseEntity.Flags.Open, false, false, true);
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    if (this.inventory == null)
      return;
    this.inventory.Kill();
    this.inventory = (ItemContainer) null;
  }

  public override bool HasSlot(BaseEntity.Slot slot)
  {
    if (this.isLockable && slot == BaseEntity.Slot.Lock)
      return true;
    return base.HasSlot(slot);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  private void RPC_OpenLoot(BaseEntity.RPCMessage rpc)
  {
    if (!this.isLootable)
      return;
    this.PlayerOpenLoot(rpc.player);
  }

  public virtual string GetPanelName()
  {
    return this.panelName;
  }

  public virtual bool CanMoveFrom(BasePlayer player, Item item)
  {
    return !this.inventory.IsLocked();
  }

  public virtual bool CanOpenLootPanel(BasePlayer player, string panelName = "")
  {
    if (this.needsBuildingPrivilegeToUse && !player.CanBuild())
      return false;
    BaseLock slot = this.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
    if (!Object.op_Inequality((Object) slot, (Object) null) || slot.OnTryToOpen(player))
      return true;
    player.ChatMessage("It is locked...");
    return false;
  }

  public virtual bool PlayerOpenLoot(BasePlayer player)
  {
    return this.PlayerOpenLoot(player, this.panelName);
  }

  public virtual bool PlayerOpenLoot(BasePlayer player, string panelToOpen)
  {
    object obj = Interface.CallHook("CanLootEntity", (object) player, (object) this);
    if (obj is bool)
      return (bool) obj;
    if (this.IsLocked())
    {
      player.ChatMessage("Can't loot right now");
      return false;
    }
    if (this.onlyOneUser && this.IsOpen())
    {
      player.ChatMessage("Already in use");
      return false;
    }
    if (!this.CanOpenLootPanel(player, panelToOpen) || !player.inventory.loot.StartLootingEntity((BaseEntity) this, true))
      return false;
    this.SetFlag(BaseEntity.Flags.Open, true, false, true);
    player.inventory.loot.AddContainer(this.inventory);
    player.inventory.loot.SendImmediate();
    player.ClientRPCPlayer<string>((Connection) null, player, "RPC_OpenLootPanel", panelToOpen);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    return true;
  }

  public virtual void PlayerStoppedLooting(BasePlayer player)
  {
    Interface.CallHook("OnLootEntityEnd", (object) player, (object) this);
    this.SetFlag(BaseEntity.Flags.Open, false, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
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

  public override void OnKilled(HitInfo info)
  {
    this.DropItems();
    base.OnKilled(info);
  }

  public void DropItems()
  {
    if (this.inventory == null || this.inventory.itemList == null || (this.inventory.itemList.Count == 0 || (double) this.dropChance == 0.0))
      return;
    if (this.ShouldDropItemsIndividually() || this.inventory.itemList.Count == 1)
      DropUtil.DropItems(this.inventory, this.GetDropPosition(), 1f);
    else
      Object.op_Inequality((Object) this.inventory.Drop("assets/prefabs/misc/item drop/item_drop.prefab", this.GetDropPosition(), ((Component) this).get_transform().get_rotation()), (Object) null);
  }

  public override Vector3 GetDropPosition()
  {
    Matrix4x4 localToWorldMatrix = ((Component) this).get_transform().get_localToWorldMatrix();
    return ((Matrix4x4) ref localToWorldMatrix).MultiplyPoint(this.dropPosition);
  }

  public override Vector3 GetDropVelocity()
  {
    Vector3 inheritedDropVelocity = this.GetInheritedDropVelocity();
    Matrix4x4 localToWorldMatrix = ((Component) this).get_transform().get_localToWorldMatrix();
    Vector3 vector3 = ((Matrix4x4) ref localToWorldMatrix).MultiplyVector(this.dropPosition);
    return Vector3.op_Addition(inheritedDropVelocity, vector3);
  }

  public virtual bool ShouldDropItemsIndividually()
  {
    return false;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.storageBox == null)
      return;
    if (this.inventory != null)
    {
      this.inventory.Load((ItemContainer) ((StorageBox) info.msg.storageBox).contents);
      this.inventory.capacity = this.inventorySlots;
    }
    else
      Debug.LogWarning((object) ("Storage container without inventory: " + ((object) this).ToString()));
  }

  public bool OccupiedCheck(BasePlayer player = null)
  {
    if (Object.op_Inequality((Object) player, (Object) null) && Object.op_Equality((Object) player.inventory.loot.entitySource, (Object) this) || !this.onlyOneUser)
      return true;
    return !this.IsOpen();
  }
}
