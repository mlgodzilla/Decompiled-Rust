// Decompiled with JetBrains decompiler
// Type: Item
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
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

public class Item
{
  public float _maxCondition = 100f;
  public int amount = 1;
  public float _condition;
  public ItemDefinition info;
  public uint uid;
  public bool dirty;
  public int position;
  public float busyTime;
  public float removeTime;
  public float fuel;
  public bool isServer;
  public Item.InstanceData instanceData;
  public ulong skin;
  public string name;
  public string text;
  public Item.Flag flags;
  public ItemContainer contents;
  public ItemContainer parent;
  private EntityRef worldEnt;
  private EntityRef heldEntity;

  public float condition
  {
    set
    {
      float condition = this._condition;
      this._condition = Mathf.Clamp(value, 0.0f, this.maxCondition);
      if (!this.isServer || (double) Mathf.Ceil(value) == (double) Mathf.Ceil(condition))
        return;
      this.MarkDirty();
    }
    get
    {
      return this._condition;
    }
  }

  public float maxCondition
  {
    set
    {
      this._maxCondition = Mathf.Clamp(value, 0.0f, this.info.condition.max);
      if (!this.isServer)
        return;
      this.MarkDirty();
    }
    get
    {
      return this._maxCondition;
    }
  }

  public float maxConditionNormalized
  {
    get
    {
      return this._maxCondition / this.info.condition.max;
    }
  }

  public float conditionNormalized
  {
    get
    {
      if (!this.hasCondition)
        return 1f;
      return this.condition / this.maxCondition;
    }
    set
    {
      if (!this.hasCondition)
        return;
      this.condition = value * this.maxCondition;
    }
  }

  public bool hasCondition
  {
    get
    {
      if (Object.op_Inequality((Object) this.info, (Object) null) && this.info.condition.enabled)
        return (double) this.info.condition.max > 0.0;
      return false;
    }
  }

  public bool isBroken
  {
    get
    {
      if (this.hasCondition)
        return (double) this.condition <= 0.0;
      return false;
    }
  }

  public void LoseCondition(float amount)
  {
    if (!this.hasCondition || Debugging.disablecondition || Interface.CallHook("IOnLoseCondition", (object) this, (object) amount) != null)
      return;
    float condition = this.condition;
    this.condition -= amount;
    if (Global.developer > 0)
      Debug.Log((object) (this.info.shortname + " was damaged by: " + (object) amount + "cond is: " + (object) this.condition + "/" + (object) this.maxCondition));
    if ((double) this.condition > 0.0 || (double) this.condition >= (double) condition)
      return;
    this.OnBroken();
  }

  public void RepairCondition(float amount)
  {
    if (!this.hasCondition)
      return;
    this.condition += amount;
  }

  public void DoRepair(float maxLossFraction)
  {
    if (!this.hasCondition)
      return;
    if (this.info.condition.maintainMaxCondition)
      maxLossFraction = 0.0f;
    float num = (float) (1.0 - (double) this.condition / (double) this.maxCondition);
    maxLossFraction = Mathf.Clamp(maxLossFraction, 0.0f, this.info.condition.max);
    this.maxCondition *= (float) (1.0 - (double) maxLossFraction * (double) num);
    this.condition = this.maxCondition;
    BaseEntity heldEntity = this.GetHeldEntity();
    if (Object.op_Inequality((Object) heldEntity, (Object) null))
      heldEntity.SetFlag(BaseEntity.Flags.Broken, false, false, true);
    if (Global.developer <= 0)
      return;
    Debug.Log((object) (this.info.shortname + " was repaired! new cond is: " + (object) this.condition + "/" + (object) this.maxCondition));
  }

  public ItemContainer GetRootContainer()
  {
    ItemContainer parent = this.parent;
    int num;
    for (num = 0; parent != null && num <= 8 && (parent.parent != null && parent.parent.parent != null); ++num)
      parent = parent.parent.parent;
    if (num == 8)
      Debug.LogWarning((object) "GetRootContainer failed with 8 iterations");
    return parent;
  }

