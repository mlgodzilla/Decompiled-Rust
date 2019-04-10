// Decompiled with JetBrains decompiler
// Type: Construction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Construction : PrefabAttribute
{
  public Vector3 rotationAmount = new Vector3(0.0f, 90f, 0.0f);
  [Range(0.0f, 10f)]
  public float healthMultiplier = 1f;
  [Range(0.0f, 10f)]
  public float costMultiplier = 1f;
  [Range(1f, 50f)]
  public float maxplaceDistance = 4f;
  public BaseEntity.Menu.Option info;
  public bool canBypassBuildingPermission;
  public bool canRotate;
  public bool checkVolumeOnRotate;
  public bool checkVolumeOnUpgrade;
  public bool canPlaceAtMaxDistance;
  public Mesh guideMesh;
  [NonSerialized]
  public Socket_Base[] allSockets;
  [NonSerialized]
  public BuildingProximity[] allProximities;
  [NonSerialized]
  public ConstructionGrade defaultGrade;
  [NonSerialized]
  public SocketHandle socketHandle;
  [NonSerialized]
  public Bounds bounds;
  [NonSerialized]
  public bool isBuildingPrivilege;
  [NonSerialized]
  public ConstructionGrade[] grades;
  [NonSerialized]
  public Deployable deployable;
  [NonSerialized]
  public ConstructionPlaceholder placeholder;
  public static string lastPlacementError;

  public BaseEntity CreateConstruction(
    Construction.Target target,
    bool bNeedsValidPlacement = false)
  {
    GameObject prefab = GameManager.server.CreatePrefab(this.fullName, Vector3.get_zero(), Quaternion.get_identity(), false);
    bool flag = this.UpdatePlacement(prefab.get_transform(), this, ref target);
    BaseEntity baseEntity = prefab.ToBaseEntity();
    if (bNeedsValidPlacement && !flag)
    {
      if (baseEntity.IsValid())
        baseEntity.Kill(BaseNetworkable.DestroyMode.None);
      else
        GameManager.Destroy(prefab, 0.0f);
      return (BaseEntity) null;
    }
    DecayEntity decayEntity = baseEntity as DecayEntity;
    if (Object.op_Implicit((Object) decayEntity))
      decayEntity.AttachToBuilding(target.entity as DecayEntity);
    return baseEntity;
  }

  public bool HasMaleSockets(Construction.Target target)
  {
    foreach (Socket_Base allSocket in this.allSockets)
    {
      if (allSocket.male && !allSocket.maleDummy && allSocket.TestTarget(target))
        return true;
    }
    return false;
  }

  public void FindMaleSockets(Construction.Target target, List<Socket_Base> sockets)
  {
    foreach (Socket_Base allSocket in this.allSockets)
    {
      if (allSocket.male && !allSocket.maleDummy && allSocket.TestTarget(target))
        sockets.Add(allSocket);
    }
  }

  protected override void AttributeSetup(
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
    this.isBuildingPrivilege = Object.op_Implicit((Object) rootObj.GetComponent<BuildingPrivlidge>());
    this.bounds = ((BaseEntity) rootObj.GetComponent<BaseEntity>()).bounds;
    this.deployable = (Deployable) ((Component) this).GetComponent<Deployable>();
    this.placeholder = (ConstructionPlaceholder) ((Component) this).GetComponentInChildren<ConstructionPlaceholder>();
    this.allSockets = (Socket_Base[]) ((Component) this).GetComponentsInChildren<Socket_Base>(true);
    this.allProximities = (BuildingProximity[]) ((Component) this).GetComponentsInChildren<BuildingProximity>(true);
    this.socketHandle = ((IEnumerable<SocketHandle>) ((Component) this).GetComponentsInChildren<SocketHandle>(true)).FirstOrDefault<SocketHandle>();
    M0[] components = rootObj.GetComponents<ConstructionGrade>();
    this.grades = new ConstructionGrade[5];
    foreach (ConstructionGrade constructionGrade in (ConstructionGrade[]) components)
    {
      constructionGrade.construction = this;
      this.grades[(int) constructionGrade.gradeBase.type] = constructionGrade;
    }
    for (int index = 0; index < this.grades.Length; ++index)
    {
      if (!((PrefabAttribute) this.grades[index] == (PrefabAttribute) null))
      {
        this.defaultGrade = this.grades[index];
        break;
      }
    }
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (Construction);
  }

  public bool UpdatePlacement(
    Transform transform,
    Construction common,
    ref Construction.Target target)
  {
    if (!target.valid)
      return false;
    if (!common.canBypassBuildingPermission && !target.player.CanBuild())
    {
      Construction.lastPlacementError = "Player doesn't have permission";
      return false;
    }
    List<Socket_Base> list = (List<Socket_Base>) Pool.GetList<Socket_Base>();
    common.FindMaleSockets(target, list);
    foreach (Socket_Base socketBase in list)
    {
      Construction.Placement placement = (Construction.Placement) null;
      if (!Object.op_Inequality((Object) target.entity, (Object) null) || !((PrefabAttribute) target.socket != (PrefabAttribute) null) || !target.entity.IsOccupied(target.socket))
      {
        if (placement == null)
          placement = socketBase.DoPlacement(target);
        if (placement != null)
        {
          if (!socketBase.CheckSocketMods(placement))
          {
            transform.set_position(placement.position);
            transform.set_rotation(placement.rotation);
          }
          else if (!this.TestPlacingThroughRock(ref placement, target))
          {
            transform.set_position(placement.position);
            transform.set_rotation(placement.rotation);
            Construction.lastPlacementError = "Placing through rock";
          }
          else if (!Construction.TestPlacingThroughWall(ref placement, transform, common, target))
          {
            transform.set_position(placement.position);
            transform.set_rotation(placement.rotation);
            Construction.lastPlacementError = "Placing through wall";
          }
          else if ((double) Vector3.Distance(placement.position, target.player.eyes.position) > (double) common.maxplaceDistance + 1.0)
          {
            transform.set_position(placement.position);
            transform.set_rotation(placement.rotation);
            Construction.lastPlacementError = "Too far away";
          }
          else
          {
            DeployVolume[] all = PrefabAttribute.server.FindAll<DeployVolume>(this.prefabID);
            if (DeployVolume.Check(placement.position, placement.rotation, all, -1))
            {
              transform.set_position(placement.position);
              transform.set_rotation(placement.rotation);
              Construction.lastPlacementError = "Not enough space";
            }
            else if (BuildingProximity.Check(target.player, this, placement.position, placement.rotation))
            {
              transform.set_position(placement.position);
              transform.set_rotation(placement.rotation);
            }
            else if (common.isBuildingPrivilege && !target.player.CanPlaceBuildingPrivilege(placement.position, placement.rotation, common.bounds))
            {
              transform.set_position(placement.position);
              transform.set_rotation(placement.rotation);
              Construction.lastPlacementError = "Cannot stack building privileges";
            }
            else
            {
              bool flag = target.player.IsBuildingBlocked(placement.position, placement.rotation, common.bounds);
              if (!common.canBypassBuildingPermission && flag)
              {
                transform.set_position(placement.position);
                transform.set_rotation(placement.rotation);
                Construction.lastPlacementError = "Building privilege";
              }
              else
              {
                target.inBuildingPrivilege = flag;
                transform.set_position(placement.position);
                transform.set_rotation(placement.rotation);
                // ISSUE: cast to a reference type
                Pool.FreeList<Socket_Base>((List<M0>&) ref list);
                return true;
              }
            }
          }
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Socket_Base>((List<M0>&) ref list);
    return false;
  }

  private bool TestPlacingThroughRock(
    ref Construction.Placement placement,
    Construction.Target target)
  {
    OBB obb;
    ((OBB) ref obb).\u002Ector(placement.position, Vector3.get_one(), placement.rotation, this.bounds);
    Vector3 center = target.player.GetCenter(true);
    Vector3 origin = ((Ray) ref target.ray).get_origin();
    Vector3 vector3_1 = origin;
    if (Physics.Linecast(center, vector3_1, 65536, (QueryTriggerInteraction) 1))
      return false;
    RaycastHit raycastHit;
    Vector3 vector3_2 = ((OBB) ref obb).Trace(target.ray, ref raycastHit, float.PositiveInfinity) ? ((RaycastHit) ref raycastHit).get_point() : ((OBB) ref obb).ClosestPoint(origin);
    return !Physics.Linecast(origin, vector3_2, 65536, (QueryTriggerInteraction) 1);
  }

  private static bool TestPlacingThroughWall(
    ref Construction.Placement placement,
    Transform transform,
    Construction common,
    Construction.Target target)
  {
    Vector3 vector3 = Vector3.op_Subtraction(placement.position, ((Ray) ref target.ray).get_origin());
    RaycastHit hit;
    if (!Physics.Raycast(((Ray) ref target.ray).get_origin(), ((Vector3) ref vector3).get_normalized(), ref hit, ((Vector3) ref vector3).get_magnitude(), 2097152))
      return true;
    StabilityEntity entity = hit.GetEntity() as StabilityEntity;
    if (Object.op_Inequality((Object) entity, (Object) null) && Object.op_Equality((Object) target.entity, (Object) entity) || (double) ((Vector3) ref vector3).get_magnitude() - (double) ((RaycastHit) ref hit).get_distance() < 0.200000002980232)
      return true;
    Construction.lastPlacementError = "object in placement path";
    transform.set_position(((RaycastHit) ref hit).get_point());
    transform.set_rotation(placement.rotation);
    return false;
  }

  public class Grade
  {
    public BuildingGrade grade;
    public float maxHealth;
    public List<ItemAmount> costToBuild;

    public PhysicMaterial physicMaterial
    {
      get
      {
        return this.grade.physicMaterial;
      }
    }

    public ProtectionProperties damageProtecton
    {
      get
      {
        return this.grade.damageProtecton;
      }
    }
  }

  public struct Target
  {
    public bool valid;
    public Ray ray;
    public BaseEntity entity;
    public Socket_Base socket;
    public bool onTerrain;
    public Vector3 position;
    public Vector3 normal;
    public Vector3 rotation;
    public BasePlayer player;
    public bool inBuildingPrivilege;

    public Quaternion GetWorldRotation(bool female)
    {
      Quaternion quaternion = this.socket.rotation;
      if (((!this.socket.male ? 0 : (this.socket.female ? 1 : 0)) & (female ? 1 : 0)) != 0)
        quaternion = Quaternion.op_Multiply(this.socket.rotation, Quaternion.Euler(180f, 0.0f, 180f));
      return Quaternion.op_Multiply(((Component) this.entity).get_transform().get_rotation(), quaternion);
    }

    public Vector3 GetWorldPosition()
    {
      Matrix4x4 localToWorldMatrix = ((Component) this.entity).get_transform().get_localToWorldMatrix();
      return ((Matrix4x4) ref localToWorldMatrix).MultiplyPoint3x4(this.socket.position);
    }
  }

  public class Placement
  {
    public Vector3 position;
    public Quaternion rotation;
  }
}
