// Decompiled with JetBrains decompiler
// Type: Projectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : BaseMonoBehaviour
{
  public float gravityModifier = 1f;
  [Range(0.0f, 1f)]
  public float stickProbability = 1f;
  public float penetrationPower = 1f;
  [Horizontal(2, -1)]
  public MinMax damageDistances = new MinMax(10f, 100f);
  [Horizontal(2, -1)]
  public MinMax damageMultipliers = new MinMax(1f, 0.8f);
  public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();
  public bool createDecals = true;
  public float flybySoundDistance = 7f;
  public float closeFlybyDistance = 3f;
  public Vector3 tumbleAxis = Vector3.get_right();
  [NonSerialized]
  public float integrity = 1f;
  [NonSerialized]
  public float maxDistance = float.PositiveInfinity;
  [NonSerialized]
  public Projectile.Modifier modifier = Projectile.Modifier.Default;
  public const float lifeTime = 8f;
  [Header("Attributes")]
  public Vector3 initialVelocity;
  public float drag;
  public float thickness;
  [Tooltip("This projectile will raycast for this many units, and then become a projectile. This is typically done for bullets.")]
  public float initialDistance;
  [Header("Impact Rules")]
  public bool remainInWorld;
  [Range(0.0f, 1f)]
  public float breakProbability;
  [Range(0.0f, 1f)]
  public float conditionLoss;
  [Range(0.0f, 1f)]
  public float ricochetChance;
  [Header("Damage")]
  public DamageProperties damageProperties;
  [Header("Rendering")]
  public ScaleRenderer rendererToScale;
  public ScaleRenderer firstPersonRenderer;
  [Header("Audio")]
  public SoundDefinition flybySound;
  public SoundDefinition closeFlybySound;
  [Header("Tumble")]
  public float tumbleSpeed;
  [Header("Swim")]
  public Vector3 swimScale;
  public Vector3 swimSpeed;
  [NonSerialized]
  public BasePlayer owner;
  [NonSerialized]
  public AttackEntity sourceWeaponPrefab;
  [NonSerialized]
  public Projectile sourceProjectilePrefab;
  [NonSerialized]
  public ItemModProjectile mod;
  [NonSerialized]
  public int projectileID;
  [NonSerialized]
  public int seed;
  [NonSerialized]
  public bool clientsideEffect;
  [NonSerialized]
  public bool clientsideAttack;
  [NonSerialized]
  public bool invisible;
  private static uint _fleshMaterialID;
  private static uint _waterMaterialID;
  private static uint cachedWaterString;

  public void CalculateDamage(HitInfo info, Projectile.Modifier mod, float scale)
  {
    float num1 = this.damageMultipliers.Lerp(mod.distanceOffset + mod.distanceScale * this.damageDistances.x, mod.distanceOffset + mod.distanceScale * this.damageDistances.y, info.ProjectileDistance);
    float num2 = scale * (mod.damageOffset + mod.damageScale * num1);
    foreach (DamageTypeEntry damageType in this.damageTypes)
      info.damageTypes.Add(damageType.type, damageType.amount * num2);
    if (Global.developer <= 0)
      return;
    Debug.Log((object) (" Projectile damage: " + (object) info.damageTypes.Total() + " (scalar=" + (object) num2 + ")"));
  }

  public static uint FleshMaterialID()
  {
    if (Projectile._fleshMaterialID == 0U)
      Projectile._fleshMaterialID = StringPool.Get("flesh");
    return Projectile._fleshMaterialID;
  }

  public static uint WaterMaterialID()
  {
    if (Projectile._waterMaterialID == 0U)
      Projectile._waterMaterialID = StringPool.Get("Water");
    return Projectile._waterMaterialID;
  }

  public static bool IsWaterMaterial(string hitMaterial)
  {
    if (Projectile.cachedWaterString == 0U)
      Projectile.cachedWaterString = StringPool.Get("Water");
    return (int) StringPool.Get(hitMaterial) == (int) Projectile.cachedWaterString;
  }

  public struct Modifier
  {
    public static Projectile.Modifier Default = new Projectile.Modifier()
    {
      damageScale = 1f,
      damageOffset = 0.0f,
      distanceScale = 1f,
      distanceOffset = 0.0f
    };
    public float damageScale;
    public float damageOffset;
    public float distanceScale;
    public float distanceOffset;
  }
}
