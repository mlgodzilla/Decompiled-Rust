// Decompiled with JetBrains decompiler
// Type: TerrainGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class TerrainGenerator : SingletonComponent<TerrainGenerator>
{
  public TerrainConfig config;
  private const float HeightMapRes = 0.5f;
  private const float SplatMapRes = 0.5f;
  private const float BaseMapRes = 0.01f;

  public GameObject CreateTerrain()
  {
    TerrainData terrainData = new TerrainData();
    terrainData.set_baseMapResolution(Mathf.NextPowerOfTwo((int) ((double) World.Size * 0.00999999977648258)));
    terrainData.set_heightmapResolution(Mathf.NextPowerOfTwo((int) ((double) World.Size * 0.5)) + 1);
    terrainData.set_alphamapResolution(Mathf.NextPowerOfTwo((int) ((double) World.Size * 0.5)));
    terrainData.set_size(new Vector3((float) World.Size, 1000f, (float) World.Size));
    Terrain component = (Terrain) Terrain.CreateTerrainGameObject(terrainData).GetComponent<Terrain>();
    ((Component) component).get_transform().set_position(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3((float) -World.Size * 0.5f, 0.0f, (float) -World.Size * 0.5f)));
    component.set_castShadows(this.config.CastShadows);
    component.set_materialType((Terrain.MaterialType) 3);
    component.set_materialTemplate(this.config.Material);
    ((Component) component).get_gameObject().set_tag(((Component) this).get_gameObject().get_tag());
    ((Component) component).get_gameObject().set_layer(((Component) this).get_gameObject().get_layer());
    ((Collider) ((Component) component).get_gameObject().GetComponent<TerrainCollider>()).set_sharedMaterial(this.config.GenericMaterial);
    M0 m0 = ((Component) component).get_gameObject().AddComponent<TerrainMeta>();
    ((Component) component).get_gameObject().AddComponent<TerrainPhysics>();
    ((Component) component).get_gameObject().AddComponent<TerrainColors>();
    ((Component) component).get_gameObject().AddComponent<TerrainCollision>();
    ((Component) component).get_gameObject().AddComponent<TerrainBiomeMap>();
    ((Component) component).get_gameObject().AddComponent<TerrainAlphaMap>();
    ((Component) component).get_gameObject().AddComponent<TerrainHeightMap>();
    ((Component) component).get_gameObject().AddComponent<TerrainSplatMap>();
    ((Component) component).get_gameObject().AddComponent<TerrainTopologyMap>();
    ((Component) component).get_gameObject().AddComponent<TerrainWaterMap>();
    ((Component) component).get_gameObject().AddComponent<TerrainPath>();
    ((TerrainMeta) m0).terrain = component;
    ((TerrainMeta) m0).config = this.config;
    Object.DestroyImmediate((Object) ((Component) this).get_gameObject());
    return ((Component) component).get_gameObject();
  }

  public TerrainGenerator()
  {
    base.\u002Ector();
  }
}
