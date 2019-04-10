// Decompiled with JetBrains decompiler
// Type: CodeLock
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CodeLock : BaseLock
{
  public string code = string.Empty;
  public string guestCode = string.Empty;
  public List<ulong> whitelistPlayers = new List<ulong>();
  public List<ulong> guestPlayers = new List<ulong>();
  public float lastWrongTime = float.NegativeInfinity;
  public GameObjectRef keyEnterDialog;
  public GameObjectRef effectUnlocked;
  public GameObjectRef effectLocked;
  public GameObjectRef effectDenied;
  public GameObjectRef effectCodeChanged;
  public GameObjectRef effectShock;
  public bool hasCode;
  public bool hasGuestCode;
  public int wrongCodes;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("CodeLock.OnRpcMessage", 0.1f))
    {
      if (rpc == 4013784361U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_ChangeCode "));
        using (TimeWarning.New("RPC_ChangeCode", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_ChangeCode", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_ChangeCode(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_ChangeCode");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2626067433U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - TryLock "));
        using (TimeWarning.New("TryLock", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("TryLock", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.TryLock(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in TryLock");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1718262U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - TryUnlock "));
        using (TimeWarning.New("TryUnlock", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("TryUnlock", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.TryUnlock(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in TryUnlock");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 418605506U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - UnlockWithCode "));
          using (TimeWarning.New("UnlockWithCode", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("UnlockWithCode", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.UnlockWithCode(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in UnlockWithCode");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.codeLock == null)
      return;
    this.hasCode = (bool) ((CodeLock) info.msg.codeLock).hasCode;
    this.hasGuestCode = (bool) ((CodeLock) info.msg.codeLock).hasGuestCode;
    if (((CodeLock) info.msg.codeLock).pv == null)
      return;
    this.code = (string) ((CodeLock.Private) ((CodeLock) info.msg.codeLock).pv).code;
    this.whitelistPlayers = (List<ulong>) ((CodeLock.Private) ((CodeLock) info.msg.codeLock).pv).users;
    this.guestCode = (string) ((CodeLock.Private) ((CodeLock) info.msg.codeLock).pv).guestCode;
    this.guestPlayers = (List<ulong>) ((CodeLock.Private) ((CodeLock) info.msg.codeLock).pv).guestUsers;
    if (this.guestCode != null && this.guestCode.Length == 4)
      return;
    this.hasGuestCode = false;
    this.guestCode = string.Empty;
    this.guestPlayers.Clear();
  }

  internal void DoEffect(string effect)
  {
    Effect.server.Run(effect, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_forward(), (Connection) null, false);
  }

  public override bool OnTryToOpen(BasePlayer player)
  {
    object obj = Interface.CallHook("CanUseLockedEntity", (object) player, (object) this);
    if (obj is bool)
      return (bool) obj;
    if (!this.IsLocked())
      return true;
    if (this.whitelistPlayers.Contains(player.userID) || this.guestPlayers.Contains(player.userID))
    {
      this.DoEffect(this.effectUnlocked.resourcePath);
      return true;
    }
    this.DoEffect(this.effectDenied.resourcePath);
    return false;
  }

  public override bool OnTryToClose(BasePlayer player)
  {
    object obj = Interface.CallHook("CanUseLockedEntity", (object) player, (object) this);
    if (obj is bool)
      return (bool) obj;
    if (!this.IsLocked())
      return true;
    if (this.whitelistPlayers.Contains(player.userID) || this.guestPlayers.Contains(player.userID))
    {
      this.DoEffect(this.effectUnlocked.resourcePath);
      return true;
    }
    this.DoEffect(this.effectDenied.resourcePath);
    return false;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.codeLock = (__Null) Pool.Get<CodeLock>();
    ((CodeLock) info.msg.codeLock).hasGuestCode = (__Null) (this.guestCode.Length > 0 ? 1 : 0);
    ((CodeLock) info.msg.codeLock).hasCode = (__Null) (this.code.Length > 0 ? 1 : 0);
    if (!info.forDisk)
      return;
    ((CodeLock) info.msg.codeLock).pv = (__Null) Pool.Get<CodeLock.Private>();
    ((CodeLock.Private) ((CodeLock) info.msg.codeLock).pv).code = (__Null) this.code;
    ((CodeLock.Private) ((CodeLock) info.msg.codeLock).pv).users = (__Null) Pool.Get<List<ulong>>();
    ((List<ulong>) ((CodeLock.Private) ((CodeLock) info.msg.codeLock).pv).users).AddRange((IEnumerable<ulong>) this.whitelistPlayers);
    ((CodeLock.Private) ((CodeLock) info.msg.codeLock).pv).guestCode = (__Null) this.guestCode;
    ((CodeLock.Private) ((CodeLock) info.msg.codeLock).pv).guestUsers = (__Null) Pool.Get<List<ulong>>();
    ((List<ulong>) ((CodeLock.Private) ((CodeLock) info.msg.codeLock).pv).guestUsers).AddRange((IEnumerable<ulong>) this.guestPlayers);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void RPC_ChangeCode(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract())
      return;
    string str = rpc.read.String();
    bool flag = rpc.read.Bit();
    if (this.IsLocked() || str.Length != 4 || !this.hasCode & flag)
      return;
    if (!this.hasCode && !flag)
      this.SetFlag(BaseEntity.Flags.Locked, true, false, true);
    if (Interface.CallHook("CanChangeCode", (object) rpc.player, (object) this, (object) str, (object) flag) != null)
      return;
    if (!flag)
    {
      this.code = str;
      this.hasCode = this.code.Length > 0;
      this.whitelistPlayers.Clear();
      this.whitelistPlayers.Add(rpc.player.userID);
    }
    else
    {
      this.guestCode = str;
      this.hasGuestCode = this.guestCode.Length > 0;
      this.guestPlayers.Clear();
      this.guestPlayers.Add(rpc.player.userID);
    }
    this.DoEffect(this.effectCodeChanged.resourcePath);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void TryUnlock(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.IsLocked() || Interface.CallHook("CanUnlock", (object) rpc.player, (object) this) != null)
      return;
    if (this.whitelistPlayers.Contains(rpc.player.userID))
    {
      this.DoEffect(this.effectUnlocked.resourcePath);
      this.SetFlag(BaseEntity.Flags.Locked, false, false, true);
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    else
      this.ClientRPCPlayer((Connection) null, rpc.player, "EnterUnlockCode");
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  private void TryLock(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || this.IsLocked() || (this.code.Length != 4 || Interface.CallHook("CanLock", (object) rpc.player, (object) this) != null) || !this.whitelistPlayers.Contains(rpc.player.userID))
      return;
    this.DoEffect(this.effectLocked.resourcePath);
    this.SetFlag(BaseEntity.Flags.Locked, true, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void UnlockWithCode(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.IsLocked())
      return;
    string str = rpc.read.String();
    if (Interface.CallHook("OnCodeEntered", (object) this, (object) rpc.player, (object) str) != null)
      return;
    bool flag1 = str == this.guestCode;
    bool flag2 = str == this.code;
    if ((str == this.code ? 1 : (!this.hasGuestCode ? 0 : (str == this.guestCode ? 1 : 0))) == 0)
    {
      if ((double) Time.get_realtimeSinceStartup() > (double) this.lastWrongTime + 10.0)
        this.wrongCodes = 0;
      this.DoEffect(this.effectDenied.resourcePath);
      this.DoEffect(this.effectShock.resourcePath);
      rpc.player.Hurt((float) (this.wrongCodes + 1) * 5f, DamageType.ElectricShock, (BaseEntity) this, false);
      ++this.wrongCodes;
      this.lastWrongTime = Time.get_realtimeSinceStartup();
    }
    else
    {
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      if (flag2)
      {
        if (this.whitelistPlayers.Contains(rpc.player.userID))
          return;
        this.DoEffect(this.effectCodeChanged.resourcePath);
        this.whitelistPlayers.Add(rpc.player.userID);
      }
      else
      {
        if (!flag1 || this.guestPlayers.Contains(rpc.player.userID))
          return;
        this.DoEffect(this.effectCodeChanged.resourcePath);
        this.guestPlayers.Add(rpc.player.userID);
      }
    }
  }
}
