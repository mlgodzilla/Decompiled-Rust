// Decompiled with JetBrains decompiler
// Type: RANDSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RANDSwitch : ElectricalBlocker
{
  private bool rand;

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    return this.GetCurrentEnergy() * (this.IsOn() ? 1 : 0);
  }

  public override void UpdateBlocked()
  {
    int num1 = this.IsOn() ? 1 : 0;
    this.SetFlag(BaseEntity.Flags.On, this.rand, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved8, this.rand, false, false);
    this.UpdateHasPower(this.input1Amount + this.input2Amount, 1);
    int num2 = this.IsOn() ? 1 : 0;
    if (num1 == num2)
      return;
    this.MarkDirty();
  }

  public bool RandomRoll()
  {
    return Random.Range(0, 2) == 1;
  }

  public override void UpdateFromInput(int inputAmount, int inputSlot)
  {
    if (inputSlot == 1 && inputAmount > 0)
    {
      this.input1Amount = inputAmount;
      this.rand = this.RandomRoll();
      this.UpdateBlocked();
    }
    if (inputSlot == 2 && inputAmount > 0)
    {
      this.rand = false;
      this.UpdateBlocked();
    }
    else
      base.UpdateFromInput(inputAmount, inputSlot);
  }
}
