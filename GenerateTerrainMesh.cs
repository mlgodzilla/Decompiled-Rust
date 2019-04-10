// Decompiled with JetBrains decompiler
// Type: GenerateTerrainMesh
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

public class GenerateTerrainMesh : ProceduralComponent
{
  public override void Process(uint seed)
  {
    if (!World.Cached)
      World.AddMap("terrain", TerrainMeta.HeightMap.ToByteArray());
    TerrainMeta.HeightMap.ApplyToTerrain();
    if (!World.Cached)
      return;
    TerrainMeta.HeightMap.FromByteArray(World.GetMap("height"));
  }

  public override bool RunOnCache
  {
    get
    {
      return true;
    }
  }
}
