// Decompiled with JetBrains decompiler
// Type: DroppedItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;

public class DroppedItem : WorldItem
{
  [Header("DroppedItem")]
  public GameObject itemModel;

  public override void ServerInit()
  {
    base.ServerInit();
    if ((double) this.GetDespawnDuration() < double.PositiveInfinity)
      this.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
    this.ReceiveCollisionMessages(true);
  }

  public virtual float GetDespawnDuration()
  {
    if (this.item != null && this.item.info.quickDespawn)
      return 30f;
    int num = this.item != null ? this.item.despawnMultiplier : 1;
    return Server.itemdespawn * (float) num;
  }

  public void IdleDestroy()
  {
    this.DestroyItem();
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public override void OnCollision(Collision collision, BaseEntity hitEntity)
  {
    if (this.item == null)
      return;
    DroppedItem droppedItem = hitEntity as DroppedItem;
    if (Object.op_Equality((Object) droppedItem, (Object) null) || droppedItem.item == null || Object.op_Inequality((Object) droppedItem.item.info, (Object) this.item.info))
      return;
    droppedItem.OnDroppedOn(this);
  }

  public void OnDroppedOn(DroppedItem di)
  {
    if (this.item == null || di.item == null || (Interface.CallHook("CanCombineDroppedItem", (object) this, (object) di) != null || this.item.info.stackable <= 1) || (Object.op_Inequality((Object) di.item.info, (Object) this.item.info) || di.item.IsBlueprint() && di.item.blueprintTarget != this.item.blueprintTarget))
      return;
    int num = di.item.amount + this.item.amount;
    if (num > this.item.info.stackable || num == 0)
      return;
    di.DestroyItem();
    di.Kill(BaseNetworkable.DestroyMode.None);
    this.item.amount = num;
    this.item.MarkDirty();
    if ((double) this.GetDespawnDuration() < double.PositiveInfinity)
      this.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
    Effect.server.Run("assets/bundled/prefabs/fx/notice/stack.world.fx.prefab", (BaseEntity) this, 0U, Vector3.get_zero(), Vector3.get_zero(), (Connection) null, false);
  }

  internal override void OnParentRemoved()
  {
    Rigidbody component = (Rigidbody) ((Component) this).GetComponent<Rigidbody>();
    if (Object.op_Equality((Object) component, (Object) null))
    {
      base.OnParentRemoved();
    }
    else
    {
      Vector3 vector3 = ((Component) this).get_transform().get_position();
      Quaternion rotation = ((Component) this).get_transform().get_rotation();
      this.SetParent((BaseEntity) null, false, false);
      RaycastHit raycastHit;
      if (Physics.Raycast(Vector3.op_Addition(vector3, Vector3.op_Multiply(Vector3.get_up(), 2f)), Vector3.get_down(), ref raycastHit, 2f, 27328512) && vector3.y < ((RaycastHit) ref raycastHit).get_point().y)
        vector3 = Vector3.op_Addition(vector3, Vector3.op_Multiply(Vector3.get_up(), 1.5f));
      ((Component) this).get_transform().set_position(vector3);
      ((Component) this).get_transform().set_rotation(rotation);
      Physics.ApplyDropped(component);
      component.set_isKinematic(false);
      component.set_useGravity(true);
      component.WakeUp();
      if ((double) this.GetDespawnDuration() >= double.PositiveInfinity)
        return;
      this.Invoke(new Action(this.IdleDestroy), this.GetDespawnDuration());
    }
  }

  public override void PostInitShared()
  {
    base.PostInitShared();
    GameObject go = this.item == null || !this.item.info.worldModelPrefab.isValid ? (GameObject) Object.Instantiate<GameObject>((M0) this.itemModel) : this.item.info.worldModelPrefab.Instantiate((Transform) null);
    go.get_transform().SetParent(((Component) this).get_transform(), false);
    go.get_transform().set_localPosition(Vector3.get_zero());
    go.get_transform().set_localRotation(Quaternion.get_identity());
    go.SetLayerRecursive(((Component) this).get_gameObject().get_layer());
    Collider component1 = (Collider) go.GetComponent<Collider>();
    if (Object.op_Implicit((Object) component1))
    {
      component1.set_enabled(false);
      component1.set_enabled(true);
    }
    if (this.isServer)
    {
      WorldModel component2 = (WorldModel) go.GetComponent<WorldModel>();
      float num1 = Object.op_Implicit((Object) component2) ? component2.mass : 1f;
      float num2 = 0.1f;
      float num3 = 0.1f;
      M0 m0 = ((Component) this).get_gameObject().AddComponent<Rigidbody>();
      ((Rigidbody) m0).set_mass(num1);
      ((Rigidbody) m0).set_drag(num2);
      ((Rigidbody) m0).set_angularDrag(num3);
      ((Rigidbody) m0).set_interpolation((RigidbodyInterpolation) 0);
      Physics.ApplyDropped((Rigidbody) m0);
      foreach (Renderer componentsInChild in (Renderer[]) go.GetComponentsInChildren<Renderer>(true))
        componentsInChild.set_enabled(false);
    }
    if (this.item != null)
    {
      PhysicsEffects component2 = (PhysicsEffects) ((Component) this).get_gameObject().GetComponent<PhysicsEffects>();
      if (Object.op_Inequality((Object) component2, (Object) null))
      {
        component2.entity = (BaseEntity) this;
        if (Object.op_Inequality((Object) this.item.info.physImpactSoundDef, (Object) null))
          component2.physImpactSoundDef = this.item.info.physImpactSoundDef;
      }
    }
    go.SetActive(true);
  }

  public override bool PhysicsDriven()
  {
    return true;
  }
}
