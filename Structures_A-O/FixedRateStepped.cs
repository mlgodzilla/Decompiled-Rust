// Decompiled with JetBrains decompiler
// Type: FixedRateStepped
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class FixedRateStepped
{
  public float rate = 0.1f;
  public int maxSteps = 3;
  internal float nextCall;

  public bool ShouldStep()
  {
    if ((double) this.nextCall > (double) Time.get_time())
      return false;
    if ((double) this.nextCall == 0.0)
      this.nextCall = Time.get_time();
    if ((double) this.nextCall + (double) this.rate * (double) this.maxSteps < (double) Time.get_time())
      this.nextCall = Time.get_time() - this.rate * (float) this.maxSteps;
    this.nextCall += this.rate;
    return true;
  }
}
