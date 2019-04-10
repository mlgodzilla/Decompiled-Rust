// Decompiled with JetBrains decompiler
// Type: SoundPlayerCull
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SoundPlayerCull : MonoBehaviour, IClientComponent, ILOD
{
  public SoundPlayer soundPlayer;
  public float cullDistance;

  public SoundPlayerCull()
  {
    base.\u002Ector();
  }
}
