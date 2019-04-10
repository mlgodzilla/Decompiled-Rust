// Decompiled with JetBrains decompiler
// Type: BuildingProximity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProximity : PrefabAttribute
{
  private const float check_radius = 2f;
  private const float check_forgiveness = 0.01f;
  private const float foundation_width = 3f;
  private const float foundation_extents = 1.5f;

  public static bool Check(
    BasePlayer player,
    Construction construction,
    Vector3 position,
    Quaternion rotation)
  {
    OBB obb;
    ((OBB) ref obb).\u002Ector(position, rotation, construction.bounds);
    float radius = ((Vector3) ref obb.extents).get_magnitude() + 2f;
    List<BuildingBlock> list = (List<BuildingBlock>) Pool.GetList<BuildingBlock>();
    Vis.Entities<BuildingBlock>((Vector3) obb.position, radius, list, 2097152, (QueryTriggerInteraction) 2);
    uint num = 0;
    for (int index = 0; index < list.Count; ++index)
    {
      BuildingBlock buildingBlock = list[index];
      Construction construction1 = construction;
      Construction blockDefinition = buildingBlock.blockDefinition;
      Vector3 vector3_1 = position;
      Vector3 position1 = ((Component) buildingBlock).get_transform().get_position();
      Quaternion quaternion = rotation;
      Quaternion rotation1 = ((Component) buildingBlock).get_transform().get_rotation();
      BuildingProximity.ProximityInfo proximity1 = BuildingProximity.GetProximity(construction1, vector3_1, quaternion, blockDefinition, position1, rotation1);
      BuildingProximity.ProximityInfo proximity2 = BuildingProximity.GetProximity(blockDefinition, position1, rotation1, construction1, vector3_1, quaternion);
      BuildingProximity.ProximityInfo proximityInfo = new BuildingProximity.ProximityInfo();
      proximityInfo.hit = proximity1.hit || proximity2.hit;
      proximityInfo.connection = proximity1.connection || proximity2.connection;
      if ((double) proximity1.sqrDist <= (double) proximity2.sqrDist)
      {
        proximityInfo.line = proximity1.line;
        proximityInfo.sqrDist = proximity1.sqrDist;
      }
      else
      {
        proximityInfo.line = proximity2.line;
        proximityInfo.sqrDist = proximity2.sqrDist;
      }
      if (proximityInfo.connection)
      {
        BuildingManager.Building building = buildingBlock.GetBuilding();
        if (building != null)
        {
          BuildingPrivlidge buildingPrivilege = building.GetDominatingBuildingPrivilege();
          if (Object.op_Inequality((Object) buildingPrivilege, (Object) null))
          {
            if (!construction.canBypassBuildingPermission && !buildingPrivilege.IsAuthed(player))
            {
              Construction.lastPlacementError = "Cannot attach to unauthorized building";
              // ISSUE: cast to a reference type
              Pool.FreeList<BuildingBlock>((List<M0>&) ref list);
              return true;
            }
            if (num == 0U)
              num = building.ID;
            else if ((int) num != (int) building.ID)
            {
              Construction.lastPlacementError = "Cannot connect two buildings with cupboards";
              // ISSUE: cast to a reference type
              Pool.FreeList<BuildingBlock>((List<M0>&) ref list);
              return true;
            }
          }
        }
      }
      if (proximityInfo.hit)
      {
        Vector3 vector3_2 = Vector3.op_Subtraction((Vector3) proximityInfo.line.point1, (Vector3) proximityInfo.line.point0);
        if ((double) Mathf.Abs((float) vector3_2.y) <= 1.49000000953674 && (double) Vector3Ex.Magnitude2D(vector3_2) <= 1.49000000953674)
        {
          Construction.lastPlacementError = "Not enough space";
          // ISSUE: cast to a reference type
          Pool.FreeList<BuildingBlock>((List<M0>&) ref list);
          return true;
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BuildingBlock>((List<M0>&) ref list);
    return false;
  }

  private static BuildingProximity.ProximityInfo GetProximity(
    Construction construction1,
    Vector3 position1,
    Quaternion rotation1,
    Construction construction2,
    Vector3 position2,
    Quaternion rotation2)
  {
    BuildingProximity.ProximityInfo proximityInfo = new BuildingProximity.ProximityInfo();
    proximityInfo.hit = false;
    proximityInfo.connection = false;
    proximityInfo.line = (Line) null;
    proximityInfo.sqrDist = float.MaxValue;
    for (int index1 = 0; index1 < construction1.allSockets.Length; ++index1)
    {
      ConstructionSocket allSocket1 = construction1.allSockets[index1] as ConstructionSocket;
      if (!((PrefabAttribute) allSocket1 == (PrefabAttribute) null))
      {
        for (int index2 = 0; index2 < construction2.allSockets.Length; ++index2)
        {
          Socket_Base allSocket2 = construction2.allSockets[index2];
          if (allSocket1.CanConnect(position1, rotation1, allSocket2, position2, rotation2))
          {
            proximityInfo.connection = true;
            return proximityInfo;
          }
        }
      }
    }
    if (!proximityInfo.connection && construction1.allProximities.Length != 0)
    {
      for (int index1 = 0; index1 < construction1.allSockets.Length; ++index1)
      {
        ConstructionSocket allSocket = construction1.allSockets[index1] as ConstructionSocket;
        if (!((PrefabAttribute) allSocket == (PrefabAttribute) null) && allSocket.socketType == ConstructionSocket.Type.Wall)
        {
          Vector3 selectPivot1 = allSocket.GetSelectPivot(position1, rotation1);
          for (int index2 = 0; index2 < construction2.allProximities.Length; ++index2)
          {
            Vector3 selectPivot2 = construction2.allProximities[index2].GetSelectPivot(position2, rotation2);
            Line line;
            ((Line) ref line).\u002Ector(selectPivot1, selectPivot2);
            Vector3 vector3 = Vector3.op_Subtraction((Vector3) line.point1, (Vector3) line.point0);
            float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
            if ((double) sqrMagnitude < (double) proximityInfo.sqrDist)
            {
              proximityInfo.hit = true;
              proximityInfo.line = line;
              proximityInfo.sqrDist = sqrMagnitude;
            }
          }
        }
      }
    }
    return proximityInfo;
  }

  public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
  {
    return Vector3.op_Addition(position, Quaternion.op_Multiply(rotation, this.worldPosition));
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (BuildingProximity);
  }

  private struct ProximityInfo
  {
    public bool hit;
    public bool connection;
    public Line line;
    public float sqrDist;
  }
}
