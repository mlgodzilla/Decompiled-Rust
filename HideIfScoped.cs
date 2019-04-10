// Decompiled with JetBrains decompiler
// Type: HideIfScoped
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class HideIfScoped : MonoBehaviour
{
  public Renderer[] renderers;

  public void SetVisible(bool vis)
  {
    foreach (Renderer renderer in this.renderers)
      renderer.set_enabled(vis);
  }

  public HideIfScoped()
  {
    base.\u002Ector();
  }
}
