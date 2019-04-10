// Decompiled with JetBrains decompiler
// Type: BasePlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using EasyAntiCheat.Server.Cerberus;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Facepunch.Rust;
using Network;
using Network.Visibility;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

public class BasePlayer : BaseCombatEntity
{
  public static List<BasePlayer> activePlayerList = new List<BasePlayer>();
  public static List<BasePlayer> sleepingPlayerList = new List<BasePlayer>();
  [Header("BasePlayer")]
  public GameObjectRef fallDamageEffect;
  public GameObjectRef drownEffect;
  [InspectorFlags]
  public BasePlayer.PlayerFlags playerFlags;
  [NonSerialized]
  public PlayerEyes eyes;
  [NonSerialized]
  public PlayerInventory inventory;
  [NonSerialized]
  public PlayerBlueprints blueprints;
  [NonSerialized]
  public PlayerMetabolism metabolism;
  [NonSerialized]
  public PlayerInput input;
  [NonSerialized]
  public BaseMovement movement;
  [NonSerialized]
  public BaseCollision collision;
  public PlayerBelt Belt;
  [NonSerialized]
  private Collider triggerCollider;
  [NonSerialized]
  private Rigidbody physicsRigidbody;
  [NonSerialized]
  public ulong userID;
  [NonSerialized]
  public string UserIDString;
  protected string _displayName;
  private ProtectionProperties cachedProtection;
  private const int displayNameMaxLength = 32;
  public bool clothingBlocksAiming;
  public float clothingMoveSpeedReduction;
  public float clothingWaterSpeedBonus;
  public float clothingAccuracyBonus;
  public bool equippingBlocked;
  [NonSerialized]
  public bool isInAir;
  [NonSerialized]
  public bool isOnPlayer;
  [NonSerialized]
  public float violationLevel;
  [NonSerialized]
  public float lastViolationTime;
  [NonSerialized]
  public float lastAdminCheatTime;
  [NonSerialized]
  public AntiHackType lastViolationType;
  [NonSerialized]
  public float vehiclePauseTime;
  [NonSerialized]
  public float speedhackPauseTime;
  [NonSerialized]
  public float speedhackDistance;
  [NonSerialized]
  public float flyhackPauseTime;
  [NonSerialized]
  public float flyhackDistanceVertical;
  [NonSerialized]
  public float flyhackDistanceHorizontal;
  [NonSerialized]
  public PlayerModel playerModel;
  private const float drinkRange = 1.5f;
  private const float drinkMovementSpeed = 0.1f;
  [NonSerialized]
  private BasePlayer.NetworkQueueList[] networkQueue;
  [NonSerialized]
  private BasePlayer.NetworkQueueList SnapshotQueue;
  [NonSerialized]
  protected bool lightsOn;
  public ulong currentTeam;
  [NonSerialized]
  public ModelState modelState;
  [NonSerialized]
  private ModelState modelStateTick;
  [NonSerialized]
  private bool wantsSendModelState;
  [NonSerialized]
  private float nextModelStateUpdate;
  [NonSerialized]
  private EntityRef mounted;
  private float nextSeatSwapTime;
  private Dictionary<int, BasePlayer.FiredProjectile> firedProjectiles;
  [NonSerialized]
  public PlayerStatistics stats;
  [NonSerialized]
  public uint svActiveItemID;
  [NonSerialized]
  public float NextChatTime;
  [NonSerialized]
  public float nextSuicideTime;
  public Vector3 viewAngles;
  private float lastSubscriptionTick;
  private float lastPlayerTick;
  private const float playerTickRate = 0.0625f;
  private float sleepStartTime;
  private float fallTickRate;
  private float lastFallTime;
  private float fallVelocity;
  private float cachedCraftLevel;
  private float nextCheckTime;
  [NonSerialized]
  public PlayerLifeStory lifeStory;
  [NonSerialized]
  public PlayerLifeStory previousLifeStory;
  private int SpectateOffset;
  public string spectateFilter;
  private float lastUpdateTime;
  private float cachedThreatLevel;
  public float weaponDrawnDuration;
  [NonSerialized]
  public InputState serverInput;
  [NonSerialized]
  private float lastTickTime;
  [NonSerialized]
  private float lastStallTime;
  [NonSerialized]
  private float lastInputTime;
  public PlayerTick lastReceivedTick;
  private float tickDeltaTime;
  private bool tickNeedsFinalizing;
  private Vector3 tickViewAngles;
  private TickInterpolator tickInterpolator;
  private float woundedDuration;
  private float woundedStartTime;
  private float lastWoundedTime;
  [NonSerialized]
  public IPlayer IPlayer;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BasePlayer.OnRpcMessage", 0.1f))
    {
      if (rpc == 935768323U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ClientKeepConnectionAlive "));
        using (TimeWarning.New("ClientKeepConnectionAlive", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.FromOwner.Test("ClientKeepConnectionAlive", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.ClientKeepConnectionAlive(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in ClientKeepConnectionAlive");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3782818894U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ClientLoadingComplete "));
        using (TimeWarning.New("ClientLoadingComplete", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.FromOwner.Test("ClientLoadingComplete", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.ClientLoadingComplete(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in ClientLoadingComplete");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1998170713U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - OnPlayerLanded "));
        using (TimeWarning.New("OnPlayerLanded", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.FromOwner.Test("OnPlayerLanded", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.OnPlayerLanded(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in OnPlayerLanded");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 363681694U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - OnProjectileAttack "));
        using (TimeWarning.New("OnProjectileAttack", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.FromOwner.Test("OnProjectileAttack", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.OnProjectileAttack(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in OnProjectileAttack");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1500391289U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - OnProjectileRicochet "));
        using (TimeWarning.New("OnProjectileRicochet", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.FromOwner.Test("OnProjectileRicochet", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.OnProjectileRicochet(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in OnProjectileRicochet");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2324190493U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - OnProjectileUpdate "));
        using (TimeWarning.New("OnProjectileUpdate", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.FromOwner.Test("OnProjectileUpdate", (BaseEntity) this, player))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.OnProjectileUpdate(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in OnProjectileUpdate");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3167788018U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - PerformanceReport "));
        using (TimeWarning.New("PerformanceReport", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.PerformanceReport(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in PerformanceReport");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 970468557U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_Assist "));
        using (TimeWarning.New("RPC_Assist", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Assist", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_Assist(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_Assist");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3263238541U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_KeepAlive "));
        using (TimeWarning.New("RPC_KeepAlive", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_KeepAlive", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_KeepAlive(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_KeepAlive");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3692395068U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_LootPlayer "));
        using (TimeWarning.New("RPC_LootPlayer", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_LootPlayer", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_LootPlayer(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_LootPlayer");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1539133504U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_StartClimb "));
        using (TimeWarning.New("RPC_StartClimb", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_StartClimb(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_StartClimb");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 970114602U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SV_Drink "));
          using (TimeWarning.New("SV_Drink", 0.1f))
          {
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SV_Drink(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SV_Drink");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override BasePlayer ToPlayer()
  {
    return this;
  }

  public Connection Connection
  {
    get
    {
      if (this.net != null)
        return this.net.get_connection();
      return (Connection) null;
    }
  }

  public string displayName
  {
    get
    {
      return this._displayName;
    }
    set
    {
      this._displayName = value;
    }
  }

  public override Quaternion GetNetworkRotation()
  {
    if (this.isServer)
      return Quaternion.Euler(this.viewAngles);
    return Quaternion.get_identity();
  }

  public string GetSubName(int maxlen = 32)
  {
    string str = this.displayName;
    if (str.Length > maxlen)
      str = str.Substring(0, maxlen) + "..";
    return str;
  }

  public bool CanInteract()
  {
    if (!this.IsDead() && !this.IsSleeping())
      return !this.IsWounded();
    return false;
  }

  public override float StartHealth()
  {
    return Random.Range(50f, 60f);
  }

  public override float StartMaxHealth()
  {
    return 100f;
  }

  public override float MaxHealth()
  {
    return this._maxHealth;
  }

  public override float MaxVelocity()
  {
    if (this.IsSleeping())
      return 0.0f;
    if (this.isMounted)
      return this.GetMounted().MaxVelocity();
    return this.GetMaxSpeed();
  }

  public override void InitShared()
  {
    this.Belt = new PlayerBelt(this);
    this.cachedProtection = (ProtectionProperties) ScriptableObject.CreateInstance<ProtectionProperties>();
    this.baseProtection = (ProtectionProperties) ScriptableObject.CreateInstance<ProtectionProperties>();
    this.inventory = (PlayerInventory) ((Component) this).GetComponent<PlayerInventory>();
    this.blueprints = (PlayerBlueprints) ((Component) this).GetComponent<PlayerBlueprints>();
    this.metabolism = (PlayerMetabolism) ((Component) this).GetComponent<PlayerMetabolism>();
    this.eyes = (PlayerEyes) ((Component) this).GetComponent<PlayerEyes>();
    this.input = (PlayerInput) ((Component) this).GetComponent<PlayerInput>();
    base.InitShared();
  }

  public override void DestroyShared()
  {
    Object.Destroy((Object) this.cachedProtection);
    Object.Destroy((Object) this.baseProtection);
    base.DestroyShared();
  }

  public static void ServerCycle(float deltaTime)
  {
    BasePlayer.activePlayerList.RemoveAll((Predicate<BasePlayer>) (x => Object.op_Equality((Object) x, (Object) null)));
    List<BasePlayer> basePlayerList = (List<BasePlayer>) Pool.Get<List<BasePlayer>>();
    basePlayerList.AddRange((IEnumerable<BasePlayer>) BasePlayer.activePlayerList);
    for (int index = 0; index < basePlayerList.Count; ++index)
    {
      if (!Object.op_Equality((Object) basePlayerList[index], (Object) null))
        basePlayerList[index].ServerUpdate(deltaTime);
    }
    if (ConVar.Server.idlekick > 0 && (ServerMgr.AvailableSlots <= 0 && ConVar.Server.idlekickmode == 1 || ConVar.Server.idlekickmode == 2))
    {
      for (int index = 0; index < basePlayerList.Count; ++index)
      {
        if ((double) basePlayerList[index].IdleTime >= (double) (ConVar.Server.idlekick * 60) && (!basePlayerList[index].IsAdmin || ConVar.Server.idlekickadmins != 0) && (!basePlayerList[index].IsDeveloper || ConVar.Server.idlekickadmins != 0))
          basePlayerList[index].Kick("Idle for " + (object) ConVar.Server.idlekick + " minutes");
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BasePlayer>((List<M0>&) ref basePlayerList);
  }

  public bool InSafeZone()
  {
    return (double) this.currentSafeLevel > 0.0;
  }

  public override bool OnStartBeingLooted(BasePlayer baseEntity)
  {
    if (baseEntity.InSafeZone() && (long) baseEntity.userID != (long) this.userID)
      return false;
    return base.OnStartBeingLooted(baseEntity);
  }

  public Bounds GetBounds(bool ducked)
  {
    return new Bounds(Vector3.op_Addition(((Component) this).get_transform().get_position(), this.GetOffset(ducked)), this.GetSize(ducked));
  }

  public Bounds GetBounds()
  {
    return this.GetBounds(this.modelState.get_ducked());
  }

  public Vector3 GetCenter(bool ducked)
  {
    return Vector3.op_Addition(((Component) this).get_transform().get_position(), this.GetOffset(ducked));
  }

  public Vector3 GetCenter()
  {
    return this.GetCenter(this.modelState.get_ducked());
  }

  public Vector3 GetOffset(bool ducked)
  {
    if (ducked)
      return new Vector3(0.0f, 0.55f, 0.0f);
    return new Vector3(0.0f, 0.9f, 0.0f);
  }

  public Vector3 GetOffset()
  {
    return this.GetOffset(this.modelState.get_ducked());
  }

  public Vector3 GetSize(bool ducked)
  {
    if (ducked)
      return new Vector3(1f, 1.1f, 1f);
    return new Vector3(1f, 1.8f, 1f);
  }

  public Vector3 GetSize()
  {
    return this.GetSize(this.modelState.get_ducked());
  }

  public float GetHeight(bool ducked)
  {
    return ducked ? 1.1f : 1.8f;
  }

  public float GetHeight()
  {
    return this.GetHeight(this.modelState.get_ducked());
  }

  public float GetRadius()
  {
    return 0.5f;
  }

  public float GetJumpHeight()
  {
    return 1.5f;
  }

  public float MaxDeployDistance(Item item)
  {
    return 8f;
  }

  public float GetMinSpeed()
  {
    return this.GetSpeed(0.0f, 1f);
  }

  public float GetMaxSpeed()
  {
    return this.GetSpeed(1f, 0.0f);
  }

  public float GetSpeed(float running, float ducking)
  {
    float num = 1f - this.clothingMoveSpeedReduction;
    if (this.IsSwimming())
      num += this.clothingWaterSpeedBonus;
    return Mathf.Lerp(Mathf.Lerp(2.8f, 5.5f, running), 1.7f, ducking) * num;
  }

  public override void OnAttacked(HitInfo info)
  {
    if (Interface.CallHook("IOnBasePlayerAttacked", (object) this, (object) info) != null)
      return;
    float health = this.health;
    if (this.isServer)
    {
      HitArea boneArea = info.boneArea;
      if (boneArea != (HitArea) -1)
      {
        List<Item> list = (List<Item>) Pool.GetList<Item>();
        list.AddRange((IEnumerable<Item>) this.inventory.containerWear.itemList);
        for (int index = 0; index < list.Count; ++index)
        {
          Item obj = list[index];
          if (obj != null)
          {
            ItemModWearable component = (ItemModWearable) ((Component) obj.info).GetComponent<ItemModWearable>();
            if (!Object.op_Equality((Object) component, (Object) null) && component.ProtectsArea(boneArea))
              obj.OnAttacked(info);
          }
        }
        // ISSUE: cast to a reference type
        Pool.FreeList<Item>((List<M0>&) ref list);
        this.inventory.ServerUpdate(0.0f);
      }
    }
    base.OnAttacked(info);
    if (this.isServer && this.isServer && info.hasDamage)
    {
      if (!info.damageTypes.Has(DamageType.Bleeding) && info.damageTypes.IsBleedCausing() && (!this.IsWounded() && !this.IsImmortal()))
        this.metabolism.bleeding.Add(info.damageTypes.Total() * 0.2f);
      if (this.isMounted)
        this.GetMounted().MounteeTookDamage(this, info);
      this.CheckDeathCondition(info);
      if (this.net != null && this.net.get_connection() != null)
      {
        Effect effect = new Effect();
        effect.Init(Effect.Type.Generic, ((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_forward(), (Connection) null);
        effect.pooledString = "assets/bundled/prefabs/fx/takedamage_hit.prefab";
        EffectNetwork.Send(effect, this.net.get_connection());
      }
      string str = StringPool.Get(info.HitBone);
      Vector3 vector3 = Vector3.op_Subtraction(info.PointEnd, info.PointStart);
      bool flag = (double) Vector3.Dot(((Vector3) ref vector3).get_normalized(), this.eyes.BodyForward()) > 0.400000005960464;
      if (info.isHeadshot)
      {
        if (flag)
          this.SignalBroadcast(BaseEntity.Signal.Flinch_RearHead, string.Empty, (Connection) null);
        else
          this.SignalBroadcast(BaseEntity.Signal.Flinch_Head, string.Empty, (Connection) null);
        BasePlayer initiatorPlayer = info.InitiatorPlayer;
        Effect.server.Run("assets/bundled/prefabs/fx/headshot.prefab", (BaseEntity) this, 0U, new Vector3(0.0f, 2f, 0.0f), Vector3.get_zero(), Object.op_Inequality((Object) initiatorPlayer, (Object) null) ? initiatorPlayer.net.get_connection() : (Connection) null, false);
        if (Object.op_Implicit((Object) initiatorPlayer))
          initiatorPlayer.stats.Add("headshot", 1, Stats.Steam);
      }
      else if (flag)
        this.SignalBroadcast(BaseEntity.Signal.Flinch_RearTorso, string.Empty, (Connection) null);
      else if (str == "spine" || str == "spine2")
        this.SignalBroadcast(BaseEntity.Signal.Flinch_Stomach, string.Empty, (Connection) null);
      else
        this.SignalBroadcast(BaseEntity.Signal.Flinch_Chest, string.Empty, (Connection) null);
    }
    if (this.stats == null)
      return;
    if (this.IsWounded())
      this.stats.combat.Log(info, health, this.health, "wounded");
    else if (this.IsDead())
      this.stats.combat.Log(info, health, this.health, "killed");
    else
      this.stats.combat.Log(info, health, this.health, (string) null);
  }

  public void UpdatePlayerCollider(bool state)
  {
    if (Object.op_Equality((Object) this.triggerCollider, (Object) null))
      this.triggerCollider = (Collider) ((Component) this).get_gameObject().GetComponent<Collider>();
    if (this.triggerCollider.get_enabled() != state)
      this.RemoveFromTriggers();
    this.triggerCollider.set_enabled(state);
  }

  public void UpdatePlayerRigidbody(bool state)
  {
    if (Object.op_Equality((Object) this.physicsRigidbody, (Object) null))
      this.physicsRigidbody = (Rigidbody) ((Component) this).get_gameObject().GetComponent<Rigidbody>();
    if (state)
    {
      if (!Object.op_Equality((Object) this.physicsRigidbody, (Object) null))
        return;
      this.physicsRigidbody = (Rigidbody) ((Component) this).get_gameObject().AddComponent<Rigidbody>();
      this.physicsRigidbody.set_useGravity(false);
      this.physicsRigidbody.set_isKinematic(true);
      this.physicsRigidbody.set_mass(1f);
      this.physicsRigidbody.set_interpolation((RigidbodyInterpolation) 0);
      this.physicsRigidbody.set_collisionDetectionMode((CollisionDetectionMode) 0);
    }
    else
    {
      this.RemoveFromTriggers();
      if (!Object.op_Inequality((Object) this.physicsRigidbody, (Object) null))
        return;
      GameManager.Destroy((Component) this.physicsRigidbody, 0.0f);
      this.physicsRigidbody = (Rigidbody) null;
    }
  }

  public bool IsEnsnared()
  {
    if (this.triggers == null)
      return false;
    for (int index = 0; index < this.triggers.Count; ++index)
    {
      if (this.triggers[index] is TriggerEnsnare)
        return true;
    }
    return false;
  }

  public bool IsAttacking()
  {
    HeldEntity heldEntity = this.GetHeldEntity();
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return false;
    AttackEntity attackEntity = heldEntity as AttackEntity;
    if (Object.op_Equality((Object) attackEntity, (Object) null))
      return false;
    return (double) attackEntity.NextAttackTime - (double) Time.get_time() > (double) attackEntity.repeatDelay - 1.0;
  }

  public bool CanAttack()
  {
    HeldEntity heldEntity = this.GetHeldEntity();
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return false;
    bool flag1 = this.IsSwimming();
    bool flag2 = heldEntity.CanBeUsedInWater();
    return !this.modelState.get_onLadder() && (flag1 || this.modelState.get_onground()) && ((!flag1 || flag2) && !this.IsEnsnared());
  }

  public bool OnLadder()
  {
    if (this.modelState.get_onLadder())
      return Object.op_Implicit((Object) this.FindTrigger<TriggerLadder>());
    return false;
  }

  public bool IsSwimming()
  {
    return (double) this.WaterFactor() >= 0.649999976158142;
  }

  public bool IsHeadUnderwater()
  {
    return (double) this.WaterFactor() > 0.75;
  }

  public bool IsOnGround()
  {
    return this.modelState.get_onground();
  }

  public bool IsRunning()
  {
    if (this.modelState != null)
      return this.modelState.get_sprinting();
    return false;
  }

  public bool IsDucked()
  {
    if (this.modelState != null)
      return this.modelState.get_ducked();
    return false;
  }

  public void ChatMessage(string msg)
  {
    if (!this.isServer || Interface.CallHook("OnMessagePlayer", (object) msg, (object) this) != null)
      return;
    this.SendConsoleCommand("chat.add", (object) 0, (object) msg);
  }

  public void ConsoleMessage(string msg)
  {
    if (!this.isServer)
      return;
    this.SendConsoleCommand("echo " + msg, (object[]) Array.Empty<object>());
  }

  public override float PenetrationResistance(HitInfo info)
  {
    return 100f;
  }

  public override void ScaleDamage(HitInfo info)
  {
    if (info.UseProtection)
    {
      HitArea boneArea = info.boneArea;
      if (boneArea != (HitArea) -1)
      {
        this.cachedProtection.Clear();
        this.cachedProtection.Add(this.inventory.containerWear.itemList, boneArea);
        this.cachedProtection.Multiply(DamageType.Arrow, ConVar.Server.arrowarmor);
        this.cachedProtection.Multiply(DamageType.Bullet, ConVar.Server.bulletarmor);
        this.cachedProtection.Multiply(DamageType.Slash, ConVar.Server.meleearmor);
        this.cachedProtection.Multiply(DamageType.Blunt, ConVar.Server.meleearmor);
        this.cachedProtection.Multiply(DamageType.Stab, ConVar.Server.meleearmor);
        this.cachedProtection.Multiply(DamageType.Bleeding, ConVar.Server.bleedingarmor);
        this.cachedProtection.Scale(info.damageTypes, 1f);
      }
      else
        this.baseProtection.Scale(info.damageTypes, 1f);
    }
    if (!Object.op_Implicit((Object) info.damageProperties))
      return;
    info.damageProperties.ScaleDamage(info);
  }

  private void UpdateMoveSpeedFromClothing()
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    float num3 = 0.0f;
    bool flag1 = false;
    bool flag2 = false;
    float num4 = 0.0f;
    foreach (Item obj in this.inventory.containerWear.itemList)
    {
      ItemModWearable component = (ItemModWearable) ((Component) obj.info).GetComponent<ItemModWearable>();
      if (Object.op_Implicit((Object) component))
      {
        if (component.blocksAiming)
          flag1 = true;
        if (component.blocksEquipping)
          flag2 = true;
        num4 += component.accuracyBonus;
        if (Object.op_Inequality((Object) component.movementProperties, (Object) null))
        {
          num2 += component.movementProperties.speedReduction;
          num1 = Mathf.Max(num1, component.movementProperties.minSpeedReduction);
          num3 += component.movementProperties.waterSpeedBonus;
        }
      }
    }
    this.clothingAccuracyBonus = num4;
    this.clothingMoveSpeedReduction = Mathf.Max(num2, num1);
    this.clothingBlocksAiming = flag1;
    this.clothingWaterSpeedBonus = num3;
    this.equippingBlocked = flag2;
    if (!this.isServer || !this.equippingBlocked)
      return;
    this.UpdateActiveItem(0U);
  }

  public virtual void UpdateProtectionFromClothing()
  {
    this.baseProtection.Clear();
    this.baseProtection.Add(this.inventory.containerWear.itemList, (HitArea) -1);
    float num = 0.1666667f;
    for (int index = 0; index < this.baseProtection.amounts.Length; ++index)
    {
      if (index != 17)
        this.baseProtection.amounts[index] *= num;
    }
  }

  public override string Categorize()
  {
    return "player";
  }

  public override string ToString()
  {
    if (this._name == null)
    {
      if (this.isServer)
        this._name = string.Format("{1}[{0}/{2}]", (object) (uint) (this.net != null ? (int) this.net.ID : 0), (object) this.displayName, (object) this.userID);
      else
        this._name = this.ShortPrefabName;
    }
    return this._name;
  }

  public string GetDebugStatus()
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendFormat("Entity: {0}\n", (object) ((object) this).ToString());
    stringBuilder.AppendFormat("Name: {0}\n", (object) this.displayName);
    stringBuilder.AppendFormat("SteamID: {0}\n", (object) this.userID);
    foreach (BasePlayer.PlayerFlags f in System.Enum.GetValues(typeof (BasePlayer.PlayerFlags)))
      stringBuilder.AppendFormat("{1}: {0}\n", (object) this.HasPlayerFlag(f), (object) f);
    return stringBuilder.ToString();
  }

  public override Item GetItem(uint itemId)
  {
    if (Object.op_Equality((Object) this.inventory, (Object) null))
      return (Item) null;
    return this.inventory.FindItemUID(itemId);
  }

  public override BaseEntity.TraitFlag Traits
  {
    get
    {
      return base.Traits | BaseEntity.TraitFlag.Human | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat | BaseEntity.TraitFlag.Alive;
    }
  }

  public override float WaterFactor()
  {
    if (this.isMounted)
      return this.GetMounted().WaterFactorForPlayer(this);
    return base.WaterFactor();
  }

  public override bool ShouldInheritNetworkGroup()
  {
    if (!this.IsNpc)
      return this.IsSpectating();
    return true;
  }

  public static bool AnyPlayersVisibleToEntity(
    Vector3 pos,
    float radius,
    BaseEntity source,
    Vector3 entityEyePos,
    bool ignorePlayersWithPriv = false)
  {
    List<RaycastHit> list1 = (List<RaycastHit>) Pool.GetList<RaycastHit>();
    List<BasePlayer> list2 = (List<BasePlayer>) Pool.GetList<BasePlayer>();
    Vis.Entities<BasePlayer>(pos, radius, list2, 131072, (QueryTriggerInteraction) 2);
    bool flag = false;
    foreach (BasePlayer basePlayer in list2)
    {
      if (!basePlayer.IsSleeping() && basePlayer.IsAlive() && (!basePlayer.IsBuildingAuthed() || !ignorePlayersWithPriv))
      {
        list1.Clear();
        Vector3 position = basePlayer.eyes.position;
        Vector3 vector3 = Vector3.op_Subtraction(entityEyePos, basePlayer.eyes.position);
        Vector3 normalized = ((Vector3) ref vector3).get_normalized();
        GamePhysics.TraceAll(new Ray(position, normalized), 0.0f, list1, 9f, 1218519297, (QueryTriggerInteraction) 0);
        for (int index = 0; index < list1.Count; ++index)
        {
          BaseEntity entity = list1[index].GetEntity();
          if (Object.op_Inequality((Object) entity, (Object) null) && (Object.op_Equality((Object) entity, (Object) source) || entity.EqualNetID((BaseNetworkable) source)))
          {
            flag = true;
            break;
          }
          if (!Object.op_Inequality((Object) entity, (Object) null) || entity.ShouldBlockProjectiles())
            break;
        }
        if (flag)
          break;
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<RaycastHit>((List<M0>&) ref list1);
    // ISSUE: cast to a reference type
    Pool.FreeList<BasePlayer>((List<M0>&) ref list2);
    return flag;
  }

  public bool TriggeredAntiHack(float seconds = 1f, float score = float.PositiveInfinity)
  {
    if ((double) Time.get_realtimeSinceStartup() - (double) this.lastViolationTime >= (double) seconds)
      return (double) this.violationLevel > (double) score;
    return true;
  }

  public bool UsedAdminCheat(float seconds = 1f)
  {
    return (double) Time.get_realtimeSinceStartup() - (double) this.lastAdminCheatTime < (double) seconds;
  }

  public void PauseVehicleNoClipDetection(float seconds = 1f)
  {
    this.vehiclePauseTime = Mathf.Max(this.vehiclePauseTime, seconds);
  }

  public void PauseFlyHackDetection(float seconds = 1f)
  {
    this.flyhackPauseTime = Mathf.Max(this.flyhackPauseTime, seconds);
  }

  public void PauseSpeedHackDetection(float seconds = 1f)
  {
    this.speedhackPauseTime = Mathf.Max(this.speedhackPauseTime, seconds);
  }

  public int GetAntiHackKicks()
  {
    return AntiHack.GetKickRecord(this);
  }

  public void ResetAntiHack()
  {
    this.violationLevel = 0.0f;
    this.lastViolationTime = 0.0f;
    this.speedhackPauseTime = 0.0f;
    this.speedhackDistance = 0.0f;
    this.flyhackPauseTime = 0.0f;
    this.flyhackDistanceVertical = 0.0f;
    this.flyhackDistanceHorizontal = 0.0f;
  }

  public override bool CanBeLooted(BasePlayer player)
  {
    object obj = Interface.CallHook("CanLootPlayer", (object) this, (object) player);
    if (obj is bool)
      return (bool) obj;
    if (Object.op_Equality((Object) player, (Object) this))
      return false;
    if (!this.IsWounded())
      return this.IsSleeping();
    return true;
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void RPC_LootPlayer(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!Object.op_Implicit((Object) player) || !player.CanInteract() || (!this.CanBeLooted(player) || !player.inventory.loot.StartLootingEntity((BaseEntity) this, true)))
      return;
    player.inventory.loot.AddContainer(this.inventory.containerMain);
    player.inventory.loot.AddContainer(this.inventory.containerWear);
    player.inventory.loot.AddContainer(this.inventory.containerBelt);
    player.inventory.loot.SendImmediate();
    player.ClientRPCPlayer<string>((Connection) null, player, "RPC_OpenLootPanel", "player_corpse");
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  public void RPC_Assist(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract() || Object.op_Equality((Object) msg.player, (Object) this) || !this.IsWounded())
      return;
    this.StopWounded();
    msg.player.stats.Add("wounded_assisted", 1, Stats.Steam);
    this.stats.Add("wounded_healed", 1, Stats.Steam);
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void RPC_KeepAlive(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract() || Object.op_Equality((Object) msg.player, (Object) this) || !this.IsWounded())
      return;
    this.ProlongWounding(10f);
  }

  [BaseEntity.RPC_Server]
  private void SV_Drink(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    Vector3 pos = msg.read.Vector3();
    if (Vector3Ex.IsNaNOrInfinity(pos) || !Object.op_Implicit((Object) player) || (!player.metabolism.CanConsume() || (double) Vector3.Distance(((Component) player).get_transform().get_position(), pos) > 5.0) || !WaterLevel.Test(pos))
      return;
    ItemDefinition atPoint = WaterResource.GetAtPoint(pos);
    if (Object.op_Equality((Object) atPoint, (Object) null))
      return;
    ItemModConsumable component1 = (ItemModConsumable) ((Component) atPoint).GetComponent<ItemModConsumable>();
    Item obj = ItemManager.Create(atPoint, component1.amountToConsume, 0UL);
    ItemModConsume component2 = (ItemModConsume) ((Component) obj.info).GetComponent<ItemModConsume>();
    if (component2.CanDoAction(obj, player))
      component2.DoAction(obj, player);
    obj?.Remove(0.0f);
    player.metabolism.MarkConsumption();
  }

  [BaseEntity.RPC_Server]
  public void RPC_StartClimb(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    bool flag = msg.read.Bit();
    Vector3 vector3_1 = msg.read.Vector3();
    uint uid = msg.read.UInt32();
    BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
    Vector3 vector3_2 = flag ? ((Component) baseNetworkable).get_transform().TransformPoint(vector3_1) : vector3_1;
    if (!player.isMounted || !GamePhysics.LineOfSight(player.eyes.position, vector3_2, 1218519041, 0.0f) || (!GamePhysics.LineOfSight(vector3_2, Vector3.op_Addition(vector3_2, player.eyes.offset), 1218519041, 0.0f) || AntiHack.TestNoClipping(player, vector3_2, vector3_2, true, 0.0f)))
      return;
    player.EnsureDismounted();
    ((Component) player).get_transform().set_position(vector3_2);
    M0 component = ((Component) player).GetComponent<Collider>();
    ((Collider) component).set_enabled(false);
    ((Collider) component).set_enabled(true);
    player.ForceUpdateTriggers(true, true, true);
    if (flag)
      player.ClientRPCPlayer<Vector3, uint>((Connection) null, player, "ForcePositionToParentOffset", vector3_1, uid);
    else
      player.ClientRPCPlayer<Vector3>((Connection) null, player, "ForcePositionTo", vector3_2);
  }

  public int GetQueuedUpdateCount(BasePlayer.NetworkQueue queue)
  {
    return this.networkQueue[(int) queue].Length;
  }

  public void SendSnapshots(ListHashSet<Networkable> ents)
  {
    using (TimeWarning.New(nameof (SendSnapshots), 0.1f))
    {
      int count = ents.get_Values().get_Count();
      Networkable[] buffer = ents.get_Values().get_Buffer();
      for (int index = 0; index < count; ++index)
        this.SnapshotQueue.Add(buffer[index].handler as BaseNetworkable);
    }
  }

  public void QueueUpdate(BasePlayer.NetworkQueue queue, BaseNetworkable ent)
  {
    if (!this.IsConnected)
      return;
    switch (queue)
    {
      case BasePlayer.NetworkQueue.Update:
        this.networkQueue[0].Add(ent);
        break;
      case BasePlayer.NetworkQueue.UpdateDistance:
        if (this.IsReceivingSnapshot || this.networkQueue[1].Contains(ent) || this.networkQueue[0].Contains(ent))
          break;
        BasePlayer.NetworkQueueList network = this.networkQueue[1];
        if ((double) this.Distance(ent as BaseEntity) < 20.0)
        {
          this.QueueUpdate(BasePlayer.NetworkQueue.Update, ent);
          break;
        }
        network.Add(ent);
        break;
    }
  }

  public void SendEntityUpdate()
  {
    using (TimeWarning.New(nameof (SendEntityUpdate), 0.1f))
    {
      this.SendEntityUpdates(this.SnapshotQueue);
      this.SendEntityUpdates(this.networkQueue[0]);
      this.SendEntityUpdates(this.networkQueue[1]);
    }
  }

  public void ClearEntityQueue(Group group = null)
  {
    this.SnapshotQueue.Clear(group);
    this.networkQueue[0].Clear(group);
    this.networkQueue[1].Clear(group);
  }

  private void SendEntityUpdates(BasePlayer.NetworkQueueList queue)
  {
    if (queue.queueInternal.Count == 0)
      return;
    int num1 = this.IsReceivingSnapshot ? ConVar.Server.updatebatchspawn : ConVar.Server.updatebatch;
    List<BaseNetworkable> list = (List<BaseNetworkable>) Pool.GetList<BaseNetworkable>();
    using (TimeWarning.New("SendEntityUpdates.SendEntityUpdates", 0.1f))
    {
      int num2 = 0;
      foreach (BaseNetworkable ent in queue.queueInternal)
      {
        this.SendEntitySnapshot(ent);
        list.Add(ent);
        ++num2;
        if (num2 > num1)
          break;
      }
    }
    if (num1 > queue.queueInternal.Count)
    {
      queue.queueInternal.Clear();
    }
    else
    {
      using (TimeWarning.New("SendEntityUpdates.Remove", 0.1f))
      {
        for (int index = 0; index < list.Count; ++index)
          queue.queueInternal.Remove(list[index]);
      }
    }
    if (queue.queueInternal.Count == 0 && queue.MaxLength > 2048)
    {
      queue.queueInternal.Clear();
      queue.queueInternal = new HashSet<BaseNetworkable>();
      queue.MaxLength = 0;
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BaseNetworkable>((List<M0>&) ref list);
  }

  public void SendEntitySnapshot(BaseNetworkable ent)
  {
    using (TimeWarning.New(nameof (SendEntitySnapshot), 0.1f))
    {
      if (Object.op_Equality((Object) ent, (Object) null) || ent.net == null || (!ent.ShouldNetworkTo(this) || !((Write) ((NetworkPeer) Net.sv).write).Start()))
        return;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ref __Null local = ref (^(Connection.Validation&) ref this.net.get_connection().validate).entityUpdates;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local = (int) ^(uint&) ref local + 1;
      BaseNetworkable.SaveInfo saveInfo = new BaseNetworkable.SaveInfo()
      {
        forConnection = this.net.get_connection(),
        forDisk = false
      };
      ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 5);
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ((Write) ((NetworkPeer) Net.sv).write).UInt32((uint) (^(Connection.Validation&) ref this.net.get_connection().validate).entityUpdates);
      ent.ToStreamForNetwork((Stream) ((NetworkPeer) Net.sv).write, saveInfo);
      ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo(this.net.get_connection()));
    }
  }

  public bool HasPlayerFlag(BasePlayer.PlayerFlags f)
  {
    return (this.playerFlags & f) == f;
  }

  public bool IsReceivingSnapshot
  {
    get
    {
      return this.HasPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot);
    }
  }

  public bool IsAdmin
  {
    get
    {
      return this.HasPlayerFlag(BasePlayer.PlayerFlags.IsAdmin);
    }
  }

  public bool IsDeveloper
  {
    get
    {
      return this.HasPlayerFlag(BasePlayer.PlayerFlags.IsDeveloper);
    }
  }

  public bool IsAiming
  {
    get
    {
      return this.HasPlayerFlag(BasePlayer.PlayerFlags.Aiming);
    }
  }

  public bool IsFlying
  {
    get
    {
      if (this.modelState == null)
        return false;
      return this.modelState.get_flying();
    }
  }

  public bool IsConnected
  {
    get
    {
      return this.isServer && Net.sv != null && (this.net != null && this.net.get_connection() != null);
    }
  }

  public void SetPlayerFlag(BasePlayer.PlayerFlags f, bool b)
  {
    if (b)
    {
      if (this.HasPlayerFlag(f))
        return;
      this.playerFlags |= f;
    }
    else
    {
      if (!this.HasPlayerFlag(f))
        return;
      this.playerFlags &= ~f;
    }
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void LightToggle()
  {
    this.lightsOn = !this.lightsOn;
    this.SetLightsOn(this.lightsOn);
  }

  public void SetLightsOn(bool isOn)
  {
    Item activeItem = this.GetActiveItem();
    if (activeItem != null)
    {
      BaseEntity heldEntity = activeItem.GetHeldEntity();
      if (Object.op_Inequality((Object) heldEntity, (Object) null))
      {
        HeldEntity component = (HeldEntity) ((Component) heldEntity).GetComponent<HeldEntity>();
        if (Object.op_Implicit((Object) component))
          ((Component) component).SendMessage(nameof (SetLightsOn), (object) !component.LightsOn(), (SendMessageOptions) 1);
      }
    }
    foreach (Item obj in this.inventory.containerWear.itemList)
    {
      ItemModWearable component = (ItemModWearable) ((Component) obj.info).GetComponent<ItemModWearable>();
      if (Object.op_Implicit((Object) component) && component.emissive)
      {
        obj.SetFlag(Item.Flag.IsOn, !obj.HasFlag(Item.Flag.IsOn));
        obj.MarkDirty();
      }
    }
    if (!this.isMounted)
      return;
    this.GetMounted().LightToggle(this);
  }

  public void DelayedTeamUpdate()
  {
    this.UpdateTeam(this.currentTeam);
  }

  public void TeamDeathCleanup()
  {
    if (this.currentTeam == 0UL)
      return;
    RelationshipManager.Instance.FindTeam(this.currentTeam)?.RemovePlayer(this.userID);
  }

  public void TeamUpdate()
  {
    if (!RelationshipManager.TeamsEnabled() || !this.IsConnected || this.currentTeam == 0UL)
      return;
    RelationshipManager.PlayerTeam team = RelationshipManager.Instance.FindTeam(this.currentTeam);
    if (team == null)
      return;
    using (PlayerTeam playerTeam = (PlayerTeam) Pool.Get<PlayerTeam>())
    {
      playerTeam.teamLeader = (__Null) (long) team.teamLeader;
      playerTeam.teamID = (__Null) (long) team.teamID;
      playerTeam.teamName = (__Null) team.teamName;
      playerTeam.members = (__Null) Pool.GetList<PlayerTeam.TeamMember>();
      foreach (ulong member in team.members)
      {
        BasePlayer byId = RelationshipManager.FindByID(member);
        PlayerTeam.TeamMember teamMember = (PlayerTeam.TeamMember) Pool.Get<PlayerTeam.TeamMember>();
        teamMember.displayName = Object.op_Inequality((Object) byId, (Object) null) ? (__Null) byId.displayName : (__Null) "DEAD";
        teamMember.healthFraction = Object.op_Inequality((Object) byId, (Object) null) ? (__Null) (double) byId.healthFraction : (__Null) 0.0;
        teamMember.position = Object.op_Inequality((Object) byId, (Object) null) ? (__Null) ((Component) byId).get_transform().get_position() : (__Null) Vector3.get_zero();
        teamMember.online = Object.op_Inequality((Object) byId, (Object) null) ? (__Null) (!byId.IsSleeping() ? 1 : 0) : (__Null) 0;
        teamMember.userID = (__Null) (long) member;
        ((List<PlayerTeam.TeamMember>) playerTeam.members).Add(teamMember);
      }
      this.ClientRPCPlayer<PlayerTeam>((Connection) null, this, "CLIENT_ReceiveTeamInfo", playerTeam);
    }
  }

  public void UpdateTeam(ulong newTeam)
  {
    this.currentTeam = newTeam;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    if (RelationshipManager.Instance.FindTeam(newTeam) == null)
      this.ClearTeam();
    else
      this.TeamUpdate();
  }

  public void ClearTeam()
  {
    this.currentTeam = 0UL;
    this.ClientRPCPlayer((Connection) null, this, "CLIENT_ClearTeam");
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void ClearPendingInvite()
  {
    this.ClientRPCPlayer<string, int>((Connection) null, this, "CLIENT_PendingInvite", "", 0);
  }

  public HeldEntity GetHeldEntity()
  {
    if (!this.isServer)
      return (HeldEntity) null;
    Item activeItem = this.GetActiveItem();
    if (activeItem == null)
      return (HeldEntity) null;
    return activeItem.GetHeldEntity() as HeldEntity;
  }

  public bool IsHoldingEntity<T>()
  {
    HeldEntity heldEntity = this.GetHeldEntity();
    if (Object.op_Equality((Object) heldEntity, (Object) null))
      return false;
    return heldEntity is T;
  }

  private void UpdateModelState()
  {
    if (this.IsDead() || this.IsSpectating())
      return;
    this.wantsSendModelState = true;
  }

  private void SendModelState()
  {
    if (!this.wantsSendModelState || (double) this.nextModelStateUpdate > (double) Time.get_time())
      return;
    this.wantsSendModelState = false;
    this.nextModelStateUpdate = Time.get_time() + 0.1f;
    if (this.IsDead() || this.IsSpectating())
      return;
    this.modelState.set_sleeping(this.IsSleeping());
    this.modelState.set_mounted(this.isMounted);
    this.modelState.set_relaxed(this.IsRelaxed());
    this.ClientRPC<ModelState>((Connection) null, "OnModelState", this.modelState);
  }

  public bool isMounted
  {
    get
    {
      return this.mounted.IsValid(this.isServer);
    }
  }

  public BaseMountable GetMounted()
  {
    return this.mounted.Get(this.isServer) as BaseMountable;
  }

  public BaseVehicle GetMountedVehicle()
  {
    BaseMountable mounted = this.GetMounted();
    if (Object.op_Equality((Object) mounted, (Object) null))
      return (BaseVehicle) null;
    return mounted.VehicleParent();
  }

  public void MarkSwapSeat()
  {
    this.nextSeatSwapTime = Time.get_time() + 0.75f;
  }

  public bool SwapSeatCooldown()
  {
    return (double) Time.get_time() < (double) this.nextSeatSwapTime;
  }

  public void MountObject(BaseMountable mount, int desiredSeat = 0)
  {
    this.mounted.Set((BaseEntity) mount);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void EnsureDismounted()
  {
    if (!this.isMounted)
      return;
    this.GetMounted().DismountPlayer(this, false);
  }

  public virtual void DismountObject()
  {
    this.mounted.Set((BaseEntity) null);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public bool IsSleeping()
  {
    return this.HasPlayerFlag(BasePlayer.PlayerFlags.Sleeping);
  }

  public bool IsSpectating()
  {
    return this.HasPlayerFlag(BasePlayer.PlayerFlags.Spectating);
  }

  public bool IsRelaxed()
  {
    return this.HasPlayerFlag(BasePlayer.PlayerFlags.Relaxed);
  }

  public bool IsServerFalling()
  {
    return this.HasPlayerFlag(BasePlayer.PlayerFlags.ServerFall);
  }

  public bool CanBuild()
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null))
      return true;
    return buildingPrivilege.IsAuthed(this);
  }

  public bool CanBuild(Vector3 position, Quaternion rotation, Bounds bounds)
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege(new OBB(position, rotation, bounds));
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null))
      return true;
    return buildingPrivilege.IsAuthed(this);
  }

  public bool CanBuild(OBB obb)
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege(obb);
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null))
      return true;
    return buildingPrivilege.IsAuthed(this);
  }

  public bool IsBuildingBlocked()
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null))
      return false;
    return !buildingPrivilege.IsAuthed(this);
  }

  public bool IsBuildingBlocked(Vector3 position, Quaternion rotation, Bounds bounds)
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege(new OBB(position, rotation, bounds));
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null))
      return false;
    return !buildingPrivilege.IsAuthed(this);
  }

  public bool IsBuildingBlocked(OBB obb)
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege(obb);
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null))
      return false;
    return !buildingPrivilege.IsAuthed(this);
  }

  public bool IsBuildingAuthed()
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null))
      return false;
    return buildingPrivilege.IsAuthed(this);
  }

  public bool IsBuildingAuthed(Vector3 position, Quaternion rotation, Bounds bounds)
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege(new OBB(position, rotation, bounds));
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null))
      return false;
    return buildingPrivilege.IsAuthed(this);
  }

  public bool IsBuildingAuthed(OBB obb)
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege(obb);
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null))
      return false;
    return buildingPrivilege.IsAuthed(this);
  }

  public bool CanPlaceBuildingPrivilege()
  {
    return Object.op_Equality((Object) this.GetBuildingPrivilege(), (Object) null);
  }

  public bool CanPlaceBuildingPrivilege(Vector3 position, Quaternion rotation, Bounds bounds)
  {
    return Object.op_Equality((Object) this.GetBuildingPrivilege(new OBB(position, rotation, bounds)), (Object) null);
  }

  public bool CanPlaceBuildingPrivilege(OBB obb)
  {
    return Object.op_Equality((Object) this.GetBuildingPrivilege(obb), (Object) null);
  }

  public bool IsNearEnemyBase()
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null) || buildingPrivilege.IsAuthed(this))
      return false;
    return buildingPrivilege.AnyAuthed();
  }

  public bool IsNearEnemyBase(Vector3 position, Quaternion rotation, Bounds bounds)
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege(new OBB(position, rotation, bounds));
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null) || buildingPrivilege.IsAuthed(this))
      return false;
    return buildingPrivilege.AnyAuthed();
  }

  public bool IsNearEnemyBase(OBB obb)
  {
    BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege(obb);
    if (Object.op_Equality((Object) buildingPrivilege, (Object) null) || buildingPrivilege.IsAuthed(this))
      return false;
    return buildingPrivilege.AnyAuthed();
  }

  [BaseEntity.RPC_Server.FromOwner]
  [BaseEntity.RPC_Server]
  public void OnProjectileAttack(BaseEntity.RPCMessage msg)
  {
    PlayerProjectileAttack projectileAttack1 = PlayerProjectileAttack.Deserialize((Stream) msg.read);
    if (projectileAttack1 == null)
      return;
    PlayerAttack playerAttack = (PlayerAttack) projectileAttack1.playerAttack;
    HitInfo info = new HitInfo();
    info.LoadFromAttack((Attack) playerAttack.attack, true);
    info.Initiator = (BaseEntity) this;
    info.ProjectileID = (int) playerAttack.projectileID;
    info.ProjectileDistance = (float) projectileAttack1.hitDistance;
    info.ProjectileVelocity = (Vector3) projectileAttack1.hitVelocity;
    info.Predicted = msg.connection;
    PlayerProjectileAttack projectileAttack2;
    if (info.IsNaNOrInfinity() || float.IsNaN((float) projectileAttack1.travelTime) || float.IsInfinity((float) projectileAttack1.travelTime))
    {
      AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + (object) (int) playerAttack.projectileID + ")");
      projectileAttack1.ResetToPool();
      projectileAttack2 = (PlayerProjectileAttack) null;
      this.stats.combat.Log(info, "projectile_nan");
    }
    else
    {
      BasePlayer.FiredProjectile firedProjectile;
      if (!this.firedProjectiles.TryGetValue((int) playerAttack.projectileID, out firedProjectile))
      {
        AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + (object) (int) playerAttack.projectileID + ")");
        projectileAttack1.ResetToPool();
        projectileAttack2 = (PlayerProjectileAttack) null;
        this.stats.combat.Log(info, "projectile_invalid");
      }
      else if ((double) firedProjectile.integrity <= 0.0)
      {
        AntiHack.Log(this, AntiHackType.ProjectileHack, "Integrity is zero (" + (object) (int) playerAttack.projectileID + ")");
        projectileAttack1.ResetToPool();
        projectileAttack2 = (PlayerProjectileAttack) null;
        this.stats.combat.Log(info, "projectile_integrity");
      }
      else if ((double) firedProjectile.firedTime < (double) Time.get_realtimeSinceStartup() - 8.0)
      {
        AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + (object) (int) playerAttack.projectileID + ")");
        projectileAttack1.ResetToPool();
        projectileAttack2 = (PlayerProjectileAttack) null;
        this.stats.combat.Log(info, "projectile_lifetime");
      }
      else
      {
        info.Weapon = firedProjectile.weaponSource;
        info.WeaponPrefab = (BaseEntity) firedProjectile.weaponPrefab;
        info.ProjectilePrefab = firedProjectile.projectilePrefab;
        info.damageProperties = firedProjectile.projectilePrefab.damageProperties;
        float deltaTime = 1f / 32f;
        Vector3 position = firedProjectile.position;
        Vector3 velocity = firedProjectile.velocity;
        float partialTime = firedProjectile.partialTime;
        float travelTime = Mathf.Clamp((float) projectileAttack1.travelTime - firedProjectile.travelTime, 0.0f, 8f);
        Vector3 gravity = Vector3.op_Multiply(Physics.get_gravity(), firedProjectile.projectilePrefab.gravityModifier);
        float drag = firedProjectile.projectilePrefab.drag;
        if (firedProjectile.protection > 0)
        {
          bool flag = true;
          float num1 = 1f + ConVar.AntiHack.projectile_forgiveness;
          double projectileClientframes = (double) ConVar.AntiHack.projectile_clientframes;
          float projectileServerframes = ConVar.AntiHack.projectile_serverframes;
          float num2 = (this.desyncTime + (Mathx.Increment(Time.get_realtimeSinceStartup()) - Mathx.Decrement(firedProjectile.firedTime)) + (float) (projectileClientframes / 60.0) + projectileServerframes * Mathx.Max(Time.get_deltaTime(), Time.get_smoothDeltaTime(), Time.get_fixedDeltaTime())) * num1;
          if (firedProjectile.protection >= 2 && Object.op_Inequality((Object) info.HitEntity, (Object) null))
          {
            double num3 = (double) info.HitEntity.MaxVelocity();
            Vector3 parentVelocity = info.HitEntity.GetParentVelocity();
            double magnitude = (double) ((Vector3) ref parentVelocity).get_magnitude();
            float num4 = (float) (num3 + magnitude);
            float num5 = info.HitEntity.BoundsPadding() + num2 * num4;
            float num6 = info.HitEntity.Distance(info.HitPositionWorld);
            if ((double) num6 > (double) num5)
            {
              AntiHack.Log(this, AntiHackType.ProjectileHack, "Entity too far away (" + ((Object) info.ProjectilePrefab).get_name() + " on " + (Object.op_Implicit((Object) info.HitEntity) ? info.HitEntity.ShortPrefabName : "world") + " with " + (object) num6 + "m > " + (object) num5 + "m in " + (object) num2 + "s)");
              this.stats.combat.Log(info, "projectile_distance");
              flag = false;
            }
          }
          if (firedProjectile.protection >= 1)
          {
            float magnitude = ((Vector3) ref firedProjectile.initialVelocity).get_magnitude();
            float num3 = info.ProjectilePrefab.initialDistance + num2 * magnitude;
            float num4 = Vector3.Distance(firedProjectile.initialPosition, info.HitPositionWorld);
            if ((double) num4 > (double) num3)
            {
              AntiHack.Log(this, AntiHackType.ProjectileHack, "Traveled too fast (" + ((Object) info.ProjectilePrefab).get_name() + " on " + (Object.op_Implicit((Object) info.HitEntity) ? info.HitEntity.ShortPrefabName : "world") + " with " + (object) num4 + "m > " + (object) num3 + "m in " + (object) num2 + "s)");
              this.stats.combat.Log(info, "projectile_speed");
              flag = false;
            }
          }
          if (firedProjectile.protection >= 3)
          {
            Vector3 p0 = Vector3.op_Addition(firedProjectile.position, Vector3.op_Multiply(((Vector3) ref firedProjectile.velocity).get_normalized(), 1f / 1000f));
            Vector3 pointStart = info.PointStart;
            Vector3 vector3 = Vector3.op_Addition(info.HitPositionWorld, Vector3.op_Multiply(((Vector3) ref info.HitNormalWorld).get_normalized(), 1f / 1000f));
            Vector3 p2 = info.PositionOnRay(vector3);
            int num3 = GamePhysics.LineOfSight(p0, pointStart, p2, vector3, 2162688, 0.0f) ? 1 : 0;
            if (num3 == 0)
              this.stats.Add("hit_" + (Object.op_Implicit((Object) info.HitEntity) ? info.HitEntity.Categorize() : "world") + "_indirect_los", 1, Stats.Server);
            else
              this.stats.Add("hit_" + (Object.op_Implicit((Object) info.HitEntity) ? info.HitEntity.Categorize() : "world") + "_direct_los", 1, Stats.Server);
            if (num3 == 0)
            {
              AntiHack.Log(this, AntiHackType.ProjectileHack, "Line of sight (" + ((Object) info.ProjectilePrefab).get_name() + " on " + (Object.op_Implicit((Object) info.HitEntity) ? info.HitEntity.ShortPrefabName : "world") + ") " + (object) p0 + " " + (object) pointStart + " " + (object) p2 + " " + (object) vector3);
              this.stats.combat.Log(info, "projectile_los");
              flag = false;
            }
          }
          if (firedProjectile.protection >= 4)
          {
            this.SimulateProjectile(ref position, ref velocity, ref partialTime, travelTime, deltaTime, gravity, drag);
            Vector3 vector3 = Vector3.op_Subtraction(info.HitPositionWorld, position);
            float num3 = Vector3Ex.Magnitude2D(vector3);
            float num4 = Mathf.Abs((float) vector3.y);
            if ((double) num3 > (double) ConVar.AntiHack.projectile_trajectory_horizontal)
            {
              AntiHack.Log(this, AntiHackType.ProjectileHack, "Horizontal trajectory (" + ((Object) firedProjectile.projectilePrefab).get_name() + " on " + (Object.op_Implicit((Object) info.HitEntity) ? info.HitEntity.ShortPrefabName : "world") + " with " + (object) num3 + "m > " + (object) ConVar.AntiHack.projectile_trajectory_horizontal + "m)");
              this.stats.combat.Log(info, "horizontal_trajectory");
              flag = false;
            }
            if ((double) num4 > (double) ConVar.AntiHack.projectile_trajectory_vertical)
            {
              AntiHack.Log(this, AntiHackType.ProjectileHack, "Vertical trajectory (" + ((Object) firedProjectile.projectilePrefab).get_name() + " on " + (Object.op_Implicit((Object) info.HitEntity) ? info.HitEntity.ShortPrefabName : "world") + " with " + (object) num4 + "m > " + (object) ConVar.AntiHack.projectile_trajectory_vertical + "m)");
              this.stats.combat.Log(info, "vertical_trajectory");
              flag = false;
            }
          }
          if (!flag)
          {
            AntiHack.AddViolation(this, AntiHackType.ProjectileHack, ConVar.AntiHack.projectile_penalty);
            projectileAttack1.ResetToPool();
            projectileAttack2 = (PlayerProjectileAttack) null;
            return;
          }
        }
        firedProjectile.position = info.HitPositionWorld;
        firedProjectile.velocity = (Vector3) projectileAttack1.hitVelocity;
        firedProjectile.travelTime = (float) projectileAttack1.travelTime;
        firedProjectile.partialTime = partialTime;
        info.ProjectilePrefab.CalculateDamage(info, firedProjectile.projectileModifier, firedProjectile.integrity);
        if (Object.op_Equality((Object) info.HitEntity, (Object) null) && (int) info.HitMaterial == (int) Projectile.WaterMaterialID())
          firedProjectile.integrity = Mathf.Clamp01(firedProjectile.integrity - 0.1f);
        else if ((double) info.ProjectilePrefab.penetrationPower <= 0.0 || Object.op_Equality((Object) info.HitEntity, (Object) null))
        {
          firedProjectile.integrity = 0.0f;
        }
        else
        {
          float num = info.HitEntity.PenetrationResistance(info) / info.ProjectilePrefab.penetrationPower;
          firedProjectile.integrity = Mathf.Clamp01(firedProjectile.integrity - num);
        }
        firedProjectile.itemMod.ServerProjectileHit(info);
        if (Object.op_Implicit((Object) info.HitEntity))
          this.stats.Add(firedProjectile.itemMod.category + "_hit_" + info.HitEntity.Categorize(), 1, Stats.Steam);
        if ((double) firedProjectile.integrity <= 0.0 && info.ProjectilePrefab.remainInWorld)
          this.CreateWorldProjectile(info, firedProjectile.itemDef, firedProjectile.itemMod, info.ProjectilePrefab, firedProjectile.pickupItem);
        if (Interface.CallHook("OnPlayerAttack", (object) this, (object) info) != null)
          return;
        this.firedProjectiles[(int) playerAttack.projectileID] = firedProjectile;
        if (Object.op_Implicit((Object) info.HitEntity))
          info.HitEntity.OnAttacked(info);
        Effect.server.ImpactEffect(info);
        projectileAttack1.ResetToPool();
        projectileAttack2 = (PlayerProjectileAttack) null;
      }
    }
  }

  [BaseEntity.RPC_Server.FromOwner]
  [BaseEntity.RPC_Server]
  public void OnProjectileRicochet(BaseEntity.RPCMessage msg)
  {
    PlayerProjectileRicochet projectileRicochet1 = PlayerProjectileRicochet.Deserialize((Stream) msg.read);
    if (projectileRicochet1 == null)
      return;
    PlayerProjectileRicochet projectileRicochet2;
    if (Vector3Ex.IsNaNOrInfinity((Vector3) projectileRicochet1.hitPosition) || Vector3Ex.IsNaNOrInfinity((Vector3) projectileRicochet1.inVelocity) || (Vector3Ex.IsNaNOrInfinity((Vector3) projectileRicochet1.outVelocity) || Vector3Ex.IsNaNOrInfinity((Vector3) projectileRicochet1.hitNormal)) || (float.IsNaN((float) projectileRicochet1.travelTime) || float.IsInfinity((float) projectileRicochet1.travelTime)))
    {
      AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + (object) (int) projectileRicochet1.projectileID + ")");
      projectileRicochet1.ResetToPool();
      projectileRicochet2 = (PlayerProjectileRicochet) null;
    }
    else
    {
      BasePlayer.FiredProjectile firedProjectile;
      if (!this.firedProjectiles.TryGetValue((int) projectileRicochet1.projectileID, out firedProjectile))
      {
        AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + (object) (int) projectileRicochet1.projectileID + ")");
        projectileRicochet1.ResetToPool();
        projectileRicochet2 = (PlayerProjectileRicochet) null;
      }
      else if ((double) firedProjectile.firedTime < (double) Time.get_realtimeSinceStartup() - 8.0)
      {
        AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + (object) (int) projectileRicochet1.projectileID + ")");
        projectileRicochet1.ResetToPool();
        projectileRicochet2 = (PlayerProjectileRicochet) null;
      }
      else
      {
        if (firedProjectile.protection >= 3)
        {
          Vector3 p0 = Vector3.op_Addition(firedProjectile.position, Vector3.op_Multiply(((Vector3) ref firedProjectile.velocity).get_normalized(), 1f / 1000f));
          Vector3 p1 = Vector3.op_Subtraction((Vector3) projectileRicochet1.hitPosition, Vector3.op_Multiply(((Vector3) ref projectileRicochet1.inVelocity).get_normalized(), 1f / 1000f));
          Vector3 p2 = Vector3.op_Addition((Vector3) projectileRicochet1.hitPosition, Vector3.op_Multiply(((Vector3) ref projectileRicochet1.outVelocity).get_normalized(), 1f / 1000f));
          if (!GamePhysics.LineOfSight(p0, p1, p2, 2162688, 0.0f))
          {
            AntiHack.Log(this, AntiHackType.ProjectileHack, "Line of sight (" + ((Object) firedProjectile.projectilePrefab).get_name() + " on ricochet) " + (object) p0 + " " + (object) p1);
            projectileRicochet1.ResetToPool();
            projectileRicochet2 = (PlayerProjectileRicochet) null;
            return;
          }
        }
        float deltaTime = 1f / 32f;
        Vector3 position1 = firedProjectile.position;
        Vector3 velocity = firedProjectile.velocity;
        float partialTime = firedProjectile.partialTime;
        float travelTime = Mathf.Clamp((float) projectileRicochet1.travelTime - firedProjectile.travelTime, 0.0f, 8f);
        Vector3 gravity = Vector3.op_Multiply(Physics.get_gravity(), firedProjectile.projectilePrefab.gravityModifier);
        float drag = firedProjectile.projectilePrefab.drag;
        if (firedProjectile.protection >= 4)
        {
          this.SimulateProjectile(ref position1, ref velocity, ref partialTime, travelTime, deltaTime, gravity, drag);
          Vector3 vector3 = Vector3.op_Subtraction((Vector3) projectileRicochet1.hitPosition, position1);
          float num1 = Vector3Ex.Magnitude2D(vector3);
          float num2 = Mathf.Abs((float) vector3.y);
          if ((double) num1 > (double) ConVar.AntiHack.projectile_trajectory_horizontal)
          {
            AntiHack.Log(this, AntiHackType.ProjectileHack, "Horizontal trajectory (" + ((Object) firedProjectile.projectilePrefab).get_name() + " on ricochet with " + (object) num1 + "m > " + (object) ConVar.AntiHack.projectile_trajectory_horizontal + "m)");
            projectileRicochet1.ResetToPool();
            projectileRicochet2 = (PlayerProjectileRicochet) null;
            return;
          }
          if ((double) num2 > (double) ConVar.AntiHack.projectile_trajectory_vertical)
          {
            AntiHack.Log(this, AntiHackType.ProjectileHack, "Vertical trajectory (" + ((Object) firedProjectile.projectilePrefab).get_name() + " on ricochet with " + (object) num2 + "m > " + (object) ConVar.AntiHack.projectile_trajectory_vertical + "m)");
            projectileRicochet1.ResetToPool();
            projectileRicochet2 = (PlayerProjectileRicochet) null;
            return;
          }
        }
        if (firedProjectile.protection >= 5)
        {
          Vector3 position2 = firedProjectile.position;
          Vector3 hitPosition = (Vector3) projectileRicochet1.hitPosition;
          if (!GamePhysics.CheckSphere(hitPosition, 0.01f, 1269916433, (QueryTriggerInteraction) 0))
          {
            AntiHack.Log(this, AntiHackType.ProjectileHack, "Collider (" + ((Object) firedProjectile.projectilePrefab).get_name() + " on ricochet) " + (object) position2 + " " + (object) hitPosition);
            projectileRicochet1.ResetToPool();
            projectileRicochet2 = (PlayerProjectileRicochet) null;
            return;
          }
        }
        firedProjectile.position = (Vector3) projectileRicochet1.hitPosition;
        firedProjectile.velocity = (Vector3) projectileRicochet1.outVelocity;
        firedProjectile.travelTime = (float) projectileRicochet1.travelTime;
        firedProjectile.partialTime = partialTime;
        this.firedProjectiles[(int) projectileRicochet1.projectileID] = firedProjectile;
        projectileRicochet1.ResetToPool();
        projectileRicochet2 = (PlayerProjectileRicochet) null;
      }
    }
  }

  [BaseEntity.RPC_Server.FromOwner]
  [BaseEntity.RPC_Server]
  public void OnProjectileUpdate(BaseEntity.RPCMessage msg)
  {
    PlayerProjectileUpdate projectileUpdate1 = PlayerProjectileUpdate.Deserialize((Stream) msg.read);
    if (projectileUpdate1 == null)
      return;
    PlayerProjectileUpdate projectileUpdate2;
    if (Vector3Ex.IsNaNOrInfinity((Vector3) projectileUpdate1.curPosition) || Vector3Ex.IsNaNOrInfinity((Vector3) projectileUpdate1.curVelocity) || (float.IsNaN((float) projectileUpdate1.travelTime) || float.IsInfinity((float) projectileUpdate1.travelTime)))
    {
      AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + (object) (int) projectileUpdate1.projectileID + ")");
      projectileUpdate1.ResetToPool();
      projectileUpdate2 = (PlayerProjectileUpdate) null;
    }
    else
    {
      BasePlayer.FiredProjectile firedProjectile;
      if (!this.firedProjectiles.TryGetValue((int) projectileUpdate1.projectileID, out firedProjectile))
      {
        AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + (object) (int) projectileUpdate1.projectileID + ")");
        projectileUpdate1.ResetToPool();
        projectileUpdate2 = (PlayerProjectileUpdate) null;
      }
      else if ((double) firedProjectile.firedTime < (double) Time.get_realtimeSinceStartup() - 8.0)
      {
        AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + (object) (int) projectileUpdate1.projectileID + ")");
        projectileUpdate1.ResetToPool();
        projectileUpdate2 = (PlayerProjectileUpdate) null;
      }
      else
      {
        if (firedProjectile.protection >= 3)
        {
          Vector3 p0 = Vector3.op_Addition(firedProjectile.position, Vector3.op_Multiply(((Vector3) ref firedProjectile.velocity).get_normalized(), 1f / 1000f));
          Vector3 curPosition = (Vector3) projectileUpdate1.curPosition;
          if (!GamePhysics.LineOfSight(p0, curPosition, 2162688, 0.0f))
          {
            AntiHack.Log(this, AntiHackType.ProjectileHack, "Line of sight (" + ((Object) firedProjectile.projectilePrefab).get_name() + " on update) " + (object) p0 + " " + (object) curPosition);
            projectileUpdate1.ResetToPool();
            projectileUpdate2 = (PlayerProjectileUpdate) null;
            return;
          }
        }
        float deltaTime = 1f / 32f;
        Vector3 position = firedProjectile.position;
        Vector3 velocity = firedProjectile.velocity;
        float partialTime = firedProjectile.partialTime;
        float travelTime = Mathf.Clamp((float) projectileUpdate1.travelTime - firedProjectile.travelTime, 0.0f, 8f);
        Vector3 gravity = Vector3.op_Multiply(Physics.get_gravity(), firedProjectile.projectilePrefab.gravityModifier);
        float drag = firedProjectile.projectilePrefab.drag;
        if (firedProjectile.protection >= 4)
        {
          this.SimulateProjectile(ref position, ref velocity, ref partialTime, travelTime, deltaTime, gravity, drag);
          Vector3 vector3 = Vector3.op_Subtraction((Vector3) projectileUpdate1.curPosition, position);
          float num1 = Vector3Ex.Magnitude2D(vector3);
          float num2 = Mathf.Abs((float) vector3.y);
          if ((double) num1 > (double) ConVar.AntiHack.projectile_trajectory_horizontal)
          {
            AntiHack.Log(this, AntiHackType.ProjectileHack, "Horizontal trajectory (" + ((Object) firedProjectile.projectilePrefab).get_name() + " on update with " + (object) num1 + "m > " + (object) ConVar.AntiHack.projectile_trajectory_horizontal + "m)");
            projectileUpdate1.ResetToPool();
            projectileUpdate2 = (PlayerProjectileUpdate) null;
            return;
          }
          if ((double) num2 > (double) ConVar.AntiHack.projectile_trajectory_vertical)
          {
            AntiHack.Log(this, AntiHackType.ProjectileHack, "Vertical trajectory (" + ((Object) firedProjectile.projectilePrefab).get_name() + " on update with " + (object) num2 + "m > " + (object) ConVar.AntiHack.projectile_trajectory_vertical + "m)");
            projectileUpdate1.ResetToPool();
            projectileUpdate2 = (PlayerProjectileUpdate) null;
            return;
          }
        }
        if (firedProjectile.protection >= 5)
          projectileUpdate1.curVelocity = (__Null) velocity;
        firedProjectile.position = (Vector3) projectileUpdate1.curPosition;
        firedProjectile.velocity = (Vector3) projectileUpdate1.curVelocity;
        firedProjectile.travelTime = (float) projectileUpdate1.travelTime;
        firedProjectile.partialTime = partialTime;
        this.firedProjectiles[(int) projectileUpdate1.projectileID] = firedProjectile;
        projectileUpdate1.ResetToPool();
        projectileUpdate2 = (PlayerProjectileUpdate) null;
      }
    }
  }

  private void SimulateProjectile(
    ref Vector3 position,
    ref Vector3 velocity,
    ref float partialTime,
    float travelTime,
    float deltaTime,
    Vector3 gravity,
    float drag)
  {
    if ((double) partialTime > Mathf.Epsilon)
    {
      float num = deltaTime - partialTime;
      position = Vector3.op_Addition(position, Vector3.op_Multiply(velocity, num));
      velocity = Vector3.op_Addition(velocity, Vector3.op_Multiply(gravity, deltaTime));
      velocity = Vector3.op_Subtraction(velocity, Vector3.op_Multiply(Vector3.op_Multiply(velocity, drag), deltaTime));
      travelTime -= num;
    }
    int num1 = Mathf.FloorToInt(travelTime / deltaTime);
    for (int index = 0; index < num1; ++index)
    {
      position = Vector3.op_Addition(position, Vector3.op_Multiply(velocity, deltaTime));
      velocity = Vector3.op_Addition(velocity, Vector3.op_Multiply(gravity, deltaTime));
      velocity = Vector3.op_Subtraction(velocity, Vector3.op_Multiply(Vector3.op_Multiply(velocity, drag), deltaTime));
    }
    partialTime = travelTime - deltaTime * (float) num1;
    if ((double) partialTime <= Mathf.Epsilon)
      return;
    position = Vector3.op_Addition(position, Vector3.op_Multiply(velocity, partialTime));
  }

  protected virtual void CreateWorldProjectile(
    HitInfo info,
    ItemDefinition itemDef,
    ItemModProjectile itemMod,
    Projectile projectilePrefab,
    Item recycleItem)
  {
    if (Interface.CallHook("CanCreateWorldProjectile", (object) info, (object) itemDef) != null)
      return;
    Vector3 projectileVelocity = info.ProjectileVelocity;
    Item obj = recycleItem != null ? recycleItem : ItemManager.Create(itemDef, 1, 0UL);
    if (Interface.CallHook("OnCreateWorldProjectile", (object) info, (object) obj) != null)
      return;
    if (!info.DidHit)
      obj.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3) ref projectileVelocity).get_normalized()), (BaseEntity) null, 0U).Kill(BaseNetworkable.DestroyMode.Gib);
    else if ((double) projectilePrefab.breakProbability > 0.0 && (double) Random.get_value() <= (double) projectilePrefab.breakProbability)
    {
      obj.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3) ref projectileVelocity).get_normalized()), (BaseEntity) null, 0U).Kill(BaseNetworkable.DestroyMode.Gib);
    }
    else
    {
      if ((double) projectilePrefab.conditionLoss > 0.0)
      {
        obj.LoseCondition(projectilePrefab.conditionLoss * 100f);
        if (obj.isBroken)
        {
          obj.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3) ref projectileVelocity).get_normalized()), (BaseEntity) null, 0U).Kill(BaseNetworkable.DestroyMode.Gib);
          return;
        }
      }
      if ((double) projectilePrefab.stickProbability > 0.0 && (double) Random.get_value() <= (double) projectilePrefab.stickProbability)
      {
        ((Rigidbody) (!Object.op_Equality((Object) info.HitEntity, (Object) null) ? (info.HitBone != 0U ? (Component) obj.CreateWorldObject(info.HitPositionLocal, Quaternion.LookRotation(Vector3.op_Multiply(info.HitNormalLocal, -1f)), info.HitEntity, info.HitBone) : (Component) obj.CreateWorldObject(info.HitPositionLocal, Quaternion.LookRotation(((Component) info.HitEntity).get_transform().InverseTransformDirection(((Vector3) ref projectileVelocity).get_normalized())), info.HitEntity, 0U)) : (Component) obj.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3) ref projectileVelocity).get_normalized()), (BaseEntity) null, 0U)).GetComponent<Rigidbody>()).set_isKinematic(true);
      }
      else
      {
        M0 component = ((Component) obj.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3) ref projectileVelocity).get_normalized()), (BaseEntity) null, 0U)).GetComponent<Rigidbody>();
        ((Rigidbody) component).AddForce(Vector3.op_Multiply(((Vector3) ref projectileVelocity).get_normalized(), 200f));
        ((Rigidbody) component).WakeUp();
      }
    }
  }

  public void CleanupExpiredProjectiles()
  {
    foreach (KeyValuePair<int, BasePlayer.FiredProjectile> keyValuePair in this.firedProjectiles.Where<KeyValuePair<int, BasePlayer.FiredProjectile>>((Func<KeyValuePair<int, BasePlayer.FiredProjectile>, bool>) (x => (double) x.Value.firedTime < (double) Time.get_realtimeSinceStartup() - 8.0 - 1.0)).ToList<KeyValuePair<int, BasePlayer.FiredProjectile>>())
      this.firedProjectiles.Remove(keyValuePair.Key);
  }

  public bool HasFiredProjectile(int id)
  {
    return this.firedProjectiles.ContainsKey(id);
  }

  public void NoteFiredProjectile(
    int projectileid,
    Vector3 startPos,
    Vector3 startVel,
    AttackEntity attackEnt,
    ItemDefinition firedItemDef,
    Item pickupItem = null)
  {
    BaseProjectile baseProjectile1 = attackEnt as BaseProjectile;
    ItemModProjectile component1 = (ItemModProjectile) ((Component) firedItemDef).GetComponent<ItemModProjectile>();
    Projectile component2 = (Projectile) component1.projectileObject.Get().GetComponent<Projectile>();
    int num1 = ConVar.AntiHack.projectile_protection;
    if (this.HasParent())
      num1 = Mathf.Min(num1, 3);
    if (Vector3Ex.IsNaNOrInfinity(startPos) || Vector3Ex.IsNaNOrInfinity(startVel))
    {
      AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + ((Object) component2).get_name() + ")");
      this.stats.combat.Log((AttackEntity) baseProjectile1, "projectile_nan");
    }
    else
    {
      if (num1 >= 1)
      {
        float num2 = 1f + ConVar.AntiHack.projectile_forgiveness;
        float magnitude1 = ((Vector3) ref startVel).get_magnitude();
        float maxVelocity = component1.GetMaxVelocity();
        BaseProjectile baseProjectile2 = attackEnt as BaseProjectile;
        if (Object.op_Implicit((Object) baseProjectile2))
          maxVelocity *= baseProjectile2.GetProjectileVelocityScale(true);
        double num3 = (double) maxVelocity;
        Vector3 parentVelocity = this.GetParentVelocity();
        double magnitude2 = (double) ((Vector3) ref parentVelocity).get_magnitude();
        float num4 = (float) (num3 + magnitude2) * num2;
        if ((double) magnitude1 > (double) num4)
        {
          AntiHack.Log(this, AntiHackType.ProjectileHack, "Velocity (" + ((Object) component2).get_name() + " with " + (object) magnitude1 + " > " + (object) num4 + ")");
          this.stats.combat.Log((AttackEntity) baseProjectile1, "projectile_velocity");
          return;
        }
      }
      BasePlayer.FiredProjectile firedProjectile = new BasePlayer.FiredProjectile()
      {
        itemDef = firedItemDef,
        itemMod = component1,
        projectilePrefab = component2,
        firedTime = Time.get_realtimeSinceStartup(),
        travelTime = 0.0f,
        weaponSource = attackEnt,
        weaponPrefab = Object.op_Equality((Object) attackEnt, (Object) null) ? (AttackEntity) null : (AttackEntity) GameManager.server.FindPrefab(StringPool.Get(attackEnt.prefabID)).GetComponent<AttackEntity>(),
        projectileModifier = Object.op_Equality((Object) baseProjectile1, (Object) null) ? Projectile.Modifier.Default : baseProjectile1.GetProjectileModifier(),
        pickupItem = pickupItem,
        integrity = 1f,
        position = startPos,
        velocity = startVel,
        initialPosition = startPos,
        initialVelocity = startVel,
        protection = num1
      };
      this.firedProjectiles.Add(projectileid, firedProjectile);
    }
  }

  public void ServerNoteFiredProjectile(
    int projectileid,
    Vector3 startPos,
    Vector3 startVel,
    AttackEntity attackEnt,
    ItemDefinition firedItemDef,
    Item pickupItem = null)
  {
    BaseProjectile baseProjectile = attackEnt as BaseProjectile;
    ItemModProjectile component1 = (ItemModProjectile) ((Component) firedItemDef).GetComponent<ItemModProjectile>();
    Projectile component2 = (Projectile) component1.projectileObject.Get().GetComponent<Projectile>();
    int num = 0;
    if (Vector3Ex.IsNaNOrInfinity(startPos) || Vector3Ex.IsNaNOrInfinity(startVel))
      return;
    BasePlayer.FiredProjectile firedProjectile = new BasePlayer.FiredProjectile()
    {
      itemDef = firedItemDef,
      itemMod = component1,
      projectilePrefab = component2,
      firedTime = Time.get_realtimeSinceStartup(),
      travelTime = 0.0f,
      weaponSource = attackEnt,
      weaponPrefab = Object.op_Equality((Object) attackEnt, (Object) null) ? (AttackEntity) null : (AttackEntity) GameManager.server.FindPrefab(StringPool.Get(attackEnt.prefabID)).GetComponent<AttackEntity>(),
      projectileModifier = Object.op_Equality((Object) baseProjectile, (Object) null) ? Projectile.Modifier.Default : baseProjectile.GetProjectileModifier(),
      pickupItem = pickupItem,
      integrity = 1f,
      position = startPos,
      velocity = startVel,
      initialPosition = startPos,
      initialVelocity = startVel,
      protection = num
    };
    this.firedProjectiles.Add(projectileid, firedProjectile);
  }

  public override bool CanUseNetworkCache(Connection connection)
  {
    return this.net == null || this.net.get_connection() != connection;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    bool flag = this.net != null && this.net.get_connection() == info.forConnection;
    info.msg.basePlayer = (__Null) Pool.Get<BasePlayer>();
    ((BasePlayer) info.msg.basePlayer).userid = (__Null) (long) this.userID;
    ((BasePlayer) info.msg.basePlayer).name = (__Null) this.displayName;
    ((BasePlayer) info.msg.basePlayer).playerFlags = (__Null) this.playerFlags;
    ((BasePlayer) info.msg.basePlayer).currentTeam = (__Null) (long) this.currentTeam;
    ((BasePlayer) info.msg.basePlayer).heldEntity = (__Null) (int) this.svActiveItemID;
    if (this.IsConnected && (this.IsAdmin || this.IsDeveloper))
    {
      ((BasePlayer) info.msg.basePlayer).skinCol = (__Null) (double) ((Connection.ClientInfo) this.net.get_connection().info).GetFloat("global.skincol", -1f);
      ((BasePlayer) info.msg.basePlayer).skinTex = (__Null) (double) ((Connection.ClientInfo) this.net.get_connection().info).GetFloat("global.skintex", -1f);
      ((BasePlayer) info.msg.basePlayer).skinMesh = (__Null) (double) ((Connection.ClientInfo) this.net.get_connection().info).GetFloat("global.skinmesh", -1f);
    }
    else
    {
      ((BasePlayer) info.msg.basePlayer).skinCol = (__Null) -1.0;
      ((BasePlayer) info.msg.basePlayer).skinTex = (__Null) -1.0;
      ((BasePlayer) info.msg.basePlayer).skinMesh = (__Null) -1.0;
    }
    if (info.forDisk | flag)
      ((BasePlayer) info.msg.basePlayer).metabolism = (__Null) this.metabolism.Save();
    if (!info.forDisk && !flag)
    {
      // ISSUE: variable of the null type
      __Null basePlayer1 = info.msg.basePlayer;
      ((BasePlayer) basePlayer1).playerFlags = (__Null) (((BasePlayer) basePlayer1).playerFlags & -5);
      // ISSUE: variable of the null type
      __Null basePlayer2 = info.msg.basePlayer;
      ((BasePlayer) basePlayer2).playerFlags = (__Null) (((BasePlayer) basePlayer2).playerFlags & -129);
    }
    ((BasePlayer) info.msg.basePlayer).inventory = (__Null) this.inventory.Save(info.forDisk | flag);
    this.modelState.set_sleeping(this.IsSleeping());
    this.modelState.set_relaxed(this.IsRelaxed());
    ((BasePlayer) info.msg.basePlayer).modelState = (__Null) this.modelState.Copy();
    if (!info.forDisk)
      ((BasePlayer) info.msg.basePlayer).mounted = (__Null) (int) this.mounted.uid;
    if (flag)
      ((BasePlayer) info.msg.basePlayer).persistantData = (__Null) ((ServerMgr) SingletonComponent<ServerMgr>.Instance).persistance.GetPlayerInfo(this.userID);
    if (!info.forDisk)
      return;
    ((BasePlayer) info.msg.basePlayer).currentLife = (__Null) this.lifeStory;
    ((BasePlayer) info.msg.basePlayer).previousLife = (__Null) this.previousLifeStory;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.basePlayer != null)
    {
      BasePlayer basePlayer = (BasePlayer) info.msg.basePlayer;
      this.userID = (ulong) basePlayer.userid;
      this.UserIDString = this.userID.ToString();
      if (basePlayer.name != null)
      {
        this._displayName = (string) basePlayer.name;
        if (string.IsNullOrEmpty(this._displayName.Trim()))
          this._displayName = "Blaster :D";
      }
      this.playerFlags = (BasePlayer.PlayerFlags) basePlayer.playerFlags;
      this.currentTeam = (ulong) basePlayer.currentTeam;
      if (basePlayer.metabolism != null)
        this.metabolism.Load((PlayerMetabolism) basePlayer.metabolism);
      if (basePlayer.inventory != null)
        this.inventory.Load((PlayerInventory) basePlayer.inventory);
      if (basePlayer.modelState != null)
      {
        if (this.modelState != null)
        {
          this.modelState.ResetToPool();
          this.modelState = (ModelState) null;
        }
        this.modelState = (ModelState) basePlayer.modelState;
        basePlayer.modelState = null;
      }
    }
    if (!info.fromDisk)
      return;
    this.lifeStory = (PlayerLifeStory) ((BasePlayer) info.msg.basePlayer).currentLife;
    if (this.lifeStory != null)
      this.lifeStory.ShouldPool = (__Null) 0;
    this.previousLifeStory = (PlayerLifeStory) ((BasePlayer) info.msg.basePlayer).previousLife;
    if (this.previousLifeStory != null)
      this.previousLifeStory.ShouldPool = (__Null) 0;
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Sleeping, false);
    this.StartSleeping();
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Connected, false);
    if (this.lifeStory != null || !this.IsAlive())
      return;
    this.LifeStoryStart();
  }

  public virtual BaseNpc.AiStatistics.FamilyEnum Family
  {
    get
    {
      return BaseNpc.AiStatistics.FamilyEnum.Player;
    }
  }

  protected override float PositionTickRate
  {
    get
    {
      return -1f;
    }
  }

  internal override void OnParentRemoved()
  {
    if (this.IsNpc)
      base.OnParentRemoved();
    else
      this.SetParent((BaseEntity) null, true, true);
  }

  public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
  {
    if (Object.op_Inequality((Object) oldParent, (Object) null))
      this.TransformState(((Component) oldParent).get_transform().get_localToWorldMatrix());
    if (!Object.op_Inequality((Object) newParent, (Object) null))
      return;
    this.TransformState(((Component) newParent).get_transform().get_worldToLocalMatrix());
  }

  private void TransformState(Matrix4x4 matrix)
  {
    this.tickInterpolator.TransformEntries(matrix);
    Vector3 vector3;
    ref Vector3 local = ref vector3;
    Quaternion rotation = ((Matrix4x4) ref matrix).get_rotation();
    // ISSUE: variable of the null type
    __Null y = ((Quaternion) ref rotation).get_eulerAngles().y;
    ((Vector3) ref local).\u002Ector(0.0f, (float) y, 0.0f);
    this.eyes.bodyRotation = Quaternion.op_Multiply(Quaternion.Euler(vector3), this.eyes.bodyRotation);
  }

  public bool CanSuicide()
  {
    if (this.IsAdmin || this.IsDeveloper)
      return true;
    return (double) Time.get_realtimeSinceStartup() > (double) this.nextSuicideTime;
  }

  public void MarkSuicide()
  {
    this.nextSuicideTime = Time.get_realtimeSinceStartup() + 60f;
  }

  public Item GetActiveItem()
  {
    if (this.svActiveItemID == 0U)
      return (Item) null;
    if (this.IsDead())
      return (Item) null;
    if (Object.op_Equality((Object) this.inventory, (Object) null) || this.inventory.containerBelt == null)
      return (Item) null;
    return this.inventory.containerBelt.FindItemByUID(this.svActiveItemID);
  }

  public void MovePosition(Vector3 newPos)
  {
    ((Component) this).get_transform().set_position(newPos);
    this.tickInterpolator.Reset(newPos);
    this.NetworkPositionTick();
  }

  public Vector3 estimatedVelocity { get; private set; }

  public float estimatedSpeed { get; private set; }

  public float estimatedSpeed2D { get; private set; }

  public int secondsConnected { get; private set; }

  public float desyncTime { get; private set; }

  public void OverrideViewAngles(Vector3 newAng)
  {
    this.viewAngles = newAng;
  }

  public override void ServerInit()
  {
    this.stats = new PlayerStatistics(this);
    if (this.userID == 0UL)
    {
      this.userID = (ulong) Random.Range(0, 10000000);
      this.UserIDString = this.userID.ToString();
      this.displayName = this.UserIDString;
    }
    this.UpdatePlayerCollider(true);
    this.UpdatePlayerRigidbody(!this.IsSleeping());
    base.ServerInit();
    BaseEntity.Query.Server.AddPlayer(this);
    this.inventory.ServerInit(this);
    this.metabolism.ServerInit(this);
  }

  internal override void DoServerDestroy()
  {
    base.DoServerDestroy();
    BaseEntity.Query.Server.RemovePlayer(this);
    if (Object.op_Implicit((Object) this.inventory))
      this.inventory.DoDestroy();
    BasePlayer.sleepingPlayerList.Remove(this);
  }

  protected void ServerUpdate(float deltaTime)
  {
    if (!((Network.Server) Net.sv).IsConnected())
      return;
    this.LifeStoryUpdate(deltaTime);
    this.FinalizeTick(deltaTime);
    this.desyncTime = Mathf.Max(this.timeSinceLastTick - deltaTime, 0.0f);
    if ((double) Time.get_realtimeSinceStartup() < (double) this.lastPlayerTick + 1.0 / 16.0)
      return;
    if ((double) this.lastPlayerTick < (double) Time.get_realtimeSinceStartup() - 6.25)
      this.lastPlayerTick = Time.get_realtimeSinceStartup() - Random.Range(0.0f, 1f / 16f);
    while ((double) this.lastPlayerTick < (double) Time.get_realtimeSinceStartup())
      this.lastPlayerTick += 1f / 16f;
    if (!this.IsConnected)
      return;
    this.ConnectedPlayerUpdate(1f / 16f);
  }

  private void ConnectedPlayerUpdate(float deltaTime)
  {
    if (this.IsReceivingSnapshot)
      this.net.UpdateSubscriptions(int.MaxValue, int.MaxValue);
    else if ((double) Time.get_realtimeSinceStartup() > (double) this.lastSubscriptionTick + (double) ConVar.Server.entitybatchtime && this.net.UpdateSubscriptions(ConVar.Server.entitybatchsize, ConVar.Server.entitybatchsize))
      this.lastSubscriptionTick = Time.get_realtimeSinceStartup();
    this.SendEntityUpdate();
    if (this.IsReceivingSnapshot)
    {
      if (this.SnapshotQueue.Length != 0 || !EACServer.IsAuthenticated(this.net.get_connection()))
        return;
      this.EnterGame();
    }
    else
    {
      if (this.IsAlive())
      {
        this.metabolism.ServerUpdate((BaseCombatEntity) this, deltaTime);
        if (this.InSafeZone())
        {
          float duration = 0.0f;
          HeldEntity heldEntity = this.GetHeldEntity();
          if (Object.op_Implicit((Object) heldEntity) && heldEntity.hostile)
            duration = deltaTime;
          if ((double) duration == 0.0)
            this.MarkWeaponDrawnDuration(0.0f);
          else
            this.AddWeaponDrawnDuration(duration);
          if ((double) this.weaponDrawnDuration >= 5.0)
            this.MarkHostileFor(30f);
        }
        else
          this.MarkWeaponDrawnDuration(0.0f);
        if ((double) this.timeSinceLastTick > (double) ConVar.Server.playertimeout)
        {
          this.lastTickTime = 0.0f;
          this.Kick("Unresponsive");
          return;
        }
      }
      int secondsConnected = (int) this.net.get_connection().GetSecondsConnected();
      int val = secondsConnected - this.secondsConnected;
      if (val > 0)
      {
        this.stats.Add("time", val, Stats.Server);
        this.secondsConnected = secondsConnected;
      }
      this.SendModelState();
    }
  }

  private void EnterGame()
  {
    this.SetPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot, false);
    this.ClientRPCPlayer((Connection) null, this, "FinishLoading");
    this.Invoke(new Action(this.DelayedTeamUpdate), 1f);
    if (this.net != null)
      EACServer.OnFinishLoading(this.net.get_connection());
    Debug.LogFormat("{0} has entered the game", new object[1]
    {
      (object) this
    });
  }

  [BaseEntity.RPC_Server.FromOwner]
  [BaseEntity.RPC_Server]
  private void ClientKeepConnectionAlive(BaseEntity.RPCMessage msg)
  {
    this.lastTickTime = Time.get_time();
  }

  [BaseEntity.RPC_Server.FromOwner]
  [BaseEntity.RPC_Server]
  private void ClientLoadingComplete(BaseEntity.RPCMessage msg)
  {
  }

  public void PlayerInit(Connection c)
  {
    using (TimeWarning.New(nameof (PlayerInit), 10L))
    {
      this.CancelInvoke(new Action(((BaseNetworkable) this).KillMessage));
      this.SetPlayerFlag(BasePlayer.PlayerFlags.Connected, true);
      BasePlayer.activePlayerList.Add(this);
      this.userID = (ulong) c.userid;
      this.UserIDString = this.userID.ToString();
      this._displayName = StringEx.ToPrintable((string) c.username, 32);
      c.player = (__Null) this;
      this.tickInterpolator.Reset(((Component) this).get_transform().get_position());
      this.lastTickTime = 0.0f;
      this.lastInputTime = 0.0f;
      this.SetPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot, true);
      this.stats.Init();
      this.previousLifeStory = ((ServerMgr) SingletonComponent<ServerMgr>.Instance).persistance.GetLastLifeStory(this.userID);
      this.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, c.authLevel > 0);
      this.SetPlayerFlag(BasePlayer.PlayerFlags.IsDeveloper, DeveloperList.IsDeveloper(this));
      if (this.IsDead() && this.net.SwitchGroup(BaseNetworkable.LimboNetworkGroup))
        this.SendNetworkGroupChange();
      this.net.OnConnected(c);
      this.net.StartSubscriber();
      this.SendAsSnapshot(this.net.get_connection(), false);
      this.ClientRPCPlayer((Connection) null, this, "StartLoading");
      if (this.net != null)
        EACServer.OnStartLoading(this.net.get_connection());
      Interface.CallHook("OnPlayerInit", (object) this);
      if (!this.IsAdmin)
        return;
      if (ConVar.AntiHack.noclip_protection <= 0)
        this.ChatMessage("antihack.noclip_protection is disabled!");
      if (ConVar.AntiHack.speedhack_protection <= 0)
        this.ChatMessage("antihack.speedhack_protection is disabled!");
      if (ConVar.AntiHack.flyhack_protection <= 0)
        this.ChatMessage("antihack.flyhack_protection is disabled!");
      if (ConVar.AntiHack.projectile_protection <= 0)
        this.ChatMessage("antihack.projectile_protection is disabled!");
      if (ConVar.AntiHack.melee_protection <= 0)
        this.ChatMessage("antihack.melee_protection is disabled!");
      if (ConVar.AntiHack.eye_protection > 0)
        return;
      this.ChatMessage("antihack.eye_protection is disabled!");
    }
  }

  public void SendDeathInformation()
  {
    this.ClientRPCPlayer((Connection) null, this, "OnDied");
  }

  public void SendRespawnOptions()
  {
    using (RespawnInformation respawnInformation = (RespawnInformation) Pool.Get<RespawnInformation>())
    {
      respawnInformation.spawnOptions = (__Null) Pool.Get<List<RespawnInformation.SpawnOptions>>();
      foreach (SleepingBag sleepingBag in SleepingBag.FindForPlayer(this.userID, true))
      {
        RespawnInformation.SpawnOptions spawnOptions = (RespawnInformation.SpawnOptions) Pool.Get<RespawnInformation.SpawnOptions>();
        spawnOptions.id = sleepingBag.net.ID;
        spawnOptions.name = (__Null) sleepingBag.niceName;
        spawnOptions.type = (__Null) 1;
        spawnOptions.unlockSeconds = (__Null) (double) sleepingBag.unlockSeconds;
        ((List<RespawnInformation.SpawnOptions>) respawnInformation.spawnOptions).Add(spawnOptions);
      }
      respawnInformation.previousLife = (__Null) this.previousLifeStory;
      respawnInformation.fadeIn = this.previousLifeStory == null ? (__Null) 0 : (__Null) ((long) (ulong) this.previousLifeStory.timeDied > (long) (Epoch.get_Current() - 5) ? 1 : 0);
      this.ClientRPCPlayer<RespawnInformation>((Connection) null, this, "OnRespawnInformation", respawnInformation);
    }
  }

  public float secondsSleeping
  {
    get
    {
      if ((double) this.sleepStartTime == -1.0 || !this.IsSleeping())
        return 0.0f;
      return Time.get_time() - this.sleepStartTime;
    }
  }

  public virtual void StartSleeping()
  {
    if (this.IsSleeping())
      return;
    Interface.CallHook("OnPlayerSleep", (object) this);
    this.EnsureDismounted();
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Sleeping, true);
    this.sleepStartTime = Time.get_time();
    if (!BasePlayer.sleepingPlayerList.Contains(this))
      BasePlayer.sleepingPlayerList.Add(this);
    this.CancelInvoke(new Action(this.InventoryUpdate));
    this.CancelInvoke(new Action(this.TeamUpdate));
    this.inventory.loot.Clear();
    this.inventory.crafting.CancelAll(true);
    this.UpdatePlayerCollider(true);
    this.UpdatePlayerRigidbody(false);
    this.EnableServerFall(true);
  }

  private void OnPhysicsNeighbourChanged()
  {
    if (!this.IsSleeping() && !this.IsWounded())
      return;
    this.Invoke(new Action(this.DelayedServerFall), 0.05f);
  }

  private void DelayedServerFall()
  {
    this.EnableServerFall(true);
  }

  public void EnableServerFall(bool wantsOn)
  {
    if (wantsOn && ConVar.Server.playerserverfall)
    {
      if (this.IsInvoking(new Action(this.ServerFall)))
        return;
      this.SetPlayerFlag(BasePlayer.PlayerFlags.ServerFall, true);
      this.lastFallTime = Time.get_time() - this.fallTickRate;
      this.InvokeRandomized(new Action(this.ServerFall), 0.0f, this.fallTickRate, this.fallTickRate * 0.1f);
      this.fallVelocity = (float) this.estimatedVelocity.y;
    }
    else
    {
      this.CancelInvoke(new Action(this.ServerFall));
      this.SetPlayerFlag(BasePlayer.PlayerFlags.ServerFall, false);
    }
  }

  public void ServerFall()
  {
    if (this.IsDead() || !this.IsWounded() && !this.IsSleeping())
    {
      this.EnableServerFall(false);
    }
    else
    {
      float deltaTime = Time.get_time() - this.lastFallTime;
      this.lastFallTime = Time.get_time();
      float radius = this.GetRadius();
      float num1 = this.GetHeight(true) * 0.5f;
      this.fallVelocity += (float) (Physics.get_gravity().y * 2.5 * 0.5) * deltaTime;
      float num2 = Mathf.Abs(this.fallVelocity * deltaTime);
      Vector3 vector3_1 = Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), radius + num1));
      Vector3 position = ((Component) this).get_transform().get_position();
      Vector3 vector3_2 = ((Component) this).get_transform().get_position();
      double num3 = (double) radius;
      Vector3 down = Vector3.get_down();
      RaycastHit raycastHit;
      ref RaycastHit local = ref raycastHit;
      double num4 = (double) num2 + (double) num1;
      if (Physics.SphereCast(vector3_1, (float) num3, down, ref local, (float) num4, 1537286401, (QueryTriggerInteraction) 1))
      {
        this.EnableServerFall(false);
        if ((double) ((RaycastHit) ref raycastHit).get_distance() > (double) num1)
          vector3_2 = Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_down(), ((RaycastHit) ref raycastHit).get_distance() - num1));
        this.ApplyFallDamageFromVelocity(this.fallVelocity);
        this.UpdateEstimatedVelocity(vector3_2, vector3_2, deltaTime);
        this.fallVelocity = 0.0f;
      }
      else
      {
        vector3_2 = Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_down(), num2));
        this.UpdateEstimatedVelocity(position, vector3_2, deltaTime);
        if (WaterLevel.Test(vector3_2))
          this.EnableServerFall(false);
      }
      this.MovePosition(vector3_2);
    }
  }

  public void DelayedRigidbodyDisable()
  {
    this.UpdatePlayerRigidbody(false);
  }

  public virtual void EndSleeping()
  {
    if (!this.IsSleeping())
      return;
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Sleeping, false);
    this.sleepStartTime = -1f;
    BasePlayer.sleepingPlayerList.Remove(this);
    this.InvokeRepeating(new Action(this.InventoryUpdate), 1f, 0.1f * Random.Range(0.99f, 1.01f));
    if (RelationshipManager.TeamsEnabled())
      this.InvokeRandomized(new Action(this.TeamUpdate), 1f, 4f, 1f);
    this.UpdatePlayerCollider(true);
    this.UpdatePlayerRigidbody(true);
    this.EnableServerFall(false);
    Interface.CallHook("OnPlayerSleepEnded", (object) this);
    if (EACServer.playerTracker == null || this.net.get_connection() == null)
      return;
    using (TimeWarning.New("playerTracker.LogPlayerSpawn", 0.1f))
    {
      EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(this.net.get_connection());
      EACServer.playerTracker.LogPlayerSpawn(client, 0, 0);
    }
  }

  public virtual void EndLooting()
  {
    if (!Object.op_Implicit((Object) this.inventory.loot))
      return;
    this.inventory.loot.Clear();
  }

  public virtual void OnDisconnected()
  {
    this.stats.Save();
    this.EndLooting();
    if (this.IsAlive() || this.IsSleeping())
    {
      this.StartSleeping();
    }
    else
    {
      this.TeamDeathCleanup();
      this.Invoke(new Action(((BaseNetworkable) this).KillMessage), 0.0f);
    }
    BasePlayer.activePlayerList.Remove(this);
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Connected, false);
    if (this.net != null)
      this.net.OnDisconnected();
    this.ResetAntiHack();
  }

  private void InventoryUpdate()
  {
    if (!this.IsConnected || this.IsDead())
      return;
    this.inventory.ServerUpdate(0.1f);
  }

  public void ApplyFallDamageFromVelocity(float velocity)
  {
    float num = Mathf.InverseLerp(-15f, -100f, velocity);
    if ((double) num == 0.0 || Interface.CallHook("OnPlayerLand", (object) this, (object) num) != null)
      return;
    this.metabolism.bleeding.Add(num * 0.5f);
    float amount = num * 500f;
    this.Hurt(amount, DamageType.Fall, (BaseEntity) null, true);
    if ((double) amount > 20.0 && this.fallDamageEffect.isValid)
      Effect.server.Run(this.fallDamageEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_zero(), (Connection) null, false);
    Interface.CallHook("OnPlayerLanded", (object) this, (object) num);
  }

  [BaseEntity.RPC_Server.FromOwner]
  [BaseEntity.RPC_Server]
  private void OnPlayerLanded(BaseEntity.RPCMessage msg)
  {
    float num = msg.read.Float();
    if (float.IsNaN(num) || float.IsInfinity(num))
      return;
    this.ApplyFallDamageFromVelocity(num);
    this.fallVelocity = 0.0f;
  }

  public void SendGlobalSnapshot()
  {
    using (TimeWarning.New(nameof (SendGlobalSnapshot), 10L))
      this.EnterVisibility(((Manager) ((Network.Server) Net.sv).visibility).Get(0U));
  }

  public void SendFullSnapshot()
  {
    using (TimeWarning.New(nameof (SendFullSnapshot), 0.1f))
    {
      using (List<Group>.Enumerator enumerator = ((List<Group>) ((Subscriber) this.net.subscriber).subscribed).GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Group current = enumerator.Current;
          if (current.ID != null)
            this.EnterVisibility(current);
        }
      }
    }
  }

  public override void OnNetworkGroupLeave(Group group)
  {
    base.OnNetworkGroupLeave(group);
    this.LeaveVisibility(group);
  }

  private void LeaveVisibility(Group group)
  {
    ServerMgr.OnLeaveVisibility(this.net.get_connection(), group);
    this.ClearEntityQueue(group);
  }

  public override void OnNetworkGroupEnter(Group group)
  {
    base.OnNetworkGroupEnter(group);
    this.EnterVisibility(group);
  }

  private void EnterVisibility(Group group)
  {
    ServerMgr.OnEnterVisibility(this.net.get_connection(), group);
    this.SendSnapshots((ListHashSet<Networkable>) group.networkables);
  }

  public void CheckDeathCondition(HitInfo info = null)
  {
    Assert.IsTrue(this.isServer, "CheckDeathCondition called on client!");
    if (this.IsSpectating() || this.IsDead() || !this.metabolism.ShouldDie())
      return;
    this.Die(info);
  }

  public virtual BaseCorpse CreateCorpse()
  {
    using (TimeWarning.New("Create corpse", 0.1f))
    {
      LootableCorpse lootableCorpse = this.DropCorpse("assets/prefabs/player/player_corpse.prefab") as LootableCorpse;
      if (Object.op_Implicit((Object) lootableCorpse))
      {
        lootableCorpse.SetFlag(BaseEntity.Flags.Reserved5, this.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
        lootableCorpse.TakeFrom(this.inventory.containerMain, this.inventory.containerWear, this.inventory.containerBelt);
        lootableCorpse.playerName = this.displayName;
        lootableCorpse.playerSteamID = this.userID;
        lootableCorpse.Spawn();
        lootableCorpse.TakeChildren((BaseEntity) this);
        M0 component = ((Component) lootableCorpse).GetComponent<ResourceDispenser>();
        int num = 2;
        if (this.lifeStory != null)
          num += Mathf.Clamp(Mathf.FloorToInt((float) (this.lifeStory.secondsAlive / 180.0)), 0, 20);
        ((ResourceDispenser) component).containedItems.Add(new ItemAmount(ItemManager.FindItemDefinition("fat.animal"), (float) num));
        return (BaseCorpse) lootableCorpse;
      }
    }
    return (BaseCorpse) null;
  }

  public override void OnKilled(HitInfo info)
  {
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Unused2, false);
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Unused1, false);
    this.EnsureDismounted();
    this.EndSleeping();
    this.EndLooting();
    this.stats.Add("deaths", 1, Stats.All);
    this.UpdatePlayerCollider(false);
    this.UpdatePlayerRigidbody(false);
    this.StopWounded();
    this.inventory.crafting.CancelAll(true);
    if (EACServer.playerTracker != null && this.net.get_connection() != null)
    {
      BasePlayer basePlayer = info == null || !Object.op_Inequality((Object) info.Initiator, (Object) null) ? (BasePlayer) null : info.Initiator.ToPlayer();
      if (Object.op_Inequality((Object) basePlayer, (Object) null) && basePlayer.net.get_connection() != null)
      {
        using (TimeWarning.New("playerTracker.LogPlayerKill", 0.1f))
        {
          EasyAntiCheat.Server.Hydra.Client client1 = EACServer.GetClient(basePlayer.net.get_connection());
          EasyAntiCheat.Server.Hydra.Client client2 = EACServer.GetClient(this.net.get_connection());
          EACServer.playerTracker.LogPlayerKill(client2, client1);
        }
      }
      else
      {
        using (TimeWarning.New("playerTracker.LogPlayerDespawn", 0.1f))
        {
          EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(this.net.get_connection());
          EACServer.playerTracker.LogPlayerDespawn(client);
        }
      }
    }
    BaseCorpse corpse = this.CreateCorpse();
    if (Object.op_Inequality((Object) corpse, (Object) null) && info != null)
    {
      Rigidbody component = (Rigidbody) ((Component) corpse).GetComponent<Rigidbody>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        Rigidbody rigidbody = component;
        Vector3 vector3_1 = Vector3.op_Addition(info.attackNormal, Vector3.op_Multiply(Vector3.get_up(), 0.5f));
        Vector3 vector3_2 = Vector3.op_Multiply(((Vector3) ref vector3_1).get_normalized(), 1f);
        rigidbody.AddForce(vector3_2, (ForceMode) 2);
      }
    }
    this.inventory.Strip();
    if (this.lastDamage == DamageType.Fall)
      this.stats.Add("death_fall", 1, Stats.Steam);
    string str1;
    string msg;
    if (info != null)
    {
      if (Object.op_Implicit((Object) info.Initiator))
      {
        if (Object.op_Equality((Object) info.Initiator, (Object) this))
        {
          str1 = ((object) this).ToString() + " was suicide by " + (object) this.lastDamage;
          msg = "You died: suicide by " + (object) this.lastDamage;
          if (this.lastDamage == DamageType.Suicide)
          {
            Analytics.Death("suicide");
            this.stats.Add("death_suicide", 1, Stats.All);
          }
          else
          {
            Analytics.Death("selfinflicted");
            this.stats.Add("death_selfinflicted", 1, Stats.Steam);
          }
        }
        else if (info.Initiator is BasePlayer)
        {
          BasePlayer player = info.Initiator.ToPlayer();
          str1 = ((object) this).ToString() + " was killed by " + ((object) player).ToString();
          msg = "You died: killed by " + player.displayName + " (" + (object) player.userID + ")";
          player.stats.Add("kill_player", 1, Stats.All);
          if (Object.op_Inequality((Object) info.WeaponPrefab, (Object) null))
            Analytics.Death(info.WeaponPrefab.ShortPrefabName);
          else
            Analytics.Death("player");
        }
        else
        {
          str1 = ((object) this).ToString() + " was killed by " + info.Initiator.ShortPrefabName + " (" + info.Initiator.Categorize() + ")";
          msg = "You died: killed by " + info.Initiator.Categorize();
          this.stats.Add("death_" + info.Initiator.Categorize(), 1, Stats.Steam);
          Analytics.Death(info.Initiator.Categorize());
        }
      }
      else if (this.lastDamage == DamageType.Fall)
      {
        str1 = ((object) this).ToString() + " was killed by fall!";
        msg = "You died: killed by fall!";
        Analytics.Death("fall");
      }
      else
      {
        string str2 = ((object) this).ToString();
        DamageType majorityDamageType = info.damageTypes.GetMajorityDamageType();
        string str3 = majorityDamageType.ToString();
        str1 = str2 + " was killed by " + str3;
        majorityDamageType = info.damageTypes.GetMajorityDamageType();
        msg = "You died: " + majorityDamageType.ToString();
      }
    }
    else
    {
      str1 = ((object) this).ToString() + " died (" + (object) this.lastDamage + ")";
      msg = "You died: " + this.lastDamage.ToString();
    }
    using (TimeWarning.New("LogMessage", 0.1f))
    {
      DebugEx.Log((object) str1, (StackTraceLogType) 0);
      this.ConsoleMessage(msg);
    }
    this.SendNetworkUpdateImmediate(false);
    this.LifeStoryLogDeath(info, this.lastDamage);
    this.LifeStoryEnd();
    if (this.net.get_connection() == null)
    {
      this.TeamDeathCleanup();
      this.Invoke(new Action(((BaseNetworkable) this).KillMessage), 0.0f);
    }
    else
    {
      this.SendRespawnOptions();
      this.SendDeathInformation();
      this.stats.Save();
    }
  }

  public void RespawnAt(Vector3 position, Quaternion rotation)
  {
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Wounded, false);
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Unused2, false);
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Unused1, false);
    this.SetPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot, true);
    this.SetPlayerFlag(BasePlayer.PlayerFlags.DisplaySash, false);
    ++ServerPerformance.spawns;
    this.SetParent((BaseEntity) null, true, false);
    ((Component) this).get_transform().set_position(position);
    ((Component) this).get_transform().set_rotation(rotation);
    this.tickInterpolator.Reset(position);
    this.lastTickTime = 0.0f;
    this.StopWounded();
    this.StopSpectating();
    this.UpdateNetworkGroup();
    this.UpdatePlayerCollider(true);
    this.UpdatePlayerRigidbody(false);
    this.StartSleeping();
    this.LifeStoryStart();
    this.metabolism.Reset();
    this.InitializeHealth(this.StartHealth(), this.StartMaxHealth());
    this.inventory.GiveDefaultItems();
    this.SendNetworkUpdateImmediate(false);
    this.ClientRPCPlayer((Connection) null, this, "StartLoading");
    if (this.net != null)
      EACServer.OnStartLoading(this.net.get_connection());
    Interface.CallHook("OnPlayerRespawned", (object) this);
  }

  public void Respawn()
  {
    BasePlayer.SpawnPoint spawnPoint = ServerMgr.FindSpawnPoint();
    object obj = Interface.CallHook("OnPlayerRespawn", (object) this);
    if (obj is BasePlayer.SpawnPoint)
      spawnPoint = (BasePlayer.SpawnPoint) obj;
    this.RespawnAt(spawnPoint.pos, spawnPoint.rot);
  }

  public bool IsImmortal()
  {
    return (this.IsAdmin || this.IsDeveloper) && (this.IsConnected && this.net.get_connection() != null) && ((Connection.ClientInfo) this.net.get_connection().info).GetBool("global.god", false) || this.WoundingCausingImmportality();
  }

  public float TimeAlive()
  {
    return (float) this.lifeStory.secondsAlive;
  }

  public override void Hurt(HitInfo info)
  {
    if (this.IsDead() || this.IsImmortal() || Interface.CallHook("IOnBasePlayerHurt", (object) this, (object) info) != null)
      return;
    if (ConVar.Server.pve && Object.op_Implicit((Object) info.Initiator) && (info.Initiator is BasePlayer && Object.op_Inequality((Object) info.Initiator, (Object) this)))
    {
      (info.Initiator as BasePlayer).Hurt(info.damageTypes.Total(), DamageType.Generic, (BaseEntity) null, true);
    }
    else
    {
      if ((double) info.damageTypes.Get(DamageType.Drowned) > 5.0 && this.drownEffect.isValid)
        Effect.server.Run(this.drownEffect.resourcePath, (BaseEntity) this, StringPool.Get("head"), Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
      this.metabolism.pending_health.Subtract(info.damageTypes.Total() * 10f);
      BasePlayer initiatorPlayer = info.InitiatorPlayer;
      if (Object.op_Implicit((Object) initiatorPlayer) && Object.op_Inequality((Object) initiatorPlayer, (Object) this))
      {
        if (initiatorPlayer.InSafeZone() || this.InSafeZone())
          initiatorPlayer.MarkHostileFor(1800f);
        if (initiatorPlayer.IsNpc && initiatorPlayer.Family == BaseNpc.AiStatistics.FamilyEnum.Murderer && (double) info.damageTypes.Get(DamageType.Explosion) > 0.0)
          info.damageTypes.ScaleAll(Halloween.scarecrow_beancan_vs_player_dmg_modifier);
      }
      base.Hurt(info);
      if (EACServer.playerTracker != null && Object.op_Inequality((Object) info.Initiator, (Object) null) && info.Initiator is BasePlayer)
      {
        BasePlayer player = info.Initiator.ToPlayer();
        if (this.net.get_connection() != null && player.net.get_connection() != null)
        {
          EasyAntiCheat.Server.Hydra.Client client1 = EACServer.GetClient(this.net.get_connection());
          EasyAntiCheat.Server.Hydra.Client client2 = EACServer.GetClient(player.net.get_connection());
          PlayerTakeDamage playerTakeDamage = (PlayerTakeDamage) null;
          playerTakeDamage.DamageTaken = (__Null) (int) info.damageTypes.Total();
          playerTakeDamage.HitBoneID = (__Null) (int) info.HitBone;
          playerTakeDamage.WeaponID = (__Null) 0;
          playerTakeDamage.DamageFlags = info.isHeadshot ? (__Null) 1 : (__Null) 0;
          if (Object.op_Inequality((Object) info.Weapon, (Object) null))
          {
            Item obj = info.Weapon.GetItem();
            if (obj != null)
              playerTakeDamage.WeaponID = (__Null) obj.info.itemid;
          }
          Vector3 position1 = player.eyes.position;
          Quaternion rotation1 = player.eyes.rotation;
          Vector3 position2 = this.eyes.position;
          Quaternion rotation2 = this.eyes.rotation;
          playerTakeDamage.AttackerPosition = (__Null) new Vector3((float) position1.x, (float) position1.y, (float) position1.z);
          playerTakeDamage.AttackerViewRotation = (__Null) new Quaternion((float) rotation1.x, (float) rotation1.y, (float) rotation1.z, (float) rotation1.w);
          playerTakeDamage.VictimPosition = (__Null) new Vector3((float) position2.x, (float) position2.y, (float) position2.z);
          playerTakeDamage.VictimViewRotation = (__Null) new Quaternion((float) rotation2.x, (float) rotation2.y, (float) rotation2.z, (float) rotation2.w);
          EACServer.playerTracker.LogPlayerTakeDamage(client1, client2, playerTakeDamage);
        }
      }
      this.metabolism.SendChangesToClient();
      if (!Vector3.op_Inequality(info.PointStart, Vector3.get_zero()))
        return;
      this.ClientRPCPlayer<Vector3, int>((Connection) null, this, "DirectionalDamage", info.PointStart, (int) info.damageTypes.GetMajorityDamageType());
    }
  }

  public static BasePlayer FindByID(ulong userID)
  {
    using (TimeWarning.New("BasePlayer.FindByID", 0.1f))
      return BasePlayer.activePlayerList.Find((Predicate<BasePlayer>) (x => (long) x.userID == (long) userID));
  }

  public static BasePlayer FindSleeping(ulong userID)
  {
    using (TimeWarning.New("BasePlayer.FindSleeping", 0.1f))
      return BasePlayer.sleepingPlayerList.Find((Predicate<BasePlayer>) (x => (long) x.userID == (long) userID));
  }

  public void Command(string strCommand, params object[] arguments)
  {
    if (this.net.get_connection() == null)
      return;
    ConsoleNetwork.SendClientCommand(this.net.get_connection(), strCommand, arguments);
  }

  public override void OnInvalidPosition()
  {
    if (this.IsDead())
      return;
    this.Die((HitInfo) null);
  }

  private static BasePlayer Find(string strNameOrIDOrIP, List<BasePlayer> list)
  {
    BasePlayer basePlayer1 = list.Find((Predicate<BasePlayer>) (x => x.UserIDString == strNameOrIDOrIP));
    if (Object.op_Implicit((Object) basePlayer1))
      return basePlayer1;
    BasePlayer basePlayer2 = list.Find((Predicate<BasePlayer>) (x => x.displayName.StartsWith(strNameOrIDOrIP, StringComparison.CurrentCultureIgnoreCase)));
    if (Object.op_Implicit((Object) basePlayer2))
      return basePlayer2;
    BasePlayer basePlayer3 = list.Find((Predicate<BasePlayer>) (x =>
    {
      if (x.net != null && x.net.get_connection() != null)
        return (string) x.net.get_connection().ipaddress == strNameOrIDOrIP;
      return false;
    }));
    if (Object.op_Implicit((Object) basePlayer3))
      return basePlayer3;
    return (BasePlayer) null;
  }

  public static BasePlayer Find(string strNameOrIDOrIP)
  {
    return BasePlayer.Find(strNameOrIDOrIP, BasePlayer.activePlayerList);
  }

  public static BasePlayer FindSleeping(string strNameOrIDOrIP)
  {
    return BasePlayer.Find(strNameOrIDOrIP, BasePlayer.sleepingPlayerList);
  }

  public void SendConsoleCommand(string command, params object[] obj)
  {
    ConsoleNetwork.SendClientCommand(this.net.get_connection(), command, obj);
  }

  public void UpdateRadiation(float fAmount)
  {
    this.metabolism.radiation_level.Increase(fAmount);
  }

  public override float RadiationExposureFraction()
  {
    return 1f - Mathf.Clamp(this.baseProtection.amounts[17], 0.0f, 1f);
  }

  public override float RadiationProtection()
  {
    return this.baseProtection.amounts[17] * 100f;
  }

  public override void OnHealthChanged(float oldvalue, float newvalue)
  {
    if (Interface.CallHook("OnPlayerHealthChange", (object) this, (object) oldvalue, (object) newvalue) != null)
      return;
    base.OnHealthChanged(oldvalue, newvalue);
    this.metabolism.isDirty = true;
  }

  public void SV_ClothingChanged()
  {
    this.UpdateProtectionFromClothing();
    this.UpdateMoveSpeedFromClothing();
  }

  public bool IsNoob()
  {
    return !this.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash);
  }

  public bool HasHostileItem()
  {
    using (TimeWarning.New("BasePlayer.HasHostileItem", 0.1f))
    {
      foreach (Item obj in this.inventory.containerBelt.itemList)
      {
        if (this.IsHostileItem(obj))
          return true;
      }
      foreach (Item obj in this.inventory.containerMain.itemList)
      {
        if (this.IsHostileItem(obj))
          return true;
      }
      return false;
    }
  }

  public bool IsHostileItem(Item item)
  {
    if (!item.info.isHoldable)
      return false;
    ItemModEntity component1 = (ItemModEntity) ((Component) item.info).GetComponent<ItemModEntity>();
    if (Object.op_Equality((Object) component1, (Object) null))
      return false;
    GameObject gameObject = component1.entityPrefab.Get();
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return false;
    AttackEntity component2 = (AttackEntity) gameObject.GetComponent<AttackEntity>();
    if (Object.op_Equality((Object) component2, (Object) null))
      return false;
    return component2.hostile;
  }

  public override void GiveItem(Item item, BaseEntity.GiveItemReason reason = BaseEntity.GiveItemReason.Generic)
  {
    if (reason == BaseEntity.GiveItemReason.ResourceHarvested)
      this.stats.Add(string.Format("harvest.{0}", (object) item.info.shortname), item.amount, Stats.Steam);
    int amount = item.amount;
    if (this.inventory.GiveItem(item, (ItemContainer) null))
    {
      if (!string.IsNullOrEmpty(item.name))
        this.Command("note.inv", (object) item.info.itemid, (object) amount, (object) item.name, (object) (int) reason);
      else
        this.Command("note.inv", (object) item.info.itemid, (object) amount, (object) string.Empty, (object) (int) reason);
    }
    else
      item.Drop(this.inventory.containerMain.dropPosition, this.inventory.containerMain.dropVelocity, (Quaternion) null);
  }

  public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
  {
    info.attackerName = (__Null) this.displayName;
    info.attackerSteamID = (__Null) (long) this.userID;
  }

  public float currentCraftLevel
  {
    get
    {
      if (this.triggers == null)
        return 0.0f;
      if ((double) this.nextCheckTime > (double) Time.get_realtimeSinceStartup())
        return this.cachedCraftLevel;
      this.nextCheckTime = Time.get_realtimeSinceStartup() + Random.Range(0.4f, 0.5f);
      float num1 = 0.0f;
      for (int index = 0; index < this.triggers.Count; ++index)
      {
        TriggerWorkbench trigger = this.triggers[index] as TriggerWorkbench;
        if (!Object.op_Equality((Object) trigger, (Object) null) && !Object.op_Equality((Object) trigger.parentBench, (Object) null) && trigger.parentBench.IsVisible(this.eyes.position, float.PositiveInfinity))
        {
          float num2 = trigger.WorkbenchLevel();
          if ((double) num2 > (double) num1)
            num1 = num2;
        }
      }
      this.cachedCraftLevel = num1;
      return num1;
    }
  }

  public float currentComfort
  {
    get
    {
      float num = 0.0f;
      if (this.isMounted)
        num = this.GetMounted().GetComfort();
      if (this.triggers == null)
        return num;
      for (int index = 0; index < this.triggers.Count; ++index)
      {
        TriggerComfort trigger = this.triggers[index] as TriggerComfort;
        if (!Object.op_Equality((Object) trigger, (Object) null))
        {
          float comfort = trigger.CalculateComfort(((Component) this).get_transform().get_position(), this);
          if ((double) comfort > (double) num)
            num = comfort;
        }
      }
      return num;
    }
  }

  public float currentSafeLevel
  {
    get
    {
      float num = 0.0f;
      if (this.triggers == null)
        return num;
      for (int index = 0; index < this.triggers.Count; ++index)
      {
        TriggerSafeZone trigger = this.triggers[index] as TriggerSafeZone;
        if (!Object.op_Equality((Object) trigger, (Object) null))
        {
          float safeLevel = trigger.GetSafeLevel(((Component) this).get_transform().get_position());
          if ((double) safeLevel > (double) num)
            num = safeLevel;
        }
      }
      return num;
    }
  }

  public virtual bool ShouldDropActiveItem()
  {
    object obj = Interface.CallHook("CanDropActiveItem", (object) this);
    if (obj is bool)
      return (bool) obj;
    return true;
  }

  public override void Die(HitInfo info = null)
  {
    using (TimeWarning.New("Player.Die", 0.1f))
    {
      if (this.IsDead())
        return;
      if (this.Belt != null && this.ShouldDropActiveItem())
      {
        Vector3 vector3;
        ((Vector3) ref vector3).\u002Ector(Random.Range(-2f, 2f), 0.2f, Random.Range(-2f, 2f));
        this.Belt.DropActive(this.GetDropPosition(), Vector3.op_Addition(this.GetInheritedDropVelocity(), Vector3.op_Multiply(((Vector3) ref vector3).get_normalized(), 3f)));
      }
      if (this.WoundInsteadOfDying(info) || Interface.CallHook("OnPlayerDie", (object) this, (object) info) != null)
        return;
      base.Die(info);
    }
  }

  public void Kick(string reason)
  {
    if (!this.IsConnected)
      return;
    ((Network.Server) Net.sv).Kick(this.net.get_connection(), reason);
    Interface.CallHook("OnPlayerKicked", (object) this, (object) reason);
  }

  public override Vector3 GetDropPosition()
  {
    return this.eyes.position;
  }

  public override Vector3 GetDropVelocity()
  {
    return Vector3.op_Addition(Vector3.op_Addition(this.GetInheritedDropVelocity(), Vector3.op_Multiply(this.eyes.BodyForward(), 4f)), Vector3Ex.Range(-0.5f, 0.5f));
  }

  public virtual void SetInfo(string key, string val)
  {
    if (!this.IsConnected)
      return;
    ((Connection.ClientInfo) this.net.get_connection().info).Set(key, val);
  }

  public virtual int GetInfoInt(string key, int defaultVal)
  {
    if (!this.IsConnected)
      return defaultVal;
    return ((Connection.ClientInfo) this.net.get_connection().info).GetInt(key, defaultVal);
  }

  [BaseEntity.RPC_Server]
  public void PerformanceReport(BaseEntity.RPCMessage msg)
  {
    Debug.LogFormat("{0}{1}{2}{3}{4}", new object[5]
    {
      (object) (msg.read.Int32().ToString() + "MB").PadRight(9),
      (object) (msg.read.Int32().ToString() + "MB").PadRight(9),
      (object) (msg.read.Float().ToString("0") + "FPS").PadRight(8),
      (object) NumberExtensions.FormatSeconds((long) msg.read.Int32()).PadRight(9),
      (object) this.displayName
    });
  }

  public override bool ShouldNetworkTo(BasePlayer player)
  {
    if (this.IsSpectating() && Object.op_Inequality((Object) player, (Object) this) && !((Connection.ClientInfo) player.net.get_connection().info).GetBool("global.specnet", false))
      return false;
    return base.ShouldNetworkTo(player);
  }

  internal void GiveAchievement(string name)
  {
    if (!GameInfo.HasAchievements)
      return;
    this.ClientRPCPlayer<string>((Connection) null, this, "RecieveAchievement", name);
  }

  public bool hasPreviousLife
  {
    get
    {
      return this.previousLifeStory != null;
    }
  }

  internal void LifeStoryStart()
  {
    Assert.IsTrue(this.lifeStory == null, "Stomping old lifeStory");
    this.lifeStory = new PlayerLifeStory()
    {
      ShouldPool = (__Null) 0
    };
    this.lifeStory.timeBorn = (__Null) Epoch.get_Current();
  }

  public void LifeStoryEnd()
  {
    ((ServerMgr) SingletonComponent<ServerMgr>.Instance).persistance.AddLifeStory(this.userID, this.lifeStory);
    this.previousLifeStory = this.lifeStory;
    this.lifeStory = (PlayerLifeStory) null;
  }

  internal void LifeStoryUpdate(float deltaTime)
  {
    if (this.lifeStory == null)
      return;
    PlayerLifeStory lifeStory1 = this.lifeStory;
    lifeStory1.secondsAlive = (__Null) (lifeStory1.secondsAlive + (double) deltaTime);
    if (!this.IsSleeping())
      return;
    PlayerLifeStory lifeStory2 = this.lifeStory;
    lifeStory2.secondsSleeping = (__Null) (lifeStory2.secondsSleeping + (double) deltaTime);
  }

  internal void LifeStoryLogDeath(HitInfo deathBlow, DamageType lastDamage)
  {
    if (this.lifeStory == null)
      return;
    this.lifeStory.timeDied = (__Null) Epoch.get_Current();
    PlayerLifeStory.DeathInfo info = (PlayerLifeStory.DeathInfo) Pool.Get<PlayerLifeStory.DeathInfo>();
    info.lastDamageType = (__Null) lastDamage;
    if (deathBlow != null)
    {
      if (Object.op_Inequality((Object) deathBlow.Initiator, (Object) null))
        deathBlow.Initiator.AttackerInfo(info);
      if (Object.op_Inequality((Object) deathBlow.WeaponPrefab, (Object) null))
        info.inflictorName = (__Null) deathBlow.WeaponPrefab.ShortPrefabName;
      info.hitBone = deathBlow.HitBone <= 0U ? (__Null) "" : (__Null) StringPool.Get(deathBlow.HitBone);
    }
    else if ((double) this.SecondsSinceAttacked <= 60.0 && Object.op_Inequality((Object) this.lastAttacker, (Object) null))
      this.lastAttacker.AttackerInfo(info);
    this.lifeStory.deathInfo = (__Null) info;
  }

  private void Tick_Spectator()
  {
    int num = 0;
    if (this.serverInput.WasJustPressed(BUTTON.JUMP))
      ++num;
    if (this.serverInput.WasJustPressed(BUTTON.DUCK))
      --num;
    if (num == 0)
      return;
    this.SpectateOffset += num;
    using (TimeWarning.New("UpdateSpectateTarget", 0.1f))
      this.UpdateSpectateTarget(this.spectateFilter);
  }

  public void UpdateSpectateTarget(string strName)
  {
    this.spectateFilter = strName;
    IEnumerable<BaseEntity> source1;
    if (this.spectateFilter.StartsWith("@"))
    {
      string filter = this.spectateFilter.Substring(1);
      source1 = BaseNetworkable.serverEntities.Where<BaseNetworkable>((Func<BaseNetworkable, bool>) (x => StringEx.Contains(((Object) x).get_name(), filter, CompareOptions.IgnoreCase))).Where<BaseNetworkable>((Func<BaseNetworkable, bool>) (x => Object.op_Inequality((Object) x, (Object) this))).Cast<BaseEntity>();
    }
    else
    {
      IEnumerable<BasePlayer> source2 = BasePlayer.activePlayerList.Where<BasePlayer>((Func<BasePlayer, bool>) (x =>
      {
        if (!x.IsSpectating() && !x.IsDead())
          return !x.IsSleeping();
        return false;
      }));
      if (strName.Length > 0)
        source2 = source2.Where<BasePlayer>((Func<BasePlayer, bool>) (x =>
        {
          if (!StringEx.Contains(x.displayName, this.spectateFilter, CompareOptions.IgnoreCase))
            return x.UserIDString.Contains(this.spectateFilter);
          return true;
        })).Where<BasePlayer>((Func<BasePlayer, bool>) (x => Object.op_Inequality((Object) x, (Object) this)));
      source1 = source2.OrderBy<BasePlayer, string>((Func<BasePlayer, string>) (x => x.displayName)).Cast<BaseEntity>();
    }
    BaseEntity[] array = source1.ToArray<BaseEntity>();
    if (array.Length == 0)
    {
      this.ChatMessage("No valid spectate targets!");
    }
    else
    {
      BaseEntity entity = array[this.SpectateOffset % array.Length];
      if (!Object.op_Inequality((Object) entity, (Object) null))
        return;
      if (entity is BasePlayer)
        this.ChatMessage("Spectating: " + (entity as BasePlayer).displayName);
      else
        this.ChatMessage("Spectating: " + ((object) entity).ToString());
      using (TimeWarning.New("SendEntitySnapshot", 0.1f))
        this.SendEntitySnapshot((BaseNetworkable) entity);
      ((Component) this).get_gameObject().Identity();
      using (TimeWarning.New("SetParent", 0.1f))
        this.SetParent(entity, false, false);
    }
  }

  public void StartSpectating()
  {
    if (this.IsSpectating() || Interface.CallHook("OnPlayerSpectate", (object) this, (object) this.spectateFilter) != null)
      return;
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Spectating, true);
    ((Component) this).get_gameObject().SetLayerRecursive(10);
    this.CancelInvoke(new Action(this.InventoryUpdate));
    this.ChatMessage("Becoming Spectator");
    this.UpdateSpectateTarget(this.spectateFilter);
  }

  public void StopSpectating()
  {
    if (!this.IsSpectating() || Interface.CallHook("OnPlayerSpectateEnd", (object) this, (object) this.spectateFilter) != null)
      return;
    this.SetParent((BaseEntity) null, false, false);
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Spectating, false);
    ((Component) this).get_gameObject().SetLayerRecursive(17);
  }

  public void Teleport(BasePlayer player)
  {
    this.Teleport(((Component) player).get_transform().get_position());
  }

  public void Teleport(string strName, bool playersOnly)
  {
    BaseEntity[] targets = BaseEntity.Util.FindTargets(strName, playersOnly);
    if (targets == null || targets.Length == 0)
      return;
    this.Teleport(((Component) targets[Random.Range(0, targets.Length)]).get_transform().get_position());
  }

  public void Teleport(Vector3 position)
  {
    this.MovePosition(position);
    this.ClientRPCPlayer<Vector3>((Connection) null, this, "ForcePositionTo", position);
  }

  public override float GetThreatLevel()
  {
    this.EnsureUpdated();
    return this.cachedThreatLevel;
  }

  public void EnsureUpdated()
  {
    if ((double) Time.get_realtimeSinceStartup() - (double) this.lastUpdateTime < 30.0)
      return;
    this.lastUpdateTime = Time.get_realtimeSinceStartup();
    this.cachedThreatLevel = 0.0f;
    if (this.IsSleeping())
      return;
    if (this.inventory.containerWear.itemList.Count > 2)
      ++this.cachedThreatLevel;
    foreach (Item obj in this.inventory.containerBelt.itemList)
    {
      BaseEntity heldEntity = obj.GetHeldEntity();
      if (Object.op_Implicit((Object) heldEntity) && heldEntity is BaseProjectile && !(heldEntity is BowWeapon))
      {
        this.cachedThreatLevel += 2f;
        break;
      }
    }
  }

  public override void MarkHostileFor(float duration = 60f)
  {
    base.MarkHostileFor(duration);
    this.ClientRPCPlayer<float>((Connection) null, this, "SetHostileLength", this.unHostileTime - Time.get_realtimeSinceStartup());
  }

  public void MarkWeaponDrawnDuration(float newDuration)
  {
    float weaponDrawnDuration = this.weaponDrawnDuration;
    this.weaponDrawnDuration = newDuration;
    if ((double) Mathf.FloorToInt(newDuration) == (double) weaponDrawnDuration)
      return;
    this.ClientRPCPlayer<float>((Connection) null, this, "SetWeaponDrawnDuration", this.weaponDrawnDuration);
  }

  public void AddWeaponDrawnDuration(float duration)
  {
    this.MarkWeaponDrawnDuration(this.weaponDrawnDuration + duration);
  }

  public float timeSinceLastTick
  {
    get
    {
      if ((double) this.lastTickTime == 0.0)
        return 0.0f;
      return Time.get_time() - this.lastTickTime;
    }
  }

  public float IdleTime
  {
    get
    {
      if ((double) this.lastInputTime == 0.0)
        return 0.0f;
      return Time.get_time() - this.lastInputTime;
    }
  }

  public bool isStalled
  {
    get
    {
      if (this.IsDead() || this.IsSleeping())
        return false;
      return (double) this.timeSinceLastTick > 1.0;
    }
  }

  public bool wasStalled
  {
    get
    {
      if (this.isStalled)
        this.lastStallTime = Time.get_time();
      return (double) Time.get_time() - (double) this.lastStallTime < 1.0;
    }
  }

  public void OnReceivedTick(Stream stream)
  {
    using (TimeWarning.New("OnReceiveTickFromStream", 0.1f))
    {
      PlayerTick msg = (PlayerTick) null;
      using (TimeWarning.New("lastReceivedTick = data.Copy", 0.1f))
        msg = PlayerTick.Deserialize(stream, this.lastReceivedTick, true);
      using (TimeWarning.New("lastReceivedTick = data.Copy", 0.1f))
        this.lastReceivedTick = msg.Copy();
      using (TimeWarning.New("OnReceiveTick", 0.1f))
        this.OnReceiveTick(msg, this.wasStalled);
      this.lastTickTime = Time.get_time();
      msg.Dispose();
    }
  }

  public void OnReceivedVoice(byte[] data)
  {
    if (Interface.CallHook("OnPlayerVoice", (object) this, (object) data) != null || !((Write) ((NetworkPeer) Net.sv).write).Start())
      return;
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 21);
    ((Write) ((NetworkPeer) Net.sv).write).UInt32((uint) this.net.ID);
    ((Write) ((NetworkPeer) Net.sv).write).BytesWithSize(data);
    // ISSUE: variable of the null type
    __Null write = ((NetworkPeer) Net.sv).write;
    SendInfo sendInfo1;
    ((SendInfo) ref sendInfo1).\u002Ector(BaseNetworkable.GetConnectionsWithin(((Component) this).get_transform().get_position(), 100f));
    sendInfo1.priority = (__Null) 0;
    SendInfo sendInfo2 = sendInfo1;
    ((Write) write).Send(sendInfo2);
  }

  private void EACStateUpdate()
  {
    if (this.net == null || this.net.get_connection() == null || (EACServer.playerTracker == null || this.IsReceivingSnapshot))
      return;
    Vector3 position = this.eyes.position;
    Quaternion rotation = this.eyes.rotation;
    EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(this.net.get_connection());
    PlayerTick playerTick = (PlayerTick) null;
    playerTick.Position = (__Null) new Vector3((float) position.x, (float) position.y, (float) position.z);
    playerTick.ViewRotation = (__Null) new Quaternion((float) rotation.x, (float) rotation.y, (float) rotation.z, (float) rotation.w);
    if (this.IsDucked())
    {
      ref __Null local = ref playerTick.TickFlags;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local = ^(int&) ref local | 1;
    }
    if (this.isMounted)
    {
      ref __Null local = ref playerTick.TickFlags;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local = ^(int&) ref local | 4;
    }
    if (this.IsWounded())
    {
      ref __Null local = ref playerTick.TickFlags;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local = ^(int&) ref local | 8;
    }
    if (this.IsSwimming())
    {
      ref __Null local = ref playerTick.TickFlags;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local = ^(int&) ref local | 16;
    }
    if (!this.IsOnGround())
    {
      ref __Null local = ref playerTick.TickFlags;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local = ^(int&) ref local | 32;
    }
    if (this.OnLadder())
    {
      ref __Null local = ref playerTick.TickFlags;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(int&) ref local = ^(int&) ref local | 64;
    }
    using (TimeWarning.New("playerTracker.LogPlayerState", 0.1f))
    {
      try
      {
        EACServer.playerTracker.LogPlayerTick(client, playerTick);
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) "Disabling EAC Logging due to exception");
        EACServer.playerTracker = (ICerberus<EasyAntiCheat.Server.Hydra.Client>) null;
        Debug.LogException(ex);
      }
    }
  }

  private void OnReceiveTick(PlayerTick msg, bool wasPlayerStalled)
  {
    if (msg.inputState != null)
      this.serverInput.Flip((InputMessage) msg.inputState);
    if (Interface.CallHook("OnPlayerTick", (object) this, (object) msg, (object) wasPlayerStalled) != null)
      return;
    if (this.serverInput.current.buttons != this.serverInput.previous.buttons)
      this.lastInputTime = Time.get_time();
    if (Interface.CallHook("OnPlayerInput", (object) this, (object) this.serverInput) != null || this.IsReceivingSnapshot)
      return;
    if (this.IsSpectating())
    {
      using (TimeWarning.New("Tick_Spectator", 0.1f))
        this.Tick_Spectator();
    }
    else
    {
      if (this.IsDead())
        return;
      if (this.IsSleeping())
      {
        if (this.serverInput.WasJustPressed(BUTTON.FIRE_PRIMARY) || this.serverInput.WasJustPressed(BUTTON.FIRE_SECONDARY) || (this.serverInput.WasJustPressed(BUTTON.JUMP) || this.serverInput.WasJustPressed(BUTTON.DUCK)))
        {
          this.EndSleeping();
          this.SendNetworkUpdateImmediate(false);
        }
        this.UpdateActiveItem(0U);
      }
      else
      {
        this.UpdateActiveItem((uint) msg.activeItem);
        this.UpdateModelStateFromTick(msg);
        if (this.IsWounded())
          return;
        if (this.isMounted)
          this.GetMounted().PlayerServerInput(this.serverInput, this);
        this.UpdatePositionFromTick(msg, wasPlayerStalled);
        this.UpdateRotationFromTick(msg);
      }
    }
  }

  internal void UpdateActiveItem(uint itemID)
  {
    Assert.IsTrue(this.isServer, "Realm should be server!");
    if ((int) this.svActiveItemID == (int) itemID)
      return;
    if (this.equippingBlocked)
      itemID = 0U;
    Item activeItem1 = this.GetActiveItem();
    this.svActiveItemID = 0U;
    if (activeItem1 != null)
    {
      HeldEntity heldEntity = activeItem1.GetHeldEntity() as HeldEntity;
      if (Object.op_Inequality((Object) heldEntity, (Object) null))
        heldEntity.SetHeld(false);
    }
    this.svActiveItemID = itemID;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    Item activeItem2 = this.GetActiveItem();
    if (activeItem2 != null)
    {
      HeldEntity heldEntity = activeItem2.GetHeldEntity() as HeldEntity;
      if (Object.op_Inequality((Object) heldEntity, (Object) null))
        heldEntity.SetHeld(true);
    }
    this.inventory.UpdatedVisibleHolsteredItems();
    Interface.CallHook("OnPlayerActiveItemChanged", (object) this, (object) activeItem1, (object) activeItem2);
  }

  internal void UpdateModelStateFromTick(PlayerTick tick)
  {
    if (tick.modelState == null || ModelState.Equal(this.modelStateTick, (ModelState) tick.modelState))
      return;
    if (this.modelStateTick != null)
      this.modelStateTick.ResetToPool();
    this.modelStateTick = (ModelState) tick.modelState;
    tick.modelState = null;
    this.tickNeedsFinalizing = true;
  }

  internal void UpdatePositionFromTick(PlayerTick tick, bool wasPlayerStalled)
  {
    if (Vector3Ex.IsNaNOrInfinity((Vector3) tick.position) || Vector3Ex.IsNaNOrInfinity((Vector3) tick.eyePos))
    {
      this.Kick("Kicked: Invalid Position");
    }
    else
    {
      if (tick.parentID != (int) this.parentEntity.uid || this.isMounted || this.modelState != null && this.modelState.get_mounted() || tick.modelState != null && ((ModelState) tick.modelState).get_mounted())
        return;
      if (wasPlayerStalled)
      {
        double num = (double) Vector3.Distance((Vector3) tick.position, this.tickInterpolator.EndPoint);
        if (num > 0.00999999977648258)
          AntiHack.ResetTimer(this);
        if (num <= 0.5)
          return;
        this.ClientRPCPlayer<Vector3, uint>((Connection) null, this, "ForcePositionToParentOffset", this.tickInterpolator.EndPoint, this.parentEntity.uid);
      }
      else if ((this.modelState == null || !this.modelState.get_flying() || !this.IsAdmin && !this.IsDeveloper) && (double) Vector3.Distance((Vector3) tick.position, this.tickInterpolator.EndPoint) > 5.0)
      {
        AntiHack.ResetTimer(this);
        this.ClientRPCPlayer<Vector3, uint>((Connection) null, this, "ForcePositionToParentOffset", this.tickInterpolator.EndPoint, this.parentEntity.uid);
      }
      else
      {
        this.tickInterpolator.AddPoint((Vector3) tick.position);
        this.tickNeedsFinalizing = true;
      }
    }
  }

  internal void UpdateRotationFromTick(PlayerTick tick)
  {
    if (tick.inputState == null)
      return;
    if (Vector3Ex.IsNaNOrInfinity((Vector3) ((InputMessage) tick.inputState).aimAngles))
    {
      this.Kick("Kicked: Invalid Rotation");
    }
    else
    {
      this.tickViewAngles = (Vector3) ((InputMessage) tick.inputState).aimAngles;
      this.tickNeedsFinalizing = true;
    }
  }

  public void UpdateEstimatedVelocity(Vector3 lastPos, Vector3 currentPos, float deltaTime)
  {
    this.estimatedVelocity = Vector3.op_Division(Vector3.op_Subtraction(currentPos, lastPos), deltaTime);
    Vector3 estimatedVelocity = this.estimatedVelocity;
    this.estimatedSpeed = ((Vector3) ref estimatedVelocity).get_magnitude();
    this.estimatedSpeed2D = Vector3Ex.Magnitude2D(this.estimatedVelocity);
    if ((double) this.estimatedSpeed < 0.00999999977648258)
      this.estimatedSpeed = 0.0f;
    if ((double) this.estimatedSpeed2D >= 0.00999999977648258)
      return;
    this.estimatedSpeed2D = 0.0f;
  }

  private void FinalizeTick(float deltaTime)
  {
    this.tickDeltaTime += deltaTime;
    if (this.IsReceivingSnapshot || !this.tickNeedsFinalizing)
      return;
    this.tickNeedsFinalizing = false;
    using (TimeWarning.New("ModelState", 0.1f))
    {
      if (this.modelStateTick != null)
      {
        if (this.modelState != null)
        {
          if (this.modelStateTick.get_flying() && !this.IsAdmin && !this.IsDeveloper)
            AntiHack.NoteAdminHack(this);
          if (ConVar.AntiHack.modelstate && this.TriggeredAntiHack(1f, float.PositiveInfinity))
            this.modelStateTick.set_ducked(this.modelState.get_ducked());
          this.modelState.ResetToPool();
          this.modelState = (ModelState) null;
        }
        this.modelState = this.modelStateTick;
        this.modelStateTick = (ModelState) null;
        this.UpdateModelState();
      }
    }
    using (TimeWarning.New("Transform", 0.1f))
    {
      this.UpdateEstimatedVelocity(this.tickInterpolator.StartPoint, this.tickInterpolator.EndPoint, this.tickDeltaTime);
      bool flag1 = Vector3.op_Inequality(this.tickInterpolator.StartPoint, this.tickInterpolator.EndPoint);
      bool flag2 = Vector3.op_Inequality(this.tickViewAngles, this.viewAngles);
      if (flag1)
      {
        if (AntiHack.ValidateMove(this, this.tickInterpolator, this.tickDeltaTime))
        {
          ((Component) this).get_transform().set_localPosition(this.tickInterpolator.EndPoint);
          AntiHack.FadeViolations(this, this.tickDeltaTime);
        }
        else
        {
          flag1 = false;
          if (ConVar.AntiHack.forceposition)
            this.ClientRPCPlayer<Vector3, uint>((Connection) null, this, "ForcePositionToParentOffset", ((Component) this).get_transform().get_localPosition(), this.parentEntity.uid);
        }
      }
      this.tickInterpolator.Reset(((Component) this).get_transform().get_localPosition());
      if (flag2)
      {
        this.viewAngles = this.tickViewAngles;
        ((Component) this).get_transform().set_rotation(Quaternion.get_identity());
        ((Component) this).get_transform().set_hasChanged(true);
      }
      if (flag1 | flag2)
      {
        this.eyes.NetworkUpdate(Quaternion.Euler(this.viewAngles));
        this.NetworkPositionTick();
      }
    }
    using (TimeWarning.New("EACStateUpdate", 0.1f))
      this.EACStateUpdate();
    using (TimeWarning.New("AntiHack.EnforceViolations", 0.1f))
      AntiHack.EnforceViolations(this);
    this.tickDeltaTime = 0.0f;
  }

  private void ForceUpdateTriggersAction()
  {
    if (this.IsDestroyed)
      return;
    this.ForceUpdateTriggers(false, true, false);
  }

  public void ForceUpdateTriggers(bool enter = true, bool exit = true, bool invoke = true)
  {
    List<TriggerBase> list1 = (List<TriggerBase>) Pool.GetList<TriggerBase>();
    List<TriggerBase> list2 = (List<TriggerBase>) Pool.GetList<TriggerBase>();
    if (this.triggers != null)
      list1.AddRange((IEnumerable<TriggerBase>) this.triggers);
    CapsuleCollider component = (CapsuleCollider) ((Component) this).GetComponent<CapsuleCollider>();
    GamePhysics.OverlapCapsule<TriggerBase>(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, this.GetRadius(), 0.0f)), Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, this.GetHeight() - this.GetRadius(), 0.0f)), this.GetRadius(), list2, 262144, (QueryTriggerInteraction) 2);
    if (exit)
    {
      foreach (TriggerBase triggerBase in list1)
      {
        if (!list2.Contains(triggerBase))
          triggerBase.OnTriggerExit((Collider) component);
      }
    }
    if (enter)
    {
      foreach (TriggerBase triggerBase in list2)
      {
        if (!list1.Contains(triggerBase))
          triggerBase.OnTriggerEnter((Collider) component);
        else if (triggerBase is TriggerParent)
          triggerBase.OnEntityEnter((BaseEntity) this);
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<TriggerBase>((List<M0>&) ref list1);
    // ISSUE: cast to a reference type
    Pool.FreeList<TriggerBase>((List<M0>&) ref list2);
    if (!invoke)
      return;
    this.Invoke(new Action(this.ForceUpdateTriggersAction), Time.get_fixedDeltaTime() * 1.5f);
  }

  public bool IsWounded()
  {
    return this.HasPlayerFlag(BasePlayer.PlayerFlags.Wounded);
  }

  public float secondsSinceWoundedStarted
  {
    get
    {
      return Time.get_realtimeSinceStartup() - this.woundedStartTime;
    }
  }

  private bool WoundInsteadOfDying(HitInfo info)
  {
    if (this.IsWounded() || !this.EligibleForWounding(info))
      return false;
    this.lastWoundedTime = Time.get_realtimeSinceStartup();
    this.health = (float) Random.Range(2, 6);
    this.metabolism.bleeding.value = 0.0f;
    this.StartWounded();
    return true;
  }

  public virtual bool EligibleForWounding(HitInfo info)
  {
    object obj = Interface.CallHook("CanBeWounded", (object) this, (object) info);
    if (obj is bool)
      return (bool) obj;
    if (!ConVar.Server.woundingenabled || this.IsSleeping() || (this.isMounted || info == null) || (double) Time.get_realtimeSinceStartup() - (double) this.lastWoundedTime < 60.0)
      return false;
    if (info.WeaponPrefab is BaseMelee)
      return true;
    if (info.WeaponPrefab is BaseProjectile)
      return !info.isHeadshot;
    switch (info.damageTypes.GetMajorityDamageType())
    {
      case DamageType.Hunger:
        return true;
      case DamageType.Thirst:
        return true;
      case DamageType.Bleeding:
        return true;
      case DamageType.Poison:
        return true;
      case DamageType.Suicide:
        return false;
      case DamageType.Fall:
        return true;
      case DamageType.Bite:
        return true;
      default:
        return false;
    }
  }

  public void StartWounded()
  {
    if (this.IsWounded() || Interface.CallHook("OnPlayerWound", (object) this) != null)
      return;
    this.stats.Add("wounded", 1, Stats.Steam);
    this.woundedDuration = Random.Range(40f, 50f);
    this.woundedStartTime = Time.get_realtimeSinceStartup();
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Wounded, true);
    this.EnableServerFall(true);
    this.SendNetworkUpdateImmediate(false);
    this.Invoke(new Action(this.WoundingTick), 1f);
  }

  public void StopWounded()
  {
    if (!this.IsDead() && Interface.CallHook("OnPlayerRecover", (object) this) != null)
      return;
    this.SetPlayerFlag(BasePlayer.PlayerFlags.Wounded, false);
    this.CancelInvoke(new Action(this.WoundingTick));
  }

  public void ProlongWounding(float delay)
  {
    this.woundedDuration = Mathf.Max(this.woundedDuration, Mathf.Min(this.secondsSinceWoundedStarted + delay, this.woundedDuration + delay));
  }

  private void WoundingTick()
  {
    using (TimeWarning.New(nameof (WoundingTick), 0.1f))
    {
      if (this.IsDead())
        return;
      if ((double) this.secondsSinceWoundedStarted >= (double) this.woundedDuration)
      {
        if (Random.Range(0, 100) < 20)
        {
          this.SetPlayerFlag(BasePlayer.PlayerFlags.Wounded, false);
          Interface.CallHook("OnPlayerRecovered", (object) this);
        }
        else
          this.Die((HitInfo) null);
      }
      else
        this.Invoke(new Action(this.WoundingTick), 1f);
    }
  }

  private bool WoundingCausingImmportality()
  {
    return this.IsWounded() && (double) this.secondsSinceWoundedStarted <= 0.25;
  }

  public BasePlayer()
  {
    ModelState modelState = new ModelState();
    modelState.set_onground(true);
    this.modelState = modelState;
    this.firedProjectiles = new Dictionary<int, BasePlayer.FiredProjectile>();
    this.sleepStartTime = -1f;
    this.fallTickRate = 0.1f;
    this.SpectateOffset = 1000000;
    this.spectateFilter = "";
    this.lastUpdateTime = float.NegativeInfinity;
    this.serverInput = new InputState();
    this.lastReceivedTick = new PlayerTick();
    this.tickInterpolator = new TickInterpolator();
    this.lastWoundedTime = float.NegativeInfinity;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public enum CameraMode
  {
    FirstPerson,
    ThirdPerson,
    Eyes,
  }

  public enum NetworkQueue
  {
    Update,
    UpdateDistance,
    Count,
  }

  private class NetworkQueueList
  {
    public HashSet<BaseNetworkable> queueInternal = new HashSet<BaseNetworkable>();
    public int MaxLength;

    public bool Contains(BaseNetworkable ent)
    {
      return this.queueInternal.Contains(ent);
    }

    public void Add(BaseNetworkable ent)
    {
      if (!this.Contains(ent))
        this.queueInternal.Add(ent);
      this.MaxLength = Mathf.Max(this.MaxLength, this.queueInternal.Count);
    }

    public void Add(BaseNetworkable[] ent)
    {
      foreach (BaseNetworkable ent1 in ent)
        this.Add(ent1);
    }

    public int Length
    {
      get
      {
        return this.queueInternal.Count;
      }
    }

    public void Clear(Group group)
    {
      using (TimeWarning.New("NetworkQueueList.Clear", 0.1f))
      {
        if (group != null)
        {
          if (group.get_isGlobal())
            return;
          this.queueInternal.RemoveWhere((Predicate<BaseNetworkable>) (x =>
          {
            if (!Object.op_Equality((Object) x, (Object) null) && x.net != null && x.net.group != null)
              return x.net.group == group;
            return true;
          }));
        }
        else
          this.queueInternal.RemoveWhere((Predicate<BaseNetworkable>) (x =>
          {
            if (!Object.op_Equality((Object) x, (Object) null) && x.net != null && x.net.group != null)
              return !((Group) x.net.group).get_isGlobal();
            return true;
          }));
      }
    }
  }

  [System.Flags]
  public enum PlayerFlags
  {
    Unused1 = 1,
    Unused2 = 2,
    IsAdmin = 4,
    ReceivingSnapshot = 8,
    Sleeping = 16, // 0x00000010
    Spectating = 32, // 0x00000020
    Wounded = 64, // 0x00000040
    IsDeveloper = 128, // 0x00000080
    Connected = 256, // 0x00000100
    VoiceMuted = 512, // 0x00000200
    ThirdPersonViewmode = 1024, // 0x00000400
    EyesViewmode = 2048, // 0x00000800
    ChatMute = 4096, // 0x00001000
    NoSprint = 8192, // 0x00002000
    Aiming = 16384, // 0x00004000
    DisplaySash = 32768, // 0x00008000
    Relaxed = 65536, // 0x00010000
    SafeZone = 131072, // 0x00020000
    ServerFall = 262144, // 0x00040000
    Workbench1 = 1048576, // 0x00100000
    Workbench2 = 2097152, // 0x00200000
    Workbench3 = 4194304, // 0x00400000
  }

  private struct FiredProjectile
  {
    public ItemDefinition itemDef;
    public ItemModProjectile itemMod;
    public Projectile projectilePrefab;
    public float firedTime;
    public float travelTime;
    public float partialTime;
    public AttackEntity weaponSource;
    public AttackEntity weaponPrefab;
    public Projectile.Modifier projectileModifier;
    public Item pickupItem;
    public float integrity;
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 initialPosition;
    public Vector3 initialVelocity;
    public int protection;
  }

  public class SpawnPoint
  {
    public Vector3 pos;
    public Quaternion rot;
  }
}
