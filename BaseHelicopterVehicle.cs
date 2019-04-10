// Decompiled with JetBrains decompiler
// Type: BaseHelicopterVehicle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseHelicopterVehicle : BaseVehicle
{
  protected BaseHelicopterVehicle.HelicopterInputState currentInputState = new BaseHelicopterVehicle.HelicopterInputState();
  public float liftDotMax = 0.75f;
  public float altForceDotMin = 0.85f;
  public float liftFraction = 0.25f;
  protected float hoverForceScale = 0.99f;
  public float thrustLerpSpeed = 1f;
  [Header("Helicopter")]
  public Rigidbody rigidBody;
  public float engineThrustMax;
  public Vector3 torqueScale;
  public Transform com;
  [Header("Effects")]
  public Transform[] GroundPoints;
  public Transform[] GroundEffects;
  public GameObjectRef explosionEffect;
  public GameObjectRef fireBall;
  public GameObjectRef impactEffectSmall;
  public GameObjectRef impactEffectLarge;
  private float avgTerrainHeight;
  public const BaseEntity.Flags Flag_InternalLights = BaseEntity.Flags.Reserved6;
  protected float lastPlayerInputTime;
  public float currentThrottle;
  public float avgThrust;
  private float nextDamageTime;
  private float nextEffectTime;
  private float pendingImpactDamage;

  public virtual float GetServiceCeiling()
  {
    return 1000f;
  }

  public override Vector3 GetLocalVelocityServer()
  {
    return this.rigidBody.get_velocity();
  }

  public override Quaternion GetAngularVelocityServer()
  {
    return Quaternion.LookRotation(this.rigidBody.get_angularVelocity(), ((Component) this).get_transform().get_up());
  }

  public override float MaxVelocity()
  {
    return 50f;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.rigidBody.set_centerOfMass(this.com.get_localPosition());
  }

  public float MouseToBinary(float amount)
  {
    return Mathf.Clamp(amount, -1f, 1f);
  }

  public virtual void PilotInput(InputState inputState, BasePlayer player)
  {
    this.currentInputState.Reset();
    this.currentInputState.throttle = inputState.IsDown(BUTTON.FORWARD) ? 1f : 0.0f;
    this.currentInputState.throttle -= inputState.IsDown(BUTTON.BACKWARD) || inputState.IsDown(BUTTON.DUCK) ? 1f : 0.0f;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    this.currentInputState.pitch = (float) (^(Vector3&) ref inputState.current.mouseDelta).y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    this.currentInputState.roll = (float) -(^(Vector3&) ref inputState.current.mouseDelta).x;
    this.currentInputState.yaw = inputState.IsDown(BUTTON.RIGHT) ? 1f : 0.0f;
    this.currentInputState.yaw -= inputState.IsDown(BUTTON.LEFT) ? 1f : 0.0f;
    this.currentInputState.pitch = this.MouseToBinary(this.currentInputState.pitch);
    this.currentInputState.roll = this.MouseToBinary(this.currentInputState.roll);
    this.lastPlayerInputTime = Time.get_time();
  }

  public override void PlayerServerInput(InputState inputState, BasePlayer player)
  {
    if (this.GetPlayerSeat(player) != 0)
      return;
    this.PilotInput(inputState, player);
  }

  public virtual void SetDefaultInputState()
  {
    this.currentInputState.Reset();
    if (this.IsMounted())
    {
      float num1 = Vector3.Dot(Vector3.get_up(), ((Component) this).get_transform().get_right());
      float num2 = Vector3.Dot(Vector3.get_up(), ((Component) this).get_transform().get_forward());
      this.currentInputState.roll = (double) num1 < 0.0 ? 1f : 0.0f;
      this.currentInputState.roll -= (double) num1 > 0.0 ? 1f : 0.0f;
      if ((double) num2 < -0.0)
      {
        this.currentInputState.pitch = -1f;
      }
      else
      {
        if ((double) num2 <= 0.0)
          return;
        this.currentInputState.pitch = 1f;
      }
    }
    else
      this.currentInputState.throttle = -1f;
  }

  public virtual bool IsEnginePowered()
  {
    return true;
  }

  public override void VehicleFixedUpdate()
  {
    base.VehicleFixedUpdate();
    if (this.isClient)
      return;
    if ((double) Time.get_time() > (double) this.lastPlayerInputTime + 0.5)
      this.SetDefaultInputState();
    this.MovementUpdate();
    this.SetFlag(BaseEntity.Flags.Reserved6, TOD_Sky.get_Instance().get_IsNight(), false, true);
  }

  public override void LightToggle(BasePlayer player)
  {
    if (this.GetPlayerSeat(player) != 0)
      return;
    this.SetFlag(BaseEntity.Flags.Reserved5, !this.HasFlag(BaseEntity.Flags.Reserved5), false, true);
  }

  public virtual void MovementUpdate()
  {
    BaseHelicopterVehicle.HelicopterInputState currentInputState = this.currentInputState;
    this.currentThrottle = Mathf.Lerp(this.currentThrottle, currentInputState.throttle, 2f * Time.get_fixedDeltaTime());
    this.currentThrottle = Mathf.Clamp(this.currentThrottle, -0.8f, 1f);
    this.rigidBody.AddRelativeTorque(new Vector3(currentInputState.pitch * (float) this.torqueScale.x, currentInputState.yaw * (float) this.torqueScale.y, currentInputState.roll * (float) this.torqueScale.z), (ForceMode) 0);
    this.avgThrust = Mathf.Lerp(this.avgThrust, this.engineThrustMax * this.currentThrottle, Time.get_fixedDeltaTime() * this.thrustLerpSpeed);
    float num1 = Mathf.Clamp01(Vector3.Dot(((Component) this).get_transform().get_up(), Vector3.get_up()));
    float num2 = Mathf.InverseLerp(this.liftDotMax, 1f, num1);
    float serviceCeiling = this.GetServiceCeiling();
    this.avgTerrainHeight = Mathf.Lerp(this.avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(((Component) this).get_transform().get_position()), Time.get_deltaTime());
    float num3 = 1f - Mathf.InverseLerp((float) ((double) this.avgTerrainHeight + (double) serviceCeiling - 20.0), this.avgTerrainHeight + serviceCeiling, (float) ((Component) this).get_transform().get_position().y);
    float num4 = num2 * num3;
    float num5 = 1f - Mathf.InverseLerp(this.altForceDotMin, 1f, num1);
    Vector3 vector3_1 = Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_up(), this.engineThrustMax), this.liftFraction), this.currentThrottle), num4);
    Vector3 vector3_2 = Vector3.op_Subtraction(((Component) this).get_transform().get_up(), Vector3.get_up());
    Vector3 vector3_3 = Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(((Vector3) ref vector3_2).get_normalized(), this.engineThrustMax), this.currentThrottle), num5);
    this.rigidBody.AddForce(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(((Component) this).get_transform().get_up(), this.rigidBody.get_mass() * (float) -Physics.get_gravity().y), num4), this.hoverForceScale), (ForceMode) 0);
    this.rigidBody.AddForce(vector3_1, (ForceMode) 0);
    this.rigidBody.AddForce(vector3_3, (ForceMode) 0);
  }

  public void DelayedImpactDamage()
  {
    this.Hurt(this.pendingImpactDamage * this.MaxHealth(), DamageType.Explosion, (BaseEntity) this, false);
    this.pendingImpactDamage = 0.0f;
  }

  public virtual bool CollisionDamageEnabled()
  {
    return true;
  }

  public void ProcessCollision(Collision collision)
  {
    if (this.isClient || !this.CollisionDamageEnabled() || (double) Time.get_time() < (double) this.nextDamageTime)
      return;
    Vector3 relativeVelocity = collision.get_relativeVelocity();
    float magnitude = ((Vector3) ref relativeVelocity).get_magnitude();
    if (Object.op_Implicit((Object) collision.get_gameObject()) && (1 << ((Component) collision.get_collider()).get_gameObject().get_layer() & 1218519297) <= 0)
      return;
    float num = Mathf.InverseLerp(5f, 30f, magnitude);
    if ((double) num <= 0.0)
      return;
    this.pendingImpactDamage += Mathf.Max(num, 0.15f);
    if ((double) Vector3.Dot(((Component) this).get_transform().get_up(), Vector3.get_up()) < 0.5)
      this.pendingImpactDamage *= 5f;
    if ((double) Time.get_time() > (double) this.nextEffectTime)
    {
      this.nextEffectTime = Time.get_time() + 0.25f;
      if (this.impactEffectSmall.isValid)
      {
        ContactPoint contact = collision.GetContact(0);
        Vector3 point = ((ContactPoint) ref contact).get_point();
        Effect.server.Run(this.impactEffectSmall.resourcePath, Vector3.op_Addition(point, Vector3.op_Multiply(Vector3.op_Subtraction(((Component) this).get_transform().get_position(), point), 0.25f)), ((Component) this).get_transform().get_up(), (Connection) null, false);
      }
    }
    Rigidbody rigidBody = this.rigidBody;
    ContactPoint contact1 = collision.GetContact(0);
    Vector3 vector3 = Vector3.op_Multiply(((ContactPoint) ref contact1).get_normal(), (float) (1.0 + 3.0 * (double) num));
    contact1 = collision.GetContact(0);
    Vector3 point1 = ((ContactPoint) ref contact1).get_point();
    rigidBody.AddForceAtPosition(vector3, point1, (ForceMode) 2);
    this.nextDamageTime = Time.get_time() + 0.333f;
    this.Invoke(new Action(this.DelayedImpactDamage), 0.015f);
  }

  private void OnCollisionEnter(Collision collision)
  {
    this.ProcessCollision(collision);
  }

  public override void OnKilled(HitInfo info)
  {
    if (this.isClient)
    {
      base.OnKilled(info);
    }
    else
    {
      if (this.explosionEffect.isValid)
        Effect.server.Run(this.explosionEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, true);
      Vector3 inheritVelocity = Vector3.op_Multiply(this.rigidBody.get_velocity(), 0.25f);
      List<ServerGib> serverGibList = (List<ServerGib>) null;
      if (this.serverGibs.isValid)
        serverGibList = ServerGib.CreateGibs(this.serverGibs.resourcePath, ((Component) this).get_gameObject(), ((ServerGib) this.serverGibs.Get().GetComponent<ServerGib>())._gibSource, inheritVelocity, 3f);
      if (this.fireBall.isValid)
      {
        for (int index = 0; index < 12; ++index)
        {
          BaseEntity entity = GameManager.server.CreateEntity(this.fireBall.resourcePath, ((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), true);
          if (Object.op_Implicit((Object) entity))
          {
            float num1 = 3f;
            float num2 = 10f;
            Vector3 onUnitSphere = Random.get_onUnitSphere();
            ((Component) entity).get_transform().set_position(Vector3.op_Addition(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 1.5f, 0.0f)), Vector3.op_Multiply(onUnitSphere, Random.Range(-4f, 4f))));
            Collider component = (Collider) ((Component) entity).GetComponent<Collider>();
            entity.Spawn();
            entity.SetVelocity(Vector3.op_Addition(inheritVelocity, Vector3.op_Multiply(onUnitSphere, Random.Range(num1, num2))));
            if (serverGibList != null)
            {
              foreach (ServerGib serverGib in serverGibList)
                Physics.IgnoreCollision(component, (Collider) serverGib.GetCollider(), true);
            }
          }
        }
      }
      base.OnKilled(info);
    }
  }

  public class HelicopterInputState
  {
    public float throttle;
    public float roll;
    public float yaw;
    public float pitch;
    public bool groundControl;

    public void Reset()
    {
      this.throttle = 0.0f;
      this.roll = 0.0f;
      this.yaw = 0.0f;
      this.pitch = 0.0f;
      this.groundControl = false;
    }
  }
}
