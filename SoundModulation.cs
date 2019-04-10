// Decompiled with JetBrains decompiler
// Type: SoundModulation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SoundModulation : MonoBehaviour, IClientComponent
{
  private const int parameterCount = 4;

  public SoundModulation()
  {
    base.\u002Ector();
  }

  public enum Parameter
  {
    Gain,
    Pitch,
    Spread,
    MaxDistance,
  }

  [Serializable]
  public class Modulator
  {
    public float value = 1f;
    public SoundModulation.Parameter param;
  }
}
