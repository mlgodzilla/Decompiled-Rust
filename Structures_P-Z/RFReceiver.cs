// Decompiled with JetBrains decompiler
// Type: RFReceiver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class RFReceiver : IOEntity, IRFObject
{
  public int frequency;
  public GameObjectRef frequencyPanelPrefab;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("RFReceiver.OnRpcMessage", 0.1f))
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
    return this.IsOn();
  }

  public override void ResetIOState()
  {
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    if (!this.IsOn())
      return 0;
    return this.GetCurrentEnergy();
  }

  public Vector3 GetPosition()
  {
    return ((Component) this).get_transform().get_position();
  }

  public float GetMaxRange()
  {
    return 100000f;
  }

  public override void Init()
  {
    base.Init();
    RFManager.AddListener(this.frequency, (IRFObject) this);
  }

  internal override void DoServerDestroy()
  {
    RFManager.RemoveListener(this.frequency, (IRFObject) this);
    base.DoServerDestroy();
  }

  public void RFSignalUpdate(bool on)
  {
    if (this.IsDestroyed || this.IsOn() == on)
      return;
    this.SetFlag(BaseEntity.Flags.On, on, false, true);
    this.SendNetworkUpdateImmediate(false);
    this.MarkDirty();
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void ServerSetFrequency(BaseEntity.RPCMessage msg)
  {
    if (Object.op_Equality((Object) msg.player, (Object) null) || !msg.player.CanBuild())
      return;
    int newFrequency = msg.read.Int32();
    RFManager.ChangeFrequency(this.frequency, newFrequency, (IRFObject) this, true, true);
    this.frequency = newFrequency;
    this.MarkDirty();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    ((IOEntity) info.msg.ioEntity).genericInt1 = (__Null) this.frequency;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity == null)
      return;
    this.frequency = (int) ((IOEntity) info.msg.ioEntity).genericInt1;
  }
}
