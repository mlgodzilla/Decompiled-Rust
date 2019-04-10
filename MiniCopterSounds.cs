// Decompiled with JetBrains decompiler
// Type: MiniCopterSounds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MiniCopterSounds : MonoBehaviour, IClientComponent
{
  public MiniCopter miniCopter;
  public GameObject soundAttachPoint;
  public SoundDefinition engineStartDef;
  public SoundDefinition engineLoopDef;
  public SoundDefinition engineStopDef;
  public SoundDefinition rotorLoopDef;
  public float engineStartFadeOutTime;
  public float engineLoopFadeInTime;
  public float engineLoopFadeOutTime;
  public float engineStopFadeOutTime;
  public float rotorLoopFadeInTime;
  public float rotorLoopFadeOutTime;
  public float enginePitchInterpRate;
  public float rotorPitchInterpRate;
  public float rotorGainInterpRate;
  public float rotorStartStopPitchRateUp;
  public float rotorStartStopPitchRateDown;
  public float rotorStartStopGainRateUp;
  public float rotorStartStopGainRateDown;
  public AnimationCurve engineUpDotPitchCurve;
  public AnimationCurve rotorUpDotPitchCurve;

  public MiniCopterSounds()
  {
    base.\u002Ector();
  }
}
