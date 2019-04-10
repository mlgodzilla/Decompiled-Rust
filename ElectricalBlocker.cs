// Decompiled with JetBrains decompiler
// Type: ElectricalBlocker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class ElectricalBlocker : IOEntity
{
  protected int input1Amount;
  protected int input2Amount;

  public override int GetPassthroughAmount(int outputSlot = 0)
  {
    return base.GetPassthroughAmount(outputSlot) * (this.IsOn() ? 0 : 1);
  }

  public override bool WantsPower()
  {
    return !this.IsOn();
  }

  public override void UpdateHasPower(int inputAmount, int inputSlot)
  {
    base.UpdateHasPower(inputAmount, inputSlot);
    this.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
  }

  public override void IOStateChanged(int inputAmount, int inputSlot)
  {
    base.IOStateChanged(inputAmount, inputSlot);
  }

  public virtual void UpdateBlocked()
  {
    int num1 = this.IsOn() ? 1 : 0;
    this.SetFlag(BaseEntity.Flags.On, this.input1Amount > 0, false, false);
    this.SetFlag(BaseEntity.Flags.Reserved8, this.IsOn(), false, false);
    this.UpdateHasPower(this.input1Amount + this.input2Amount, 1);
    int num2 = this.IsOn() ? 1 : 0;
    if (num1 == num2)
      return;
    this.MarkDirty();
  }

  public override void UpdateFromInput(int inputAmount, int inputSlot)
  {
    if (inputSlot == 1)
    {
      this.input1Amount = inputAmount;
      this.UpdateBlocked();
    }
    else
    {
      if (inputSlot != 0)
        return;
      this.input2Amount = inputAmount;
      base.UpdateFromInput(inputAmount, inputSlot);
    }
  }
}
