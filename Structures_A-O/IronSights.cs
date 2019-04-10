// Decompiled with JetBrains decompiler
// Type: IronSights
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class IronSights : MonoBehaviour
{
  public bool Enabled;
  [Header("View Setup")]
  public IronsightAimPoint aimPoint;
  public float fieldOfViewOffset;
  public float zoomFactor;
  [Header("Animation")]
  public float introSpeed;
  public AnimationCurve introCurve;
  public float outroSpeed;
  public AnimationCurve outroCurve;
  [Header("Sounds")]
  public SoundDefinition upSound;
  public SoundDefinition downSound;
  [Header("Info")]
  public IronSightOverride ironsightsOverride;

  public IronSights()
  {
    base.\u002Ector();
  }
}
