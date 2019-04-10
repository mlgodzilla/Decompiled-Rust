// Decompiled with JetBrains decompiler
// Type: sedanAnimation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class sedanAnimation : MonoBehaviour
{
  public Transform[] frontAxles;
  public Transform FL_shock;
  public Transform FL_wheel;
  public Transform FR_shock;
  public Transform FR_wheel;
  public Transform RL_shock;
  public Transform RL_wheel;
  public Transform RR_shock;
  public Transform RR_wheel;
  public WheelCollider FL_wheelCollider;
  public WheelCollider FR_wheelCollider;
  public WheelCollider RL_wheelCollider;
  public WheelCollider RR_wheelCollider;
  public Transform steeringWheel;
  public float motorForceConstant;
  public float brakeForceConstant;
  public float brakePedal;
  public float gasPedal;
  public float steering;
  private Rigidbody myRigidbody;
  public float GasLerpTime;
  public float SteeringLerpTime;
  private float wheelSpinConstant;
  private float shockRestingPosY;
  private float shockDistance;
  private float traceDistanceNeutralPoint;

  private void Start()
  {
    this.myRigidbody = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
  }

  private void Update()
  {
    this.DoSteering();
    this.ApplyForceAtWheels();
    this.UpdateTireAnimation();
    this.InputPlayer();
  }

  private void InputPlayer()
  {
    if (Input.GetKey((KeyCode) 119))
    {
      this.gasPedal = Mathf.Clamp(this.gasPedal + Time.get_deltaTime() * this.GasLerpTime, -100f, 100f);
      this.brakePedal = Mathf.Lerp(this.brakePedal, 0.0f, Time.get_deltaTime() * this.GasLerpTime);
    }
    else if (Input.GetKey((KeyCode) 115))
    {
      this.gasPedal = Mathf.Clamp(this.gasPedal - Time.get_deltaTime() * this.GasLerpTime, -100f, 100f);
      this.brakePedal = Mathf.Lerp(this.brakePedal, 0.0f, Time.get_deltaTime() * this.GasLerpTime);
    }
    else
    {
      this.gasPedal = Mathf.Lerp(this.gasPedal, 0.0f, Time.get_deltaTime() * this.GasLerpTime);
      this.brakePedal = Mathf.Lerp(this.brakePedal, 100f, (float) ((double) Time.get_deltaTime() * (double) this.GasLerpTime / 5.0));
    }
    if (Input.GetKey((KeyCode) 97))
      this.steering = Mathf.Clamp(this.steering - Time.get_deltaTime() * this.SteeringLerpTime, -60f, 60f);
    else if (Input.GetKey((KeyCode) 100))
      this.steering = Mathf.Clamp(this.steering + Time.get_deltaTime() * this.SteeringLerpTime, -60f, 60f);
    else
      this.steering = Mathf.Lerp(this.steering, 0.0f, Time.get_deltaTime() * this.SteeringLerpTime);
  }

  private void DoSteering()
  {
    this.FL_wheelCollider.set_steerAngle(this.steering);
    this.FR_wheelCollider.set_steerAngle(this.steering);
  }

  private void ApplyForceAtWheels()
  {
    if (this.FL_wheelCollider.get_isGrounded())
    {
      this.FL_wheelCollider.set_motorTorque(this.gasPedal * this.motorForceConstant);
      this.FL_wheelCollider.set_brakeTorque(this.brakePedal * this.brakeForceConstant);
    }
    if (this.FR_wheelCollider.get_isGrounded())
    {
      this.FR_wheelCollider.set_motorTorque(this.gasPedal * this.motorForceConstant);
      this.FR_wheelCollider.set_brakeTorque(this.brakePedal * this.brakeForceConstant);
    }
    if (this.RL_wheelCollider.get_isGrounded())
    {
      this.RL_wheelCollider.set_motorTorque(this.gasPedal * this.motorForceConstant);
      this.RL_wheelCollider.set_brakeTorque(this.brakePedal * this.brakeForceConstant);
    }
    if (!this.RR_wheelCollider.get_isGrounded())
      return;
    this.RR_wheelCollider.set_motorTorque(this.gasPedal * this.motorForceConstant);
    this.RR_wheelCollider.set_brakeTorque(this.brakePedal * this.brakeForceConstant);
  }

  private void UpdateTireAnimation()
  {
    float num = Vector3.Dot(this.myRigidbody.get_velocity(), ((Component) this.myRigidbody).get_transform().get_forward());
    if (this.FL_wheelCollider.get_isGrounded())
    {
      this.FL_shock.set_localPosition(new Vector3((float) this.FL_shock.get_localPosition().x, this.shockRestingPosY + this.GetShockHeightDelta(this.FL_wheelCollider), (float) this.FL_shock.get_localPosition().z));
      this.FL_wheel.set_localEulerAngles(new Vector3((float) this.FL_wheel.get_localEulerAngles().x, (float) this.FL_wheel.get_localEulerAngles().y, (float) (this.FL_wheel.get_localEulerAngles().z - (double) num * (double) Time.get_deltaTime() * (double) this.wheelSpinConstant)));
    }
    else
      this.FL_shock.set_localPosition(Vector3.Lerp(this.FL_shock.get_localPosition(), new Vector3((float) this.FL_shock.get_localPosition().x, this.shockRestingPosY, (float) this.FL_shock.get_localPosition().z), Time.get_deltaTime() * 2f));
    if (this.FR_wheelCollider.get_isGrounded())
    {
      this.FR_shock.set_localPosition(new Vector3((float) this.FR_shock.get_localPosition().x, this.shockRestingPosY + this.GetShockHeightDelta(this.FR_wheelCollider), (float) this.FR_shock.get_localPosition().z));
      this.FR_wheel.set_localEulerAngles(new Vector3((float) this.FR_wheel.get_localEulerAngles().x, (float) this.FR_wheel.get_localEulerAngles().y, (float) (this.FR_wheel.get_localEulerAngles().z - (double) num * (double) Time.get_deltaTime() * (double) this.wheelSpinConstant)));
    }
    else
      this.FR_shock.set_localPosition(Vector3.Lerp(this.FR_shock.get_localPosition(), new Vector3((float) this.FR_shock.get_localPosition().x, this.shockRestingPosY, (float) this.FR_shock.get_localPosition().z), Time.get_deltaTime() * 2f));
    if (this.RL_wheelCollider.get_isGrounded())
    {
      this.RL_shock.set_localPosition(new Vector3((float) this.RL_shock.get_localPosition().x, this.shockRestingPosY + this.GetShockHeightDelta(this.RL_wheelCollider), (float) this.RL_shock.get_localPosition().z));
      this.RL_wheel.set_localEulerAngles(new Vector3((float) this.RL_wheel.get_localEulerAngles().x, (float) this.RL_wheel.get_localEulerAngles().y, (float) (this.RL_wheel.get_localEulerAngles().z - (double) num * (double) Time.get_deltaTime() * (double) this.wheelSpinConstant)));
    }
    else
      this.RL_shock.set_localPosition(Vector3.Lerp(this.RL_shock.get_localPosition(), new Vector3((float) this.RL_shock.get_localPosition().x, this.shockRestingPosY, (float) this.RL_shock.get_localPosition().z), Time.get_deltaTime() * 2f));
    if (this.RR_wheelCollider.get_isGrounded())
    {
      this.RR_shock.set_localPosition(new Vector3((float) this.RR_shock.get_localPosition().x, this.shockRestingPosY + this.GetShockHeightDelta(this.RR_wheelCollider), (float) this.RR_shock.get_localPosition().z));
      this.RR_wheel.set_localEulerAngles(new Vector3((float) this.RR_wheel.get_localEulerAngles().x, (float) this.RR_wheel.get_localEulerAngles().y, (float) (this.RR_wheel.get_localEulerAngles().z - (double) num * (double) Time.get_deltaTime() * (double) this.wheelSpinConstant)));
    }
    else
      this.RR_shock.set_localPosition(Vector3.Lerp(this.RR_shock.get_localPosition(), new Vector3((float) this.RR_shock.get_localPosition().x, this.shockRestingPosY, (float) this.RR_shock.get_localPosition().z), Time.get_deltaTime() * 2f));
    foreach (Transform frontAxle in this.frontAxles)
      frontAxle.set_localEulerAngles(new Vector3(this.steering, (float) frontAxle.get_localEulerAngles().y, (float) frontAxle.get_localEulerAngles().z));
  }

  private float GetShockHeightDelta(WheelCollider wheel)
  {
    int mask = LayerMask.GetMask(new string[3]
    {
      "Terrain",
      "World",
      "Construction"
    });
    RaycastHit raycastHit;
    Physics.Linecast(((Component) wheel).get_transform().get_position(), Vector3.op_Subtraction(((Component) wheel).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 10f)), ref raycastHit, mask);
    return Mathx.RemapValClamped(((RaycastHit) ref raycastHit).get_distance(), this.traceDistanceNeutralPoint - this.shockDistance, this.traceDistanceNeutralPoint + this.shockDistance, this.shockDistance * 0.75f, -0.75f * this.shockDistance);
  }

  public sedanAnimation()
  {
    base.\u002Ector();
  }
}
