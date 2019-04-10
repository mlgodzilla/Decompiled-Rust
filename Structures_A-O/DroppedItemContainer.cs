// Decompiled with JetBrains decompiler
// Type: DroppedItemContainer
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

public class DroppedItemContainer : BaseCombatEntity
{
  public string lootPanelName = "generic";
  [NonSerialized]
  public ulong playerSteamID;
  [NonSerialized]
  public string _playerName;
  public ItemContainer inventory;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("DroppedItemContainer.OnRpcMessage", 0.1f))
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

  public string playerName
  {
    get
    {
      return this._playerName;
    }
    set
    {
      this._playerName = value;
    }
  }

  public override bool OnStartBeingLooted(BasePlayer baseEntity)
  {
    if (baseEntity.InSafeZone() && (long) baseEntity.userID != (long) this.playerSteamID)
      return false;
    return base.OnStartBeingLooted(baseEntity);
  }

  public override void ServerInit()
  {
    this.ResetRemovalTime();
    base.ServerInit();
  }

  public void RemoveMe()
  {
    if (this.IsOpen())
      this.ResetRemovalTime();
    else
      this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public void ResetRemovalTime(float dur)
  {
    using (TimeWarning.New(nameof (ResetRemovalTime), 0.1f))
      this.Invoke(new Action(this.RemoveMe), dur);
  }

  public void ResetRemovalTime()
  {
    this.ResetRemovalTime(this.CalculateRemovalTime());
  }

  public float CalculateRemovalTime()
  {
    int num = 1;
    if (this.inventory != null)
    {
      foreach (Item obj in this.inventory.itemList)
        num = Mathf.Max(num, obj.despawnMultiplier);
    }
    return (float) num * Server.itemdespawn;
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    if (this.inventory == null)
      return;
    this.inventory.Kill();
    this.inventory = (ItemContainer) null;
  }

  public void TakeFrom(params ItemContainer[] source)
  {
    Assert.IsTrue(this.inventory == null, "Initializing Twice");
    using (TimeWarning.New("DroppedItemContainer.TakeFrom", 0.1f))
    {
      int iMaxCapacity = 0;
      foreach (ItemContainer itemContainer in source)
        iMaxCapacity += itemContainer.itemList.Count;
      this.inventory = new ItemContainer();
      this.inventory.ServerInitialize((Item) null, iMaxCapacity);
      this.inventory.GiveUID();
      this.inventory.entityOwner = (BaseEntity) this;
      this.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
      foreach (ItemContainer itemContainer in source)
      {
        foreach (Item obj in itemContainer.itemList.ToArray())
        {
          if (!obj.MoveToContainer(this.inventory, -1, true))
            obj.Remove(0.0f);
        }
      }
      this.ResetRemovalTime();
    }
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  private void RPC_OpenLoot(BaseEntity.RPCMessage rpc)
  {
    if (this.inventory == null)
      return;
    BasePlayer player = rpc.player;
    if (!Object.op_Implicit((Object) player) || !player.CanInteract() || (Interface.CallHook("CanLootEntity", (object) player, (object) this) != null || !player.inventory.loot.StartLootingEntity((BaseEntity) this, true)))
      return;
    this.SetFlag(BaseEntity.Flags.Open, true, false, true);
    player.inventory.loot.AddContainer(this.inventory);
    player.inventory.loot.SendImmediate();
    player.ClientRPCPlayer<string>((Connection) null, player, "RPC_OpenLootPanel", this.lootPanelName);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void PlayerStoppedLooting(BasePlayer player)
  {
    if (this.inventory == null || this.inventory.itemList == null || this.inventory.itemList.Count == 0)
    {
      this.Kill(BaseNetworkable.DestroyMode.None);
    }
    else
    {
      this.ResetRemovalTime();
      this.SetFlag(BaseEntity.Flags.Open, false, false, true);
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
  }

  public override void PreServerLoad()
  {
    base.PreServerLoad();
    this.inventory = new ItemContainer();
    this.inventory.entityOwner = (BaseEntity) this;
    this.inventory.ServerInitialize((Item) null, 0);
    this.inventory.SetFlag(ItemContainer.Flag.NoItemInput, true);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.lootableCorpse = (__Null) Pool.Get<LootableCorpse>();
    ((LootableCorpse) info.msg.lootableCorpse).playerName = (__Null) this.playerName;
    ((LootableCorpse) info.msg.lootableCorpse).playerID = (__Null) (long) this.playerSteamID;
    if (!info.forDisk)
      return;
    if (this.inventory != null)
    {
      info.msg.storageBox = (__Null) Pool.Get<StorageBox>();
      ((StorageBox) info.msg.storageBox).contents = (__Null) this.inventory.Save();
    }
    else
      Debug.LogWarning((object) ("Dropped item container without inventory: " + ((object) this).ToString()));
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.lootableCorpse != null)
    {
      this.playerName = (string) ((LootableCorpse) info.msg.lootableCorpse).playerName;
      this.playerSteamID = (ulong) ((LootableCorpse) info.msg.lootableCorpse).playerID;
    }
    if (info.msg.storageBox == null)
      return;
    if (this.inventory != null)
      this.inventory.Load((ItemContainer) ((StorageBox) info.msg.storageBox).contents);
    else
      Debug.LogWarning((object) ("Dropped item container without inventory: " + ((object) this).ToString()));
  }
}
