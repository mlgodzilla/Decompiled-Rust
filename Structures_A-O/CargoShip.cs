// Decompiled with JetBrains decompiler
// Type: CargoShip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CargoShip : BaseEntity
{
  [ServerVar]
  public static bool event_enabled = true;
  [ServerVar]
  public static float event_duration_minutes = 50f;
  [ServerVar]
  public static float egress_duration_minutes = 10f;
  [ServerVar]
  public static int loot_rounds = 3;
  [ServerVar]
  public static float loot_round_spacing_minutes = 10f;
  public int targetNodeIndex = -1;
  private Vector3 currentVelocity = Vector3.get_zero();
  public GameObject wakeParent;
  public GameObjectRef scientistTurretPrefab;
  public Transform[] scientistSpawnPoints;
  public List<Transform> crateSpawns;
  public GameObjectRef lockedCratePrefab;
  public GameObjectRef militaryCratePrefab;
  public GameObjectRef eliteCratePrefab;
  public GameObjectRef junkCratePrefab;
  public Transform waterLine;
  public Transform rudder;
  public Transform propeller;
  public GameObjectRef escapeBoatPrefab;
  public Transform escapeBoatPoint;
  public GameObject radiation;
  public GameObjectRef mapMarkerEntityPrefab;
  public GameObject hornOrigin;
  public SoundDefinition hornDef;
  public CargoShipSounds cargoShipSounds;
  public GameObject[] layouts;
  private BaseEntity mapMarkerInstance;
  private float currentThrottle;
  private float currentTurnSpeed;
  private float turnScale;
  public GameObjectRef playerTest;
  private int lootRoundsPassed;
  private int hornCount;
  private float currentRadiation;
  private bool egressing;

  public override bool PhysicsDriven()
  {
    return true;
  }

  public void UpdateLayoutFromFlags()
  {
    if (this.HasFlag(BaseEntity.Flags.Reserved1))
    {
      this.layouts[0].SetActive(true);
    }
    else
    {
      if (!this.HasFlag(BaseEntity.Flags.Reserved2))
        return;
      this.layouts[1].SetActive(true);
    }
  }

  public void TriggeredEventSpawn()
  {
    float x = (float) TerrainMeta.Size.x;
    Vector3 vector3 = Vector3Ex.Range(-1f, 1f);
    vector3.y = (__Null) 0.0;
    ((Vector3) ref vector3).Normalize();
    Vector3 worldPos = Vector3.op_Multiply(vector3, x * 1f);
    worldPos.y = (__Null) (double) TerrainMeta.WaterMap.GetHeight(worldPos);
    ((Component) this).get_transform().set_position(worldPos);
    if (CargoShip.event_enabled && (double) CargoShip.event_duration_minutes != 0.0)
      return;
    this.Invoke(new Action(this.DelayedDestroy), 1f);
  }

  public void CreateMapMarker()
  {
    if (Object.op_Implicit((Object) this.mapMarkerInstance))
      this.mapMarkerInstance.Kill(BaseNetworkable.DestroyMode.None);
    BaseEntity entity = GameManager.server.CreateEntity(this.mapMarkerEntityPrefab.resourcePath, Vector3.get_zero(), Quaternion.get_identity(), true);
    entity.Spawn();
    entity.SetParent((BaseEntity) this, false, false);
    this.mapMarkerInstance = entity;
  }

  public void DisableCollisionTest()
  {
  }

  public void SpawnCrate(string resourcePath)
  {
    int index = Random.Range(0, this.crateSpawns.Count);
    Vector3 position = this.crateSpawns[index].get_position();
    Quaternion rotation = this.crateSpawns[index].get_rotation();
    this.crateSpawns.Remove(this.crateSpawns[index]);
    BaseEntity entity = GameManager.server.CreateEntity(resourcePath, position, rotation, true);
    if (!Object.op_Implicit((Object) entity))
      return;
    entity.enableSaving = false;
    ((Component) entity).SendMessage("SetWasDropped", (SendMessageOptions) 1);
    entity.Spawn();
    entity.SetParent((BaseEntity) this, true, false);
    Rigidbody component = (Rigidbody) ((Component) entity).GetComponent<Rigidbody>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.set_isKinematic(true);
  }

  public void RespawnLoot()
  {
    this.InvokeRepeating(new Action(this.PlayHorn), 0.0f, 8f);
    this.SpawnCrate(this.lockedCratePrefab.resourcePath);
    this.SpawnCrate(this.eliteCratePrefab.resourcePath);
    for (int index = 0; index < 4; ++index)
      this.SpawnCrate(this.militaryCratePrefab.resourcePath);
    for (int index = 0; index < 4; ++index)
      this.SpawnCrate(this.junkCratePrefab.resourcePath);
    ++this.lootRoundsPassed;
    if (this.lootRoundsPassed < CargoShip.loot_rounds)
      return;
    this.CancelInvoke(new Action(this.RespawnLoot));
  }

  public void SpawnSubEntities()
  {
    BaseEntity entity = GameManager.server.CreateEntity(this.escapeBoatPrefab.resourcePath, this.escapeBoatPoint.get_position(), this.escapeBoatPoint.get_rotation(), true);
    if (!Object.op_Implicit((Object) entity))
      return;
    entity.enableSaving = false;
    entity.SetParent((BaseEntity) this, true, false);
    entity.Spawn();
    ((Rigidbody) ((Component) entity).GetComponent<Rigidbody>()).set_isKinematic(true);
    RHIB component = (RHIB) ((Component) entity).GetComponent<RHIB>();
    if (!Object.op_Implicit((Object) component))
      return;
    BaseEntity baseEntity = component.fuelStorageInstance.Get(true);
    if (!Object.op_Implicit((Object) baseEntity))
      return;
    ((StorageContainer) ((Component) baseEntity).GetComponent<StorageContainer>()).inventory.AddItem(ItemManager.FindItemDefinition("lowgradefuel"), 50);
  }

  public void PlayHorn()
  {
    this.ClientRPC((Connection) null, "DoHornSound");
    ++this.hornCount;
    if (this.hornCount < 3)
      return;
    this.hornCount = 0;
    this.CancelInvoke(new Action(this.PlayHorn));
  }

  public void PickLayout()
  {
    if (this.HasFlag(BaseEntity.Flags.Reserved1) || this.HasFlag(BaseEntity.Flags.Reserved2))
      return;
    switch (Random.Range(0, this.layouts.Length))
    {
      case 0:
        this.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
        break;
      case 1:
        this.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
        break;
    }
  }

  public override void Spawn()
  {
    if (Application.isLoadingSave == null)
      this.PickLayout();
    base.Spawn();
  }

  public override void ServerInit()
  {
    foreach (GameObject layout in this.layouts)
      layout.SetActive(false);
    this.UpdateLayoutFromFlags();
    base.ServerInit();
    this.Invoke(new Action(this.FindInitialNode), 2f);
    this.InvokeRepeating(new Action(this.BuildingCheck), 1f, 5f);
    this.InvokeRepeating(new Action(this.RespawnLoot), 10f, 60f * CargoShip.loot_round_spacing_minutes);
    this.Invoke(new Action(this.DisableCollisionTest), 10f);
    ((Component) this).get_transform().set_position(new Vector3((float) ((Component) this).get_transform().get_position().x, TerrainMeta.WaterMap.GetHeight(((Component) this).get_transform().get_position()) - (float) ((Component) this).get_transform().InverseTransformPoint(((Component) this.waterLine).get_transform().get_position()).y, (float) ((Component) this).get_transform().get_position().z));
    this.SpawnSubEntities();
    this.Invoke(new Action(this.StartEgress), 60f * CargoShip.event_duration_minutes);
    this.CreateMapMarker();
  }

  public void UpdateRadiation()
  {
    ++this.currentRadiation;
    foreach (TriggerRadiation componentsInChild in (TriggerRadiation[]) this.radiation.GetComponentsInChildren<TriggerRadiation>())
      componentsInChild.RadiationAmountOverride = this.currentRadiation;
  }

  public void StartEgress()
  {
    if (this.egressing)
      return;
    this.egressing = true;
    this.CancelInvoke(new Action(this.PlayHorn));
    this.radiation.SetActive(true);
    this.SetFlag(BaseEntity.Flags.Reserved8, true, false, true);
    this.InvokeRepeating(new Action(this.UpdateRadiation), 10f, 1f);
    this.Invoke(new Action(this.DelayedDestroy), 60f * CargoShip.egress_duration_minutes);
  }

  public void DelayedDestroy()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public void FindInitialNode()
  {
    this.targetNodeIndex = this.GetClosestNodeToUs();
  }

  public void BuildingCheck()
  {
    List<DecayEntity> list = (List<DecayEntity>) Pool.GetList<DecayEntity>();
    Vis.Entities<DecayEntity>(this.WorldSpaceBounds(), list, 2097152, (QueryTriggerInteraction) 2);
    foreach (DecayEntity decayEntity in list)
    {
      if (decayEntity.isServer && decayEntity.IsAlive())
        decayEntity.Kill(BaseNetworkable.DestroyMode.Gib);
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<DecayEntity>((List<M0>&) ref list);
  }

  public void FixedUpdate()
  {
    if (this.isClient)
      return;
    this.UpdateMovement();
  }

  public void UpdateMovement()
  {
    if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0 || this.targetNodeIndex == -1)
      return;
    Vector3 vector3_1 = TerrainMeta.Path.OceanPatrolFar[this.targetNodeIndex];
    if (this.egressing)
    {
      Vector3 position = ((Component) this).get_transform().get_position();
      Vector3 vector3_2 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), Vector3.get_zero());
      Vector3 vector3_3 = Vector3.op_Multiply(((Vector3) ref vector3_2).get_normalized(), 10000f);
      vector3_1 = Vector3.op_Addition(position, vector3_3);
    }
    Vector3 vector3_4 = Vector3.op_Subtraction(vector3_1, ((Component) this).get_transform().get_position());
    Vector3 normalized = ((Vector3) ref vector3_4).get_normalized();
    float num1 = Mathf.InverseLerp(0.0f, 1f, Vector3.Dot(((Component) this).get_transform().get_forward(), normalized));
    float num2 = Vector3.Dot(((Component) this).get_transform().get_right(), normalized);
    float num3 = 2.5f;
    this.turnScale = Mathf.Lerp(this.turnScale, Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(num2)), Time.get_deltaTime() * 0.2f);
    float num4 = (double) num2 < 0.0 ? -1f : 1f;
    this.currentTurnSpeed = num3 * this.turnScale * num4;
    ((Component) this).get_transform().Rotate(Vector3.get_up(), Time.get_deltaTime() * this.currentTurnSpeed, (Space) 0);
    this.currentThrottle = Mathf.Lerp(this.currentThrottle, num1, Time.get_deltaTime() * 0.2f);
    this.currentVelocity = Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 8f * this.currentThrottle);
    Transform transform = ((Component) this).get_transform();
    transform.set_position(Vector3.op_Addition(transform.get_position(), Vector3.op_Multiply(this.currentVelocity, Time.get_deltaTime())));
    if ((double) Vector3.Distance(((Component) this).get_transform().get_position(), vector3_1) >= 80.0)
      return;
    ++this.targetNodeIndex;
    if (this.targetNodeIndex < TerrainMeta.Path.OceanPatrolFar.Count)
      return;
    this.targetNodeIndex = 0;
  }

  public int GetClosestNodeToUs()
  {
    int num1 = 0;
    float num2 = float.PositiveInfinity;
    for (int index = 0; index < TerrainMeta.Path.OceanPatrolFar.Count; ++index)
    {
      float num3 = Vector3.Distance(((Component) this).get_transform().get_position(), TerrainMeta.Path.OceanPatrolFar[index]);
      if ((double) num3 < (double) num2)
      {
        num1 = index;
        num2 = num3;
      }
    }
    return num1;
  }

  public override Vector3 GetLocalVelocityServer()
  {
    return this.currentVelocity;
  }

  public override Quaternion GetAngularVelocityServer()
  {
    return Quaternion.Euler(0.0f, this.currentTurnSpeed, 0.0f);
  }

  public override float InheritedVelocityScale()
  {
    return 1f;
  }

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("CargoShip.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }
}
