// Decompiled with JetBrains decompiler
// Type: FloatConditions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;

[Serializable]
public class FloatConditions
{
  public FloatConditions.Condition[] conditions;

  public bool AllTrue(float val)
  {
    foreach (FloatConditions.Condition condition in this.conditions)
    {
      if (!condition.Test(val))
        return false;
    }
    return true;
  }

  [Serializable]
  public struct Condition
  {
    public FloatConditions.Condition.Types type;
    public float value;

    public bool Test(float val)
    {
      switch (this.type)
      {
        case FloatConditions.Condition.Types.Equal:
          return (double) val == (double) this.value;
        case FloatConditions.Condition.Types.NotEqual:
          return (double) val != (double) this.value;
        case FloatConditions.Condition.Types.Higher:
          return (double) val > (double) this.value;
        case FloatConditions.Condition.Types.Lower:
          return (double) val < (double) this.value;
        default:
          return false;
      }
    }

    public enum Types
    {
      Equal,
      NotEqual,
      Higher,
      Lower,
    }
  }
}
