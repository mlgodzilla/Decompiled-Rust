// Decompiled with JetBrains decompiler
// Type: Mountain
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class Mountain : TerrainPlacement
{
  public float Fade = 10f;

  protected void OnDrawGizmosSelected()
  {
    Vector3 vector3 = Vector3.op_Multiply(Vector3.get_up(), 0.5f * this.Fade);
    Gizmos.set_color(new Color(0.5f, 0.5f, 0.5f, 1f));
    Gizmos.DrawCube(Vector3.op_Addition(((Component) this).get_transform().get_position(), vector3), new Vector3((float) this.size.x, this.Fade, (float) this.size.z));
    Gizmos.DrawWireCube(Vector3.op_Addition(((Component) this).get_transform().get_position(), vector3), new Vector3((float) this.size.x, this.Fade, (float) this.size.z));
  }

  protected override void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
  {
    Vector3 position = ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.get_zero());
    TextureData heightdata = new TextureData(this.heightmap);
    TerrainMeta.HeightMap.ForEachParallel(((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) -this.extents.x, 0.0f, (float) -this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) this.extents.x, 0.0f, (float) -this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) -this.extents.x, 0.0f, (float) this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) this.extents.x, 0.0f, (float) this.extents.z))), (Action<int, int>) ((x, z) =>
    {
      float normZ = TerrainMeta.HeightMap.Coordinate(z);
      float normX = TerrainMeta.HeightMap.Coordinate(x);
      Vector3 vector3_1;
      ((Vector3) ref vector3_1).\u002Ector(TerrainMeta.DenormalizeX(normX), 0.0f, TerrainMeta.DenormalizeZ(normZ));
      Vector3 vector3_2 = Vector3.op_Subtraction(((Matrix4x4) ref worldToLocal).MultiplyPoint3x4(vector3_1), this.offset);
      float y = (float) (position.y + this.offset.y + (double) heightdata.GetInterpolatedHalf((float) ((vector3_2.x + this.extents.x) / this.size.x), (float) ((vector3_2.z + this.extents.z) / this.size.z)) * this.size.y);
      float opacity = Mathf.InverseLerp((float) position.y, (float) position.y + this.Fade, y);
      if ((double) opacity == 0.0)
        return;
      float num = TerrainMeta.NormalizeY(y);
      float height = Mathx.SmoothMax(TerrainMeta.HeightMap.GetHeight01(x, z), num, 0.1f);
      TerrainMeta.HeightMap.SetHeight(x, z, height, opacity);
    }));
  }

  protected override void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
  {
    bool should0 = this.ShouldSplat(1);
    bool should1 = this.ShouldSplat(2);
    bool should2 = this.ShouldSplat(4);
    bool should3 = this.ShouldSplat(8);
    bool should4 = this.ShouldSplat(16);
    bool should5 = this.ShouldSplat(32);
    bool should6 = this.ShouldSplat(64);
    bool should7 = this.ShouldSplat(128);
    if (!should0 && !should1 && (!should2 && !should3) && (!should4 && !should5 && (!should6 && !should7)))
      return;
    Vector3 position = ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.get_zero());
    TextureData heightdata = new TextureData(this.heightmap);
    TextureData splat0data = new TextureData(this.splatmap0);
    TextureData splat1data = new TextureData(this.splatmap1);
    TerrainMeta.SplatMap.ForEachParallel(((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) -this.extents.x, 0.0f, (float) -this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) this.extents.x, 0.0f, (float) -this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) -this.extents.x, 0.0f, (float) this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) this.extents.x, 0.0f, (float) this.extents.z))), (Action<int, int>) ((x, z) =>
    {
      float normZ = TerrainMeta.SplatMap.Coordinate(z);
      float normX = TerrainMeta.SplatMap.Coordinate(x);
      Vector3 vector3_1;
      ((Vector3) ref vector3_1).\u002Ector(TerrainMeta.DenormalizeX(normX), 0.0f, TerrainMeta.DenormalizeZ(normZ));
      Vector3 vector3_2 = Vector3.op_Subtraction(((Matrix4x4) ref worldToLocal).MultiplyPoint3x4(vector3_1), this.offset);
      float opacity = Mathf.InverseLerp((float) position.y, (float) position.y + this.Fade, (float) (position.y + this.offset.y + (double) heightdata.GetInterpolatedHalf((float) ((vector3_2.x + this.extents.x) / this.size.x), (float) ((vector3_2.z + this.extents.z) / this.size.z)) * this.size.y));
      if ((double) opacity == 0.0)
        return;
      Vector4 interpolatedVector1 = splat0data.GetInterpolatedVector((float) ((vector3_2.x + this.extents.x) / this.size.x), (float) ((vector3_2.z + this.extents.z) / this.size.z));
      Vector4 interpolatedVector2 = splat1data.GetInterpolatedVector((float) ((vector3_2.x + this.extents.x) / this.size.x), (float) ((vector3_2.z + this.extents.z) / this.size.z));
      if (!should0)
        interpolatedVector1.x = (__Null) 0.0;
      if (!should1)
        interpolatedVector1.y = (__Null) 0.0;
      if (!should2)
        interpolatedVector1.z = (__Null) 0.0;
      if (!should3)
        interpolatedVector1.w = (__Null) 0.0;
      if (!should4)
        interpolatedVector2.x = (__Null) 0.0;
      if (!should5)
        interpolatedVector2.y = (__Null) 0.0;
      if (!should6)
        interpolatedVector2.z = (__Null) 0.0;
      if (!should7)
        interpolatedVector2.w = (__Null) 0.0;
      TerrainMeta.SplatMap.SetSplatRaw(x, z, interpolatedVector1, interpolatedVector2, opacity);
    }));
  }

  protected override void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
  {
  }

  protected override void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
  {
    bool should0 = this.ShouldBiome(1);
    bool should1 = this.ShouldBiome(2);
    bool should2 = this.ShouldBiome(4);
    bool should3 = this.ShouldBiome(8);
    if (!should0 && !should1 && (!should2 && !should3))
      return;
    Vector3 position = ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.get_zero());
    TextureData heightdata = new TextureData(this.heightmap);
    TextureData biomedata = new TextureData(this.biomemap);
    TerrainMeta.BiomeMap.ForEachParallel(((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) -this.extents.x, 0.0f, (float) -this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) this.extents.x, 0.0f, (float) -this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) -this.extents.x, 0.0f, (float) this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) this.extents.x, 0.0f, (float) this.extents.z))), (Action<int, int>) ((x, z) =>
    {
      float normZ = TerrainMeta.BiomeMap.Coordinate(z);
      float normX = TerrainMeta.BiomeMap.Coordinate(x);
      Vector3 vector3_1;
      ((Vector3) ref vector3_1).\u002Ector(TerrainMeta.DenormalizeX(normX), 0.0f, TerrainMeta.DenormalizeZ(normZ));
      Vector3 vector3_2 = Vector3.op_Subtraction(((Matrix4x4) ref worldToLocal).MultiplyPoint3x4(vector3_1), this.offset);
      float opacity = Mathf.InverseLerp((float) position.y, (float) position.y + this.Fade, (float) (position.y + this.offset.y + (double) heightdata.GetInterpolatedHalf((float) ((vector3_2.x + this.extents.x) / this.size.x), (float) ((vector3_2.z + this.extents.z) / this.size.z)) * this.size.y));
      if ((double) opacity == 0.0)
        return;
      Vector4 interpolatedVector = biomedata.GetInterpolatedVector((float) ((vector3_2.x + this.extents.x) / this.size.x), (float) ((vector3_2.z + this.extents.z) / this.size.z));
      if (!should0)
        interpolatedVector.x = (__Null) 0.0;
      if (!should1)
        interpolatedVector.y = (__Null) 0.0;
      if (!should2)
        interpolatedVector.z = (__Null) 0.0;
      if (!should3)
        interpolatedVector.w = (__Null) 0.0;
      TerrainMeta.BiomeMap.SetBiomeRaw(x, z, interpolatedVector, opacity);
    }));
  }

  protected override void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
  {
    TextureData topologydata = new TextureData(this.topologymap);
    TerrainMeta.TopologyMap.ForEachParallel(((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) -this.extents.x, 0.0f, (float) -this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) this.extents.x, 0.0f, (float) -this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) -this.extents.x, 0.0f, (float) this.extents.z))), ((Matrix4x4) ref localToWorld).MultiplyPoint3x4(Vector3.op_Addition(this.offset, new Vector3((float) this.extents.x, 0.0f, (float) this.extents.z))), (Action<int, int>) ((x, z) =>
    {
      GenerateCliffTopology.Process(x, z);
      float normZ = TerrainMeta.TopologyMap.Coordinate(z);
      float normX = TerrainMeta.TopologyMap.Coordinate(x);
      Vector3 vector3_1;
      ((Vector3) ref vector3_1).\u002Ector(TerrainMeta.DenormalizeX(normX), 0.0f, TerrainMeta.DenormalizeZ(normZ));
      Vector3 vector3_2 = Vector3.op_Subtraction(((Matrix4x4) ref worldToLocal).MultiplyPoint3x4(vector3_1), this.offset);
      int interpolatedInt = topologydata.GetInterpolatedInt((float) ((vector3_2.x + this.extents.x) / this.size.x), (float) ((vector3_2.z + this.extents.z) / this.size.z));
      if (!this.ShouldTopology(interpolatedInt))
        return;
      TerrainMeta.TopologyMap.AddTopology(x, z, interpolatedInt & this.TopologyMask);
    }));
  }

  protected override void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
  {
  }
}
