// Decompiled with JetBrains decompiler
// Type: RecoilProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Recoil Properties")]
public class RecoilProperties : ScriptableObject
{
  public float recoilYawMin;
  public float recoilYawMax;
  public float recoilPitchMin;
  public float recoilPitchMax;
  public float timeToTakeMin;
  public float timeToTakeMax;
  public float ADSScale;
  public float movementPenalty;
  public float clampPitch;
  public AnimationCurve pitchCurve;
  public AnimationCurve yawCurve;
  public bool useCurves;
  public int shotsUntilMax;

  public RecoilProperties()
  {
    base.\u002Ector();
  }
}
