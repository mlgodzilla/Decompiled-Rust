// Decompiled with JetBrains decompiler
// Type: MusicClip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class MusicClip : ScriptableObject
{
  public AudioClip audioClip;
  public int lengthInBars;
  public int lengthInBarsWithTail;
  public List<float> fadeInPoints;

  public float GetNextFadeInPoint(float currentClipTimeBars)
  {
    if (this.fadeInPoints.Count == 0)
      return currentClipTimeBars;
    float num1 = -1f;
    float num2 = float.PositiveInfinity;
    for (int index = 0; index < this.fadeInPoints.Count; ++index)
    {
      float fadeInPoint = this.fadeInPoints[index];
      float num3 = fadeInPoint - currentClipTimeBars;
      if ((double) fadeInPoint > 0.00999999977648258 && (double) num3 > 0.0 && (double) num3 < (double) num2)
      {
        num2 = num3;
        num1 = fadeInPoint;
      }
    }
    return num1;
  }

  public MusicClip()
  {
    base.\u002Ector();
  }
}
