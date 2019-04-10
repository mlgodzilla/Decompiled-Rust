// Decompiled with JetBrains decompiler
// Type: MaterialOverlay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class MaterialOverlay : MonoBehaviour
{
  public Material material;

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (!Object.op_Implicit((Object) this.material))
    {
      Graphics.Blit((Texture) source, destination);
    }
    else
    {
      for (int index = 0; index < this.material.get_passCount(); ++index)
        Graphics.Blit((Texture) source, destination, this.material, index);
    }
  }

  public MaterialOverlay()
  {
    base.\u002Ector();
  }
}
