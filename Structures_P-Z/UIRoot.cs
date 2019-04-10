// Decompiled with JetBrains decompiler
// Type: UIRoot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public abstract class UIRoot : MonoBehaviour
{
  private GraphicRaycaster[] graphicRaycasters;
  public Canvas overlayCanvas;

  private void ToggleRaycasters(bool state)
  {
    for (int index = 0; index < this.graphicRaycasters.Length; ++index)
    {
      GraphicRaycaster graphicRaycaster = this.graphicRaycasters[index];
      if (((Behaviour) graphicRaycaster).get_enabled() != state)
        ((Behaviour) graphicRaycaster).set_enabled(state);
    }
  }

  protected virtual void Awake()
  {
    Object.DontDestroyOnLoad((Object) ((Component) this).get_gameObject());
  }

  protected virtual void Start()
  {
    this.graphicRaycasters = (GraphicRaycaster[]) ((Component) this).GetComponentsInChildren<GraphicRaycaster>(true);
  }

  protected void Update()
  {
    this.Refresh();
  }

  protected abstract void Refresh();

  protected UIRoot()
  {
    base.\u002Ector();
  }
}
