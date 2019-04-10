// Decompiled with JetBrains decompiler
// Type: CableTunnel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class CableTunnel : IOEntity
{
  private int[] inputAmounts = new int[4];
  private const int numChannels = 4;

  public override bool WantsPower()
  {
    return true;
  }

  public override void IOStateChanged(int inputAmount, int inputSlot)
  {
    int inputAmount1 = this.inputAmounts[inputSlot];
    this.inputAmounts[inputSlot] = inputAmount;
    if (inputAmount != inputAmount1)
      this.ensureOutputsUpdated = true;
    base.IOStateChanged(inputAmount, inputSlot);
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
      for (int index = 0; index < 4; ++index)
      {
        IOEntity.IOSlot output = this.outputs[index];
        if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null))
          output.connectedTo.Get(true).UpdateFromInput(this.inputAmounts[index], output.connectedToSlot);
      }
    }
  }
}
