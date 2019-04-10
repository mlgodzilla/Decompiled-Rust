// Decompiled with JetBrains decompiler
// Type: StashContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class StashContainer : StorageContainer
{
  public float uncoverRange = 3f;
  public Transform visuals;
  public float burriedOffset;
  public float raisedOffset;
  public GameObjectRef buryEffect;
  private float lastToggleTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("StashContainer.OnRpcMessage", 0.1f))
    {
      if (rpc == 4130263076U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_HideStash "));
        using (TimeWarning.New("RPC_HideStash", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_HideStash(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_HideStash");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 298671803U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_WantsUnhide "));
          using (TimeWarning.New("RPC_WantsUnhide", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_WantsUnhide(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_WantsUnhide");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool PlayerInRange(BasePlayer ply)
  {
    if ((double) Vector3.Distance(((Component) this).get_transform().get_position(), ((Component) ply).get_transform().get_position()) <= (double) this.uncoverRange)
    {
      Vector3 vector3 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), ply.eyes.position);
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      if ((double) Vector3.Dot(ply.eyes.BodyForward(), normalized) > 0.949999988079071)
        return true;
    }
    return false;
  }

  public void SetHidden(bool isHidden)
  {
    if ((double) Time.get_realtimeSinceStartup() - (double) this.lastToggleTime < 3.0 || isHidden == this.HasFlag(BaseEntity.Flags.Reserved5))
      return;
    this.lastToggleTime = Time.get_realtimeSinceStartup();
    this.Invoke(new Action(this.Decay), 259200f);
    if (!this.isServer)
      return;
    this.SetFlag(BaseEntity.Flags.Reserved5, isHidden, false, true);
  }

  public void DisableNetworking()
  {
    this.limitNetworking = true;
    this.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
  }

  public void Decay()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.SetHidden(false);
  }

  public void ToggleHidden()
  {
    this.SetHidden(!this.IsHidden());
  }

  [BaseEntity.RPC_Server]
  public void RPC_HideStash(BaseEntity.RPCMessage rpc)
  {
    if (Interface.CallHook("CanHideStash", (object) rpc.player, (object) this) != null)
      return;
    this.SetHidden(true);
  }

  [BaseEntity.RPC_Server]
  public void RPC_WantsUnhide(BaseEntity.RPCMessage rpc)
  {
    if (!this.IsHidden() || Interface.CallHook("CanSeeStash", (object) rpc.player, (object) this) != null || !this.PlayerInRange(rpc.player))
      return;
    this.SetHidden(false);
  }

  public bool IsHidden()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved5);
  }

  public static class StashContainerFlags
  {
    public const BaseEntity.Flags Hidden = BaseEntity.Flags.Reserved5;
  }
}
