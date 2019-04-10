// Decompiled with JetBrains decompiler
// Type: RFBroadcaster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class RFBroadcaster : IOEntity, IRFObject
{
  public bool playerUsable = true;
  public int frequency;
  public GameObjectRef frequencyPanelPrefab;
  public const BaseEntity.Flags Flag_Broadcasting = BaseEntity.Flags.Reserved3;
  private float nextChangeTime;
  private float nextStopTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("RFBroadcaster.OnRpcMessage", 0.1f))
    {
      if (rpc == 2778616053U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ServerSetFrequency "));
          using (TimeWarning.New("ServerSetFrequency", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("ServerSetFrequency", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.ServerSetFrequency(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in ServerSetFrequency");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public int GetFrequency()
  {
    return this.frequency;
  }

  public override bool WantsPower()
  {
    return true;
  }

  public Vector3 GetPosition()
  {
    return ((Component) this).get_transform().get_position();
  }

  public float GetMaxRange()
  {
    return 100000f;
  }

  public void RFSignalUpdate(bool on)
  {
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void ServerSetFrequency(BaseEntity.RPCMessage msg)
  {
    if (Object.op_Equality((Object) msg.player, (Object) null) || !msg.player.CanBuild() || (!this.playerUsable || (double) Time.get_time() < (double) this.nextChangeTime))
      return;
    this.nextChangeTime = Time.get_time() + 2f;
    int num = msg.read.Int32();
    if (RFManager.IsReserved(num))
    {
      RFManager.ReserveErrorPrint(msg.player);
    }
    else
    {
      RFManager.ChangeFrequency(this.frequency, num, (IRFObject) this, false, this.IsPowered());
      this.frequency = num;
      this.MarkDirty();
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    ((IOEntity) info.msg.ioEntity).genericInt1 = (__Null) this.frequency;
  }

  public override void IOStateChanged(int inputAmount, int inputSlot)
  {
    if (inputAmount > 0)
    {
      this.CancelInvoke(new Action(this.StopBroadcasting));
      RFManager.AddBroadcaster(this.frequency, (IRFObject) this);
      this.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
      this.nextStopTime = Time.get_time() + 1f;
    }
    else
      this.Invoke(new Action(this.StopBroadcasting), Mathf.Clamp01(this.nextStopTime - Time.get_time()));
  }

  public void StopBroadcasting()
  {
    this.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
    RFManager.RemoveBroadcaster(this.frequency, (IRFObject) this);
  }

  internal override void DoServerDestroy()
  {
    RFManager.RemoveBroadcaster(this.frequency, (IRFObject) this);
    base.DoServerDestroy();
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity == null)
      return;
    this.frequency = (int) ((IOEntity) info.msg.ioEntity).genericInt1;
  }
}
