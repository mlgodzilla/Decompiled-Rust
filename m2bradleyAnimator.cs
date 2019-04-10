// Decompiled with JetBrains decompiler
// Type: m2bradleyAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class m2bradleyAnimator : MonoBehaviour
{
  public Animator m2Animator;
  public Material treadLeftMaterial;
  public Material treadRightMaterial;
  private Rigidbody mainRigidbody;
  [Header("GunBones")]
  public Transform turret;
  public Transform mainCannon;
  public Transform coaxGun;
  public Transform rocketsPitch;
  public Transform spotLightYaw;
  public Transform spotLightPitch;
  public Transform sideMG;
  public Transform[] sideguns;
  [Header("WheelBones")]
  public Transform[] ShocksBones;
  public Transform[] ShockTraceLineBegin;
  public Vector3[] vecShocksOffsetPosition;
  [Header("Targeting")]
  public Transform targetTurret;
  public Transform targetSpotLight;
  public Transform[] targetSideguns;
  private Vector3 vecTurret;
  private Vector3 vecMainCannon;
  private Vector3 vecCoaxGun;
  private Vector3 vecRocketsPitch;
  private Vector3 vecSpotLightBase;
  private Vector3 vecSpotLight;
  private float sideMGPitchValue;
  [Header("MuzzleFlash locations")]
  public GameObject muzzleflashCannon;
  public GameObject muzzleflashCoaxGun;
  public GameObject muzzleflashSideMG;
  public GameObject[] muzzleflashRockets;
  public GameObject spotLightHaloSawnpoint;
  public GameObject[] muzzleflashSideguns;
  [Header("MuzzleFlash Particle Systems")]
  public GameObjectRef machineGunMuzzleFlashFX;
  public GameObjectRef mainCannonFireFX;
  public GameObjectRef rocketLaunchFX;
  [Header("Misc")]
  public bool rocketsOpen;
  public Vector3[] vecSideGunRotation;
  public float treadConstant;
  public float wheelSpinConstant;
  [Header("Gun Movement speeds")]
  public float sidegunsTurnSpeed;
  public float turretTurnSpeed;
  public float cannonPitchSpeed;
  public float rocketPitchSpeed;
  public float spotLightTurnSpeed;
  public float machineGunSpeed;
  private float wheelAngle;

  private void Start()
  {
    this.mainRigidbody = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    for (int index = 0; index < this.ShocksBones.Length; ++index)
      this.vecShocksOffsetPosition[index] = this.ShocksBones[index].get_localPosition();
  }

  private void Update()
  {
    this.TrackTurret();
    this.TrackSpotLight();
    this.TrackSideGuns();
    this.AnimateWheelsTreads();
    this.AdjustShocksHeight();
    this.m2Animator.SetBool("rocketpods", this.rocketsOpen);
  }

  private void AnimateWheelsTreads()
  {
    float num1 = 0.0f;
    if (Object.op_Inequality((Object) this.mainRigidbody, (Object) null))
      num1 = Vector3.Dot(this.mainRigidbody.get_velocity(), ((Component) this).get_transform().get_forward());
    float num2 = (float) ((double) Time.get_time() * -1.0 * (double) num1 * (double) this.treadConstant % 1.0);
    this.treadLeftMaterial.SetTextureOffset("_MainTex", new Vector2(num2, 0.0f));
    this.treadLeftMaterial.SetTextureOffset("_BumpMap", new Vector2(num2, 0.0f));
    this.treadLeftMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(num2, 0.0f));
    this.treadRightMaterial.SetTextureOffset("_MainTex", new Vector2(num2, 0.0f));
    this.treadRightMaterial.SetTextureOffset("_BumpMap", new Vector2(num2, 0.0f));
    this.treadRightMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(num2, 0.0f));
    if ((double) num1 >= 0.0)
    {
      this.wheelAngle = (float) (((double) this.wheelAngle + (double) Time.get_deltaTime() * (double) num1 * (double) this.wheelSpinConstant) % 360.0);
    }
    else
    {
      this.wheelAngle += Time.get_deltaTime() * num1 * this.wheelSpinConstant;
      if ((double) this.wheelAngle <= 0.0)
        this.wheelAngle = 360f;
    }
    this.m2Animator.SetFloat("wheel_spin", this.wheelAngle);
    this.m2Animator.SetFloat("speed", num1);
  }

  private void AdjustShocksHeight()
  {
    Ray ray = (Ray) null;
    int mask = LayerMask.GetMask(new string[3]
    {
      "Terrain",
      "World",
      "Construction"
    });
    int length = this.ShocksBones.Length;
    float num1 = 0.55f;
    float num2 = 0.79f;
    for (int index = 0; index < length; ++index)
    {
      ((Ray) ref ray).set_origin(this.ShockTraceLineBegin[index].get_position());
      ((Ray) ref ray).set_direction(Vector3.op_Multiply(((Component) this).get_transform().get_up(), -1f));
      RaycastHit raycastHit;
      float num3 = !Physics.SphereCast(ray, 0.15f, ref raycastHit, num2, mask) ? 0.26f : ((RaycastHit) ref raycastHit).get_distance() - num1;
      this.vecShocksOffsetPosition[index].y = (__Null) (double) Mathf.Lerp((float) this.vecShocksOffsetPosition[index].y, Mathf.Clamp(num3 * -1f, -0.26f, 0.0f), Time.get_deltaTime() * 5f);
      this.ShocksBones[index].set_localPosition(this.vecShocksOffsetPosition[index]);
    }
  }

  private void TrackTurret()
  {
    if (!Object.op_Inequality((Object) this.targetTurret, (Object) null))
      return;
    Vector3 vector3 = Vector3.op_Subtraction(this.targetTurret.get_position(), this.turret.get_position());
    ((Vector3) ref vector3).get_normalized();
    float yaw;
    float pitch;
    this.CalculateYawPitchOffset(this.turret, this.turret.get_position(), this.targetTurret.get_position(), out yaw, out pitch);
    yaw = this.NormalizeYaw(yaw);
    float num1 = Time.get_deltaTime() * this.turretTurnSpeed;
    if ((double) yaw < -0.5)
      this.vecTurret.y = (__Null) ((this.vecTurret.y - (double) num1) % 360.0);
    else if ((double) yaw > 0.5)
      this.vecTurret.y = (__Null) ((this.vecTurret.y + (double) num1) % 360.0);
    this.turret.set_localEulerAngles(this.vecTurret);
    float num2 = Time.get_deltaTime() * this.cannonPitchSpeed;
    this.CalculateYawPitchOffset(this.mainCannon, this.mainCannon.get_position(), this.targetTurret.get_position(), out yaw, out pitch);
    if ((double) pitch < -0.5)
      this.vecMainCannon.x = (__Null) (this.vecMainCannon.x - (double) num2);
    else if ((double) pitch > 0.5)
      this.vecMainCannon.x = (__Null) (this.vecMainCannon.x + (double) num2);
    this.vecMainCannon.x = (__Null) (double) Mathf.Clamp((float) this.vecMainCannon.x, -55f, 5f);
    this.mainCannon.set_localEulerAngles(this.vecMainCannon);
    if ((double) pitch < -0.5)
      this.vecCoaxGun.x = (__Null) (this.vecCoaxGun.x - (double) num2);
    else if ((double) pitch > 0.5)
      this.vecCoaxGun.x = (__Null) (this.vecCoaxGun.x + (double) num2);
    this.vecCoaxGun.x = (__Null) (double) Mathf.Clamp((float) this.vecCoaxGun.x, -65f, 15f);
    this.coaxGun.set_localEulerAngles(this.vecCoaxGun);
    if (this.rocketsOpen)
    {
      float num3 = Time.get_deltaTime() * this.rocketPitchSpeed;
      this.CalculateYawPitchOffset(this.rocketsPitch, this.rocketsPitch.get_position(), this.targetTurret.get_position(), out yaw, out pitch);
      if ((double) pitch < -0.5)
      {
        ref __Null local = ref this.vecRocketsPitch.x;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local - num3;
      }
      else if ((double) pitch > 0.5)
      {
        ref __Null local = ref this.vecRocketsPitch.x;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local + num3;
      }
      this.vecRocketsPitch.x = (__Null) (double) Mathf.Clamp((float) this.vecRocketsPitch.x, -45f, 45f);
    }
    else
      this.vecRocketsPitch.x = (__Null) (double) Mathf.Lerp((float) this.vecRocketsPitch.x, 0.0f, Time.get_deltaTime() * 1.7f);
    this.rocketsPitch.set_localEulerAngles(this.vecRocketsPitch);
  }

  private void TrackSpotLight()
  {
    if (!Object.op_Inequality((Object) this.targetSpotLight, (Object) null))
      return;
    Vector3 vector3 = Vector3.op_Subtraction(this.targetSpotLight.get_position(), this.spotLightYaw.get_position());
    ((Vector3) ref vector3).get_normalized();
    float yaw;
    float pitch;
    this.CalculateYawPitchOffset(this.spotLightYaw, this.spotLightYaw.get_position(), this.targetSpotLight.get_position(), out yaw, out pitch);
    yaw = this.NormalizeYaw(yaw);
    float num = Time.get_deltaTime() * this.spotLightTurnSpeed;
    if ((double) yaw < -0.5)
      this.vecSpotLightBase.y = (__Null) ((this.vecSpotLightBase.y - (double) num) % 360.0);
    else if ((double) yaw > 0.5)
      this.vecSpotLightBase.y = (__Null) ((this.vecSpotLightBase.y + (double) num) % 360.0);
    this.spotLightYaw.set_localEulerAngles(this.vecSpotLightBase);
    this.CalculateYawPitchOffset(this.spotLightPitch, this.spotLightPitch.get_position(), this.targetSpotLight.get_position(), out yaw, out pitch);
    if ((double) pitch < -0.5)
    {
      ref __Null local = ref this.vecSpotLight.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local - num;
    }
    else if ((double) pitch > 0.5)
    {
      ref __Null local = ref this.vecSpotLight.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local + num;
    }
    this.vecSpotLight.x = (__Null) (double) Mathf.Clamp((float) this.vecSpotLight.x, -50f, 50f);
    this.spotLightPitch.set_localEulerAngles(this.vecSpotLight);
    this.m2Animator.SetFloat("sideMG_pitch", (float) this.vecSpotLight.x, 0.5f, Time.get_deltaTime());
  }

  private void TrackSideGuns()
  {
    for (int index = 0; index < this.sideguns.Length; ++index)
    {
      if (!Object.op_Equality((Object) this.targetSideguns[index], (Object) null))
      {
        Vector3 vector3 = Vector3.op_Subtraction(this.targetSideguns[index].get_position(), this.sideguns[index].get_position());
        ((Vector3) ref vector3).get_normalized();
        float yaw;
        float pitch;
        this.CalculateYawPitchOffset(this.sideguns[index], this.sideguns[index].get_position(), this.targetSideguns[index].get_position(), out yaw, out pitch);
        yaw = this.NormalizeYaw(yaw);
        float num = Time.get_deltaTime() * this.sidegunsTurnSpeed;
        if ((double) yaw < -0.5)
        {
          ref __Null local = ref this.vecSideGunRotation[index].y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local - num;
        }
        else if ((double) yaw > 0.5)
        {
          ref __Null local = ref this.vecSideGunRotation[index].y;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + num;
        }
        if ((double) pitch < -0.5)
        {
          ref __Null local = ref this.vecSideGunRotation[index].x;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local - num;
        }
        else if ((double) pitch > 0.5)
        {
          ref __Null local = ref this.vecSideGunRotation[index].x;
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          // ISSUE: cast to a reference type
          // ISSUE: explicit reference operation
          ^(float&) ref local = ^(float&) ref local + num;
        }
        this.vecSideGunRotation[index].x = (__Null) (double) Mathf.Clamp((float) this.vecSideGunRotation[index].x, -45f, 45f);
        this.vecSideGunRotation[index].y = (__Null) (double) Mathf.Clamp((float) this.vecSideGunRotation[index].y, -45f, 45f);
        this.sideguns[index].set_localEulerAngles(this.vecSideGunRotation[index]);
      }
    }
  }

  public void CalculateYawPitchOffset(
    Transform objectTransform,
    Vector3 vecStart,
    Vector3 vecEnd,
    out float yaw,
    out float pitch)
  {
    Vector3 vector3_1 = objectTransform.InverseTransformDirection(Vector3.op_Subtraction(vecEnd, vecStart));
    float num1 = Mathf.Sqrt((float) (vector3_1.x * vector3_1.x + vector3_1.z * vector3_1.z));
    pitch = (float) (-(double) Mathf.Atan2((float) vector3_1.y, num1) * 57.2957763671875);
    Vector3 vector3_2 = Vector3.op_Subtraction(vecEnd, vecStart);
    Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
    Vector3 forward = objectTransform.get_forward();
    forward.y = (__Null) 0.0;
    ((Vector3) ref forward).Normalize();
    float num2 = Vector3.Dot(normalized, forward);
    float num3 = 360f * Vector3.Dot(normalized, objectTransform.get_right());
    float num4 = (float) (360.0 * -(double) num2);
    yaw = (float) (((double) Mathf.Atan2(num3, num4) + 3.14159274101257) * 57.2957763671875);
  }

  public float NormalizeYaw(float flYaw)
  {
    return (double) flYaw <= 180.0 ? flYaw * -1f : 360f - flYaw;
  }

  public m2bradleyAnimator()
  {
    base.\u002Ector();
  }
}
