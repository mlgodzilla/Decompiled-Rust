// Decompiled with JetBrains decompiler
// Type: SpawnFilter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class SpawnFilter
{
  [InspectorFlags]
  public TerrainSplat.Enum SplatType = (TerrainSplat.Enum) -1;
  [InspectorFlags]
  public TerrainBiome.Enum BiomeType = (TerrainBiome.Enum) -1;
  [InspectorFlags]
  public TerrainTopology.Enum TopologyAny = (TerrainTopology.Enum) -1;
  [InspectorFlags]
  public TerrainTopology.Enum TopologyAll;
  [InspectorFlags]
  public TerrainTopology.Enum TopologyNot;

  public bool Test(Vector3 worldPos)
  {
    return (double) this.GetFactor(worldPos) > 0.5;
  }

  public bool Test(float normX, float normZ)
  {
    return (double) this.GetFactor(normX, normZ) > 0.5;
  }

  public float GetFactor(Vector3 worldPos)
  {
    return this.GetFactor(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z));
  }

  public float GetFactor(float normX, float normZ)
  {
    if (Object.op_Equality((Object) TerrainMeta.TopologyMap, (Object) null))
      return 0.0f;
    int splatType = (int) this.SplatType;
    int biomeType = (int) this.BiomeType;
    int topologyAny = (int) this.TopologyAny;
    int topologyAll = (int) this.TopologyAll;
    int topologyNot = (int) this.TopologyNot;
    switch (topologyAny)
    {
      case -1:
        if (topologyAll != 0 || topologyNot != 0)
          goto default;
        else
          break;
      case 0:
        Debug.LogError((object) "Empty topology filter is invalid.");
        break;
      default:
        int topology = TerrainMeta.TopologyMap.GetTopology(normX, normZ);
        if (topologyAny != -1 && (topology & topologyAny) == 0 || topologyNot != 0 && (topology & topologyNot) != 0 || topologyAll != 0 && (topology & topologyAll) != topologyAll)
          return 0.0f;
        break;
    }
    switch (biomeType)
    {
      case -1:
        switch (splatType)
        {
          case -1:
            return 1f;
          case 0:
            Debug.LogError((object) "Empty splat filter is invalid.");
            goto case -1;
          default:
            return TerrainMeta.SplatMap.GetSplat(normX, normZ, splatType);
        }
      case 0:
        Debug.LogError((object) "Empty biome filter is invalid.");
        goto case -1;
      default:
        if ((TerrainMeta.BiomeMap.GetBiomeMaxType(normX, normZ, -1) & biomeType) == 0)
          return 0.0f;
        goto case -1;
    }
  }
}
