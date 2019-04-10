// Decompiled with JetBrains decompiler
// Type: BlendedSoundLoops
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class BlendedSoundLoops : MonoBehaviour, IClientComponent
{
  [Range(0.0f, 1f)]
  public float blend;
  public float blendSmoothing;
  public float loopFadeOutTime;
  public float loopFadeInTime;
  public float gainModSmoothing;
  public float pitchModSmoothing;
  public bool shouldPlay;
  public List<BlendedSoundLoops.Loop> loops;
  public float maxDistance;

  private void OnValidate()
  {
    this.maxDistance = 0.0f;
    foreach (BlendedSoundLoops.Loop loop in this.loops)
    {
      if ((double) loop.soundDef.maxDistance > (double) this.maxDistance)
        this.maxDistance = loop.soundDef.maxDistance;
    }
  }

  public BlendedSoundLoops()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class Loop
  {
    public SoundDefinition soundDef;
    public AnimationCurve gainCurve;
    public AnimationCurve pitchCurve;
    [HideInInspector]
    public Sound sound;
    [HideInInspector]
    public SoundModulation.Modulator gainMod;
    [HideInInspector]
    public SoundModulation.Modulator pitchMod;
  }
}
