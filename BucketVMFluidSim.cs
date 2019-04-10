// Decompiled with JetBrains decompiler
// Type: BucketVMFluidSim
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BucketVMFluidSim : MonoBehaviour
{
  public Animator waterbucketAnim;
  public ParticleSystem waterPour;
  public ParticleSystem waterTurbulence;
  public ParticleSystem waterFill;
  public float waterLevel;
  public float targetWaterLevel;
  public AudioSource waterSpill;
  private float PlayerEyePitch;
  private float turb_forward;
  private float turb_side;
  private Vector3 lastPosition;
  protected Vector3 groundSpeedLast;
  private Vector3 lastAngle;
  protected Vector3 vecAngleSpeedLast;
  private Vector3 initialPosition;

  public BucketVMFluidSim()
  {
    base.\u002Ector();
  }
}
