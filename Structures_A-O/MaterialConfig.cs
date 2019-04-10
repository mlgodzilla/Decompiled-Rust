// Decompiled with JetBrains decompiler
// Type: MaterialConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Material Config")]
public class MaterialConfig : ScriptableObject
{
  [Horizontal(4, 0)]
  public MaterialConfig.ShaderParametersFloat[] Floats;
  [Horizontal(4, 0)]
  public MaterialConfig.ShaderParametersColor[] Colors;
  [Horizontal(4, 0)]
  public MaterialConfig.ShaderParametersTexture[] Textures;
  public string[] ScaleUV;
  private MaterialPropertyBlock properties;

  public MaterialPropertyBlock GetMaterialPropertyBlock(
    Material mat,
    Vector3 pos,
    Vector3 scale)
  {
    if (this.properties == null)
      this.properties = new MaterialPropertyBlock();
    this.properties.Clear();
    for (int index = 0; index < this.Floats.Length; ++index)
    {
      MaterialConfig.ShaderParametersFloat shaderParametersFloat = this.Floats[index];
      float src;
      float dst;
      float blendParameters = shaderParametersFloat.FindBlendParameters(pos, out src, out dst);
      this.properties.SetFloat(shaderParametersFloat.Name, Mathf.Lerp(src, dst, blendParameters));
    }
    for (int index = 0; index < this.Colors.Length; ++index)
    {
      MaterialConfig.ShaderParametersColor color = this.Colors[index];
      Color src;
      Color dst;
      float blendParameters = color.FindBlendParameters(pos, out src, out dst);
      this.properties.SetColor(color.Name, Color.Lerp(src, dst, blendParameters));
    }
    for (int index = 0; index < this.Textures.Length; ++index)
    {
      MaterialConfig.ShaderParametersTexture texture = this.Textures[index];
      Texture blendParameters = texture.FindBlendParameters(pos);
      if (Object.op_Implicit((Object) blendParameters))
        this.properties.SetTexture(texture.Name, blendParameters);
    }
    for (int index = 0; index < this.ScaleUV.Length; ++index)
    {
      Vector4 vector = mat.GetVector(this.ScaleUV[index]);
      ((Vector4) ref vector).\u002Ector((float) (vector.x * scale.y), (float) (vector.y * scale.y), (float) vector.z, (float) vector.w);
      this.properties.SetVector(this.ScaleUV[index], vector);
    }
    return this.properties;
  }

  public MaterialConfig()
  {
    base.\u002Ector();
  }

  public class ShaderParameters<T>
  {
    public string Name;
    public T Arid;
    public T Temperate;
    public T Tundra;
    public T Arctic;
    private T[] climates;

    public float FindBlendParameters(Vector3 pos, out T src, out T dst)
    {
      if (Object.op_Equality((Object) TerrainMeta.BiomeMap, (Object) null))
      {
        src = this.Temperate;
        dst = this.Tundra;
        return 0.0f;
      }
      if (this.climates == null || this.climates.Length == 0)
        this.climates = new T[4]
        {
          this.Arid,
          this.Temperate,
          this.Tundra,
          this.Arctic
        };
      int biomeMaxType1 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1);
      int biomeMaxType2 = TerrainMeta.BiomeMap.GetBiomeMaxType(pos, ~biomeMaxType1);
      src = this.climates[TerrainBiome.TypeToIndex(biomeMaxType1)];
      dst = this.climates[TerrainBiome.TypeToIndex(biomeMaxType2)];
      return TerrainMeta.BiomeMap.GetBiome(pos, biomeMaxType2);
    }

    public T FindBlendParameters(Vector3 pos)
    {
      if (Object.op_Equality((Object) TerrainMeta.BiomeMap, (Object) null))
        return this.Temperate;
      if (this.climates == null || this.climates.Length == 0)
        this.climates = new T[4]
        {
          this.Arid,
          this.Temperate,
          this.Tundra,
          this.Arctic
        };
      return this.climates[TerrainBiome.TypeToIndex(TerrainMeta.BiomeMap.GetBiomeMaxType(pos, -1))];
    }
  }

  [Serializable]
  public class ShaderParametersFloat : MaterialConfig.ShaderParameters<float>
  {
  }

  [Serializable]
  public class ShaderParametersColor : MaterialConfig.ShaderParameters<Color>
  {
  }

  [Serializable]
  public class ShaderParametersTexture : MaterialConfig.ShaderParameters<Texture>
  {
  }
}
