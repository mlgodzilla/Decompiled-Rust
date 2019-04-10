// Decompiled with JetBrains decompiler
// Type: ItemContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public sealed class ItemContainer
{
  public List<ItemSlot> availableSlots = new List<ItemSlot>();
  public int capacity = 2;
  public List<Item> itemList = new List<Item>();
  public float temperature = 15f;
  public ItemContainer.Flag flags;
  public ItemContainer.ContentsType allowedContents;
  public ItemDefinition onlyAllowedItem;
  public uint uid;
  public bool dirty;
  public Item parent;
  public BasePlayer playerOwner;
  public BaseEntity entityOwner;
  public bool isServer;
  public int maxStackSize;
  public Func<Item, int, bool> canAcceptItem;
  public Action<Item, bool> onItemAddedRemoved;
  public Action<Item> onPreItemRemove;

  public bool HasFlag(ItemContainer.Flag f)
  {
    return (this.flags & f) == f;
  }

  public void SetFlag(ItemContainer.Flag f, bool b)
  {
    if (b)
      this.flags |= f;
    else
      this.flags &= ~f;
  }

  public bool IsLocked()
  {
    return this.HasFlag(ItemContainer.Flag.IsLocked);
  }

  public bool PlayerItemInputBlocked()
  {
    return this.HasFlag(ItemContainer.Flag.NoItemInput);
  }

  public event Action onDirty;

  public void ServerInitialize(Item parentItem, int iMaxCapacity)
  {
    this.parent = parentItem;
    this.capacity = iMaxCapacity;
    this.uid = 0U;
    this.isServer = true;
    if (this.allowedContents == (ItemContainer.ContentsType) 0)
      this.allowedContents = ItemContainer.ContentsType.Generic;
    this.MarkDirty();
  }

  public void GiveUID()
  {
    Assert.IsTrue(this.uid == 0U, "Calling GiveUID - but already has a uid!");
    this.uid = ((Server) Net.sv).TakeUID();
  }

  public void MarkDirty()
  {
    this.dirty = true;
    if (this.parent != null)
      this.parent.MarkDirty();
    if (this.onDirty == null)
      return;
    this.onDirty();
  }

  public DroppedItemContainer Drop(string prefab, Vector3 pos, Quaternion rot)
  {
    if ((this.itemList != null ? this.itemList.Count : 0) == 0)
      return (DroppedItemContainer) null;
    BaseEntity entity = GameManager.server.CreateEntity(prefab, pos, rot, true);
    if (Object.op_Equality((Object) entity, (Object) null))
      return (DroppedItemContainer) null;
    DroppedItemContainer droppedItemContainer = entity as DroppedItemContainer;
    if (Object.op_Inequality((Object) droppedItemContainer, (Object) null))
      droppedItemContainer.TakeFrom(this);
    droppedItemContainer.Spawn();
    return droppedItemContainer;
  }

  public static DroppedItemContainer Drop(
    string prefab,
    Vector3 pos,
    Quaternion rot,
    params ItemContainer[] containers)
  {
    int num = 0;
    for (int index = 0; index < containers.Length; ++index)
    {
      ItemContainer container = containers[index];
      num += container.itemList != null ? container.itemList.Count : 0;
    }
    if (num == 0)
      return (DroppedItemContainer) null;
    BaseEntity entity = GameManager.server.CreateEntity(prefab, pos, rot, true);
    if (Object.op_Equality((Object) entity, (Object) null))
      return (DroppedItemContainer) null;
    DroppedItemContainer droppedItemContainer = entity as DroppedItemContainer;
    if (Object.op_Inequality((Object) droppedItemContainer, (Object) null))
      droppedItemContainer.TakeFrom(containers);
    droppedItemContainer.Spawn();
    return droppedItemContainer;
  }

  public void OnChanged()
  {
    for (int index = 0; index < this.itemList.Count; ++index)
      this.itemList[index].OnChanged();
  }

  public Item FindItemByUID(uint iUID)
  {
    for (int index = 0; index < this.itemList.Count; ++index)
    {
      Item obj1 = this.itemList[index];
      if (obj1.IsValid())
      {
        Item obj2 = obj1.FindItem(iUID);
        if (obj2 != null)
          return obj2;
      }
    }
    return (Item) null;
  }

  public bool IsFull()
  {
    return this.itemList.Count >= this.capacity;
  }

  public bool CanTake(Item item)
  {
    return !this.IsFull();
  }

  public bool Insert(Item item)
  {
    if (this.itemList.Contains(item) || this.IsFull())
      return false;
    this.itemList.Add(item);
    item.parent = this;
    if (!this.FindPosition(item))
      return false;
    this.MarkDirty();
    if (this.onItemAddedRemoved != null)
      this.onItemAddedRemoved(item, true);
    Interface.CallHook("OnItemAddedToContainer", (object) this, (object) item);
    return true;
  }

  public bool SlotTaken(int i)
  {
    return this.GetSlot(i) != null;
  }

  public Item GetSlot(int slot)
  {
    for (int index = 0; index < this.itemList.Count; ++index)
    {
      if (this.itemList[index].position == slot)
        return this.itemList[index];
    }
    return (Item) null;
  }

  public bool FindPosition(Item item)
  {
    int position = item.position;
    item.position = -1;
    if (position >= 0 && !this.SlotTaken(position))
    {
      item.position = position;
      return true;
    }
    for (int i = 0; i < this.capacity; ++i)
    {
      if (!this.SlotTaken(i))
      {
        item.position = i;
        return true;
      }
    }
    return false;
  }

  public void SetLocked(bool isLocked)
  {
    this.SetFlag(ItemContainer.Flag.IsLocked, isLocked);
    this.MarkDirty();
  }

  public bool Remove(Item item)
  {
    if (!this.itemList.Contains(item))
      return false;
    if (this.onPreItemRemove != null)
      this.onPreItemRemove(item);
    this.itemList.Remove(item);
    item.parent = (ItemContainer) null;
    this.MarkDirty();
    if (this.onItemAddedRemoved != null)
      this.onItemAddedRemoved(item, false);
    Interface.CallHook("OnItemRemovedFromContainer", (object) this, (object) item);
    return true;
  }

  public void Clear()
  {
    foreach (Item obj in this.itemList.ToArray())
      obj.Remove(0.0f);
  }

  public void Kill()
  {
    this.onDirty = (Action) null;
    this.canAcceptItem = (Func<Item, int, bool>) null;
    this.onItemAddedRemoved = (Action<Item, bool>) null;
    if (Net.sv != null)
    {
      ((Server) Net.sv).ReturnUID(this.uid);
      this.uid = 0U;
    }
    foreach (Item obj in this.itemList.ToList<Item>())
      obj.Remove(0.0f);
    this.itemList.Clear();
  }

  public int GetAmount(int itemid, bool onlyUsableAmounts)
  {
    int num = 0;
    foreach (Item obj in this.itemList)
    {
      if (obj.info.itemid == itemid && (!onlyUsableAmounts || !obj.IsBusy()))
        num += obj.amount;
    }
    return num;
  }

  public Item FindItemByItemID(int itemid)
  {
    return this.itemList.FirstOrDefault<Item>((Func<Item, bool>) (x => x.info.itemid == itemid));
  }

  public Item FindItemsByItemName(string name)
  {
    ItemDefinition itemDefinition = ItemManager.FindItemDefinition(name);
    if (Object.op_Equality((Object) itemDefinition, (Object) null))
      return (Item) null;
    for (int index = 0; index < this.itemList.Count; ++index)
    {
      if (Object.op_Equality((Object) this.itemList[index].info, (Object) itemDefinition))
        return this.itemList[index];
    }
    return (Item) null;
  }

  public List<Item> FindItemsByItemID(int itemid)
  {
    return this.itemList.FindAll((Predicate<Item>) (x => x.info.itemid == itemid));
  }

  public ItemContainer Save()
  {
    ItemContainer itemContainer = (ItemContainer) Pool.Get<ItemContainer>();
    itemContainer.contents = (__Null) Pool.GetList<Item>();
    itemContainer.UID = (__Null) (int) this.uid;
    itemContainer.slots = (__Null) this.capacity;
    itemContainer.temperature = (__Null) (double) this.temperature;
    itemContainer.allowedContents = (__Null) this.allowedContents;
    itemContainer.allowedItem = Object.op_Inequality((Object) this.onlyAllowedItem, (Object) null) ? (__Null) this.onlyAllowedItem.itemid : (__Null) 0;
    itemContainer.flags = (__Null) this.flags;
    itemContainer.maxStackSize = (__Null) this.maxStackSize;
    if (this.availableSlots != null && this.availableSlots.Count > 0)
    {
      itemContainer.availableSlots = (__Null) Pool.GetList<int>();
      for (int index = 0; index < this.availableSlots.Count; ++index)
        ((List<int>) itemContainer.availableSlots).Add((int) this.availableSlots[index]);
    }
    for (int index = 0; index < this.itemList.Count; ++index)
    {
      Item obj = this.itemList[index];
      if (obj.IsValid())
        ((List<Item>) itemContainer.contents).Add(obj.Save(true, true));
    }
    return itemContainer;
  }

  public void Load(ItemContainer container)
  {
    using (TimeWarning.New("ItemContainer.Load", 0.1f))
    {
      this.uid = (uint) container.UID;
      this.capacity = (int) container.slots;
      List<Item> itemList = this.itemList;
      this.itemList = (List<Item>) Pool.GetList<Item>();
      this.temperature = (float) container.temperature;
      this.flags = (ItemContainer.Flag) container.flags;
      this.allowedContents = container.allowedContents == null ? ItemContainer.ContentsType.Generic : (ItemContainer.ContentsType) container.allowedContents;
      this.onlyAllowedItem = container.allowedItem != null ? ItemManager.FindItemDefinition((int) container.allowedItem) : (ItemDefinition) null;
      this.maxStackSize = (int) container.maxStackSize;
      this.availableSlots.Clear();
      for (int index = 0; index < ((List<int>) container.availableSlots).Count; ++index)
        this.availableSlots.Add((ItemSlot) ((List<int>) container.availableSlots)[index]);
      using (TimeWarning.New("container.contents", 0.1f))
      {
        using (List<Item>.Enumerator enumerator = ((List<Item>) container.contents).GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Item current = enumerator.Current;
            Item created = (Item) null;
            foreach (Item obj in itemList)
            {
              if ((int) obj.uid == current.UID)
              {
                created = obj;
                break;
              }
            }
            Item obj1 = ItemManager.Load(current, created, this.isServer);
            if (obj1 != null)
            {
              obj1.parent = this;
              obj1.position = (int) current.slot;
              this.Insert(obj1);
            }
          }
        }
      }
      using (TimeWarning.New("Delete old items", 0.1f))
      {
        foreach (Item obj in itemList)
        {
          if (!this.itemList.Contains(obj))
            obj.Remove(0.0f);
        }
      }
      this.dirty = true;
      // ISSUE: cast to a reference type
      Pool.FreeList<Item>((List<M0>&) ref itemList);
    }
  }

  public BasePlayer GetOwnerPlayer()
  {
    return this.playerOwner;
  }

  public int Take(List<Item> collect, int itemid, int iAmount)
  {
    int num1 = 0;
    if (iAmount == 0)
      return num1;
    List<Item> list = (List<Item>) Pool.GetList<Item>();
    foreach (Item obj in this.itemList)
    {
      if (obj.info.itemid == itemid)
      {
        int num2 = iAmount - num1;
        if (num2 > 0)
        {
          if (obj.amount > num2)
          {
            obj.MarkDirty();
            obj.amount -= num2;
            num1 += num2;
            Item byItemId = ItemManager.CreateByItemID(itemid, 1, 0UL);
            byItemId.amount = num2;
            byItemId.CollectedForCrafting(this.playerOwner);
            if (collect != null)
            {
              collect.Add(byItemId);
              break;
            }
            break;
          }
          if (obj.amount <= num2)
          {
            num1 += obj.amount;
            list.Add(obj);
            collect?.Add(obj);
          }
          if (num1 == iAmount)
            break;
        }
      }
    }
    foreach (Item obj in list)
      obj.RemoveFromContainer();
    // ISSUE: cast to a reference type
    Pool.FreeList<Item>((List<M0>&) ref list);
    return num1;
  }

  public Vector3 dropPosition
  {
    get
    {
      if (Object.op_Implicit((Object) this.playerOwner))
        return this.playerOwner.GetDropPosition();
      if (Object.op_Implicit((Object) this.entityOwner))
        return this.entityOwner.GetDropPosition();
      if (this.parent != null)
      {
        BaseEntity worldEntity = this.parent.GetWorldEntity();
        if (Object.op_Inequality((Object) worldEntity, (Object) null))
          return worldEntity.GetDropPosition();
      }
      Debug.LogWarning((object) "ItemContainer.dropPosition dropped through");
      return Vector3.get_zero();
    }
  }

  public Vector3 dropVelocity
  {
    get
    {
      if (Object.op_Implicit((Object) this.playerOwner))
        return this.playerOwner.GetDropVelocity();
      if (Object.op_Implicit((Object) this.entityOwner))
        return this.entityOwner.GetDropVelocity();
      if (this.parent != null)
      {
        BaseEntity worldEntity = this.parent.GetWorldEntity();
        if (Object.op_Inequality((Object) worldEntity, (Object) null))
          return worldEntity.GetDropVelocity();
      }
      Debug.LogWarning((object) "ItemContainer.dropVelocity dropped through");
      return Vector3.get_zero();
    }
  }

  public void OnCycle(float delta)
  {
    for (int index = 0; index < this.itemList.Count; ++index)
    {
      if (this.itemList[index].IsValid())
        this.itemList[index].OnCycle(delta);
    }
  }

  public void FindAmmo(List<Item> list, AmmoTypes ammoType)
  {
    for (int index = 0; index < this.itemList.Count; ++index)
      this.itemList[index].FindAmmo(list, ammoType);
  }

  public bool HasAmmo(AmmoTypes ammoType)
  {
    for (int index = 0; index < this.itemList.Count; ++index)
    {
      if (this.itemList[index].HasAmmo(ammoType))
        return true;
    }
    return false;
  }

  public void AddItem(ItemDefinition itemToCreate, int p)
  {
    for (int index = 0; index < this.itemList.Count; ++index)
    {
      if (p == 0)
        return;
      if (!Object.op_Inequality((Object) this.itemList[index].info, (Object) itemToCreate))
      {
        int num = this.itemList[index].MaxStackable();
        if (num > this.itemList[index].amount)
        {
          this.MarkDirty();
          this.itemList[index].amount += p;
          p -= p;
          if (this.itemList[index].amount > num)
          {
            p = this.itemList[index].amount - num;
            if (p > 0)
              this.itemList[index].amount -= p;
          }
        }
      }
    }
    if (p == 0)
      return;
    Item obj = ItemManager.Create(itemToCreate, p, 0UL);
    if (obj.MoveToContainer(this, -1, true))
      return;
    obj.Remove(0.0f);
  }

  public void OnMovedToWorld()
  {
    for (int index = 0; index < this.itemList.Count; ++index)
      this.itemList[index].OnMovedToWorld();
  }

  public void OnRemovedFromWorld()
  {
    for (int index = 0; index < this.itemList.Count; ++index)
      this.itemList[index].OnRemovedFromWorld();
  }

  public uint ContentsHash()
  {
    uint num = 0;
    for (int slot1 = 0; slot1 < this.capacity; ++slot1)
    {
      Item slot2 = this.GetSlot(slot1);
      if (slot2 != null)
        num = CRC.Compute32(CRC.Compute32(num, slot2.info.itemid), slot2.skin);
    }
    return num;
  }

  public ItemContainer FindContainer(uint id)
  {
    if ((int) id == (int) this.uid)
      return this;
    for (int index = 0; index < this.itemList.Count; ++index)
    {
      Item obj = this.itemList[index];
      if (obj.contents != null)
      {
        ItemContainer container = obj.contents.FindContainer(id);
        if (container != null)
          return container;
      }
    }
    return (ItemContainer) null;
  }

  public ItemContainer.CanAcceptResult CanAcceptItem(Item item, int targetPos)
  {
    if (this.canAcceptItem != null && !this.canAcceptItem(item, targetPos) || (this.allowedContents & item.info.itemType) != item.info.itemType || Object.op_Inequality((Object) this.onlyAllowedItem, (Object) null) && Object.op_Inequality((Object) this.onlyAllowedItem, (Object) item.info))
      return ItemContainer.CanAcceptResult.CannotAccept;
    if (this.availableSlots != null && this.availableSlots.Count > 0)
    {
      if (item.info.occupySlots == (ItemSlot) 0 || item.info.occupySlots == ItemSlot.None)
        return ItemContainer.CanAcceptResult.CannotAccept;
      int[] numArray = new int[32];
      foreach (ItemSlot availableSlot in this.availableSlots)
        ++numArray[(int) Mathf.Log((float) availableSlot, 2f)];
      foreach (Item obj in this.itemList)
      {
        for (int index = 0; index < 32; ++index)
        {
          if ((obj.info.occupySlots & (ItemSlot) (1 << index)) != (ItemSlot) 0)
            --numArray[index];
        }
      }
      for (int index = 0; index < 32; ++index)
      {
        if ((item.info.occupySlots & (ItemSlot) (1 << index)) != (ItemSlot) 0 && numArray[index] <= 0)
          return ItemContainer.CanAcceptResult.CannotAcceptRightNow;
      }
    }
    object obj1 = Interface.CallHook(nameof (CanAcceptItem), (object) this, (object) item, (object) targetPos);
    if (obj1 is ItemContainer.CanAcceptResult)
      return (ItemContainer.CanAcceptResult) obj1;
    return ItemContainer.CanAcceptResult.CanAccept;
  }

  [System.Flags]
  public enum Flag
  {
    IsPlayer = 1,
    Clothing = 2,
    Belt = 4,
    SingleType = 8,
    IsLocked = 16, // 0x00000010
    ShowSlotsOnIcon = 32, // 0x00000020
    NoBrokenItems = 64, // 0x00000040
    NoItemInput = 128, // 0x00000080
  }

  [System.Flags]
  public enum ContentsType
  {
    Generic = 1,
    Liquid = 2,
  }

  public enum CanAcceptResult
  {
    CanAccept,
    CannotAccept,
    CannotAcceptRightNow,
  }
}
