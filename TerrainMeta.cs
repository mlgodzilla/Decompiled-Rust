// Decompiled with JetBrains decompiler
// Type: TerrainMeta
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Oxide.Core;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainMeta : MonoBehaviour
{
  public Terrain terrain;
  public TerrainConfig config;
  public TerrainMeta.PaintMode paint;
  [HideInInspector]
  public TerrainMeta.PaintMode currentPaintMode;

  public static TerrainConfig Config { get; private set; }

  public static Terrain Terrain { get; private set; }

  public static Transform Transform { get; private set; }

  public static Vector3 Position { get; private set; }

  public static Vector3 Size { get; private set; }

  public static Vector3 Center
  {
    get
    {
      return Vector3.op_Addition(TerrainMeta.Position, Vector3.op_Multiply(TerrainMeta.Size, 0.5f));
    }
  }

  public static Vector3 OneOverSize { get; private set; }

  public static Vector3 HighestPoint { get; set; }

  public static Vector3 LowestPoint { get; set; }

  public static float LootAxisAngle { get; private set; }

  public static float BiomeAxisAngle { get; private set; }

  public static TerrainData Data { get; private set; }

  public static TerrainCollider Collider { get; private set; }

  public static TerrainCollision Collision { get; private set; }

  public static TerrainPhysics Physics { get; private set; }

  public static TerrainColors Colors { get; private set; }

  public static TerrainQuality Quality { get; private set; }

  public static TerrainPath Path { get; private set; }

  public static TerrainBiomeMap BiomeMap { get; private set; }

  public static TerrainAlphaMap AlphaMap { get; private set; }

  public static TerrainBlendMap BlendMap { get; private set; }

  public static TerrainHeightMap HeightMap { get; private set; }

  public static TerrainSplatMap SplatMap { get; private set; }

  public static TerrainTopologyMap TopologyMap { get; private set; }

  public static TerrainWaterMap WaterMap { get; private set; }

  public static TerrainDistanceMap DistanceMap { get; private set; }

  public static TerrainTexturing Texturing { get; private set; }

  public static bool OutOfBounds(Vector3 worldPos)
  {
    return worldPos.x < TerrainMeta.Position.x || worldPos.z < TerrainMeta.Position.z || (worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x || worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z);
  }

  public static bool OutOfMargin(Vector3 worldPos)
  {
    return worldPos.x < TerrainMeta.Position.x - TerrainMeta.Size.x || worldPos.z < TerrainMeta.Position.z - TerrainMeta.Size.z || (worldPos.x > TerrainMeta.Position.x + TerrainMeta.Size.x + TerrainMeta.Size.x || worldPos.z > TerrainMeta.Position.z + TerrainMeta.Size.z + TerrainMeta.Size.z);
  }

  public static Vector3 Normalize(Vector3 worldPos)
  {
    // ISSUE: variable of the null type
    __Null local = (worldPos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
    float num1 = (float) ((worldPos.y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y);
    float num2 = (float) ((worldPos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z);
    double num3 = (double) num1;
    double num4 = (double) num2;
    return new Vector3((float) local, (float) num3, (float) num4);
  }

  public static float NormalizeX(float x)
  {
    return (float) (((double) x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x);
  }

  public static float NormalizeY(float y)
  {
    return (float) (((double) y - TerrainMeta.Position.y) * TerrainMeta.OneOverSize.y);
  }

  public static float NormalizeZ(float z)
  {
    return (float) (((double) z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z);
  }

  public static Vector3 Denormalize(Vector3 normPos)
  {
    // ISSUE: variable of the null type
    __Null local = TerrainMeta.Position.x + normPos.x * TerrainMeta.Size.x;
    float num1 = (float) (TerrainMeta.Position.y + normPos.y * TerrainMeta.Size.y);
    float num2 = (float) (TerrainMeta.Position.z + normPos.z * TerrainMeta.Size.z);
    double num3 = (double) num1;
    double num4 = (double) num2;
    return new Vector3((float) local, (float) num3, (float) num4);
  }

  public static float DenormalizeX(float normX)
  {
    return (float) (TerrainMeta.Position.x + (double) normX * TerrainMeta.Size.x);
  }

  public static float DenormalizeY(float normY)
  {
    return (float) (TerrainMeta.Position.y + (double) normY * TerrainMeta.Size.y);
  }

  public static float DenormalizeZ(float normZ)
  {
    return (float) (TerrainMeta.Position.z + (double) normZ * TerrainMeta.Size.z);
  }

  protected void Awake()
  {
    if (!Application.get_isPlaying())
      return;
    Shader.DisableKeyword("TERRAIN_PAINTING");
  }

  public void Init(Terrain terrainOverride = null, TerrainConfig configOverride = null)
  {
    if (Object.op_Inequality((Object) terrainOverride, (Object) null))
      this.terrain = terrainOverride;
    if (Object.op_Inequality((Object) configOverride, (Object) null))
      this.config = configOverride;
    TerrainMeta.Terrain = this.terrain;
    TerrainMeta.Config = this.config;
    TerrainMeta.Transform = ((Component) this.terrain).get_transform();
    TerrainMeta.Data = this.terrain.get_terrainData();
    TerrainMeta.Size = this.terrain.get_terrainData().get_size();
    TerrainMeta.OneOverSize = Vector3Ex.Inverse(TerrainMeta.Size);
    TerrainMeta.Position = this.terrain.GetPosition();
    TerrainMeta.Collider = (TerrainCollider) ((Component) this.terrain).GetComponent<TerrainCollider>();
    TerrainMeta.Collision = (TerrainCollision) ((Component) this.terrain).GetComponent<TerrainCollision>();
    TerrainMeta.Physics = (TerrainPhysics) ((Component) this.terrain).GetComponent<TerrainPhysics>();
    TerrainMeta.Colors = (TerrainColors) ((Component) this.terrain).GetComponent<TerrainColors>();
    TerrainMeta.Quality = (TerrainQuality) ((Component) this.terrain).GetComponent<TerrainQuality>();
    TerrainMeta.Path = (TerrainPath) ((Component) this.terrain).GetComponent<TerrainPath>();
    TerrainMeta.BiomeMap = (TerrainBiomeMap) ((Component) this.terrain).GetComponent<TerrainBiomeMap>();
    TerrainMeta.AlphaMap = (TerrainAlphaMap) ((Component) this.terrain).GetComponent<TerrainAlphaMap>();
    TerrainMeta.BlendMap = (TerrainBlendMap) ((Component) this.terrain).GetComponent<TerrainBlendMap>();
    TerrainMeta.HeightMap = (TerrainHeightMap) ((Component) this.terrain).GetComponent<TerrainHeightMap>();
    TerrainMeta.SplatMap = (TerrainSplatMap) ((Component) this.terrain).GetComponent<TerrainSplatMap>();
    TerrainMeta.TopologyMap = (TerrainTopologyMap) ((Component) this.terrain).GetComponent<TerrainTopologyMap>();
    TerrainMeta.WaterMap = (TerrainWaterMap) ((Component) this.terrain).GetComponent<TerrainWaterMap>();
    TerrainMeta.DistanceMap = (TerrainDistanceMap) ((Component) this.terrain).GetComponent<TerrainDistanceMap>();
    TerrainMeta.Texturing = (TerrainTexturing) ((Component) this.terrain).GetComponent<TerrainTexturing>();
    TerrainMeta.HighestPoint = new Vector3((float) TerrainMeta.Position.x, (float) (TerrainMeta.Position.y + TerrainMeta.Size.y), (float) TerrainMeta.Position.z);
    TerrainMeta.LowestPoint = new Vector3((float) TerrainMeta.Position.x, (float) TerrainMeta.Position.y, (float) TerrainMeta.Position.z);
    foreach (TerrainExtension component in (TerrainExtension[]) ((Component) this).GetComponents<TerrainExtension>())
      component.Init(this.terrain, this.config);
    uint seed = World.Seed;
    int num1 = SeedRandom.Range(ref seed, 0, 4) * 90;
    int num2 = SeedRandom.Range(ref seed, -45, 46);
    int num3 = SeedRandom.Sign(ref seed);
    TerrainMeta.LootAxisAngle = (float) num1;
    TerrainMeta.BiomeAxisAngle = (float) (num1 + num2 + num3 * 90);
  }

  public static void InitNoTerrain()
  {
    TerrainMeta.Size = new Vector3(4096f, 4096f, 4096f);
  }

  public void SetupComponents()
  {
    foreach (TerrainExtension component in (TerrainExtension[]) ((Component) this).GetComponents<TerrainExtension>())
    {
      component.Setup();
      component.isInitialized = true;
    }
  }

  public void PostSetupComponents()
  {
    foreach (TerrainExtension component in (TerrainExtension[]) ((Component) this).GetComponents<TerrainExtension>())
      component.PostSetup();
    Interface.CallHook("OnTerrainInitialized");
  }

  public void BindShaderProperties()
  {
    if (Object.op_Implicit((Object) this.config))
    {
      Shader.SetGlobalTexture("Terrain_AlbedoArray", this.config.AlbedoArray);
      Shader.SetGlobalTexture("Terrain_NormalArray", this.config.NormalArray);
      Shader.SetGlobalVector("Terrain_TexelSize", Vector4.op_Implicit(new Vector2(1f / this.config.GetMinSplatTiling(), 1f / this.config.GetMinSplatTiling())));
      Shader.SetGlobalVector("Terrain_TexelSize0", new Vector4(1f / this.config.Splats[0].SplatTiling, 1f / this.config.Splats[1].SplatTiling, 1f / this.config.Splats[2].SplatTiling, 1f / this.config.Splats[3].SplatTiling));
      Shader.SetGlobalVector("Terrain_TexelSize1", new Vector4(1f / this.config.Splats[4].SplatTiling, 1f / this.config.Splats[5].SplatTiling, 1f / this.config.Splats[6].SplatTiling, 1f / this.config.Splats[7].SplatTiling));
      Shader.SetGlobalVector("Splat0_UVMIX", Vector4.op_Implicit(new Vector3(this.config.Splats[0].UVMIXMult, this.config.Splats[0].UVMIXStart, 1f / this.config.Splats[0].UVMIXDist)));
      Shader.SetGlobalVector("Splat1_UVMIX", Vector4.op_Implicit(new Vector3(this.config.Splats[1].UVMIXMult, this.config.Splats[1].UVMIXStart, 1f / this.config.Splats[1].UVMIXDist)));
      Shader.SetGlobalVector("Splat2_UVMIX", Vector4.op_Implicit(new Vector3(this.config.Splats[2].UVMIXMult, this.config.Splats[2].UVMIXStart, 1f / this.config.Splats[2].UVMIXDist)));
      Shader.SetGlobalVector("Splat3_UVMIX", Vector4.op_Implicit(new Vector3(this.config.Splats[3].UVMIXMult, this.config.Splats[3].UVMIXStart, 1f / this.config.Splats[3].UVMIXDist)));
      Shader.SetGlobalVector("Splat4_UVMIX", Vector4.op_Implicit(new Vector3(this.config.Splats[4].UVMIXMult, this.config.Splats[4].UVMIXStart, 1f / this.config.Splats[4].UVMIXDist)));
      Shader.SetGlobalVector("Splat5_UVMIX", Vector4.op_Implicit(new Vector3(this.config.Splats[5].UVMIXMult, this.config.Splats[5].UVMIXStart, 1f / this.config.Splats[5].UVMIXDist)));
      Shader.SetGlobalVector("Splat6_UVMIX", Vector4.op_Implicit(new Vector3(this.config.Splats[6].UVMIXMult, this.config.Splats[6].UVMIXStart, 1f / this.config.Splats[6].UVMIXDist)));
      Shader.SetGlobalVector("Splat7_UVMIX", Vector4.op_Implicit(new Vector3(this.config.Splats[7].UVMIXMult, this.config.Splats[7].UVMIXStart, 1f / this.config.Splats[7].UVMIXDist)));
    }
    if (Object.op_Implicit((Object) TerrainMeta.HeightMap))
      Shader.SetGlobalTexture("Terrain_Normal", (Texture) TerrainMeta.HeightMap.NormalTexture);
    if (Object.op_Implicit((Object) TerrainMeta.AlphaMap))
      Shader.SetGlobalTexture("Terrain_Alpha", (Texture) TerrainMeta.AlphaMap.AlphaTexture);
    if (Object.op_Implicit((Object) TerrainMeta.BiomeMap))
      Shader.SetGlobalTexture("Terrain_Biome", (Texture) TerrainMeta.BiomeMap.BiomeTexture);
    if (Object.op_Implicit((Object) TerrainMeta.SplatMap))
    {
      Shader.SetGlobalTexture("Terrain_Control0", (Texture) TerrainMeta.SplatMap.SplatTexture0);
      Shader.SetGlobalTexture("Terrain_Control1", (Texture) TerrainMeta.SplatMap.SplatTexture1);
    }
    Object.op_Implicit((Object) TerrainMeta.WaterMap);
    if (Object.op_Implicit((Object) TerrainMeta.DistanceMap))
      Shader.SetGlobalTexture("Terrain_Distance", (Texture) TerrainMeta.DistanceMap.DistanceTexture);
    if (!Object.op_Implicit((Object) this.terrain))
      return;
    Shader.SetGlobalVector("Terrain_Position", Vector4.op_Implicit(TerrainMeta.Position));
    Shader.SetGlobalVector("Terrain_Size", Vector4.op_Implicit(TerrainMeta.Size));
    Shader.SetGlobalVector("Terrain_RcpSize", Vector4.op_Implicit(TerrainMeta.OneOverSize));
    if (!Object.op_Implicit((Object) this.terrain.get_materialTemplate()))
      return;
    if (this.terrain.get_materialTemplate().IsKeywordEnabled("_TERRAIN_BLEND_LINEAR"))
      this.terrain.get_materialTemplate().DisableKeyword("_TERRAIN_BLEND_LINEAR");
    if (!this.terrain.get_materialTemplate().IsKeywordEnabled("_TERRAIN_VERTEX_NORMALS"))
      return;
    this.terrain.get_materialTemplate().DisableKeyword("_TERRAIN_VERTEX_NORMALS");
  }

  public TerrainMeta()
  {
    base.\u002Ector();
  }

  public enum PaintMode
  {
    None,
    Splats,
    Biomes,
    Alpha,
    Blend,
    Field,
    Cliff,
    Summit,
    Beachside,
    Beach,
    Forest,
    Forestside,
    Ocean,
    Oceanside,
    Decor,
    Monument,
    Road,
    Roadside,
    Bridge,
    River,
    Riverside,
    Lake,
    Lakeside,
    Offshore,
    Powerline,
    Runway,
    Building,
    Cliffside,
    Mountain,
    Clutter,
    Alt,
    Tier0,
    Tier1,
    Tier2,
    Mainland,
    Hilltop,
  }
}
