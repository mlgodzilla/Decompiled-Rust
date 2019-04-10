// Decompiled with JetBrains decompiler
// Type: TimerSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using ProtoBuf;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class TimerSwitch : IOEntity
{
  public float timerLength = 10f;
  private float timePassed = -1f;
  public Transform timerDrum;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("TimerSwitch.OnRpcMessage", 0.1f))
    {
      if (rpc == 4167839872U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SVSwitch "));
          using (TimeWarning.New("SVSwitch", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("SVSwitch", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SVSwitch(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SVSwitch");
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
    base.ResetIOState();
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    if (!this.IsInvoking(new Action(this.AdvanceTime)))
      return;
    this.EndTimer();
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    if (outputSlot != 0)
      return base.GetPassthroughAmount(0);
    if ((!this.IsPowered() ? 0 : (this.IsOn() ? 1 : 0)) == 0)
      return 0;
    return base.GetPassthroughAmount(0);
  }

  public override void UpdateHasPower(int inputAmount, int inputSlot)
  {
    if (inputSlot != 0)
      return;
    this.SetFlag(BaseEntity.Flags.Reserved8, inputAmount > 0, false, false);
  }

  public override void UpdateFromInput(int inputAmount, int inputSlot)
  {
    switch (inputSlot)
    {
      case 0:
        base.UpdateFromInput(inputAmount, inputSlot);
        if (!this.IsPowered() && this.IsInvoking(new Action(this.AdvanceTime)))
        {
          this.EndTimer();
          break;
        }
        if ((double) this.timePassed == -1.0)
          break;
        this.SetFlag(BaseEntity.Flags.On, false, false, false);
        this.SwitchPressed();
        break;
      case 1:
        if (inputAmount <= 0)
          break;
        this.SwitchPressed();
        break;
    }
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void SVSwitch(BaseEntity.RPCMessage msg)
  {
    this.SwitchPressed();
  }

  public void SwitchPressed()
  {
    if (this.IsOn() || !this.IsPowered())
      return;
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.MarkDirty();
    this.InvokeRepeating(new Action(this.AdvanceTime), 0.0f, 0.1f);
    this.SendNetworkUpdateImmediate(false);
  }

  public void AdvanceTime()
  {
    if ((double) this.timePassed < 0.0)
      this.timePassed = 0.0f;
    this.timePassed += 0.1f;
    if ((double) this.timePassed >= (double) this.timerLength)
      this.EndTimer();
    else
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void EndTimer()
  {
    this.CancelInvoke(new Action(this.AdvanceTime));
    this.timePassed = -1f;
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.SendNetworkUpdateImmediate(false);
    this.MarkDirty();
  }

  public float GetPassedTime()
  {
    return this.timePassed;
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    if ((double) this.timePassed == -1.0)
    {
      if (!this.IsOn())
        return;
      this.SetFlag(BaseEntity.Flags.On, false, false, true);
    }
    else
      this.SwitchPressed();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    ((IOEntity) info.msg.ioEntity).genericFloat1 = (__Null) (double) this.GetPassedTime();
    ((IOEntity) info.msg.ioEntity).genericFloat2 = (__Null) (double) this.timerLength;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity == null)
      return;
    this.timerLength = (float) ((IOEntity) info.msg.ioEntity).genericFloat2;
    this.timePassed = (float) ((IOEntity) info.msg.ioEntity).genericFloat1;
  }
}
