// Decompiled with JetBrains decompiler
// Type: BaseCorpse
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using ProtoBuf;
using System;
using UnityEngine;

public class BaseCorpse : BaseCombatEntity
{
  public GameObjectRef prefabRagdoll;
  public BaseEntity parentEnt;
  [NonSerialized]
  internal ResourceDispenser resourceDispenser;

  public override void ServerInit()
  {
    this.SetupRigidBody();
    this.ResetRemovalTime();
    this.resourceDispenser = (ResourceDispenser) ((Component) this).GetComponent<ResourceDispenser>();
    base.ServerInit();
  }

  public virtual void InitCorpse(BaseEntity pr)
  {
    this.parentEnt = pr;
    ((Component) this).get_transform().set_position(this.parentEnt.CenterPoint());
    ((Component) this).get_transform().set_rotation(((Component) this.parentEnt).get_transform().get_rotation());
  }

  public virtual bool CanRemove()
  {
    return true;
  }

  public void RemoveCorpse()
  {
    if (!this.CanRemove())
      this.ResetRemovalTime();
    else
      this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public void ResetRemovalTime(float dur)
  {
    using (TimeWarning.New(nameof (ResetRemovalTime), 0.1f))
    {
      if (this.IsInvoking(new Action(this.RemoveCorpse)))
        this.CancelInvoke(new Action(this.RemoveCorpse));
      this.Invoke(new Action(this.RemoveCorpse), dur);
    }
  }

  public virtual float GetRemovalTime()
  {
    return Server.corpsedespawn;
  }

  public void ResetRemovalTime()
  {
    this.ResetRemovalTime(this.GetRemovalTime());
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    info.msg.corpse = (__Null) Pool.Get<Corpse>();
    if (!this.parentEnt.IsValid())
      return;
    ((Corpse) info.msg.corpse).parentID = this.parentEnt.net.ID;
  }

  public void TakeChildren(BaseEntity takeChildrenFrom)
  {
    if (takeChildrenFrom.children == null)
      return;
    using (TimeWarning.New("Corpse.TakeChildren", 0.1f))
    {
      foreach (BaseEntity baseEntity in takeChildrenFrom.children.ToArray())
        baseEntity.SwitchParent((BaseEntity) this);
    }
  }

  private Rigidbody SetupRigidBody()
  {
    if (this.isServer)
    {
      GameObject prefab = this.gameManager.FindPrefab(this.prefabRagdoll.resourcePath);
      if (Object.op_Equality((Object) prefab, (Object) null))
        return (Rigidbody) null;
      Ragdoll component1 = (Ragdoll) prefab.GetComponent<Ragdoll>();
      if (Object.op_Equality((Object) component1, (Object) null))
        return (Rigidbody) null;
      if (Object.op_Equality((Object) component1.primaryBody, (Object) null))
      {
        Debug.LogError((object) ("[BaseCorpse] ragdoll.primaryBody isn't set!" + ((Object) ((Component) component1).get_gameObject()).get_name()));
        return (Rigidbody) null;
      }
      BoxCollider component2 = (BoxCollider) ((Component) component1.primaryBody).GetComponent<BoxCollider>();
      if (Object.op_Equality((Object) component2, (Object) null))
      {
        Debug.LogError((object) "Ragdoll has unsupported primary collider (make it supported) ", (Object) component1);
        return (Rigidbody) null;
      }
      M0 m0 = ((Component) this).get_gameObject().AddComponent<BoxCollider>();
      ((BoxCollider) m0).set_size(Vector3.op_Multiply(component2.get_size(), 2f));
      ((BoxCollider) m0).set_center(component2.get_center());
      ((Collider) m0).set_sharedMaterial(((Collider) component2).get_sharedMaterial());
    }
    Rigidbody rigidBody = (Rigidbody) ((Component) this).get_gameObject().GetComponent<Rigidbody>();
    if (Object.op_Equality((Object) rigidBody, (Object) null))
      rigidBody = (Rigidbody) ((Component) this).get_gameObject().AddComponent<Rigidbody>();
    rigidBody.set_mass(10f);
    rigidBody.set_useGravity(true);
    rigidBody.set_drag(0.5f);
    rigidBody.set_collisionDetectionMode((CollisionDetectionMode) 0);
    if (this.isServer)
    {
      Buoyancy component = (Buoyancy) ((Component) this).GetComponent<Buoyancy>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.rigidBody = rigidBody;
      Physics.ApplyDropped(rigidBody);
      Vector3 vector3 = Vector3Ex.Range(-1f, 1f);
      ref __Null local = ref vector3.y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local + 1f;
      rigidBody.set_velocity(vector3);
      rigidBody.set_angularVelocity(Vector3Ex.Range(-10f, 10f));
    }
    return rigidBody;
  }

  public override void Load(BaseNetworkable.LoadInfo info)
  {
    base.Load(info);
    if (info.msg.corpse == null)
      return;
    this.Load((Corpse) info.msg.corpse);
  }

  private void Load(Corpse corpse)
  {
    if (this.isServer)
      this.parentEnt = BaseNetworkable.serverEntities.Find((uint) corpse.parentID) as BaseEntity;
    int num = this.isClient ? 1 : 0;
  }

  public override void OnAttacked(HitInfo info)
  {
    if (!this.isServer)
      return;
    this.ResetRemovalTime();
    if (Object.op_Implicit((Object) this.resourceDispenser))
      this.resourceDispenser.OnAttacked(info);
    if (info.DidGather)
      return;
    base.OnAttacked(info);
  }

  public override string Categorize()
  {
    return "corpse";
  }

  public override BaseEntity.TraitFlag Traits
  {
    get
    {
      return base.Traits | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat;
    }
  }

  public override void Eat(BaseNpc baseNpc, float timeSpent)
  {
    this.ResetRemovalTime();
    this.Hurt(timeSpent * 5f);
    baseNpc.AddCalories(timeSpent * 2f);
  }
}
