// Decompiled with JetBrains decompiler
// Type: HairDye
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class HairDye
{
  private static MaterialPropertyDesc[] transferableProps = new MaterialPropertyDesc[8]
  {
    new MaterialPropertyDesc("_DyeColor", typeof (Color)),
    new MaterialPropertyDesc("_RootColor", typeof (Color)),
    new MaterialPropertyDesc("_TipColor", typeof (Color)),
    new MaterialPropertyDesc("_Brightness", typeof (float)),
    new MaterialPropertyDesc("_DyeRoughness", typeof (float)),
    new MaterialPropertyDesc("_DyeScatter", typeof (float)),
    new MaterialPropertyDesc("_HairSpecular", typeof (float)),
    new MaterialPropertyDesc("_HairRoughness", typeof (float))
  };
  private static int _HairBaseColorUV1 = Shader.PropertyToID(nameof (_HairBaseColorUV1));
  private static int _HairBaseColorUV2 = Shader.PropertyToID(nameof (_HairBaseColorUV2));
  private static int _HairPackedMapUV1 = Shader.PropertyToID(nameof (_HairPackedMapUV1));
  private static int _HairPackedMapUV2 = Shader.PropertyToID(nameof (_HairPackedMapUV2));
  [ColorUsage(false, true)]
  public Color capBaseColor;
  public Material sourceMaterial;
  [InspectorFlags]
  public HairDye.CopyPropertyMask copyProperties;

  public void Apply(HairDyeCollection collection, MaterialPropertyBlock block)
  {
    if (!Object.op_Inequality((Object) this.sourceMaterial, (Object) null))
      return;
    for (int index = 0; index < 8; ++index)
    {
      if ((this.copyProperties & (HairDye.CopyPropertyMask) (1 << index)) != (HairDye.CopyPropertyMask) 0)
      {
        MaterialPropertyDesc transferableProp = HairDye.transferableProps[index];
        if (this.sourceMaterial.HasProperty(transferableProp.nameID))
        {
          if (transferableProp.type == typeof (Color))
            block.SetColor(transferableProp.nameID, this.sourceMaterial.GetColor(transferableProp.nameID));
          else if (transferableProp.type == typeof (float))
            block.SetFloat(transferableProp.nameID, this.sourceMaterial.GetFloat(transferableProp.nameID));
        }
      }
    }
  }

  public void ApplyCap(HairDyeCollection collection, HairType type, MaterialPropertyBlock block)
  {
    if (!collection.applyCap)
      return;
    switch (type)
    {
      case HairType.Head:
      case HairType.Armpit:
      case HairType.Pubic:
        block.SetColor(HairDye._HairBaseColorUV1, ((Color) ref this.capBaseColor).get_gamma());
        block.SetTexture(HairDye._HairPackedMapUV1, Object.op_Inequality((Object) collection.capMask, (Object) null) ? collection.capMask : (Texture) Texture2D.get_blackTexture());
        break;
      case HairType.Facial:
        block.SetColor(HairDye._HairBaseColorUV2, ((Color) ref this.capBaseColor).get_gamma());
        block.SetTexture(HairDye._HairPackedMapUV2, Object.op_Inequality((Object) collection.capMask, (Object) null) ? collection.capMask : (Texture) Texture2D.get_blackTexture());
        break;
    }
  }

  public enum CopyProperty
  {
    DyeColor,
    RootColor,
    TipColor,
    Brightness,
    DyeRoughness,
    DyeScatter,
    Specular,
    Roughness,
    Count,
  }

  [System.Flags]
  public enum CopyPropertyMask
  {
    DyeColor = 1,
    RootColor = 2,
    TipColor = 4,
    Brightness = 8,
    DyeRoughness = 16, // 0x00000010
    DyeScatter = 32, // 0x00000020
    Specular = 64, // 0x00000040
    Roughness = 128, // 0x00000080
  }
}
