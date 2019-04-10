// Decompiled with JetBrains decompiler
// Type: TerrainAtlasSet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Terrain Atlas Set")]
public class TerrainAtlasSet : ScriptableObject
{
  public static string[] sourceTypeNames = new string[4]
  {
    "Albedo",
    "Normal",
    "Specular",
    "Height"
  };
  public static string[] sourceTypeNamesExt = new string[4]
  {
    "Albedo (rgb)",
    "Normal (rgb)",
    "Specular (rgba)",
    "Height (gray)"
  };
  public static string[] sourceTypePostfix = new string[4]
  {
    "_albedo",
    "_normal",
    "_specular",
    "_height"
  };
  public const int SplatCount = 8;
  public const int SplatSize = 2048;
  public const int MaxSplatSize = 2047;
  public const int SplatPadding = 256;
  public const int AtlasSize = 8192;
  public const int RegionSize = 2560;
  public const int SplatsPerLine = 3;
  public const int SourceTypeCount = 4;
  public const int AtlasMipCount = 10;
  public string[] splatNames;
  public bool[] albedoHighpass;
  public string[] albedoPaths;
  public Color[] defaultValues;
  public TerrainAtlasSet.SourceMapSet[] sourceMaps;
  public bool highQualityCompression;
  public bool generateTextureAtlases;
  public bool generateTextureArrays;
  public string splatSearchPrefix;
  public string splatSearchFolder;
  public string albedoAtlasSavePath;
  public string normalAtlasSavePath;
  public string albedoArraySavePath;
  public string normalArraySavePath;

  public void CheckReset()
  {
    if (this.splatNames == null)
      this.splatNames = new string[8]
      {
        "Dirt",
        "Snow",
        "Sand",
        "Rock",
        "Grass",
        "Forest",
        "Stones",
        "Gravel"
      };
    else if (this.splatNames.Length != 8)
      Array.Resize<string>(ref this.splatNames, 8);
    if (this.albedoHighpass == null)
      this.albedoHighpass = new bool[8];
    else if (this.albedoHighpass.Length != 8)
      Array.Resize<bool>(ref this.albedoHighpass, 8);
    if (this.albedoPaths == null)
      this.albedoPaths = new string[8];
    else if (this.albedoPaths.Length != 8)
      Array.Resize<string>(ref this.albedoPaths, 8);
    if (this.defaultValues == null)
      this.defaultValues = new Color[4]
      {
        new Color(1f, 1f, 1f, 0.5f),
        new Color(0.5f, 0.5f, 1f, 0.0f),
        new Color(0.5f, 0.5f, 0.5f, 0.5f),
        Color.get_black()
      };
    else if (this.defaultValues.Length != 4)
      Array.Resize<Color>(ref this.defaultValues, 4);
    if (this.sourceMaps == null)
      this.sourceMaps = new TerrainAtlasSet.SourceMapSet[4];
    else if (this.sourceMaps.Length != 4)
      Array.Resize<TerrainAtlasSet.SourceMapSet>(ref this.sourceMaps, 4);
    for (int index = 0; index < 4; ++index)
    {
      this.sourceMaps[index] = this.sourceMaps[index] != null ? this.sourceMaps[index] : new TerrainAtlasSet.SourceMapSet();
      this.sourceMaps[index].CheckReset();
    }
  }

  public TerrainAtlasSet()
  {
    base.\u002Ector();
  }

  public enum SourceType
  {
    ALBEDO,
    NORMAL,
    SPECULAR,
    HEIGHT,
    COUNT,
  }

  [Serializable]
  public class SourceMapSet
  {
    public Texture2D[] maps;

    internal void CheckReset()
    {
      if (this.maps == null)
      {
        this.maps = new Texture2D[8];
      }
      else
      {
        if (this.maps.Length == 8)
          return;
        Array.Resize<Texture2D>(ref this.maps, 8);
      }
    }
  }
}
