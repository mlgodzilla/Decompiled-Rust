// Decompiled with JetBrains decompiler
// Type: CargoShipSounds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class CargoShipSounds : MonoBehaviour, IClientComponent
{
  public SoundDefinition waveSoundDef;
  public AnimationCurve waveSoundYGainCurve;
  public AnimationCurve waveSoundEdgeDistanceGainCurve;
  private Sound waveSoundL;
  private Sound waveSoundR;
  private SoundModulation.Modulator waveSoundLGainMod;
  private SoundModulation.Modulator waveSoundRGainMod;
  public SoundDefinition sternWakeSoundDef;
  private Sound sternWakeSound;
  private SoundModulation.Modulator sternWakeSoundGainMod;
  public SoundDefinition engineHumSoundDef;
  private Sound engineHumSound;
  public GameObject engineHumTarget;
  public SoundDefinition hugeRumbleSoundDef;
  public AnimationCurve hugeRumbleYDiffCurve;
  public AnimationCurve hugeRumbleRelativeSpeedCurve;
  private Sound hugeRumbleSound;
  private SoundModulation.Modulator hugeRumbleGainMod;
  private Vector3 lastCameraPos;
  private Vector3 lastRumblePos;
  private Vector3 lastRumbleLocalPos;
  public Collider soundFollowCollider;
  public Collider soundFollowColliderL;
  public Collider soundFollowColliderR;
  public Collider sternSoundFollowCollider;

  public CargoShipSounds()
  {
    base.\u002Ector();
  }
}
