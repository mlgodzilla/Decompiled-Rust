// Decompiled with JetBrains decompiler
// Type: SubsurfaceProfileTexture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SubsurfaceProfileTexture
{
  private List<SubsurfaceProfileTexture.SubsurfaceProfileEntry> entries = new List<SubsurfaceProfileTexture.SubsurfaceProfileEntry>(16);
  public const int SUBSURFACE_RADIUS_SCALE = 1024;
  public const int SUBSURFACE_KERNEL_SIZE = 3;
  private Texture2D texture;

  public Texture2D Texture
  {
    get
    {
      if (!Object.op_Equality((Object) this.texture, (Object) null))
        return this.texture;
      return this.CreateTexture();
    }
  }

  public SubsurfaceProfileTexture()
  {
    this.AddProfile(SubsurfaceProfileData.Default, (SubsurfaceProfile) null);
  }

  public int FindEntryIndex(SubsurfaceProfile profile)
  {
    for (int index = 0; index < this.entries.Count; ++index)
    {
      if (Object.op_Equality((Object) this.entries[index].profile, (Object) profile))
        return index;
    }
    return -1;
  }

  public int AddProfile(SubsurfaceProfileData data, SubsurfaceProfile profile)
  {
    int index1 = -1;
    for (int index2 = 0; index2 < this.entries.Count; ++index2)
    {
      if (Object.op_Equality((Object) this.entries[index2].profile, (Object) profile))
      {
        index1 = index2;
        this.entries[index1] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, profile);
        break;
      }
    }
    if (index1 < 0)
    {
      index1 = this.entries.Count;
      this.entries.Add(new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, profile));
    }
    this.ReleaseTexture();
    return index1;
  }

  public void UpdateProfile(int id, SubsurfaceProfileData data)
  {
    if (id < 0)
      return;
    this.entries[id] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, this.entries[id].profile);
    this.ReleaseTexture();
  }

  public void RemoveProfile(int id)
  {
    if (id < 0)
      return;
    this.entries[id] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(SubsurfaceProfileData.Invalid, (SubsurfaceProfile) null);
    this.CheckReleaseTexture();
  }

  public static Color ColorClamp(Color color, float min = 0.0f, float max = 1f)
  {
    Color color1;
    color1.r = (__Null) (double) Mathf.Clamp((float) color.r, min, max);
    color1.g = (__Null) (double) Mathf.Clamp((float) color.g, min, max);
    color1.b = (__Null) (double) Mathf.Clamp((float) color.b, min, max);
    color1.a = (__Null) (double) Mathf.Clamp((float) color.a, min, max);
    return color1;
  }

  private Texture2D CreateTexture()
  {
    if (this.entries.Count <= 0)
      return (Texture2D) null;
    int length = 32;
    int num1 = Mathf.Max(this.entries.Count, 64);
    this.ReleaseTexture();
    this.texture = new Texture2D(length, num1, (TextureFormat) 17, false, true);
    ((Object) this.texture).set_name("SubsurfaceProfiles");
    ((UnityEngine.Texture) this.texture).set_wrapMode((TextureWrapMode) 1);
    ((UnityEngine.Texture) this.texture).set_filterMode((FilterMode) 1);
    Color[] pixels = this.texture.GetPixels(0);
    for (int index = 0; index < pixels.Length; ++index)
      pixels[index] = Color.get_clear();
    Color[] target = new Color[length];
    for (int index1 = 0; index1 < this.entries.Count; ++index1)
    {
      SubsurfaceProfileData data = this.entries[index1].data;
      data.SubsurfaceColor = SubsurfaceProfileTexture.ColorClamp(data.SubsurfaceColor, 0.0f, 1f);
      data.FalloffColor = SubsurfaceProfileTexture.ColorClamp(data.FalloffColor, 0.009f, 1f);
      target[0] = data.SubsurfaceColor;
      target[0].a = (__Null) 0.0;
      SeparableSSS.CalculateKernel(target, 1, 13, data.SubsurfaceColor, data.FalloffColor);
      SeparableSSS.CalculateKernel(target, 14, 9, data.SubsurfaceColor, data.FalloffColor);
      SeparableSSS.CalculateKernel(target, 23, 6, data.SubsurfaceColor, data.FalloffColor);
      int num2 = length * (num1 - index1 - 1);
      for (int index2 = 0; index2 < 29; ++index2)
      {
        Color color = Color.op_Multiply(target[index2], new Color(1f, 1f, 1f, 0.3333333f));
        ref __Null local = ref color.a;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local * (data.ScatterRadius / 1024f);
        pixels[num2 + index2] = color;
      }
    }
    this.texture.SetPixels(pixels, 0);
    this.texture.Apply(false, false);
    return this.texture;
  }

  private void CheckReleaseTexture()
  {
    int num = 0;
    for (int index = 0; index < this.entries.Count; ++index)
      num += Object.op_Equality((Object) this.entries[index].profile, (Object) null) ? 1 : 0;
    if (this.entries.Count != num)
      return;
    this.ReleaseTexture();
  }

  private void ReleaseTexture()
  {
    if (!Object.op_Inequality((Object) this.texture, (Object) null))
      return;
    Object.DestroyImmediate((Object) this.texture);
    this.texture = (Texture2D) null;
  }

  private struct SubsurfaceProfileEntry
  {
    public SubsurfaceProfileData data;
    public SubsurfaceProfile profile;

    public SubsurfaceProfileEntry(SubsurfaceProfileData data, SubsurfaceProfile profile)
    {
      this.data = data;
      this.profile = profile;
    }
  }
}
