// Decompiled with JetBrains decompiler
// Type: GenerateTextures
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class GenerateTextures : ProceduralComponent
{
  public override void Process(uint seed)
  {
    if (World.Cached)
      return;
    World.AddMap("height", TerrainMeta.HeightMap.ToByteArray());
    World.AddMap("splat", TerrainMeta.SplatMap.ToByteArray());
    World.AddMap("biome", TerrainMeta.BiomeMap.ToByteArray());
    World.AddMap("topology", TerrainMeta.TopologyMap.ToByteArray());
    World.AddMap("alpha", TerrainMeta.AlphaMap.ToByteArray());
    World.AddMap("water", TerrainMeta.WaterMap.ToByteArray());
  }

  public override bool RunOnCache
  {
    get
    {
      return true;
    }
  }
}
