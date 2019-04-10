// Decompiled with JetBrains decompiler
// Type: BaseVehicle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseVehicle : BaseMountable
{
  [Tooltip("Allow players to mount other mountables/ladders from this vehicle")]
  public bool mountChaining = true;
  public GameObjectRef serverGibs;
  public bool shouldShowHudHealth;
  [Header("Mount Points")]
  public BaseVehicle.MountPointInfo[] mountPoints;
  public const BaseEntity.Flags Flag_Headlights = BaseEntity.Flags.Reserved5;
  public bool seatClipCheck;

  public override bool DirectlyMountable()
  {
    return true;
  }

  public bool HasAnyPassengers()
  {
    foreach (BaseVehicle.MountPointInfo mountPoint in this.mountPoints)
    {
      if (Object.op_Inequality((Object) mountPoint.mountable, (Object) null) && Object.op_Implicit((Object) mountPoint.mountable.GetMounted()))
        return true;
    }
    return false;
  }

  public override void VehicleFixedUpdate()
  {
    base.VehicleFixedUpdate();
    if (!this.seatClipCheck || !this.HasAnyPassengers())
      return;
    Vector3 vector3 = ((Component) this).get_transform().TransformPoint(((Bounds) ref this.bounds).get_center());
    int num1 = 1210122497;
    Vector3 extents = ((Bounds) ref this.bounds).get_extents();
    Quaternion rotation = ((Component) this).get_transform().get_rotation();
    int num2 = num1;
    if (Physics.OverlapBox(vector3, extents, rotation, num2).Length == 0)
      return;
    this.CheckSeatsForClipping();
  }

  public virtual bool IsSeatVisible(BaseMountable mountable, Vector3 eyePos, int mask = 1218511105)
  {
    if (Object.op_Equality((Object) mountable, (Object) null))
      return false;
    Vector3 p1 = Vector3.op_Addition(((Component) mountable).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_up(), 0.15f));
    return GamePhysics.LineOfSight(eyePos, p1, mask, 0.0f);
  }

  public virtual bool IsSeatClipping(BaseMountable mountable, int mask = 1218511105)
  {
    if (Object.op_Equality((Object) mountable, (Object) null))
      return false;
    Vector3 position1 = ((Component) mountable).get_transform().get_position();
    Vector3 position2 = ((Component) mountable.eyeOverride).get_transform().get_position();
    Vector3 vector3 = Vector3.op_Multiply(((Component) this).get_transform().get_up(), 0.15f);
    Vector3 end = Vector3.op_Addition(position1, vector3);
    return GamePhysics.CheckCapsule(position2, end, 0.1f, mask, (QueryTriggerInteraction) 0);
  }

  public virtual void CheckSeatsForClipping()
  {
    foreach (BaseVehicle.MountPointInfo mountPoint in this.mountPoints)
    {
      BaseMountable mountable = mountPoint.mountable;
      if (!Object.op_Equality((Object) mountable, (Object) null) && mountable.IsMounted() && this.IsSeatClipping(mountable, 1210122497))
        this.SeatClippedWorld(mountable);
    }
  }

  public virtual void SeatClippedWorld(BaseMountable mountable)
  {
    mountable.DismountPlayer(mountable.GetMounted(), false);
  }

  public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
  {
  }

  public override void DismountAllPlayers()
  {
    foreach (BaseVehicle.MountPointInfo mountPoint in this.mountPoints)
    {
      if (Object.op_Inequality((Object) mountPoint.mountable, (Object) null))
        mountPoint.mountable.DismountAllPlayers();
    }
  }

  public override void ServerInit()
  {
    base.ServerInit();
  }

  public virtual void SpawnSubEntities()
  {
    for (int index = 0; index < this.mountPoints.Length; ++index)
    {
      BaseVehicle.MountPointInfo mountPoint = this.mountPoints[index];
      Vector3 vector3 = Quaternion.op_Multiply(Quaternion.Euler(mountPoint.rot), Vector3.get_forward());
      BaseEntity entity = GameManager.server.CreateEntity(mountPoint.prefab.resourcePath, mountPoint.pos, Quaternion.LookRotation(vector3, Vector3.get_up()), true);
      entity.Spawn();
      entity.SetParent((BaseEntity) this, false, false);
      mountPoint.mountable = (BaseMountable) ((Component) entity).GetComponent<BaseMountable>();
    }
  }

  public override void Spawn()
  {
    base.Spawn();
    this.SpawnSubEntities();
  }

  public bool HasDriver()
  {
    if (!this.HasMountPoints())
      return this.IsMounted();
    BaseVehicle.MountPointInfo mountPoint = this.mountPoints[0];
    if (mountPoint == null || Object.op_Equality((Object) mountPoint.mountable, (Object) null))
      return false;
    return mountPoint.mountable.IsMounted();
  }

  public int GetPlayerSeat(BasePlayer player)
  {
    if (!this.HasMountPoints() && Object.op_Equality((Object) this.GetMounted(), (Object) player))
      return 0;
    for (int index = 0; index < this.mountPoints.Length; ++index)
    {
      BaseVehicle.MountPointInfo mountPoint = this.mountPoints[index];
      if (Object.op_Inequality((Object) mountPoint.mountable, (Object) null) && Object.op_Equality((Object) mountPoint.mountable.GetMounted(), (Object) player))
        return index;
    }
    return -1;
  }

  public void SwapSeats(BasePlayer player, int targetSeat = 0)
  {
    if (!this.HasMountPoints())
      return;
    int playerSeat = this.GetPlayerSeat(player);
    if (playerSeat == -1)
      return;
    BaseMountable mountable = this.mountPoints[playerSeat].mountable;
    int index1 = playerSeat;
    BaseMountable baseMountable = (BaseMountable) null;
    if (Object.op_Equality((Object) baseMountable, (Object) null))
    {
      for (int index2 = 0; index2 < this.mountPoints.Length; ++index2)
      {
        ++index1;
        if (index1 >= this.mountPoints.Length)
          index1 = 0;
        BaseVehicle.MountPointInfo mountPoint = this.mountPoints[index1];
        if (Object.op_Inequality((Object) mountPoint.mountable, (Object) null) && !mountPoint.mountable.IsMounted() && (!this.IsSeatClipping(mountPoint.mountable, 1218511105) && this.IsSeatVisible(mountPoint.mountable, player.eyes.position, 1218511105)))
        {
          baseMountable = mountPoint.mountable;
          break;
        }
      }
    }
    if (!Object.op_Inequality((Object) baseMountable, (Object) null) || !Object.op_Inequality((Object) baseMountable, (Object) mountable))
      return;
    mountable.DismountPlayer(player, true);
    baseMountable.MountPlayer(player);
    player.MarkSwapSeat();
  }

  public bool HasMountPoints()
  {
    return (uint) this.mountPoints.Length > 0U;
  }

  public BaseMountable GetIdealMountPoint(Vector3 pos)
  {
    if (!this.HasMountPoints())
      return (BaseMountable) null;
    BaseMountable baseMountable = (BaseMountable) null;
    float num1 = float.PositiveInfinity;
    foreach (BaseVehicle.MountPointInfo mountPoint in this.mountPoints)
    {
      float num2 = Vector3.Distance(mountPoint.mountable.mountAnchor.get_position(), pos);
      if ((double) num2 < (double) num1 && !mountPoint.mountable.IsMounted() && (!this.IsSeatClipping(mountPoint.mountable, 1218511105) && this.IsSeatVisible(mountPoint.mountable, pos, 1218511105)))
      {
        baseMountable = mountPoint.mountable;
        num1 = num2;
      }
    }
    return baseMountable;
  }

  public override bool IsMounted()
  {
    return this.HasDriver();
  }

  public virtual bool MountEligable()
  {
    return true;
  }

  public override void AttemptMount(BasePlayer player)
  {
    if (Object.op_Inequality((Object) this._mounted, (Object) null) || !this.MountEligable())
      return;
    BaseMountable idealMountPoint = this.GetIdealMountPoint(player.eyes.position);
    if (Object.op_Equality((Object) idealMountPoint, (Object) null))
      return;
    if (Object.op_Equality((Object) idealMountPoint, (Object) this))
      base.AttemptMount(player);
    else
      idealMountPoint.AttemptMount(player);
  }

  public override bool AttemptDismount(BasePlayer player)
  {
    if (Object.op_Inequality((Object) player, (Object) this._mounted))
      return false;
    this.DismountPlayer(player, false);
    return true;
  }

  public override Vector3 GetDismountPosition(BasePlayer player)
  {
    BaseVehicle baseVehicle = this.VehicleParent();
    if (Object.op_Inequality((Object) baseVehicle, (Object) null))
      return baseVehicle.GetDismountPosition(player);
    List<Vector3> list = (List<Vector3>) Pool.GetList<Vector3>();
    foreach (Transform dismountPosition in this.dismountPositions)
    {
      if (this.ValidDismountPosition(((Component) dismountPosition).get_transform().get_position()))
        list.Add(((Component) dismountPosition).get_transform().get_position());
    }
    if (list.Count == 0)
    {
      Debug.LogWarning((object) ("Failed to find dismount position for player :" + player.displayName + " / " + (object) player.userID + " on obj : " + ((Object) ((Component) this).get_gameObject()).get_name()));
      // ISSUE: cast to a reference type
      Pool.FreeList<Vector3>((List<M0>&) ref list);
      return BaseMountable.DISMOUNT_POS_INVALID;
    }
    Vector3 pos = ((Component) player).get_transform().get_position();
    list.Sort((Comparison<Vector3>) ((a, b) => Vector3.Distance(a, pos).CompareTo(Vector3.Distance(b, pos))));
    Vector3 vector3 = list[0];
    // ISSUE: cast to a reference type
    Pool.FreeList<Vector3>((List<M0>&) ref list);
    return vector3;
  }

  public override bool SupportsChildDeployables()
  {
    return false;
  }

  [Serializable]
  public class MountPointInfo
  {
    public Vector3 pos;
    public Vector3 rot;
    public GameObjectRef prefab;
    public BaseMountable mountable;
  }
}
