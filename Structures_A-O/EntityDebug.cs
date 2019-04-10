// Decompiled with JetBrains decompiler
// Type: EntityDebug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Diagnostics;
using UnityEngine;

public class EntityDebug : EntityComponent<BaseEntity>
{
  internal Stopwatch stopwatch = Stopwatch.StartNew();

  private void Update()
  {
    if (!this.baseEntity.IsValid() || !this.baseEntity.IsDebugging())
    {
      ((Behaviour) this).set_enabled(false);
    }
    else
    {
      if (this.stopwatch.Elapsed.TotalSeconds < 0.5)
        return;
      int num = this.baseEntity.isClient ? 1 : 0;
      if (this.baseEntity.isServer)
        this.baseEntity.DebugServer(1, (float) this.stopwatch.Elapsed.TotalSeconds);
      this.stopwatch.Reset();
      this.stopwatch.Start();
    }
  }
}
