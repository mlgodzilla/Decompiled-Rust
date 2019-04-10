// Decompiled with JetBrains decompiler
// Type: TerrainHeightMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class TerrainHeightMap : TerrainMap<short>
{
  public Texture2D HeightTexture;
  public Texture2D NormalTexture;
  private float normY;

  public override void Setup()
  {
    if (Object.op_Inequality((Object) this.HeightTexture, (Object) null))
    {
      if (((Texture) this.HeightTexture).get_width() == ((Texture) this.HeightTexture).get_height())
      {
        this.res = ((Texture) this.HeightTexture).get_width();
        this.src = this.dst = new short[this.res * this.res];
        Color32[] pixels32 = this.HeightTexture.GetPixels32();
        int num1 = 0;
        int index = 0;
        for (; num1 < this.res; ++num1)
        {
          int num2 = 0;
          while (num2 < this.res)
          {
            Color32 color32 = pixels32[index];
            this.dst[num1 * this.res + num2] = BitUtility.DecodeShort(color32);
            ++num2;
            ++index;
          }
        }
      }
      else
        Debug.LogError((object) ("Invalid height texture: " + ((Object) this.HeightTexture).get_name()));
    }
    else
    {
      this.res = this.terrain.get_terrainData().get_heightmapResolution();
      this.src = this.dst = new short[this.res * this.res];
    }
    this.normY = (float) (TerrainMeta.Size.x / TerrainMeta.Size.y) / (float) this.res;
  }

  public void ApplyToTerrain()
  {
    float[,] heights = this.terrain.get_terrainData().GetHeights(0, 0, this.res, this.res);
    Parallel.For(0, this.res, (Action<int>) (z =>
    {
      for (int x = 0; x < this.res; ++x)
        heights[z, x] = this.GetHeight01(x, z);
    }));
    this.terrain.get_terrainData().SetHeights(0, 0, heights);
    TerrainCollider component = (TerrainCollider) ((Component) this.terrain).GetComponent<TerrainCollider>();
    if (!Object.op_Implicit((Object) component))
      return;
    ((Collider) component).set_enabled(false);
    ((Collider) component).set_enabled(true);
  }

  public void GenerateTextures(bool heightTexture = true, bool normalTexture = true)
  {
    if (heightTexture)
    {
      Color32[] heights = new Color32[this.res * this.res];
      Parallel.For(0, this.res, (Action<int>) (z =>
      {
        for (int index = 0; index < this.res; ++index)
          heights[z * this.res + index] = BitUtility.EncodeShort(this.src[z * this.res + index]);
      }));
      this.HeightTexture = new Texture2D(this.res, this.res, (TextureFormat) 4, true, true);
      ((Object) this.HeightTexture).set_name("HeightTexture");
      ((Texture) this.HeightTexture).set_wrapMode((TextureWrapMode) 1);
      this.HeightTexture.SetPixels32(heights);
    }
    if (!normalTexture)
      return;
    int normalres = this.res - 1;
    Color32[] normals = new Color32[normalres * normalres];
    Parallel.For(0, normalres, (Action<int>) (z =>
    {
      float normZ = ((float) z + 0.5f) / (float) normalres;
      for (int index = 0; index < normalres; ++index)
      {
        Vector3 normal = this.GetNormal(((float) index + 0.5f) / (float) normalres, normZ);
        normals[z * normalres + index] = Color32.op_Implicit(BitUtility.EncodeNormal(normal));
      }
    }));
    this.NormalTexture = new Texture2D(normalres, normalres, (TextureFormat) 4, true, true);
    ((Object) this.NormalTexture).set_name("NormalTexture");
    ((Texture) this.NormalTexture).set_wrapMode((TextureWrapMode) 1);
    this.NormalTexture.SetPixels32(normals);
  }

  public void ApplyTextures()
  {
    this.HeightTexture.Apply(true, false);
    this.NormalTexture.Apply(true, false);
    this.NormalTexture.Compress(false);
    this.HeightTexture.Apply(false, true);
    this.NormalTexture.Apply(false, true);
  }

  public float GetHeight(Vector3 worldPos)
  {
    return (float) (TerrainMeta.Position.y + (double) this.GetHeight01(worldPos) * TerrainMeta.Size.y);
  }

  public float GetHeight(float normX, float normZ)
  {
    return (float) (TerrainMeta.Position.y + (double) this.GetHeight01(normX, normZ) * TerrainMeta.Size.y);
  }

  public float GetHeightFast(Vector2 uv)
  {
    int num1 = this.res - 1;
    float num2 = (float) uv.x * (float) num1;
    double num3 = uv.y * (double) num1;
    int num4 = (int) num2;
    int num5 = (int) num3;
    float num6 = num2 - (float) num4;
    float num7 = (float) num3 - (float) num5;
    int num8 = num4 >= 0 ? num4 : 0;
    int num9 = num5 >= 0 ? num5 : 0;
    int num10 = num8 <= num1 ? num8 : num1;
    int num11 = num9 <= num1 ? num9 : num1;
    int num12 = (double) num2 < (double) num1 ? 1 : 0;
    int num13 = num3 < (double) num1 ? this.res : 0;
    int index1 = num11 * this.res + num10;
    int index2 = index1 + num12;
    int index3 = index1 + num13;
    int index4 = index3 + num12;
    float num14 = (float) this.src[index1] * 3.051944E-05f;
    float num15 = (float) this.src[index2] * 3.051944E-05f;
    float num16 = (float) this.src[index3] * 3.051944E-05f;
    double num17 = (double) this.src[index4] * 3.05194407701492E-05;
    float num18 = (num15 - num14) * num6 + num14;
    double num19 = (double) num16;
    return (float) (TerrainMeta.Position.y + (double) (((float) (num17 - num19) * num6 + num16 - num18) * num7 + num18) * TerrainMeta.Size.y);
  }

  public float GetHeight(int x, int z)
  {
    return (float) (TerrainMeta.Position.y + (double) this.GetHeight01(x, z) * TerrainMeta.Size.y);
  }

  public float GetHeight01(Vector3 worldPos)
  {
    return this.GetHeight01(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z));
  }

  public float GetHeight01(float normX, float normZ)
  {
    int num1 = this.res - 1;
    float num2 = normX * (float) num1;
    float num3 = normZ * (float) num1;
    int x1 = Mathf.Clamp((int) num2, 0, num1);
    int z1 = Mathf.Clamp((int) num3, 0, num1);
    int x2 = Mathf.Min(x1 + 1, num1);
    int z2 = Mathf.Min(z1 + 1, num1);
    double height01_1 = (double) this.GetHeight01(x1, z1);
    float height01_2 = this.GetHeight01(x2, z1);
    float height01_3 = this.GetHeight01(x1, z2);
    float height01_4 = this.GetHeight01(x2, z2);
    float num4 = num2 - (float) x1;
    float num5 = num3 - (float) z1;
    double num6 = (double) height01_2;
    double num7 = (double) num4;
    return Mathf.Lerp(Mathf.Lerp((float) height01_1, (float) num6, (float) num7), Mathf.Lerp(height01_3, height01_4, num4), num5);
  }

  public float GetHeight01(int x, int z)
  {
    return BitUtility.Short2Float((int) this.src[z * this.res + x]);
  }

  private float GetSrcHeight01(int x, int z)
  {
    return BitUtility.Short2Float((int) this.src[z * this.res + x]);
  }

  private float GetDstHeight01(int x, int z)
  {
    return BitUtility.Short2Float((int) this.dst[z * this.res + x]);
  }

  public Vector3 GetNormal(Vector3 worldPos)
  {
    return this.GetNormal(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z));
  }

  public Vector3 GetNormal(float normX, float normZ)
  {
    int num1 = this.res - 1;
    float num2 = normX * (float) num1;
    float num3 = normZ * (float) num1;
    int x1 = Mathf.Clamp((int) num2, 0, num1);
    int z1 = Mathf.Clamp((int) num3, 0, num1);
    int x2 = Mathf.Min(x1 + 1, num1);
    int z2 = Mathf.Min(z1 + 1, num1);
    Vector3 normal1 = this.GetNormal(x1, z1);
    Vector3 normal2 = this.GetNormal(x2, z1);
    Vector3 normal3 = this.GetNormal(x1, z2);
    Vector3 normal4 = this.GetNormal(x2, z2);
    float num4 = num2 - (float) x1;
    float num5 = num3 - (float) z1;
    Vector3 vector3_1 = normal2;
    double num6 = (double) num4;
    Vector3 vector3_2 = Vector3.Lerp(Vector3.Lerp(normal1, vector3_1, (float) num6), Vector3.Lerp(normal3, normal4, num4), num5);
    return ((Vector3) ref vector3_2).get_normalized();
  }

  public Vector3 GetNormal(int x, int z)
  {
    int num = this.res - 1;
    int x1 = Mathf.Clamp(x - 1, 0, num);
    int z1 = Mathf.Clamp(z - 1, 0, num);
    int x2 = Mathf.Clamp(x + 1, 0, num);
    int z2 = Mathf.Clamp(z + 1, 0, num);
    Vector3 vector3 = new Vector3((float) -(((double) this.GetHeight01(x2, z1) - (double) this.GetHeight01(x1, z1)) * 0.5), this.normY, -(float) (((double) this.GetHeight01(x1, z2) - (double) this.GetHeight01(x1, z1)) * 0.5));
    return ((Vector3) ref vector3).get_normalized();
  }

  private Vector3 GetNormalSobel(int x, int z)
  {
    int num1 = this.res - 1;
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector((float) TerrainMeta.Size.x / (float) num1, (float) TerrainMeta.Size.y, (float) TerrainMeta.Size.z / (float) num1);
    int x1 = x;
    int z1 = z;
    int x2 = Mathf.Clamp(x - 1, 0, num1);
    int z2 = Mathf.Clamp(z - 1, 0, num1);
    int x3 = Mathf.Clamp(x + 1, 0, num1);
    int z3 = Mathf.Clamp(z + 1, 0, num1);
    float num2 = (this.GetHeight01(x2, z2) * -1f + this.GetHeight01(x2, z1) * -2f + this.GetHeight01(x2, z3) * -1f + this.GetHeight01(x3, z2) * 1f + this.GetHeight01(x3, z1) * 2f + this.GetHeight01(x3, z3) * 1f) * (float) vector3_1.y / (float) vector3_1.x;
    float num3 = (this.GetHeight01(x2, z2) * -1f + this.GetHeight01(x1, z2) * -2f + this.GetHeight01(x3, z2) * -1f + this.GetHeight01(x2, z3) * 1f + this.GetHeight01(x1, z3) * 2f + this.GetHeight01(x3, z3) * 1f) * (float) vector3_1.y / (float) vector3_1.z;
    Vector3 vector3_2;
    ((Vector3) ref vector3_2).\u002Ector(-num2, 8f, -num3);
    return ((Vector3) ref vector3_2).get_normalized();
  }

  public float GetSlope(Vector3 worldPos)
  {
    return Vector3.Angle(Vector3.get_up(), this.GetNormal(worldPos));
  }

  public float GetSlope(float normX, float normZ)
  {
    return Vector3.Angle(Vector3.get_up(), this.GetNormal(normX, normZ));
  }

  public float GetSlope(int x, int z)
  {
    return Vector3.Angle(Vector3.get_up(), this.GetNormal(x, z));
  }

  public float GetSlope01(Vector3 worldPos)
  {
    return this.GetSlope(worldPos) * 0.01111111f;
  }

  public float GetSlope01(float normX, float normZ)
  {
    return this.GetSlope(normX, normZ) * 0.01111111f;
  }

  public float GetSlope01(int x, int z)
  {
    return this.GetSlope(x, z) * 0.01111111f;
  }

  public void SetHeight(Vector3 worldPos, float height)
  {
    this.SetHeight(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), height);
  }

  public void SetHeight(float normX, float normZ, float height)
  {
    this.SetHeight(this.Index(normX), this.Index(normZ), height);
  }

  public void SetHeight(int x, int z, float height)
  {
    this.dst[z * this.res + x] = BitUtility.Float2Short(height);
  }

  public void SetHeight(Vector3 worldPos, float height, float opacity)
  {
    this.SetHeight(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), height, opacity);
  }

  public void SetHeight(float normX, float normZ, float height, float opacity)
  {
    this.SetHeight(this.Index(normX), this.Index(normZ), height, opacity);
  }

  public void SetHeight(int x, int z, float height, float opacity)
  {
    float height1 = Mathf.SmoothStep(this.GetDstHeight01(x, z), height, opacity);
    this.SetHeight(x, z, height1);
  }

  public void AddHeight(Vector3 worldPos, float delta)
  {
    this.AddHeight(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), delta);
  }

  public void AddHeight(float normX, float normZ, float delta)
  {
    this.AddHeight(this.Index(normX), this.Index(normZ), delta);
  }

  public void AddHeight(int x, int z, float delta)
  {
    float height = Mathf.Clamp01(this.GetDstHeight01(x, z) + delta);
    this.SetHeight(x, z, height);
  }

  public void LowerHeight(Vector3 worldPos, float height, float opacity)
  {
    this.LowerHeight(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), height, opacity);
  }

  public void LowerHeight(float normX, float normZ, float height, float opacity)
  {
    this.LowerHeight(this.Index(normX), this.Index(normZ), height, opacity);
  }

  public void LowerHeight(int x, int z, float height, float opacity)
  {
    float height1 = Mathf.Min(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
    this.SetHeight(x, z, height1);
  }

  public void RaiseHeight(Vector3 worldPos, float height, float opacity)
  {
    this.RaiseHeight(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), height, opacity);
  }

  public void RaiseHeight(float normX, float normZ, float height, float opacity)
  {
    this.RaiseHeight(this.Index(normX), this.Index(normZ), height, opacity);
  }

  public void RaiseHeight(int x, int z, float height, float opacity)
  {
    float height1 = Mathf.Max(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
    this.SetHeight(x, z, height1);
  }

  public void SetHeight(Vector3 worldPos, float opacity, float radius, float fade = 0.0f)
  {
    this.SetHeight(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), TerrainMeta.NormalizeY((float) worldPos.y), opacity, radius, fade);
  }

  public void SetHeight(
    float normX,
    float normZ,
    float height,
    float opacity,
    float radius,
    float fade = 0.0f)
  {
    Action<int, int, float> action = (Action<int, int, float>) ((x, z, lerp) =>
    {
      if ((double) lerp <= 0.0)
        return;
      this.SetHeight(x, z, height, lerp * opacity);
    });
    this.ApplyFilter(normX, normZ, radius, fade, action);
  }

  public void LowerHeight(Vector3 worldPos, float opacity, float radius, float fade = 0.0f)
  {
    this.LowerHeight(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), TerrainMeta.NormalizeY((float) worldPos.y), opacity, radius, fade);
  }

  public void LowerHeight(
    float normX,
    float normZ,
    float height,
    float opacity,
    float radius,
    float fade = 0.0f)
  {
    Action<int, int, float> action = (Action<int, int, float>) ((x, z, lerp) =>
    {
      if ((double) lerp <= 0.0)
        return;
      this.LowerHeight(x, z, height, lerp * opacity);
    });
    this.ApplyFilter(normX, normZ, radius, fade, action);
  }

  public void RaiseHeight(Vector3 worldPos, float opacity, float radius, float fade = 0.0f)
  {
    this.RaiseHeight(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), TerrainMeta.NormalizeY((float) worldPos.y), opacity, radius, fade);
  }

  public void RaiseHeight(
    float normX,
    float normZ,
    float height,
    float opacity,
    float radius,
    float fade = 0.0f)
  {
    Action<int, int, float> action = (Action<int, int, float>) ((x, z, lerp) =>
    {
      if ((double) lerp <= 0.0)
        return;
      this.RaiseHeight(x, z, height, lerp * opacity);
    });
    this.ApplyFilter(normX, normZ, radius, fade, action);
  }

  public void AddHeight(Vector3 worldPos, float delta, float radius, float fade = 0.0f)
  {
    this.AddHeight(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), delta, radius, fade);
  }

  public void AddHeight(float normX, float normZ, float delta, float radius, float fade = 0.0f)
  {
    Action<int, int, float> action = (Action<int, int, float>) ((x, z, lerp) =>
    {
      if ((double) lerp <= 0.0)
        return;
      this.AddHeight(x, z, lerp * delta);
    });
    this.ApplyFilter(normX, normZ, radius, fade, action);
  }
}
