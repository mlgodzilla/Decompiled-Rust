// Decompiled with JetBrains decompiler
// Type: SunSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using UnityEngine;

public class SunSettings : MonoBehaviour, IClientComponent
{
  private Light light;

  private void OnEnable()
  {
    this.light = (Light) ((Component) this).GetComponent<Light>();
  }

  private void Update()
  {
    LightShadows lightShadows = (LightShadows) Mathf.Clamp(Graphics.shadowmode, 1, 2);
    if (this.light.get_shadows() == lightShadows)
      return;
    this.light.set_shadows(lightShadows);
  }

  public SunSettings()
  {
    base.\u002Ector();
  }
}
