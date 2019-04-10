// Decompiled with JetBrains decompiler
// Type: HitTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class HitTest
{
  public HitTest.Type type;
  public Ray AttackRay;
  public float Radius;
  public float Forgiveness;
  public float MaxDistance;
  public RaycastHit RayHit;
  public bool MultiHit;
  public bool BestHit;
  public bool DidHit;
  public DamageProperties damageProperties;
  public GameObject gameObject;
  public Collider collider;
  public BaseEntity ignoreEntity;
  public BaseEntity HitEntity;
  public Vector3 HitPoint;
  public Vector3 HitNormal;
  public float HitDistance;
  public Transform HitTransform;
  public uint HitPart;
  public string HitMaterial;

  public Vector3 HitPointWorld()
  {
    if (!Object.op_Inequality((Object) this.HitEntity, (Object) null))
      return this.HitPoint;
    Transform transform = this.HitTransform;
    if (!Object.op_Implicit((Object) transform))
      transform = ((Component) this.HitEntity).get_transform();
    return transform.TransformPoint(this.HitPoint);
  }

  public Vector3 HitNormalWorld()
  {
    if (!Object.op_Inequality((Object) this.HitEntity, (Object) null))
      return this.HitNormal;
    Transform transform = this.HitTransform;
    if (!Object.op_Implicit((Object) transform))
      transform = ((Component) this.HitEntity).get_transform();
    return transform.TransformDirection(this.HitNormal);
  }

  public void Clear()
  {
    this.type = HitTest.Type.Generic;
    this.AttackRay = (Ray) null;
    this.Radius = 0.0f;
    this.Forgiveness = 0.0f;
    this.MaxDistance = 0.0f;
    this.RayHit = (RaycastHit) null;
    this.MultiHit = false;
    this.BestHit = false;
    this.DidHit = false;
    this.damageProperties = (DamageProperties) null;
    this.gameObject = (GameObject) null;
    this.collider = (Collider) null;
    this.ignoreEntity = (BaseEntity) null;
    this.HitEntity = (BaseEntity) null;
    this.HitPoint = (Vector3) null;
    this.HitNormal = (Vector3) null;
    this.HitDistance = 0.0f;
    this.HitTransform = (Transform) null;
    this.HitPart = 0U;
    this.HitMaterial = (string) null;
  }

  public enum Type
  {
    Generic,
    ProjectileEffect,
    Projectile,
    MeleeAttack,
    Use,
  }
}
