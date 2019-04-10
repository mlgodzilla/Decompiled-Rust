// Decompiled with JetBrains decompiler
// Type: UnityStandardAssets.ImageEffects.ScopeEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Other/Scope Overlay")]
  public class ScopeEffect : PostEffectsBase, IImageEffect
  {
    public Material overlayMaterial;

    public virtual bool CheckResources()
    {
      return true;
    }

    public bool IsActive()
    {
      if (((Behaviour) this).get_enabled())
        return base.CheckResources();
      return false;
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      this.overlayMaterial.SetVector("_Screen", Vector4.op_Implicit(new Vector2((float) Screen.get_width(), (float) Screen.get_height())));
      Graphics.Blit((Texture) source, destination, this.overlayMaterial);
    }

    public ScopeEffect()
    {
      base.\u002Ector();
    }
  }
}
