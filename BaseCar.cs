// Decompiled with JetBrains decompiler
// Type: BaseCar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BaseCar : BaseWheeledVehicle
{
  public float motorForceConstant = 150f;
  public float brakeForceConstant = 500f;
  public float GasLerpTime = 20f;
  public float SteeringLerpTime = 20f;
  private bool lightsOn = true;
  public float brakePedal;
  public float gasPedal;
  public float steering;
  public Transform centerOfMass;
  public Transform steeringWheel;
  public Transform driverEye;
  public Rigidbody myRigidBody;
  private static bool chairtest;
  public GameObjectRef chairRef;
  public Transform chairAnchorTest;
  private float throttle;
  private float brake;

  public override float MaxVelocity()
  {
    return 50f;
  }

  public override Vector3 EyePositionForPlayer(BasePlayer player)
  {
    if (Object.op_Equality((Object) player.GetMounted(), (Object) this))
      return ((Component) this.driverEye).get_transform().get_position();
    return Vector3.get_zero();
  }

  public override float GetComfort()
  {
    return 0.0f;
  }

  public override void ServerInit()
  {
    if (this.isClient)
      return;
    base.ServerInit();
    this.myRigidBody = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    this.myRigidBody.set_centerOfMass(this.centerOfMass.get_localPosition());
    this.myRigidBody.set_isKinematic(false);
    if (!BaseCar.chairtest)
      return;
    this.SpawnChairTest();
  }

  public void SpawnChairTest()
  {
    BaseEntity entity = GameManager.server.CreateEntity(this.chairRef.resourcePath, ((Component) this.chairAnchorTest).get_transform().get_localPosition(), (Quaternion) null, true);
    entity.Spawn();
    DestroyOnGroundMissing component1 = (DestroyOnGroundMissing) ((Component) entity).GetComponent<DestroyOnGroundMissing>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      ((Behaviour) component1).set_enabled(false);
    MeshCollider component2 = (MeshCollider) ((Component) entity).GetComponent<MeshCollider>();
    if (Object.op_Implicit((Object) component2))
      component2.set_convex(true);
    entity.SetParent((BaseEntity) this, false, false);
  }

  public new void FixedUpdate()
  {
    base.FixedUpdate();
    if (this.isClient)
      return;
    if (!this.HasDriver())
      this.NoDriverInput();
    this.ConvertInputToThrottle();
    this.DoSteering();
    this.ApplyForceAtWheels();
    this.SetFlag(BaseEntity.Flags.Reserved1, this.IsMounted(), false, true);
    this.SetFlag(BaseEntity.Flags.Reserved2, this.IsMounted() && this.lightsOn, false, true);
  }

  private void DoSteering()
  {
    foreach (BaseWheeledVehicle.VehicleWheel wheel in this.wheels)
    {
      if (wheel.steerWheel)
        wheel.wheelCollider.set_steerAngle(this.steering);
    }
    this.SetFlag(BaseEntity.Flags.Reserved4, (double) this.steering < -2.0, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved5, (double) this.steering > 2.0, false, true);
  }

  public void ConvertInputToThrottle()
  {
  }

  private void ApplyForceAtWheels()
  {
    if (Object.op_Equality((Object) this.myRigidBody, (Object) null))
      return;
    Vector3 velocity = this.myRigidBody.get_velocity();
    float num1 = ((Vector3) ref velocity).get_magnitude() * Vector3.Dot(((Vector3) ref velocity).get_normalized(), ((Component) this).get_transform().get_forward());
    float num2 = this.brakePedal;
    float gasPedal = this.gasPedal;
    if ((double) num1 > 0.0 && (double) gasPedal < 0.0)
      num2 = 100f;
    else if ((double) num1 < 0.0 && (double) gasPedal > 0.0)
      num2 = 100f;
    foreach (BaseWheeledVehicle.VehicleWheel wheel in this.wheels)
    {
      if (wheel.wheelCollider.get_isGrounded())
      {
        if (wheel.powerWheel)
          wheel.wheelCollider.set_motorTorque(gasPedal * this.motorForceConstant);
        if (wheel.brakeWheel)
          wheel.wheelCollider.set_brakeTorque(num2 * this.brakeForceConstant);
      }
    }
    this.SetFlag(BaseEntity.Flags.Reserved3, (double) num2 >= 100.0 && this.IsMounted(), false, true);
  }

  public void NoDriverInput()
  {
    if (BaseCar.chairtest)
    {
      this.gasPedal = Mathf.Sin(Time.get_time()) * 50f;
    }
    else
    {
      this.gasPedal = 0.0f;
      this.brakePedal = Mathf.Lerp(this.brakePedal, 100f, (float) ((double) Time.get_deltaTime() * (double) this.GasLerpTime / 5.0));
    }
  }

  public override void PlayerServerInput(InputState inputState, BasePlayer player)
  {
    if (this.GetPlayerSeat(player) != 0)
      return;
    this.DriverInput(inputState, player);
  }

  public void DriverInput(InputState inputState, BasePlayer player)
  {
    if (inputState.IsDown(BUTTON.FORWARD))
    {
      this.gasPedal = 100f;
      this.brakePedal = 0.0f;
    }
    else if (inputState.IsDown(BUTTON.BACKWARD))
    {
      this.gasPedal = -30f;
      this.brakePedal = 0.0f;
    }
    else
    {
      this.gasPedal = 0.0f;
      this.brakePedal = 30f;
    }
    if (inputState.IsDown(BUTTON.LEFT))
      this.steering = -60f;
    else if (inputState.IsDown(BUTTON.RIGHT))
      this.steering = 60f;
    else
      this.steering = 0.0f;
  }

  public override void LightToggle(BasePlayer player)
  {
    if (this.GetPlayerSeat(player) != 0)
      return;
    this.lightsOn = !this.lightsOn;
  }
}
