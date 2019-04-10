// Decompiled with JetBrains decompiler
// Type: PowerCounter
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
using UnityEngine.UI;

public class PowerCounter : IOEntity
{
  private int targetCounterNumber = 10;
  private int counterNumber;
  public CanvasGroup screenAlpha;
  public Text screenText;
  public const BaseEntity.Flags Flag_ShowPassthrough = BaseEntity.Flags.Reserved2;
  public GameObjectRef counterConfigPanel;
  public Color passthroughColor;
  public Color counterColor;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("PowerCounter.OnRpcMessage", 0.1f))
    {
      if (rpc == 3554226761U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SERVER_SetTarget "));
        using (TimeWarning.New("SERVER_SetTarget", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_SetTarget", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.SERVER_SetTarget(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in SERVER_SetTarget");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3222475159U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ToggleDisplayMode "));
          using (TimeWarning.New("ToggleDisplayMode", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("ToggleDisplayMode", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.ToggleDisplayMode(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in ToggleDisplayMode");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool DisplayPassthrough()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved2);
  }

  public bool DisplayCounter()
  {
    return !this.DisplayPassthrough();
  }

  public bool CanPlayerAdmin(BasePlayer player)
  {
    if (Object.op_Inequality((Object) player, (Object) null))
      return player.CanBuild();
    return false;
  }

  public int GetTarget()
  {
    return this.targetCounterNumber;
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void SERVER_SetTarget(BaseEntity.RPCMessage msg)
  {
    if (!this.CanPlayerAdmin(msg.player))
      return;
    this.targetCounterNumber = msg.read.Int32();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void ToggleDisplayMode(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanBuild())
      return;
    this.SetFlag(BaseEntity.Flags.Reserved2, msg.read.Bit(), false, false);
    this.MarkDirty();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    if (this.DisplayPassthrough() || this.counterNumber >= this.targetCounterNumber)
      return base.GetPassthroughAmount(outputSlot);
    return 0;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (info.msg.ioEntity == null)
      info.msg.ioEntity = (__Null) Pool.Get<IOEntity>();
    ((IOEntity) info.msg.ioEntity).genericInt1 = (__Null) this.counterNumber;
    ((IOEntity) info.msg.ioEntity).genericInt2 = (__Null) this.GetPassthroughAmount(0);
    ((IOEntity) info.msg.ioEntity).genericInt3 = (__Null) this.GetTarget();
  }

  public void SetCounterNumber(int newNumber)
  {
    this.counterNumber = newNumber;
  }

  public override void SendIONetworkUpdate()
  {
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override void UpdateHasPower(int inputAmount, int inputSlot)
  {
    if (inputSlot != 0)
      return;
    base.UpdateHasPower(inputAmount, inputSlot);
  }

  public override void UpdateFromInput(int inputAmount, int inputSlot)
  {
    if (this.DisplayCounter() && inputAmount > 0 && inputSlot != 0)
    {
      int counterNumber1 = this.counterNumber;
      switch (inputSlot)
      {
        case 1:
          ++this.counterNumber;
          break;
        case 2:
          --this.counterNumber;
          if (this.counterNumber < 0)
          {
            this.counterNumber = 0;
            break;
          }
          break;
        case 3:
          this.counterNumber = 0;
          break;
      }
      this.counterNumber = Mathf.Clamp(this.counterNumber, 0, 100);
      int counterNumber2 = this.counterNumber;
      if (counterNumber1 != counterNumber2)
      {
        this.MarkDirty();
        this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      }
    }
    if (inputSlot != 0)
      return;
    base.UpdateFromInput(inputAmount, inputSlot);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity == null)
      return;
    if (this.isServer)
      this.counterNumber = (int) ((IOEntity) info.msg.ioEntity).genericInt1;
    this.targetCounterNumber = (int) ((IOEntity) info.msg.ioEntity).genericInt3;
  }
}
