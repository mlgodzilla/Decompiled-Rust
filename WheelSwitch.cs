// Decompiled with JetBrains decompiler
// Type: WheelSwitch
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

public class WheelSwitch : IOEntity
{
  public float rotateSpeed = 90f;
  public BaseEntity.Flags BeingRotated = BaseEntity.Flags.Reserved1;
  public BaseEntity.Flags RotatingLeft = BaseEntity.Flags.Reserved2;
  public BaseEntity.Flags RotatingRight = BaseEntity.Flags.Reserved3;
  public float kineticEnergyPerSec = 1f;
  private float progressTickRate = 0.1f;
  public Transform wheelObj;
  public float rotateProgress;
  public Animator animator;
  private BasePlayer rotatorPlayer;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("WheelSwitch.OnRpcMessage", 0.1f))
    {
      if (rpc == 2223603322U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - BeginRotate "));
        using (TimeWarning.New("BeginRotate", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("BeginRotate", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.BeginRotate(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in BeginRotate");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 434251040U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - CancelRotate "));
          using (TimeWarning.New("CancelRotate", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("CancelRotate", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.CancelRotate(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in CancelRotate");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ResetIOState()
  {
    this.CancelPlayerRotation();
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void BeginRotate(BaseEntity.RPCMessage msg)
  {
    if (this.IsBeingRotated())
      return;
    this.SetFlag(this.BeingRotated, true, false, true);
    this.rotatorPlayer = msg.player;
    this.InvokeRepeating(new Action(this.RotateProgress), 0.0f, this.progressTickRate);
  }

  public void CancelPlayerRotation()
  {
    this.CancelInvoke(new Action(this.RotateProgress));
    this.SetFlag(this.BeingRotated, false, false, true);
    foreach (IOEntity.IOSlot output in this.outputs)
    {
      if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null))
      {
        double num = (double) output.connectedTo.Get(true).IOInput((IOEntity) this, this.ioType, 0.0f, output.connectedToSlot);
      }
    }
    this.rotatorPlayer = (BasePlayer) null;
  }

  public void RotateProgress()
  {
    if (!Object.op_Implicit((Object) this.rotatorPlayer) || this.rotatorPlayer.IsDead() || (this.rotatorPlayer.IsSleeping() || (double) Vector3Ex.Distance2D(((Component) this.rotatorPlayer).get_transform().get_position(), ((Component) this).get_transform().get_position()) > 2.0))
    {
      this.CancelPlayerRotation();
    }
    else
    {
      float inputAmount = this.kineticEnergyPerSec * this.progressTickRate;
      foreach (IOEntity.IOSlot output in this.outputs)
      {
        if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null))
          inputAmount = output.connectedTo.Get(true).IOInput((IOEntity) this, this.ioType, inputAmount, output.connectedToSlot);
      }
      if ((double) inputAmount == 0.0)
        this.SetRotateProgress(this.rotateProgress + 0.1f);
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
  }

  public void SetRotateProgress(float newValue)
  {
    float rotateProgress = this.rotateProgress;
    this.rotateProgress = newValue;
    this.SetFlag(BaseEntity.Flags.Reserved4, (double) rotateProgress != (double) newValue, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    this.CancelInvoke(new Action(this.StoppedRotatingCheck));
    this.Invoke(new Action(this.StoppedRotatingCheck), 0.25f);
  }

  public void StoppedRotatingCheck()
  {
    this.SetFlag(BaseEntity.Flags.Reserved4, false, false, true);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void CancelRotate(BaseEntity.RPCMessage msg)
  {
    this.CancelPlayerRotation();
  }

  public void Powered()
  {
    float inputAmount = this.kineticEnergyPerSec * this.progressTickRate;
    foreach (IOEntity.IOSlot output in this.outputs)
    {
      if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null))
        inputAmount = output.connectedTo.Get(true).IOInput((IOEntity) this, this.ioType, inputAmount, output.connectedToSlot);
    }
    this.SetRotateProgress(this.rotateProgress + 0.1f);
  }

  public override float IOInput(
    IOEntity from,
    IOEntity.IOType inputType,
    float inputAmount,
    int slot = 0)
  {
    if ((double) inputAmount < 0.0)
    {
      this.SetRotateProgress(this.rotateProgress + inputAmount);
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    if (inputType == IOEntity.IOType.Electric && slot == 1)
    {
      if ((double) inputAmount == 0.0)
        this.CancelInvoke(new Action(this.Powered));
      else
        this.InvokeRepeating(new Action(this.Powered), 0.0f, this.progressTickRate);
    }
    return Mathf.Clamp(inputAmount - 1f, 0.0f, inputAmount);
  }

  public bool IsBeingRotated()
  {
    return this.HasFlag(this.BeingRotated);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.sphereEntity == null)
      return;
    this.rotateProgress = (float) ((SphereEntity) info.msg.sphereEntity).radius;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.sphereEntity = (__Null) Pool.Get<SphereEntity>();
    ((SphereEntity) info.msg.sphereEntity).radius = (__Null) (double) this.rotateProgress;
  }
}
