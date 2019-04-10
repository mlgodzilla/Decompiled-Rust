// Decompiled with JetBrains decompiler
// Type: AIHelicopterAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AIHelicopterAnimation : MonoBehaviour
{
  public PatrolHelicopterAI _ai;
  public float swayAmount;
  public float lastStrafeScalar;
  public float lastForwardBackScalar;
  public float degreeMax;
  public Vector3 lastPosition;
  public float oldMoveSpeed;
  public float smoothRateOfChange;
  public float flareAmount;

  public void Awake()
  {
    this.lastPosition = ((Component) this).get_transform().get_position();
  }

  public Vector3 GetMoveDirection()
  {
    return this._ai.GetMoveDirection();
  }

  public float GetMoveSpeed()
  {
    return this._ai.GetMoveSpeed();
  }

  public void Update()
  {
    this.lastPosition = ((Component) this).get_transform().get_position();
    Vector3 moveDirection = this.GetMoveDirection();
    float moveSpeed = this.GetMoveSpeed();
    float num1 = (float) (0.25 + (double) Mathf.Clamp01(moveSpeed / this._ai.maxSpeed) * 0.75);
    this.smoothRateOfChange = Mathf.Lerp(this.smoothRateOfChange, moveSpeed - this.oldMoveSpeed, Time.get_deltaTime() * 5f);
    this.oldMoveSpeed = moveSpeed;
    float num2 = Mathf.Lerp(this.lastForwardBackScalar, (1f - Mathf.Clamp01(Vector3.Angle(moveDirection, ((Component) this).get_transform().get_forward()) / this.degreeMax) - (1f - Mathf.Clamp01(Vector3.Angle(moveDirection, Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward())) / this.degreeMax))) * num1, Time.get_deltaTime() * 2f);
    this.lastForwardBackScalar = num2;
    float num3 = Mathf.Lerp(this.lastStrafeScalar, (1f - Mathf.Clamp01(Vector3.Angle(moveDirection, ((Component) this).get_transform().get_right()) / this.degreeMax) - (1f - Mathf.Clamp01(Vector3.Angle(moveDirection, Vector3.op_UnaryNegation(((Component) this).get_transform().get_right())) / this.degreeMax))) * num1, Time.get_deltaTime() * 2f);
    this.lastStrafeScalar = num3;
    Vector3 zero = Vector3.get_zero();
    ref __Null local1 = ref zero.x;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local1 = ^(float&) ref local1 + num2 * this.swayAmount;
    ref __Null local2 = ref zero.z;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local2 = ^(float&) ref local2 - num3 * this.swayAmount;
    Quaternion.get_identity();
    this._ai.helicopterBase.rotorPivot.get_transform().set_localRotation(Quaternion.Euler((float) zero.x, (float) zero.y, (float) zero.z));
  }

  public AIHelicopterAnimation()
  {
    base.\u002Ector();
  }
}
