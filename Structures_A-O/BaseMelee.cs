// Decompiled with JetBrains decompiler
// Type: BaseMelee
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Rust.Ai;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseMelee : AttackEntity
{
  public float maxDistance = 1.5f;
  public float attackRadius = 0.3f;
  public bool isAutomatic = true;
  public bool blockSprintOnAttack = true;
  public bool useStandardHitEffects = true;
  [Header("NPCUsage")]
  public float aiStrikeDelay = 0.2f;
  public List<BaseMelee.MaterialFX> materialStrikeFX = new List<BaseMelee.MaterialFX>();
  [Range(0.0f, 1f)]
  [Header("Other")]
  public float heartStress = 0.5f;
  [Header("Melee")]
  public DamageProperties damageProperties;
  public List<DamageTypeEntry> damageTypes;
  [Header("Effects")]
  public GameObjectRef strikeFX;
  public GameObjectRef swingEffect;
  public ResourceDispenser.GatherProperties gathering;
  [Header("Throwing")]
  public bool canThrowAsProjectile;
  public bool canAiHearIt;
  public bool onlyThrowAsProjectile;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BaseMelee.OnRpcMessage", 0.1f))
    {
      if (rpc == 3168282921U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - CLProject "));
        using (TimeWarning.New("CLProject", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.FromOwner.Test("CLProject", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.CLProject(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in CLProject");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 4088326849U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - PlayerAttack "));
          using (TimeWarning.New("PlayerAttack", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsActiveItem.Test("PlayerAttack", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.PlayerAttack(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in PlayerAttack");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void GetAttackStats(HitInfo info)
  {
    info.damageTypes.Add(this.damageTypes);
    info.CanGather = this.gathering.Any();
  }

  public virtual void DoAttackShared(HitInfo info)
  {
    if (Interface.CallHook("OnPlayerAttack", (object) this.GetOwnerPlayer(), (object) info) != null)
      return;
    this.GetAttackStats(info);
    if (Object.op_Inequality((Object) info.HitEntity, (Object) null))
    {
      using (TimeWarning.New("OnAttacked", 50L))
        info.HitEntity.OnAttacked(info);
    }
    if (info.DoHitEffects)
    {
      if (this.isServer)
      {
        using (TimeWarning.New("ImpactEffect", 20L))
          Effect.server.ImpactEffect(info);
      }
      else
      {
        using (TimeWarning.New("ImpactEffect", 20L))
          Effect.client.ImpactEffect(info);
      }
    }
    if (!this.isServer || this.IsDestroyed)
      return;
    using (TimeWarning.New("UpdateItemCondition", 50L))
      this.UpdateItemCondition(info);
    this.StartAttackCooldown(this.repeatDelay);
  }

  public ResourceDispenser.GatherPropertyEntry GetGatherInfoFromIndex(
    ResourceDispenser.GatherType index)
  {
    return this.gathering.GetFromIndex(index);
  }

  public virtual bool CanHit(HitTest info)
  {
    return true;
  }

  public float TotalDamage()
  {
    float num = 0.0f;
    foreach (DamageTypeEntry damageType in this.damageTypes)
    {
      if ((double) damageType.amount > 0.0)
        num += damageType.amount;
    }
    return num;
  }

  public bool IsItemBroken()
  {
    Item ownerItem = this.GetOwnerItem();
    if (ownerItem == null)
      return true;
    return ownerItem.isBroken;
  }

  public void LoseCondition(float amount)
  {
    this.GetOwnerItem()?.LoseCondition(amount);
  }

  public virtual float GetConditionLoss()
  {
    return 1f;
  }

  public void UpdateItemCondition(HitInfo info)
  {
    Item ownerItem = this.GetOwnerItem();
    if (ownerItem == null || !ownerItem.hasCondition || (info == null || !info.DidHit) || info.DidGather)
      return;
    float conditionLoss = this.GetConditionLoss();
    float num = 0.0f;
    foreach (DamageTypeEntry damageType in this.damageTypes)
    {
      if ((double) damageType.amount > 0.0)
        num += Mathf.Clamp(damageType.amount - info.damageTypes.Get(damageType.type), 0.0f, damageType.amount);
    }
    float amount = conditionLoss + num * 0.2f;
    ownerItem.LoseCondition(amount);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  public void PlayerAttack(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.VerifyClientAttack(player))
    {
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    else
    {
      using (TimeWarning.New(nameof (PlayerAttack), 50L))
      {
        using (PlayerAttack playerAttack = PlayerAttack.Deserialize((Stream) msg.read))
        {
          if (playerAttack == null)
            return;
          HitInfo info = (HitInfo) Pool.Get<HitInfo>();
          info.LoadFromAttack((Attack) playerAttack.attack, true);
          info.Initiator = (BaseEntity) player;
          info.Weapon = (AttackEntity) this;
          info.WeaponPrefab = (BaseEntity) this;
          info.Predicted = msg.connection;
          info.damageProperties = this.damageProperties;
          if (Interface.CallHook("OnMeleeAttack", (object) player, (object) info) != null)
            return;
          if (info.IsNaNOrInfinity())
          {
            string shortPrefabName = this.ShortPrefabName;
            AntiHack.Log(player, AntiHackType.MeleeHack, "Contains NaN (" + shortPrefabName + ")");
            player.stats.combat.Log(info, "melee_nan");
          }
          else
          {
            if (ConVar.AntiHack.melee_protection > 0 && Object.op_Implicit((Object) info.HitEntity))
            {
              bool flag = true;
              float num1 = 1f + ConVar.AntiHack.melee_forgiveness;
              double meleeClientframes = (double) ConVar.AntiHack.melee_clientframes;
              float meleeServerframes = ConVar.AntiHack.melee_serverframes;
              float num2 = (float) (meleeClientframes / 60.0);
              float num3 = meleeServerframes * Mathx.Max(Time.get_deltaTime(), Time.get_smoothDeltaTime(), Time.get_fixedDeltaTime());
              float num4 = (player.desyncTime + num2 + num3) * num1;
              if (ConVar.AntiHack.projectile_protection >= 2)
              {
                double num5 = (double) info.HitEntity.MaxVelocity();
                Vector3 parentVelocity = info.HitEntity.GetParentVelocity();
                double magnitude = (double) ((Vector3) ref parentVelocity).get_magnitude();
                float num6 = (float) (num5 + magnitude);
                float num7 = info.HitEntity.BoundsPadding() + num4 * num6;
                float num8 = info.HitEntity.Distance(info.HitPositionWorld);
                if ((double) num8 > (double) num7)
                {
                  string shortPrefabName1 = this.ShortPrefabName;
                  string shortPrefabName2 = info.HitEntity.ShortPrefabName;
                  AntiHack.Log(player, AntiHackType.MeleeHack, "Entity too far away (" + shortPrefabName1 + " on " + shortPrefabName2 + " with " + (object) num8 + "m > " + (object) num7 + "m in " + (object) num4 + "s)");
                  player.stats.combat.Log(info, "melee_distance");
                  flag = false;
                }
              }
              if (ConVar.AntiHack.melee_protection >= 1)
              {
                double num5 = (double) info.Initiator.MaxVelocity();
                Vector3 parentVelocity = info.Initiator.GetParentVelocity();
                double magnitude = (double) ((Vector3) ref parentVelocity).get_magnitude();
                float num6 = (float) (num5 + magnitude);
                float num7 = (float) ((double) info.Initiator.BoundsPadding() + (double) num4 * (double) num6 + (double) num1 * (double) this.maxDistance);
                float num8 = info.Initiator.Distance(info.HitPositionWorld);
                if ((double) num8 > (double) num7)
                {
                  string shortPrefabName1 = this.ShortPrefabName;
                  string shortPrefabName2 = info.HitEntity.ShortPrefabName;
                  AntiHack.Log(player, AntiHackType.MeleeHack, "Initiator too far away (" + shortPrefabName1 + " on " + shortPrefabName2 + " with " + (object) num8 + "m > " + (object) num7 + "m in " + (object) num4 + "s)");
                  player.stats.combat.Log(info, "melee_distance");
                  flag = false;
                }
              }
              if (ConVar.AntiHack.melee_protection >= 3)
              {
                Vector3 pointStart = info.PointStart;
                Vector3 position1 = Vector3.op_Addition(info.HitPositionWorld, Vector3.op_Multiply(((Vector3) ref info.HitNormalWorld).get_normalized(), 1f / 1000f));
                Vector3 center = player.eyes.center;
                Vector3 position2 = player.eyes.position;
                Vector3 p2 = pointStart;
                Vector3 p3 = info.PositionOnRay(position1);
                Vector3 p4 = position1;
                int num5 = GamePhysics.LineOfSight(center, position2, p2, p3, p4, 2162688, 0.0f) ? 1 : 0;
                if (num5 == 0)
                  player.stats.Add("hit_" + info.HitEntity.Categorize() + "_indirect_los", 1, Stats.Server);
                else
                  player.stats.Add("hit_" + info.HitEntity.Categorize() + "_direct_los", 1, Stats.Server);
                if (num5 == 0)
                {
                  string shortPrefabName1 = this.ShortPrefabName;
                  string shortPrefabName2 = info.HitEntity.ShortPrefabName;
                  AntiHack.Log(player, AntiHackType.MeleeHack, "Line of sight (" + shortPrefabName1 + " on " + shortPrefabName2 + ") " + (object) center + " " + (object) position2 + " " + (object) p2 + " " + (object) p3 + " " + (object) p4);
                  player.stats.combat.Log(info, "melee_los");
                  flag = false;
                }
              }
              if (!flag)
              {
                AntiHack.AddViolation(player, AntiHackType.MeleeHack, ConVar.AntiHack.melee_penalty);
                return;
              }
            }
            player.metabolism.UseHeart(this.heartStress * 0.2f);
            using (TimeWarning.New("DoAttackShared", 50L))
              this.DoAttackShared(info);
          }
        }
      }
    }
  }

  public override bool CanBeUsedInWater()
  {
    return true;
  }

  public string GetStrikeEffectPath(string materialName)
  {
    for (int index = 0; index < this.materialStrikeFX.Count; ++index)
    {
      if (this.materialStrikeFX[index].materialName == materialName && this.materialStrikeFX[index].fx.isValid)
        return this.materialStrikeFX[index].fx.resourcePath;
    }
    return this.strikeFX.resourcePath;
  }

  public override void ServerUse()
  {
    if (this.isClient || this.HasAttackCooldown())
      return;
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return;
    this.StartAttackCooldown(this.repeatDelay * 2f);
    ownerPlayer.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, (Connection) null);
    if (this.swingEffect.isValid)
      Effect.server.Run(this.swingEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_forward(), ownerPlayer.net.get_connection(), false);
    if (this.IsInvoking(new Action(this.ServerUse_Strike)))
      this.CancelInvoke(new Action(this.ServerUse_Strike));
    this.Invoke(new Action(this.ServerUse_Strike), this.aiStrikeDelay);
  }

  public void ServerUse_Strike()
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return;
    Vector3 position = ownerPlayer.eyes.position;
    Vector3 vector3 = ownerPlayer.eyes.BodyForward();
    for (int index1 = 0; index1 < 2; ++index1)
    {
      List<RaycastHit> list = (List<RaycastHit>) Pool.GetList<RaycastHit>();
      GamePhysics.TraceAll(new Ray(Vector3.op_Subtraction(position, Vector3.op_Multiply(vector3, index1 == 0 ? 0.0f : 0.2f)), vector3), index1 == 0 ? 0.0f : this.attackRadius, list, this.effectiveRange + 0.2f, 1219701521, (QueryTriggerInteraction) 0);
      bool flag = false;
      for (int index2 = 0; index2 < list.Count; ++index2)
      {
        RaycastHit hit = list[index2];
        BaseEntity entity = hit.GetEntity();
        if (!Object.op_Equality((Object) entity, (Object) null) && (!Object.op_Inequality((Object) entity, (Object) null) || !Object.op_Equality((Object) entity, (Object) ownerPlayer) && !entity.EqualNetID((BaseNetworkable) ownerPlayer)) && ((!Object.op_Inequality((Object) entity, (Object) null) || !entity.isClient) && !(entity.Categorize() == ownerPlayer.Categorize())))
        {
          float num = 0.0f;
          foreach (DamageTypeEntry damageType in this.damageTypes)
            num += damageType.amount;
          entity.OnAttacked(new HitInfo((BaseEntity) ownerPlayer, entity, DamageType.Slash, num * this.npcDamageScale));
          HitInfo info = (HitInfo) Pool.Get<HitInfo>();
          info.HitPositionWorld = ((RaycastHit) ref hit).get_point();
          info.HitNormalWorld = Vector3.op_UnaryNegation(vector3);
          info.HitMaterial = entity is BaseNpc || entity is BasePlayer ? StringPool.Get("Flesh") : StringPool.Get(Object.op_Inequality((Object) hit.GetCollider().get_sharedMaterial(), (Object) null) ? hit.GetCollider().get_sharedMaterial().GetName() : "generic");
          Effect.server.ImpactEffect(info);
          // ISSUE: cast to a reference type
          Pool.Free<HitInfo>((M0&) ref info);
          flag = true;
          if (!Object.op_Inequality((Object) entity, (Object) null) || entity.ShouldBlockProjectiles())
            break;
        }
      }
      // ISSUE: cast to a reference type
      Pool.FreeList<RaycastHit>((List<M0>&) ref list);
      if (flag)
        break;
    }
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.FromOwner]
  private void CLProject(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.VerifyClientAttack(player))
    {
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }
    else
    {
      if (Object.op_Equality((Object) player, (Object) null) || player.IsHeadUnderwater())
        return;
      if (!this.canThrowAsProjectile)
      {
        AntiHack.Log(player, AntiHackType.ProjectileHack, "Not throwable (" + this.ShortPrefabName + ")");
        player.stats.combat.Log((AttackEntity) this, "not_throwable");
      }
      else
      {
        Item pickupItem = this.GetItem();
        if (pickupItem == null)
        {
          AntiHack.Log(player, AntiHackType.ProjectileHack, "Item not found (" + this.ShortPrefabName + ")");
          player.stats.combat.Log((AttackEntity) this, "item_missing");
        }
        else
        {
          ItemModProjectile component1 = (ItemModProjectile) ((Component) pickupItem.info).GetComponent<ItemModProjectile>();
          if (Object.op_Equality((Object) component1, (Object) null))
          {
            AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + this.ShortPrefabName + ")");
            player.stats.combat.Log((AttackEntity) this, "mod_missing");
          }
          else
          {
            ProjectileShoot projectileShoot = ProjectileShoot.Deserialize((Stream) msg.read);
            if (((List<ProjectileShoot.Projectile>) projectileShoot.projectiles).Count != 1)
            {
              AntiHack.Log(player, AntiHackType.ProjectileHack, "Projectile count mismatch (" + this.ShortPrefabName + ")");
              player.stats.combat.Log((AttackEntity) this, "count_mismatch");
            }
            else
            {
              player.CleanupExpiredProjectiles();
              using (List<ProjectileShoot.Projectile>.Enumerator enumerator = ((List<ProjectileShoot.Projectile>) projectileShoot.projectiles).GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  ProjectileShoot.Projectile current = enumerator.Current;
                  if (player.HasFiredProjectile((int) current.projectileID))
                  {
                    AntiHack.Log(player, AntiHackType.ProjectileHack, "Duplicate ID (" + (object) (int) current.projectileID + ")");
                    player.stats.combat.Log((AttackEntity) this, "duplicate_id");
                  }
                  else if (this.ValidateEyePos(player, (Vector3) current.startPos))
                  {
                    player.NoteFiredProjectile((int) current.projectileID, (Vector3) current.startPos, (Vector3) current.startVel, (AttackEntity) this, pickupItem.info, pickupItem);
                    Effect effect = new Effect();
                    effect.Init(Effect.Type.Projectile, (Vector3) current.startPos, (Vector3) current.startVel, msg.connection);
                    effect.scale = (__Null) 1.0;
                    effect.pooledString = component1.projectileObject.resourcePath;
                    effect.number = current.seed;
                    EffectNetwork.Send(effect);
                  }
                }
              }
              pickupItem.SetParent((ItemContainer) null);
              Interface.CallHook("OnMeleeThrown", (object) player, (object) pickupItem);
              if (!this.canAiHearIt)
                return;
              float num = 0.0f;
              if (component1.projectileObject != null)
              {
                GameObject gameObject = component1.projectileObject.Get();
                if (Object.op_Inequality((Object) gameObject, (Object) null))
                {
                  Projectile component2 = (Projectile) gameObject.GetComponent<Projectile>();
                  if (Object.op_Inequality((Object) component2, (Object) null))
                  {
                    foreach (DamageTypeEntry damageType in component2.damageTypes)
                      num += damageType.amount;
                  }
                }
              }
              if (!Object.op_Inequality((Object) player, (Object) null))
                return;
              Sense.Stimulate(new Sensation()
              {
                Type = SensationType.ThrownWeapon,
                Position = ((Component) player).get_transform().get_position(),
                Radius = 50f,
                DamagePotential = num,
                InitiatorPlayer = player,
                Initiator = (BaseEntity) player
              });
            }
          }
        }
      }
    }
  }

  [Serializable]
  public class MaterialFX
  {
    public string materialName;
    public GameObjectRef fx;
  }
}
