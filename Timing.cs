// Decompiled with JetBrains decompiler
// Type: Timing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Diagnostics;
using UnityEngine;

public struct Timing
{
  private Stopwatch sw;
  private string name;

  public static Timing Start(string name)
  {
    return new Timing(name);
  }

  public void End()
  {
    if (this.sw.Elapsed.TotalSeconds <= 0.300000011920929)
      return;
    Debug.Log((object) ("[" + this.sw.Elapsed.TotalSeconds.ToString("0.0") + "s] " + this.name));
  }

  public Timing(string name)
  {
    this.sw = Stopwatch.StartNew();
    this.name = name;
  }
}