  public virtual void OnBroken()
  {
    if (!this.hasCondition)
      return;
    BaseEntity heldEntity = this.GetHeldEntity();
    if (Object.op_Inequality((Object) heldEntity, (Object) null))
      heldEntity.SetFlag(BaseEntity.Flags.Broken, true, false, true);
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Implicit((Object) ownerPlayer) && ownerPlayer.GetActiveItem() == this)
    {
      Effect.server.Run("assets/bundled/prefabs/fx/item_break.prefab", (BaseEntity) ownerPlayer, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
      ownerPlayer.ChatMessage("Your active item was broken!");
    }
    if (!this.info.condition.repairable || (double) this.maxCondition <= 5.0)
      this.Remove(0.0f);
    else if (this.parent != null && this.parent.HasFlag(ItemContainer.Flag.NoBrokenItems))
    {
      ItemContainer rootContainer = this.GetRootContainer();
      if (rootContainer.HasFlag(ItemContainer.Flag.NoBrokenItems))
      {
        this.Remove(0.0f);
      }
      else
      {
        BasePlayer playerOwner = rootContainer.playerOwner;
        if (Object.op_Inequality((Object) playerOwner, (Object) null) && !this.MoveToContainer(playerOwner.inventory.containerMain, -1, true))
          this.Drop(((Component) playerOwner).get_transform().get_position(), Vector3.op_Multiply(playerOwner.eyes.BodyForward(), 1.5f), (Quaternion) null);
      }
    }
    this.MarkDirty();
  }

  public int despawnMultiplier
  {
    get
    {
      if (!Object.op_Inequality((Object) this.info, (Object) null))
        return 1;
      return Mathf.Clamp((this.info.rarity - 1) * 4, 1, 100);
    }
  }

  public ItemDefinition blueprintTargetDef
  {
    get
    {
      if (!this.IsBlueprint())
        return (ItemDefinition) null;
      return ItemManager.FindItemDefinition(this.blueprintTarget);
    }
  }

  public int blueprintTarget
  {
    get
    {
      if (this.instanceData == null)
        return 0;
      return (int) this.instanceData.blueprintTarget;
    }
    set
    {
      if (this.instanceData == null)
        this.instanceData = new Item.InstanceData();
      this.instanceData.ShouldPool = (__Null) 0;
      this.instanceData.blueprintTarget = (__Null) value;
    }
  }

  public int blueprintAmount
  {
    get
    {
      return this.amount;
    }
    set
    {
      this.amount = value;
    }
  }

  public bool IsBlueprint()
  {
    return (uint) this.blueprintTarget > 0U;
  }

  public event Action<Item> OnDirty;

  public bool HasFlag(Item.Flag f)
  {
    return (this.flags & f) == f;
  }

  public void SetFlag(Item.Flag f, bool b)
  {
    if (b)
      this.flags |= f;
    else
      this.flags &= ~f;
  }

  public bool IsOn()
  {
    return this.HasFlag(Item.Flag.IsOn);
  }

  public bool IsOnFire()
  {
    return this.HasFlag(Item.Flag.OnFire);
  }

  public bool IsCooking()
  {
    return this.HasFlag(Item.Flag.Cooking);
  }

  public bool IsLocked()
  {
    if (this.HasFlag(Item.Flag.IsLocked))
      return true;
    if (this.parent != null)
      return this.parent.IsLocked();
    return false;
  }

  public Item parentItem
  {
    get
    {
      if (this.parent == null)
        return (Item) null;
      return this.parent.parent;
    }
  }

  public void MarkDirty()
  {
    this.OnChanged();
    this.dirty = true;
    if (this.parent != null)
      this.parent.MarkDirty();
    if (this.OnDirty == null)
      return;
    this.OnDirty(this);
  }

  public void OnChanged()
  {
    foreach (ItemMod itemMod in this.info.itemMods)
      itemMod.OnChanged(this);
    if (this.contents == null)
      return;
    this.contents.OnChanged();
  }

  public void CollectedForCrafting(BasePlayer crafter)
  {
    foreach (ItemMod itemMod in this.info.itemMods)
      itemMod.CollectedForCrafting(this, crafter);
  }

