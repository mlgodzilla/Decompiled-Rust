// Decompiled with JetBrains decompiler
// Type: AmbientLightLOD
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AmbientLightLOD : FacepunchBehaviour, ILOD, IClientComponent
{
  public bool isDynamic;
  public float enabledRadius;
  public bool toggleFade;
  public float toggleFadeDuration;
  public bool StickyGizmos;

  protected void OnValidate()
  {
    LightEx.CheckConflict(((Component) this).get_gameObject());
  }

  public AmbientLightLOD()
  {
    base.\u002Ector();
  }
}
