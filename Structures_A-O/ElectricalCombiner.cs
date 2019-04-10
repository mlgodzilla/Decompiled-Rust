// Decompiled with JetBrains decompiler
// Type: ElectricalCombiner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ElectricalCombiner : IOEntity
{
  public int input1Amount;
  public int input2Amount;
  private bool wasShorted;

  public override bool IsRootEntity()
  {
    return true;
  }

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    int num = this.input1Amount + this.input2Amount;
    Mathf.Clamp(num - 1, 0, num);
    return num;
  }

  public override void UpdateHasPower(int inputAmount, int inputSlot)
  {
    this.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
  }

  public override void UpdateFromInput(int inputAmount, int slot)
  {
    bool b1;
    if (this.IsConnectedTo((IOEntity) this, slot, IOEntity.backtracking * 2, true) && inputAmount > 0)
    {
      inputAmount = 0;
      b1 = true;
    }
    else
      b1 = false;
    if (this.wasShorted != b1)
    {
      this.SetFlag(BaseEntity.Flags.Reserved7, b1, false, true);
      this.wasShorted = b1;
    }
    switch (slot)
    {
      case 0:
        this.input1Amount = inputAmount;
        break;
      case 1:
        this.input2Amount = inputAmount;
        break;
    }
    int inputAmount1 = this.input1Amount + this.input2Amount;
    bool b2 = inputAmount1 > 0;
    this.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved3, b2, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 || this.input2Amount > 0, false, false);
    this.SetFlag(BaseEntity.Flags.On, inputAmount1 > 0, false, true);
    base.UpdateFromInput(inputAmount1, slot);
  }
}
