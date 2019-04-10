// Decompiled with JetBrains decompiler
// Type: PlayerModel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PlayerModel : ListComponent<PlayerModel>
{
  protected static int speed = Animator.StringToHash(nameof (speed));
  protected static int acceleration = Animator.StringToHash(nameof (acceleration));
  protected static int rotationYaw = Animator.StringToHash(nameof (rotationYaw));
  protected static int forward = Animator.StringToHash(nameof (forward));
  protected static int right = Animator.StringToHash(nameof (right));
  protected static int up = Animator.StringToHash(nameof (up));
  protected static int ducked = Animator.StringToHash(nameof (ducked));
  protected static int grounded = Animator.StringToHash(nameof (grounded));
  protected static int waterlevel = Animator.StringToHash(nameof (waterlevel));
  protected static int attack = Animator.StringToHash(nameof (attack));
  protected static int attack_alt = Animator.StringToHash(nameof (attack_alt));
  protected static int deploy = Animator.StringToHash(nameof (deploy));
  protected static int reload = Animator.StringToHash(nameof (reload));
  protected static int throwWeapon = Animator.StringToHash("throw");
  protected static int holster = Animator.StringToHash(nameof (holster));
  protected static int aiming = Animator.StringToHash(nameof (aiming));
  protected static int onLadder = Animator.StringToHash(nameof (onLadder));
  protected static int posing = Animator.StringToHash(nameof (posing));
  protected static int poseType = Animator.StringToHash(nameof (poseType));
  protected static int relaxGunPose = Animator.StringToHash(nameof (relaxGunPose));
  protected static int vehicle_aim_yaw = Animator.StringToHash("vehicleAimYaw");
  protected static int vehicle_aim_speed = Animator.StringToHash("vehicleAimYawSpeed");
  protected static int leftFootIK = Animator.StringToHash(nameof (leftFootIK));
  protected static int rightFootIK = Animator.StringToHash(nameof (rightFootIK));
  public BoxCollider collision;
  public GameObject censorshipCube;
  public GameObject censorshipCubeBreasts;
  public GameObject jawBone;
  public GameObject neckBone;
  public GameObject headBone;
  public SkeletonScale skeletonScale;
  public EyeController eyeController;
  public Transform[] SpineBones;
  public Transform leftFootBone;
  public Transform rightFootBone;
  public Vector3 rightHandTarget;
  public Vector3 leftHandTargetPosition;
  public Quaternion leftHandTargetRotation;
  public RuntimeAnimatorController DefaultHoldType;
  public RuntimeAnimatorController SleepGesture;
  public RuntimeAnimatorController WoundedGesture;
  public RuntimeAnimatorController CurrentGesture;
  [Header("Skin")]
  public SkinSetCollection MaleSkin;
  public SkinSetCollection FemaleSkin;
  public SubsurfaceProfile subsurfaceProfile;
  [Header("Parameters")]
  [Range(0.0f, 1f)]
  public float voiceVolume;
  [Range(0.0f, 1f)]
  public float skinColor;
  [Range(0.0f, 1f)]
  public float skinNumber;
  [Range(0.0f, 1f)]
  public float meshNumber;
  [Range(0.0f, 1f)]
  public float hairNumber;
  [Range(0.0f, 1f)]
  public int skinType;
  public MovementSounds movementSounds;

  public bool IsFemale
  {
    get
    {
      return this.skinType == 1;
    }
  }

  public SkinSetCollection SkinSet
  {
    get
    {
      if (this.IsFemale)
        return this.FemaleSkin;
      return this.MaleSkin;
    }
  }

  public Quaternion LookAngles { get; set; }

  public PlayerModel()
  {
    base.\u002Ector();
  }

  public enum MountPoses
  {
    Chair = 0,
    Driving = 1,
    Horseback = 2,
    HeliUnarmed = 3,
    HeliArmed = 4,
    HandMotorBoat = 5,
    MotorBoatPassenger = 6,
    SitGeneric = 7,
    SitRaft = 8,
    StandDrive = 9,
    SitShootingGeneric = 10, // 0x0000000A
    SitMinicopter_Pilot = 11, // 0x0000000B
    SitMinicopter_Passenger = 12, // 0x0000000C
    Standing = 128, // 0x00000080
  }
}
