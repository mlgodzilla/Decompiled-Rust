// Decompiled with JetBrains decompiler
// Type: AutoTurret
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

public class AutoTurret : StorageContainer
{
  private static float[] visibilityOffsets = new float[3]
  {
    0.0f,
    0.15f,
    -0.15f
  };
  public float bulletSpeed = 200f;
  public float sightRange = 30f;
  public float focusSoundFreqMin = 2.5f;
  public float focusSoundFreqMax = 7f;
  public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();
  private bool targetVisible = true;
  private Vector3 targetAimDir = Vector3.get_forward();
  private Vector3 lastSentAimDir = Vector3.get_zero();
  public GameObjectRef gun_fire_effect;
  public GameObjectRef bulletEffect;
  public AmbienceEmitter ambienceEmitter;
  public BaseCombatEntity target;
  public Transform eyePos;
  public Transform muzzlePos;
  public Vector3 aimDir;
  public Transform gun_yaw;
  public Transform gun_pitch;
  public SoundDefinition turnLoopDef;
  public SoundDefinition movementChangeDef;
  public SoundDefinition ambientLoopDef;
  public SoundDefinition focusCameraDef;
  public GameObjectRef peacekeeperToggleSound;
  public GameObjectRef onlineSound;
  public GameObjectRef offlineSound;
  public GameObjectRef targetAcquiredEffect;
  public GameObjectRef targetLostEffect;
  public float aimCone;
  public ItemDefinition ammoType;
  public TargetTrigger targetTrigger;
  private float nextShotTime;
  private float nextVisCheck;
  public float lastTargetSeenTime;
  private bool booting;
  private float nextIdleAimTime;
  private Item ammoItem;
  public const float bulletDamage = 15f;
  private float nextForcedAimTime;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("AutoTurret.OnRpcMessage", 0.1f))
    {
      if (rpc == 1092560690U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - AddSelfAuthorize "));
        using (TimeWarning.New("AddSelfAuthorize", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("AddSelfAuthorize", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.AddSelfAuthorize(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in AddSelfAuthorize");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 253307592U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - ClearList "));
        using (TimeWarning.New("ClearList", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("ClearList", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.ClearList(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in ClearList");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1500257773U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - FlipAim "));
        using (TimeWarning.New("FlipAim", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("FlipAim", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.FlipAim(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in FlipAim");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3617985969U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RemoveSelfAuthorize "));
        using (TimeWarning.New("RemoveSelfAuthorize", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("RemoveSelfAuthorize", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RemoveSelfAuthorize(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RemoveSelfAuthorize");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1770263114U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SERVER_AttackAll "));
        using (TimeWarning.New("SERVER_AttackAll", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_AttackAll", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.SERVER_AttackAll(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in SERVER_AttackAll");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3265538831U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SERVER_Peacekeeper "));
        using (TimeWarning.New("SERVER_Peacekeeper", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_Peacekeeper", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.SERVER_Peacekeeper(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in SERVER_Peacekeeper");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 190717079U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SERVER_TurnOff "));
        using (TimeWarning.New("SERVER_TurnOff", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_TurnOff", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.SERVER_TurnOff(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in SERVER_TurnOff");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 2543523181U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - SERVER_TurnOn "));
          using (TimeWarning.New("SERVER_TurnOn", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_TurnOn", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.SERVER_TurnOn(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in SERVER_TurnOn");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public bool IsOnline()
  {
    return this.IsOn();
  }

  public bool IsOffline()
  {
    return !this.IsOnline();
  }

  public virtual Transform GetCenterMuzzle()
  {
    return this.muzzlePos;
  }

  public float AngleToTarget(BaseCombatEntity potentialtarget)
  {
    Transform centerMuzzle = this.GetCenterMuzzle();
    Vector3 vector3 = Vector3.op_Subtraction(this.AimOffset(potentialtarget), centerMuzzle.get_position());
    return Vector3.Angle(centerMuzzle.get_forward(), ((Vector3) ref vector3).get_normalized());
  }

  public virtual bool InFiringArc(BaseCombatEntity potentialtarget)
  {
    return (double) this.AngleToTarget(potentialtarget) <= 90.0;
  }

  public override bool CanPickup(BasePlayer player)
  {
    if (base.CanPickup(player) && this.IsOffline())
      return this.IsAuthed(player);
    return false;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.autoturret = (__Null) Pool.Get<AutoTurret>();
    ((AutoTurret) info.msg.autoturret).users = (__Null) this.authorizedPlayers;
  }

  public override void PostSave(BaseNetworkable.SaveInfo info)
  {
    base.PostSave(info);
    ((AutoTurret) info.msg.autoturret).users = null;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.autoturret == null)
      return;
    this.authorizedPlayers = (List<PlayerNameID>) ((AutoTurret) info.msg.autoturret).users;
    ((AutoTurret) info.msg.autoturret).users = null;
  }

  public Vector3 AimOffset(BaseCombatEntity aimat)
  {
    BasePlayer basePlayer = aimat as BasePlayer;
    if (!Object.op_Inequality((Object) basePlayer, (Object) null))
      return Vector3.op_Addition(((Component) aimat).get_transform().get_position(), new Vector3(0.0f, 0.3f, 0.0f));
    if (basePlayer.IsSleeping())
      return Vector3.op_Addition(((Component) basePlayer).get_transform().get_position(), Vector3.op_Multiply(Vector3.get_up(), 0.1f));
    return basePlayer.eyes.position;
  }

  public float GetAimSpeed()
  {
    return this.HasTarget() ? 5f : 1f;
  }

  public void UpdateAiming()
  {
    if (Vector3.op_Equality(this.aimDir, Vector3.get_zero()))
      return;
    float num = this.isServer ? 16f : 5f;
    Quaternion quaternion1 = Quaternion.LookRotation(this.aimDir);
    Quaternion quaternion2 = Quaternion.Euler(0.0f, (float) ((Quaternion) ref quaternion1).get_eulerAngles().y, 0.0f);
    Quaternion quaternion3 = Quaternion.Euler((float) ((Quaternion) ref quaternion1).get_eulerAngles().x, 0.0f, 0.0f);
    if (Quaternion.op_Inequality(((Component) this.gun_yaw).get_transform().get_rotation(), quaternion2))
      ((Component) this.gun_yaw).get_transform().set_rotation(Quaternion.Lerp(((Component) this.gun_yaw).get_transform().get_rotation(), quaternion2, Time.get_deltaTime() * num));
    if (!Quaternion.op_Inequality(((Component) this.gun_pitch).get_transform().get_localRotation(), quaternion3))
      return;
    ((Component) this.gun_pitch).get_transform().set_localRotation(Quaternion.Lerp(((Component) this.gun_pitch).get_transform().get_localRotation(), quaternion3, Time.get_deltaTime() * num));
  }

  public bool IsAuthed(BasePlayer player)
  {
    return ((IEnumerable<PlayerNameID>) this.authorizedPlayers).Any<PlayerNameID>((Func<PlayerNameID, bool>) (x => x.userid == (long) player.userID));
  }

  public bool AnyAuthed()
  {
    return this.authorizedPlayers.Count > 0;
  }

  public virtual bool CanChangeSettings(BasePlayer player)
  {
    if (this.IsAuthed(player))
      return this.IsOffline();
    return false;
  }

  public bool PeacekeeperMode()
  {
    return this.HasFlag(BaseEntity.Flags.Reserved1);
  }

  public void SetOnline()
  {
    this.SetIsOnline(true);
  }

  public void SetIsOnline(bool online)
  {
    if (online == this.HasFlag(BaseEntity.Flags.On) || Interface.CallHook("OnTurretToggle", (object) this) != null)
      return;
    this.SetFlag(BaseEntity.Flags.On, online, false, true);
    this.booting = false;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    if (this.IsOffline())
    {
      this.SetTarget((BaseCombatEntity) null);
      this.isLootable = true;
    }
    else
      this.isLootable = false;
  }

  public void InitiateShutdown()
  {
    if (this.IsOffline() || Interface.CallHook("OnTurretShutdown", (object) this) != null)
      return;
    Effect.server.Run(this.offlineSound.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    this.SetIsOnline(false);
  }

  public void InitiateStartup()
  {
    if (this.IsOnline() || this.booting || Interface.CallHook("OnTurretStartup", (object) this) != null)
      return;
    Effect.server.Run(this.onlineSound.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    this.Invoke(new Action(this.SetOnline), 2f);
    this.booting = true;
  }

  public void SetPeacekeepermode(bool isOn)
  {
    if (this.PeacekeeperMode() == isOn)
      return;
    this.SetFlag(BaseEntity.Flags.Reserved1, isOn, false, true);
    Effect.server.Run(this.peacekeeperToggleSound.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    Interface.CallHook("OnTurretModeToggle", (object) this);
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  private void FlipAim(BaseEntity.RPCMessage rpc)
  {
    if (this.IsOnline() || !this.IsAuthed(rpc.player) || this.booting)
      return;
    ((Component) this).get_transform().set_rotation(Quaternion.LookRotation(Vector3.op_UnaryNegation(((Component) this).get_transform().get_forward()), ((Component) this).get_transform().get_up()));
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  private void AddSelfAuthorize(BaseEntity.RPCMessage rpc)
  {
    if (this.IsOnline() || Interface.CallHook("OnTurretAuthorize", (object) this, (object) rpc.player) != null)
      return;
    this.authorizedPlayers.RemoveAll((Predicate<PlayerNameID>) (x => x.userid == (long) rpc.player.userID));
    this.authorizedPlayers.Add(new PlayerNameID()
    {
      userid = (__Null) (long) rpc.player.userID,
      username = (__Null) rpc.player.displayName
    });
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  private void RemoveSelfAuthorize(BaseEntity.RPCMessage rpc)
  {
    if (this.booting || this.IsOnline() || (!this.IsAuthed(rpc.player) || Interface.CallHook("OnTurretDeauthorize", (object) this, (object) rpc.player) != null))
      return;
    this.authorizedPlayers.RemoveAll((Predicate<PlayerNameID>) (x => x.userid == (long) rpc.player.userID));
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  private void ClearList(BaseEntity.RPCMessage rpc)
  {
    if (this.booting || this.IsOnline() || (!this.IsAuthed(rpc.player) || Interface.CallHook("OnTurretClearList", (object) this, (object) rpc.player) != null))
      return;
    this.authorizedPlayers.Clear();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.IsVisible(3f)]
  private void SERVER_TurnOn(BaseEntity.RPCMessage rpc)
  {
    if (!this.IsAuthed(rpc.player))
      return;
    this.InitiateStartup();
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  private void SERVER_TurnOff(BaseEntity.RPCMessage rpc)
  {
    if (!this.IsAuthed(rpc.player))
      return;
    this.InitiateShutdown();
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  private void SERVER_Peacekeeper(BaseEntity.RPCMessage rpc)
  {
    if (!this.IsAuthed(rpc.player))
      return;
    this.SetPeacekeepermode(true);
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  private void SERVER_AttackAll(BaseEntity.RPCMessage rpc)
  {
    if (!this.IsAuthed(rpc.player))
      return;
    this.SetPeacekeepermode(false);
  }

  public virtual float TargetScanRate()
  {
    return 1f;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRepeating(new Action(this.ServerTick), Random.Range(0.0f, 1f), 0.015f);
    this.InvokeRandomized(new Action(this.SendAimDir), Random.Range(0.0f, 1f), 0.2f, 0.05f);
    this.InvokeRandomized(new Action(this.TargetScan), Random.Range(0.0f, 1f), this.TargetScanRate(), 0.2f);
    ((SphereCollider) ((Component) this.targetTrigger).GetComponent<SphereCollider>()).set_radius(this.sightRange);
  }

  public void SendAimDir()
  {
    if ((double) Time.get_realtimeSinceStartup() <= (double) this.nextForcedAimTime && !this.HasTarget() && (double) Vector3.Angle(this.lastSentAimDir, this.aimDir) <= 0.0299999993294477)
      return;
    this.lastSentAimDir = this.aimDir;
    this.ClientRPC<Vector3>((Connection) null, "CLIENT_ReceiveAimDir", this.aimDir);
    this.nextForcedAimTime = Time.get_realtimeSinceStartup() + 2f;
  }

  public void SetTarget(BaseCombatEntity targ)
  {
    if (Interface.CallHook("OnTurretTarget", (object) this, (object) targ) != null)
      return;
    if (Object.op_Inequality((Object) targ, (Object) this.target))
      Effect.server.Run(Object.op_Equality((Object) targ, (Object) null) ? this.targetLostEffect.resourcePath : this.targetAcquiredEffect.resourcePath, ((Component) this).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    this.target = targ;
  }

  public virtual bool CheckPeekers()
  {
    return true;
  }

  public bool ObjectVisible(BaseCombatEntity obj)
  {
    object obj1 = Interface.CallHook("CanBeTargeted", (object) obj, (object) this);
    if (obj1 is bool)
      return (bool) obj1;
    List<RaycastHit> list = (List<RaycastHit>) Pool.GetList<RaycastHit>();
    Vector3 position = ((Component) this.eyePos).get_transform().get_position();
    Vector3 vector3_1 = this.AimOffset(obj);
    float num = Vector3.Distance(vector3_1, position);
    Vector3 vector3_2 = Vector3.op_Subtraction(vector3_1, position);
    Vector3 vector3_3 = Vector3.Cross(((Vector3) ref vector3_2).get_normalized(), Vector3.get_up());
    for (int index1 = 0; (double) index1 < (this.CheckPeekers() ? 3.0 : 1.0); ++index1)
    {
      Vector3 vector3_4 = Vector3.op_Subtraction(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(vector3_3, AutoTurret.visibilityOffsets[index1])), position);
      Vector3 normalized = ((Vector3) ref vector3_4).get_normalized();
      list.Clear();
      GamePhysics.TraceAll(new Ray(position, normalized), 0.0f, list, num * 1.1f, 1218652417, (QueryTriggerInteraction) 0);
      for (int index2 = 0; index2 < list.Count; ++index2)
      {
        BaseEntity entity = list[index2].GetEntity();
        if ((!Object.op_Inequality((Object) entity, (Object) null) || !entity.isClient) && (!Object.op_Inequality((Object) entity, (Object) null) || !Object.op_Inequality((Object) entity.ToPlayer(), (Object) null) || entity.EqualNetID((BaseNetworkable) obj)) && (!Object.op_Inequality((Object) entity, (Object) null) || !entity.EqualNetID((BaseNetworkable) this)))
        {
          if (Object.op_Inequality((Object) entity, (Object) null) && (Object.op_Equality((Object) entity, (Object) obj) || entity.EqualNetID((BaseNetworkable) obj)))
          {
            // ISSUE: cast to a reference type
            Pool.FreeList<RaycastHit>((List<M0>&) ref list);
            return true;
          }
          if (!Object.op_Inequality((Object) entity, (Object) null) || entity.ShouldBlockProjectiles())
            break;
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<RaycastHit>((List<M0>&) ref list);
    return false;
  }

  public virtual void FireGun(
    Vector3 targetPos,
    float aimCone,
    Transform muzzleToUse = null,
    BaseCombatEntity target = null)
  {
    if (this.IsOffline())
      return;
    if (Object.op_Equality((Object) muzzleToUse, (Object) null))
      muzzleToUse = this.muzzlePos;
    Vector3 vector3_1 = Vector3.op_Subtraction(((Component) this.GetCenterMuzzle()).get_transform().get_position(), Vector3.op_Multiply(this.GetCenterMuzzle().get_forward(), 0.25f));
    Vector3 forward = ((Component) this.GetCenterMuzzle()).get_transform().get_forward();
    Vector3 aimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, forward, true);
    targetPos = Vector3.op_Addition(vector3_1, Vector3.op_Multiply(aimConeDirection, 300f));
    List<RaycastHit> list = (List<RaycastHit>) Pool.GetList<RaycastHit>();
    GamePhysics.TraceAll(new Ray(vector3_1, aimConeDirection), 0.0f, list, 300f, 1219701521, (QueryTriggerInteraction) 0);
    for (int index = 0; index < list.Count; ++index)
    {
      RaycastHit hit = list[index];
      BaseEntity entity1 = hit.GetEntity();
      if ((!Object.op_Inequality((Object) entity1, (Object) null) || !Object.op_Equality((Object) entity1, (Object) this) && !entity1.EqualNetID((BaseNetworkable) this)) && (!Object.op_Inequality((Object) target, (Object) null) || !Object.op_Inequality((Object) entity1, (Object) null) || (!Object.op_Inequality((Object) ((Component) entity1).GetComponent<BasePlayer>(), (Object) null) || entity1.EqualNetID((BaseNetworkable) target))))
      {
        BaseCombatEntity entity2 = entity1 as BaseCombatEntity;
        if (Object.op_Inequality((Object) entity2, (Object) null))
          this.ApplyDamage(entity2, ((RaycastHit) ref hit).get_point(), aimConeDirection);
        if (!Object.op_Inequality((Object) entity1, (Object) null) || entity1.ShouldBlockProjectiles())
        {
          targetPos = ((RaycastHit) ref hit).get_point();
          Vector3 vector3_2 = Vector3.op_Subtraction(targetPos, vector3_1);
          ((Vector3) ref vector3_2).get_normalized();
          break;
        }
      }
    }
    this.ClientRPC<uint, Vector3>((Connection) null, "CLIENT_FireGun", StringPool.Get(((Object) ((Component) muzzleToUse).get_gameObject()).get_name()), targetPos);
    // ISSUE: cast to a reference type
    Pool.FreeList<RaycastHit>((List<M0>&) ref list);
  }

  private void ApplyDamage(BaseCombatEntity entity, Vector3 point, Vector3 normal)
  {
    float damageAmount = 15f * Random.Range(0.9f, 1.1f);
    if (entity is BasePlayer && Object.op_Inequality((Object) entity, (Object) this.target))
      damageAmount *= 0.5f;
    if (this.PeacekeeperMode() && Object.op_Equality((Object) entity, (Object) this.target))
      this.target.MarkHostileFor(1800f);
    HitInfo info = new HitInfo((BaseEntity) this, (BaseEntity) entity, DamageType.Bullet, damageAmount, point);
    entity.OnAttacked(info);
    if (!(entity is BasePlayer) && !(entity is BaseNpc))
      return;
    Effect.server.ImpactEffect(new HitInfo()
    {
      HitPositionWorld = point,
      HitNormalWorld = Vector3.op_UnaryNegation(normal),
      HitMaterial = StringPool.Get("Flesh")
    });
  }

  public void IdleTick()
  {
    if ((double) Time.get_realtimeSinceStartup() > (double) this.nextIdleAimTime)
    {
      this.nextIdleAimTime = Time.get_realtimeSinceStartup() + Random.Range(4f, 5f);
      this.targetAimDir = Quaternion.op_Multiply(Quaternion.op_Multiply(Quaternion.LookRotation(this.eyePos.get_forward(), Vector3.get_up()), Quaternion.AngleAxis(Random.Range(-45f, 45f), Vector3.get_up())), Vector3.get_forward());
    }
    if (this.HasTarget())
      return;
    this.aimDir = Vector3.Lerp(this.aimDir, this.targetAimDir, Time.get_deltaTime() * 2f);
  }

  public virtual bool HasAmmo()
  {
    if (this.ammoItem != null && this.ammoItem.amount > 0)
      return this.ammoItem.parent == this.inventory;
    return false;
  }

  public void Reload()
  {
    foreach (Item obj in this.inventory.itemList)
    {
      if (obj.info.itemid == this.ammoType.itemid && obj.amount > 0)
      {
        this.ammoItem = obj;
        break;
      }
    }
  }

  public void EnsureReloaded()
  {
    if (this.HasAmmo())
      return;
    this.Reload();
  }

  public override void PlayerStoppedLooting(BasePlayer player)
  {
    base.PlayerStoppedLooting(player);
    this.EnsureReloaded();
    this.nextShotTime = Time.get_time();
  }

  public void TargetTick()
  {
    if ((double) Time.get_realtimeSinceStartup() >= (double) this.nextVisCheck)
    {
      this.nextVisCheck = Time.get_realtimeSinceStartup() + Random.Range(0.2f, 0.3f);
      this.targetVisible = this.ObjectVisible(this.target);
      if (this.targetVisible)
        this.lastTargetSeenTime = Time.get_realtimeSinceStartup();
    }
    if ((double) Time.get_time() >= (double) this.nextShotTime && this.targetVisible && (double) this.AngleToTarget(this.target) < 10.0)
    {
      this.EnsureReloaded();
      if (this.HasAmmo())
      {
        this.FireGun(this.AimOffset(this.target), this.aimCone, (Transform) null, this.PeacekeeperMode() ? this.target : (BaseCombatEntity) null);
        this.nextShotTime = Time.get_time() + 0.115f;
        if (this.ammoItem != null)
          this.ammoItem.UseItem(1);
      }
      else
        this.nextShotTime = Time.get_time() + 60f;
    }
    if (!this.target.IsDead() && (double) Time.get_realtimeSinceStartup() - (double) this.lastTargetSeenTime <= 3.0 && (double) Vector3.Distance(((Component) this).get_transform().get_position(), ((Component) this.target).get_transform().get_position()) <= (double) this.sightRange && (!this.PeacekeeperMode() || this.IsEntityHostile(this.target)))
      return;
    this.SetTarget((BaseCombatEntity) null);
  }

  public bool HasTarget()
  {
    if (Object.op_Inequality((Object) this.target, (Object) null))
      return this.target.IsAlive();
    return false;
  }

  public void OfflineTick()
  {
    this.aimDir = Vector3.get_up();
  }

  public virtual bool IsEntityHostile(BaseCombatEntity ent)
  {
    return ent.IsHostile();
  }

  public void TargetScan()
  {
    if (this.HasTarget() || this.IsOffline() || this.targetTrigger.entityContents == null)
      return;
    foreach (BaseEntity entityContent in this.targetTrigger.entityContents)
    {
      if (!Object.op_Equality((Object) entityContent, (Object) null))
      {
        BaseCombatEntity component = (BaseCombatEntity) ((Component) entityContent).GetComponent<BaseCombatEntity>();
        if (!Object.op_Equality((Object) component, (Object) null) && component.IsAlive() && (this.InFiringArc(component) && this.ObjectVisible(component)))
        {
          if (!Sentry.targetall)
          {
            BasePlayer player = component as BasePlayer;
            if (Object.op_Implicit((Object) player) && (this.IsAuthed(player) || this.Ignore(player)))
              continue;
          }
          if (!(component is AutoTurret))
          {
            if (this.PeacekeeperMode())
            {
              if (this.IsEntityHostile(component))
              {
                if (Object.op_Equality((Object) this.target, (Object) null))
                  this.nextShotTime = Time.get_time() + 1f;
              }
              else
                continue;
            }
            this.SetTarget(component);
            break;
          }
        }
      }
    }
  }

  protected virtual bool Ignore(BasePlayer player)
  {
    return false;
  }

  public void ServerTick()
  {
    if (this.isClient || this.IsDestroyed)
      return;
    if (!this.IsOnline())
      this.OfflineTick();
    else if (this.HasTarget())
      this.TargetTick();
    else
      this.IdleTick();
    this.UpdateFacingToTarget();
  }

  public override void OnAttacked(HitInfo info)
  {
    base.OnAttacked(info);
    if ((!this.IsOnline() || this.HasTarget()) && this.targetVisible || Object.op_Inequality((Object) (info.Initiator as AutoTurret), (Object) null))
      return;
    BasePlayer initiator = info.Initiator as BasePlayer;
    if (Object.op_Implicit((Object) initiator) && this.IsAuthed(initiator))
      return;
    this.SetTarget(info.Initiator as BaseCombatEntity);
  }

  public void UpdateFacingToTarget()
  {
    if (Object.op_Inequality((Object) this.target, (Object) null) && this.targetVisible)
    {
      Vector3 vector3_1 = this.AimOffset(this.target);
      Vector3 vector3_2 = ((Component) this.gun_pitch).get_transform().InverseTransformPoint(((Component) this.muzzlePos).get_transform().get_position());
      vector3_2.z = (__Null) (double) (vector3_2.x = (__Null) 0.0f);
      Vector3 vector3_3 = Vector3.op_Addition(this.gun_pitch.get_position(), vector3_2);
      this.aimDir = Vector3.op_Subtraction(vector3_1, vector3_3);
    }
    this.UpdateAiming();
  }

  public static class TurretFlags
  {
    public const BaseEntity.Flags Peacekeeper = BaseEntity.Flags.Reserved1;
  }
}
