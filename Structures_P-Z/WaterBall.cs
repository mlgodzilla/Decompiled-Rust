// Decompiled with JetBrains decompiler
// Type: WaterBall
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterBall : BaseEntity
{
  public ItemDefinition liquidType;
  public int waterAmount;
  public GameObjectRef waterExplosion;
  public Rigidbody myRigidBody;

  public override void ServerInit()
  {
    base.ServerInit();
    this.Invoke(new Action(this.Extinguish), 10f);
  }

  public void Extinguish()
  {
    this.CancelInvoke(new Action(this.Extinguish));
    if (this.IsDestroyed)
      return;
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public void FixedUpdate()
  {
    if (!this.isServer)
      return;
    ((Rigidbody) ((Component) this).GetComponent<Rigidbody>()).AddForce(Physics.get_gravity(), (ForceMode) 5);
  }

  public void DoSplash()
  {
    float radius = 2.5f;
    List<BaseEntity> list1 = (List<BaseEntity>) Pool.GetList<BaseEntity>();
    Vis.Entities<BaseEntity>(Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, radius * 0.75f, 0.0f)), radius, list1, 1219701521, (QueryTriggerInteraction) 2);
    for (int index = 0; this.waterAmount > 0 && index < 3; ++index)
    {
      List<ISplashable> list2 = (List<ISplashable>) Pool.GetList<ISplashable>();
      foreach (BaseEntity baseEntity in list1)
      {
        if (!baseEntity.isClient)
        {
          ISplashable splashable = baseEntity as ISplashable;
          if (splashable != null && !list2.Contains(splashable) && splashable.wantsSplash(this.liquidType, this.waterAmount))
            list2.Add(splashable);
        }
      }
      if (list2.Count != 0)
      {
        int num = Mathf.CeilToInt((float) (this.waterAmount / list2.Count));
        foreach (ISplashable splashable in list2)
        {
          this.waterAmount -= splashable.DoSplash(this.liquidType, Mathf.Min(this.waterAmount, num));
          if (this.waterAmount <= 0)
            break;
        }
        // ISSUE: cast to a reference type
        Pool.FreeList<ISplashable>((List<M0>&) ref list2);
      }
      else
        break;
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BaseEntity>((List<M0>&) ref list1);
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (this.isClient || this.myRigidBody.get_isKinematic())
      return;
    this.DoSplash();
    Effect.server.Run(this.waterExplosion.resourcePath, Vector3.op_Addition(((Component) this).get_transform().get_position(), new Vector3(0.0f, 0.0f, 0.0f)), Vector3.get_up(), (Connection) null, false);
    this.myRigidBody.set_isKinematic(true);
    this.Invoke(new Action(this.Extinguish), 2f);
  }
}
