// Decompiled with JetBrains decompiler
// Type: TerrainWaterMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class TerrainWaterMap : TerrainMap<short>
{
  public Texture2D WaterTexture;
  private float normY;

  public override void Setup()
  {
    if (Object.op_Inequality((Object) this.WaterTexture, (Object) null))
    {
      if (((Texture) this.WaterTexture).get_width() == ((Texture) this.WaterTexture).get_height())
      {
        this.res = ((Texture) this.WaterTexture).get_width();
        this.src = this.dst = new short[this.res * this.res];
        Color32[] pixels32 = this.WaterTexture.GetPixels32();
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
        Debug.LogError((object) ("Invalid water texture: " + ((Object) this.WaterTexture).get_name()));
    }
    else
    {
      this.res = this.terrain.get_terrainData().get_heightmapResolution();
      this.src = this.dst = new short[this.res * this.res];
    }
    this.normY = (float) (TerrainMeta.Size.x / TerrainMeta.Size.y) / (float) this.res;
  }

  public void GenerateTextures()
  {
    Color32[] heights = new Color32[this.res * this.res];
    Parallel.For(0, this.res, (Action<int>) (z =>
    {
      for (int index = 0; index < this.res; ++index)
        heights[z * this.res + index] = BitUtility.EncodeShort(this.src[z * this.res + index]);
    }));
    this.WaterTexture = new Texture2D(this.res, this.res, (TextureFormat) 4, true, true);
    ((Object) this.WaterTexture).set_name("WaterTexture");
    ((Texture) this.WaterTexture).set_wrapMode((TextureWrapMode) 1);
    this.WaterTexture.SetPixels32(heights);
  }

  public void ApplyTextures()
  {
    this.WaterTexture.Apply(true, true);
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
    return Mathf.Lerp(Mathf.Lerp(this.GetHeight01(x1, z1), this.GetHeight01(x2, z1), num2 - (float) x1), Mathf.Lerp(this.GetHeight01(x1, z2), this.GetHeight01(x2, z2), num2 - (float) x1), num3 - (float) z1);
  }

  public float GetHeight01(int x, int z)
  {
    return BitUtility.Short2Float((int) this.src[z * this.res + x]);
  }

  public Vector3 GetNormal(Vector3 worldPos)
  {
    return this.GetNormal(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z));
  }

  public Vector3 GetNormal(float normX, float normZ)
  {
    int num1 = this.res - 1;
    double num2 = (double) normX * (double) num1;
    float num3 = normZ * (float) num1;
    int x1 = Mathf.Clamp((int) num2, 0, num1);
    int z1 = Mathf.Clamp((int) num3, 0, num1);
    int x2 = Mathf.Min(x1 + 1, num1);
    int z2 = Mathf.Min(z1 + 1, num1);
    Vector3 vector3 = new Vector3((float) -((double) this.GetHeight01(x2, z1) - (double) this.GetHeight01(x1, z1)), this.normY, -(this.GetHeight01(x1, z2) - this.GetHeight01(x1, z1)));
    return ((Vector3) ref vector3).get_normalized();
  }

  public Vector3 GetNormalFast(Vector2 uv)
  {
    int num1 = this.res - 1;
    int num2 = (int) (uv.x * (double) num1);
    int num3 = (int) (uv.y * (double) num1);
    int num4 = num2 >= 0 ? num2 : 0;
    int num5 = num3 >= 0 ? num3 : 0;
    int num6 = num4 <= num1 ? num4 : num1;
    int num7 = num5 <= num1 ? num5 : num1;
    int num8 = num6 < num1 ? 1 : 0;
    int num9 = num7 < num1 ? this.res : 0;
    int index1 = num7 * this.res + num6;
    int index2 = index1 + num8;
    int index3 = index1 + num9;
    short num10 = this.src[index1];
    int num11 = (int) this.src[index2];
    short num12 = this.src[index3];
    int num13 = (int) num10;
    return new Vector3((float) -((double) (num11 - num13) * 3.05194407701492E-05), this.normY, -((float) ((int) num12 - (int) num10) * 3.051944E-05f));
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

  public float GetDepth(Vector3 worldPos)
  {
    return this.GetHeight(worldPos) - TerrainMeta.HeightMap.GetHeight(worldPos);
  }

  public float GetDepth(float normX, float normZ)
  {
    return this.GetHeight(normX, normZ) - TerrainMeta.HeightMap.GetHeight(normX, normZ);
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
}
