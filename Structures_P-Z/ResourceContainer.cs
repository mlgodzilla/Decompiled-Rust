// Decompiled with JetBrains decompiler
// Type: ResourceContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ResourceContainer : EntityComponent<BaseEntity>
{
  public bool lootable = true;
  [NonSerialized]
  public ItemContainer container;
  [NonSerialized]
  public float lastAccessTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("ResourceContainer.OnRpcMessage", 0.1f))
    {
      if (rpc == 548378753U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - StartLootingContainer "));
          using (TimeWarning.New("StartLootingContainer", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("StartLootingContainer", this.GetBaseEntity(), player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.StartLootingContainer(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in StartLootingContainer");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public int accessedSecondsAgo
  {
    get
    {
      return (int) ((double) Time.get_realtimeSinceStartup() - (double) this.lastAccessTime);
    }
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  private void StartLootingContainer(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!Object.op_Implicit((Object) player) || !player.CanInteract() || (!this.lootable || Interface.CallHook("CanLootEntity", (object) player, (object) this) != null) || !player.inventory.loot.StartLootingEntity(this.baseEntity, true))
      return;
    this.lastAccessTime = Time.get_realtimeSinceStartup();
    player.inventory.loot.AddContainer(this.container);
  }
}
