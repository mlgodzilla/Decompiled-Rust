// Decompiled with JetBrains decompiler
// Type: BaseProjectile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using EasyAntiCheat.Server.Cerberus;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Rust.Ai;
using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseProjectile : AttackEntity
{
  private static readonly Effect reusableInstance = new Effect();
  [Header("NPC Info")]
  public float NoiseRadius = 100f;
  [Header("Projectile")]
  public float damageScale = 1f;
  public float distanceScale = 1f;
  public float projectileVelocityScale = 1f;
  [Header("Reloading")]
  public float reloadTime = 1f;
  public bool canUnloadAmmo = true;
  [Header("Recoil")]
  public float aimSway = 3f;
  public float aimSwaySpeed = 1f;
  [Header("Aim Cone")]
  public AnimationCurve aimconeCurve = new AnimationCurve(new Keyframe[2]
  {
    new Keyframe(0.0f, 1f),
    new Keyframe(1f, 1f)
  });
  public float hipAimCone = 1.8f;
  public float aimconePenaltyRecoverTime = 0.1f;
  public float aimconePenaltyRecoverDelay = 0.1f;
  public float stancePenaltyScale = 1f;
  [Header("Iconsights")]
  public bool hasADS = true;
  [NonSerialized]
  private float nextReloadTime = float.NegativeInfinity;
  [NonSerialized]
  private float startReloadTime = float.NegativeInfinity;
  private float lastReloadTime = -10f;
  public bool automatic;
  [Header("Effects")]
  public GameObjectRef attackFX;
  public GameObjectRef silencedAttack;
  public GameObjectRef muzzleBrakeAttack;
  public Transform MuzzlePoint;
  public BaseProjectile.Magazine primaryMagazine;
  public bool fractionalReload;
  public float reloadStartDuration;
  public float reloadFractionDuration;
  public float reloadEndDuration;
  public RecoilProperties recoil;
  public float aimCone;
  public float aimconePenaltyPerShot;
  public float aimConePenaltyMax;
  public bool noAimingWhileCycling;
  public bool manualCycle;
  [NonSerialized]
  protected bool needsCycle;
  [NonSerialized]
  protected bool isCycling;
  [NonSerialized]
  public bool aiming;
  private float stancePenalty;
  private float aimconePenalty;
  protected bool reloadStarted;
  protected bool reloadFinished;
  private int fractionalInsertCounter;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BaseProjectile.OnRpcMessage", 0.1f))
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
      if (rpc == 1720368164U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - Reload "));
        using (TimeWarning.New("Reload", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsActiveItem.Test("Reload", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.Reload(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in Reload");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 240404208U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ServerFractionalReloadInsert "));
        using (TimeWarning.New("ServerFractionalReloadInsert", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsActiveItem.Test("ServerFractionalReloadInsert", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.ServerFractionalReloadInsert(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in ServerFractionalReloadInsert");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 555589155U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - StartReload "));
        using (TimeWarning.New("StartReload", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsActiveItem.Test("StartReload", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.StartReload(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in StartReload");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1918419884U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SwitchAmmoTo "));
          using (TimeWarning.New("SwitchAmmoTo", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsActiveItem.Test("SwitchAmmoTo", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SwitchAmmoTo(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SwitchAmmoTo");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public virtual float GetDamageScale(bool getMax = false)
  {
    return this.damageScale;
  }

  public virtual float GetDistanceScale(bool getMax = false)
  {
    return this.distanceScale;
  }

  public virtual float GetProjectileVelocityScale(bool getMax = false)
  {
    return this.projectileVelocityScale;
  }

  protected void StartReloadCooldown(float cooldown)
  {
    this.nextReloadTime = this.CalculateCooldownTime(this.nextReloadTime, cooldown, false);
    this.startReloadTime = this.nextReloadTime - cooldown;
  }

  protected void ResetReloadCooldown()
  {
    this.nextReloadTime = float.NegativeInfinity;
  }

  protected bool HasReloadCooldown()
  {
    return (double) Time.get_time() < (double) this.nextReloadTime;
  }

  protected float GetReloadCooldown()
  {
    return Mathf.Max(this.nextReloadTime - Time.get_time(), 0.0f);
  }

  protected float GetReloadIdle()
  {
    return Mathf.Max(Time.get_time() - this.nextReloadTime, 0.0f);
  }

  private void OnDrawGizmos()
  {
    if (!this.isClient || !Object.op_Inequality((Object) this.MuzzlePoint, (Object) null))
      return;
    Gizmos.set_color(Color.get_blue());
    Gizmos.DrawLine(this.MuzzlePoint.get_position(), Vector3.op_Addition(this.MuzzlePoint.get_position(), Vector3.op_Multiply(this.MuzzlePoint.get_forward(), 10f)));
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer))
      return;
    Gizmos.set_color(Color.get_cyan());
    Gizmos.DrawLine(this.MuzzlePoint.get_position(), Vector3.op_Addition(this.MuzzlePoint.get_position(), Vector3.op_Multiply(Quaternion.op_Multiply(ownerPlayer.eyes.rotation, Vector3.get_forward()), 10f)));
  }

  public virtual RecoilProperties GetRecoil()
  {
    return this.recoil;
  }

  public virtual void DidAttackServerside()
  {
  }

  public override bool ServerIsReloading()
  {
    return (double) Time.get_time() < (double) this.lastReloadTime + (double) this.reloadTime;
  }

  public override bool CanReload()
  {
    return this.primaryMagazine.contents < this.primaryMagazine.capacity;
  }

  public override float AmmoFraction()
  {
    return (float) this.primaryMagazine.contents / (float) this.primaryMagazine.capacity;
  }

  public override void TopUpAmmo()
  {
    this.primaryMagazine.contents = this.primaryMagazine.capacity;
  }

  public override void ServerReload()
  {
    if (this.ServerIsReloading())
      return;
    this.lastReloadTime = Time.get_time();
    this.StartAttackCooldown(this.reloadTime);
    this.GetOwnerPlayer().SignalBroadcast(BaseEntity.Signal.Reload, (Connection) null);
    this.primaryMagazine.contents = this.primaryMagazine.capacity;
  }

  public override Vector3 ModifyAIAim(Vector3 eulerInput, float swayModifier = 1f)
  {
    float num1 = Time.get_time() * (this.aimSwaySpeed * 1f + this.aiAimSwayOffset);
    float num2 = Mathf.Sin(Time.get_time() * 2f);
    float num3 = (double) num2 < 0.0 ? 1f - Mathf.Clamp(Mathf.Abs(num2) / 1f, 0.0f, 1f) : 1f;
    float num4 = (float) (((double) this.aimSway * 1.0 + (double) this.aiAimSwayOffset) * (false ? 0.600000023841858 : 1.0)) * num3 * swayModifier;
    ref __Null local1 = ref eulerInput.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local1 = ^(float&) ref local1 + (Mathf.PerlinNoise(num1, num1) - 0.5f) * num4 * Time.get_deltaTime();
    ref __Null local2 = ref eulerInput.x;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local2 = ^(float&) ref local2 + (Mathf.PerlinNoise(num1 + 0.1f, num1 + 0.2f) - 0.5f) * num4 * Time.get_deltaTime();
    return eulerInput;
  }

  public float GetAIAimcone()
  {
    NPCPlayer ownerPlayer = this.GetOwnerPlayer() as NPCPlayer;
    if (Object.op_Implicit((Object) ownerPlayer))
      return ownerPlayer.GetAimConeScale() * this.aiAimCone;
    return this.aiAimCone;
  }

  public override void ServerUse()
  {
    this.ServerUse(1f);
  }

  public override void ServerUse(float damageModifier)
  {
    if (this.isClient || this.HasAttackCooldown())
      return;
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return;
    if (this.primaryMagazine.contents <= 0)
    {
      this.SignalBroadcast(BaseEntity.Signal.DryFire, (Connection) null);
      this.StartAttackCooldown(1f);
    }
    --this.primaryMagazine.contents;
    if (this.primaryMagazine.contents < 0)
      this.primaryMagazine.contents = 0;
    if (ownerPlayer.IsNpc && (ownerPlayer.isMounted || Object.op_Inequality((Object) ownerPlayer.GetParentEntity(), (Object) null)))
    {
      NPCPlayer npcPlayer = ownerPlayer as NPCPlayer;
      if (Object.op_Inequality((Object) npcPlayer, (Object) null))
      {
        npcPlayer.SetAimDirection(npcPlayer.GetAimDirection());
      }
      else
      {
        HTNPlayer htnPlayer = ownerPlayer as HTNPlayer;
        if (Object.op_Inequality((Object) htnPlayer, (Object) null))
        {
          htnPlayer.AiDomain.ForceProjectileOrientation();
          htnPlayer.ForceOrientationTick();
        }
      }
    }
    this.StartAttackCooldown(this.repeatDelay);
    Vector3 position = ownerPlayer.eyes.position;
    ItemModProjectile component1 = (ItemModProjectile) ((Component) this.primaryMagazine.ammoType).GetComponent<ItemModProjectile>();
    this.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, (Connection) null);
    Projectile component2 = (Projectile) component1.projectileObject.Get().GetComponent<Projectile>();
    BaseEntity baseEntity = (BaseEntity) null;
    if (ownerPlayer.IsNpc && AI.npc_only_hurt_active_target_in_safezone && ownerPlayer.InSafeZone())
    {
      IAIAgent aiAgent = ownerPlayer as IAIAgent;
      if (aiAgent != null)
      {
        baseEntity = aiAgent.AttackTarget;
      }
      else
      {
        IHTNAgent htnAgent = ownerPlayer as IHTNAgent;
        if (htnAgent != null)
          baseEntity = htnAgent.MainTarget;
      }
    }
    bool flag = ownerPlayer is IHTNAgent;
    for (int index1 = 0; index1 < component1.numProjectiles; ++index1)
    {
      Vector3 vector3_1 = !flag ? AimConeUtil.GetModifiedAimConeDirection((float) ((double) component1.projectileSpread + (double) this.aimCone + (double) this.GetAIAimcone() * 1.0), ownerPlayer.eyes.BodyForward(), true) : AimConeUtil.GetModifiedAimConeDirection(component1.projectileSpread + this.aimCone, Quaternion.op_Multiply(ownerPlayer.eyes.rotation, Vector3.get_forward()), true);
      List<RaycastHit> list = (List<RaycastHit>) Pool.GetList<RaycastHit>();
      GamePhysics.TraceAll(new Ray(position, vector3_1), 0.0f, list, 300f, 1219701521, (QueryTriggerInteraction) 0);
      for (int index2 = 0; index2 < list.Count; ++index2)
      {
        BaseEntity entity = list[index2].GetEntity();
        if ((!Object.op_Inequality((Object) entity, (Object) null) || !Object.op_Equality((Object) entity, (Object) this) && !entity.EqualNetID((BaseNetworkable) this)) && (!Object.op_Inequality((Object) entity, (Object) null) || !entity.isClient))
        {
          BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
          if (Object.op_Inequality((Object) baseCombatEntity, (Object) null) && (Object.op_Equality((Object) baseEntity, (Object) null) || Object.op_Equality((Object) entity, (Object) baseEntity) || entity.EqualNetID((BaseNetworkable) baseEntity)))
          {
            float num1 = 0.0f;
            foreach (DamageTypeEntry damageType in component2.damageTypes)
              num1 += damageType.amount;
            float num2 = num1 * damageModifier;
            baseCombatEntity.Hurt(num2 * this.npcDamageScale, DamageType.Bullet, (BaseEntity) ownerPlayer, true);
          }
          if (!Object.op_Inequality((Object) entity, (Object) null) || entity.ShouldBlockProjectiles())
            break;
        }
      }
      Vector3 vector3_2 = ownerPlayer.isMounted ? Vector3.op_Multiply(vector3_1, 6f) : Vector3.get_zero();
      this.CreateProjectileEffectClientside(component1.projectileObject.resourcePath, Vector3.op_Addition(ownerPlayer.eyes.position, vector3_2), Vector3.op_Multiply(vector3_1, component1.projectileVelocity), Random.Range(1, 100), (Connection) null, this.IsSilenced(), true);
    }
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.primaryMagazine.ServerInit();
  }

  public override void ServerCommand(Item item, string command, BasePlayer player)
  {
    if (item == null || !(command == "unload_ammo") || this.HasReloadCooldown())
      return;
    this.UnloadAmmo(item, player);
  }

  public void UnloadAmmo(Item item, BasePlayer player)
  {
    BaseProjectile component = (BaseProjectile) ((Component) item.GetHeldEntity()).GetComponent<BaseProjectile>();
    if (!component.canUnloadAmmo || !Object.op_Implicit((Object) component))
      return;
    int contents = component.primaryMagazine.contents;
    if (contents <= 0)
      return;
    component.primaryMagazine.contents = 0;
    this.SendNetworkUpdateImmediate(false);
    Item obj = ItemManager.Create(component.primaryMagazine.ammoType, contents, 0UL);
    if (obj.MoveToContainer(player.inventory.containerMain, -1, true))
      return;
    obj.Drop(player.GetDropPosition(), player.GetDropVelocity(), (Quaternion) null);
  }

  public override void CollectedForCrafting(Item item, BasePlayer crafter)
  {
    if (Object.op_Equality((Object) crafter, (Object) null) || item == null)
      return;
    this.UnloadAmmo(item, crafter);
  }

  public override void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
  {
    if (Object.op_Equality((Object) crafter, (Object) null) || item == null)
      return;
    BaseProjectile component = (BaseProjectile) ((Component) item.GetHeldEntity()).GetComponent<BaseProjectile>();
    if (!Object.op_Implicit((Object) component))
      return;
    component.primaryMagazine.contents = 0;
  }

  public override void SetLightsOn(bool isOn)
  {
    base.SetLightsOn(isOn);
    if (this.children == null)
      return;
    foreach (BaseEntity baseEntity in this.children.Cast<ProjectileWeaponMod>().Where<ProjectileWeaponMod>((Func<ProjectileWeaponMod, bool>) (x =>
    {
      if (Object.op_Inequality((Object) x, (Object) null))
        return x.isLight;
      return false;
    })))
      baseEntity.SetFlag(BaseEntity.Flags.On, isOn, false, true);
  }

  public bool CanAiAttack()
  {
    return true;
  }

  public virtual float GetAimCone()
  {
    float num1 = ProjectileWeaponMod.Average((BaseEntity) this, (Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>) (x => x.sightAimCone), (Func<ProjectileWeaponMod.Modifier, float>) (y => y.scalar), 1f);
    float num2 = ProjectileWeaponMod.Sum((BaseEntity) this, (Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>) (x => x.sightAimCone), (Func<ProjectileWeaponMod.Modifier, float>) (y => y.offset), 0.0f);
    float num3 = ProjectileWeaponMod.Average((BaseEntity) this, (Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>) (x => x.hipAimCone), (Func<ProjectileWeaponMod.Modifier, float>) (y => y.scalar), 1f);
    float num4 = ProjectileWeaponMod.Sum((BaseEntity) this, (Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>) (x => x.hipAimCone), (Func<ProjectileWeaponMod.Modifier, float>) (y => y.offset), 0.0f);
    if (this.aiming || this.isServer)
      return (float) ((double) this.aimCone + (double) this.aimconePenalty + (double) this.stancePenalty * (double) this.stancePenaltyScale) * num1 + num2;
    return (float) (((double) this.aimCone + (double) this.aimconePenalty + (double) this.stancePenalty * (double) this.stancePenaltyScale) * (double) num1 + (double) num2 + (double) this.hipAimCone * (double) num3) + num4;
  }

  public float ScaleRepeatDelay(float delay)
  {
    float num1 = ProjectileWeaponMod.Average((BaseEntity) this, (Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>) (x => x.repeatDelay), (Func<ProjectileWeaponMod.Modifier, float>) (y => y.scalar), 1f);
    float num2 = ProjectileWeaponMod.Sum((BaseEntity) this, (Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>) (x => x.repeatDelay), (Func<ProjectileWeaponMod.Modifier, float>) (y => y.offset), 0.0f);
    return delay * num1 + num2;
  }

  public Projectile.Modifier GetProjectileModifier()
  {
    return new Projectile.Modifier()
    {
      damageOffset = ProjectileWeaponMod.Sum((BaseEntity) this, (Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>) (x => x.projectileDamage), (Func<ProjectileWeaponMod.Modifier, float>) (y => y.offset), 0.0f),
      damageScale = ProjectileWeaponMod.Average((BaseEntity) this, (Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>) (x => x.projectileDamage), (Func<ProjectileWeaponMod.Modifier, float>) (y => y.scalar), 1f) * this.GetDamageScale(false),
      distanceOffset = ProjectileWeaponMod.Sum((BaseEntity) this, (Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>) (x => x.projectileDistance), (Func<ProjectileWeaponMod.Modifier, float>) (y => y.offset), 0.0f),
      distanceScale = ProjectileWeaponMod.Average((BaseEntity) this, (Func<ProjectileWeaponMod, ProjectileWeaponMod.Modifier>) (x => x.projectileDistance), (Func<ProjectileWeaponMod.Modifier, float>) (y => y.scalar), 1f) * this.GetDistanceScale(false)
    };
  }

  public float GetReloadDuration()
  {
    if (this.fractionalReload)
      return (float) ((double) this.reloadStartDuration + (double) this.reloadEndDuration + (double) this.reloadFractionDuration * (double) Mathf.Min(this.primaryMagazine.capacity - this.primaryMagazine.contents, this.GetAvailableAmmo()));
    return this.reloadTime;
  }

  public int GetAvailableAmmo()
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (Object.op_Equality((Object) ownerPlayer, (Object) null))
      return 0;
    List<Item> list = (List<Item>) Pool.GetList<Item>();
    ownerPlayer.inventory.FindAmmo(list, this.primaryMagazine.definition.ammoTypes);
    int num = 0;
    if (list.Count != 0)
    {
      for (int index = 0; index < list.Count; ++index)
      {
        Item obj = list[index];
        if (Object.op_Equality((Object) obj.info, (Object) this.primaryMagazine.ammoType))
          num += obj.amount;
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Item>((List<M0>&) ref list);
    return num;
  }

  protected void ReloadMagazine(int desiredAmount = -1)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer) || Interface.CallHook("OnReloadMagazine", (object) ownerPlayer, (object) this) != null)
      return;
    this.primaryMagazine.Reload(ownerPlayer, desiredAmount);
    this.SendNetworkUpdateImmediate(false);
    ItemManager.DoRemoves();
    ownerPlayer.inventory.ServerUpdate(0.0f);
  }

  [BaseEntity.RPC_Server.IsActiveItem]
  [BaseEntity.RPC_Server]
  private void SwitchAmmoTo(BaseEntity.RPCMessage msg)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer))
      return;
    int itemID = msg.read.Int32();
    if (itemID == this.primaryMagazine.ammoType.itemid)
      return;
    ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemID);
    if (Object.op_Equality((Object) itemDefinition, (Object) null))
      return;
    ItemModProjectile component = (ItemModProjectile) ((Component) itemDefinition).GetComponent<ItemModProjectile>();
    if (!Object.op_Implicit((Object) component) || !component.IsAmmo(this.primaryMagazine.definition.ammoTypes) || Interface.CallHook("OnSwitchAmmo", (object) ownerPlayer, (object) this) != null)
      return;
    if (this.primaryMagazine.contents > 0)
    {
      ownerPlayer.GiveItem(ItemManager.CreateByItemID(this.primaryMagazine.ammoType.itemid, this.primaryMagazine.contents, 0UL), BaseEntity.GiveItemReason.Generic);
      this.primaryMagazine.contents = 0;
    }
    this.primaryMagazine.ammoType = itemDefinition;
    this.SendNetworkUpdateImmediate(false);
    ItemManager.DoRemoves();
    ownerPlayer.inventory.ServerUpdate(0.0f);
  }

  [BaseEntity.RPC_Server.IsActiveItem]
  [BaseEntity.RPC_Server]
  private void StartReload(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.VerifyClientRPC(player))
    {
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      this.reloadStarted = false;
      this.reloadFinished = false;
    }
    else
    {
      if (Interface.CallHook("OnReloadWeapon", (object) player, (object) this) != null)
        return;
      this.reloadFinished = false;
      this.reloadStarted = true;
      this.fractionalInsertCounter = 0;
      this.primaryMagazine.SwitchAmmoTypesIfNeeded(player);
      this.StartReloadCooldown(this.GetReloadDuration());
    }
  }

  [BaseEntity.RPC_Server.IsActiveItem]
  [BaseEntity.RPC_Server]
  private void ServerFractionalReloadInsert(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.VerifyClientRPC(player))
    {
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      this.reloadStarted = false;
      this.reloadFinished = false;
    }
    else if (!this.reloadStarted)
    {
      AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload request skipped (" + this.ShortPrefabName + ")");
      player.stats.combat.Log((AttackEntity) this, "reload_skip");
      this.reloadStarted = false;
      this.reloadFinished = false;
    }
    else if ((double) this.GetReloadIdle() > 3.0)
    {
      AntiHack.Log(player, AntiHackType.ReloadHack, "T+" + (object) this.GetReloadIdle() + "s (" + this.ShortPrefabName + ")");
      player.stats.combat.Log((AttackEntity) this, "reload_time");
      this.reloadStarted = false;
      this.reloadFinished = false;
    }
    else
    {
      if ((double) Time.get_time() < (double) this.startReloadTime + (double) this.reloadStartDuration)
      {
        AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload too early (" + this.ShortPrefabName + ")");
        player.stats.combat.Log((AttackEntity) this, "reload_fraction_too_early");
        this.reloadStarted = false;
        this.reloadFinished = false;
      }
      if ((double) Time.get_time() < (double) this.startReloadTime + (double) this.reloadStartDuration + (double) this.fractionalInsertCounter * (double) this.reloadFractionDuration)
      {
        AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload rate too high (" + this.ShortPrefabName + ")");
        player.stats.combat.Log((AttackEntity) this, "reload_fraction_rate");
        this.reloadStarted = false;
        this.reloadFinished = false;
      }
      else
      {
        ++this.fractionalInsertCounter;
        if (this.primaryMagazine.contents >= this.primaryMagazine.capacity)
          return;
        this.ReloadMagazine(1);
      }
    }
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsActiveItem]
  private void Reload(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.VerifyClientRPC(player))
    {
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      this.reloadStarted = false;
      this.reloadFinished = false;
    }
    else if (!this.reloadStarted)
    {
      AntiHack.Log(player, AntiHackType.ReloadHack, "Request skipped (" + this.ShortPrefabName + ")");
      player.stats.combat.Log((AttackEntity) this, "reload_skip");
      this.reloadStarted = false;
      this.reloadFinished = false;
    }
    else
    {
      if (!this.fractionalReload)
      {
        if ((double) this.GetReloadCooldown() > 1.0)
        {
          AntiHack.Log(player, AntiHackType.ReloadHack, "T-" + (object) this.GetReloadCooldown() + "s (" + this.ShortPrefabName + ")");
          player.stats.combat.Log((AttackEntity) this, "reload_time");
          this.reloadStarted = false;
          this.reloadFinished = false;
          return;
        }
        if ((double) this.GetReloadIdle() > 1.0)
        {
          AntiHack.Log(player, AntiHackType.ReloadHack, "T+" + (object) this.GetReloadIdle() + "s (" + this.ShortPrefabName + ")");
          player.stats.combat.Log((AttackEntity) this, "reload_time");
          this.reloadStarted = false;
          this.reloadFinished = false;
          return;
        }
      }
      if (this.fractionalReload)
        this.ResetReloadCooldown();
      this.reloadStarted = false;
      this.reloadFinished = true;
      if (this.fractionalReload)
        return;
      this.ReloadMagazine(-1);
    }
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.FromOwner]
  private void CLProject(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.VerifyClientAttack(player))
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    else if (this.reloadFinished && this.HasReloadCooldown())
    {
      AntiHack.Log(player, AntiHackType.ProjectileHack, "Reloading (" + this.ShortPrefabName + ")");
      player.stats.combat.Log((AttackEntity) this, "reload_cooldown");
    }
    else
    {
      this.reloadStarted = false;
      this.reloadFinished = false;
      if (this.primaryMagazine.contents <= 0 && !this.UsingInfiniteAmmoCheat)
      {
        AntiHack.Log(player, AntiHackType.ProjectileHack, "Magazine empty (" + this.ShortPrefabName + ")");
        player.stats.combat.Log((AttackEntity) this, "ammo_missing");
      }
      else
      {
        ItemDefinition ammoType = this.primaryMagazine.ammoType;
        ProjectileShoot projectileShoot = ProjectileShoot.Deserialize((Stream) msg.read);
        if (ammoType.itemid != projectileShoot.ammoType)
        {
          AntiHack.Log(player, AntiHackType.ProjectileHack, "Ammo mismatch (" + this.ShortPrefabName + ")");
          player.stats.combat.Log((AttackEntity) this, "ammo_mismatch");
        }
        else
        {
          if (!this.UsingInfiniteAmmoCheat)
            --this.primaryMagazine.contents;
          ItemModProjectile component1 = (ItemModProjectile) ((Component) ammoType).GetComponent<ItemModProjectile>();
          if (Object.op_Equality((Object) component1, (Object) null))
          {
            AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + this.ShortPrefabName + ")");
            player.stats.combat.Log((AttackEntity) this, "mod_missing");
          }
          else if (((List<ProjectileShoot.Projectile>) projectileShoot.projectiles).Count > component1.numProjectiles)
          {
            AntiHack.Log(player, AntiHackType.ProjectileHack, "Count mismatch (" + this.ShortPrefabName + ")");
            player.stats.combat.Log((AttackEntity) this, "count_mismatch");
          }
          else
          {
            Interface.CallHook("OnWeaponFired", (object) this, (object) msg.player, (object) component1, (object) projectileShoot);
            this.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, msg.connection);
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
                  player.NoteFiredProjectile((int) current.projectileID, (Vector3) current.startPos, (Vector3) current.startVel, (AttackEntity) this, ammoType, (Item) null);
                  this.CreateProjectileEffectClientside(component1.projectileObject.resourcePath, (Vector3) current.startPos, (Vector3) current.startVel, (int) current.seed, msg.connection, this.IsSilenced(), false);
                }
              }
            }
            player.stats.Add(component1.category + "_fired", ((IEnumerable<ProjectileShoot.Projectile>) projectileShoot.projectiles).Count<ProjectileShoot.Projectile>(), Stats.Steam);
            this.StartAttackCooldown(this.ScaleRepeatDelay(this.repeatDelay) + this.animationDelay);
            player.MarkHostileFor(60f);
            this.UpdateItemCondition();
            this.DidAttackServerside();
            float num1 = 0.0f;
            if (component1.projectileObject != null)
            {
              GameObject gameObject = component1.projectileObject.Get();
              if (Object.op_Inequality((Object) gameObject, (Object) null))
              {
                Projectile component2 = (Projectile) gameObject.GetComponent<Projectile>();
                if (Object.op_Inequality((Object) component2, (Object) null))
                {
                  foreach (DamageTypeEntry damageType in component2.damageTypes)
                    num1 += damageType.amount;
                }
              }
            }
            float noiseRadius = this.NoiseRadius;
            if (this.IsSilenced())
              noiseRadius *= AI.npc_gun_noise_silencer_modifier;
            Sense.Stimulate(new Sensation()
            {
              Type = SensationType.Gunshot,
              Position = ((Component) player).get_transform().get_position(),
              Radius = noiseRadius,
              DamagePotential = num1,
              InitiatorPlayer = player,
              Initiator = (BaseEntity) player
            });
            if (EACServer.playerTracker == null)
              return;
            using (TimeWarning.New("LogPlayerShooting", 0.1f))
            {
              Vector3 networkPosition = player.GetNetworkPosition();
              Quaternion networkRotation = player.GetNetworkRotation();
              Item obj = this.GetItem();
              int num2 = obj != null ? obj.info.itemid : 0;
              EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(player.net.get_connection());
              PlayerUseWeapon playerUseWeapon = (PlayerUseWeapon) null;
              playerUseWeapon.Position = (__Null) new Vector3((float) networkPosition.x, (float) networkPosition.y, (float) networkPosition.z);
              playerUseWeapon.ViewRotation = (__Null) new Quaternion((float) networkRotation.x, (float) networkRotation.y, (float) networkRotation.z, (float) networkRotation.w);
              playerUseWeapon.WeaponID = (__Null) num2;
              EACServer.playerTracker.LogPlayerUseWeapon(client, playerUseWeapon);
            }
          }
        }
      }
    }
  }

  private void CreateProjectileEffectClientside(
    string prefabName,
    Vector3 pos,
    Vector3 velocity,
    int seed,
    Connection sourceConnection,
    bool silenced = false,
    bool forceClientsideEffects = false)
  {
    Effect reusableInstance = BaseProjectile.reusableInstance;
    reusableInstance.Clear();
    reusableInstance.Init(Effect.Type.Projectile, pos, velocity, sourceConnection);
    reusableInstance.scale = silenced ? (__Null) 0.0 : (__Null) 1.0;
    if (forceClientsideEffects)
      reusableInstance.scale = (__Null) 2.0;
    reusableInstance.pooledString = prefabName;
    reusableInstance.number = (__Null) seed;
    EffectNetwork.Send(reusableInstance);
  }

  public void UpdateItemCondition()
  {
    Item ownerItem = this.GetOwnerItem();
    if (ownerItem == null)
      return;
    float barrelConditionLoss = ((ItemModProjectile) ((Component) this.primaryMagazine.ammoType).GetComponent<ItemModProjectile>()).barrelConditionLoss;
    float num = 0.25f;
    ownerItem.LoseCondition(num + barrelConditionLoss);
    if (ownerItem.contents == null || ownerItem.contents.itemList == null)
      return;
    for (int index = ownerItem.contents.itemList.Count - 1; index >= 0; --index)
      ownerItem.contents.itemList[index]?.LoseCondition(num + barrelConditionLoss);
  }

  public bool IsSilenced()
  {
    if (this.children != null)
    {
      foreach (BaseEntity child in this.children)
      {
        ProjectileWeaponMod projectileWeaponMod = child as ProjectileWeaponMod;
        if (Object.op_Inequality((Object) projectileWeaponMod, (Object) null) && projectileWeaponMod.isSilencer && !projectileWeaponMod.IsBroken())
          return true;
      }
    }
    return false;
  }

  private bool UsingInfiniteAmmoCheat
  {
    get
    {
      return false;
    }
  }

  public override bool CanUseNetworkCache(Connection sendingTo)
  {
    Connection ownerConnection = this.GetOwnerConnection();
    if (sendingTo == null || ownerConnection == null)
      return true;
    return sendingTo != ownerConnection;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.baseProjectile = (__Null) Pool.Get<BaseProjectile>();
    if (!info.forDisk && !info.SendingTo(this.GetOwnerConnection()) && !this.ForceSendMagazine())
      return;
    ((BaseProjectile) info.msg.baseProjectile).primaryMagazine = (__Null) this.primaryMagazine.Save();
  }

  public virtual bool ForceSendMagazine()
  {
    return false;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.baseProjectile == null || ((BaseProjectile) info.msg.baseProjectile).primaryMagazine == null)
      return;
    this.primaryMagazine.Load((ProtoBuf.Magazine) ((BaseProjectile) info.msg.baseProjectile).primaryMagazine);
  }

  [Serializable]
  public class Magazine
  {
    public BaseProjectile.Magazine.Definition definition;
    public int capacity;
    public int contents;
    [ItemSelector(ItemCategory.All)]
    public ItemDefinition ammoType;

    public void ServerInit()
    {
      if (this.definition.builtInSize <= 0)
        return;
      this.capacity = this.definition.builtInSize;
    }

    public ProtoBuf.Magazine Save()
    {
      ProtoBuf.Magazine magazine = (ProtoBuf.Magazine) Pool.Get<ProtoBuf.Magazine>();
      if (Object.op_Equality((Object) this.ammoType, (Object) null))
      {
        magazine.capacity = (__Null) this.capacity;
        magazine.contents = (__Null) 0;
        magazine.ammoType = (__Null) 0;
      }
      else
      {
        magazine.capacity = (__Null) this.capacity;
        magazine.contents = (__Null) this.contents;
        magazine.ammoType = (__Null) this.ammoType.itemid;
      }
      return magazine;
    }

    public void Load(ProtoBuf.Magazine mag)
    {
      this.contents = (int) mag.contents;
      this.capacity = (int) mag.capacity;
      this.ammoType = ItemManager.FindItemDefinition((int) mag.ammoType);
    }

    public bool CanReload(BasePlayer owner)
    {
      if (this.contents >= this.capacity)
        return false;
      return owner.inventory.HasAmmo(this.definition.ammoTypes);
    }

    public bool CanAiReload(BasePlayer owner)
    {
      return this.contents < this.capacity;
    }

    public void SwitchAmmoTypesIfNeeded(BasePlayer owner)
    {
      if (owner.inventory.FindItemIDs(this.ammoType.itemid).ToList<Item>().Count != 0)
        return;
      List<Item> list1 = new List<Item>();
      owner.inventory.FindAmmo(list1, this.definition.ammoTypes);
      if (list1.Count == 0)
        return;
      List<Item> list2 = owner.inventory.FindItemIDs(list1[0].info.itemid).ToList<Item>();
      if (list2 == null || list2.Count == 0)
        return;
      if (this.contents > 0)
      {
        owner.GiveItem(ItemManager.CreateByItemID(this.ammoType.itemid, this.contents, 0UL), BaseEntity.GiveItemReason.Generic);
        this.contents = 0;
      }
      this.ammoType = list2[0].info;
    }

    public bool Reload(BasePlayer owner, int desiredAmount = -1)
    {
      List<Item> list1 = owner.inventory.FindItemIDs(this.ammoType.itemid).ToList<Item>();
      if (list1.Count == 0)
      {
        List<Item> list2 = new List<Item>();
        owner.inventory.FindAmmo(list2, this.definition.ammoTypes);
        if (list2.Count == 0)
          return false;
        list1 = owner.inventory.FindItemIDs(list2[0].info.itemid).ToList<Item>();
        if (list1 == null || list1.Count == 0)
          return false;
        if (this.contents > 0)
        {
          owner.GiveItem(ItemManager.CreateByItemID(this.ammoType.itemid, this.contents, 0UL), BaseEntity.GiveItemReason.Generic);
          this.contents = 0;
        }
        this.ammoType = list1[0].info;
      }
      int num = desiredAmount;
      if (num == -1)
        num = this.capacity - this.contents;
      foreach (Item obj in list1)
      {
        int amount = obj.amount;
        int amountToConsume = Mathf.Min(num, obj.amount);
        obj.UseItem(amountToConsume);
        this.contents += amountToConsume;
        num -= amountToConsume;
        if (num <= 0)
          break;
      }
      return false;
    }

    [Serializable]
    public struct Definition
    {
      [Tooltip("Set to 0 to not use inbuilt mag")]
      public int builtInSize;
      [Tooltip("If using inbuilt mag, will accept these types of ammo")]
      [InspectorFlags]
      public AmmoTypes ammoTypes;
    }
  }
}
