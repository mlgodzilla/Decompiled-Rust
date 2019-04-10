// Decompiled with JetBrains decompiler
// Type: KeyLock
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

public class KeyLock : BaseLock
{
  [ItemSelector(ItemCategory.All)]
  public ItemDefinition keyItemType;
  public int keyCode;
  public bool firstKeyCreated;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("KeyLock.OnRpcMessage", 0.1f))
    {
      if (rpc == 4135414453U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_CreateKey "));
        using (TimeWarning.New("RPC_CreateKey", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_CreateKey", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_CreateKey(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_CreateKey");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 954115386U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Lock "));
        using (TimeWarning.New("RPC_Lock", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Lock", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_Lock(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_Lock");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1663222372U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Unlock "));
          using (TimeWarning.New("RPC_Unlock", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Unlock", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Unlock(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Unlock");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override bool HasLockPermission(BasePlayer player)
  {
    if (player.IsDead())
      return false;
    if ((long) player.userID == (long) this.OwnerID)
      return true;
    foreach (Item itemId in player.inventory.FindItemIDs(this.keyItemType.itemid))
    {
      if (this.CanKeyUnlockUs(itemId))
        return true;
    }
    return false;
  }

  private bool CanKeyUnlockUs(Item key)
  {
    return key.instanceData != null && key.instanceData.dataInt == this.keyCode;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.keyLock == null)
      return;
    this.keyCode = (int) ((KeyLock) info.msg.keyLock).code;
  }

  public override bool ShouldNetworkOwnerInfo()
  {
    return true;
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    if (this.OwnerID != 0UL || !Object.op_Implicit((Object) this.GetParentEntity()))
      return;
    this.OwnerID = this.GetParentEntity().OwnerID;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.keyLock = (__Null) Pool.Get<KeyLock>();
    ((KeyLock) info.msg.keyLock).code = (__Null) this.keyCode;
  }

  public override void OnDeployed(BaseEntity parent)
  {
    base.OnDeployed(parent);
    this.keyCode = Random.Range(1, 100000);
  }

  public override bool OnTryToOpen(BasePlayer player)
  {
    object obj = Interface.CallHook("CanUseLockedEntity", (object) player, (object) this);
    if (obj is bool)
      return (bool) obj;
    if (this.HasLockPermission(player))
      return true;
    return !this.IsLocked();
  }

  public override bool OnTryToClose(BasePlayer player)
  {
    object obj = Interface.CallHook("CanUseLockedEntity", (object) player, (object) this);
    if (obj is bool)
      return (bool) obj;
    if (this.HasLockPermission(player))
      return true;
    return !this.IsLocked();
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void RPC_Unlock(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.IsLocked() || (Interface.CallHook("CanUnlock", (object) rpc.player, (object) this) != null || !this.HasLockPermission(rpc.player)))
      return;
    this.SetFlag(BaseEntity.Flags.Locked, false, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  private void RPC_Lock(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || this.IsLocked() || (Interface.CallHook("CanLock", (object) rpc.player, (object) this) != null || !this.HasLockPermission(rpc.player)))
      return;
    this.LockLock(rpc.player);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  private void RPC_CreateKey(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || this.IsLocked() && !this.HasLockPermission(rpc.player))
      return;
    ItemDefinition itemDefinition = ItemManager.FindItemDefinition(this.keyItemType.itemid);
    if (Object.op_Equality((Object) itemDefinition, (Object) null))
    {
      Debug.LogWarning((object) ("RPC_CreateKey: Itemdef is missing! " + (object) this.keyItemType));
    }
    else
    {
      ItemBlueprint blueprint = ItemManager.FindBlueprint(itemDefinition);
      if (!rpc.player.inventory.crafting.CanCraft(blueprint, 1))
        return;
      Item.InstanceData instanceData = (Item.InstanceData) Pool.Get<Item.InstanceData>();
      instanceData.dataInt = (__Null) this.keyCode;
      rpc.player.inventory.crafting.CraftItem(blueprint, rpc.player, instanceData, 1, 0, (Item) null);
      if (this.firstKeyCreated)
        return;
      this.LockLock(rpc.player);
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      this.firstKeyCreated = true;
    }
  }

  public void LockLock(BasePlayer player)
  {
    this.SetFlag(BaseEntity.Flags.Locked, true, false, true);
    if (!player.IsValid())
      return;
    player.GiveAchievement("LOCK_LOCK");
  }
}
