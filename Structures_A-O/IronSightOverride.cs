// Decompiled with JetBrains decompiler
// Type: IronSightOverride
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class IronSightOverride : MonoBehaviour
{
  public IronsightAimPoint aimPoint;
  public float fieldOfViewOffset;
  public float zoomFactor;
  [Tooltip("If set to 1, the FOV is set to what this override is set to. If set to 0.5 it's half way between the weapon iconsights default and this scope.")]
  public float fovBias;

  public IronSightOverride()
  {
    base.\u002Ector();
  }
}