  public void ReturnedFromCancelledCraft(BasePlayer crafter)
  {
    foreach (ItemMod itemMod in this.info.itemMods)
      itemMod.ReturnedFromCancelledCraft(this, crafter);
  }

  public void Initialize(ItemDefinition template)
  {
    this.uid = ((Network.Server) Net.sv).TakeUID();
    this.condition = this.maxCondition = this.info.condition.max;
    this.OnItemCreated();
  }

  public void OnItemCreated()
  {
    this.onCycle = (Action<Item, float>) null;
    foreach (ItemMod itemMod in this.info.itemMods)
      itemMod.OnItemCreated(this);
  }

  public void OnVirginSpawn()
  {
    foreach (ItemMod itemMod in this.info.itemMods)
      itemMod.OnVirginItem(this);
  }

  public void RemoveFromWorld()
  {
    BaseEntity worldEntity = this.GetWorldEntity();
    if (Object.op_Equality((Object) worldEntity, (Object) null))
      return;
    this.SetWorldEntity((BaseEntity) null);
    this.OnRemovedFromWorld();
    if (this.contents != null)
      this.contents.OnRemovedFromWorld();
    if (!worldEntity.IsValid())
      return;
    worldEntity.Kill(BaseNetworkable.DestroyMode.None);
  }

  public void OnRemovedFromWorld()
  {
    foreach (ItemMod itemMod in this.info.itemMods)
      itemMod.OnRemovedFromWorld(this);
  }

  public void RemoveFromContainer()
  {
    if (this.parent == null)
      return;
    this.SetParent((ItemContainer) null);
  }

  public void SetParent(ItemContainer target)
  {
    if (target == this.parent)
      return;
    if (this.parent != null)
    {
      this.parent.Remove(this);
      this.parent = (ItemContainer) null;
    }
    if (target == null)
    {
      this.position = 0;
    }
    else
    {
      this.parent = target;
      if (!this.parent.Insert(this))
      {
        this.Remove(0.0f);
        Debug.LogError((object) "Item.SetParent caused remove - this shouldn't ever happen");
      }
    }
    this.MarkDirty();
    foreach (ItemMod itemMod in this.info.itemMods)
      itemMod.OnParentChanged(this);
  }

  public void OnAttacked(HitInfo hitInfo)
  {
    foreach (ItemMod itemMod in this.info.itemMods)
      itemMod.OnAttacked(this, hitInfo);
  }

  public bool IsChildContainer(ItemContainer c)
  {
    if (this.contents == null)
      return false;
    if (this.contents == c)
      return true;
    foreach (Item obj in this.contents.itemList)
    {
      if (obj.IsChildContainer(c))
        return true;
    }
    return false;
  }

  public bool CanMoveTo(ItemContainer newcontainer, int iTargetPos = -1, bool allowStack = true)
  {
    return !this.IsChildContainer(newcontainer) && newcontainer.CanAcceptItem(this, iTargetPos) == ItemContainer.CanAcceptResult.CanAccept && iTargetPos < newcontainer.capacity && (this.parent == null || newcontainer != this.parent || iTargetPos != this.position);
  }

