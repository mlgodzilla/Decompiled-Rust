// Decompiled with JetBrains decompiler
// Type: DoorCloser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class DoorCloser : BaseEntity
{
  public float delay = 3f;
  [ItemSelector(ItemCategory.All)]
  public ItemDefinition itemType;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("DoorCloser.OnRpcMessage", 0.1f))
    {
      if (rpc == 342802563U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Take "));
          using (TimeWarning.New("RPC_Take", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Take", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Take(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Take");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override float BoundsPadding()
  {
    return 1f;
  }

  public void Think()
  {
    this.Invoke(new Action(this.SendClose), this.delay);
  }

  public void SendClose()
  {
    BaseEntity parentEntity = this.GetParentEntity();
    if (this.children != null)
    {
      foreach (Object child in this.children)
      {
        if (Object.op_Inequality(child, (Object) null))
        {
          this.Invoke(new Action(this.SendClose), this.delay);
          return;
        }
      }
    }
    if (!Object.op_Implicit((Object) parentEntity))
      return;
    ((Component) parentEntity).SendMessage("CloseRequest");
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  public void RPC_Take(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !rpc.player.CanBuild() || Interface.CallHook("ICanPickupEntity", (object) rpc.player, (object) this) != null)
      return;
    Door door = this.GetDoor();
    if (Object.op_Equality((Object) door, (Object) null) || !door.GetPlayerLockPermission(rpc.player))
      return;
    Item obj = ItemManager.Create(this.itemType, 1, this.skinID);
    if (obj != null)
      rpc.player.GiveItem(obj, BaseEntity.GiveItemReason.Generic);
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public Door GetDoor()
  {
    return this.GetParentEntity() as Door;
  }
}
