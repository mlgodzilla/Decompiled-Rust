// Decompiled with JetBrains decompiler
// Type: MovementSounds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class MovementSounds : MonoBehaviour
{
  public SoundDefinition waterMovementDef;
  public float waterMovementFadeInSpeed;
  public float waterMovementFadeOutSpeed;
  private Sound waterMovement;
  private SoundModulation.Modulator waterGainMod;
  private Vector3 lastPos;
  public bool mute;

  public MovementSounds()
  {
    base.\u002Ector();
  }
}
