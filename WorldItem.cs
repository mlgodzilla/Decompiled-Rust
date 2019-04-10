// Decompiled with JetBrains decompiler
// Type: WorldItem
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

public class WorldItem : BaseEntity
{
  [Header("WorldItem")]
  public bool allowPickup = true;
  protected float eatSeconds = 10f;
  protected float caloriesPerSecond = 1f;
  [NonSerialized]
  public Item item;
  private bool _isInvokingSendItemUpdate;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("WorldItem.OnRpcMessage", 0.1f))
    {
      if (rpc == 2778075470U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Pickup "));
          using (TimeWarning.New("Pickup", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("Pickup", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.Pickup(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in Pickup");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override Item GetItem()
  {
    return this.item;
  }

  public void InitializeItem(Item in_item)
  {
    if (this.item != null)
      this.RemoveItem();
    this.item = in_item;
    if (this.item == null)
      return;
    this.item.OnDirty += new Action<Item>(this.OnItemDirty);
    ((Object) this).set_name(this.item.info.shortname + " (world)");
    this.item.SetWorldEntity((BaseEntity) this);
    this.OnItemDirty(this.item);
  }

  public void RemoveItem()
  {
    if (this.item == null)
      return;
    this.item.OnDirty -= new Action<Item>(this.OnItemDirty);
    this.item = (Item) null;
  }

  public void DestroyItem()
  {
    if (this.item == null)
      return;
    this.item.OnDirty -= new Action<Item>(this.OnItemDirty);
    this.item.Remove(0.0f);
    this.item = (Item) null;
  }

  protected virtual void OnItemDirty(Item in_item)
  {
    Assert.IsTrue(this.item == in_item, "WorldItem:OnItemDirty - dirty item isn't ours!");
    if (this.item != null)
      ((Component) this).BroadcastMessage("OnItemChanged", (object) this.item, (SendMessageOptions) 1);
    this.DoItemNetworking();
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.worldItem == null || ((WorldItem) info.msg.worldItem).item == null)
      return;
    Item in_item = ItemManager.Load((Item) ((WorldItem) info.msg.worldItem).item, this.item, this.isServer);
    if (in_item == null)
      return;
    this.InitializeItem(in_item);
  }

  public override BaseEntity.TraitFlag Traits
  {
    get
    {
      if (this.item != null)
        return this.item.Traits;
      return base.Traits;
    }
  }

  public override void Eat(BaseNpc baseNpc, float timeSpent)
  {
    if ((double) this.eatSeconds <= 0.0)
      return;
    this.eatSeconds -= timeSpent;
    baseNpc.AddCalories(this.caloriesPerSecond * timeSpent);
    if ((double) this.eatSeconds >= 0.0)
      return;
    this.DestroyItem();
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override string ToString()
  {
    if (this._name == null)
    {
      if (this.isServer)
        this._name = string.Format("{1}[{0}] {2}", (object) (uint) (this.net != null ? (int) this.net.ID : 0), (object) this.ShortPrefabName, (object) ((Object) this).get_name());
      else
        this._name = this.ShortPrefabName;
    }
    return this._name;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (this.item == null)
      return;
    ((Component) this).BroadcastMessage("OnItemChanged", (object) this.item, (SendMessageOptions) 1);
  }

  private void DoItemNetworking()
  {
    if (this._isInvokingSendItemUpdate)
      return;
    this._isInvokingSendItemUpdate = true;
    this.Invoke(new Action(this.SendItemUpdate), 0.1f);
  }

  private void SendItemUpdate()
  {
    this._isInvokingSendItemUpdate = false;
    if (this.item == null)
      return;
    using (UpdateItem updateItem = (UpdateItem) Pool.Get<UpdateItem>())
    {
      updateItem.item = (__Null) this.item.Save(false, false);
      this.ClientRPC<UpdateItem>((Connection) null, "UpdateItem", updateItem);
    }
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void Pickup(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract() || this.item == null || (!this.allowPickup || Interface.CallHook("OnItemPickup", (object) this.item, (object) msg.player) != null))
      return;
    this.ClientRPC((Connection) null, "PickupSound");
    msg.player.GiveItem(this.item, BaseEntity.GiveItemReason.PickedUp);
    msg.player.SignalBroadcast(BaseEntity.Signal.Gesture, "pickup_item", (Connection) null);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (this.item == null)
      return;
    bool forDisk = info.forDisk;
    info.msg.worldItem = (__Null) Pool.Get<WorldItem>();
    ((WorldItem) info.msg.worldItem).item = (__Null) this.item.Save(forDisk, false);
  }

  public override void OnInvalidPosition()
  {
    this.DestroyItem();
    base.OnInvalidPosition();
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    this.RemoveItem();
  }

  public override void SwitchParent(BaseEntity ent)
  {
    this.SetParent(ent, this.parentBone, false, false);
  }
}
