// Decompiled with JetBrains decompiler
// Type: LightGroupAtTime
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LightGroupAtTime : FacepunchBehaviour
{
  public AnimationCurve IntensityScaleOverTime;
  public Transform SearchRoot;

  public LightGroupAtTime()
  {
    AnimationCurve animationCurve = new AnimationCurve();
    animationCurve.set_keys(new Keyframe[5]
    {
      new Keyframe(0.0f, 1f),
      new Keyframe(8f, 0.0f),
      new Keyframe(12f, 0.0f),
      new Keyframe(19f, 1f),
      new Keyframe(24f, 1f)
    });
    this.IntensityScaleOverTime = animationCurve;
    base.\u002Ector();
  }
}
