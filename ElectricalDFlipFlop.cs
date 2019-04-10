// Decompiled with JetBrains decompiler
// Type: ElectricalDFlipFlop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ElectricalDFlipFlop : IOEntity
{
  [NonSerialized]
  private int setAmount;
  [NonSerialized]
  private int resetAmount;
  [NonSerialized]
  private int toggleAmount;

  public override void UpdateHasPower(int inputAmount, int inputSlot)
  {
    if (inputSlot != 0)
      return;
    base.UpdateHasPower(inputAmount, inputSlot);
  }

  public bool GetDesiredState()
  {
    if (this.setAmount > 0 && this.resetAmount == 0 || this.setAmount > 0 && this.resetAmount > 0)
      return true;
    if (this.setAmount == 0 && this.resetAmount > 0)
      return false;
    if (this.toggleAmount > 0)
      return !this.IsOn();
    if (this.setAmount == 0 && this.resetAmount == 0)
      return this.IsOn();
    return false;
  }

  public void UpdateState()
  {
    if (!this.IsPowered())
      return;
    int num1 = this.IsOn() ? 1 : 0;
    this.SetFlag(BaseEntity.Flags.On, this.GetDesiredState(), false, true);
    int num2 = this.IsOn() ? 1 : 0;
    if (num1 == num2)
      return;
    this.MarkDirtyForceUpdateOutputs();
  }

  public override void UpdateFromInput(int inputAmount, int inputSlot)
  {
    switch (inputSlot)
    {
      case 0:
        base.UpdateFromInput(inputAmount, inputSlot);
        this.UpdateState();
        break;
      case 1:
        this.setAmount = inputAmount;
        this.UpdateState();
        break;
      case 2:
        this.resetAmount = inputAmount;
        this.UpdateState();
        break;
      case 3:
        this.toggleAmount = inputAmount;
        this.UpdateState();
        break;
    }
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    return base.GetPassthroughAmount(outputSlot);
  }

  public override void UpdateOutputs()
  {
    if (this.outputs.Length == 0)
    {
      this.ensureOutputsUpdated = false;
    }
    else
    {
      if (!this.ensureOutputsUpdated)
        return;
      if (Object.op_Inequality((Object) this.outputs[0].connectedTo.Get(true), (Object) null))
        this.outputs[0].connectedTo.Get(true).UpdateFromInput(this.IsOn() ? this.currentEnergy : 0, this.outputs[0].connectedToSlot);
      if (!Object.op_Inequality((Object) this.outputs[1].connectedTo.Get(true), (Object) null))
        return;
      this.outputs[1].connectedTo.Get(true).UpdateFromInput(this.IsOn() ? 0 : this.currentEnergy, this.outputs[1].connectedToSlot);
    }
  }
}
