// Decompiled with JetBrains decompiler
// Type: ProgressDoor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ProgressDoor : IOEntity
{
  public float energyForOpen = 1f;
  public float secondsToClose = 1f;
  public float storedEnergy;
  public float openProgress;

  public override void ResetIOState()
  {
    this.storedEnergy = 0.0f;
    this.UpdateProgress();
  }

  public override float IOInput(
    IOEntity from,
    IOEntity.IOType inputType,
    float inputAmount,
    int slot = 0)
  {
    if ((double) inputAmount > 0.0)
    {
      this.AddEnergy(inputAmount);
      if ((double) this.storedEnergy == (double) this.energyForOpen)
        return inputAmount;
      return 0.0f;
    }
    this.NoEnergy();
    return inputAmount;
  }

  public virtual void NoEnergy()
  {
  }

  public virtual void AddEnergy(float amount)
  {
    if ((double) amount <= 0.0)
      return;
    this.storedEnergy += amount;
    this.storedEnergy = Mathf.Clamp(this.storedEnergy, 0.0f, this.energyForOpen);
  }

  public virtual void UpdateProgress()
  {
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }
}
