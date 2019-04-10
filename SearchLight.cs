// Decompiled with JetBrains decompiler
// Type: SearchLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class SearchLight : StorageContainer
{
  public Vector3 aimDir = Vector3.get_zero();
  public GameObject pitchObject;
  public GameObject yawObject;
  public GameObject eyePoint;
  public GameObject lightEffect;
  public SoundPlayer turnLoop;
  public ItemDefinition fuelType;
  public BasePlayer mountedPlayer;
  public float secondsRemaining;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("SearchLight.OnRpcMessage", 0.1f))
    {
      if (rpc == 3043863856U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Switch "));
        using (TimeWarning.New("RPC_Switch", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Switch", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_Switch(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_Switch");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3611615802U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_UseLight "));
          using (TimeWarning.New("RPC_UseLight", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_UseLight", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_UseLight(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_UseLight");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ResetState()
  {
    this.aimDir = Vector3.get_zero();
  }

  public bool IsMounted()
  {
    return Object.op_Inequality((Object) this.mountedPlayer, (Object) null);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.autoturret = (__Null) new AutoTurret();
    ((AutoTurret) info.msg.autoturret).aimDir = (__Null) this.aimDir;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.autoturret == null)
      return;
    this.aimDir = (Vector3) ((AutoTurret) info.msg.autoturret).aimDir;
  }

  public void PlayerEnter(BasePlayer player)
  {
    if (this.IsMounted() && Object.op_Inequality((Object) player, (Object) this.mountedPlayer))
      return;
    this.PlayerExit();
    if (!Object.op_Inequality((Object) player, (Object) null))
      return;
    this.mountedPlayer = player;
    this.SetFlag(BaseEntity.Flags.Reserved5, true, false, true);
  }

  public void PlayerExit()
  {
    if (Object.op_Implicit((Object) this.mountedPlayer))
      this.mountedPlayer = (BasePlayer) null;
    this.SetFlag(BaseEntity.Flags.Reserved5, false, false, true);
  }

  public void MountedUpdate()
  {
    if (Object.op_Equality((Object) this.mountedPlayer, (Object) null) || this.mountedPlayer.IsSleeping() || (!this.mountedPlayer.IsAlive() || this.mountedPlayer.IsWounded()) || (double) Vector3.Distance(((Component) this.mountedPlayer).get_transform().get_position(), ((Component) this).get_transform().get_position()) > 2.0)
    {
      this.PlayerExit();
    }
    else
    {
      this.SetTargetAimpoint(Vector3.op_Addition(this.eyePoint.get_transform().get_position(), Vector3.op_Multiply(this.mountedPlayer.eyes.BodyForward(), 100f)));
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
  }

  public void SetTargetAimpoint(Vector3 worldPos)
  {
    Vector3 vector3 = Vector3.op_Subtraction(worldPos, this.eyePoint.get_transform().get_position());
    this.aimDir = ((Vector3) ref vector3).get_normalized();
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  public void RPC_UseLight(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    bool flag = msg.read.Bit();
    if (flag && this.IsMounted() || this.needsBuildingPrivilegeToUse && !msg.player.CanBuild())
      return;
    if (flag)
      this.PlayerEnter(player);
    else
      this.PlayerExit();
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  public void RPC_Switch(BaseEntity.RPCMessage msg)
  {
    bool b = msg.read.Bit();
    if (this.needsBuildingPrivilegeToUse && !msg.player.CanBuild())
      return;
    this.SetFlag(BaseEntity.Flags.On, b, false, true);
    this.FuelUpdate();
  }

  public override void OnKilled(HitInfo info)
  {
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    base.OnKilled(info);
  }

  public void FuelUpdate()
  {
    if (!this.IsOn())
      return;
    this.secondsRemaining -= Time.get_deltaTime();
    if ((double) this.secondsRemaining > 0.0)
      return;
    Item slot = this.inventory.GetSlot(0);
    if (slot == null || Object.op_Inequality((Object) slot.info, (Object) this.inventory.onlyAllowedItem))
    {
      this.SetFlag(BaseEntity.Flags.On, false, false, true);
    }
    else
    {
      slot.UseItem(1);
      this.secondsRemaining += 20f;
    }
  }

  public void Update()
  {
    if (!this.isServer)
      return;
    if (this.IsMounted())
      this.MountedUpdate();
    this.FuelUpdate();
  }

  public static class SearchLightFlags
  {
    public const BaseEntity.Flags PlayerUsing = BaseEntity.Flags.Reserved5;
  }
}
