// Decompiled with JetBrains decompiler
// Type: Effect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Rust;
using UnityEngine;

public class Effect : EffectData
{
  private static Effect reusableInstace = new Effect();
  public Vector3 Up;
  public Vector3 worldPos;
  public Vector3 worldNrm;
  public bool attached;
  public Transform transform;
  public GameObject gameObject;
  public string pooledString;
  public bool broadcast;

  public Effect()
  {
    base.\u002Ector();
  }

  public Effect(
    string effectName,
    Vector3 posWorld,
    Vector3 normWorld,
    Connection sourceConnection = null)
  {
    base.\u002Ector();
    this.Init(Effect.Type.Generic, posWorld, normWorld, sourceConnection);
    this.pooledString = effectName;
  }

  public Effect(
    string effectName,
    BaseEntity ent,
    uint boneID,
    Vector3 posLocal,
    Vector3 normLocal,
    Connection sourceConnection = null)
  {
    base.\u002Ector();
    this.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
    this.pooledString = effectName;
  }

  public void Init(
    Effect.Type fxtype,
    BaseEntity ent,
    uint boneID,
    Vector3 posLocal,
    Vector3 normLocal,
    Connection sourceConnection = null)
  {
    this.Clear();
    this.type = (__Null) (int) fxtype;
    this.attached = true;
    this.origin = (__Null) posLocal;
    this.normal = (__Null) normLocal;
    this.gameObject = (GameObject) null;
    this.Up = Vector3.get_zero();
    if (Object.op_Inequality((Object) ent, (Object) null) && !ent.IsValid())
      Debug.LogWarning((object) "Effect.Init - invalid entity");
    this.entity = ent.IsValid() ? (__Null) (int) ent.net.ID : (__Null) 0;
    this.source = sourceConnection != null ? (__Null) (long) sourceConnection.userid : (__Null) 0L;
    this.bone = (__Null) (int) boneID;
  }

  public void Init(
    Effect.Type fxtype,
    Vector3 posWorld,
    Vector3 normWorld,
    Connection sourceConnection = null)
  {
    this.Clear();
    this.type = (__Null) (int) fxtype;
    this.attached = false;
    this.worldPos = posWorld;
    this.worldNrm = normWorld;
    this.gameObject = (GameObject) null;
    this.Up = Vector3.get_zero();
    this.entity = (__Null) 0;
    this.origin = (__Null) this.worldPos;
    this.normal = (__Null) this.worldNrm;
    this.bone = (__Null) 0;
    this.source = sourceConnection != null ? (__Null) (long) sourceConnection.userid : (__Null) 0L;
  }

  public void Clear()
  {
    this.worldPos = Vector3.get_zero();
    this.worldNrm = Vector3.get_zero();
    this.attached = false;
    this.transform = (Transform) null;
    this.gameObject = (GameObject) null;
    this.pooledString = (string) null;
    this.broadcast = false;
  }

  public enum Type : uint
  {
    Generic,
    Projectile,
  }

  public static class client
  {
    public static void Run(
      Effect.Type fxtype,
      BaseEntity ent,
      uint boneID,
      Vector3 posLocal,
      Vector3 normLocal)
    {
    }

    public static void Run(
      string strName,
      BaseEntity ent,
      uint boneID,
      Vector3 posLocal,
      Vector3 normLocal)
    {
      string.IsNullOrEmpty(strName);
    }

    public static void Run(Effect.Type fxtype, Vector3 posWorld, Vector3 normWorld, Vector3 up = null)
    {
    }

    public static void Run(string strName, Vector3 posWorld = null, Vector3 normWorld = null, Vector3 up = null)
    {
      string.IsNullOrEmpty(strName);
    }

    public static void Run(string strName, GameObject obj)
    {
      string.IsNullOrEmpty(strName);
    }

    public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
    {
      if (info.HitEntity.IsValid())
        Effect.client.Run(effectName, info.HitEntity, info.HitBone, Vector3.op_Addition(info.HitPositionLocal, Vector3.op_Multiply(info.HitNormalLocal, 0.1f)), info.HitNormalLocal);
      else
        Effect.client.Run(effectName, Vector3.op_Addition(info.HitPositionWorld, Vector3.op_Multiply(info.HitNormalWorld, 0.1f)), info.HitNormalWorld, (Vector3) null);
    }

