// Decompiled with JetBrains decompiler
// Type: MusicChangeIntensity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeIntensity : MonoBehaviour
{
  public float raiseTo;
  public List<MusicChangeIntensity.DistanceIntensity> distanceIntensities;
  public float tickInterval;

  public MusicChangeIntensity()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class DistanceIntensity
  {
    public float distance = 60f;
    public float raiseTo;
    public bool forceStartMusicInSuppressedMusicZones;
  }
}
