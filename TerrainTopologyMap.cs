// Decompiled with JetBrains decompiler
// Type: TerrainTopologyMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class TerrainTopologyMap : TerrainMap<int>
{
  public Texture2D TopologyTexture;

  public override void Setup()
  {
    if (Object.op_Inequality((Object) this.TopologyTexture, (Object) null))
    {
      if (((Texture) this.TopologyTexture).get_width() == ((Texture) this.TopologyTexture).get_height())
      {
        this.res = ((Texture) this.TopologyTexture).get_width();
        this.src = this.dst = new int[this.res * this.res];
        Color32[] pixels32 = this.TopologyTexture.GetPixels32();
        int num1 = 0;
        int index = 0;
        for (; num1 < this.res; ++num1)
        {
          int num2 = 0;
          while (num2 < this.res)
          {
            this.dst[num1 * this.res + num2] = BitUtility.DecodeInt(pixels32[index]);
            ++num2;
            ++index;
          }
        }
      }
      else
        Debug.LogError((object) ("Invalid topology texture: " + ((Object) this.TopologyTexture).get_name()));
    }
    else
    {
      this.res = this.terrain.get_terrainData().get_alphamapResolution();
      this.src = this.dst = new int[this.res * this.res];
    }
  }

  public void GenerateTextures()
  {
    this.TopologyTexture = new Texture2D(this.res, this.res, (TextureFormat) 4, false, true);
    ((Object) this.TopologyTexture).set_name("TopologyTexture");
    ((Texture) this.TopologyTexture).set_wrapMode((TextureWrapMode) 1);
    Color32[] col = new Color32[this.res * this.res];
    Parallel.For(0, this.res, (Action<int>) (z =>
    {
      for (int index = 0; index < this.res; ++index)
        col[z * this.res + index] = BitUtility.EncodeInt(this.src[z * this.res + index]);
    }));
    this.TopologyTexture.SetPixels32(col);
  }

  public void ApplyTextures()
  {
    this.TopologyTexture.Apply(false, true);
  }

  public bool GetTopology(Vector3 worldPos, int mask)
  {
    return this.GetTopology(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask);
  }

  public bool GetTopology(float normX, float normZ, int mask)
  {
    return this.GetTopology(this.Index(normX), this.Index(normZ), mask);
  }

  public bool GetTopology(int x, int z, int mask)
  {
    return (uint) (this.src[z * this.res + x] & mask) > 0U;
  }

  public int GetTopology(Vector3 worldPos)
  {
    return this.GetTopology(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z));
  }

  public int GetTopology(float normX, float normZ)
  {
    return this.GetTopology(this.Index(normX), this.Index(normZ));
  }

  public int GetTopologyFast(Vector2 uv)
  {
    int num1 = this.res - 1;
    int num2 = (int) (uv.x * (double) this.res);
    int num3 = (int) (uv.y * (double) this.res);
    int num4 = num2 >= 0 ? num2 : 0;
    int num5 = num3 >= 0 ? num3 : 0;
    int num6 = num4 <= num1 ? num4 : num1;
    return this.src[(num5 <= num1 ? num5 : num1) * this.res + num6];
  }

  public int GetTopology(int x, int z)
  {
    return this.src[z * this.res + x];
  }

  public void SetTopology(Vector3 worldPos, int mask)
  {
    this.SetTopology(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask);
  }

  public void SetTopology(float normX, float normZ, int mask)
  {
    this.SetTopology(this.Index(normX), this.Index(normZ), mask);
  }

  public void SetTopology(int x, int z, int mask)
  {
    this.dst[z * this.res + x] = mask;
  }

  public void AddTopology(Vector3 worldPos, int mask)
  {
    this.AddTopology(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask);
  }

  public void AddTopology(float normX, float normZ, int mask)
  {
    this.AddTopology(this.Index(normX), this.Index(normZ), mask);
  }

  public void AddTopology(int x, int z, int mask)
  {
    this.dst[z * this.res + x] |= mask;
  }

  public void RemoveTopology(Vector3 worldPos, int mask)
  {
    this.RemoveTopology(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask);
  }

  public void RemoveTopology(float normX, float normZ, int mask)
  {
    this.RemoveTopology(this.Index(normX), this.Index(normZ), mask);
  }

  public void RemoveTopology(int x, int z, int mask)
  {
    this.dst[z * this.res + x] &= ~mask;
  }

  public int GetTopology(Vector3 worldPos, float radius)
  {
    return this.GetTopology(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), radius);
  }

  public int GetTopology(float normX, float normZ, float radius)
  {
    int num1 = 0;
    float num2 = (float) TerrainMeta.OneOverSize.x * radius;
    int num3 = this.Index(normX - num2);
    int num4 = this.Index(normX + num2);
    int num5 = this.Index(normZ - num2);
    int num6 = this.Index(normZ + num2);
    for (int index1 = num5; index1 <= num6; ++index1)
    {
      for (int index2 = num3; index2 <= num4; ++index2)
        num1 |= this.src[index1 * this.res + index2];
    }
    return num1;
  }

  public void SetTopology(Vector3 worldPos, int mask, float radius, float fade = 0.0f)
  {
    this.SetTopology(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask, radius, fade);
  }

  public void SetTopology(float normX, float normZ, int mask, float radius, float fade = 0.0f)
  {
    Action<int, int, float> action = (Action<int, int, float>) ((x, z, lerp) =>
    {
      if ((double) lerp <= 0.5)
        return;
      this.dst[z * this.res + x] = mask;
    });
    this.ApplyFilter(normX, normZ, radius, fade, action);
  }

  public void AddTopology(Vector3 worldPos, int mask, float radius, float fade = 0.0f)
  {
    this.AddTopology(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask, radius, fade);
  }

  public void AddTopology(float normX, float normZ, int mask, float radius, float fade = 0.0f)
  {
    Action<int, int, float> action = (Action<int, int, float>) ((x, z, lerp) =>
    {
      if ((double) lerp <= 0.5)
        return;
      this.dst[z * this.res + x] |= mask;
    });
    this.ApplyFilter(normX, normZ, radius, fade, action);
  }
}
