// Decompiled with JetBrains decompiler
// Type: ServerBuildingManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ServerBuildingManager : BuildingManager
{
  private int decayTickBuildingIndex;
  private int decayTickEntityIndex;
  private int decayTickWorldIndex;
  private int navmeshCarveTickBuildingIndex;
  private uint maxBuildingID;

  public void CheckSplit(DecayEntity ent)
  {
    if (ent.buildingID == 0U)
      return;
    BuildingManager.Building building = ent.GetBuilding();
    if (building == null || !this.ShouldSplit(building))
      return;
    this.Split(building);
  }

  private bool ShouldSplit(BuildingManager.Building building)
  {
    if (building.HasBuildingBlocks())
    {
      building.buildingBlocks.get_Item(0).EntityLinkBroadcast();
      using (IEnumerator<BuildingBlock> enumerator = building.buildingBlocks.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          if (!enumerator.Current.ReceivedEntityLinkBroadcast())
            return true;
        }
      }
    }
    return false;
  }

  private void Split(BuildingManager.Building building)
  {
    while (building.HasBuildingBlocks())
    {
      BuildingBlock buildingBlock = building.buildingBlocks.get_Item(0);
      uint newID = BuildingManager.server.NewBuildingID();
      Action<BuildingBlock> action = (Action<BuildingBlock>) (b => b.AttachToBuilding(newID));
      buildingBlock.EntityLinkBroadcast<BuildingBlock>(action);
    }
    while (building.HasBuildingPrivileges())
    {
      BuildingPrivlidge buildingPrivlidge = building.buildingPrivileges.get_Item(0);
      BuildingBlock nearbyBuildingBlock = buildingPrivlidge.GetNearbyBuildingBlock();
      buildingPrivlidge.AttachToBuilding(Object.op_Implicit((Object) nearbyBuildingBlock) ? nearbyBuildingBlock.buildingID : 0U);
    }
    while (building.HasDecayEntities())
    {
      DecayEntity decayEntity = building.decayEntities.get_Item(0);
      BuildingBlock nearbyBuildingBlock = decayEntity.GetNearbyBuildingBlock();
      decayEntity.AttachToBuilding(Object.op_Implicit((Object) nearbyBuildingBlock) ? nearbyBuildingBlock.buildingID : 0U);
    }
    if (!ConVar.AI.nav_carve_use_building_optimization)
      return;
    building.isNavMeshCarvingDirty = true;
    int ticks = 2;
    this.UpdateNavMeshCarver(building, ref ticks, 0);
  }

  public void CheckMerge(DecayEntity ent)
  {
    if (ent.buildingID == 0U)
      return;
    BuildingManager.Building building = ent.GetBuilding();
    if (building == null)
      return;
    ent.EntityLinkMessage<BuildingBlock>((Action<BuildingBlock>) (b =>
    {
      if ((int) b.buildingID == (int) building.ID)
        return;
      BuildingManager.Building building1 = b.GetBuilding();
      if (building1 == null)
        return;
      this.Merge(building, building1);
    }));
    if (!ConVar.AI.nav_carve_use_building_optimization)
      return;
    building.isNavMeshCarvingDirty = true;
    int ticks = 2;
    this.UpdateNavMeshCarver(building, ref ticks, 0);
  }

  private void Merge(BuildingManager.Building building1, BuildingManager.Building building2)
  {
    while (building2.HasDecayEntities())
      building2.decayEntities.get_Item(0).AttachToBuilding(building1.ID);
    if (!ConVar.AI.nav_carve_use_building_optimization)
      return;
    building1.isNavMeshCarvingDirty = true;
    building2.isNavMeshCarvingDirty = true;
    int ticks = 3;
    this.UpdateNavMeshCarver(building1, ref ticks, 0);
    this.UpdateNavMeshCarver(building1, ref ticks, 0);
  }

  public void Cycle()
  {
    using (TimeWarning.New("StabilityCheckQueue", 0.1f))
      StabilityEntity.stabilityCheckQueue.RunQueue((double) Stability.stabilityqueue);
    using (TimeWarning.New("UpdateSurroundingsQueue", 0.1f))
      StabilityEntity.updateSurroundingsQueue.RunQueue((double) Stability.surroundingsqueue);
    using (TimeWarning.New("UpdateSkinQueue", 0.1f))
      BuildingBlock.updateSkinQueueServer.RunQueue(1.0);
    using (TimeWarning.New("BuildingDecayTick", 0.1f))
    {
      int num = 5;
      BufferList<BuildingManager.Building> values1 = this.buildingDictionary.get_Values();
      for (int tickBuildingIndex = this.decayTickBuildingIndex; tickBuildingIndex < values1.get_Count() && num > 0; ++tickBuildingIndex)
      {
        BufferList<DecayEntity> values2 = values1.get_Item(tickBuildingIndex).decayEntities.get_Values();
        for (int decayTickEntityIndex = this.decayTickEntityIndex; decayTickEntityIndex < values2.get_Count() && num > 0; ++decayTickEntityIndex)
        {
          values2.get_Item(decayTickEntityIndex).DecayTick();
          --num;
          if (num <= 0)
          {
            this.decayTickBuildingIndex = tickBuildingIndex;
            this.decayTickEntityIndex = decayTickEntityIndex;
          }
        }
        if (num > 0)
          this.decayTickEntityIndex = 0;
      }
      if (num > 0)
        this.decayTickBuildingIndex = 0;
    }
    using (TimeWarning.New("WorldDecayTick", 0.1f))
    {
      int num = 5;
      BufferList<DecayEntity> values = this.decayEntities.get_Values();
      for (int decayTickWorldIndex = this.decayTickWorldIndex; decayTickWorldIndex < values.get_Count() && num > 0; ++decayTickWorldIndex)
      {
        values.get_Item(decayTickWorldIndex).DecayTick();
        --num;
        if (num <= 0)
          this.decayTickWorldIndex = decayTickWorldIndex;
      }
      if (num > 0)
        this.decayTickWorldIndex = 0;
    }
    if (!ConVar.AI.nav_carve_use_building_optimization)
      return;
    using (TimeWarning.New("NavMeshCarving", 0.1f))
    {
      int ticks = 5;
      BufferList<BuildingManager.Building> values = this.buildingDictionary.get_Values();
      for (int tickBuildingIndex = this.navmeshCarveTickBuildingIndex; tickBuildingIndex < values.get_Count() && ticks > 0; ++tickBuildingIndex)
        this.UpdateNavMeshCarver(values.get_Item(tickBuildingIndex), ref ticks, tickBuildingIndex);
      if (ticks <= 0)
        return;
      this.navmeshCarveTickBuildingIndex = 0;
    }
  }

  public void UpdateNavMeshCarver(BuildingManager.Building building, ref int ticks, int i)
  {
    if (!ConVar.AI.nav_carve_use_building_optimization || !building.isNavMeshCarveOptimized && building.navmeshCarvers.get_Count() < ConVar.AI.nav_carve_min_building_blocks_to_apply_optimization || !building.isNavMeshCarvingDirty)
      return;
    building.isNavMeshCarvingDirty = false;
    if (building.navmeshCarvers == null)
    {
      if (!Object.op_Inequality((Object) building.buildingNavMeshObstacle, (Object) null))
        return;
      Object.Destroy((Object) ((Component) building.buildingNavMeshObstacle).get_gameObject());
      building.buildingNavMeshObstacle = (NavMeshObstacle) null;
      building.isNavMeshCarveOptimized = false;
    }
    else
    {
      Vector3 vector3_1;
      ((Vector3) ref vector3_1).\u002Ector((float) World.Size, (float) World.Size, (float) World.Size);
      Vector3 vector3_2;
      ((Vector3) ref vector3_2).\u002Ector((float) -World.Size, (float) -World.Size, (float) -World.Size);
      int count = building.navmeshCarvers.get_Count();
      if (count > 0)
      {
        for (int index1 = 0; index1 < count; ++index1)
        {
          NavMeshObstacle navMeshObstacle = building.navmeshCarvers.get_Item(index1);
          if (((Behaviour) navMeshObstacle).get_enabled())
            ((Behaviour) navMeshObstacle).set_enabled(false);
          for (int index2 = 0; index2 < 3; ++index2)
          {
            Vector3 position = ((Component) navMeshObstacle).get_transform().get_position();
            if ((double) ((Vector3) ref position).get_Item(index2) < (double) ((Vector3) ref vector3_1).get_Item(index2))
            {
              ref Vector3 local = ref vector3_1;
              int num1 = index2;
              position = ((Component) navMeshObstacle).get_transform().get_position();
              double num2 = (double) ((Vector3) ref position).get_Item(index2);
              ((Vector3) ref local).set_Item(num1, (float) num2);
            }
            position = ((Component) navMeshObstacle).get_transform().get_position();
            if ((double) ((Vector3) ref position).get_Item(index2) > (double) ((Vector3) ref vector3_2).get_Item(index2))
            {
              ref Vector3 local = ref vector3_2;
              int num1 = index2;
              position = ((Component) navMeshObstacle).get_transform().get_position();
              double num2 = (double) ((Vector3) ref position).get_Item(index2);
              ((Vector3) ref local).set_Item(num1, (float) num2);
            }
          }
        }
        Vector3 vector3_3 = Vector3.op_Multiply(Vector3.op_Addition(vector3_2, vector3_1), 0.5f);
        Vector3 vector3_4 = Vector3.get_zero();
        float num3 = Mathf.Abs((float) (vector3_3.x - vector3_1.x));
        float num4 = Mathf.Abs((float) (vector3_3.y - vector3_1.y));
        float num5 = Mathf.Abs((float) (vector3_3.z - vector3_1.z));
        float num6 = Mathf.Abs((float) (vector3_2.x - vector3_3.x));
        float num7 = Mathf.Abs((float) (vector3_2.y - vector3_3.y));
        float num8 = Mathf.Abs((float) (vector3_2.z - vector3_3.z));
        vector3_4.x = (__Null) (double) Mathf.Max((double) num3 > (double) num6 ? num3 : num6, ConVar.AI.nav_carve_min_base_size);
        vector3_4.y = (__Null) (double) Mathf.Max((double) num4 > (double) num7 ? num4 : num7, ConVar.AI.nav_carve_min_base_size);
        vector3_4.z = (__Null) (double) Mathf.Max((double) num5 > (double) num8 ? num5 : num8, ConVar.AI.nav_carve_min_base_size);
        vector3_4 = count >= 10 ? Vector3.op_Multiply(vector3_4, ConVar.AI.nav_carve_size_multiplier - 1f) : Vector3.op_Multiply(vector3_4, ConVar.AI.nav_carve_size_multiplier);
        if (building.navmeshCarvers.get_Count() > 0)
        {
          if (Object.op_Equality((Object) building.buildingNavMeshObstacle, (Object) null))
          {
            building.buildingNavMeshObstacle = (NavMeshObstacle) new GameObject(string.Format("Building ({0}) NavMesh Carver", (object) building.ID)).AddComponent<NavMeshObstacle>();
            ((Behaviour) building.buildingNavMeshObstacle).set_enabled(false);
            building.buildingNavMeshObstacle.set_carving(true);
            building.buildingNavMeshObstacle.set_shape((NavMeshObstacleShape) 1);
            building.buildingNavMeshObstacle.set_height(ConVar.AI.nav_carve_height);
            building.isNavMeshCarveOptimized = true;
          }
          if (Object.op_Inequality((Object) building.buildingNavMeshObstacle, (Object) null))
          {
            ((Component) building.buildingNavMeshObstacle).get_transform().set_position(vector3_3);
            building.buildingNavMeshObstacle.set_size(vector3_4);
            if (!((Behaviour) building.buildingNavMeshObstacle).get_enabled())
              ((Behaviour) building.buildingNavMeshObstacle).set_enabled(true);
          }
        }
      }
      else if (Object.op_Inequality((Object) building.buildingNavMeshObstacle, (Object) null))
      {
        Object.Destroy((Object) ((Component) building.buildingNavMeshObstacle).get_gameObject());
        building.buildingNavMeshObstacle = (NavMeshObstacle) null;
        building.isNavMeshCarveOptimized = false;
      }
      --ticks;
      if (ticks > 0)
        return;
      this.navmeshCarveTickBuildingIndex = i;
    }
  }

  public uint NewBuildingID()
  {
    return ++this.maxBuildingID;
  }

  public void LoadBuildingID(uint id)
  {
    this.maxBuildingID = Mathx.Max(this.maxBuildingID, id);
  }

  protected override BuildingManager.Building CreateBuilding(uint id)
  {
    return new BuildingManager.Building() { ID = id };
  }

  protected override void DisposeBuilding(ref BuildingManager.Building building)
  {
    building = (BuildingManager.Building) null;
  }
}
