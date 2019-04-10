// Decompiled with JetBrains decompiler
// Type: HitInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

public class HitInfo
{
  public bool DoHitEffects = true;
  public bool DoDecals = true;
  public bool UseProtection = true;
  public DamageTypeList damageTypes = new DamageTypeList();
  public float gatherScale = 1f;
  public BaseEntity Initiator;
  public BaseEntity WeaponPrefab;
  public AttackEntity Weapon;
  public bool IsPredicting;
  public Connection Predicted;
  public bool DidHit;
  public BaseEntity HitEntity;
  public uint HitBone;
  public uint HitPart;
  public uint HitMaterial;
  public Vector3 HitPositionWorld;
  public Vector3 HitPositionLocal;
  public Vector3 HitNormalWorld;
  public Vector3 HitNormalLocal;
  public Vector3 PointStart;
  public Vector3 PointEnd;
  public int ProjectileID;
  public float ProjectileDistance;
  public Vector3 ProjectileVelocity;
  public Projectile ProjectilePrefab;
  public PhysicMaterial material;
  public DamageProperties damageProperties;
  public bool CanGather;
  public bool DidGather;

  public bool IsProjectile()
  {
    return (uint) this.ProjectileID > 0U;
  }

  public BasePlayer InitiatorPlayer
  {
    get
    {
      if (!Object.op_Implicit((Object) this.Initiator))
        return (BasePlayer) null;
      return this.Initiator.ToPlayer();
    }
  }

  public Vector3 attackNormal
  {
    get
    {
      Vector3 vector3 = Vector3.op_Subtraction(this.PointEnd, this.PointStart);
      return ((Vector3) ref vector3).get_normalized();
    }
  }

  public bool hasDamage
  {
    get
    {
      return (double) this.damageTypes.Total() > 0.0;
    }
  }

  public HitInfo()
  {
  }

  public HitInfo(
    BaseEntity attacker,
    BaseEntity target,
    DamageType type,
    float damageAmount,
    Vector3 vhitPosition)
  {
    this.Initiator = attacker;
    this.HitEntity = target;
    this.HitPositionWorld = vhitPosition;
    if (Object.op_Inequality((Object) attacker, (Object) null))
      this.PointStart = ((Component) attacker).get_transform().get_position();
    this.damageTypes.Add(type, damageAmount);
  }

  public HitInfo(BaseEntity attacker, BaseEntity target, DamageType type, float damageAmount)
    : this(attacker, target, type, damageAmount, ((Component) target).get_transform().get_position())
  {
  }

  public void LoadFromAttack(Attack attack, bool serverSide)
  {
    this.HitEntity = (BaseEntity) null;
    this.PointStart = (Vector3) attack.pointStart;
    this.PointEnd = (Vector3) attack.pointEnd;
    if (attack.hitID > 0)
    {
      this.DidHit = true;
      if (serverSide)
        this.HitEntity = BaseNetworkable.serverEntities.Find((uint) attack.hitID) as BaseEntity;
      if (Object.op_Implicit((Object) this.HitEntity))
      {
        this.HitBone = (uint) attack.hitBone;
        this.HitPart = (uint) attack.hitPartID;
      }
    }
    this.DidHit = true;
    this.HitPositionLocal = (Vector3) attack.hitPositionLocal;
    this.HitPositionWorld = (Vector3) attack.hitPositionWorld;
    this.HitNormalLocal = ((Vector3) ref attack.hitNormalLocal).get_normalized();
    this.HitNormalWorld = ((Vector3) ref attack.hitNormalWorld).get_normalized();
    this.HitMaterial = (uint) attack.hitMaterialID;
  }

  public bool isHeadshot
  {
    get
    {
      if (Object.op_Equality((Object) this.HitEntity, (Object) null))
        return false;
      BaseCombatEntity hitEntity = this.HitEntity as BaseCombatEntity;
      if (Object.op_Equality((Object) hitEntity, (Object) null) || Object.op_Equality((Object) hitEntity.skeletonProperties, (Object) null))
        return false;
      SkeletonProperties.BoneProperty bone = hitEntity.skeletonProperties.FindBone(this.HitBone);
      if (bone == null)
        return false;
      return bone.area == HitArea.Head;
    }
  }

  public Translate.Phrase bonePhrase
  {
    get
    {
      if (Object.op_Equality((Object) this.HitEntity, (Object) null))
        return (Translate.Phrase) null;
      BaseCombatEntity hitEntity = this.HitEntity as BaseCombatEntity;
      if (Object.op_Equality((Object) hitEntity, (Object) null))
        return (Translate.Phrase) null;
      if (Object.op_Equality((Object) hitEntity.skeletonProperties, (Object) null))
        return (Translate.Phrase) null;
      return hitEntity.skeletonProperties.FindBone(this.HitBone)?.name;
    }
  }

  public string boneName
  {
    get
    {
      Translate.Phrase bonePhrase = this.bonePhrase;
      if (bonePhrase != null)
        return bonePhrase.english;
      return "N/A";
    }
  }

  public HitArea boneArea
  {
    get
    {
      if (Object.op_Equality((Object) this.HitEntity, (Object) null))
        return (HitArea) -1;
      BaseCombatEntity hitEntity = this.HitEntity as BaseCombatEntity;
      if (Object.op_Equality((Object) hitEntity, (Object) null))
        return (HitArea) -1;
      return hitEntity.SkeletonLookup(this.HitBone);
    }
  }

  public Vector3 PositionOnRay(Vector3 position)
  {
    Ray ray;
    ((Ray) ref ray).\u002Ector(this.PointStart, this.attackNormal);
    if (Object.op_Equality((Object) this.ProjectilePrefab, (Object) null))
      return ray.ClosestPoint(position);
    Sphere sphere;
    ((Sphere) ref sphere).\u002Ector(position, this.ProjectilePrefab.thickness);
    RaycastHit raycastHit;
    if (((Sphere) ref sphere).Trace(ray, ref raycastHit, float.PositiveInfinity))
      return ((RaycastHit) ref raycastHit).get_point();
    return position;
  }

  public Vector3 HitPositionOnRay()
  {
    return this.PositionOnRay(this.HitPositionWorld);
  }

  public bool IsNaNOrInfinity()
  {
    return Vector3Ex.IsNaNOrInfinity(this.PointStart) || Vector3Ex.IsNaNOrInfinity(this.PointEnd) || (Vector3Ex.IsNaNOrInfinity(this.HitPositionWorld) || Vector3Ex.IsNaNOrInfinity(this.HitPositionLocal)) || (Vector3Ex.IsNaNOrInfinity(this.HitNormalWorld) || Vector3Ex.IsNaNOrInfinity(this.HitNormalLocal) || (Vector3Ex.IsNaNOrInfinity(this.ProjectileVelocity) || float.IsNaN(this.ProjectileDistance))) || float.IsInfinity(this.ProjectileDistance);
  }
}
