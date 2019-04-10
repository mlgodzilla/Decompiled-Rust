// Decompiled with JetBrains decompiler
// Type: CameraSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using UnityEngine;

public class CameraSettings : MonoBehaviour, IClientComponent
{
  private Camera cam;

  private void OnEnable()
  {
    this.cam = (Camera) ((Component) this).GetComponent<Camera>();
  }

  private void Update()
  {
    this.cam.set_farClipPlane(Mathf.Clamp(Graphics.drawdistance, 500f, 2500f));
  }

  public CameraSettings()
  {
    base.\u002Ector();
  }
}
