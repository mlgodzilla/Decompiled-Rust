// Decompiled with JetBrains decompiler
// Type: FogMachine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class FogMachine : StorageContainer
{
  public float fogLength = 60f;
  public float nozzleBlastDuration = 5f;
  public float fuelPerSec = 1f;
  public const BaseEntity.Flags FogFieldOn = BaseEntity.Flags.Reserved8;
  public const BaseEntity.Flags MotionMode = BaseEntity.Flags.Reserved7;
  public const BaseEntity.Flags Emitting = BaseEntity.Flags.Reserved6;
  public const BaseEntity.Flags Flag_HasJuice = BaseEntity.Flags.Reserved5;
  private float pendingFuel;

  public bool IsEmitting()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved6);
  }

  public bool HasJuice()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved5);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void SetFogOn(BaseEntity.RPCMessage msg)
  {
    if (this.IsEmitting() || this.IsOn() || (!this.HasFuel() || !msg.player.CanBuild()))
      return;
    this.SetFlag(BaseEntity.Flags.On, true, false, true);
    this.InvokeRepeating(new Action(this.StartFogging), 0.0f, this.fogLength - 1f);
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void SetFogOff(BaseEntity.RPCMessage msg)
  {
    if (!this.IsOn() || !msg.player.CanBuild())
      return;
    this.CancelInvoke(new Action(this.StartFogging));
    this.SetFlag(BaseEntity.Flags.On, false, false, true);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void SetMotionDetection(BaseEntity.RPCMessage msg)
  {
    bool b = msg.read.Bit();
    if (!msg.player.CanBuild())
      return;
    this.SetFlag(BaseEntity.Flags.Reserved7, b, false, true);
    if (b)
      this.SetFlag(BaseEntity.Flags.On, false, false, true);
    this.UpdateMotionMode();
  }

  public void UpdateMotionMode()
  {
    if (this.HasFlag(BaseEntity.Flags.Reserved7))
      this.InvokeRandomized(new Action(this.CheckTrigger), Random.Range(0.0f, 0.5f), 0.5f, 0.1f);
    else
      this.CancelInvoke(new Action(this.CheckTrigger));
  }

  public void CheckTrigger()
  {
    if (this.IsEmitting() || !BasePlayer.AnyPlayersVisibleToEntity(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 3f)), 3f, (BaseEntity) this, Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 0.1f)), true))
      return;
    this.StartFogging();
  }

  public void StartFogging()
  {
    if (!this.UseFuel(1f))
    {
      this.CancelInvoke(new Action(this.StartFogging));
      this.SetFlag(BaseEntity.Flags.On, false, false, true);
    }
    else
    {
      this.SetFlag(BaseEntity.Flags.Reserved6, true, false, true);
      this.Invoke(new Action(this.EnableFogField), 1f);
      this.Invoke(new Action(this.DisableNozzle), this.nozzleBlastDuration);
      this.Invoke(new Action(this.FinishFogging), this.fogLength);
    }
  }

  public virtual void EnableFogField()
  {
    this.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
  }

  public void DisableNozzle()
  {
    this.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
  }

  public virtual void FinishFogging()
  {
    this.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.SetFlag(BaseEntity.Flags.Reserved8, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved6, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved5, this.HasFuel(), false, true);
    if (this.IsOn())
      this.InvokeRepeating(new Action(this.StartFogging), 0.0f, this.fogLength - 1f);
    this.UpdateMotionMode();
  }

  public override void PlayerStoppedLooting(BasePlayer player)
  {
    this.SetFlag(BaseEntity.Flags.Reserved5, this.HasFuel(), false, true);
    base.PlayerStoppedLooting(player);
  }

  public int GetFuelAmount()
  {
    Item slot = this.inventory.GetSlot(0);
    if (slot == null || slot.amount < 1)
      return 0;
    return slot.amount;
  }

  public bool HasFuel()
  {
    return this.GetFuelAmount() >= 1;
  }

  public bool UseFuel(float seconds)
  {
    Item slot = this.inventory.GetSlot(0);
    if (slot == null || slot.amount < 1)
      return false;
    this.pendingFuel += seconds * this.fuelPerSec;
    if ((double) this.pendingFuel >= 1.0)
    {
      int amountToConsume = Mathf.FloorToInt(this.pendingFuel);
      slot.UseItem(amountToConsume);
      this.pendingFuel -= (float) amountToConsume;
    }
    return true;
  }

  public virtual bool MotionModeEnabled()
  {
    return true;
  }

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("FogMachine.OnRpcMessage", 0.1f))
    {
      if (rpc == 2788115565U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetFogOff "));
        using (TimeWarning.New("SetFogOff", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("SetFogOff", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.SetFogOff(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in SetFogOff");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3905831928U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetFogOn "));
        using (TimeWarning.New("SetFogOn", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("SetFogOn", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.SetFogOn(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in SetFogOn");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1773639087U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SetMotionDetection "));
          using (TimeWarning.New("SetMotionDetection", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("SetMotionDetection", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SetMotionDetection(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SetMotionDetection");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }
}