  public bool MoveToContainer(ItemContainer newcontainer, int iTargetPos = -1, bool allowStack = true)
  {
    using (TimeWarning.New(nameof (MoveToContainer), 0.1f))
    {
      ItemContainer parent1 = this.parent;
      if (!this.CanMoveTo(newcontainer, iTargetPos, allowStack))
        return false;
      if (iTargetPos >= 0 && newcontainer.SlotTaken(iTargetPos))
      {
        Item slot = newcontainer.GetSlot(iTargetPos);
        if (allowStack)
        {
          int num = slot.MaxStackable();
          if (slot.CanStack(this))
          {
            if (slot.amount >= num)
              return false;
            slot.amount += this.amount;
            slot.MarkDirty();
            this.RemoveFromWorld();
            this.RemoveFromContainer();
            this.Remove(0.0f);
            int split_Amount = slot.amount - num;
            if (split_Amount > 0)
            {
              Item obj = slot.SplitItem(split_Amount);
              if (obj != null && !obj.MoveToContainer(newcontainer, -1, false) && (parent1 == null || !obj.MoveToContainer(parent1, -1, true)))
                obj.Drop(newcontainer.dropPosition, newcontainer.dropVelocity, (Quaternion) null);
              slot.amount = num;
            }
            return true;
          }
        }
        if (this.parent == null)
          return false;
        ItemContainer parent2 = this.parent;
        int position = this.position;
        if (!slot.CanMoveTo(parent2, position, true))
          return false;
        this.RemoveFromContainer();
        slot.RemoveFromContainer();
        slot.MoveToContainer(parent2, position, true);
        return this.MoveToContainer(newcontainer, iTargetPos, true);
      }
      if (this.parent == newcontainer)
      {
        if (iTargetPos < 0 || iTargetPos == this.position || this.parent.SlotTaken(iTargetPos))
          return false;
        this.position = iTargetPos;
        this.MarkDirty();
        return true;
      }
      if (iTargetPos == -1 & allowStack && this.info.stackable > 1)
      {
        Item obj = newcontainer.FindItemsByItemID(this.info.itemid).OrderBy<Item, int>((Func<Item, int>) (x => x.amount)).FirstOrDefault<Item>();
        if (obj != null && obj.CanStack(this))
        {
          int num1 = obj.MaxStackable();
          if (obj.amount < num1)
          {
            obj.amount += this.amount;
            obj.MarkDirty();
            int num2 = obj.amount - num1;
            if (num2 <= 0)
            {
              this.RemoveFromWorld();
              this.RemoveFromContainer();
              this.Remove(0.0f);
              return true;
            }
            this.amount = num2;
            this.MarkDirty();
            obj.amount = num1;
            return this.MoveToContainer(newcontainer, iTargetPos, allowStack);
          }
        }
      }
      if (newcontainer.maxStackSize > 0 && newcontainer.maxStackSize < this.amount)
      {
        Item obj = this.SplitItem(newcontainer.maxStackSize);
        if (obj != null && !obj.MoveToContainer(newcontainer, iTargetPos, false) && (parent1 == null || !obj.MoveToContainer(parent1, -1, true)))
          obj.Drop(newcontainer.dropPosition, newcontainer.dropVelocity, (Quaternion) null);
        return true;
      }
      if (!newcontainer.CanTake(this))
        return false;
      this.RemoveFromContainer();
      this.RemoveFromWorld();
      this.position = iTargetPos;
      this.SetParent(newcontainer);
      return true;
    }
  }

  public BaseEntity CreateWorldObject(
    Vector3 pos,
    Quaternion rotation = null,
    BaseEntity parentEnt = null,
    uint parentBone = 0)
  {
    BaseEntity worldEntity = this.GetWorldEntity();
    if (Object.op_Inequality((Object) worldEntity, (Object) null))
      return worldEntity;
    BaseEntity entity = GameManager.server.CreateEntity("assets/prefabs/misc/burlap sack/generic_world.prefab", pos, rotation, true);
    if (Object.op_Equality((Object) entity, (Object) null))
    {
      Debug.LogWarning((object) "Couldn't create world object for prefab: items/generic_world");
      return (BaseEntity) null;
    }
    WorldItem worldItem = entity as WorldItem;
    if (Object.op_Inequality((Object) worldItem, (Object) null))
      worldItem.InitializeItem(this);
    if (Object.op_Inequality((Object) parentEnt, (Object) null))
      entity.SetParent(parentEnt, parentBone, false, false);
    entity.Spawn();
    this.SetWorldEntity(entity);
    return this.GetWorldEntity();
  }

