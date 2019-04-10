// Decompiled with JetBrains decompiler
// Type: Signage
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

public class Signage : BaseCombatEntity, ILOD
{
  public GameObjectRef changeTextDialog;
  public MeshPaintableSource paintableSource;
  [NonSerialized]
  public uint textureID;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Signage.OnRpcMessage", 0.1f))
    {
      if (rpc == 1455609404U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - LockSign "));
        using (TimeWarning.New("LockSign", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("LockSign", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.LockSign(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in LockSign");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 4149904254U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - UnLockSign "));
        using (TimeWarning.New("UnLockSign", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("UnLockSign", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.UnLockSign(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in UnLockSign");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1255380462U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - UpdateSign "));
          using (TimeWarning.New("UpdateSign", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("UpdateSign", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.UpdateSign(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in UpdateSign");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public virtual bool CanUpdateSign(BasePlayer player)
  {
    object obj = Interface.CallHook(nameof (CanUpdateSign), (object) player, (object) this);
    if (obj is bool)
      return (bool) obj;
    if (player.IsAdmin || player.IsDeveloper)
      return true;
    if (!player.CanBuild())
      return false;
    if (this.IsLocked())
      return (long) player.userID == (long) this.OwnerID;
    return true;
  }

  public bool CanUnlockSign(BasePlayer player)
  {
    if (!this.IsLocked())
      return false;
    return this.CanUpdateSign(player);
  }

  public bool CanLockSign(BasePlayer player)
  {
    if (this.IsLocked())
      return false;
    return this.CanUpdateSign(player);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.sign != null && ((Sign) info.msg.sign).imageid != (int) this.textureID)
      this.textureID = (uint) ((Sign) info.msg.sign).imageid;
    if (!this.isServer)
      return;
    if (this.textureID != 0U && FileStorage.server.Get(this.textureID, FileStorage.Type.png, (uint) this.net.ID) == null)
      this.textureID = 0U;
    if (this.textureID != 0U)
      return;
    this.SetFlag(BaseEntity.Flags.Locked, false, false, true);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  public void LockSign(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract() || !this.CanUpdateSign(msg.player))
      return;
    this.SetFlag(BaseEntity.Flags.Locked, true, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    this.OwnerID = msg.player.userID;
    Interface.CallHook("OnSignLocked", (object) this, (object) msg.player);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  public void UnLockSign(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract() || !this.CanUnlockSign(msg.player))
      return;
    this.SetFlag(BaseEntity.Flags.Locked, false, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.sign = (__Null) Pool.Get<Sign>();
    ((Sign) info.msg.sign).imageid = (__Null) (int) this.textureID;
  }

  public override void OnKilled(HitInfo info)
  {
    if (this.net != null)
      FileStorage.server.RemoveAllByEntity((uint) this.net.ID);
    this.textureID = 0U;
    base.OnKilled(info);
  }

  public override bool ShouldNetworkOwnerInfo()
  {
    return true;
  }

  public override string Categorize()
  {
    return "sign";
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  public void UpdateSign(BaseEntity.RPCMessage msg)
  {
    if (!this.CanUpdateSign(msg.player))
      return;
    byte[] data = msg.read.BytesWithSize();
    if (data == null || !ImageProcessing.IsValidPNG(data, 1024, 1024))
      return;
    FileStorage.server.RemoveAllByEntity((uint) this.net.ID);
    this.textureID = FileStorage.server.Store(data, FileStorage.Type.png, (uint) this.net.ID, 0U);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    Interface.CallHook("OnSignUpdated", (object) this, (object) msg.player);
  }
}
