// Decompiled with JetBrains decompiler
// Type: AtmosphereVolumeRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (Camera))]
public class AtmosphereVolumeRenderer : MonoBehaviour
{
  public FogMode Mode;
  public bool DistanceFog;
  public bool HeightFog;
  public AtmosphereVolume Volume;

  public AtmosphereVolumeRenderer()
  {
    base.\u002Ector();
  }
}
