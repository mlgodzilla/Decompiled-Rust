// Decompiled with JetBrains decompiler
// Type: Deployer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Deployer : HeldEntity
{
  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("Deployer.OnRpcMessage", 0.1f))
    {
      if (rpc == 3001117906U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoDeploy "));
          using (TimeWarning.New("DoDeploy", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoDeploy", (BaseEntity) this, player))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.DoDeploy(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in DoDeploy");
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

  public Quaternion GetDeployedRotation(Vector3 normal, Vector3 placeDir)
  {
    return Quaternion.op_Multiply(Quaternion.LookRotation(normal, placeDir), Quaternion.Euler(90f, 0.0f, 0.0f));
  }

  public bool IsPlacementAngleAcceptable(Vector3 pos, Quaternion rot)
  {
    return (double) Mathf.Acos(Vector3.Dot(Quaternion.op_Multiply(rot, Vector3.get_up()), Vector3.get_up())) <= 0.610865235328674;
  }

  public bool CheckPlacement(Deployable deployable, Ray ray, float fDistance)
  {
    using (TimeWarning.New("Deploy.CheckPlacement", 0.1f))
    {
      RaycastHit raycastHit;
      if (!Physics.Raycast(ray, ref raycastHit, fDistance, 1235288065))
        return false;
      DeployVolume[] all = PrefabAttribute.server.FindAll<DeployVolume>(deployable.prefabID);
      Vector3 point = ((RaycastHit) ref raycastHit).get_point();
      Quaternion deployedRotation = this.GetDeployedRotation(((RaycastHit) ref raycastHit).get_normal(), ((Ray) ref ray).get_direction());
      Quaternion rotation = deployedRotation;
      DeployVolume[] volumes = all;
      if (DeployVolume.Check(point, rotation, volumes, -1))
        return false;
      if (!this.IsPlacementAngleAcceptable(((RaycastHit) ref raycastHit).get_point(), deployedRotation))
        return false;
    }
    return true;
  }

  [BaseEntity.RPC_Server.IsActiveItem]
  [BaseEntity.RPC_Server]
  private void DoDeploy(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract())
      return;
    Deployable deployable = this.GetDeployable();
    if ((PrefabAttribute) deployable == (PrefabAttribute) null)
      return;
    Ray ray = msg.read.Ray();
    uint entityID = msg.read.UInt32();
    if (deployable.toSlot)
      this.DoDeploy_Slot(deployable, ray, entityID);
    else
      this.DoDeploy_Regular(deployable, ray);
  }

  public void DoDeploy_Slot(Deployable deployable, Ray ray, uint entityID)
  {
    if (!this.HasItemAmount())
      return;
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer) || !ownerPlayer.CanBuild())
      return;
    BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(entityID) as BaseEntity;
    if (Object.op_Equality((Object) baseEntity, (Object) null) || !baseEntity.HasSlot(deployable.slot) || Object.op_Inequality((Object) baseEntity.GetSlot(deployable.slot), (Object) null))
      return;
    ItemModDeployable modDeployable = this.GetModDeployable();
    BaseEntity entity = GameManager.server.CreateEntity(modDeployable.entityPrefab.resourcePath, (Vector3) null, (Quaternion) null, true);
    if (Object.op_Inequality((Object) entity, (Object) null))
    {
      entity.SetParent(baseEntity, baseEntity.GetSlotAnchorName(deployable.slot), false, false);
      entity.OwnerID = ownerPlayer.userID;
      entity.OnDeployed(baseEntity);
      entity.Spawn();
      baseEntity.SetSlot(deployable.slot, entity);
      if (deployable.placeEffect.isValid)
        Effect.server.Run(deployable.placeEffect.resourcePath, ((Component) baseEntity).get_transform().get_position(), Vector3.get_up(), (Connection) null, false);
    }
    modDeployable.OnDeployed(entity, ownerPlayer);
    Interface.CallHook("OnItemDeployed", (object) this, (object) baseEntity);
    this.UseItemAmount(1);
  }

  public void DoDeploy_Regular(Deployable deployable, Ray ray)
  {
    if (!this.HasItemAmount())
      return;
    BasePlayer ownerPlayer = this.GetOwnerPlayer();
    if (!Object.op_Implicit((Object) ownerPlayer))
      return;
    if (!ownerPlayer.CanBuild())
      ownerPlayer.ChatMessage("Building is blocked!");
    else if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack(1f, float.PositiveInfinity))
    {
      ownerPlayer.ChatMessage("AntiHack!");
    }
    else
    {
      RaycastHit raycastHit;
      if (!this.CheckPlacement(deployable, ray, 8f) || !Physics.Raycast(ray, ref raycastHit, 8f, 1235288065))
        return;
      Quaternion deployedRotation = this.GetDeployedRotation(((RaycastHit) ref raycastHit).get_normal(), ((Ray) ref ray).get_direction());
      Item ownerItem = this.GetOwnerItem();
      ItemModDeployable modDeployable = this.GetModDeployable();
      BaseEntity entity = GameManager.server.CreateEntity(modDeployable.entityPrefab.resourcePath, ((RaycastHit) ref raycastHit).get_point(), deployedRotation, true);
      if (!Object.op_Implicit((Object) entity))
      {
        Debug.LogWarning((object) ("Couldn't create prefab:" + modDeployable.entityPrefab.resourcePath));
      }
      else
      {
        entity.skinID = ownerItem.skin;
        ((Component) entity).SendMessage("SetDeployedBy", (object) ownerPlayer, (SendMessageOptions) 1);
        entity.OwnerID = ownerPlayer.userID;
        entity.Spawn();
        modDeployable.OnDeployed(entity, ownerPlayer);
        Interface.CallHook("OnItemDeployed", (object) this, (object) entity);
        this.UseItemAmount(1);
      }
    }
  }
}
