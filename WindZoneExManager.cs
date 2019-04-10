// Decompiled with JetBrains decompiler
// Type: WindZoneExManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof (WindZone))]
public class WindZoneExManager : MonoBehaviour
{
  public float maxAccumMain;
  public float maxAccumTurbulence;
  public float globalMainScale;
  public float globalTurbulenceScale;
  public Transform testPosition;

  public WindZoneExManager()
  {
    base.\u002Ector();
  }

  private enum TestMode
  {
    Disabled,
    Low,
  }
}