  public BaseEntity Drop(Vector3 vPos, Vector3 vVelocity, Quaternion rotation = null)
  {
    this.RemoveFromWorld();
    BaseEntity baseEntity = (BaseEntity) null;
    if (Vector3.op_Inequality(vPos, Vector3.get_zero()) && !this.info.HasFlag(ItemDefinition.Flag.NoDropping))
    {
      baseEntity = this.CreateWorldObject(vPos, rotation, (BaseEntity) null, 0U);
      if (Object.op_Implicit((Object) baseEntity))
        baseEntity.SetVelocity(vVelocity);
    }
    else
      this.Remove(0.0f);
    Interface.CallHook("OnItemDropped", (object) this, (object) baseEntity);
    this.RemoveFromContainer();
    return baseEntity;
  }

  public bool IsBusy()
  {
    return (double) this.busyTime > (double) Time.get_time();
  }

  public void BusyFor(float fTime)
  {
    this.busyTime = Time.get_time() + fTime;
  }

  public void Remove(float fTime = 0.0f)
  {
    if ((double) this.removeTime > 0.0 || Interface.CallHook("OnItemRemove", (object) this) != null)
      return;
    if (this.isServer)
    {
      foreach (ItemMod itemMod in this.info.itemMods)
        itemMod.OnRemove(this);
    }
    this.onCycle = (Action<Item, float>) null;
    this.removeTime = Time.get_time() + fTime;
    this.OnDirty = (Action<Item>) null;
    this.position = -1;
    if (!this.isServer)
      return;
    ItemManager.RemoveItem(this, fTime);
  }

  public void DoRemove()
  {
    this.OnDirty = (Action<Item>) null;
    this.onCycle = (Action<Item, float>) null;
    if (this.isServer && this.uid > 0U && Net.sv != null)
    {
      ((Network.Server) Net.sv).ReturnUID(this.uid);
      this.uid = 0U;
    }
    if (this.contents != null)
    {
      this.contents.Kill();
      this.contents = (ItemContainer) null;
    }
    if (this.isServer)
    {
      this.RemoveFromWorld();
      this.RemoveFromContainer();
    }
    BaseEntity heldEntity = this.GetHeldEntity();
    if (!heldEntity.IsValid())
      return;
    Debug.LogWarning((object) ("Item's Held Entity not removed!" + this.info.displayName.english + " -> " + (object) heldEntity), (Object) heldEntity);
  }

  public void SwitchOnOff(bool bNewState, BasePlayer player)
  {
    if (this.HasFlag(Item.Flag.IsOn) == bNewState)
      return;
    this.SetFlag(Item.Flag.IsOn, bNewState);
    this.MarkDirty();
  }

  public void LockUnlock(bool bNewState, BasePlayer player)
  {
    if (this.HasFlag(Item.Flag.IsLocked) == bNewState)
      return;
    this.SetFlag(Item.Flag.IsLocked, bNewState);
    this.MarkDirty();
  }

  public float temperature
  {
    get
    {
      if (this.parent != null)
        return this.parent.temperature;
      return 15f;
    }
  }

  public BasePlayer GetOwnerPlayer()
  {
    if (this.parent == null)
      return (BasePlayer) null;
    return this.parent.GetOwnerPlayer();
  }

  public Item SplitItem(int split_Amount)
  {
    Assert.IsTrue(split_Amount > 0, "split_Amount <= 0");
    if (split_Amount <= 0)
      return (Item) null;
    if (split_Amount >= this.amount)
      return (Item) null;
    object obj = Interface.CallHook("OnItemSplit", (object) this, (object) split_Amount);
    if (obj is Item)
      return (Item) obj;
    this.amount -= split_Amount;
    Item byItemId = ItemManager.CreateByItemID(this.info.itemid, 1, 0UL);
    byItemId.amount = split_Amount;
    if (this.IsBlueprint())
      byItemId.blueprintTarget = this.blueprintTarget;
    this.MarkDirty();
    return byItemId;
  }

  public bool CanBeHeld()
  {
    return !this.isBroken;
  }

  public bool CanStack(Item item)
  {
    object obj = Interface.CallHook("CanStackItem", (object) this, (object) item);
    if (obj is bool)
      return (bool) obj;
    return item != this && this.info.stackable > 1 && (item.info.stackable > 1 && item.info.itemid == this.info.itemid) && ((!this.hasCondition || (double) this.condition == (double) this.maxCondition) && (!item.hasCondition || (double) item.condition == (double) item.maxCondition)) && (this.IsValid() && (!this.IsBlueprint() || this.blueprintTarget == item.blueprintTarget));
  }

