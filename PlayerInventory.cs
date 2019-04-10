// Decompiled with JetBrains decompiler
// Type: PlayerInventory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Facepunch.Steamworks;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerInventory : EntityComponent<BasePlayer>
{
  public ItemContainer containerMain;
  public ItemContainer containerBelt;
  public ItemContainer containerWear;
  public ItemCrafter crafting;
  public PlayerLoot loot;
  [ServerVar]
  public static bool forceBirthday;
  private static float nextCheckTime;
  private static bool wasBirthday;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("PlayerInventory.OnRpcMessage", 0.1f))
    {
      if (rpc == 3482449460U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ItemCmd "));
        using (TimeWarning.New("ItemCmd", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.FromOwner.Test("ItemCmd", this.GetBaseEntity(), player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.ItemCmd(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in ItemCmd");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3041092525U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - MoveItem "));
          using (TimeWarning.New("MoveItem", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.FromOwner.Test("MoveItem", this.GetBaseEntity(), player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.MoveItem(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in MoveItem");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  protected void Initialize()
  {
    this.containerMain = new ItemContainer();
    this.containerMain.SetFlag(ItemContainer.Flag.IsPlayer, true);
    this.containerBelt = new ItemContainer();
    this.containerBelt.SetFlag(ItemContainer.Flag.IsPlayer, true);
    this.containerBelt.SetFlag(ItemContainer.Flag.Belt, true);
    this.containerWear = new ItemContainer();
    this.containerWear.SetFlag(ItemContainer.Flag.IsPlayer, true);
    this.containerWear.SetFlag(ItemContainer.Flag.Clothing, true);
    this.crafting = (ItemCrafter) ((Component) this).GetComponent<ItemCrafter>();
    this.crafting.AddContainer(this.containerMain);
    this.crafting.AddContainer(this.containerBelt);
    this.loot = (PlayerLoot) ((Component) this).GetComponent<PlayerLoot>();
    if (Object.op_Implicit((Object) this.loot))
      return;
    this.loot = (PlayerLoot) ((Component) this).get_gameObject().AddComponent<PlayerLoot>();
  }

  public void DoDestroy()
  {
    if (this.containerMain != null)
    {
      this.containerMain.Kill();
      this.containerMain = (ItemContainer) null;
    }
    if (this.containerBelt != null)
    {
      this.containerBelt.Kill();
      this.containerBelt = (ItemContainer) null;
    }
    if (this.containerWear == null)
      return;
    this.containerWear.Kill();
    this.containerWear = (ItemContainer) null;
  }

  public void ServerInit(BasePlayer owner)
  {
    this.Initialize();
    this.containerMain.ServerInitialize((Item) null, 24);
    if (this.containerMain.uid == 0U)
      this.containerMain.GiveUID();
    this.containerBelt.ServerInitialize((Item) null, 6);
    if (this.containerBelt.uid == 0U)
      this.containerBelt.GiveUID();
    this.containerWear.ServerInitialize((Item) null, 7);
    if (this.containerWear.uid == 0U)
      this.containerWear.GiveUID();
    this.containerMain.playerOwner = owner;
    this.containerBelt.playerOwner = owner;
    this.containerWear.playerOwner = owner;
    this.containerWear.onItemAddedRemoved = new Action<Item, bool>(this.OnClothingChanged);
    this.containerWear.canAcceptItem = new Func<Item, int, bool>(this.CanWearItem);
    this.containerBelt.canAcceptItem = new Func<Item, int, bool>(this.CanEquipItem);
    this.containerMain.onPreItemRemove = new Action<Item>(this.OnItemRemoved);
    this.containerWear.onPreItemRemove = new Action<Item>(this.OnItemRemoved);
    this.containerBelt.onPreItemRemove = new Action<Item>(this.OnItemRemoved);
    this.containerMain.onDirty += new Action(this.OnContentsDirty);
    this.containerBelt.onDirty += new Action(this.OnContentsDirty);
    this.containerWear.onDirty += new Action(this.OnContentsDirty);
    this.containerBelt.onItemAddedRemoved = new Action<Item, bool>(this.OnItemAddedOrRemoved);
    this.containerMain.onItemAddedRemoved = new Action<Item, bool>(this.OnItemAddedOrRemoved);
  }

  public void OnItemAddedOrRemoved(Item item, bool bAdded)
  {
    if (item.info.isHoldable)
      this.Invoke(new Action(this.UpdatedVisibleHolsteredItems), 0.1f);
    if (!bAdded)
      return;
    BasePlayer baseEntity = this.baseEntity;
    if (baseEntity.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash) || !baseEntity.IsHostileItem(item))
      return;
    this.baseEntity.SetPlayerFlag(BasePlayer.PlayerFlags.DisplaySash, true);
  }

  public void UpdatedVisibleHolsteredItems()
  {
    List<HeldEntity> list1 = (List<HeldEntity>) Pool.GetList<HeldEntity>();
    List<Item> list2 = (List<Item>) Pool.GetList<Item>();
    this.AllItemsNoAlloc(ref list2);
    foreach (Item obj in list2)
    {
      if (obj.info.isHoldable && !Object.op_Equality((Object) obj.GetHeldEntity(), (Object) null))
      {
        HeldEntity component = (HeldEntity) ((Component) obj.GetHeldEntity()).GetComponent<HeldEntity>();
        if (!Object.op_Equality((Object) component, (Object) null))
          list1.Add(component);
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Item>((List<M0>&) ref list2);
    IOrderedEnumerable<HeldEntity> orderedEnumerable = list1.OrderByDescending<HeldEntity, float>((Func<HeldEntity, float>) (x => x.hostileScore));
    bool flag1 = true;
    bool flag2 = true;
    bool flag3 = true;
    foreach (HeldEntity heldEntity in (IEnumerable<HeldEntity>) orderedEnumerable)
    {
      if (!Object.op_Equality((Object) heldEntity, (Object) null) && heldEntity.holsterInfo.displayWhenHolstered)
      {
        if (flag3 && !heldEntity.IsDeployed() && heldEntity.holsterInfo.slot == HeldEntity.HolsterInfo.HolsterSlot.BACK)
        {
          heldEntity.SetVisibleWhileHolstered(true);
          flag3 = false;
        }
        else if (flag2 && !heldEntity.IsDeployed() && heldEntity.holsterInfo.slot == HeldEntity.HolsterInfo.HolsterSlot.RIGHT_THIGH)
        {
          heldEntity.SetVisibleWhileHolstered(true);
          flag2 = false;
        }
        else if (flag1 && !heldEntity.IsDeployed() && heldEntity.holsterInfo.slot == HeldEntity.HolsterInfo.HolsterSlot.LEFT_THIGH)
        {
          heldEntity.SetVisibleWhileHolstered(true);
          flag1 = false;
        }
        else
          heldEntity.SetVisibleWhileHolstered(false);
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<HeldEntity>((List<M0>&) ref list1);
  }

  private void OnContentsDirty()
  {
    if (!Object.op_Inequality((Object) this.baseEntity, (Object) null))
      return;
    this.baseEntity.InvalidateNetworkCache();
  }

  private bool CanMoveItemsFrom(BaseEntity entity, Item item)
  {
    StorageContainer storageContainer = entity as StorageContainer;
    return !Object.op_Implicit((Object) storageContainer) || storageContainer.CanMoveFrom(this.baseEntity, item);
  }

  [BaseEntity.RPC_Server.FromOwner]
  [BaseEntity.RPC_Server]
  private void ItemCmd(BaseEntity.RPCMessage msg)
  {
    uint id = msg.read.UInt32();
    string command = msg.read.String();
    Item itemUid = this.FindItemUID(id);
    if (itemUid == null || Interface.CallHook("OnItemAction", (object) itemUid, (object) command, (object) msg.player) != null || (itemUid.IsLocked() || !this.CanMoveItemsFrom(itemUid.parent.entityOwner, itemUid)))
      return;
    if (command == "drop")
    {
      int split_Amount = itemUid.amount;
      if (msg.read.get_unread() >= 4)
        split_Amount = msg.read.Int32();
      this.baseEntity.stats.Add("item_drop", 1, Stats.Steam);
      if (split_Amount < itemUid.amount)
        itemUid.SplitItem(split_Amount)?.Drop(this.baseEntity.GetDropPosition(), this.baseEntity.GetDropVelocity(), (Quaternion) null);
      else
        itemUid.Drop(this.baseEntity.GetDropPosition(), this.baseEntity.GetDropVelocity(), (Quaternion) null);
      this.baseEntity.SignalBroadcast(BaseEntity.Signal.Gesture, "drop_item", (Connection) null);
    }
    else
    {
      itemUid.ServerCommand(command, this.baseEntity);
      ItemManager.DoRemoves();
      this.ServerUpdate(0.0f);
    }
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.FromOwner]
  private void MoveItem(BaseEntity.RPCMessage msg)
  {
    uint id1 = msg.read.UInt32();
    uint id2 = msg.read.UInt32();
    int iTargetPos = (int) msg.read.Int8();
    int num = (int) msg.read.UInt16();
    Item itemUid = this.FindItemUID(id1);
    if (itemUid == null)
    {
      msg.player.ChatMessage("Invalid item (" + (object) id1 + ")");
    }
    else
    {
      if (Interface.CallHook("CanMoveItem", (object) itemUid, (object) this, (object) id2, (object) iTargetPos, (object) num) != null)
        return;
      if (!this.CanMoveItemsFrom(itemUid.parent.entityOwner, itemUid))
      {
        msg.player.ChatMessage("Cannot move item!");
      }
      else
      {
        if (num <= 0)
          num = itemUid.amount;
        int split_Amount = Mathf.Clamp(num, 1, itemUid.info.stackable);
        if (id2 == 0U)
        {
          if (this.GiveItem(itemUid, (ItemContainer) null))
            return;
          msg.player.ChatMessage("GiveItem failed!");
        }
        else
        {
          ItemContainer container = this.FindContainer(id2);
          if (container == null)
          {
            msg.player.ChatMessage("Invalid container (" + (object) id2 + ")");
          }
          else
          {
            ItemContainer parent = itemUid.parent;
            if (parent != null && parent.IsLocked() || container.IsLocked())
              msg.player.ChatMessage("Container is locked!");
            else if (container.PlayerItemInputBlocked())
            {
              msg.player.ChatMessage("Container does not accept player items!");
            }
            else
            {
              using (TimeWarning.New("Split", 0.1f))
              {
                if (itemUid.amount > split_Amount)
                {
                  Item obj = itemUid.SplitItem(split_Amount);
                  if (!obj.MoveToContainer(container, iTargetPos, true))
                  {
                    itemUid.amount += obj.amount;
                    obj.Remove(0.0f);
                  }
                  ItemManager.DoRemoves();
                  this.ServerUpdate(0.0f);
                  return;
                }
              }
              if (!itemUid.MoveToContainer(container, iTargetPos, true))
                return;
              ItemManager.DoRemoves();
              this.ServerUpdate(0.0f);
            }
          }
        }
      }
    }
  }

  private void OnClothingChanged(Item item, bool bAdded)
  {
    this.baseEntity.SV_ClothingChanged();
    ItemManager.DoRemoves();
    this.ServerUpdate(0.0f);
  }

  private void OnItemRemoved(Item item)
  {
    this.baseEntity.InvalidateNetworkCache();
  }

  private bool CanEquipItem(Item item, int targetSlot)
  {
    object obj1 = Interface.CallHook(nameof (CanEquipItem), (object) this, (object) item, (object) targetSlot);
    if (obj1 is bool)
      return (bool) obj1;
    ItemModContainerRestriction component1 = (ItemModContainerRestriction) ((Component) item.info).GetComponent<ItemModContainerRestriction>();
    if (Object.op_Equality((Object) component1, (Object) null))
      return true;
    foreach (Item obj2 in this.containerBelt.itemList.ToArray())
    {
      if (obj2 != item)
      {
        ItemModContainerRestriction component2 = (ItemModContainerRestriction) ((Component) obj2.info).GetComponent<ItemModContainerRestriction>();
        if (!Object.op_Equality((Object) component2, (Object) null) && !component1.CanExistWith(component2) && !obj2.MoveToContainer(this.containerMain, -1, true))
          obj2.Drop(this.baseEntity.GetDropPosition(), this.baseEntity.GetDropVelocity(), (Quaternion) null);
      }
    }
    return true;
  }

  private bool CanWearItem(Item item, int targetSlot)
  {
    ItemModWearable component1 = (ItemModWearable) ((Component) item.info).GetComponent<ItemModWearable>();
    if (Object.op_Equality((Object) component1, (Object) null))
      return false;
    object obj1 = Interface.CallHook(nameof (CanWearItem), (object) this, (object) item, (object) targetSlot);
    if (obj1 is bool)
      return (bool) obj1;
    foreach (Item obj2 in this.containerWear.itemList.ToArray())
    {
      if (obj2 != item)
      {
        ItemModWearable component2 = (ItemModWearable) ((Component) obj2.info).GetComponent<ItemModWearable>();
        if (!Object.op_Equality((Object) component2, (Object) null) && !component1.CanExistWith(component2) && !obj2.MoveToContainer(this.containerMain, -1, true))
          obj2.Drop(this.baseEntity.GetDropPosition(), this.baseEntity.GetDropVelocity(), (Quaternion) null);
      }
    }
    return true;
  }

  public void ServerUpdate(float delta)
  {
    this.loot.Check();
    if ((double) delta > 0.0)
      this.crafting.ServerUpdate(delta);
    float currentTemperature = this.baseEntity.currentTemperature;
    this.UpdateContainer(delta, PlayerInventory.Type.Main, this.containerMain, false, currentTemperature);
    this.UpdateContainer(delta, PlayerInventory.Type.Belt, this.containerBelt, true, currentTemperature);
    this.UpdateContainer(delta, PlayerInventory.Type.Wear, this.containerWear, true, currentTemperature);
  }

  public void UpdateContainer(
    float delta,
    PlayerInventory.Type type,
    ItemContainer container,
    bool bSendInventoryToEveryone,
    float temperature)
  {
    if (container == null)
      return;
    container.temperature = temperature;
    if ((double) delta > 0.0)
      container.OnCycle(delta);
    if (!container.dirty)
      return;
    this.SendUpdatedInventory(type, container, bSendInventoryToEveryone);
    this.baseEntity.InvalidateNetworkCache();
  }

  public void SendSnapshot()
  {
    using (TimeWarning.New("PlayerInventory.SendSnapshot", 0.1f))
    {
      this.SendUpdatedInventory(PlayerInventory.Type.Main, this.containerMain, false);
      this.SendUpdatedInventory(PlayerInventory.Type.Belt, this.containerBelt, true);
      this.SendUpdatedInventory(PlayerInventory.Type.Wear, this.containerWear, true);
    }
  }

  public void SendUpdatedInventory(
    PlayerInventory.Type type,
    ItemContainer container,
    bool bSendInventoryToEveryone = false)
  {
    using (UpdateItemContainer updateItemContainer = (UpdateItemContainer) Pool.Get<UpdateItemContainer>())
    {
      updateItemContainer.type = (__Null) type;
      if (container != null)
      {
        container.dirty = false;
        updateItemContainer.container = (__Null) Pool.Get<List<ItemContainer>>();
        ((List<ItemContainer>) updateItemContainer.container).Add(container.Save());
      }
      if (bSendInventoryToEveryone)
        this.baseEntity.ClientRPC<UpdateItemContainer>((Connection) null, "UpdatedItemContainer", updateItemContainer);
      else
        this.baseEntity.ClientRPCPlayer<UpdateItemContainer>((Connection) null, this.baseEntity, "UpdatedItemContainer", updateItemContainer);
    }
  }

  public Item FindItemUID(uint id)
  {
    if (id == 0U)
      return (Item) null;
    if (this.containerMain != null)
    {
      Item itemByUid = this.containerMain.FindItemByUID(id);
      if (itemByUid != null && itemByUid.IsValid())
        return itemByUid;
    }
    if (this.containerBelt != null)
    {
      Item itemByUid = this.containerBelt.FindItemByUID(id);
      if (itemByUid != null && itemByUid.IsValid())
        return itemByUid;
    }
    if (this.containerWear != null)
    {
      Item itemByUid = this.containerWear.FindItemByUID(id);
      if (itemByUid != null && itemByUid.IsValid())
        return itemByUid;
    }
    return this.loot.FindItem(id);
  }

  public Item FindItemID(string itemName)
  {
    ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemName);
    if (Object.op_Equality((Object) itemDefinition, (Object) null))
      return (Item) null;
    return this.FindItemID(itemDefinition.itemid);
  }

  public Item FindItemID(int id)
  {
    if (this.containerMain != null)
    {
      Item itemByItemId = this.containerMain.FindItemByItemID(id);
      if (itemByItemId != null && itemByItemId.IsValid())
        return itemByItemId;
    }
    if (this.containerBelt != null)
    {
      Item itemByItemId = this.containerBelt.FindItemByItemID(id);
      if (itemByItemId != null && itemByItemId.IsValid())
        return itemByItemId;
    }
    if (this.containerWear != null)
    {
      Item itemByItemId = this.containerWear.FindItemByItemID(id);
      if (itemByItemId != null && itemByItemId.IsValid())
        return itemByItemId;
    }
    return (Item) null;
  }

  public List<Item> FindItemIDs(int id)
  {
    List<Item> objList = new List<Item>();
    if (this.containerMain != null)
      objList.AddRange((IEnumerable<Item>) this.containerMain.FindItemsByItemID(id));
    if (this.containerBelt != null)
      objList.AddRange((IEnumerable<Item>) this.containerBelt.FindItemsByItemID(id));
    if (this.containerWear != null)
      objList.AddRange((IEnumerable<Item>) this.containerWear.FindItemsByItemID(id));
    return objList;
  }

  public ItemContainer FindContainer(uint id)
  {
    using (TimeWarning.New(nameof (FindContainer), 0.1f))
      return this.containerMain.FindContainer(id) ?? this.containerBelt.FindContainer(id) ?? this.containerWear.FindContainer(id) ?? this.loot.FindContainer(id);
  }

  public ItemContainer GetContainer(PlayerInventory.Type id)
  {
    if (id == PlayerInventory.Type.Main)
      return this.containerMain;
    if (PlayerInventory.Type.Belt == id)
      return this.containerBelt;
    if (PlayerInventory.Type.Wear == id)
      return this.containerWear;
    return (ItemContainer) null;
  }

  public bool GiveItem(Item item, ItemContainer container = null)
  {
    if (item == null)
      return false;
    int position = -1;
    this.GetIdealPickupContainer(item, ref container, ref position);
    return container != null && item.MoveToContainer(container, position, true) || (item.MoveToContainer(this.containerMain, -1, true) || item.MoveToContainer(this.containerBelt, -1, true));
  }

  protected void GetIdealPickupContainer(Item item, ref ItemContainer container, ref int position)
  {
    if (item.info.stackable > 1)
    {
      if (this.containerBelt != null && this.containerBelt.FindItemByItemID(item.info.itemid) != null)
      {
        container = this.containerBelt;
        return;
      }
      if (this.containerMain != null && this.containerMain.FindItemByItemID(item.info.itemid) != null)
      {
        container = this.containerMain;
        return;
      }
    }
    if (!item.info.isUsable || item.info.HasFlag(ItemDefinition.Flag.NotStraightToBelt))
      return;
    container = this.containerBelt;
  }

  public void Strip()
  {
    this.containerMain.Clear();
    this.containerBelt.Clear();
    this.containerWear.Clear();
    ItemManager.DoRemoves();
  }

  public static bool IsBirthday()
  {
    if (PlayerInventory.forceBirthday)
      return true;
    if ((double) Time.get_time() < (double) PlayerInventory.nextCheckTime)
      return PlayerInventory.wasBirthday;
    PlayerInventory.nextCheckTime = Time.get_time() + 60f;
    DateTime now = DateTime.Now;
    PlayerInventory.wasBirthday = now.Day == 11 && now.Month == 12;
    return PlayerInventory.wasBirthday;
  }

  public static bool IsChristmas()
  {
    return XMas.enabled;
  }

  public void GiveDefaultItems()
  {
    this.Strip();
    ulong skin = 0;
    int infoInt = this.baseEntity.GetInfoInt("client.rockskin", 0);
    if (infoInt > 0 && this.baseEntity.blueprints.steamInventory.HasItem(infoInt))
    {
      Inventory.Definition definition = ((BaseSteamworks) Global.get_SteamServer()).get_Inventory().FindDefinition(infoInt);
      if (definition != null)
        skin = (ulong) definition.GetProperty<ulong>("workshopdownload");
    }
    this.GiveItem(ItemManager.CreateByName("rock", 1, skin), this.containerBelt);
    this.GiveItem(ItemManager.CreateByName("torch", 1, 0UL), this.containerBelt);
    if (PlayerInventory.IsBirthday())
    {
      this.GiveItem(ItemManager.CreateByName("cakefiveyear", 1, 0UL), this.containerBelt);
      this.GiveItem(ItemManager.CreateByName("partyhat", 1, 0UL), this.containerWear);
    }
    if (!PlayerInventory.IsChristmas())
      return;
    this.GiveItem(ItemManager.CreateByName("snowball", 1, 0UL), this.containerBelt);
    this.GiveItem(ItemManager.CreateByName("snowball", 1, 0UL), this.containerBelt);
    this.GiveItem(ItemManager.CreateByName("snowball", 1, 0UL), this.containerBelt);
  }

  public PlayerInventory Save(bool bForDisk)
  {
    PlayerInventory playerInventory = (PlayerInventory) Pool.Get<PlayerInventory>();
    if (bForDisk)
      playerInventory.invMain = (__Null) this.containerMain.Save();
    playerInventory.invBelt = (__Null) this.containerBelt.Save();
    playerInventory.invWear = (__Null) this.containerWear.Save();
    return playerInventory;
  }

  public void Load(PlayerInventory msg)
  {
    if (msg.invMain != null)
      this.containerMain.Load((ItemContainer) msg.invMain);
    if (msg.invBelt != null)
      this.containerBelt.Load((ItemContainer) msg.invBelt);
    if (msg.invWear == null)
      return;
    this.containerWear.Load((ItemContainer) msg.invWear);
  }

  public int Take(List<Item> collect, int itemid, int amount)
  {
    int num = 0;
    if (this.containerMain != null)
      num += this.containerMain.Take(collect, itemid, amount);
    if (amount == num)
      return num;
    if (this.containerBelt != null)
      num += this.containerBelt.Take(collect, itemid, amount);
    if (amount == num || this.containerWear == null)
      return num;
    num += this.containerWear.Take(collect, itemid, amount);
    return num;
  }

  public int GetAmount(int itemid)
  {
    if (itemid == 0)
      return 0;
    int num = 0;
    if (this.containerMain != null)
      num += this.containerMain.GetAmount(itemid, true);
    if (this.containerBelt != null)
      num += this.containerBelt.GetAmount(itemid, true);
    if (this.containerWear != null)
      num += this.containerWear.GetAmount(itemid, true);
    return num;
  }

  public Item[] AllItems()
  {
    List<Item> objList = new List<Item>();
    if (this.containerMain != null)
      objList.AddRange((IEnumerable<Item>) this.containerMain.itemList);
    if (this.containerBelt != null)
      objList.AddRange((IEnumerable<Item>) this.containerBelt.itemList);
    if (this.containerWear != null)
      objList.AddRange((IEnumerable<Item>) this.containerWear.itemList);
    return objList.ToArray();
  }

  public int AllItemsNoAlloc(ref List<Item> items)
  {
    items.Clear();
    if (this.containerMain != null)
      items.AddRange((IEnumerable<Item>) this.containerMain.itemList);
    if (this.containerBelt != null)
      items.AddRange((IEnumerable<Item>) this.containerBelt.itemList);
    if (this.containerWear != null)
      items.AddRange((IEnumerable<Item>) this.containerWear.itemList);
    return items.Count;
  }

  public void FindAmmo(List<Item> list, AmmoTypes ammoType)
  {
    if (this.containerMain != null)
      this.containerMain.FindAmmo(list, ammoType);
    if (this.containerBelt == null)
      return;
    this.containerBelt.FindAmmo(list, ammoType);
  }

  public bool HasAmmo(AmmoTypes ammoType)
  {
    if (!this.containerMain.HasAmmo(ammoType))
      return this.containerBelt.HasAmmo(ammoType);
    return true;
  }

  public enum Type
  {
    Main,
    Belt,
    Wear,
  }
}
