// Decompiled with JetBrains decompiler
// Type: VitalLevel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public struct VitalLevel
{
  public float Level;
  private float lastUsedTime;

  internal void Add(float f)
  {
    this.Level += f;
    if ((double) this.Level > 1.0)
      this.Level = 1f;
    if ((double) this.Level >= 0.0)
      return;
    this.Level = 0.0f;
  }

  public float TimeSinceUsed
  {
    get
    {
      return Time.get_time() - this.lastUsedTime;
    }
  }

  internal void Use(float f)
  {
    if (Mathf.Approximately(f, 0.0f))
      return;
    this.Level -= Mathf.Abs(f);
    if ((double) this.Level < 0.0)
      this.Level = 0.0f;
    this.lastUsedTime = Time.get_time();
  }
}
