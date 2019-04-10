// Decompiled with JetBrains decompiler
// Type: ProjectileWeaponMod
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileWeaponMod : BaseEntity
{
  [Header("Silencer")]
  public GameObjectRef defaultSilencerEffect;
  public bool isSilencer;
  [Header("Weapon Basics")]
  public ProjectileWeaponMod.Modifier repeatDelay;
  public ProjectileWeaponMod.Modifier projectileVelocity;
  public ProjectileWeaponMod.Modifier projectileDamage;
  public ProjectileWeaponMod.Modifier projectileDistance;
  [Header("Recoil")]
  public ProjectileWeaponMod.Modifier aimsway;
  public ProjectileWeaponMod.Modifier aimswaySpeed;
  public ProjectileWeaponMod.Modifier recoil;
  [Header("Aim Cone")]
  public ProjectileWeaponMod.Modifier sightAimCone;
  public ProjectileWeaponMod.Modifier hipAimCone;
  [Header("Light Effects")]
  public bool isLight;
  [Header("MuzzleBrake")]
  public bool isMuzzleBrake;
  [Header("MuzzleBoost")]
  public bool isMuzzleBoost;
  [Header("Scope")]
  public bool isScope;
  public float zoomAmountDisplayOnly;
  public bool needsOnForEffects;

  public override void ServerInit()
  {
    this.SetFlag(BaseEntity.Flags.Disabled, true, false, true);
    base.ServerInit();
  }

  public override void PostServerLoad()
  {
    this.limitNetworking = this.HasFlag(BaseEntity.Flags.Disabled);
  }

  public static float Sum(
    BaseEntity parentEnt,
    Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier,
    Func<ProjectileWeaponMod.Modifier, float> selector_value,
    float def)
  {
    if (parentEnt.children == null)
      return def;
    IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
    if (mods.Count<float>() != 0)
      return mods.Sum();
    return def;
  }

  public static float Average(
    BaseEntity parentEnt,
    Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier,
    Func<ProjectileWeaponMod.Modifier, float> selector_value,
    float def)
  {
    if (parentEnt.children == null)
      return def;
    IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
    if (mods.Count<float>() != 0)
      return mods.Average();
    return def;
  }

  public static float Max(
    BaseEntity parentEnt,
    Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier,
    Func<ProjectileWeaponMod.Modifier, float> selector_value,
    float def)
  {
    if (parentEnt.children == null)
      return def;
    IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
    if (mods.Count<float>() != 0)
      return mods.Max();
    return def;
  }

  public static float Min(
    BaseEntity parentEnt,
    Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier,
    Func<ProjectileWeaponMod.Modifier, float> selector_value,
    float def)
  {
    if (parentEnt.children == null)
      return def;
    IEnumerable<float> mods = ProjectileWeaponMod.GetMods(parentEnt, selector_modifier, selector_value);
    if (mods.Count<float>() != 0)
      return mods.Min();
    return def;
  }

  public static IEnumerable<float> GetMods(
    BaseEntity parentEnt,
    Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier> selector_modifier,
    Func<ProjectileWeaponMod.Modifier, float> selector_value)
  {
    return parentEnt.children.Cast<ProjectileWeaponMod>().Where<ProjectileWeaponMod>((Func<ProjectileWeaponMod, bool>) (x =>
    {
      if (!Object.op_Inequality((Object) x, (Object) null))
        return false;
      if (x.needsOnForEffects)
        return x.HasFlag(BaseEntity.Flags.On);
      return true;
    })).Select<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>(selector_modifier).Where<ProjectileWeaponMod.Modifier>((Func<ProjectileWeaponMod.Modifier, bool>) (x => x.enabled)).Select<ProjectileWeaponMod.Modifier, float>(selector_value);
  }

  public static bool HasBrokenWeaponMod(BaseEntity parentEnt)
  {
    return parentEnt.children != null && parentEnt.children.Cast<ProjectileWeaponMod>().Any<ProjectileWeaponMod>((Func<ProjectileWeaponMod, bool>) (x =>
    {
      if (Object.op_Inequality((Object) x, (Object) null))
        return x.IsBroken();
      return false;
    }));
  }

  [Serializable]
  public struct Modifier
  {
    public bool enabled;
    [Tooltip("1 means no change. 0.5 is half.")]
    public float scalar;
    [Tooltip("Added after the scalar is applied.")]
    public float offset;
  }
}
