// Decompiled with JetBrains decompiler
// Type: TerrainPlacement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public abstract class TerrainPlacement : PrefabAttribute
{
  [ReadOnly]
  public Vector3 size = Vector3.get_zero();
  [ReadOnly]
  public Vector3 extents = Vector3.get_zero();
  [ReadOnly]
  public Vector3 offset = Vector3.get_zero();
  public bool HeightMap = true;
  public bool AlphaMap = true;
  public bool WaterMap;
  [InspectorFlags]
  public TerrainSplat.Enum SplatMask;
  [InspectorFlags]
  public TerrainBiome.Enum BiomeMask;
  [InspectorFlags]
  public TerrainTopology.Enum TopologyMask;
  [HideInInspector]
  public Texture2D heightmap;
  [HideInInspector]
  public Texture2D splatmap0;
  [HideInInspector]
  public Texture2D splatmap1;
  [HideInInspector]
  public Texture2D alphamap;
  [HideInInspector]
  public Texture2D biomemap;
  [HideInInspector]
  public Texture2D topologymap;
  [HideInInspector]
  public Texture2D watermap;
  [HideInInspector]
  public Texture2D blendmap;

  [ContextMenu("Refresh Terrain Data")]
  public void RefreshTerrainData()
  {
    TerrainData terrainData = Terrain.get_activeTerrain().get_terrainData();
    TerrainHeightMap component1 = (TerrainHeightMap) ((Component) Terrain.get_activeTerrain()).GetComponent<TerrainHeightMap>();
    if (Object.op_Implicit((Object) component1))
      this.heightmap = component1.HeightTexture;
    TerrainSplatMap component2 = (TerrainSplatMap) ((Component) Terrain.get_activeTerrain()).GetComponent<TerrainSplatMap>();
    if (Object.op_Implicit((Object) component2))
    {
      this.splatmap0 = component2.SplatTexture0;
      this.splatmap1 = component2.SplatTexture1;
    }
    TerrainAlphaMap component3 = (TerrainAlphaMap) ((Component) Terrain.get_activeTerrain()).GetComponent<TerrainAlphaMap>();
    if (Object.op_Implicit((Object) component3))
      this.alphamap = component3.AlphaTexture;
    TerrainBiomeMap component4 = (TerrainBiomeMap) ((Component) Terrain.get_activeTerrain()).GetComponent<TerrainBiomeMap>();
    if (Object.op_Implicit((Object) component4))
      this.biomemap = component4.BiomeTexture;
    TerrainTopologyMap component5 = (TerrainTopologyMap) ((Component) Terrain.get_activeTerrain()).GetComponent<TerrainTopologyMap>();
    if (Object.op_Implicit((Object) component5))
      this.topologymap = component5.TopologyTexture;
    TerrainWaterMap component6 = (TerrainWaterMap) ((Component) Terrain.get_activeTerrain()).GetComponent<TerrainWaterMap>();
    if (Object.op_Implicit((Object) component6))
      this.watermap = component6.WaterTexture;
    TerrainBlendMap component7 = (TerrainBlendMap) ((Component) Terrain.get_activeTerrain()).GetComponent<TerrainBlendMap>();
    if (Object.op_Implicit((Object) component7))
      this.blendmap = component7.BlendTexture;
    this.size = terrainData.get_size();
    this.extents = Vector3.op_Multiply(terrainData.get_size(), 0.5f);
    this.offset = Vector3.op_Subtraction(Vector3.op_Addition(Terrain.get_activeTerrain().GetPosition(), Vector3.op_Multiply(Vector3Ex.XZ3D(terrainData.get_size()), 0.5f)), ((Component) this).get_transform().get_position());
  }

  public void Apply(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
  {
    if (this.ShouldHeight())
      this.ApplyHeight(localToWorld, worldToLocal);
    if (this.ShouldSplat(-1))
      this.ApplySplat(localToWorld, worldToLocal);
    if (this.ShouldAlpha())
      this.ApplyAlpha(localToWorld, worldToLocal);
    if (this.ShouldBiome(-1))
      this.ApplyBiome(localToWorld, worldToLocal);
    if (this.ShouldTopology(-1))
      this.ApplyTopology(localToWorld, worldToLocal);
    if (!this.ShouldWater())
      return;
    this.ApplyWater(localToWorld, worldToLocal);
  }

  protected bool ShouldHeight()
  {
    if (Object.op_Inequality((Object) this.heightmap, (Object) null))
      return this.HeightMap;
    return false;
  }

  protected bool ShouldSplat(int id = -1)
  {
    if (Object.op_Inequality((Object) this.splatmap0, (Object) null) && Object.op_Inequality((Object) this.splatmap1, (Object) null))
      return (this.SplatMask & id) > 0;
    return false;
  }

  protected bool ShouldAlpha()
  {
    if (Object.op_Inequality((Object) this.alphamap, (Object) null))
      return this.AlphaMap;
    return false;
  }

  protected bool ShouldBiome(int id = -1)
  {
    if (Object.op_Inequality((Object) this.biomemap, (Object) null))
      return (this.BiomeMask & id) > 0;
    return false;
  }

  protected bool ShouldTopology(int id = -1)
  {
    if (Object.op_Inequality((Object) this.topologymap, (Object) null))
      return (this.TopologyMask & id) > 0;
    return false;
  }

  protected bool ShouldWater()
  {
    if (Object.op_Inequality((Object) this.watermap, (Object) null))
      return this.WaterMap;
    return false;
  }

  protected abstract void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

  protected abstract void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

  protected abstract void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

  protected abstract void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

  protected abstract void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

  protected abstract void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal);

  protected override System.Type GetIndexedType()
  {
    return typeof (TerrainPlacement);
  }
}
