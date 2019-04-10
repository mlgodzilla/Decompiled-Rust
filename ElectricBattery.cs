// Decompiled with JetBrains decompiler
// Type: ElectricBattery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using ProtoBuf;
using System;
using UnityEngine;

public class ElectricBattery : IOEntity
{
  public float chargeRatio = 0.25f;
  public int maxOutput;
  public float maxCapactiySeconds;
  public float capacitySeconds;
  public bool rechargable;
  private const float tickRateSeconds = 1f;

  public override bool IsRootEntity()
  {
    return true;
  }

  public override void SendAdditionalData(BasePlayer player)
  {
    float num = 0.0f;
    this.ClientRPCPlayer<int, int, float, float>((Connection) null, player, "Client_ReceiveAdditionalData", this.currentEnergy, this.GetPassthroughAmount(0), this.capacitySeconds, num);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRandomized(new Action(this.CheckDischarge), Random.Range(0.0f, 1f), 1f, 0.1f);
  }

  public void CheckDischarge()
  {
    if ((double) this.capacitySeconds < 5.0)
    {
      this.SetDischarging(false);
    }
    else
    {
      IOEntity ioEntity = this.outputs[0].connectedTo.Get(true);
      if (Object.op_Implicit((Object) ioEntity))
        this.SetDischarging(ioEntity.WantsPower());
      else
        this.SetDischarging(false);
    }
  }

  public void SetDischarging(bool wantsOn)
  {
    this.SetPassthroughOn(wantsOn);
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    if (this.IsOn())
      return Mathf.FloorToInt((float) this.maxOutput * ((double) this.capacitySeconds >= 1.0 ? 1f : 0.0f));
    return 0;
  }

  public override bool WantsPower()
  {
    return (double) this.capacitySeconds < (double) this.maxCapactiySeconds;
  }

  public override void IOStateChanged(int inputAmount, int inputSlot)
  {
    base.IOStateChanged(inputAmount, inputSlot);
    if (inputSlot != 0)
      return;
    if (!this.IsPowered())
    {
      if (!this.rechargable)
        return;
      this.CancelInvoke(new Action(this.AddCharge));
    }
    else
    {
      if (!this.rechargable || this.IsInvoking(new Action(this.AddCharge)))
        return;
      this.InvokeRandomized(new Action(this.AddCharge), 1f, 1f, 0.1f);
    }
  }

  public void TickUsage()
  {
    int num1 = (double) this.capacitySeconds > 0.0 ? 1 : 0;
    if ((double) this.capacitySeconds >= 1.0)
      --this.capacitySeconds;
    if ((double) this.capacitySeconds <= 0.0)
      this.capacitySeconds = 0.0f;
    int num2 = (double) this.capacitySeconds > 0.0 ? 1 : 0;
    if (num1 == num2)
      return;
    this.MarkDirty();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void AddCharge()
  {
    this.capacitySeconds += 1f * Mathf.InverseLerp(0.0f, (float) this.maxOutput, (float) this.currentEnergy) * this.chargeRatio;
    this.capacitySeconds = Mathf.Clamp(this.capacitySeconds, 0.0f, this.maxCapactiySeconds);
  }

  public void SetPassthroughOn(bool wantsOn)
  {
    if (wantsOn == this.IsOn())
      return;
    this.SetFlag(BaseEntity.Flags.On, wantsOn, false, true);
    if (this.IsOn())
    {
      if (!this.IsInvoking(new Action(this.TickUsage)))
        this.InvokeRandomized(new Action(this.TickUsage), 1f, 1f, 0.1f);
    }
    else
      this.CancelInvoke(new Action(this.TickUsage));
    this.MarkDirty();
  }

  public void Unbusy()
  {
    this.SetFlag(BaseEntity.Flags.Busy, false, false, true);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (info.msg.ioEntity == null)
      info.msg.ioEntity = (__Null) Pool.Get<IOEntity>();
    ((IOEntity) info.msg.ioEntity).genericFloat1 = (__Null) (double) this.capacitySeconds;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity == null)
      return;
    this.capacitySeconds = (float) ((IOEntity) info.msg.ioEntity).genericFloat1;
  }
}
