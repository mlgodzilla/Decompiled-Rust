// Decompiled with JetBrains decompiler
// Type: LootableCorpse
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

public class LootableCorpse : BaseCorpse
{
  public string lootPanelName = "generic";
  [NonSerialized]
  public ulong playerSteamID;
  [NonSerialized]
  public string _playerName;
  [NonSerialized]
  public ItemContainer[] containers;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("LootableCorpse.OnRpcMessage", 0.1f))
    {
      if (rpc == 2278459738U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_LootCorpse "));
          using (TimeWarning.New("RPC_LootCorpse", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_LootCorpse", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_LootCorpse(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_LootCorpse");
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

  public override void ServerInit()
  {
    base.ServerInit();
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    this.DropItems();
    if (this.containers != null)
    {
      foreach (ItemContainer container in this.containers)
        container.Kill();
    }
    this.containers = (ItemContainer[]) null;
  }

  public void TakeFrom(params ItemContainer[] source)
  {
    Assert.IsTrue(this.containers == null, "Initializing Twice");
    using (TimeWarning.New("Corpse.TakeFrom", 0.1f))
    {
      this.containers = new ItemContainer[source.Length];
      for (int index = 0; index < source.Length; ++index)
      {
        this.containers[index] = new ItemContainer();
        this.containers[index].ServerInitialize((Item) null, source[index].capacity);
        this.containers[index].GiveUID();
        this.containers[index].entityOwner = (BaseEntity) this;
        foreach (Item obj in source[index].itemList.ToArray())
        {
          if (!obj.MoveToContainer(this.containers[index], -1, true))
            obj.Remove(0.0f);
        }
      }
      this.ResetRemovalTime();
    }
  }

  public override bool CanRemove()
  {
    return !this.IsOpen();
  }

  public virtual bool CanLoot()
  {
    return true;
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  private void RPC_LootCorpse(BaseEntity.RPCMessage rpc)
  {
    BasePlayer player = rpc.player;
    if (!Object.op_Implicit((Object) player) || !player.CanInteract() || (!this.CanLoot() || this.containers == null) || (Interface.CallHook("CanLootEntity", (object) player, (object) this) != null || !player.inventory.loot.StartLootingEntity((BaseEntity) this, true)))
      return;
    this.SetFlag(BaseEntity.Flags.Open, true, false, true);
    foreach (ItemContainer container in this.containers)
      player.inventory.loot.AddContainer(container);
    player.inventory.loot.SendImmediate();
    this.ClientRPCPlayer((Connection) null, player, "RPC_ClientLootCorpse");
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void PlayerStoppedLooting(BasePlayer player)
  {
    Interface.CallHook("OnLootEntityEnd", (object) player, (object) this);
    this.ResetRemovalTime();
    this.SetFlag(BaseEntity.Flags.Open, false, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void DropItems()
  {
    if (this.containers == null)
      return;
    DroppedItemContainer droppedItemContainer = ItemContainer.Drop("assets/prefabs/misc/item drop/item_drop_backpack.prefab", ((Component) this).get_transform().get_position(), Quaternion.get_identity(), this.containers);
    if (!Object.op_Inequality((Object) droppedItemContainer, (Object) null))
      return;
    droppedItemContainer.playerName = this.playerName;
    droppedItemContainer.playerSteamID = this.playerSteamID;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.lootableCorpse = (__Null) Pool.Get<LootableCorpse>();
    ((LootableCorpse) info.msg.lootableCorpse).playerName = (__Null) this.playerName;
    ((LootableCorpse) info.msg.lootableCorpse).playerID = (__Null) (long) this.playerSteamID;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.lootableCorpse == null)
      return;
    this.playerName = (string) ((LootableCorpse) info.msg.lootableCorpse).playerName;
    this.playerSteamID = (ulong) ((LootableCorpse) info.msg.lootableCorpse).playerID;
  }
}
