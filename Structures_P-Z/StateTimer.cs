// Decompiled with JetBrains decompiler
// Type: StateTimer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public struct StateTimer
{
  public float ReleaseTime;
  public Action OnFinished;

  public void Activate(float seconds, Action onFinished = null)
  {
    this.ReleaseTime = Time.get_time() + seconds;
    this.OnFinished = onFinished;
  }

  public bool IsActive
  {
    get
    {
      int num = (double) this.ReleaseTime > (double) Time.get_time() ? 1 : 0;
      if (num != 0)
        return num != 0;
      if (this.OnFinished == null)
        return num != 0;
      this.OnFinished();
      this.OnFinished = (Action) null;
      return num != 0;
    }
  }
}
