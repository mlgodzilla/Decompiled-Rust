// Decompiled with JetBrains decompiler
// Type: MetabolismAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class MetabolismAttribute
{
  public float startMin;
  public float startMax;
  public float min;
  public float max;
  public float value;
  public float lastValue;
  internal float lastGreatFraction;
  private const float greatInterval = 0.1f;

  public float greatFraction
  {
    get
    {
      return Mathf.Floor(this.Fraction() / 0.1f) / 10f;
    }
  }

  public void Reset()
  {
    this.value = Mathf.Clamp(Random.Range(this.startMin, this.startMax), this.min, this.max);
  }

  public float Fraction()
  {
    return Mathf.InverseLerp(this.min, this.max, this.value);
  }

  public float InverseFraction()
  {
    return 1f - this.Fraction();
  }

  public void Add(float val)
  {
    this.value = Mathf.Clamp(this.value + val, this.min, this.max);
  }

  public void Subtract(float val)
  {
    this.value = Mathf.Clamp(this.value - val, this.min, this.max);
  }

  public void Increase(float fTarget)
  {
    fTarget = Mathf.Clamp(fTarget, this.min, this.max);
    if ((double) fTarget <= (double) this.value)
      return;
    this.value = fTarget;
  }

  public void MoveTowards(float fTarget, float fRate)
  {
    if ((double) fRate == 0.0)
      return;
    this.value = Mathf.Clamp(Mathf.MoveTowards(this.value, fTarget, fRate), this.min, this.max);
  }

  public bool HasChanged()
  {
    int num = (double) this.lastValue != (double) this.value ? 1 : 0;
    this.lastValue = this.value;
    return num != 0;
  }

  public bool HasGreatlyChanged()
  {
    float greatFraction = this.greatFraction;
    int num = (double) this.lastGreatFraction != (double) greatFraction ? 1 : 0;
    this.lastGreatFraction = greatFraction;
    return num != 0;
  }

  public void SetValue(float newValue)
  {
    this.value = newValue;
  }

  public enum Type
  {
    Calories,
    Hydration,
    Heartrate,
    Poison,
    Radiation,
    Bleeding,
    Health,
    HealthOverTime,
  }
}
