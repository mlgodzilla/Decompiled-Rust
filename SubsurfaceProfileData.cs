// Decompiled with JetBrains decompiler
// Type: SubsurfaceProfileData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public struct SubsurfaceProfileData
{
  [Range(0.1f, 50f)]
  public float ScatterRadius;
  [ColorUsage(false, true, 1f, 1f, 1f, 1f)]
  public Color SubsurfaceColor;
  [ColorUsage(false, true, 1f, 1f, 1f, 1f)]
  public Color FalloffColor;

  public static SubsurfaceProfileData Default
  {
    get
    {
      return new SubsurfaceProfileData()
      {
        ScatterRadius = 1.2f,
        SubsurfaceColor = new Color(0.48f, 0.41f, 0.28f),
        FalloffColor = new Color(1f, 0.37f, 0.3f)
      };
    }
  }

  public static SubsurfaceProfileData Invalid
  {
    get
    {
      return new SubsurfaceProfileData()
      {
        ScatterRadius = 0.0f,
        SubsurfaceColor = Color.get_clear(),
        FalloffColor = Color.get_clear()
      };
    }
  }
}
