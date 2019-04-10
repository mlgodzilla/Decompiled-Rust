// Decompiled with JetBrains decompiler
// Type: BaseOven
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseOven : StorageContainer, ISplashable
{
  public bool allowByproductCreation = true;
  public bool disabledBySplash = true;
  public BaseOven.TemperatureType temperature;
  public BaseEntity.Menu.Option switchOnMenu;
  public BaseEntity.Menu.Option switchOffMenu;
  public ItemAmount[] startupContents;
  public ItemDefinition fuelType;
  public bool canModFire;
  private const float UpdateRate = 0.5f;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BaseOven.OnRpcMessage", 0.1f))
    {
      if (rpc == 4167839872U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SVSwitch "));
          using (TimeWarning.New("SVSwitch", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("SVSwitch", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SVSwitch(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SVSwitch");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    if (!this.IsOn())
      return;
    this.StartCooking();
  }

  public override void OnInventoryFirstCreated(ItemContainer container)
  {
    base.OnInventoryFirstCreated(container);
    if (this.startupContents == null)
      return;
    foreach (ItemAmount startupContent in this.startupContents)
      ItemManager.Create(startupContent.itemDef, (int) startupContent.amount, 0UL).MoveToContainer(container, -1, true);
  }

  public override void OnItemAddedOrRemoved(Item item, bool bAdded)
  {
    base.OnItemAddedOrRemoved(item, bAdded);
    if (item == null || !item.HasFlag(Item.Flag.OnFire))
      return;
    item.SetFlag(Item.Flag.OnFire, false);
    item.MarkDirty();
  }

  public void OvenFull()
  {
    this.StopCooking();
  }

  private Item FindBurnable()
  {
    object obj1 = Interface.CallHook("OnFindBurnable", (object) this);
    if (obj1 is Item)
      return (Item) obj1;
    if (this.inventory == null)
      return (Item) null;
    foreach (Item obj2 in this.inventory.itemList)
    {
      if (Object.op_Implicit((Object) ((Component) obj2.info).GetComponent<ItemModBurnable>()) && (Object.op_Equality((Object) this.fuelType, (Object) null) || Object.op_Equality((Object) obj2.info, (Object) this.fuelType)))
        return obj2;
    }
    return (Item) null;
  }

  public void Cook()
  {
    Item burnable = this.FindBurnable();
    if (burnable == null)
    {
      this.StopCooking();
    }
    else
    {
      this.inventory.OnCycle(0.5f);
      BaseEntity slot = this.GetSlot(BaseEntity.Slot.FireMod);
      if (Object.op_Implicit((Object) slot))
        ((Component) slot).SendMessage(nameof (Cook), (object) 0.5f, (SendMessageOptions) 1);
      ItemModBurnable component = (ItemModBurnable) ((Component) burnable.info).GetComponent<ItemModBurnable>();
      burnable.fuel -= (float) (0.5 * ((double) this.cookingTemperature / 200.0));
      if (!burnable.HasFlag(Item.Flag.OnFire))
      {
        burnable.SetFlag(Item.Flag.OnFire, true);
        burnable.MarkDirty();
      }
      if ((double) burnable.fuel > 0.0)
        return;
      this.ConsumeFuel(burnable, component);
    }
  }

  private void ConsumeFuel(Item fuel, ItemModBurnable burnable)
  {
    Interface.CallHook("OnConsumeFuel", (object) this, (object) fuel, (object) burnable);
    if (this.allowByproductCreation && Object.op_Inequality((Object) burnable.byproductItem, (Object) null) && (double) Random.Range(0.0f, 1f) > (double) burnable.byproductChance)
    {
      Item obj = ItemManager.Create(burnable.byproductItem, burnable.byproductAmount, 0UL);
      if (!obj.MoveToContainer(this.inventory, -1, true))
      {
        this.OvenFull();
        obj.Drop(this.inventory.dropPosition, this.inventory.dropVelocity, (Quaternion) null);
      }
    }
    if (fuel.amount <= 1)
    {
      fuel.Remove(0.0f);
    }
    else
    {
      --fuel.amount;
      fuel.fuel = burnable.fuelAmount;
      fuel.MarkDirty();
    }
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void SVSwitch(BaseEntity.RPCMessage msg)
  {
    bool flag = msg.read.Bit();
    if (Interface.CallHook("OnOvenToggle", (object) this, (object) msg.player) != null || flag == this.IsOn() || this.needsBuildingPrivilegeToUse && !msg.player.CanBuild())
      return;
    if (flag)
      this.StartCooking();
    else
      this.StopCooking();
  }

  public float cookingTemperature
  {
    get
    {
      switch (this.temperature)
      {
        case BaseOven.TemperatureType.Warming:
          return 50f;
        case BaseOven.TemperatureType.Cooking:
          return 200f;
        case BaseOven.TemperatureType.Smelting:
          return 1000f;
        case BaseOven.TemperatureType.Fractioning:
          return 1500f;
        default:
          return 15f;
      }
    }
  }

  public void UpdateAttachmentTemperature()
  {
    BaseEntity slot = this.GetSlot(BaseEntity.Slot.FireMod);
    if (!Object.op_Implicit((Object) slot))
      return;
    ((Component) slot).SendMessage("ParentTemperatureUpdate", (object) this.inventory.temperature, (SendMessageOptions) 1);
  }

  public virtual void StartCooking()
  {
    if (this.FindBurnable() == null)
      return;
    this.inventory.temperature = this.cookingTemperature;
    this.UpdateAttachmentTemperature();
    this.InvokeRepeating(new Action(this.Cook), 0.5f, 0.5f);
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
  }

  public virtual void StopCooking()
  {
    this.UpdateAttachmentTemperature();
    if (this.inventory != null)
    {
      this.inventory.temperature = 15f;
      foreach (Item obj in this.inventory.itemList)
      {
        if (obj.HasFlag(Item.Flag.OnFire))
        {
          obj.SetFlag(Item.Flag.OnFire, false);
          obj.MarkDirty();
        }
      }
    }
    this.CancelInvoke(new Action(this.Cook));
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }

  public bool wantsSplash(ItemDefinition splashType, int amount)
  {
    if (this.IsOn())
      return this.disabledBySplash;
    return false;
  }

  public int DoSplash(ItemDefinition splashType, int amount)
  {
    this.StopCooking();
    return Mathf.Min(200, amount);
  }

  public override bool HasSlot(BaseEntity.Slot slot)
  {
    if (this.canModFire && slot == BaseEntity.Slot.FireMod)
      return true;
    return base.HasSlot(slot);
  }

  public enum TemperatureType
  {
    Normal,
    Warming,
    Cooking,
    Smelting,
    Fractioning,
  }
}
