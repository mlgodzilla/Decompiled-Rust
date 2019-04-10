// Decompiled with JetBrains decompiler
// Type: LightLOD
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LightLOD : MonoBehaviour, ILOD, IClientComponent
{
  public float DistanceBias;
  public bool ToggleLight;
  public bool ToggleShadows;

  protected void OnValidate()
  {
    LightEx.CheckConflict(((Component) this).get_gameObject());
  }

  public LightLOD()
  {
    base.\u002Ector();
  }
}