  public bool IsValid()
  {
    return (double) this.removeTime <= 0.0;
  }

  public void SetWorldEntity(BaseEntity ent)
  {
    if (!ent.IsValid())
    {
      this.worldEnt.Set((BaseEntity) null);
      this.MarkDirty();
    }
    else
    {
      if ((int) this.worldEnt.uid == ent.net.ID)
        return;
      this.worldEnt.Set(ent);
      this.MarkDirty();
      this.OnMovedToWorld();
      if (this.contents == null)
        return;
      this.contents.OnMovedToWorld();
    }
  }

  public void OnMovedToWorld()
  {
    foreach (ItemMod itemMod in this.info.itemMods)
      itemMod.OnMovedToWorld(this);
  }

  public BaseEntity GetWorldEntity()
  {
    return this.worldEnt.Get(this.isServer);
  }

  public void SetHeldEntity(BaseEntity ent)
  {
    if (!ent.IsValid())
    {
      this.heldEntity.Set((BaseEntity) null);
      this.MarkDirty();
    }
    else
    {
      if ((int) this.heldEntity.uid == ent.net.ID)
        return;
      this.heldEntity.Set(ent);
      this.MarkDirty();
      if (!ent.IsValid())
        return;
      HeldEntity heldEntity = ent as HeldEntity;
      if (!Object.op_Inequality((Object) heldEntity, (Object) null))
        return;
      heldEntity.SetupHeldEntity(this);
    }
  }

  public BaseEntity GetHeldEntity()
  {
    return this.heldEntity.Get(this.isServer);
  }

  public event Action<Item, float> onCycle;

  public void OnCycle(float delta)
  {
    if (this.onCycle == null)
      return;
    this.onCycle(this, delta);
  }

  public void ServerCommand(string command, BasePlayer player)
  {
    HeldEntity heldEntity = this.GetHeldEntity() as HeldEntity;
    if (Object.op_Inequality((Object) heldEntity, (Object) null))
      heldEntity.ServerCommand(this, command, player);
    foreach (ItemMod itemMod in this.info.itemMods)
      itemMod.ServerCommand(this, command, player);
  }

  public void UseItem(int amountToConsume = 1)
  {
    if (amountToConsume <= 0)
      return;
    Interface.CallHook("OnItemUse", (object) this, (object) amountToConsume);
    this.amount -= amountToConsume;
    if (this.amount <= 0)
    {
      this.amount = 0;
      this.Remove(0.0f);
    }
    else
      this.MarkDirty();
  }

  public bool HasAmmo(AmmoTypes ammoType)
  {
    ItemModProjectile component = (ItemModProjectile) ((Component) this.info).GetComponent<ItemModProjectile>();
    if (Object.op_Implicit((Object) component) && component.IsAmmo(ammoType))
      return true;
    if (this.contents != null)
      return this.contents.HasAmmo(ammoType);
    return false;
  }

  public void FindAmmo(List<Item> list, AmmoTypes ammoType)
  {
    ItemModProjectile component = (ItemModProjectile) ((Component) this.info).GetComponent<ItemModProjectile>();
    if (Object.op_Implicit((Object) component) && component.IsAmmo(ammoType))
    {
      list.Add(this);
    }
    else
    {
      if (this.contents == null)
        return;
      this.contents.FindAmmo(list, ammoType);
    }
  }

  public override string ToString()
  {
    return "Item." + this.info.shortname + "x" + (object) this.amount + "." + (object) this.uid;
  }

  public Item FindItem(uint iUID)
  {
    if ((int) this.uid == (int) iUID)
      return this;
    if (this.contents == null)
      return (Item) null;
    return this.contents.FindItemByUID(iUID);
  }

