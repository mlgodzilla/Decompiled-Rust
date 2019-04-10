// Decompiled with JetBrains decompiler
// Type: FootstepSound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class FootstepSound : MonoBehaviour, IClientComponent
{
  public SoundDefinition lightSound;
  public SoundDefinition medSound;
  public SoundDefinition hardSound;
  private const float panAmount = 0.05f;

  public FootstepSound()
  {
    base.\u002Ector();
  }

  public enum Hardness
  {
    Light = 1,
    Medium = 2,
    Hard = 3,
  }
}
