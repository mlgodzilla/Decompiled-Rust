// Decompiled with JetBrains decompiler
// Type: WaterWell
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

public class WaterWell : LiquidContainer
{
  public float caloriesPerPump = 5f;
  public float pressurePerPump = 0.2f;
  public float pressureForProduction = 1f;
  public int waterPerPump = 50;
  public Animator animator;
  private const BaseEntity.Flags Pumping = BaseEntity.Flags.Reserved2;
  private const BaseEntity.Flags WaterFlow = BaseEntity.Flags.Reserved3;
  public float currentPressure;
  public GameObject waterLevelObj;
  public float waterLevelObjFullOffset;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("WaterWell.OnRpcMessage", 0.1f))
    {
      if (rpc == 2538739344U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Pump "));
          using (TimeWarning.New("RPC_Pump", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_Pump", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_Pump(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_Pump");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  public void RPC_Pump(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (Object.op_Equality((Object) player, (Object) null) || player.IsDead() || (player.IsSleeping() || (double) player.metabolism.calories.value < (double) this.caloriesPerPump) || this.HasFlag(BaseEntity.Flags.Reserved2))
      return;
    this.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
    player.metabolism.calories.value -= this.caloriesPerPump;
    player.metabolism.SendChangesToClient();
    this.currentPressure = Mathf.Clamp01(this.currentPressure + this.pressurePerPump);
    this.Invoke(new Action(this.StopPump), 1.8f);
    if ((double) this.currentPressure >= 0.0)
    {
      this.CancelInvoke(new Action(this.Produce));
      this.Invoke(new Action(this.Produce), 1f);
    }
    this.SendNetworkUpdateImmediate(false);
  }

  public void StopPump()
  {
    this.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
    this.SendNetworkUpdateImmediate(false);
  }

  protected override void OnInventoryDirty()
  {
    base.OnInventoryDirty();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void Produce()
  {
    this.inventory.AddItem(this.defaultLiquid, this.waterPerPump);
    this.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
    this.ScheduleTapOff();
    this.SendNetworkUpdateImmediate(false);
  }

  public void ScheduleTapOff()
  {
    this.CancelInvoke(new Action(this.TapOff));
    this.Invoke(new Action(this.TapOff), 1f);
  }

  private void TapOff()
  {
    this.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
  }

  public void ReducePressure()
  {
    this.currentPressure = Mathf.Clamp01(this.currentPressure - Random.Range(0.1f, 0.2f));
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.waterwell = (__Null) Pool.Get<WaterWell>();
    ((WaterWell) info.msg.waterwell).pressure = (__Null) (double) this.currentPressure;
    ((WaterWell) info.msg.waterwell).waterLevel = (__Null) (double) this.GetWaterAmount();
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.waterwell == null)
      return;
    this.currentPressure = (float) ((WaterWell) info.msg.waterwell).pressure;
  }

  public float GetWaterAmount()
  {
    if (!this.isServer)
      return 0.0f;
    Item slot = this.inventory.GetSlot(0);
    if (slot == null)
      return 0.0f;
    return (float) slot.amount;
  }
}
