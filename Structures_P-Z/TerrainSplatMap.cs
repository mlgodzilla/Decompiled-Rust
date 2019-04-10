// Decompiled with JetBrains decompiler
// Type: TerrainSplatMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class TerrainSplatMap : TerrainMap<byte>
{
  public Texture2D SplatTexture0;
  public Texture2D SplatTexture1;
  internal int num;

  public override void Setup()
  {
    this.res = this.terrain.get_terrainData().get_alphamapResolution();
    this.num = this.config.Splats.Length;
    this.src = this.dst = new byte[this.num * this.res * this.res];
    if (Object.op_Inequality((Object) this.SplatTexture0, (Object) null))
    {
      if (((Texture) this.SplatTexture0).get_width() == ((Texture) this.SplatTexture0).get_height() && ((Texture) this.SplatTexture0).get_width() == this.res)
      {
        Color32[] pixels32 = this.SplatTexture0.GetPixels32();
        int num1 = 0;
        int index1 = 0;
        for (; num1 < this.res; ++num1)
        {
          int num2 = 0;
          while (num2 < this.res)
          {
            Color32 color32 = pixels32[index1];
            if (this.num > 0)
            {
              byte[] dst = this.dst;
              int res = this.res;
              int index2 = (0 + num1) * this.res + num2;
              // ISSUE: variable of the null type
              __Null r = color32.r;
              dst[index2] = (byte) r;
            }
            if (this.num > 1)
              this.dst[(this.res + num1) * this.res + num2] = (byte) color32.g;
            if (this.num > 2)
              this.dst[(2 * this.res + num1) * this.res + num2] = (byte) color32.b;
            if (this.num > 3)
              this.dst[(3 * this.res + num1) * this.res + num2] = (byte) color32.a;
            ++num2;
            ++index1;
          }
        }
      }
      else
        Debug.LogError((object) ("Invalid splat texture: " + ((Object) this.SplatTexture0).get_name()), (Object) this.SplatTexture0);
    }
    if (!Object.op_Inequality((Object) this.SplatTexture1, (Object) null))
      return;
    if (((Texture) this.SplatTexture1).get_width() == ((Texture) this.SplatTexture1).get_height() && ((Texture) this.SplatTexture1).get_width() == this.res && this.num > 5)
    {
      Color32[] pixels32 = this.SplatTexture1.GetPixels32();
      int num1 = 0;
      int index = 0;
      for (; num1 < this.res; ++num1)
      {
        int num2 = 0;
        while (num2 < this.res)
        {
          Color32 color32 = pixels32[index];
          if (this.num > 4)
            this.dst[(4 * this.res + num1) * this.res + num2] = (byte) color32.r;
          if (this.num > 5)
            this.dst[(5 * this.res + num1) * this.res + num2] = (byte) color32.g;
          if (this.num > 6)
            this.dst[(6 * this.res + num1) * this.res + num2] = (byte) color32.b;
          if (this.num > 7)
            this.dst[(7 * this.res + num1) * this.res + num2] = (byte) color32.a;
          ++num2;
          ++index;
        }
      }
    }
    else
      Debug.LogError((object) ("Invalid splat texture: " + ((Object) this.SplatTexture1).get_name()), (Object) this.SplatTexture1);
  }

  public void GenerateTextures()
  {
    this.SplatTexture0 = new Texture2D(this.res, this.res, (TextureFormat) 4, true, true);
    ((Object) this.SplatTexture0).set_name("SplatTexture0");
    ((Texture) this.SplatTexture0).set_wrapMode((TextureWrapMode) 1);
    Color32[] cols1 = new Color32[this.res * this.res];
    Parallel.For(0, this.res, (Action<int>) (z =>
    {
      for (int index1 = 0; index1 < this.res; ++index1)
      {
        int num1;
        if (this.num <= 0)
        {
          num1 = 0;
        }
        else
        {
          byte[] src = this.src;
          int res = this.res;
          int index2 = (0 + z) * this.res + index1;
          num1 = (int) src[index2];
        }
        byte num2 = (byte) num1;
        byte num3 = this.num > 1 ? this.src[(this.res + z) * this.res + index1] : (byte) 0;
        byte num4 = this.num > 2 ? this.src[(2 * this.res + z) * this.res + index1] : (byte) 0;
        byte num5 = this.num > 3 ? this.src[(3 * this.res + z) * this.res + index1] : (byte) 0;
        cols1[z * this.res + index1] = new Color32(num2, num3, num4, num5);
      }
    }));
    this.SplatTexture0.SetPixels32(cols1);
    this.SplatTexture1 = new Texture2D(this.res, this.res, (TextureFormat) 4, true, true);
    ((Object) this.SplatTexture1).set_name("SplatTexture1");
    ((Texture) this.SplatTexture1).set_wrapMode((TextureWrapMode) 1);
    Color32[] cols2 = new Color32[this.res * this.res];
    Parallel.For(0, this.res, (Action<int>) (z =>
    {
      for (int index = 0; index < this.res; ++index)
      {
        byte num1 = this.num > 4 ? this.src[(4 * this.res + z) * this.res + index] : (byte) 0;
        byte num2 = this.num > 5 ? this.src[(5 * this.res + z) * this.res + index] : (byte) 0;
        byte num3 = this.num > 6 ? this.src[(6 * this.res + z) * this.res + index] : (byte) 0;
        byte num4 = this.num > 7 ? this.src[(7 * this.res + z) * this.res + index] : (byte) 0;
        cols2[z * this.res + index] = new Color32(num1, num2, num3, num4);
      }
    }));
    this.SplatTexture1.SetPixels32(cols2);
  }

  public void ApplyTextures()
  {
    this.SplatTexture0.Apply(true, true);
    this.SplatTexture1.Apply(true, true);
  }

  public float GetSplatMax(Vector3 worldPos, int mask = -1)
  {
    return this.GetSplatMax(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask);
  }

  public float GetSplatMax(float normX, float normZ, int mask = -1)
  {
    return this.GetSplatMax(this.Index(normX), this.Index(normZ), mask);
  }

  public float GetSplatMax(int x, int z, int mask = -1)
  {
    byte num1 = 0;
    for (int index = 0; index < this.num; ++index)
    {
      if ((TerrainSplat.IndexToType(index) & mask) != 0)
      {
        byte num2 = this.src[(index * this.res + z) * this.res + x];
        if ((int) num2 >= (int) num1)
          num1 = num2;
      }
    }
    return BitUtility.Byte2Float((int) num1);
  }

  public int GetSplatMaxIndex(Vector3 worldPos, int mask = -1)
  {
    return this.GetSplatMaxIndex(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask);
  }

  public int GetSplatMaxIndex(float normX, float normZ, int mask = -1)
  {
    return this.GetSplatMaxIndex(this.Index(normX), this.Index(normZ), mask);
  }

  public int GetSplatMaxIndex(int x, int z, int mask = -1)
  {
    byte num1 = 0;
    int num2 = 0;
    for (int index = 0; index < this.num; ++index)
    {
      if ((TerrainSplat.IndexToType(index) & mask) != 0)
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

  public int GetSplatMaxType(Vector3 worldPos, int mask = -1)
  {
    return TerrainSplat.IndexToType(this.GetSplatMaxIndex(worldPos, mask));
  }

  public int GetSplatMaxType(float normX, float normZ, int mask = -1)
  {
    return TerrainSplat.IndexToType(this.GetSplatMaxIndex(normX, normZ, mask));
  }

  public int GetSplatMaxType(int x, int z, int mask = -1)
  {
    return TerrainSplat.IndexToType(this.GetSplatMaxIndex(x, z, mask));
  }

  public float GetSplat(Vector3 worldPos, int mask)
  {
    return this.GetSplat(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), mask);
  }

  public float GetSplat(float normX, float normZ, int mask)
  {
    int num1 = this.res - 1;
    float num2 = normX * (float) num1;
    float num3 = normZ * (float) num1;
    int x1 = Mathf.Clamp((int) num2, 0, num1);
    int z1 = Mathf.Clamp((int) num3, 0, num1);
    int x2 = Mathf.Min(x1 + 1, num1);
    int z2 = Mathf.Min(z1 + 1, num1);
    return Mathf.Lerp(Mathf.Lerp(this.GetSplat(x1, z1, mask), this.GetSplat(x2, z1, mask), num2 - (float) x1), Mathf.Lerp(this.GetSplat(x1, z2, mask), this.GetSplat(x2, z2, mask), num2 - (float) x1), num3 - (float) z1);
  }

  public float GetSplat(int x, int z, int mask)
  {
    if (Mathf.IsPowerOfTwo(mask))
      return BitUtility.Byte2Float((int) this.src[(TerrainSplat.TypeToIndex(mask) * this.res + z) * this.res + x]);
    int num = 0;
    for (int index = 0; index < this.num; ++index)
    {
      if ((TerrainSplat.IndexToType(index) & mask) != 0)
        num += (int) this.src[(index * this.res + z) * this.res + x];
    }
    return Mathf.Clamp01(BitUtility.Byte2Float(num));
  }

  public void SetSplat(Vector3 worldPos, int id)
  {
    this.SetSplat(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), id);
  }

  public void SetSplat(float normX, float normZ, int id)
  {
    this.SetSplat(this.Index(normX), this.Index(normZ), id);
  }

  public void SetSplat(int x, int z, int id)
  {
    int index1 = TerrainSplat.TypeToIndex(id);
    for (int index2 = 0; index2 < this.num; ++index2)
    {
      if (index2 == index1)
        this.dst[(index2 * this.res + z) * this.res + x] = byte.MaxValue;
      else
        this.dst[(index2 * this.res + z) * this.res + x] = (byte) 0;
    }
  }

  public void SetSplat(Vector3 worldPos, int id, float v)
  {
    this.SetSplat(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), id, v);
  }

  public void SetSplat(float normX, float normZ, int id, float v)
  {
    this.SetSplat(this.Index(normX), this.Index(normZ), id, v);
  }

  public void SetSplat(int x, int z, int id, float v)
  {
    this.SetSplat(x, z, id, this.GetSplat(x, z, id), v);
  }

  public void SetSplatRaw(int x, int z, Vector4 v1, Vector4 v2, float opacity)
  {
    if ((double) opacity == 0.0)
      return;
    float num1 = Mathf.Clamp01((float) (v1.x + v1.y + v1.z + v1.w + v2.x + v2.y + v2.z + v2.w));
    if ((double) num1 == 0.0)
      return;
    float num2 = (float) (1.0 - (double) opacity * (double) num1);
    float num3 = opacity;
    if ((double) num2 == 0.0 && (double) num3 == 1.0)
    {
      byte[] dst = this.dst;
      int res = this.res;
      int index = (0 + z) * this.res + x;
      int num4 = (int) BitUtility.Float2Byte((float) v1.x);
      dst[index] = (byte) num4;
      this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte((float) v1.y);
      this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) v1.z);
      this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) v1.w);
      this.dst[(4 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) v2.x);
      this.dst[(5 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) v2.y);
      this.dst[(6 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) v2.z);
      this.dst[(7 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) v2.w);
    }
    else
    {
      byte[] dst = this.dst;
      int res1 = this.res;
      int index1 = (0 + z) * this.res + x;
      byte[] src = this.src;
      int res2 = this.res;
      int index2 = (0 + z) * this.res + x;
      int num4 = (int) BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) src[index2]) * (double) num2 + v1.x * (double) num3));
      dst[index1] = (byte) num4;
      this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) this.src[(this.res + z) * this.res + x]) * (double) num2 + v1.y * (double) num3));
      this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) this.src[(2 * this.res + z) * this.res + x]) * (double) num2 + v1.z * (double) num3));
      this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) this.src[(3 * this.res + z) * this.res + x]) * (double) num2 + v1.w * (double) num3));
      this.dst[(4 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) this.src[(4 * this.res + z) * this.res + x]) * (double) num2 + v2.x * (double) num3));
      this.dst[(5 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) this.src[(5 * this.res + z) * this.res + x]) * (double) num2 + v2.y * (double) num3));
      this.dst[(6 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) this.src[(6 * this.res + z) * this.res + x]) * (double) num2 + v2.z * (double) num3));
      this.dst[(7 * this.res + z) * this.res + x] = BitUtility.Float2Byte((float) ((double) BitUtility.Byte2Float((int) this.src[(7 * this.res + z) * this.res + x]) * (double) num2 + v2.w * (double) num3));
    }
  }

  public void SetSplat(Vector3 worldPos, int id, float opacity, float radius, float fade = 0.0f)
  {
    this.SetSplat(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), id, opacity, radius, fade);
  }

  public void SetSplat(
    float normX,
    float normZ,
    int id,
    float opacity,
    float radius,
    float fade = 0.0f)
  {
    int idx = TerrainSplat.TypeToIndex(id);
    Action<int, int, float> action = (Action<int, int, float>) ((x, z, lerp) =>
    {
      if ((double) lerp <= 0.0)
        return;
      float old_val = (float) this.dst[(idx * this.res + z) * this.res + x];
      float new_val = Mathf.Lerp(old_val, 1f, lerp * opacity);
      this.SetSplat(x, z, id, old_val, new_val);
    });
    this.ApplyFilter(normX, normZ, radius, fade, action);
  }

  public void AddSplat(Vector3 worldPos, int id, float delta, float radius, float fade = 0.0f)
  {
    this.AddSplat(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), id, delta, radius, fade);
  }

  public void AddSplat(float normX, float normZ, int id, float delta, float radius, float fade = 0.0f)
  {
    int idx = TerrainSplat.TypeToIndex(id);
    Action<int, int, float> action = (Action<int, int, float>) ((x, z, lerp) =>
    {
      if ((double) lerp <= 0.0)
        return;
      float old_val = (float) this.dst[(idx * this.res + z) * this.res + x];
      float new_val = Mathf.Clamp01(old_val + lerp * delta);
      this.SetSplat(x, z, id, old_val, new_val);
    });
    this.ApplyFilter(normX, normZ, radius, fade, action);
  }

  private void SetSplat(int x, int z, int id, float old_val, float new_val)
  {
    int index1 = TerrainSplat.TypeToIndex(id);
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
