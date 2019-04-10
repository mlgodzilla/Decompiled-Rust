// Decompiled with JetBrains decompiler
// Type: BaseLock
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseLock : BaseEntity
{
  [ItemSelector(ItemCategory.All)]
  public ItemDefinition itemType;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BaseLock.OnRpcMessage", 0.1f))
    {
      if (rpc == 3572556655U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_TakeLock "));
          using (TimeWarning.New("RPC_TakeLock", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_TakeLock", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_TakeLock(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_TakeLock");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public virtual bool GetPlayerLockPermission(BasePlayer player)
  {
    return this.OnTryToOpen(player);
  }

  public virtual bool OnTryToOpen(BasePlayer player)
  {
    return !this.IsLocked();
  }

  public virtual bool OnTryToClose(BasePlayer player)
  {
    return true;
  }

  public virtual bool HasLockPermission(BasePlayer player)
  {
    return true;
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  public void RPC_TakeLock(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || this.IsLocked() || Interface.CallHook("CanPickupLock", (object) rpc.player, (object) this) != null)
      return;
    Item obj = ItemManager.Create(this.itemType, 1, this.skinID);
    if (obj != null)
      rpc.player.GiveItem(obj, BaseEntity.GiveItemReason.Generic);
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override float BoundsPadding()
  {
    return 2f;
  }
}
