// Decompiled with JetBrains decompiler
// Type: BuildingManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.AI;

public abstract class BuildingManager
{
  public static ServerBuildingManager server = new ServerBuildingManager();
  protected ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);
  protected ListDictionary<uint, BuildingManager.Building> buildingDictionary = new ListDictionary<uint, BuildingManager.Building>(8);

  public BuildingManager.Building GetBuilding(uint buildingID)
  {
    BuildingManager.Building building = (BuildingManager.Building) null;
    this.buildingDictionary.TryGetValue(buildingID, ref building);
    return building;
  }

  public void Add(DecayEntity ent)
  {
    if (ent.buildingID == 0U)
    {
      if (this.decayEntities.Contains(ent))
        return;
      this.decayEntities.Add(ent);
    }
    else
    {
      BuildingManager.Building building = this.GetBuilding(ent.buildingID);
      if (building == null)
      {
        building = this.CreateBuilding(ent.buildingID);
        this.buildingDictionary.Add(ent.buildingID, building);
      }
      building.Add(ent);
      building.Dirty();
    }
  }

  public void Remove(DecayEntity ent)
  {
    if (ent.buildingID == 0U)
    {
      this.decayEntities.Remove(ent);
    }
    else
    {
      BuildingManager.Building building = this.GetBuilding(ent.buildingID);
      if (building == null)
        return;
      building.Remove(ent);
      if (building.IsEmpty())
      {
        this.buildingDictionary.Remove(ent.buildingID);
        this.DisposeBuilding(ref building);
      }
      else
        building.Dirty();
    }
  }

  public void Clear()
  {
    this.buildingDictionary.Clear();
  }

  protected abstract BuildingManager.Building CreateBuilding(uint id);

  protected abstract void DisposeBuilding(ref BuildingManager.Building building);

  public class Building
  {
    public ListHashSet<BuildingPrivlidge> buildingPrivileges = new ListHashSet<BuildingPrivlidge>(8);
    public ListHashSet<BuildingBlock> buildingBlocks = new ListHashSet<BuildingBlock>(8);
    public ListHashSet<DecayEntity> decayEntities = new ListHashSet<DecayEntity>(8);
    public uint ID;
    public NavMeshObstacle buildingNavMeshObstacle;
    public ListHashSet<NavMeshObstacle> navmeshCarvers;
    public bool isNavMeshCarvingDirty;
    public bool isNavMeshCarveOptimized;

    public bool IsEmpty()
    {
      return !this.HasBuildingPrivileges() && !this.HasBuildingBlocks() && !this.HasDecayEntities();
    }

    public BuildingPrivlidge GetDominatingBuildingPrivilege()
    {
      BuildingPrivlidge buildingPrivlidge1 = (BuildingPrivlidge) null;
      if (this.HasBuildingPrivileges())
      {
        for (int index = 0; index < this.buildingPrivileges.get_Count(); ++index)
        {
          BuildingPrivlidge buildingPrivlidge2 = this.buildingPrivileges.get_Item(index);
          if (!Object.op_Equality((Object) buildingPrivlidge2, (Object) null) && buildingPrivlidge2.IsOlderThan((BaseEntity) buildingPrivlidge1))
            buildingPrivlidge1 = buildingPrivlidge2;
        }
      }
      return buildingPrivlidge1;
    }

    public bool HasBuildingPrivileges()
    {
      if (this.buildingPrivileges != null)
        return this.buildingPrivileges.get_Count() > 0;
      return false;
    }

    public bool HasBuildingBlocks()
    {
      if (this.buildingBlocks != null)
        return this.buildingBlocks.get_Count() > 0;
      return false;
    }

    public bool HasDecayEntities()
    {
      if (this.decayEntities != null)
        return this.decayEntities.get_Count() > 0;
      return false;
    }

    public void AddBuildingPrivilege(BuildingPrivlidge ent)
    {
      if (Object.op_Equality((Object) ent, (Object) null) || this.buildingPrivileges.Contains(ent))
        return;
      this.buildingPrivileges.Add(ent);
    }

    public void RemoveBuildingPrivilege(BuildingPrivlidge ent)
    {
      if (Object.op_Equality((Object) ent, (Object) null))
        return;
      this.buildingPrivileges.Remove(ent);
    }

    public void AddBuildingBlock(BuildingBlock ent)
    {
      if (Object.op_Equality((Object) ent, (Object) null) || this.buildingBlocks.Contains(ent))
        return;
      this.buildingBlocks.Add(ent);
      if (!ConVar.AI.nav_carve_use_building_optimization)
        return;
      NavMeshObstacle component = (NavMeshObstacle) ((Component) ent).GetComponent<NavMeshObstacle>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      this.isNavMeshCarvingDirty = true;
      if (this.navmeshCarvers == null)
        this.navmeshCarvers = new ListHashSet<NavMeshObstacle>(8);
      this.navmeshCarvers.Add(component);
    }

    public void RemoveBuildingBlock(BuildingBlock ent)
    {
      if (Object.op_Equality((Object) ent, (Object) null))
        return;
      this.buildingBlocks.Remove(ent);
      if (!ConVar.AI.nav_carve_use_building_optimization || this.navmeshCarvers == null)
        return;
      NavMeshObstacle component = (NavMeshObstacle) ((Component) ent).GetComponent<NavMeshObstacle>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      this.navmeshCarvers.Remove(component);
      if (this.navmeshCarvers.get_Count() == 0)
        this.navmeshCarvers = (ListHashSet<NavMeshObstacle>) null;
      this.isNavMeshCarvingDirty = true;
      if (this.navmeshCarvers != null)
        return;
      BuildingManager.Building building = ent.GetBuilding();
      if (building == null)
        return;
      int ticks = 2;
      BuildingManager.server.UpdateNavMeshCarver(building, ref ticks, 0);
    }

    public void AddDecayEntity(DecayEntity ent)
    {
      if (Object.op_Equality((Object) ent, (Object) null) || this.decayEntities.Contains(ent))
        return;
      this.decayEntities.Add(ent);
    }

    public void RemoveDecayEntity(DecayEntity ent)
    {
      if (Object.op_Equality((Object) ent, (Object) null))
        return;
      this.decayEntities.Remove(ent);
    }

    public void Add(DecayEntity ent)
    {
      this.AddDecayEntity(ent);
      this.AddBuildingBlock(ent as BuildingBlock);
      this.AddBuildingPrivilege(ent as BuildingPrivlidge);
    }

    public void Remove(DecayEntity ent)
    {
      this.RemoveDecayEntity(ent);
      this.RemoveBuildingBlock(ent as BuildingBlock);
      this.RemoveBuildingPrivilege(ent as BuildingPrivlidge);
    }

    public void Dirty()
    {
      BuildingPrivlidge buildingPrivilege = this.GetDominatingBuildingPrivilege();
      if (!Object.op_Inequality((Object) buildingPrivilege, (Object) null))
        return;
      buildingPrivilege.BuildingDirty();
    }
  }
}
