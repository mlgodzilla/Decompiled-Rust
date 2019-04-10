// Decompiled with JetBrains decompiler
// Type: SlidingProgressDoor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class SlidingProgressDoor : ProgressDoor
{
  public Vector3 openPosition;
  public Vector3 closedPosition;
  public GameObject doorObject;
  private float lastEnergyTime;
  private float lastServerUpdateTime;

  public override void Spawn()
  {
    base.Spawn();
    this.InvokeRepeating(new Action(this.ServerUpdate), 0.0f, 0.1f);
  }

  public override void NoEnergy()
  {
    base.NoEnergy();
  }

  public override void AddEnergy(float amount)
  {
    this.lastEnergyTime = Time.get_time();
    base.AddEnergy(amount);
  }

  public void ServerUpdate()
  {
    if (!this.isServer)
      return;
    if ((double) this.lastServerUpdateTime == 0.0)
      this.lastServerUpdateTime = Time.get_realtimeSinceStartup();
    float num1 = Time.get_realtimeSinceStartup() - this.lastServerUpdateTime;
    this.lastServerUpdateTime = Time.get_realtimeSinceStartup();
    if ((double) Time.get_time() > (double) this.lastEnergyTime + 0.333000004291534)
    {
      float num2 = Mathf.Min(this.storedEnergy, this.energyForOpen * num1 / this.secondsToClose);
      this.storedEnergy -= num2;
      this.storedEnergy = Mathf.Clamp(this.storedEnergy, 0.0f, this.energyForOpen);
      if ((double) num2 > 0.0)
      {
        foreach (IOEntity.IOSlot output in this.outputs)
        {
          if (Object.op_Inequality((Object) output.connectedTo.Get(true), (Object) null))
          {
            double num3 = (double) output.connectedTo.Get(true).IOInput((IOEntity) this, this.ioType, -num2, output.connectedToSlot);
          }
        }
      }
    }
    this.UpdateProgress();
  }

  public override void UpdateProgress()
  {
    base.UpdateProgress();
    Vector3 localPosition = this.doorObject.get_transform().get_localPosition();
    Vector3 vector3 = Vector3.Lerp(this.closedPosition, this.openPosition, this.storedEnergy / this.energyForOpen);
    this.doorObject.get_transform().set_localPosition(vector3);
    if (!this.isServer)
      return;
    this.SetFlag(BaseEntity.Flags.Reserved1, (double) Vector3.Distance(localPosition, vector3) > 0.00999999977648258, false, true);
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    // ISSUE: variable of the null type
    __Null sphereEntity = info.msg.sphereEntity;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.sphereEntity = (__Null) Pool.Get<SphereEntity>();
    ((SphereEntity) info.msg.sphereEntity).radius = (__Null) (double) this.storedEnergy;
  }
}
