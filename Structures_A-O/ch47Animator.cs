// Decompiled with JetBrains decompiler
// Type: ch47Animator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ch47Animator : MonoBehaviour
{
  public Animator animator;
  public bool bottomDoorOpen;
  public bool landingGearDown;
  public bool leftDoorOpen;
  public bool rightDoorOpen;
  public bool rearDoorOpen;
  public bool rearDoorExtensionOpen;
  public Transform rearRotorBlade;
  public Transform frontRotorBlade;
  public float rotorBladeSpeed;
  public float wheelTurnSpeed;
  public float wheelTurnAngle;
  public SkinnedMeshRenderer[] blurredRotorBlades;
  public SkinnedMeshRenderer[] RotorBlades;
  private bool blurredRotorBladesEnabled;
  public float blurSpeedThreshold;

  private void Start()
  {
    this.EnableBlurredRotorBlades(false);
    this.animator.SetBool("rotorblade_stop", false);
  }

  public void SetDropDoorOpen(bool isOpen)
  {
    this.bottomDoorOpen = isOpen;
  }

  private void Update()
  {
    this.animator.SetBool("bottomdoor", this.bottomDoorOpen);
    this.animator.SetBool("landinggear", this.landingGearDown);
    this.animator.SetBool("leftdoor", this.leftDoorOpen);
    this.animator.SetBool("rightdoor", this.rightDoorOpen);
    this.animator.SetBool("reardoor", this.rearDoorOpen);
    this.animator.SetBool("reardoor_extension", this.rearDoorExtensionOpen);
    if ((double) this.rotorBladeSpeed >= (double) this.blurSpeedThreshold && !this.blurredRotorBladesEnabled)
      this.EnableBlurredRotorBlades(true);
    else if ((double) this.rotorBladeSpeed < (double) this.blurSpeedThreshold && this.blurredRotorBladesEnabled)
      this.EnableBlurredRotorBlades(false);
    if ((double) this.rotorBladeSpeed <= 0.0)
      this.animator.SetBool("rotorblade_stop", true);
    else
      this.animator.SetBool("rotorblade_stop", false);
  }

  private void LateUpdate()
  {
    float num = (float) ((double) Time.get_deltaTime() * (double) this.rotorBladeSpeed * 15.0);
    Vector3 localEulerAngles1 = this.frontRotorBlade.get_localEulerAngles();
    this.frontRotorBlade.set_localEulerAngles(new Vector3((float) localEulerAngles1.x, (float) localEulerAngles1.y + num, (float) localEulerAngles1.z));
    Vector3 localEulerAngles2 = this.rearRotorBlade.get_localEulerAngles();
    this.rearRotorBlade.set_localEulerAngles(new Vector3((float) localEulerAngles2.x, (float) localEulerAngles2.y - num, (float) localEulerAngles2.z));
  }

  private void EnableBlurredRotorBlades(bool enabled)
  {
    this.blurredRotorBladesEnabled = enabled;
    foreach (Renderer blurredRotorBlade in this.blurredRotorBlades)
      blurredRotorBlade.set_enabled(enabled);
    foreach (Renderer rotorBlade in this.RotorBlades)
      rotorBlade.set_enabled(!enabled);
  }

  public ch47Animator()
  {
    base.\u002Ector();
  }
}
