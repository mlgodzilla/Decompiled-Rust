// Decompiled with JetBrains decompiler
// Type: CameraEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CameraEx : MonoBehaviour
{
  public bool overrideAmbientLight;
  public AmbientMode ambientMode;
  public Color ambientGroundColor;
  public Color ambientEquatorColor;
  public Color ambientLight;
  public float ambientIntensity;
  internal Color old_ambientLight;
  internal Color old_ambientGroundColor;
  internal Color old_ambientEquatorColor;
  internal float old_ambientIntensity;
  internal AmbientMode old_ambientMode;
  public float aspect;

  public CameraEx()
  {
    base.\u002Ector();
  }
}
