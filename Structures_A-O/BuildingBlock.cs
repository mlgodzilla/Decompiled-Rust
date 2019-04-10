// Decompiled with JetBrains decompiler
// Type: BuildingBlock
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
using UnityEngine;
using UnityEngine.Assertions;

public class BuildingBlock : StabilityEntity
{
  public BuildingGrade.Enum lastGrade = BuildingGrade.Enum.None;
  [NonSerialized]
  public Construction blockDefinition;
  private static Vector3[] outsideLookupOffsets;
  private bool forceSkinRefresh;
  public int modelState;
  private int lastModelState;
  public BuildingGrade.Enum grade;
  public ConstructionSkin currentSkin;
  private DeferredAction skinChange;
  private MeshRenderer placeholderRenderer;
  private MeshCollider placeholderCollider;
  public static BuildingBlock.UpdateSkinWorkQueue updateSkinQueueServer;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("BuildingBlock.OnRpcMessage", 0.1f))
    {
      if (rpc == 2858062413U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoDemolish "));
        using (TimeWarning.New("DoDemolish", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("DoDemolish", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.DoDemolish(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in DoDemolish");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 216608990U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoImmediateDemolish "));
        using (TimeWarning.New("DoImmediateDemolish", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("DoImmediateDemolish", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.DoImmediateDemolish(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in DoImmediateDemolish");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 1956645865U && Object.op_Inequality((Object) player, (Object) null))
      {
        Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
        if (Global.developer > 2)
          Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoRotation "));
        using (TimeWarning.New("DoRotation", 0.1f))
        {
          using (TimeWarning.New("Conditions", 0.1f))
          {
            if (!BaseEntity.RPC_Server.MaxDistance.Test("DoRotation", (BaseEntity) this, player, 3f))
              return true;
          }
          try
          {
            using (TimeWarning.New("Call", 0.1f))
              this.DoRotation(new BaseEntity.RPCMessage()
              {
                connection = (Connection) msg.connection,
                player = player,
                read = msg.get_read()
              });
          }
          catch (Exception ex)
          {
            player.Kick("RPC Error in DoRotation");
            Debug.LogException(ex);
          }
        }
        return true;
      }
      if (rpc == 3746288057U)
      {
        if (Object.op_Inequality((Object) player, (Object) null))
        {
          Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
          if (Global.developer > 2)
            Debug.Log((object) ("SV_RPCMessage: " + (object) player + " - DoUpgradeToGrade "));
          using (TimeWarning.New("DoUpgradeToGrade", 0.1f))
          {
            using (TimeWarning.New("Conditions", 0.1f))
            {
              if (!BaseEntity.RPC_Server.MaxDistance.Test("DoUpgradeToGrade", (BaseEntity) this, player, 3f))
                return true;
            }
            try
            {
              using (TimeWarning.New("Call", 0.1f))
                this.DoUpgradeToGrade(new BaseEntity.RPCMessage()
                {
                  connection = (Connection) msg.connection,
                  player = player,
                  read = msg.get_read()
                });
            }
            catch (Exception ex)
            {
              player.Kick("RPC Error in DoUpgradeToGrade");
              Debug.LogException(ex);
            }
          }
          return true;
        }
      }
    }
    return base.OnRpcMessage(player, rpc, msg);
  }

  public override void ResetState()
  {
    base.ResetState();
    this.blockDefinition = (Construction) null;
    this.forceSkinRefresh = false;
    this.modelState = 0;
    this.lastModelState = 0;
    this.grade = BuildingGrade.Enum.Twigs;
    this.lastGrade = BuildingGrade.Enum.None;
    this.DestroySkin();
    this.UpdatePlaceholder(true);
  }

  public override void InitShared()
  {
    base.InitShared();
    this.placeholderRenderer = (MeshRenderer) ((Component) this).GetComponent<MeshRenderer>();
    this.placeholderCollider = (MeshCollider) ((Component) this).GetComponent<MeshCollider>();
  }

  public override void PostInitShared()
  {
    this.baseProtection = this.currentGrade.gradeBase.damageProtecton;
    this.grade = this.currentGrade.gradeBase.type;
    base.PostInitShared();
  }

  public override void DestroyShared()
  {
    if (this.isServer)
      this.RefreshNeighbours(false);
    base.DestroyShared();
  }

  public override string Categorize()
  {
    return "building";
  }

  public override float BoundsPadding()
  {
    return 1f;
  }

  public override bool IsOutside()
  {
    float outsideTestRange = ConVar.Decay.outside_test_range;
    Vector3 vector3 = this.PivotPoint();
    for (int index = 0; index < BuildingBlock.outsideLookupOffsets.Length; ++index)
    {
      Vector3 outsideLookupOffset = BuildingBlock.outsideLookupOffsets[index];
      if (!Physics.Raycast(new Ray(Vector3.op_Addition(vector3, Vector3.op_Multiply(outsideLookupOffset, outsideTestRange)), Vector3.op_UnaryNegation(outsideLookupOffset)), outsideTestRange - 0.5f, 2097152))
        return true;
    }
    return false;
  }

  public bool CanDemolish(BasePlayer player)
  {
    object obj = Interface.CallHook(nameof (CanDemolish), (object) player, (object) this);
    if (obj is bool)
      return (bool) obj;
    if (this.IsDemolishable())
      return this.HasDemolishPrivilege(player);
    return false;
  }

  private bool IsDemolishable()
  {
    return ConVar.Server.pve || this.HasFlag(BaseEntity.Flags.Reserved2);
  }

  private bool HasDemolishPrivilege(BasePlayer player)
  {
    return player.IsBuildingAuthed(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), this.bounds);
  }

  [BaseEntity.RPC_Server.MaxDistance(3f)]
  [BaseEntity.RPC_Server]
  private void DoDemolish(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract() || !this.CanDemolish(msg.player) || Interface.CallHook("OnStructureDemolish", (object) this, (object) msg.player, (object) false) != null)
      return;
    this.Kill(BaseNetworkable.DestroyMode.Gib);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  private void DoImmediateDemolish(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract() || !msg.player.IsAdmin || Interface.CallHook("OnStructureDemolish", (object) this, (object) msg.player, (object) true) != null)
      return;
    this.Kill(BaseNetworkable.DestroyMode.Gib);
  }

  public void StopBeingDemolishable()
  {
    this.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void StartBeingDemolishable()
  {
    this.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
    this.Invoke(new Action(this.StopBeingDemolishable), 600f);
  }

  public void SetConditionalModel(int state)
  {
    this.modelState = state;
  }

  public bool GetConditionalModel(int index)
  {
    return (uint) (this.modelState & 1 << index) > 0U;
  }

  public ConstructionGrade currentGrade
  {
    get
    {
      ConstructionGrade grade = this.GetGrade(this.grade);
      if ((PrefabAttribute) grade != (PrefabAttribute) null)
        return grade;
      for (int index = 0; index < this.blockDefinition.grades.Length; ++index)
      {
        if ((PrefabAttribute) this.blockDefinition.grades[index] != (PrefabAttribute) null)
          return this.blockDefinition.grades[index];
      }
      Debug.LogWarning((object) ("Building block grade not found: " + (object) this.grade));
      return (ConstructionGrade) null;
    }
  }

  private ConstructionGrade GetGrade(BuildingGrade.Enum iGrade)
  {
    if (this.grade < (BuildingGrade.Enum) this.blockDefinition.grades.Length)
      return this.blockDefinition.grades[(int) iGrade];
    Debug.LogWarning((object) ("Grade out of range " + (object) ((Component) this).get_gameObject() + " " + (object) this.grade + " / " + (object) this.blockDefinition.grades.Length));
    return this.blockDefinition.defaultGrade;
  }

  private bool CanChangeToGrade(BuildingGrade.Enum iGrade, BasePlayer player)
  {
    object obj = Interface.CallHook("CanChangeGrade", (object) player, (object) this, (object) iGrade);
    if (obj is bool)
      return (bool) obj;
    if (this.HasUpgradePrivilege(iGrade, player))
      return !this.IsUpgradeBlocked();
    return false;
  }

  private bool HasUpgradePrivilege(BuildingGrade.Enum iGrade, BasePlayer player)
  {
    if (iGrade == this.grade || iGrade >= (BuildingGrade.Enum) this.blockDefinition.grades.Length || (iGrade < BuildingGrade.Enum.Twigs || iGrade < this.grade))
      return false;
    return !player.IsBuildingBlocked(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), this.bounds);
  }

  private bool IsUpgradeBlocked()
  {
    if (!this.blockDefinition.checkVolumeOnUpgrade)
      return false;
    return DeployVolume.Check(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID), ~(1 << ((Component) this).get_gameObject().get_layer()));
  }

  private bool CanAffordUpgrade(BuildingGrade.Enum iGrade, BasePlayer player)
  {
    object obj = Interface.CallHook(nameof (CanAffordUpgrade), (object) player, (object) this, (object) iGrade);
    if (obj is bool)
      return (bool) obj;
    foreach (ItemAmount itemAmount in this.GetGrade(iGrade).costToBuild)
    {
      if ((double) player.inventory.GetAmount(itemAmount.itemid) < (double) itemAmount.amount)
        return false;
    }
    return true;
  }

  public void SetGrade(BuildingGrade.Enum iGradeID)
  {
    if (this.blockDefinition.grades == null || iGradeID >= (BuildingGrade.Enum) this.blockDefinition.grades.Length)
    {
      Debug.LogError((object) ("Tried to set to undefined grade! " + this.blockDefinition.fullName), (Object) ((Component) this).get_gameObject());
    }
    else
    {
      this.grade = iGradeID;
      this.grade = this.currentGrade.gradeBase.type;
      this.UpdateGrade();
    }
  }

  private void UpdateGrade()
  {
    this.baseProtection = this.currentGrade.gradeBase.damageProtecton;
  }

  public void SetHealthToMax()
  {
    this.health = this.MaxHealth();
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  private void DoUpgradeToGrade(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract())
      return;
    BuildingGrade.Enum @enum = (BuildingGrade.Enum) msg.read.Int32();
    ConstructionGrade grade = this.GetGrade(@enum);
    if ((PrefabAttribute) grade == (PrefabAttribute) null || !this.CanChangeToGrade(@enum, msg.player) || (!this.CanAffordUpgrade(@enum, msg.player) || (double) this.SecondsSinceAttacked < 30.0) || Interface.CallHook("OnStructureUpgrade", (object) this, (object) msg.player, (object) @enum) != null)
      return;
    this.PayForUpgrade(grade, msg.player);
    this.SetGrade(@enum);
    this.SetHealthToMax();
    this.StartBeingRotatable();
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    this.UpdateSkin(false);
    this.ResetUpkeepTime();
    BuildingManager.server.GetBuilding(this.buildingID)?.Dirty();
    Effect.server.Run("assets/bundled/prefabs/fx/build/promote_" + @enum.ToString().ToLower() + ".prefab", (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
  }

  private void PayForUpgrade(ConstructionGrade g, BasePlayer player)
  {
    List<Item> collect = new List<Item>();
    foreach (ItemAmount itemAmount in g.costToBuild)
    {
      player.inventory.Take(collect, itemAmount.itemid, (int) itemAmount.amount);
      player.Command("note.inv " + (object) itemAmount.itemid + " " + (object) (float) ((double) itemAmount.amount * -1.0), (object[]) Array.Empty<object>());
    }
    foreach (Item obj in collect)
      obj.Remove(0.0f);
  }

  private bool NeedsSkinChange()
  {
    if (!Object.op_Equality((Object) this.currentSkin, (Object) null) && !this.forceSkinRefresh && this.lastGrade == this.grade)
      return this.lastModelState != this.modelState;
    return true;
  }

  public void UpdateSkin(bool force = false)
  {
    if (force)
      this.forceSkinRefresh = true;
    if (!this.NeedsSkinChange())
      return;
    if ((double) this.cachedStability <= 0.0 || this.isServer)
    {
      this.ChangeSkin();
    }
    else
    {
      if (!(bool) this.skinChange)
        this.skinChange = new DeferredAction((Object) this, new Action(this.ChangeSkin), ActionPriority.Medium);
      if (!this.skinChange.Idle)
        return;
      this.skinChange.Invoke();
    }
  }

  private void DestroySkin()
  {
    if (!Object.op_Inequality((Object) this.currentSkin, (Object) null))
      return;
    this.currentSkin.Destroy(this);
    this.currentSkin = (ConstructionSkin) null;
  }

  private void RefreshNeighbours(bool linkToNeighbours)
  {
    List<EntityLink> entityLinks = this.GetEntityLinks(linkToNeighbours);
    for (int index1 = 0; index1 < entityLinks.Count; ++index1)
    {
      EntityLink entityLink = entityLinks[index1];
      for (int index2 = 0; index2 < entityLink.connections.Count; ++index2)
      {
        BuildingBlock owner = entityLink.connections[index2].owner as BuildingBlock;
        if (!Object.op_Equality((Object) owner, (Object) null))
        {
          if (Application.isLoading != null)
            owner.UpdateSkin(true);
          else
            BuildingBlock.updateSkinQueueServer.Add(owner);
        }
      }
    }
  }

  private void UpdatePlaceholder(bool state)
  {
    if (Object.op_Implicit((Object) this.placeholderRenderer))
      ((Renderer) this.placeholderRenderer).set_enabled(state);
    if (!Object.op_Implicit((Object) this.placeholderCollider))
      return;
    ((Collider) this.placeholderCollider).set_enabled(state);
  }

  private void ChangeSkin()
  {
    if (this.IsDestroyed)
      return;
    ConstructionGrade currentGrade = this.currentGrade;
    if (currentGrade.skinObject.isValid)
    {
      this.ChangeSkin(currentGrade.skinObject);
    }
    else
    {
      foreach (ConstructionGrade grade in this.blockDefinition.grades)
      {
        if (grade.skinObject.isValid)
        {
          this.ChangeSkin(grade.skinObject);
          return;
        }
      }
      Debug.LogWarning((object) ("No skins found for " + (object) ((Component) this).get_gameObject()));
    }
  }

  public void ChangeSkin(GameObjectRef prefab)
  {
    bool flag1 = this.lastGrade != this.grade;
    this.lastGrade = this.grade;
    if (flag1)
    {
      if (Object.op_Equality((Object) this.currentSkin, (Object) null))
        this.UpdatePlaceholder(false);
      else
        this.DestroySkin();
      this.currentSkin = (ConstructionSkin) this.gameManager.CreatePrefab(prefab.resourcePath, ((Component) this).get_transform(), true).GetComponent<ConstructionSkin>();
      Model component = (Model) ((Component) this.currentSkin).GetComponent<Model>();
      this.SetModel(component);
      Assert.IsTrue(Object.op_Equality((Object) this.model, (Object) component), "Didn't manage to set model successfully!");
    }
    if (this.isServer)
      this.modelState = this.currentSkin.DetermineConditionalModelState(this);
    bool flag2 = this.lastModelState != this.modelState;
    this.lastModelState = this.modelState;
    if (flag1 | flag2 || this.forceSkinRefresh)
    {
      this.currentSkin.Refresh(this);
      this.forceSkinRefresh = false;
    }
    if (!this.isServer)
      return;
    if (flag1)
      this.RefreshNeighbours(true);
    if (!flag2)
      return;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public override bool ShouldBlockProjectiles()
  {
    return (uint) this.grade > 0U;
  }

  private void OnHammered()
  {
  }

  public override float MaxHealth()
  {
    return this.currentGrade.maxHealth;
  }

  public override List<ItemAmount> BuildCost()
  {
    return this.currentGrade.costToBuild;
  }

  public override void OnHealthChanged(float oldvalue, float newvalue)
  {
    base.OnHealthChanged(oldvalue, newvalue);
    if (Mathf.RoundToInt(oldvalue) == Mathf.RoundToInt(newvalue))
      return;
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.UpdateDistance);
  }

  public override float RepairCostFraction()
  {
    return 1f;
  }

  public bool CanRotate(BasePlayer player)
  {
    if (this.IsRotatable() && this.HasRotationPrivilege(player))
      return !this.IsRotationBlocked();
    return false;
  }

  private bool IsRotatable()
  {
    return this.blockDefinition.grades != null && this.blockDefinition.canRotate && this.HasFlag(BaseEntity.Flags.Reserved1);
  }

  private bool IsRotationBlocked()
  {
    if (!this.blockDefinition.checkVolumeOnRotate)
      return false;
    return DeployVolume.Check(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID), ~(1 << ((Component) this).get_gameObject().get_layer()));
  }

  private bool HasRotationPrivilege(BasePlayer player)
  {
    return !player.IsBuildingBlocked(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), this.bounds);
  }

  [BaseEntity.RPC_Server]
  [BaseEntity.RPC_Server.MaxDistance(3f)]
  private void DoRotation(BaseEntity.RPCMessage msg)
  {
    if (!msg.player.CanInteract() || !this.CanRotate(msg.player) || (!this.blockDefinition.canRotate || Interface.CallHook("OnStructureRotate", (object) this, (object) msg.player) != null))
      return;
    Transform transform = ((Component) this).get_transform();
    transform.set_localRotation(Quaternion.op_Multiply(transform.get_localRotation(), Quaternion.Euler(this.blockDefinition.rotationAmount)));
    this.RefreshEntityLinks();
    this.UpdateSurroundingEntities();
    this.UpdateSkin(true);
    this.SendNetworkUpdateImmediate(false);
    this.ClientRPC((Connection) null, "RefreshSkin");
  }

  public void StopBeingRotatable()
  {
    this.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
    this.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
  }

  public void StartBeingRotatable()
  {
    if (this.blockDefinition.grades == null || !this.blockDefinition.canRotate)
      return;
    this.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
    this.Invoke(new Action(this.StopBeingRotatable), 600f);
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.buildingBlock = (__Null) Pool.Get<BuildingBlock>();
    ((BuildingBlock) info.msg.buildingBlock).model = (__Null) this.modelState;
    ((BuildingBlock) info.msg.buildingBlock).grade = (__Null) this.grade;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.buildingBlock != null)
    {
      this.SetConditionalModel((int) ((BuildingBlock) info.msg.buildingBlock).model);
      this.SetGrade((BuildingGrade.Enum) ((BuildingBlock) info.msg.buildingBlock).grade);
    }
    if (!info.fromDisk)
      return;
    this.SetFlag(BaseEntity.Flags.Reserved2, false, false, true);
    this.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
    this.UpdateSkin(false);
  }

  public override void ServerInit()
  {
    this.blockDefinition = PrefabAttribute.server.Find<Construction>(this.prefabID);
    if ((PrefabAttribute) this.blockDefinition == (PrefabAttribute) null)
      Debug.LogError((object) ("Couldn't find Construction for prefab " + (object) this.prefabID));
    base.ServerInit();
    this.UpdateSkin(false);
    if (this.HasFlag(BaseEntity.Flags.Reserved1) || Application.isLoadingSave == null)
      this.StartBeingRotatable();
    if (!this.HasFlag(BaseEntity.Flags.Reserved2) && Application.isLoadingSave != null)
      return;
    this.StartBeingDemolishable();
  }

  public override void Hurt(HitInfo info)
  {
    if (ConVar.Server.pve && Object.op_Implicit((Object) info.Initiator) && info.Initiator is BasePlayer)
      (info.Initiator as BasePlayer).Hurt(info.damageTypes.Total(), DamageType.Generic, (BaseEntity) null, true);
    else
      base.Hurt(info);
  }

  static BuildingBlock()
  {
    Vector3[] vector3Array = new Vector3[5];
    Vector3 vector3_1 = new Vector3(0.0f, 1f, 0.0f);
    vector3Array[0] = ((Vector3) ref vector3_1).get_normalized();
    Vector3 vector3_2 = new Vector3(1f, 1f, 0.0f);
    vector3Array[1] = ((Vector3) ref vector3_2).get_normalized();
    Vector3 vector3_3 = new Vector3(-1f, 1f, 0.0f);
    vector3Array[2] = ((Vector3) ref vector3_3).get_normalized();
    Vector3 vector3_4 = new Vector3(0.0f, 1f, 1f);
    vector3Array[3] = ((Vector3) ref vector3_4).get_normalized();
    Vector3 vector3_5 = new Vector3(0.0f, 1f, -1f);
    vector3Array[4] = ((Vector3) ref vector3_5).get_normalized();
    BuildingBlock.outsideLookupOffsets = vector3Array;
    BuildingBlock.updateSkinQueueServer = new BuildingBlock.UpdateSkinWorkQueue();
  }

  public static class BlockFlags
  {
    public const BaseEntity.Flags CanRotate = BaseEntity.Flags.Reserved1;
    public const BaseEntity.Flags CanDemolish = BaseEntity.Flags.Reserved2;
  }

  public class UpdateSkinWorkQueue : ObjectWorkQueue<BuildingBlock>
  {
    protected virtual void RunJob(BuildingBlock entity)
    {
      if (!base.ShouldAdd(entity))
        return;
      entity.UpdateSkin(true);
    }

    protected virtual bool ShouldAdd(BuildingBlock entity)
    {
      return entity.IsValid();
    }

    public UpdateSkinWorkQueue()
    {
      base.\u002Ector();
    }
  }
}
