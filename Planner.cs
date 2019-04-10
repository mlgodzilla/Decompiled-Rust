// Decompiled with JetBrains decompiler
// Type: Planner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class Planner : HeldEntity
{
  public BaseEntity[] buildableList;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Planner.OnRpcMessage", 0.1f))
    {
      if (rpc == 1872774636U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoPlace "));
          using (TimeWarning.New("DoPlace", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoPlace", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.DoPlace(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in DoPlace");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public ItemModDeployable GetModDeployable()
  {
    ItemDefinition ownerItemDefinition = this.GetOwnerItemDefinition();
    if (Object.op_Equality((Object) ownerItemDefinition, (Object) null))
      return (ItemModDeployable) null;
    return (ItemModDeployable) ((Component) ownerItemDefinition).GetComponent<ItemModDeployable>();
  }

  public Deployable GetDeployable()
  {
    ItemModDeployable modDeployable = this.GetModDeployable();
    if (Object.op_Equality((Object) modDeployable, (Object) null))
      return (Deployable) null;
    return modDeployable.GetDeployable((BaseEntity) this);
  }

  public bool isTypeDeployable
  {
    get
    {
      return Object.op_Inequality((Object) this.GetModDeployable(), (Object) null);
    }
  }

  [BaseEntity.RPC_Server.IsActiveItem]
  [BaseEntity.RPC_Server]
  private void DoPlace(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract())
      return;
    using (CreateBuilding msg1 = CreateBuilding.Deserialize((Stream) msg.read))
      this.DoBuild(msg1);
  }

  public Socket_Base FindSocket(string name, uint prefabIDToFind)
  {
    return ((IEnumerable<Socket_Base>) PrefabAttribute.server.FindAll<Socket_Base>(prefabIDToFind)).FirstOrDefault<Socket_Base>((Func<Socket_Base, bool>) (s => s.socketName == name));
  }

  public void DoBuild(CreateBuilding msg)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer))
      return;
    if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack(1f, float.PositiveInfinity))
    {
      ownerPlayer.ChatMessage("AntiHack!");
    }
    else
    {
      Construction component = PrefabAttribute.server.Find<Construction>((uint) msg.blockID);
      if ((PrefabAttribute) component == (PrefabAttribute) null)
        ownerPlayer.ChatMessage("Couldn't find Construction " + (object) (uint) msg.blockID);
      else if (!this.CanAffordToPlace(component))
        ownerPlayer.ChatMessage("Can't afford to place!");
      else if (!ownerPlayer.CanBuild() && !component.canBypassBuildingPermission)
      {
        ownerPlayer.ChatMessage("Building is blocked!");
      }
      else
      {
        Deployable deployable = this.GetDeployable();
        Construction.Target target = new Construction.Target();
        BaseEntity baseEntity = (BaseEntity) null;
        if (msg.entity > 0)
        {
          baseEntity = BaseNetworkable.serverEntities.Find((uint) msg.entity) as BaseEntity;
          if (Object.op_Implicit((Object) baseEntity))
          {
            msg.position = (__Null) ((Component) baseEntity).get_transform().TransformPoint((Vector3) msg.position);
            msg.normal = (__Null) ((Component) baseEntity).get_transform().TransformDirection((Vector3) msg.normal);
            msg.rotation = (__Null) Quaternion.op_Multiply(((Component) baseEntity).get_transform().get_rotation(), (Vector3) msg.rotation);
            if (msg.socket == null)
            {
              if ((bool) ((PrefabAttribute) deployable) && deployable.setSocketParent && (double) baseEntity.Distance((Vector3) msg.position) > 1.0)
              {
                ownerPlayer.ChatMessage("Parent too far away: " + (object) baseEntity.Distance((Vector3) msg.position));
                return;
              }
              if (baseEntity is Door)
              {
                ownerPlayer.ChatMessage("Can't deploy on door");
                return;
              }
            }
            target.entity = baseEntity;
            if (msg.socket > 0)
            {
              string name = StringPool.Get((uint) msg.socket);
              if (name != "" && Object.op_Inequality((Object) target.entity, (Object) null))
                target.socket = this.FindSocket(name, target.entity.prefabID);
              else
                ownerPlayer.ChatMessage("Invalid Socket!");
            }
          }
          else
          {
            ownerPlayer.ChatMessage("Couldn't find entity " + (object) (uint) msg.entity);
            return;
          }
        }
        target.ray = (Ray) msg.ray;
        target.onTerrain = (bool) msg.onterrain;
        target.position = (Vector3) msg.position;
        target.normal = (Vector3) msg.normal;
        target.rotation = (Vector3) msg.rotation;
        target.player = ownerPlayer;
        target.valid = true;
        if (Interface.CallHook("CanBuild", (object) this, (object) component, (object) target) != null)
          return;
        if ((bool) ((PrefabAttribute) deployable) && deployable.placeEffect.isValid)
        {
          if (Object.op_Implicit((Object) baseEntity) && msg.socket > 0)
            Effect.server.Run(deployable.placeEffect.resourcePath, ((Component) baseEntity).get_transform().TransformPoint(target.socket.worldPosition), ((Component) baseEntity).get_transform().get_up(), (Connection) null, false);
          else
            Effect.server.Run(deployable.placeEffect.resourcePath, (Vector3) msg.position, (Vector3) msg.normal, (Connection) null, false);
        }
        this.DoBuild(target, component);
      }
    }
  }

  public void DoBuild(Construction.Target target, Construction component)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer) || target.ray.IsNaNOrInfinity() || (Vector3Ex.IsNaNOrInfinity(target.position) || Vector3Ex.IsNaNOrInfinity(target.normal)))
      return;
    if ((PrefabAttribute) target.socket != (PrefabAttribute) null)
    {
      if (!target.socket.female)
      {
        ownerPlayer.ChatMessage("Target socket is not female. (" + target.socket.socketName + ")");
        return;
      }
      if (Object.op_Inequality((Object) target.entity, (Object) null) && target.entity.IsOccupied(target.socket))
      {
        ownerPlayer.ChatMessage("Target socket is occupied. (" + target.socket.socketName + ")");
        return;
      }
    }
    else if (ConVar.AntiHack.eye_protection >= 2)
    {
      Vector3 center = ownerPlayer.eyes.center;
      Vector3 position1 = ownerPlayer.eyes.position;
      Vector3 origin = ((Ray) ref target.ray).get_origin();
      Vector3 position2 = target.position;
      Vector3 p1 = position1;
      Vector3 p2 = origin;
      Vector3 p3 = position2;
      if (!GamePhysics.LineOfSight(center, p1, p2, p3, 2162688, 0.01f))
      {
        ownerPlayer.ChatMessage("Line of sight blocked.");
        return;
      }
    }
    Construction.lastPlacementError = "No Error";
    GameObject go = this.DoPlacement(target, component);
    if (Object.op_Equality((Object) go, (Object) null))
      ownerPlayer.ChatMessage("Can't place: " + Construction.lastPlacementError);
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    Interface.CallHook("OnEntityBuilt", (object) this, (object) go);
    Deployable deployable = this.GetDeployable();
    if ((PrefabAttribute) deployable != (PrefabAttribute) null)
    {
      BaseEntity baseEntity = go.ToBaseEntity();
      if (deployable.setSocketParent && Object.op_Inequality((Object) target.entity, (Object) null) && (target.entity.SupportsChildDeployables() && Object.op_Implicit((Object) baseEntity)))
        baseEntity.SetParent(target.entity, true, false);
      if (deployable.wantsInstanceData && this.GetOwnerItem().instanceData != null)
        (baseEntity as IInstanceDataReceiver).ReceiveInstanceData(this.GetOwnerItem().instanceData);
      if (deployable.copyInventoryFromItem)
      {
        StorageContainer component1 = (StorageContainer) ((Component) baseEntity).GetComponent<StorageContainer>();
        if (Object.op_Implicit((Object) component1))
          component1.ReceiveInventoryFromItem(this.GetOwnerItem());
      }
      ItemModDeployable modDeployable = this.GetModDeployable();
      if (Object.op_Inequality((Object) modDeployable, (Object) null))
        modDeployable.OnDeployed(baseEntity, ownerPlayer);
    }
    this.PayForPlacement(ownerPlayer, component);
  }

  public GameObject DoPlacement(Construction.Target placement, Construction component)
  {
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer))
      return (GameObject) null;
    BaseEntity construction = component.CreateConstruction(placement, true);
    if (!Object.op_Implicit((Object) construction))
      return (GameObject) null;
    float num1 = 1f;
    float num2 = 0.0f;
    Item ownerItem = this.GetOwnerItem();
    if (ownerItem != null)
    {
      construction.skinID = ownerItem.skin;
      if (ownerItem.hasCondition)
        num1 = ownerItem.conditionNormalized;
    }
    ((Component) construction).get_gameObject().AwakeFromInstantiate();
    BuildingBlock buildingBlock = construction as BuildingBlock;
    if (Object.op_Implicit((Object) buildingBlock))
    {
      buildingBlock.blockDefinition = PrefabAttribute.server.Find<Construction>(buildingBlock.prefabID);
      if (!(bool) ((PrefabAttribute) buildingBlock.blockDefinition))
      {
        Debug.LogError((object) "Placing a building block that has no block definition!");
        return (GameObject) null;
      }
      buildingBlock.SetGrade(buildingBlock.blockDefinition.defaultGrade.gradeBase.type);
      num2 = buildingBlock.currentGrade.maxHealth;
    }
    BaseCombatEntity baseCombatEntity = construction as BaseCombatEntity;
    if (Object.op_Implicit((Object) baseCombatEntity))
    {
      float newmax = Object.op_Inequality((Object) buildingBlock, (Object) null) ? buildingBlock.currentGrade.maxHealth : baseCombatEntity.startHealth;
      baseCombatEntity.ResetLifeStateOnSpawn = false;
      baseCombatEntity.InitializeHealth(newmax * num1, newmax);
    }
    ((Component) construction).get_gameObject().SendMessage("SetDeployedBy", (object) ownerPlayer, (SendMessageOptions) 1);
    construction.OwnerID = ownerPlayer.userID;
    construction.Spawn();
    if (Object.op_Implicit((Object) buildingBlock))
      Effect.server.Run("assets/bundled/prefabs/fx/build/frame_place.prefab", construction, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
    StabilityEntity stabilityEntity = construction as StabilityEntity;
    if (Object.op_Implicit((Object) stabilityEntity))
      stabilityEntity.UpdateSurroundingEntities();
    return ((Component) construction).get_gameObject();
  }

  public void PayForPlacement(BasePlayer player, Construction component)
  {
    if (this.isTypeDeployable)
    {
      this.GetItem().UseItem(1);
    }
    else
    {
      List<Item> collect = new List<Item>();
      foreach (ItemAmount itemAmount in component.defaultGrade.costToBuild)
      {
        player.inventory.Take(collect, itemAmount.itemDef.itemid, (int) itemAmount.amount);
        player.Command("note.inv", (object) itemAmount.itemDef.itemid, (object) (float) ((double) itemAmount.amount * -1.0));
      }
      foreach (Item obj in collect)
        obj.Remove(0.0f);
    }
  }

  public bool CanAffordToPlace(Construction component)
  {
    if (this.isTypeDeployable)
      return true;
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer))
      return false;
    foreach (ItemAmount itemAmount in component.defaultGrade.costToBuild)
    {
      if ((double) ownerPlayer.inventory.GetAmount(itemAmount.itemDef.itemid) < (double) itemAmount.amount)
        return false;
    }
    return true;
  }
}
