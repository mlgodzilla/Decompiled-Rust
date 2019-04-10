// Decompiled with JetBrains decompiler
// Type: BaseMountable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseMountable : BaseCombatEntity
{
  public static readonly Vector3 DISMOUNT_POS_INVALID = new Vector3(0.0f, 5000f, 0.0f);
  public Vector2 pitchClamp = new Vector2(-80f, 50f);
  public Vector2 yawClamp = new Vector2(-80f, 80f);
  public bool canWieldItems = true;
  public float maxMountDistance = 1.5f;
  public BasePlayer _mounted;
  [Header("View")]
  public Transform eyeOverride;
  [Header("Mounting")]
  public PlayerModel.MountPoses mountPose;
  public Transform mountAnchor;
  public Transform dismountAnchor;
  public Transform[] dismountPositions;
  public Transform dismountCheckEyes;
  public SoundDefinition mountSoundDef;
  public SoundDefinition dismountSoundDef;
  public bool isMobile;
  public const float playerHeight = 1.8f;
  public const float playerRadius = 0.5f;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BaseMountable.OnRpcMessage", 0.1f))
    {
      if (rpc == 1735799362U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_WantsDismount "));
        using (TimeWarning.New("RPC_WantsDismount", 0.1f))
        {
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_WantsDismount(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_WantsDismount");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 4014300952U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_WantsMount "));
          using (TimeWarning.New("RPC_WantsMount", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_WantsMount", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_WantsMount(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_WantsMount");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public virtual bool CanHoldItems()
  {
    return this.canWieldItems;
  }

  public virtual bool DirectlyMountable()
  {
    return true;
  }

  public virtual Vector3 EyePositionForPlayer(BasePlayer player)
  {
    if (Object.op_Equality((Object) player.GetMounted(), (Object) this))
      return ((Component) this.eyeOverride).get_transform().get_position();
    return Vector3.get_zero();
  }

  public virtual float WaterFactorForPlayer(BasePlayer player)
  {
    OBB obb = player.WorldSpaceBounds();
    return WaterLevel.Factor(((OBB) ref obb).ToBounds());
  }

  public override float MaxVelocity()
  {
    BaseEntity parentEntity = this.GetParentEntity();
    if (Object.op_Implicit((Object) parentEntity))
      return parentEntity.MaxVelocity();
    return base.MaxVelocity();
  }

  public BaseVehicle VehicleParent()
  {
    return this.GetParentEntity() as BaseVehicle;
  }

  public override void PostServerLoad()
  {
    base.PostServerLoad();
    this.SetFlag(BaseEntity.Flags.Busy, false, false, true);
  }

  public virtual void MounteeTookDamage(BasePlayer mountee, HitInfo info)
  {
  }

  public virtual float GetSteering(BasePlayer player)
  {
    return 0.0f;
  }

  public virtual void LightToggle(BasePlayer player)
  {
  }

  public virtual bool IsMounted()
  {
    return Object.op_Inequality((Object) this._mounted, (Object) null);
  }

  public BasePlayer GetMounted()
  {
    return this._mounted;
  }

  protected override float PositionTickRate
  {
    get
    {
      return 0.05f;
    }
  }

  public override bool CanPickup(BasePlayer player)
  {
    if (base.CanPickup(player))
      return !this.IsMounted();
    return false;
  }

  public override void OnKilled(HitInfo info)
  {
    this.DismountAllPlayers();
    base.OnKilled(info);
  }

  [BaseEntity.RPC_Server.IsVisible(3f)]
  [BaseEntity.RPC_Server]
  public void RPC_WantsMount(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.DirectlyMountable())
      return;
    this.AttemptMount(player);
  }

  public virtual void AttemptMount(BasePlayer player)
  {
    if (Object.op_Inequality((Object) this._mounted, (Object) null) || !this.HasValidDismountPosition(player))
      return;
    this.MountPlayer(player);
  }

  public virtual bool AttemptDismount(BasePlayer player)
  {
    if (Object.op_Inequality((Object) player, (Object) this._mounted))
      return false;
    this.DismountPlayer(player, false);
    return true;
  }

  [BaseEntity.RPC_Server]
  public void RPC_WantsDismount(BaseEntity.RPCMessage msg)
  {
    BasePlayer player = msg.player;
    if (!this.HasValidDismountPosition(player))
      return;
    this.AttemptDismount(player);
  }

  public void MountPlayer(BasePlayer player)
  {
    if (Object.op_Inequality((Object) this._mounted, (Object) null) || Interface.CallHook("CanMountEntity", (object) player, (object) this) != null)
      return;
    player.EnsureDismounted();
    this._mounted = player;
    TriggerParent trigger = player.FindTrigger<TriggerParent>();
    if (Object.op_Implicit((Object) trigger))
      trigger.OnTriggerExit((Collider) ((Component) player).GetComponent<Collider>());
    player.MountObject(this, 0);
    player.MovePosition(((Component) this.mountAnchor).get_transform().get_position());
    ((Component) player).get_transform().set_rotation(((Component) this.mountAnchor).get_transform().get_rotation());
    player.ServerRotation = ((Component) this.mountAnchor).get_transform().get_rotation();
    BasePlayer basePlayer = player;
    Quaternion rotation = ((Component) this.mountAnchor).get_transform().get_rotation();
    Vector3 eulerAngles = ((Quaternion) ref rotation).get_eulerAngles();
    basePlayer.OverrideViewAngles(eulerAngles);
    this._mounted.eyes.NetworkUpdate(((Component) this.mountAnchor).get_transform().get_rotation());
    player.ClientRPCPlayer<Vector3>((Connection) null, player, "ForcePositionTo", ((Component) player).get_transform().get_position());
    this.SetFlag(BaseEntity.Flags.Busy, true, false, true);
    Interface.CallHook("OnEntityMounted", (object) this, (object) player);
  }

  public virtual void DismountAllPlayers()
  {
    if (!Object.op_Implicit((Object) this._mounted))
      return;
    this.DismountPlayer(this._mounted, false);
  }

  public void DismountPlayer(BasePlayer player, bool lite = false)
  {
    if (Object.op_Equality((Object) this._mounted, (Object) null) || Object.op_Inequality((Object) this._mounted, (Object) player) || Interface.CallHook("CanDismountEntity", (object) player, (object) this) != null)
      return;
    if (lite)
    {
      this._mounted.DismountObject();
      this._mounted = (BasePlayer) null;
      this.SetFlag(BaseEntity.Flags.Busy, false, false, true);
    }
    else
    {
      Vector3 dismountPosition = this.GetDismountPosition(player);
      if (Vector3.op_Equality(dismountPosition, BaseMountable.DISMOUNT_POS_INVALID))
      {
        Vector3 position = ((Component) player).get_transform().get_position();
        this._mounted.DismountObject();
        this._mounted.MovePosition(position);
        this._mounted.ClientRPCPlayer<Vector3>((Connection) null, this._mounted, "ForcePositionTo", position);
        BasePlayer mounted = this._mounted;
        this._mounted = (BasePlayer) null;
        Debug.LogWarning((object) ("Killing player due to invalid dismount point :" + player.displayName + " / " + (object) player.userID + " on obj : " + ((Object) ((Component) this).get_gameObject()).get_name()));
        mounted.Hurt(1000f, DamageType.Suicide, (BaseEntity) mounted, false);
        this.SetFlag(BaseEntity.Flags.Busy, false, false, true);
      }
      else
      {
        this._mounted.DismountObject();
        ((Component) this._mounted).get_transform().set_rotation(Quaternion.LookRotation(Vector3.get_forward(), Vector3.get_up()));
        this._mounted.MovePosition(dismountPosition);
        this._mounted.SendNetworkUpdateImmediate(false);
        this._mounted = (BasePlayer) null;
        this.SetFlag(BaseEntity.Flags.Busy, false, false, true);
        player.ForceUpdateTriggers(true, true, true);
        if (Object.op_Implicit((Object) player.GetParentEntity()))
        {
          BaseEntity parentEntity = player.GetParentEntity();
          player.ClientRPCPlayer<Vector3, uint>((Connection) null, player, "ForcePositionToParentOffset", ((Component) parentEntity).get_transform().InverseTransformPoint(dismountPosition), (uint) parentEntity.net.ID);
        }
        else
        {
          player.ClientRPCPlayer<Vector3>((Connection) null, player, "ForcePositionTo", dismountPosition);
          Interface.CallHook("OnEntityDismounted", (object) this, (object) player);
        }
      }
    }
  }

  public Vector3 DismountVisCheckOrigin()
  {
    if (Object.op_Equality((Object) this.dismountCheckEyes, (Object) null))
      return Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 0.3f, 0.0f));
    return this.dismountCheckEyes.get_position();
  }

  public virtual bool ValidDismountPosition(Vector3 disPos)
  {
    if (!Physics.CheckCapsule(Vector3.op_Addition(disPos, new Vector3(0.0f, 0.5f, 0.0f)), Vector3.op_Addition(disPos, new Vector3(0.0f, 1.3f, 0.0f)), 0.5f, 1537286401))
    {
      Vector3 vector3 = this.DismountVisCheckOrigin();
      Vector3 position = Vector3.op_Addition(disPos, Vector3.op_Multiply(((Component) this).get_transform().get_up(), 0.5f));
      if (this.IsVisible(position, float.PositiveInfinity) && !Physics.Linecast(vector3, position, 1486946561))
        return true;
    }
    return false;
  }

  public virtual bool HasValidDismountPosition(BasePlayer player)
  {
    BaseVehicle baseVehicle = this.VehicleParent();
    if (Object.op_Inequality((Object) baseVehicle, (Object) null))
      return baseVehicle.HasValidDismountPosition(player);
    foreach (Component dismountPosition in this.dismountPositions)
    {
      if (this.ValidDismountPosition(dismountPosition.get_transform().get_position()))
        return true;
    }
    return false;
  }

  public virtual Vector3 GetDismountPosition(BasePlayer player)
  {
    BaseVehicle baseVehicle = this.VehicleParent();
    if (Object.op_Inequality((Object) baseVehicle, (Object) null))
      return baseVehicle.GetDismountPosition(player);
    int num = 0;
    foreach (Transform dismountPosition in this.dismountPositions)
    {
      if (this.ValidDismountPosition(((Component) dismountPosition).get_transform().get_position()))
        return ((Component) dismountPosition).get_transform().get_position();
      ++num;
    }
    Debug.LogWarning((object) ("Failed to find dismount position for player :" + player.displayName + " / " + (object) player.userID + " on obj : " + ((Object) ((Component) this).get_gameObject()).get_name()));
    return BaseMountable.DISMOUNT_POS_INVALID;
  }

  public override void ServerInit()
  {
    base.ServerInit();
  }

  public void FixedUpdate()
  {
    if (this.isClient || !this.isMobile)
      return;
    this.VehicleFixedUpdate();
    if (!Object.op_Implicit((Object) this._mounted))
      return;
    ((Component) this._mounted).get_transform().set_rotation(((Component) this.mountAnchor).get_transform().get_rotation());
    this._mounted.ServerRotation = ((Component) this.mountAnchor).get_transform().get_rotation();
    this._mounted.MovePosition(((Component) this.mountAnchor).get_transform().get_position());
  }

  public virtual void VehicleFixedUpdate()
  {
  }

  public virtual void PlayerServerInput(InputState inputState, BasePlayer player)
  {
    Object.op_Inequality((Object) player, (Object) this._mounted);
  }

  public virtual float GetComfort()
  {
    return 0.0f;
  }

  public bool NearMountPoint(BasePlayer player)
  {
    RaycastHit hit;
    return (double) Vector3.Distance(((Component) player).get_transform().get_position(), this.mountAnchor.get_position()) <= (double) this.maxMountDistance && Physics.SphereCast(player.eyes.HeadRay(), 0.25f, ref hit, 2f, 1218652417) && (Object.op_Implicit((Object) hit.GetEntity()) && hit.GetEntity().net.ID == this.net.ID);
  }

  public static Vector3 ConvertVector(Vector3 vec)
  {
    for (int index = 0; index < 3; ++index)
    {
      if ((double) ((Vector3) ref vec).get_Item(index) > 180.0)
      {
        ref Vector3 local = ref vec;
        int num = index;
        ((Vector3) ref local).set_Item(num, ((Vector3) ref local).get_Item(num) - 360f);
      }
      else if ((double) ((Vector3) ref vec).get_Item(index) < -180.0)
      {
        ref Vector3 local = ref vec;
        int num = index;
        ((Vector3) ref local).set_Item(num, ((Vector3) ref local).get_Item(num) + 360f);
      }
    }
    return vec;
  }
}
