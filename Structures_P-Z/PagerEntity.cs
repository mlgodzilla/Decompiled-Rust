// Decompiled with JetBrains decompiler
// Type: PagerEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class PagerEntity : BaseEntity, IRFObject
{
  public static BaseEntity.Flags Flag_Silent = BaseEntity.Flags.Reserved1;
  private int frequency = 55;
  public float beepRepeat = 2f;
  public GameObjectRef pagerEffect;
  public GameObjectRef silentEffect;
  private float nextChangeTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("PagerEntity.OnRpcMessage", 0.1f))
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

  public override void SwitchParent(BaseEntity ent)
  {
    this.SetParent(ent, false, true);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    RFManager.AddListener(this.frequency, (IRFObject) this);
  }

  internal override void DoServerDestroy()
  {
    RFManager.RemoveListener(this.frequency, (IRFObject) this);
    base.DoServerDestroy();
  }

  public Vector3 GetPosition()
  {
    return ((Component) this).get_transform().get_position();
  }

  public float GetMaxRange()
  {
    return float.PositiveInfinity;
  }

  public void RFSignalUpdate(bool on)
  {
    if (this.IsDestroyed)
      return;
    bool flag = this.IsOn();
    if (on == flag)
      return;
    this.SetFlag(BaseEntity.Flags.On, on, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void SetSilentMode(bool wantsSilent)
  {
    this.SetFlag(PagerEntity.Flag_Silent, wantsSilent, false, true);
  }

  public void SetOff()
  {
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }

  public void ChangeFrequency(int newFreq)
  {
    RFManager.ChangeFrequency(this.frequency, newFreq, (IRFObject) this, true, true);
    this.frequency = newFreq;
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void ServerSetFrequency(BaseEntity.RPCMessage msg)
  {
    if (Object.op_Equality((Object) msg.player, (Object) null) || !msg.player.CanBuild() || (double) Time.get_time() < (double) this.nextChangeTime)
      return;
    this.nextChangeTime = Time.get_time() + 2f;
    int newFrequency = msg.read.Int32();
    RFManager.ChangeFrequency(this.frequency, newFrequency, (IRFObject) this, true, true);
    this.frequency = newFrequency;
    this.SendNetworkUpdateImmediate(false);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.ioEntity = (__Null) Pool.Get<IOEntity>();
    ((IOEntity) info.msg.ioEntity).genericInt1 = (__Null) this.frequency;
  }

  internal override void OnParentRemoved()
  {
    this.SetParent((BaseEntity) null, false, true);
  }

  public void OnParentDestroying()
  {
    if (!this.isServer)
      return;
    ((Component) this).get_transform().set_parent((Transform) null);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity != null)
      this.frequency = (int) ((IOEntity) info.msg.ioEntity).genericInt1;
    if (!this.isServer || !info.fromDisk)
      return;
    this.ChangeFrequency(this.frequency);
  }
}
