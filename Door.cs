// Decompiled with JetBrains decompiler
// Type: Door
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class Door : AnimatedBuildingBlock
{
  private static int nonWalkableArea = -1;
  private static int animalAgentTypeId = -1;
  private static int humanoidAgentTypeId = -1;
  public bool canTakeLock = true;
  public bool canNpcOpen = true;
  public bool canHandOpen = true;
  private float decayResetTimeLast = float.NegativeInfinity;
  private float nextKnockTime = float.NegativeInfinity;
  public GameObjectRef knockEffect;
  public bool hasHatch;
  public bool canTakeCloser;
  public bool canTakeKnocker;
  public bool isSecurityDoor;
  public NavMeshModifierVolume NavMeshVolumeAnimals;
  public NavMeshModifierVolume NavMeshVolumeHumanoids;
  public NavMeshLink NavMeshLink;
  public NPCDoorTriggerBox NpcTriggerBox;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Door.OnRpcMessage", 0.1f))
    {
      if (rpc == 3999508679U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_CloseDoor "));
        using (TimeWarning.New("RPC_CloseDoor", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_CloseDoor", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_CloseDoor(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_CloseDoor");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1487779344U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_KnockDoor "));
        using (TimeWarning.New("RPC_KnockDoor", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_KnockDoor", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_KnockDoor(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_KnockDoor");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3314360565U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_OpenDoor "));
        using (TimeWarning.New("RPC_OpenDoor", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_OpenDoor", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.RPC_OpenDoor(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in RPC_OpenDoor");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3000490601U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - RPC_ToggleHatch "));
          using (TimeWarning.New("RPC_ToggleHatch", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_ToggleHatch", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.RPC_ToggleHatch(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in RPC_ToggleHatch");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ServerInit()
  {
    base.ServerInit();
    if (Door.nonWalkableArea < 0)
      Door.nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
    if (Door.animalAgentTypeId < 0)
    {
      NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(1);
      Door.animalAgentTypeId = ((NavMeshBuildSettings) ref settingsByIndex).get_agentTypeID();
    }
    if (Object.op_Equality((Object) this.NavMeshVolumeAnimals, (Object) null))
    {
      this.NavMeshVolumeAnimals = (NavMeshModifierVolume) ((Component) this).get_gameObject().AddComponent<NavMeshModifierVolume>();
      this.NavMeshVolumeAnimals.set_area(Door.nonWalkableArea);
      this.NavMeshVolumeAnimals.AddAgentType(Door.animalAgentTypeId);
      this.NavMeshVolumeAnimals.set_center(Vector3.get_zero());
      this.NavMeshVolumeAnimals.set_size(Vector3.get_one());
    }
    if (this.HasSlot(BaseEntity.Slot.Lock))
      this.canNpcOpen = false;
    if (!this.canNpcOpen)
    {
      if (Door.humanoidAgentTypeId < 0)
      {
        NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(0);
        Door.humanoidAgentTypeId = ((NavMeshBuildSettings) ref settingsByIndex).get_agentTypeID();
      }
      if (Object.op_Equality((Object) this.NavMeshVolumeHumanoids, (Object) null))
      {
        this.NavMeshVolumeHumanoids = (NavMeshModifierVolume) ((Component) this).get_gameObject().AddComponent<NavMeshModifierVolume>();
        this.NavMeshVolumeHumanoids.set_area(Door.nonWalkableArea);
        this.NavMeshVolumeHumanoids.AddAgentType(Door.humanoidAgentTypeId);
        this.NavMeshVolumeHumanoids.set_center(Vector3.get_zero());
        this.NavMeshVolumeHumanoids.set_size(Vector3.op_Addition(Vector3.op_Addition(Vector3.get_one(), Vector3.get_up()), Vector3.get_forward()));
      }
    }
    else if (Object.op_Equality((Object) this.NpcTriggerBox, (Object) null))
    {
      if (this.isSecurityDoor)
      {
        M0 m0 = ((Component) this).get_gameObject().AddComponent<NavMeshObstacle>();
        ((NavMeshObstacle) m0).set_carving(true);
        ((NavMeshObstacle) m0).set_center(Vector3.get_zero());
        ((NavMeshObstacle) m0).set_size(Vector3.get_one());
        ((NavMeshObstacle) m0).set_shape((NavMeshObstacleShape) 1);
      }
      this.NpcTriggerBox = (NPCDoorTriggerBox) new GameObject("NpcTriggerBox").AddComponent<NPCDoorTriggerBox>();
      this.NpcTriggerBox.Setup(this);
    }
    AIInformationZone forPoint = AIInformationZone.GetForPoint(((Component) this).get_transform().get_position(), (BaseEntity) null);
    if (!Object.op_Inequality((Object) forPoint, (Object) null) || !Object.op_Equality((Object) this.NavMeshLink, (Object) null))
      return;
    this.NavMeshLink = forPoint.GetClosestNavMeshLink(((Component) this).get_transform().get_position());
  }

  public override void ResetState()
  {
    base.ResetState();
    this.decayResetTimeLast = float.NegativeInfinity;
    if (!this.isSecurityDoor || !Object.op_Inequality((Object) this.NavMeshLink, (Object) null))
      return;
    this.SetNavMeshLinkEnabled(false);
  }

  public override bool HasSlot(BaseEntity.Slot slot)
  {
    if (slot == BaseEntity.Slot.Lock && this.canTakeLock || slot == BaseEntity.Slot.UpperModifier || (slot == BaseEntity.Slot.CenterDecoration && this.canTakeCloser || slot == BaseEntity.Slot.LowerCenterDecoration && this.canTakeKnocker))
      return true;
    return base.HasSlot(slot);
  }

  public override bool CanPickup(BasePlayer player)
  {
    if (!this.IsOpen() || Object.op_Implicit((Object) this.GetSlot(BaseEntity.Slot.Lock)) || (Object.op_Implicit((Object) this.GetSlot(BaseEntity.Slot.UpperModifier)) || Object.op_Implicit((Object) this.GetSlot(BaseEntity.Slot.CenterDecoration))) || Object.op_Implicit((Object) this.GetSlot(BaseEntity.Slot.LowerCenterDecoration)))
      return false;
    return base.CanPickup(player);
  }

  public void CloseRequest()
  {
    this.SetOpen(false);
  }

  public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
  {
    base.OnFlagsChanged(old, next);
    BaseEntity slot = this.GetSlot(BaseEntity.Slot.UpperModifier);
    if (!Object.op_Implicit((Object) slot))
      return;
    ((Component) slot).SendMessage("Think");
  }

  public void SetOpen(bool open)
  {
    this.SetFlag(BaseEntity.Flags.Open, open, false, true);
    this.SendNetworkUpdateImmediate(false);
    if (!this.isSecurityDoor || !Object.op_Inequality((Object) this.NavMeshLink, (Object) null))
      return;
    this.SetNavMeshLinkEnabled(open);
  }

  public void SetLocked(bool locked)
  {
    this.SetFlag(BaseEntity.Flags.Locked, false, false, true);
    this.SendNetworkUpdateImmediate(false);
  }

  public bool GetPlayerLockPermission(BasePlayer player)
  {
    BaseLock slot = this.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
    if (Object.op_Equality((Object) slot, (Object) null))
      return true;
    return slot.GetPlayerLockPermission(player);
  }

  public void SetNavMeshLinkEnabled(bool wantsOn)
  {
    if (!Object.op_Inequality((Object) this.NavMeshLink, (Object) null))
      return;
    if (wantsOn)
    {
      ((Component) this.NavMeshLink).get_gameObject().SetActive(true);
      ((Behaviour) this.NavMeshLink).set_enabled(true);
    }
    else
    {
      ((Behaviour) this.NavMeshLink).set_enabled(false);
      ((Component) this.NavMeshLink).get_gameObject().SetActive(false);
    }
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void RPC_OpenDoor(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.canHandOpen || (this.IsOpen() || this.IsBusy()) || this.IsLocked())
      return;
    BaseLock slot = this.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
    if (Object.op_Inequality((Object) slot, (Object) null))
    {
      if (!slot.OnTryToOpen(rpc.player))
        return;
      if (slot.IsLocked() && (double) Time.get_realtimeSinceStartup() - (double) this.decayResetTimeLast > 60.0)
      {
        BuildingBlock linkedEntity = this.FindLinkedEntity<BuildingBlock>();
        if (Object.op_Implicit((Object) linkedEntity))
          Decay.BuildingDecayTouch(linkedEntity);
        else
          Decay.RadialDecayTouch(((Component) this).get_transform().get_position(), 40f, 2097408);
        this.decayResetTimeLast = Time.get_realtimeSinceStartup();
      }
    }
    this.SetFlag(BaseEntity.Flags.Open, true, false, true);
    this.SendNetworkUpdateImmediate(false);
    if (this.isSecurityDoor && Object.op_Inequality((Object) this.NavMeshLink, (Object) null))
      this.SetNavMeshLinkEnabled(true);
    Interface.CallHook("OnDoorOpened", (object) this, (object) rpc.player);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void RPC_CloseDoor(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.canHandOpen || (!this.IsOpen() || this.IsBusy()) || this.IsLocked())
      return;
    BaseLock slot = this.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
    if (Object.op_Inequality((Object) slot, (Object) null) && !slot.OnTryToClose(rpc.player))
      return;
    this.SetFlag(BaseEntity.Flags.Open, false, false, true);
    this.SendNetworkUpdateImmediate(false);
    if (this.isSecurityDoor && Object.op_Inequality((Object) this.NavMeshLink, (Object) null))
      this.SetNavMeshLinkEnabled(false);
    Interface.CallHook("OnDoorClosed", (object) this, (object) rpc.player);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void RPC_KnockDoor(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.knockEffect.isValid || (double) Time.get_realtimeSinceStartup() < (double) this.nextKnockTime)
      return;
    this.nextKnockTime = Time.get_realtimeSinceStartup() + 0.5f;
    BaseEntity slot = this.GetSlot(BaseEntity.Slot.LowerCenterDecoration);
    if (Object.op_Inequality((Object) slot, (Object) null))
    {
      DoorKnocker component = (DoorKnocker) ((Component) slot).GetComponent<DoorKnocker>();
      if (Object.op_Implicit((Object) component))
      {
        component.Knock(rpc.player);
        return;
      }
    }
    Effect.server.Run(this.knockEffect.resourcePath, (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    Interface.CallHook("OnDoorKnocked", (object) this, (object) rpc.player);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  private void RPC_ToggleHatch(BaseEntity.RPCMessage rpc)
  {
    if (!rpc.player.CanInteract() || !this.hasHatch)
      return;
    BaseLock slot = this.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
    if (Object.op_Implicit((Object) slot) && !slot.OnTryToOpen(rpc.player))
      return;
    this.SetFlag(BaseEntity.Flags.Reserved3, !this.HasFlag(BaseEntity.Flags.Reserved3), false, true);
  }

  public override float BoundsPadding()
  {
    return 2f;
  }
}
