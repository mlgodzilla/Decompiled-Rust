// Decompiled with JetBrains decompiler
// Type: TerrainConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Rust/Terrain Config")]
public class TerrainConfig : ScriptableObject
{
  public bool CastShadows;
  public LayerMask GroundMask;
  public LayerMask WaterMask;
  public PhysicMaterial GenericMaterial;
  public Material Material;
  public Texture[] AlbedoArrays;
  public Texture[] NormalArrays;
  public float HeightMapErrorMin;
  public float HeightMapErrorMax;
  public float BaseMapDistanceMin;
  public float BaseMapDistanceMax;
  public float ShaderLodMin;
  public float ShaderLodMax;
  public TerrainConfig.SplatType[] Splats;

  public Texture AlbedoArray
  {
    get
    {
      return this.AlbedoArrays[Mathf.Clamp(QualitySettings.get_masterTextureLimit(), 0, 2)];
    }
  }

  public Texture NormalArray
  {
    get
    {
      return this.NormalArrays[Mathf.Clamp(QualitySettings.get_masterTextureLimit(), 0, 2)];
    }
  }

  public PhysicMaterial[] GetPhysicMaterials()
  {
    PhysicMaterial[] physicMaterialArray = new PhysicMaterial[this.Splats.Length];
    for (int index = 0; index < this.Splats.Length; ++index)
      physicMaterialArray[index] = this.Splats[index].Material;
    return physicMaterialArray;
  }

  public Color[] GetAridColors()
  {
    Color[] colorArray = new Color[this.Splats.Length];
    for (int index = 0; index < this.Splats.Length; ++index)
      colorArray[index] = this.Splats[index].AridColor;
    return colorArray;
  }

  public Color[] GetTemperateColors()
  {
    Color[] colorArray = new Color[this.Splats.Length];
    for (int index = 0; index < this.Splats.Length; ++index)
      colorArray[index] = this.Splats[index].TemperateColor;
    return colorArray;
  }

  public Color[] GetTundraColors()
  {
    Color[] colorArray = new Color[this.Splats.Length];
    for (int index = 0; index < this.Splats.Length; ++index)
      colorArray[index] = this.Splats[index].TundraColor;
    return colorArray;
  }

  public Color[] GetArcticColors()
  {
    Color[] colorArray = new Color[this.Splats.Length];
    for (int index = 0; index < this.Splats.Length; ++index)
      colorArray[index] = this.Splats[index].ArcticColor;
    return colorArray;
  }

  public float[] GetSplatTiling()
  {
    float[] numArray = new float[this.Splats.Length];
    for (int index = 0; index < this.Splats.Length; ++index)
      numArray[index] = this.Splats[index].SplatTiling;
    return numArray;
  }

  public float GetMaxSplatTiling()
  {
    float num = float.MinValue;
    for (int index = 0; index < this.Splats.Length; ++index)
    {
      if ((double) this.Splats[index].SplatTiling > (double) num)
        num = this.Splats[index].SplatTiling;
    }
    return num;
  }

  public float GetMinSplatTiling()
  {
    float num = float.MaxValue;
    for (int index = 0; index < this.Splats.Length; ++index)
    {
      if ((double) this.Splats[index].SplatTiling < (double) num)
        num = this.Splats[index].SplatTiling;
    }
    return num;
  }

  public Vector3[] GetPackedUVMIX()
  {
    Vector3[] vector3Array = new Vector3[this.Splats.Length];
    for (int index = 0; index < this.Splats.Length; ++index)
      vector3Array[index] = new Vector3(this.Splats[index].UVMIXMult, this.Splats[index].UVMIXStart, this.Splats[index].UVMIXDist);
    return vector3Array;
  }

  public TerrainConfig()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class SplatType
  {
    public string Name = "";
    [FormerlySerializedAs("WarmColor")]
    public Color AridColor = Color.get_white();
    [FormerlySerializedAs("Color")]
    public Color TemperateColor = Color.get_white();
    [FormerlySerializedAs("ColdColor")]
    public Color TundraColor = Color.get_white();
    [FormerlySerializedAs("ColdColor")]
    public Color ArcticColor = Color.get_white();
    public float SplatTiling = 5f;
    [Range(0.0f, 1f)]
    public float UVMIXMult = 0.15f;
    public float UVMIXDist = 100f;
    public PhysicMaterial Material;
    public float UVMIXStart;
  }
}
