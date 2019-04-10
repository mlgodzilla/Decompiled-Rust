// Decompiled with JetBrains decompiler
// Type: Profile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Diagnostics;
using UnityEngine;

public class Profile
{
  public Stopwatch watch = new Stopwatch();
  public string category;
  public string name;
  public float warnTime;

  public Profile(string cat, string nam, float WarnTime = 1f)
  {
    this.category = cat;
    this.name = nam;
    this.warnTime = WarnTime;
  }

  public void Start()
  {
    this.watch.Reset();
    this.watch.Start();
  }

  public void Stop()
  {
    this.watch.Stop();
    if ((double) this.watch.Elapsed.Seconds <= (double) this.warnTime)
      return;
    Debug.Log((object) (this.category + "." + this.name + ": Took " + (object) this.watch.Elapsed.Seconds + " seconds"));
  }
}
