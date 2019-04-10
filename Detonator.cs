// Decompiled with JetBrains decompiler
// Type: Detonator
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

public class Detonator : HeldEntity, IRFObject
{
  public int frequency = 55;
  private float timeSinceDeploy;
  public GameObjectRef frequencyPanelPrefab;
  public GameObjectRef attackEffect;
  public GameObjectRef unAttackEffect;
  private float nextChangeTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Detonator.OnRpcMessage", 0.1f))
    {
      if (rpc == 2778616053U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ServerSetFrequency "));
        using (TimeWarning.New("ServerSetFrequency", 0.1f))
        {
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
      if (rpc == 1106698135U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetPressed "));
          using (TimeWarning.New("SetPressed", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SetPressed(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SetPressed");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  [BaseEntity.RPC_Server]
  public void SetPressed(BaseEntity.RPCMessage msg)
  {
    if (Object.op_Equality((Object) msg.player, (Object) null) || Object.op_Inequality((Object) msg.player, (Object) this.GetOwnerPlayer()))
      return;
    int num1 = this.HasFlag(BaseEntity.Flags.On) ? 1 : 0;
    bool pressed = msg.read.Bit();
    this.InternalSetPressed(pressed);
    int num2 = pressed ? 1 : 0;
    if (num1 == num2)
      return;
    Effect.server.Run(pressed ? this.attackEffect.resourcePath : this.unAttackEffect.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
  }

  internal void InternalSetPressed(bool pressed)
  {
    this.SetFlag(BaseEntity.Flags.On, pressed, false, true);
    if (pressed)
      RFManager.AddBroadcaster(this.frequency, (IRFObject) this);
    else
      RFManager.RemoveBroadcaster(this.frequency, (IRFObject) this);
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

  public override void SetHeld(bool bHeld)
  {
    if (!bHeld)
      this.InternalSetPressed(false);
    base.SetHeld(bHeld);
  }

  [BaseEntity.RPC_Server]
  public void ServerSetFrequency(BaseEntity.RPCMessage msg)
  {
    if (Object.op_Equality((Object) msg.player, (Object) null) || !msg.player.CanBuild() || (Object.op_Inequality((Object) this.GetOwnerPlayer(), (Object) msg.player) || (double) Time.get_time() < (double) this.nextChangeTime))
      return;
    this.nextChangeTime = Time.get_time() + 2f;
    int num = msg.read.Int32();
    if (RFManager.IsReserved(num))
    {
      RFManager.ReserveErrorPrint(msg.player);
    }
    else
    {
      RFManager.ChangeFrequency(this.frequency, num, (IRFObject) this, false, this.IsOn());
      this.frequency = num;
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      this.GetItem()?.MarkDirty();
    }
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (info.msg.ioEntity == null)
      info.msg.ioEntity = (__Null) Pool.Get<IOEntity>();
    ((IOEntity) info.msg.ioEntity).genericInt1 = (__Null) this.frequency;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity == null)
      return;
    this.frequency = (int) ((IOEntity) info.msg.ioEntity).genericInt1;
  }

  public int GetFrequency()
  {
    return this.frequency;
  }
}
