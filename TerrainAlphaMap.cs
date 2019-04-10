// Decompiled with JetBrains decompiler
// Type: TerrainAlphaMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;

public class TerrainAlphaMap : TerrainMap<byte>
{
  [FormerlySerializedAs("ColorTexture")]
  public Texture2D AlphaTexture;

  public override void Setup()
  {
    if (Object.op_Inequality((Object) this.AlphaTexture, (Object) null))
    {
      if (((Texture) this.AlphaTexture).get_width() == ((Texture) this.AlphaTexture).get_height())
      {
        this.res = ((Texture) this.AlphaTexture).get_width();
        this.src = this.dst = new byte[this.res * this.res];
        Color32[] pixels32 = this.AlphaTexture.GetPixels32();
        int num1 = 0;
        int index = 0;
        for (; num1 < this.res; ++num1)
        {
          int num2 = 0;
          while (num2 < this.res)
          {
            this.dst[num1 * this.res + num2] = (byte) pixels32[index].a;
            ++num2;
            ++index;
          }
        }
      }
      else
        Debug.LogError((object) ("Invalid alpha texture: " + ((Object) this.AlphaTexture).get_name()));
    }
    else
    {
      this.res = this.terrain.get_terrainData().get_alphamapResolution();
      this.src = this.dst = new byte[this.res * this.res];
      for (int index1 = 0; index1 < this.res; ++index1)
      {
        for (int index2 = 0; index2 < this.res; ++index2)
          this.dst[index1 * this.res + index2] = byte.MaxValue;
      }
    }
  }

  public void GenerateTextures()
  {
    this.AlphaTexture = new Texture2D(this.res, this.res, (TextureFormat) 1, false, true);
    ((Object) this.AlphaTexture).set_name("AlphaTexture");
    ((Texture) this.AlphaTexture).set_wrapMode((TextureWrapMode) 1);
    Color32[] col = new Color32[this.res * this.res];
    Parallel.For(0, this.res, (Action<int>) (z =>
    {
      for (int index = 0; index < this.res; ++index)
      {
        byte num = this.src[z * this.res + index];
        col[z * this.res + index] = new Color32(num, num, num, num);
      }
    }));
    this.AlphaTexture.SetPixels32(col);
  }

  public void ApplyTextures()
  {
    this.AlphaTexture.Apply(true, false);
    this.AlphaTexture.Compress(false);
    this.AlphaTexture.Apply(false, true);
  }

  public float GetAlpha(Vector3 worldPos)
  {
    return this.GetAlpha(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z));
  }

  public float GetAlpha(float normX, float normZ)
  {
    int num1 = this.res - 1;
    float num2 = normX * (float) num1;
    float num3 = normZ * (float) num1;
    int x1 = Mathf.Clamp((int) num2, 0, num1);
    int z1 = Mathf.Clamp((int) num3, 0, num1);
    int x2 = Mathf.Min(x1 + 1, num1);
    int z2 = Mathf.Min(z1 + 1, num1);
    return Mathf.Lerp(Mathf.Lerp(this.GetAlpha(x1, z1), this.GetAlpha(x2, z1), num2 - (float) x1), Mathf.Lerp(this.GetAlpha(x1, z2), this.GetAlpha(x2, z2), num2 - (float) x1), num3 - (float) z1);
  }

  public float GetAlpha(int x, int z)
  {
    return BitUtility.Byte2Float((int) this.src[z * this.res + x]);
  }

  public void SetAlpha(Vector3 worldPos, float a)
  {
    this.SetAlpha(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), a);
  }

  public void SetAlpha(float normX, float normZ, float a)
  {
    this.SetAlpha(this.Index(normX), this.Index(normZ), a);
  }

  public void SetAlpha(int x, int z, float a)
  {
    this.dst[z * this.res + x] = BitUtility.Float2Byte(a);
  }

  public void SetAlpha(int x, int z, float a, float opacity)
  {
    this.SetAlpha(x, z, Mathf.Lerp(this.GetAlpha(x, z), a, opacity));
  }

  public void SetAlpha(Vector3 worldPos, float a, float opacity, float radius, float fade = 0.0f)
  {
    this.SetAlpha(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z), a, opacity, radius, fade);
  }

  public void SetAlpha(
    float normX,
    float normZ,
    float a,
    float opacity,
    float radius,
    float fade = 0.0f)
  {
    Action<int, int, float> action = (Action<int, int, float>) ((x, z, lerp) =>
    {
      lerp *= opacity;
      if ((double) lerp <= 0.0)
        return;
      this.SetAlpha(x, z, a, lerp);
    });
    this.ApplyFilter(normX, normZ, radius, fade, action);
  }
}
