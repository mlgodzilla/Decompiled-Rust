﻿// Decompiled with JetBrains decompiler
// Type: CustomDoorManipulator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class CustomDoorManipulator : DoorManipulator
{
  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("CustomDoorManipulator.OnRpcMessage", 0.1f))
    {
      if (rpc == 1224330484U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoPair "));
        using (TimeWarning.New("DoPair", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("DoPair", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.DoPair(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in DoPair");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3800726972U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ServerActionChange "));
          using (TimeWarning.New("ServerActionChange", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("ServerActionChange", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.ServerActionChange(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in ServerActionChange");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override bool PairWithLockedDoors()
  {
    return false;
  }

  public bool CanPlayerAdmin(BasePlayer player)
  {
    if (Object.op_Inequality((Object) player, (Object) null) && player.CanBuild())
      return !this.IsOn();
    return false;
  }

  public bool IsPaired()
  {
    return Object.op_Inequality((Object) this.targetDoor, (Object) null);
  }

  public void RefreshDoor()
  {
    this.SetTargetDoor(this.targetDoor);
  }

  private void OnPhysicsNeighbourChanged()
  {
    this.SetTargetDoor(this.targetDoor);
    this.Invoke(new Action(this.RefreshDoor), 0.1f);
  }

  public override void SetupInitialDoorConnection()
  {
    if (!this.entityRef.IsValid(true) || !Object.op_Equality((Object) this.targetDoor, (Object) null))
      return;
    this.SetTargetDoor((Door) ((Component) this.entityRef.Get(true)).GetComponent<Door>());
  }

  public override void DoActionDoorMissing()
  {
    this.SetTargetDoor((Door) null);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void DoPair(BaseEntity.RPCMessage msg)
  {
    Door targetDoor = this.targetDoor;
    Door door = this.FindDoor(this.PairWithLockedDoors());
    if (!Object.op_Inequality((Object) door, (Object) targetDoor))
      return;
    this.SetTargetDoor(door);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void ServerActionChange(BaseEntity.RPCMessage msg)
  {
  }
}
