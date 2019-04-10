// Decompiled with JetBrains decompiler
// Type: SubsurfaceProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;

public class SubsurfaceProfile : ScriptableObject
{
  private static SubsurfaceProfileTexture profileTexture = new SubsurfaceProfileTexture();
  public SubsurfaceProfileData Data;
  private int id;

  public static Texture2D Texture
  {
    get
    {
      if (SubsurfaceProfile.profileTexture == null)
        return (Texture2D) null;
      return SubsurfaceProfile.profileTexture.Texture;
    }
  }

  public int Id
  {
    get
    {
      return this.id;
    }
  }

  private void OnEnable()
  {
    this.id = SubsurfaceProfile.profileTexture.AddProfile(this.Data, this);
  }

  private void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    SubsurfaceProfile.profileTexture.RemoveProfile(this.id);
  }

  public void Update()
  {
    SubsurfaceProfile.profileTexture.UpdateProfile(this.id, this.Data);
  }

  public SubsurfaceProfile()
  {
    base.\u002Ector();
  }
}
