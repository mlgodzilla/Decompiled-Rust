// Decompiled with JetBrains decompiler
// Type: TerrainBiomeMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class TerrainBiomeMap : TerrainMap<byte>
{
  public Texture2D BiomeTexture;
  internal int num;

  public override void Setup()
  {
    if (Object.op_Inequality((Object) this.BiomeTexture, (Object) null))
    {
      if (((Texture) this.BiomeTexture).get_width() == ((Texture) this.BiomeTexture).get_height())
      {
        this.res = ((Texture) this.BiomeTexture).get_width();
        this.num = 4;
        this.src = this.dst = new byte[this.num * this.res * this.res];
        Color32[] pixels32 = this.BiomeTexture.GetPixels32();
        int num1 = 0;
        int index1 = 0;
        for (; num1 < this.res; ++num1)
        {
          int num2 = 0;
          while (num2 < this.res)
          {
            Color32 color32 = pixels32[index1];
            byte[] dst = this.dst;
            int res = this.res;
            int index2 = (0 + num1) * this.res + num2;
            // ISSUE: variable of the null type
            __Null r = color32.r;
            dst[index2] = (byte) r;
            this.dst[(this.res + num1) * this.res + num2] = (byte) color32.g;
            this.dst[(2 * this.res + num1) * this.res + num2] = (byte) color32.b;
            this.dst[(3 * this.res + num1) * this.res + num2] = (byte) color32.a;
            ++num2;
            ++index1;
          }
        }
      }
      else
        Debug.LogError((object) ("Invalid biome texture: " + ((Object) this.BiomeTexture).get_name()));
    }
    else
    {
      this.res = this.terrain.get_terrainData().get_alphamapResolution();
      this.num = 4;
      this.src = this.dst = new byte[this.num * this.res * this.res];
    }
  }

  public void GenerateTextures()
  {
    this.BiomeTexture = new Texture2D(this.res, this.res, (TextureFormat) 4, true, true);
    ((Object) this.BiomeTexture).set_name("BiomeTexture");
    ((Texture) this.BiomeTexture).set_wrapMode((TextureWrapMode) 1);
    Color32[] col = new Color32[this.res * this.res];
    Parallel.For(0, this.res, (Action<int>) (z =>
    {
      for (int index1 = 0; index1 < this.res; ++index1)
      {
        byte[] src = this.src;
        int res = this.res;
        int index2 = (0 + z) * this.res + index1;
        byte num1 = src[index2];
        byte num2 = this.src[(this.res + z) * this.res + index1];
        byte num3 = this.src[(2 * this.res + z) * this.res + index1];
        byte num4 = this.src[(3 * this.res + z) * this.res + index1];
        col[z * this.res + index1] = new Color32(num1, num2, num3, num4);
      }
    }));
    this.BiomeTexture.SetPixels32(col);
  }

  public void ApplyTextures()
  {
    this.BiomeTexture.Apply(true, false);
    this.BiomeTexture.Compress(false);
    this.BiomeTexture.Apply(false, true);
  }

  public float GetBiomeMax(Vector3 worldPos, int mask = -1)
  {
    return this.GetBiomeMax(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask);
  }

  public float GetBiomeMax(float normX, float normZ, int mask = -1)
  {
    return this.GetBiomeMax(this.Index(normX), this.Index(normZ), mask);
  }

  public float GetBiomeMax(int x, int z, int mask = -1)
  {
    byte num1 = 0;
    for (int index = 0; index < this.num; ++index)
    {
      if ((TerrainBiome.IndexToType(index) & mask) != 0)
      {
        byte num2 = this.src[(index * this.res + z) * this.res + x];
        if ((int) num2 >= (int) num1)
          num1 = num2;
      }
    }
    return (float) num1;
  }

  public int GetBiomeMaxIndex(Vector3 worldPos, int mask = -1)
  {
    return this.GetBiomeMaxIndex(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask);
  }

  public int GetBiomeMaxIndex(float normX, float normZ, int mask = -1)
  {
    return this.GetBiomeMaxIndex(this.Index(normX), this.Index(normZ), mask);
  }

  public int GetBiomeMaxIndex(int x, int z, int mask = -1)
  {
    byte num1 = 0;
    int num2 = 0;
    for (int index = 0; index < this.num; ++index)
    {
      if ((TerrainBiome.IndexToType(index) & mask) != 0)
      {
        byte num3 = this.src[(index * this.res + z) * this.res + x];
        if ((int) num3 >= (int) num1)
        {
          num1 = num3;
          num2 = index;
        }
      }
    }
    return num2;
  }

  public int GetBiomeMaxType(Vector3 worldPos, int mask = -1)
  {
    return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(worldPos, mask));
  }

  public int GetBiomeMaxType(float normX, float normZ, int mask = -1)
  {
    return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(normX, normZ, mask));
  }

  public int GetBiomeMaxType(int x, int z, int mask = -1)
  {
    return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(x, z, mask));
  }

  public float GetBiome(Vector3 worldPos, int mask)
  {
    return this.GetBiome(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask);
  }

  public float GetBiome(float normX, float normZ, int mask)
  {
    return this.GetBiome(this.Index(normX), this.Index(normZ), mask);
  }

  public float GetBiome(int x, int z, int mask)
  {
    if (Mathf.IsPowerOfTwo(mask))
      return BitUtility.Byte2Float((int) this.src[(TerrainBiome.TypeToIndex(mask) * this.res + z) * this.res + x]);
    int num = 0;
    for (int index = 0; index < this.num; ++index)
    {
      if ((TerrainBiome.IndexToType(index) & mask) != 0)
        num += (int) this.src[(index * this.res + z) * this.res + x];
    }
    return Mathf.Clamp01(BitUtility.Byte2Float(num));
  }

  public void SetBiome(Vector3 worldPos, int id)
  {
    this.SetBiome(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), id);
  }

  public void SetBiome(float normX, float normZ, int id)
  {
    this.SetBiome(this.Index(normX), this.Index(normZ), id);
  }

  public void SetBiome(int x, int z, int id)
  {
    int index1 = TerrainBiome.TypeToIndex(id);
    for (int index2 = 0; index2 < this.num; ++index2)
    {
      if (index2 == index1)
        this.dst[(index2 * this.res + z) * this.res + x] = byte.MaxValue;
      else
        this.dst[(index2 * this.res + z) * this.res + x] = (byte) 0;
    }
  }

  public void SetBiome(Vector3 worldPos, int id, float v)
  {
    this.SetBiome(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), id, v);
  }

  public void SetBiome(float normX, float normZ, int id, float v)
  {
    this.SetBiome(this.Index(normX), this.Index(normZ), id, v);
  }

  public void SetBiome(int x, int z, int id, float v)
  {
    this.SetBiome(x, z, id, this.GetBiome(x, z, id), v);
  }

  public void SetBiomeRaw(int x, int z, Vector4 v, float opacity)
  {
    if ((double) opacity == 0.0)
      return;
    float num1 = Mathf.Clamp01((float) (v.x + v.y + v.z + v.w));
    if ((double) num1 == 0.0)
      return;
    float num2 = (float) (1.0 - (double) opacity * (double) num1);
    float num3 = opacity;
    if ((double) num2 == 0.0 && (double) num3 == 1.0)
    {
      byte[] dst = this.dst;
      int res = this.res;
      int index = (0 + z) * this.res + x;
      int num4 = (int) BitUtility.Float2Byte((float) v.x);
      dst[index] = (byte) num4;
      this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte((float) v.y);
      this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) v.z);
      this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) v.w);
    }
    else
    {
      byte[] dst = this.dst;
      int res1 = this.res;
      int index1 = (0 + z) * this.res + x;
      byte[] src = this.src;
      int res2 = this.res;
      int index2 = (0 + z) * this.res + x;
      int num4 = (int) BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) src[index2]) * (double) num2 + v.x * (double) num3));
      dst[index1] = (byte) num4;
      this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) this.src[(this.res + z) * this.res + x]) * (double) num2 + v.y * (double) num3));
      this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) this.src[(2 * this.res + z) * this.res + x]) * (double) num2 + v.z * (double) num3));
      this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) this.src[(3 * this.res + z) * this.res + x]) * (double) num2 + v.w * (double) num3));
    }
  }

  private void SetBiome(int x, int z, int id, float old_val, float new_val)
  {
    int index1 = TerrainBiome.TypeToIndex(id);
    if ((double) old_val >= 1.0)
      return;
    float num = (float) ((1.0 - (double) new_val) / (1.0 - (double) old_val));
    for (int index2 = 0; index2 < this.num; ++index2)
    {
      if (index2 == index1)
        this.dst[(index2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(new_val);
      else
        this.dst[(index2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(num * BitUtility.Byte2Float((int) this.dst[(index2 * this.res + z) * this.res + x]));
    }
  }
}
