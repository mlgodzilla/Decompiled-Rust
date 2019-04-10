// Decompiled with JetBrains decompiler
// Type: BradleyMoveTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BradleyMoveTest : MonoBehaviour
{
  public WheelCollider[] leftWheels;
  public WheelCollider[] rightWheels;
  public float moveForceMax;
  public float brakeForce;
  public float throttle;
  public float turnForce;
  public float sideStiffnessMax;
  public float sideStiffnessMin;
  public Transform centerOfMass;
  public float turning;
  public bool brake;
  public Rigidbody myRigidBody;
  public Vector3 destination;
  public float stoppingDist;
  public GameObject followTest;

  public void Awake()
  {
    this.Initialize();
  }

  public void Initialize()
  {
    this.myRigidBody.set_centerOfMass(this.centerOfMass.get_localPosition());
    this.destination = ((Component) this).get_transform().get_position();
  }

  public void SetDestination(Vector3 dest)
  {
    this.destination = dest;
  }

  public void FixedUpdate()
  {
    Vector3 velocity = this.myRigidBody.get_velocity();
    this.SetDestination(this.followTest.get_transform().get_position());
    float num1 = Vector3.Distance(((Component) this).get_transform().get_position(), this.destination);
    if ((double) num1 > (double) this.stoppingDist)
    {
      Vector3 zero = Vector3.get_zero();
      float num2 = Vector3.Dot(zero, ((Component) this).get_transform().get_right());
      float num3 = Vector3.Dot(zero, Vector3.op_UnaryNegation(((Component) this).get_transform().get_right()));
      float num4 = Vector3.Dot(zero, ((Component) this).get_transform().get_right());
      this.turning = (double) Vector3.Dot(zero, Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward())) <= (double) num4 ? num4 : ((double) num2 < (double) num3 ? -1f : 1f);
      this.throttle = Mathf.InverseLerp(this.stoppingDist, 30f, num1);
    }
    this.throttle = Mathf.Clamp(this.throttle, -1f, 1f);
    float num5 = this.throttle;
    float num6 = this.throttle;
    if ((double) this.turning > 0.0)
    {
      num6 = -this.turning;
      num5 = this.turning;
    }
    else if ((double) this.turning < 0.0)
    {
      num5 = this.turning;
      num6 = this.turning * -1f;
    }
    this.ApplyBrakes(this.brake ? 1f : 0.0f);
    float throttle = this.throttle;
    float newThrottle1 = Mathf.Clamp(num5 + throttle, -1f, 1f);
    float newThrottle2 = Mathf.Clamp(num6 + throttle, -1f, 1f);
    this.AdjustFriction();
    float torqueAmount = Mathf.Lerp(this.moveForceMax, this.turnForce, Mathf.InverseLerp(3f, 1f, ((Vector3) ref velocity).get_magnitude() * Mathf.Abs(Vector3.Dot(((Vector3) ref velocity).get_normalized(), ((Component) this).get_transform().get_forward()))));
    this.SetMotorTorque(newThrottle1, false, torqueAmount);
    this.SetMotorTorque(newThrottle2, true, torqueAmount);
  }

  public void ApplyBrakes(float amount)
  {
    this.ApplyBrakeTorque(amount, true);
    this.ApplyBrakeTorque(amount, false);
  }

  public float GetMotorTorque(bool rightSide)
  {
    float num = 0.0f;
    foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
      num += wheelCollider.get_motorTorque();
    return num / (float) this.rightWheels.Length;
  }

  public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
  {
    newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
    float num = torqueAmount * newThrottle;
    foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
      wheelCollider.set_motorTorque(num);
  }

  public void ApplyBrakeTorque(float amount, bool rightSide)
  {
    foreach (WheelCollider wheelCollider in rightSide ? this.rightWheels : this.leftWheels)
      wheelCollider.set_brakeTorque(this.brakeForce * amount);
  }

  public void AdjustFriction()
  {
  }

  public BradleyMoveTest()
  {
    base.\u002Ector();
  }
}
