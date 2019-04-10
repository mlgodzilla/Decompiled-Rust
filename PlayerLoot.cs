// Decompiled with JetBrains decompiler
// Type: PlayerLoot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerLoot : EntityComponent<BasePlayer>
{
  public List<ItemContainer> containers = new List<ItemContainer>();
  public bool PositionChecks = true;
  public BaseEntity entitySource;
  public Item itemSource;
  private bool isInvokingSendUpdate;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("PlayerLoot.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool IsLooting()
  {
    return this.containers.Count > 0;
  }

  public void Clear()
  {
    if (!this.IsLooting())
      return;
    Interface.CallHook("OnPlayerLootEnd", (object) this);
    this.MarkDirty();
    if (Object.op_Implicit((Object) this.entitySource))
      ((Component) this.entitySource).SendMessage("PlayerStoppedLooting", (object) this.baseEntity, (SendMessageOptions) 1);
    foreach (ItemContainer container in this.containers)
    {
      if (container != null)
        container.onDirty -= new Action(this.MarkDirty);
    }
    this.containers.Clear();
    this.entitySource = (BaseEntity) null;
    this.itemSource = (Item) null;
  }

  public ItemContainer FindContainer(uint id)
  {
    this.Check();
    if (!this.IsLooting())
      return (ItemContainer) null;
    foreach (ItemContainer container1 in this.containers)
    {
      ItemContainer container2 = container1.FindContainer(id);
      if (container2 != null)
        return container2;
    }
    return (ItemContainer) null;
  }

  public Item FindItem(uint id)
  {
    this.Check();
    if (!this.IsLooting())
      return (Item) null;
    foreach (ItemContainer container in this.containers)
    {
      Item itemByUid = container.FindItemByUID(id);
      if (itemByUid != null && itemByUid.IsValid())
        return itemByUid;
    }
    return (Item) null;
  }

  public void Check()
  {
    if (!this.IsLooting() || !this.baseEntity.isServer)
      return;
    if (Object.op_Equality((Object) this.entitySource, (Object) null))
    {
      this.baseEntity.ChatMessage("Stopping Looting because lootable doesn't exist!");
      this.Clear();
    }
    else if (!this.entitySource.CanBeLooted(this.baseEntity))
    {
      this.Clear();
    }
    else
    {
      if (!this.PositionChecks || (double) this.entitySource.Distance(this.baseEntity.eyes.position) <= 2.09999990463257)
        return;
      this.Clear();
    }
  }

  public void MarkDirty()
  {
    if (this.isInvokingSendUpdate)
      return;
    this.isInvokingSendUpdate = true;
    this.Invoke(new Action(this.SendUpdate), 0.1f);
  }

  public void SendImmediate()
  {
    if (this.isInvokingSendUpdate)
    {
      this.isInvokingSendUpdate = false;
      this.CancelInvoke(new Action(this.SendUpdate));
    }
    this.SendUpdate();
  }

  private void SendUpdate()
  {
    this.isInvokingSendUpdate = false;
    if (!this.baseEntity.IsValid())
      return;
    using (PlayerUpdateLoot playerUpdateLoot = (PlayerUpdateLoot) Pool.Get<PlayerUpdateLoot>())
    {
      if (Object.op_Implicit((Object) this.entitySource) && this.entitySource.net != null)
        playerUpdateLoot.entityID = this.entitySource.net.ID;
      if (this.itemSource != null)
        playerUpdateLoot.itemID = (__Null) (int) this.itemSource.uid;
      if (this.containers.Count > 0)
      {
        playerUpdateLoot.containers = (__Null) Pool.Get<List<ItemContainer>>();
        foreach (ItemContainer container in this.containers)
          ((List<ItemContainer>) playerUpdateLoot.containers).Add(container.Save());
      }
      this.baseEntity.ClientRPCPlayer<PlayerUpdateLoot>((Connection) null, this.baseEntity, "UpdateLoot", playerUpdateLoot);
    }
  }

  public bool StartLootingEntity(BaseEntity targetEntity, bool doPositionChecks = true)
  {
    this.Clear();
    if (!Object.op_Implicit((Object) targetEntity) || !targetEntity.OnStartBeingLooted(this.baseEntity))
      return false;
    Assert.IsTrue(targetEntity.isServer, "Assure is server");
    this.PositionChecks = doPositionChecks;
    this.entitySource = targetEntity;
    this.itemSource = (Item) null;
    Interface.CallHook("OnLootEntity", (object) ((Component) this).GetComponent<BasePlayer>(), (object) targetEntity);
    this.MarkDirty();
    return true;
  }

  public void AddContainer(ItemContainer container)
  {
    if (container == null)
      return;
    this.containers.Add(container);
    container.onDirty += new Action(this.MarkDirty);
  }

  public void StartLootingItem(Item item)
  {
    this.Clear();
    if (item == null || item.contents == null)
      return;
    this.PositionChecks = true;
    this.containers.Add(item.contents);
    item.contents.onDirty += new Action(this.MarkDirty);
    this.itemSource = item;
    this.entitySource = item.GetWorldEntity();
    Interface.CallHook("OnLootItem", (object) ((Component) this).GetComponent<BasePlayer>(), (object) item);
    this.MarkDirty();
  }
}
