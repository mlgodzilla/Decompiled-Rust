// Decompiled with JetBrains decompiler
// Type: TerrainDistanceMap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class TerrainDistanceMap : TerrainMap<byte>
{
  public Texture2D DistanceTexture;

  public override void Setup()
  {
    this.res = this.terrain.get_terrainData().get_heightmapResolution();
    this.src = this.dst = new byte[4 * this.res * this.res];
    if (!Object.op_Inequality((Object) this.DistanceTexture, (Object) null))
      return;
    if (((Texture) this.DistanceTexture).get_width() == ((Texture) this.DistanceTexture).get_height() && ((Texture) this.DistanceTexture).get_width() == this.res)
    {
      Color32[] pixels32 = this.DistanceTexture.GetPixels32();
      int z = 0;
      int index = 0;
      for (; z < this.res; ++z)
      {
        int x = 0;
        while (x < this.res)
        {
          this.SetDistance(x, z, BitUtility.DecodeVector2i(pixels32[index]));
          ++x;
          ++index;
        }
      }
    }
    else
      Debug.LogError((object) ("Invalid distance texture: " + ((Object) this.DistanceTexture).get_name()), (Object) this.DistanceTexture);
  }

  public void GenerateTextures()
  {
    this.DistanceTexture = new Texture2D(this.res, this.res, (TextureFormat) 4, true, true);
    ((Object) this.DistanceTexture).set_name("DistanceTexture");
    ((Texture) this.DistanceTexture).set_wrapMode((TextureWrapMode) 1);
    Color32[] cols = new Color32[this.res * this.res];
    Parallel.For(0, this.res, (Action<int>) (z =>
    {
      for (int x = 0; x < this.res; ++x)
        cols[z * this.res + x] = BitUtility.EncodeVector2i(this.GetDistance(x, z));
    }));
    this.DistanceTexture.SetPixels32(cols);
  }

  public void ApplyTextures()
  {
    this.DistanceTexture.Apply(true, true);
  }

  public Vector2i GetDistance(Vector3 worldPos)
  {
    return this.GetDistance(TerrainMeta.NormalizeX((float) worldPos.x), TerrainMeta.NormalizeZ((float) worldPos.z));
  }

  public Vector2i GetDistance(float normX, float normZ)
  {
    int num = this.res - 1;
    return this.GetDistance(Mathf.Clamp(Mathf.RoundToInt(normX * (float) num), 0, num), Mathf.Clamp(Mathf.RoundToInt(normZ * (float) num), 0, num));
  }

  public Vector2i GetDistance(int x, int z)
  {
    byte[] src = this.src;
    int res = this.res;
    int index = (0 + z) * this.res + x;
    byte num1 = src[index];
    byte num2 = this.src[(this.res + z) * this.res + x];
    byte num3 = this.src[(2 * this.res + z) * this.res + x];
    byte num4 = this.src[(3 * this.res + z) * this.res + x];
    if (num1 == byte.MaxValue && num2 == byte.MaxValue && (num3 == byte.MaxValue && num4 == byte.MaxValue))
      return new Vector2i(256, 256);
    return new Vector2i((int) num1 - (int) num2, (int) num3 - (int) num4);
  }

  public void SetDistance(int x, int z, Vector2i v)
  {
    byte[] dst = this.dst;
    int res = this.res;
    int index = (0 + z) * this.res + x;
    int num = (int) (byte) Mathf.Clamp((int) v.x, 0, (int) byte.MaxValue);
    dst[index] = (byte) num;
    this.dst[(this.res + z) * this.res + x] = (byte) Mathf.Clamp((int) -v.x, 0, (int) byte.MaxValue);
    this.dst[(2 * this.res + z) * this.res + x] = (byte) Mathf.Clamp((int) v.y, 0, (int) byte.MaxValue);
    this.dst[(3 * this.res + z) * this.res + x] = (byte) Mathf.Clamp((int) -v.y, 0, (int) byte.MaxValue);
  }
}
