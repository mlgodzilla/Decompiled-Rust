// Decompiled with JetBrains decompiler
// Type: GenerateDecorTopology
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class GenerateDecorTopology : ProceduralComponent
{
  public bool KeepExisting = true;

  public override void Process(uint seed)
  {
    TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
    int topores = topomap.res;
    Parallel.For(0, topores, (Action<int>) (z =>
    {
      for (int x = 0; x < topores; ++x)
      {
        if (topomap.GetTopology(x, z, 4194306))
          topomap.AddTopology(x, z, 512);
        else if (!this.KeepExisting)
          topomap.RemoveTopology(x, z, 512);
      }
    }));
  }
}
