﻿// Decompiled with JetBrains decompiler
// Type: TerrainTopologySet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainTopologySet : TerrainModifier
{
  [InspectorFlags]
  public TerrainTopology.Enum TopologyType = (TerrainTopology.Enum) 512;

  protected override void Apply(Vector3 position, float opacity, float radius, float fade)
  {
    if (!Object.op_Implicit((Object) TerrainMeta.TopologyMap))
      return;
    TerrainMeta.TopologyMap.SetTopology(position, (int) this.TopologyType, radius, fade);
  }
}
