// Decompiled with JetBrains decompiler
// Type: ORSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ORSwitch : IOEntity
{
  private int input1Amount;
  private int input2Amount;

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    return Mathf.Max(0, Mathf.Max(this.input1Amount, this.input2Amount) - this.ConsumptionAmount());
  }

  public override void UpdateHasPower(int inputAmount, int inputSlot)
  {
    this.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
  }

  public override void IOStateChanged(int inputAmount, int inputSlot)
  {
    base.IOStateChanged(inputAmount, inputSlot);
  }

  public override void UpdateFromInput(int inputAmount, int slot)
  {
    if (this.IsConnectedTo((IOEntity) this, slot, IOEntity.backtracking, false))
      inputAmount = 0;
    switch (slot)
    {
      case 0:
        this.input1Amount = inputAmount;
        break;
      case 1:
        this.input2Amount = inputAmount;
        break;
    }
    int num = this.input1Amount + this.input2Amount;
    bool b = num > 0;
    this.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved3, b, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 || this.input2Amount > 0, false, false);
    this.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
    base.UpdateFromInput(inputAmount, slot);
  }
}
