// Decompiled with JetBrains decompiler
// Type: BaseCombatEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseCombatEntity : BaseEntity
{
  public bool ShowHealthInfo = true;
  public bool sendsMeleeHitNotification = true;
  public bool markAttackerHostile = true;
  public float _maxHealth = 100f;
  public float lastAttackedTime = float.NegativeInfinity;
  public float lastDealtDamageTime = float.NegativeInfinity;
  [NonSerialized]
  public bool ResetLifeStateOnSpawn = true;
  [Header("BaseCombatEntity")]
  public SkeletonProperties skeletonProperties;
  public ProtectionProperties baseProtection;
  public float startHealth;
  public BaseCombatEntity.Pickup pickup;
  public BaseCombatEntity.Repair repair;
  public BaseCombatEntity.LifeState lifestate;
  public bool sendsHitNotification;
  public float _health;
  private int lastNotifyFrame;
  private const float MAX_HEALTH_REPAIR = 50f;
  [NonSerialized]
  public DamageType lastDamage;
  [NonSerialized]
  public BaseEntity lastAttacker;
  [NonSerialized]
  public Collider _collider;
  public DirectionProperties[] propDirection;
  public float unHostileTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BaseCombatEntity.OnRpcMessage", 0.1f))
    {
      if (rpc == 1191093595U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_PickupStart "));
          using (TimeWarning.New("RPC_PickupStart", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_PickupStart", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_PickupStart(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_PickupStart");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public virtual bool IsDead()
  {
    return this.lifestate == BaseCombatEntity.LifeState.Dead;
  }

  public virtual bool IsAlive()
  {
    return this.lifestate == BaseCombatEntity.LifeState.Alive;
  }

  public Vector3 LastAttackedDir { get; set; }

  public float SecondsSinceAttacked
  {
    get
    {
      return Time.get_time() - this.lastAttackedTime;
    }
  }

  public float SecondsSinceDealtDamage
  {
    get
    {
      return Time.get_time() - this.lastDealtDamageTime;
    }
  }

  public float healthFraction
  {
    get
    {
      return this.Health() / this.MaxHealth();
    }
    set
    {
      this.health = this.MaxHealth() * value;
    }
  }

  public override void ResetState()
  {
    base.ResetState();
    this._health = this._maxHealth;
    this.lastAttackedTime = float.NegativeInfinity;
    this.lastDealtDamageTime = float.NegativeInfinity;
  }

  public override void DestroyShared()
  {
    base.DestroyShared();
    if (!this.isServer)
      return;
    this.UpdateSurroundings();
  }

  public virtual float GetThreatLevel()
  {
    return 0.0f;
  }

  public override float PenetrationResistance(HitInfo info)
  {
    if (!Object.op_Implicit((Object) this.baseProtection))
      return 1f;
    return this.baseProtection.density;
  }

  public virtual void ScaleDamage(HitInfo info)
  {
    if (!info.UseProtection || !Object.op_Inequality((Object) this.baseProtection, (Object) null))
      return;
    this.baseProtection.Scale(info.damageTypes, 1f);
  }

  public HitArea SkeletonLookup(uint boneID)
  {
    if (Object.op_Equality((Object) this.skeletonProperties, (Object) null))
      return (HitArea) -1;
    SkeletonProperties.BoneProperty bone = this.skeletonProperties.FindBone(boneID);
    if (bone == null)
      return (HitArea) -1;
    return bone.area;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.baseCombat = (__Null) Pool.Get<BaseCombat>();
    ((BaseCombat) info.msg.baseCombat).state = (__Null) this.lifestate;
    ((BaseCombat) info.msg.baseCombat).health = (__Null) (double) this._health;
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    if ((double) this.Health() > (double) this.MaxHealth())
      this.health = this.MaxHealth();
    if (!float.IsNaN(this.Health()))
      return;
    this.health = this.MaxHealth();
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    if (this.isServer)
      this.lifestate = BaseCombatEntity.LifeState.Alive;
    if (info.msg.baseCombat != null)
    {
      this.lifestate = (BaseCombatEntity.LifeState) ((BaseCombat) info.msg.baseCombat).state;
      this._health = (float) ((BaseCombat) info.msg.baseCombat).health;
    }
    base.Load(info);
  }

  public float health
  {
    get
    {
      return this._health;
    }
    set
    {
      float health = this._health;
      this._health = Mathf.Clamp(value, 0.0f, this.MaxHealth());
      if (!this.isServer || (double) this._health == (double) health)
        return;
      this.OnHealthChanged(health, this._health);
    }
  }

  public override float Health()
  {
    return this._health;
  }

  public override float MaxHealth()
  {
    return this._maxHealth;
  }

  public virtual float StartHealth()
  {
    return this.startHealth;
  }

  public virtual float StartMaxHealth()
  {
    return this.StartHealth();
  }

  public void DoHitNotify(HitInfo info)
  {
    using (TimeWarning.New(nameof (DoHitNotify), 0.1f))
    {
      if (!this.sendsHitNotification || Object.op_Equality((Object) info.Initiator, (Object) null) || (!(info.Initiator is BasePlayer) || info.isHeadshot) || (Object.op_Equality((Object) this, (Object) info.Initiator) || Time.get_frameCount() == this.lastNotifyFrame))
        return;
      this.lastNotifyFrame = Time.get_frameCount();
      bool flag1 = info.Weapon is BaseMelee;
      if (!this.isServer || flag1 && !this.sendsMeleeHitNotification)
        return;
      bool flag2 = info.Initiator.net.get_connection() == info.Predicted;
      this.ClientRPCPlayer<bool>((Connection) null, info.Initiator as BasePlayer, "HitNotify", flag2);
    }
  }

  public override void OnAttacked(HitInfo info)
  {
    using (TimeWarning.New("BaseCombatEntity.OnAttacked", 0.1f))
    {
      if (!this.IsDead())
        this.DoHitNotify(info);
      if (this.isServer)
        this.Hurt(info);
    }
    base.OnAttacked(info);
  }

  public virtual bool CanPickup(BasePlayer player)
  {
    object obj = Interface.CallHook("CanPickupEntity", (object) player, (object) this);
    if (obj is bool)
      return (bool) obj;
    if (!this.pickup.enabled)
      return false;
    if (!this.pickup.requireBuildingPrivilege)
      return true;
    if (!player.CanBuild())
      return false;
    if (this.pickup.requireHammer)
      return player.IsHoldingEntity<Hammer>();
    return true;
  }

  public virtual void OnPickedUp(Item createdItem, BasePlayer player)
  {
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  private void RPC_PickupStart(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.CanPickup(rpc.player))
      return;
    Item createdItem = ItemManager.Create(this.pickup.itemTarget, this.pickup.itemCount, this.skinID);
    if (this.pickup.setConditionFromHealth && createdItem.hasCondition)
      createdItem.conditionNormalized = Mathf.Clamp01(this.healthFraction - this.pickup.subtractCondition);
    rpc.player.GiveItem(createdItem, BaseEntity.GiveItemReason.PickedUp);
    this.OnPickedUp(createdItem, rpc.player);
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public virtual List<ItemAmount> BuildCost()
  {
    if (Object.op_Equality((Object) this.repair.itemTarget, (Object) null))
      return (List<ItemAmount>) null;
    ItemBlueprint blueprint = ItemManager.FindBlueprint(this.repair.itemTarget);
    if (Object.op_Equality((Object) blueprint, (Object) null))
      return (List<ItemAmount>) null;
    return blueprint.ingredients;
  }

  public virtual float RepairCostFraction()
  {
    return 0.5f;
  }

  public virtual List<ItemAmount> RepairCost(float healthMissingFraction)
  {
    List<ItemAmount> itemAmountList1 = this.BuildCost();
    if (itemAmountList1 == null)
      return (List<ItemAmount>) null;
    List<ItemAmount> itemAmountList2 = new List<ItemAmount>();
    foreach (ItemAmount itemAmount in itemAmountList1)
    {
      int num = Mathf.RoundToInt(itemAmount.amount * this.RepairCostFraction() * healthMissingFraction);
      if (num > 0)
        itemAmountList2.Add(new ItemAmount(itemAmount.itemDef, (float) num));
    }
    return itemAmountList2;
  }

  public virtual void OnRepair()
  {
    Effect.server.Run("assets/bundled/prefabs/fx/build/repair.prefab", (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
  }

  public virtual void OnRepairFinished()
  {
    Effect.server.Run("assets/bundled/prefabs/fx/build/repair_full.prefab", (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
  }

  public virtual void OnRepairFailed()
  {
    Effect.server.Run("assets/bundled/prefabs/fx/build/repair_failed.prefab", (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
  }

  public virtual void DoRepair(BasePlayer player)
  {
    if (!this.repair.enabled || Interface.CallHook("OnStructureRepair", (object) this, (object) player) != null)
      return;
    if ((double) this.SecondsSinceAttacked <= 30.0)
    {
      this.OnRepairFailed();
    }
    else
    {
      float num1 = this.MaxHealth() - this.health;
      float healthMissingFraction = num1 / this.MaxHealth();
      if ((double) num1 <= 0.0 || (double) healthMissingFraction <= 0.0)
      {
        this.OnRepairFailed();
      }
      else
      {
        List<ItemAmount> source = this.RepairCost(healthMissingFraction);
        if (source == null)
          return;
        float num2 = source.Sum<ItemAmount>((Func<ItemAmount, float>) (x => x.amount));
        if ((double) num2 > 0.0)
        {
          float num3 = Mathf.Min(source.Min<ItemAmount>((Func<ItemAmount, float>) (x => Mathf.Clamp01((float) player.inventory.GetAmount(x.itemid) / x.amount))), 50f / num1);
          if ((double) num3 <= 0.0)
          {
            this.OnRepairFailed();
            return;
          }
          int num4 = 0;
          foreach (ItemAmount itemAmount in source)
          {
            int amount = Mathf.CeilToInt(num3 * itemAmount.amount);
            int num5 = player.inventory.Take((List<Item>) null, itemAmount.itemid, amount);
            if (num5 > 0)
            {
              num4 += num5;
              player.Command("note.inv", (object) itemAmount.itemid, (object) (num5 * -1));
            }
          }
          float num6 = (float) num4 / num2;
          this.health += num1 * num6;
          this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
        }
        else
        {
          this.health += num1;
          this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
        }
        if ((double) this.health >= (double) this.MaxHealth())
          this.OnRepairFinished();
        else
          this.OnRepair();
      }
    }
  }

  public virtual void InitializeHealth(float newhealth, float newmax)
  {
    this._maxHealth = newmax;
    this._health = newhealth;
    this.lifestate = BaseCombatEntity.LifeState.Alive;
  }

  public override void ServerInit()
  {
    this._collider = ((Component) this).get_transform().GetComponentInChildrenIncludeDisabled<Collider>();
    this.propDirection = PrefabAttribute.server.FindAll<DirectionProperties>(this.prefabID);
    if (this.ResetLifeStateOnSpawn)
    {
      this.InitializeHealth(this.StartHealth(), this.StartMaxHealth());
      this.lifestate = BaseCombatEntity.LifeState.Alive;
    }
    base.ServerInit();
  }

  public virtual void Hurt(float amount)
  {
    this.Hurt(Mathf.Abs(amount), DamageType.Generic, (BaseEntity) null, true);
  }

  public virtual void Hurt(float amount, DamageType type, BaseEntity attacker = null, bool useProtection = true)
  {
    using (TimeWarning.New(nameof (Hurt), 0.1f))
      this.Hurt(new HitInfo(attacker, (BaseEntity) this, type, amount, ((Component) this).get_transform().get_position())
      {
        UseProtection = useProtection
      });
  }

  public virtual void Hurt(HitInfo info)
  {
    Assert.IsTrue(this.isServer, "This should be called serverside only");
    if (this.IsDead())
      return;
    using (TimeWarning.New("Hurt( HitInfo )", 50L))
    {
      float health = this.health;
      this.ScaleDamage(info);
      if (Vector3.op_Inequality(info.PointStart, Vector3.get_zero()))
      {
        for (int index = 0; index < this.propDirection.Length; ++index)
        {
          if (!Object.op_Equality((Object) this.propDirection[index].extraProtection, (Object) null) && !this.propDirection[index].IsWeakspot(((Component) this).get_transform(), info))
            this.propDirection[index].extraProtection.Scale(info.damageTypes, 1f);
        }
      }
      info.damageTypes.Scale(DamageType.Arrow, ConVar.Server.arrowdamage);
      info.damageTypes.Scale(DamageType.Bullet, ConVar.Server.bulletdamage);
      info.damageTypes.Scale(DamageType.Slash, ConVar.Server.meleedamage);
      info.damageTypes.Scale(DamageType.Blunt, ConVar.Server.meleedamage);
      info.damageTypes.Scale(DamageType.Stab, ConVar.Server.meleedamage);
      info.damageTypes.Scale(DamageType.Bleeding, ConVar.Server.bleedingdamage);
      if (Interface.CallHook("IOnBaseCombatEntityHurt", (object) this, (object) info) != null)
        return;
      this.DebugHurt(info);
      this.health = health - info.damageTypes.Total();
      this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      if (Global.developer > 1)
        Debug.Log((object) ("[Combat]".PadRight(10) + ((Object) ((Component) this).get_gameObject()).get_name() + " hurt " + (object) info.damageTypes.GetMajorityDamageType() + "/" + (object) info.damageTypes.Total() + " - " + this.health.ToString("0") + " health left"));
      this.lastDamage = info.damageTypes.GetMajorityDamageType();
      this.lastAttacker = info.Initiator;
      if (Object.op_Inequality((Object) this.lastAttacker, (Object) null))
      {
        BaseCombatEntity lastAttacker = this.lastAttacker as BaseCombatEntity;
        if (Object.op_Inequality((Object) lastAttacker, (Object) null))
          lastAttacker.lastDealtDamageTime = Time.get_time();
      }
      BaseCombatEntity lastAttacker1 = this.lastAttacker as BaseCombatEntity;
      if (this.markAttackerHostile && Object.op_Inequality((Object) lastAttacker1, (Object) null) && Object.op_Inequality((Object) lastAttacker1, (Object) this))
        lastAttacker1.MarkHostileFor(60f);
      if (this.lastDamage != DamageType.Decay)
      {
        this.lastAttackedTime = Time.get_time();
        if (Object.op_Inequality((Object) this.lastAttacker, (Object) null))
        {
          Vector3 vector3 = Vector3.op_Subtraction(((Component) this.lastAttacker).get_transform().get_position(), ((Component) this).get_transform().get_position());
          this.LastAttackedDir = ((Vector3) ref vector3).get_normalized();
        }
      }
      if ((double) this.health <= 0.0)
        this.Die(info);
      BasePlayer initiatorPlayer = info.InitiatorPlayer;
      if (!Object.op_Implicit((Object) initiatorPlayer))
        return;
      if (this.IsDead())
        initiatorPlayer.stats.combat.Log(info, health, this.health, "killed");
      else
        initiatorPlayer.stats.combat.Log(info, health, this.health, (string) null);
    }
  }

  public bool IsHostile()
  {
    object obj = Interface.CallHook("CanEntityBeHostile", (object) this);
    if (obj is bool)
      return (bool) obj;
    return (double) this.unHostileTime > (double) Time.get_realtimeSinceStartup();
  }

  public virtual void MarkHostileFor(float duration = 60f)
  {
    if (Interface.CallHook("OnEntityMarkHostile", (object) this, (object) duration) != null)
      return;
    this.unHostileTime = Mathf.Max(this.unHostileTime, Time.get_realtimeSinceStartup() + duration);
  }

  private void DebugHurt(HitInfo info)
  {
    if (!ConVar.Vis.damage)
      return;
    if (Vector3.op_Inequality(info.PointStart, info.PointEnd))
    {
      ConsoleNetwork.BroadcastToAllClients("ddraw.arrow", (object) 60, (object) Color.get_cyan(), (object) info.PointStart, (object) info.PointEnd, (object) 0.1f);
      ConsoleNetwork.BroadcastToAllClients("ddraw.sphere", (object) 60, (object) Color.get_cyan(), (object) info.HitPositionWorld, (object) 0.01f);
    }
    string str1 = "";
    for (int index = 0; index < info.damageTypes.types.Length; ++index)
    {
      float type = info.damageTypes.types[index];
      if ((double) type != 0.0)
        str1 = str1 + " " + ((DamageType) index).ToString().PadRight(10) + type.ToString("0.00") + "\n";
    }
    string str2 = "<color=lightblue>Damage:</color>".PadRight(10) + info.damageTypes.Total().ToString("0.00") + "\n<color=lightblue>Health:</color>".PadRight(10) + this.health.ToString("0.00") + " / " + ((double) this.health - (double) info.damageTypes.Total() <= 0.0 ? (object) "<color=red>" : (object) "<color=green>") + (this.health - info.damageTypes.Total()).ToString("0.00") + "</color>" + "\n<color=lightblue>HitEnt:</color>".PadRight(10) + (object) this + "\n<color=lightblue>HitBone:</color>".PadRight(10) + info.boneName + "\n<color=lightblue>Attacker:</color>".PadRight(10) + (object) info.Initiator + "\n<color=lightblue>WeaponPrefab:</color>".PadRight(10) + (object) info.WeaponPrefab + "\n<color=lightblue>Damages:</color>\n" + str1;
    ConsoleNetwork.BroadcastToAllClients("ddraw.text", (object) 60, (object) Color.get_white(), (object) info.HitPositionWorld, (object) str2);
  }

  public virtual void ChangeHealth(float amount)
  {
    if ((double) amount == 0.0)
      return;
    if ((double) amount > 0.0)
      this.Heal(amount);
    else
      this.Hurt(Mathf.Abs(amount));
  }

  public virtual void OnHealthChanged(float oldvalue, float newvalue)
  {
  }

  public virtual void Heal(float amount)
  {
    if (Global.developer > 1)
      Debug.Log((object) ("[Combat]".PadRight(10) + ((Object) ((Component) this).get_gameObject()).get_name() + " healed"));
    this.health = Mathf.Clamp(this.health + amount, 0.0f, this.MaxHealth());
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public virtual void OnKilled(HitInfo info)
  {
    this.Kill(BaseNetworkable.DestroyMode.Gib);
  }

  public virtual void Die(HitInfo info = null)
  {
    if (this.IsDead())
      return;
    Interface.CallHook("OnEntityDeath", (object) this, (object) info);
    if (Global.developer > 1)
      Debug.Log((object) ("[Combat]".PadRight(10) + ((Object) ((Component) this).get_gameObject()).get_name() + " died"));
    this.health = 0.0f;
    this.lifestate = BaseCombatEntity.LifeState.Dead;
    using (TimeWarning.New("OnKilled", 0.1f))
      this.OnKilled(info);
  }

  public void DieInstantly()
  {
    if (this.IsDead())
      return;
    if (Global.developer > 1)
      Debug.Log((object) ("[Combat]".PadRight(10) + ((Object) ((Component) this).get_gameObject()).get_name() + " died"));
    this.health = 0.0f;
    this.lifestate = BaseCombatEntity.LifeState.Dead;
    this.OnKilled((HitInfo) null);
  }

  public void UpdateSurroundings()
  {
    StabilityEntity.UpdateSurroundingsQueue surroundingsQueue = StabilityEntity.updateSurroundingsQueue;
    OBB obb = this.WorldSpaceBounds();
    Bounds bounds = ((OBB) ref obb).ToBounds();
    surroundingsQueue.Add(bounds);
  }

  public enum LifeState
  {
    Alive,
    Dead,
  }

  [Serializable]
  public struct Pickup
  {
    public bool enabled;
    [ItemSelector(ItemCategory.All)]
    public ItemDefinition itemTarget;
    public int itemCount;
    [Tooltip("Should we set the condition of the item based on the health of the picked up entity")]
    public bool setConditionFromHealth;
    [Tooltip("How much to reduce the item condition when picking up")]
    public float subtractCondition;
    [Tooltip("Must have building access to pick up")]
    public bool requireBuildingPrivilege;
    [Tooltip("Must have hammer equipped to pick up")]
    public bool requireHammer;
    [Tooltip("Inventory Must be empty (if applicable) to be picked up")]
    public bool requireEmptyInv;
  }

  [Serializable]
  public struct Repair
  {
    public bool enabled;
    [ItemSelector(ItemCategory.All)]
    public ItemDefinition itemTarget;
  }
}
