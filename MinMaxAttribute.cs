// Decompiled with JetBrains decompiler
// Type: MinMaxAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MinMaxAttribute : PropertyAttribute
{
  public float min;
  public float max;

  public MinMaxAttribute(float min, float max)
  {
    this.\u002Ector();
    this.min = min;
    this.max = max;
  }
}
