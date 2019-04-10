// Decompiled with JetBrains decompiler
// Type: MinMax
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class MinMax
{
  public float y = 1f;
  public float x;

  public MinMax(float x, float y)
  {
    this.x = x;
    this.y = y;
  }

  public float Random()
  {
    return Random.Range(this.x, this.y);
  }

  public float Lerp(float t)
  {
    return Mathf.Lerp(this.x, this.y, t);
  }

  public float Lerp(float a, float b, float t)
  {
    return Mathf.Lerp(this.x, this.y, Mathf.InverseLerp(a, b, t));
  }
}