  public int MaxStackable()
  {
    int num = this.info.stackable;
    if (this.parent != null && this.parent.maxStackSize > 0)
      num = Mathf.Min(this.parent.maxStackSize, num);
    object obj = Interface.CallHook("OnMaxStackable", (object) this);
    if (obj is int)
      return (int) obj;
    return num;
  }

  public BaseEntity.TraitFlag Traits
  {
    get
    {
      return this.info.Traits;
    }
  }

  public virtual Item Save(bool bIncludeContainer = false, bool bIncludeOwners = true)
  {
    this.dirty = false;
    Item obj = (Item) Pool.Get<Item>();
    obj.UID = (__Null) (int) this.uid;
    obj.itemid = (__Null) this.info.itemid;
    obj.slot = (__Null) this.position;
    obj.amount = (__Null) this.amount;
    obj.flags = (__Null) this.flags;
    obj.removetime = (__Null) (double) this.removeTime;
    obj.locktime = (__Null) (double) this.busyTime;
    obj.instanceData = (__Null) this.instanceData;
    obj.worldEntity = (__Null) (int) this.worldEnt.uid;
    obj.heldEntity = (__Null) (int) this.heldEntity.uid;
    obj.skinid = (__Null) (long) this.skin;
    obj.name = (__Null) this.name;
    obj.text = (__Null) this.text;
    if (this.hasCondition)
    {
      obj.conditionData = (__Null) Pool.Get<Item.ConditionData>();
      ((Item.ConditionData) obj.conditionData).maxCondition = (__Null) (double) this._maxCondition;
      ((Item.ConditionData) obj.conditionData).condition = (__Null) (double) this._condition;
    }
    if (this.contents != null & bIncludeContainer)
      obj.contents = (__Null) this.contents.Save();
    return obj;
  }

  public virtual void Load(Item load)
  {
    if (Object.op_Equality((Object) this.info, (Object) null) || this.info.itemid != load.itemid)
      this.info = ItemManager.FindItemDefinition((int) load.itemid);
    this.uid = (uint) load.UID;
    this.name = (string) load.name;
    this.text = (string) load.text;
    this.amount = (int) load.amount;
    this.position = (int) load.slot;
    this.busyTime = (float) load.locktime;
    this.removeTime = (float) load.removetime;
    this.flags = (Item.Flag) load.flags;
    this.worldEnt.uid = (uint) load.worldEntity;
    this.heldEntity.uid = (uint) load.heldEntity;
    if (this.instanceData != null)
    {
      this.instanceData.ShouldPool = (__Null) 1;
      this.instanceData.ResetToPool();
      this.instanceData = (Item.InstanceData) null;
    }
    this.instanceData = (Item.InstanceData) load.instanceData;
    if (this.instanceData != null)
      this.instanceData.ShouldPool = (__Null) 0;
    this.skin = (ulong) load.skinid;
    if (Object.op_Equality((Object) this.info, (Object) null) || this.info.itemid != load.itemid)
      this.info = ItemManager.FindItemDefinition((int) load.itemid);
    if (Object.op_Equality((Object) this.info, (Object) null))
      return;
    this._condition = 0.0f;
    this._maxCondition = 0.0f;
    if (load.conditionData != null)
    {
      this._condition = (float) ((Item.ConditionData) load.conditionData).condition;
      this._maxCondition = (float) ((Item.ConditionData) load.conditionData).maxCondition;
    }
    else if (this.info.condition.enabled)
    {
      this._condition = this.info.condition.max;
      this._maxCondition = this.info.condition.max;
    }
    if (load.contents != null)
    {
      if (this.contents == null)
      {
        this.contents = new ItemContainer();
        if (this.isServer)
          this.contents.ServerInitialize(this, (int) ((ItemContainer) load.contents).slots);
      }
      this.contents.Load((ItemContainer) load.contents);
    }
    if (!this.isServer)
      return;
    this.removeTime = 0.0f;
    this.OnItemCreated();
  }

  [System.Flags]
  public enum Flag
  {
    None = 0,
    Placeholder = 1,
    IsOn = 2,
    OnFire = 4,
    IsLocked = 8,
    Cooking = 16, // 0x00000010
  }
}
