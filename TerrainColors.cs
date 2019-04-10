// Decompiled with JetBrains decompiler
// Type: TerrainColors
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainColors : TerrainExtension
{
  private TerrainSplatMap splatMap;
  private TerrainBiomeMap biomeMap;

  public override void Setup()
  {
    this.splatMap = (TerrainSplatMap) ((Component) this.terrain).GetComponent<TerrainSplatMap>();
    this.biomeMap = (TerrainBiomeMap) ((Component) this.terrain).GetComponent<TerrainBiomeMap>();
  }

  public Color GetColor(Vector3 worldPos, int mask = -1)
  {
    return this.GetColor(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask);
  }

  public Color GetColor(float normX, float normZ, int mask = -1)
  {
    double biome1 = (double) this.biomeMap.GetBiome(normX, normZ, 1);
    float biome2 = this.biomeMap.GetBiome(normX, normZ, 2);
    float biome3 = this.biomeMap.GetBiome(normX, normZ, 4);
    float biome4 = this.biomeMap.GetBiome(normX, normZ, 8);
    TerrainConfig.SplatType splat = this.config.Splats[TerrainSplat.TypeToIndex(this.splatMap.GetSplatMaxType(normX, normZ, mask))];
    Color aridColor = splat.AridColor;
    return Color.op_Addition(Color.op_Addition(Color.op_Addition(Color.op_Multiply((float) biome1, aridColor), Color.op_Multiply(biome2, splat.TemperateColor)), Color.op_Multiply(biome3, splat.TundraColor)), Color.op_Multiply(biome4, splat.ArcticColor));
  }
}
