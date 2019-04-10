// Decompiled with JetBrains decompiler
// Type: SleepingBag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class SleepingBag : DecayEntity
{
  public static List<SleepingBag> sleepingBags = new List<SleepingBag>();
  public float secondsBetweenReuses = 300f;
  public string niceName = "Unnamed Bag";
  public Vector3 spawnOffset = Vector3.get_zero();
  [NonSerialized]
  public ulong deployerUserID;
  public GameObject renameDialog;
  public GameObject assignDialog;
  public bool canBePublic;
  public float unlockTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("SleepingBag.OnRpcMessage", 0.1f))
    {
      if (rpc == 3057055788U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - AssignToFriend "));
        using (TimeWarning.New("AssignToFriend", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("AssignToFriend", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.AssignToFriend(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in AssignToFriend");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1335950295U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Rename "));
        using (TimeWarning.New("Rename", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("Rename", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.Rename(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in Rename");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 42669546U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_MakeBed "));
        using (TimeWarning.New("RPC_MakeBed", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_MakeBed", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_MakeBed(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_MakeBed");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 393812086U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_MakePublic "));
          using (TimeWarning.New("RPC_MakePublic", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_MakePublic", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_MakePublic(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_MakePublic");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool IsPublic()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved3);
  }

  public float unlockSeconds
  {
    get
    {
      if ((double) this.unlockTime < (double) Time.get_realtimeSinceStartup())
        return 0.0f;
      return this.unlockTime - Time.get_realtimeSinceStartup();
    }
  }

  public static SleepingBag[] FindForPlayer(ulong playerID, bool ignoreTimers)
  {
    return SleepingBag.sleepingBags.Where<SleepingBag>((Func<SleepingBag, bool>) (x =>
    {
      if ((long) x.deployerUserID != (long) playerID)
        return false;
      if (!ignoreTimers)
        return (double) x.unlockTime < (double) Time.get_realtimeSinceStartup();
      return true;
    })).ToArray<SleepingBag>();
  }

  public static SleepingBag FindForPlayer(
    ulong playerID,
    uint sleepingBagID,
    bool ignoreTimers)
  {
    return SleepingBag.sleepingBags.FirstOrDefault<SleepingBag>((Func<SleepingBag, bool>) (x =>
    {
      if ((long) x.deployerUserID != (long) playerID || x.net.ID != (int) sleepingBagID)
        return false;
      if (!ignoreTimers)
        return (double) x.unlockTime < (double) Time.get_realtimeSinceStartup();
      return true;
    }));
  }

  public static bool SpawnPlayer(BasePlayer player, uint sleepingBag)
  {
    SleepingBag[] forPlayer = SleepingBag.FindForPlayer(player.userID, true);
    SleepingBag sleepingBag1 = ((IEnumerable<SleepingBag>) forPlayer).FirstOrDefault<SleepingBag>((Func<SleepingBag, bool>) (x =>
    {
      if ((long) x.deployerUserID == (long) player.userID && x.net.ID == (int) sleepingBag)
        return (double) x.unlockTime < (double) Time.get_realtimeSinceStartup();
      return false;
    }));
    if (Object.op_Equality((Object) sleepingBag1, (Object) null))
      return false;
    Vector3 position = Vector3.op_Addition(((Component) sleepingBag1).get_transform().get_position(), sleepingBag1.spawnOffset);
    Quaternion rotation1 = ((Component) sleepingBag1).get_transform().get_rotation();
    Quaternion rotation2 = Quaternion.Euler(0.0f, (float) ((Quaternion) ref rotation1).get_eulerAngles().y, 0.0f);
    player.RespawnAt(position, rotation2);
    foreach (SleepingBag sleepingBag2 in forPlayer)
    {
      if ((double) Vector3.Distance(position, ((Component) sleepingBag2).get_transform().get_position()) <= (double) Server.respawnresetrange)
        sleepingBag2.unlockTime = Time.get_realtimeSinceStartup() + sleepingBag2.secondsBetweenReuses;
    }
    return true;
  }

  public static bool DestroyBag(BasePlayer player, uint sleepingBag)
  {
    SleepingBag forPlayer = SleepingBag.FindForPlayer(player.userID, sleepingBag, false);
    if (Object.op_Equality((Object) forPlayer, (Object) null))
      return false;
    if (forPlayer.canBePublic)
    {
      forPlayer.SetPublic(true);
      forPlayer.deployerUserID = 0UL;
    }
    else
      forPlayer.Kill(BaseNetworkable.DestroyMode.None);
    player.SendRespawnOptions();
    return true;
  }

  public void SetPublic(bool isPublic)
  {
    this.SetFlag(BaseEntity.Flags.Reserved3, isPublic, false, true);
  }

  private void SetDeployedBy(BasePlayer player)
  {
    if (Object.op_Equality((Object) player, (Object) null))
      return;
    this.deployerUserID = player.userID;
    float num = Time.get_realtimeSinceStartup();
    foreach (SleepingBag sleepingBag in SleepingBag.sleepingBags.Where<SleepingBag>((Func<SleepingBag, bool>) (x =>
    {
      if ((long) x.deployerUserID == (long) player.userID)
        return (double) x.unlockTime > (double) Time.get_realtimeSinceStartup();
      return false;
    })).ToArray<SleepingBag>())
    {
      if ((double) sleepingBag.unlockTime > (double) num && (double) Vector3.Distance(((Component) sleepingBag).get_transform().get_position(), ((Component) this).get_transform().get_position()) <= (double) Server.respawnresetrange)
        num = sleepingBag.unlockTime;
    }
    this.unlockTime = Mathf.Max(num, Time.get_realtimeSinceStartup() + this.secondsBetweenReuses);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (SleepingBag.sleepingBags.Contains(this))
      return;
    SleepingBag.sleepingBags.Add(this);
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    SleepingBag.sleepingBags.RemoveAll((Predicate<SleepingBag>) (x => Object.op_Equality((Object) x, (Object) this)));
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.sleepingBag = (__Null) Pool.Get<SleepingBag>();
    ((SleepingBag) info.msg.sleepingBag).name = (__Null) this.niceName;
    ((SleepingBag) info.msg.sleepingBag).deployerID = (__Null) (long) this.deployerUserID;
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  public void Rename(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract())
      return;
    string str1 = msg.read.String();
    if (Interface.CallHook("CanRenameBed", (object) msg.player, (object) this, (object) str1) != null)
      return;
    string str2 = WordFilter.Filter(str1);
    if (string.IsNullOrEmpty(str2))
      str2 = "Unnamed Sleeping Bag";
    if (str2.Length > 24)
      str2 = str2.Substring(0, 22) + "..";
    this.niceName = str2;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  public void AssignToFriend(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract() || (long) this.deployerUserID != (long) msg.player.userID)
      return;
    ulong num = msg.read.UInt64();
    if (num == 0UL || Interface.CallHook("CanAssignBed", (object) msg.player, (object) this, (object) num) != null)
      return;
    this.deployerUserID = num;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  public void RPC_MakePublic(BaseEntity.RPCMessage msg)
  {
    if (!this.canBePublic || !msg.player.CanInteract() || (long) this.deployerUserID != (long) msg.player.userID && !msg.player.CanBuild())
      return;
    bool isPublic = msg.read.Bit();
    if (isPublic == this.IsPublic() || Interface.CallHook("CanSetBedPublic", (object) msg.player, (object) this) != null)
      return;
    this.SetPublic(isPublic);
    if (!this.IsPublic())
      this.deployerUserID = msg.player.userID;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  public void RPC_MakeBed(BaseEntity.RPCMessage msg)
  {
    if (!this.canBePublic || !this.IsPublic() || !msg.player.CanInteract())
      return;
    this.deployerUserID = msg.player.userID;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.sleepingBag == null)
      return;
    this.niceName = (string) ((SleepingBag) info.msg.sleepingBag).name;
    this.deployerUserID = (ulong) ((SleepingBag) info.msg.sleepingBag).deployerID;
  }

  public override bool CanPickup(BasePlayer player)
  {
    if (base.CanPickup(player))
      return (long) player.userID == (long) this.deployerUserID;
    return false;
  }
}