    public static void ImpactEffect(HitInfo info)
    {
      string materialName = StringPool.Get(info.HitMaterial);
      string strName = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), materialName);
      string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), materialName);
      if (Object.op_Inequality((Object) TerrainMeta.WaterMap, (Object) null) && (int) info.HitMaterial != (int) Projectile.WaterMaterialID() && ((int) info.HitMaterial != (int) Projectile.FleshMaterialID() && info.HitPositionWorld.y < (double) TerrainMeta.WaterMap.GetHeight(info.HitPositionWorld)))
        return;
      if (info.HitEntity.IsValid())
      {
        GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
        if (impactEffect.isValid)
          strName = impactEffect.resourcePath;
        Effect.client.Run(strName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
        if (info.DoDecals)
          Effect.client.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
      }
      else
      {
        Effect.client.Run(strName, info.HitPositionWorld, info.HitNormalWorld, (Vector3) null);
        Effect.client.Run(decal, info.HitPositionWorld, info.HitNormalWorld, (Vector3) null);
      }
      if (Object.op_Implicit((Object) info.WeaponPrefab))
      {
        BaseMelee weaponPrefab = info.WeaponPrefab as BaseMelee;
        if (Object.op_Inequality((Object) weaponPrefab, (Object) null))
        {
          string strikeEffectPath = weaponPrefab.GetStrikeEffectPath(materialName);
          if (info.HitEntity.IsValid())
            Effect.client.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
          else
            Effect.client.Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, (Vector3) null);
        }
      }
      if (info.damageTypes.Has(DamageType.Explosion))
        Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
      if (!info.damageTypes.Has(DamageType.Heat))
        return;
      Effect.client.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
    }
  }

  public static class server
  {
    public static void Run(
      Effect.Type fxtype,
      BaseEntity ent,
      uint boneID,
      Vector3 posLocal,
      Vector3 normLocal,
      Connection sourceConnection = null,
      bool broadcast = false)
    {
      Effect.reusableInstace.Init(fxtype, ent, boneID, posLocal, normLocal, sourceConnection);
      Effect.reusableInstace.broadcast = broadcast;
      EffectNetwork.Send(Effect.reusableInstace);
    }

    public static void Run(
      string strName,
      BaseEntity ent,
      uint boneID,
      Vector3 posLocal,
      Vector3 normLocal,
      Connection sourceConnection = null,
      bool broadcast = false)
    {
      if (string.IsNullOrEmpty(strName))
        return;
      Effect.reusableInstace.Init(Effect.Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
      Effect.reusableInstace.pooledString = strName;
      Effect.reusableInstace.broadcast = broadcast;
      EffectNetwork.Send(Effect.reusableInstace);
    }

    public static void Run(
      Effect.Type fxtype,
      Vector3 posWorld,
      Vector3 normWorld,
      Connection sourceConnection = null,
      bool broadcast = false)
    {
      Effect.reusableInstace.Init(fxtype, posWorld, normWorld, sourceConnection);
      Effect.reusableInstace.broadcast = broadcast;
      EffectNetwork.Send(Effect.reusableInstace);
    }

    public static void Run(
      string strName,
      Vector3 posWorld = null,
      Vector3 normWorld = null,
      Connection sourceConnection = null,
      bool broadcast = false)
    {
      if (string.IsNullOrEmpty(strName))
        return;
      Effect.reusableInstace.Init(Effect.Type.Generic, posWorld, normWorld, sourceConnection);
      Effect.reusableInstace.pooledString = strName;
      Effect.reusableInstace.broadcast = broadcast;
      EffectNetwork.Send(Effect.reusableInstace);
    }

    public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
    {
      if (info.HitEntity.IsValid())
        Effect.server.Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
      else
        Effect.server.Run(effectName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
    }

    public static void ImpactEffect(HitInfo info)
    {
      if (!info.DoHitEffects)
        return;
      string materialName = StringPool.Get(info.HitMaterial);
      if (Object.op_Inequality((Object) TerrainMeta.WaterMap, (Object) null) && (int) info.HitMaterial != (int) Projectile.WaterMaterialID() && ((int) info.HitMaterial != (int) Projectile.FleshMaterialID() && info.HitPositionWorld.y < (double) WaterLevel.GetWaterDepth(info.HitPositionWorld)))
        return;
      string strName = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), materialName);
      string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), materialName);
      if (info.HitEntity.IsValid())
      {
        GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
        if (impactEffect.isValid)
          strName = impactEffect.resourcePath;
        Effect.server.Run(strName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
        Effect.server.Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
      }
      else
      {
        Effect.server.Run(strName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
        Effect.server.Run(decal, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
      }
      if (Object.op_Implicit((Object) info.WeaponPrefab))
      {
        BaseMelee weaponPrefab = info.WeaponPrefab as BaseMelee;
        if (Object.op_Inequality((Object) weaponPrefab, (Object) null))
        {
          string strikeEffectPath = weaponPrefab.GetStrikeEffectPath(materialName);
          if (info.HitEntity.IsValid())
            Effect.server.Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted, false);
          else
            Effect.server.Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, info.Predicted, false);
        }
      }
      if (info.damageTypes.Has(DamageType.Explosion))
        Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
      if (!info.damageTypes.Has(DamageType.Heat))
        return;
      Effect.server.DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
    }
  }
}
