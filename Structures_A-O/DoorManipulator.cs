// Decompiled with JetBrains decompiler
// Type: DoorManipulator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorManipulator : IOEntity
{
  private bool toggle = true;
  public EntityRef entityRef;
  public Door targetDoor;
  public DoorManipulator.DoorEffect powerAction;

  public virtual bool PairWithLockedDoors()
  {
    return true;
  }

  public virtual void SetTargetDoor(Door newTargetDoor)
  {
    Door targetDoor1 = this.targetDoor;
    this.targetDoor = newTargetDoor;
    this.SetFlag(BaseEntity.Flags.On, Object.op_Inequality((Object) this.targetDoor, (Object) null), false, true);
    this.entityRef.Set((BaseEntity) newTargetDoor);
    Door targetDoor2 = this.targetDoor;
    if (!Object.op_Inequality((Object) targetDoor1, (Object) targetDoor2) || !Object.op_Inequality((Object) this.targetDoor, (Object) null))
      return;
    this.DoAction();
  }

  public virtual void SetupInitialDoorConnection()
  {
    if (Object.op_Equality((Object) this.targetDoor, (Object) null) && !this.entityRef.IsValid(true))
      this.SetTargetDoor(this.FindDoor(this.PairWithLockedDoors()));
    if (Object.op_Inequality((Object) this.targetDoor, (Object) null) && !this.entityRef.IsValid(true))
      this.entityRef.Set((BaseEntity) this.targetDoor);
    if (!this.entityRef.IsValid(true) || !Object.op_Equality((Object) this.targetDoor, (Object) null))
      return;
    this.SetTargetDoor((Door) ((Component) this.entityRef.Get(true)).GetComponent<Door>());
  }

  public override void Init()
  {
    base.Init();
    this.SetupInitialDoorConnection();
  }

  public Door FindDoor(bool allowLocked = true)
  {
    List<Door> list = (List<Door>) Pool.GetList<Door>();
    Vis.Entities<Door>(((Component) this).get_transform().get_position(), 1f, list, 2097152, (QueryTriggerInteraction) 1);
    Door door1 = (Door) null;
    float num1 = float.PositiveInfinity;
    foreach (Door door2 in list)
    {
      if (door2.isServer)
      {
        if (!allowLocked)
        {
          BaseLock slot = door2.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
          if (Object.op_Inequality((Object) slot, (Object) null) && slot.IsLocked())
            continue;
        }
        float num2 = Vector3.Distance(((Component) door2).get_transform().get_position(), ((Component) this).get_transform().get_position());
        if ((double) num2 < (double) num1)
        {
          door1 = door2;
          num1 = num2;
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Door>((List<M0>&) ref list);
    return door1;
  }

  public virtual void DoActionDoorMissing()
  {
    this.SetTargetDoor(this.FindDoor(this.PairWithLockedDoors()));
  }

  public void DoAction()
  {
    bool flag = this.IsPowered();
    if (Object.op_Equality((Object) this.targetDoor, (Object) null))
      this.DoActionDoorMissing();
    if (!Object.op_Inequality((Object) this.targetDoor, (Object) null))
      return;
    if (this.targetDoor.IsBusy())
      this.Invoke(new Action(this.DoAction), 1f);
    else if (this.powerAction == DoorManipulator.DoorEffect.Open)
    {
      if (flag)
      {
        if (this.targetDoor.IsOpen())
          return;
        this.targetDoor.SetOpen(true);
      }
      else
      {
        if (!this.targetDoor.IsOpen())
          return;
        this.targetDoor.SetOpen(false);
      }
    }
    else if (this.powerAction == DoorManipulator.DoorEffect.Close)
    {
      if (flag)
      {
        if (!this.targetDoor.IsOpen())
          return;
        this.targetDoor.SetOpen(false);
      }
      else
      {
        if (this.targetDoor.IsOpen())
          return;
        this.targetDoor.SetOpen(true);
      }
    }
    else
    {
      if (this.powerAction != DoorManipulator.DoorEffect.Toggle)
        return;
      if (flag && this.toggle)
      {
        this.targetDoor.SetOpen(!this.targetDoor.IsOpen());
        this.toggle = false;
      }
      else
      {
        if (this.toggle)
          return;
        this.toggle = true;
      }
    }
  }

  public override void IOStateChanged(int inputAmount, int inputSlot)
  {
    base.IOStateChanged(inputAmount, inputSlot);
    this.DoAction();
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    ((IOEntity) info.msg.ioEntity).genericEntRef1 = (__Null) (int) this.entityRef.uid;
    ((IOEntity) info.msg.ioEntity).genericInt1 = (__Null) this.powerAction;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.ioEntity == null)
      return;
    this.entityRef.uid = (uint) ((IOEntity) info.msg.ioEntity).genericEntRef1;
    this.powerAction = (DoorManipulator.DoorEffect) ((IOEntity) info.msg.ioEntity).genericInt1;
  }

  public enum DoorEffect
  {
    Close,
    Open,
    Toggle,
  }
}
