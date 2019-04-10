// Decompiled with JetBrains decompiler
// Type: ExplosionsDeactivateRendererByTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ExplosionsDeactivateRendererByTime : MonoBehaviour
{
  public float TimeDelay;
  private Renderer rend;

  private void Awake()
  {
    this.rend = (Renderer) ((Component) this).GetComponent<Renderer>();
  }

  private void DeactivateRenderer()
  {
    this.rend.set_enabled(false);
  }

  private void OnEnable()
  {
    this.rend.set_enabled(true);
    this.Invoke("DeactivateRenderer", this.TimeDelay);
  }

  public ExplosionsDeactivateRendererByTime()
  {
    base.\u002Ector();
  }
}
