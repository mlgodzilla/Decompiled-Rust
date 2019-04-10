// Decompiled with JetBrains decompiler
// Type: ServerGib
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerGib : BaseCombatEntity
{
  public GameObject _gibSource;
  public string _gibName;
  public PhysicMaterial physicsMaterial;
  private MeshCollider meshCollider;
  private Rigidbody rigidBody;

  public override float BoundsPadding()
  {
    return 3f;
  }

  public static List<ServerGib> CreateGibs(
    string entityToCreatePath,
    GameObject creator,
    GameObject gibSource,
    Vector3 inheritVelocity,
    float spreadVelocity)
  {
    List<ServerGib> serverGibList = new List<ServerGib>();
    foreach (MeshRenderer componentsInChild in (MeshRenderer[]) gibSource.GetComponentsInChildren<MeshRenderer>(true))
    {
      MeshFilter component1 = (MeshFilter) ((Component) componentsInChild).GetComponent<MeshFilter>();
      Vector3 localPosition1 = ((Component) componentsInChild).get_transform().get_localPosition();
      Vector3 normalized = ((Vector3) ref localPosition1).get_normalized();
      Matrix4x4 localToWorldMatrix = creator.get_transform().get_localToWorldMatrix();
      Vector3 pos = Vector3.op_Addition(((Matrix4x4) ref localToWorldMatrix).MultiplyPoint(((Component) componentsInChild).get_transform().get_localPosition()), Vector3.op_Multiply(normalized, 0.5f));
      Quaternion rot = Quaternion.op_Multiply(creator.get_transform().get_rotation(), ((Component) componentsInChild).get_transform().get_localRotation());
      BaseEntity entity = GameManager.server.CreateEntity(entityToCreatePath, pos, rot, true);
      if (Object.op_Implicit((Object) entity))
      {
        ServerGib component2 = (ServerGib) ((Component) entity).GetComponent<ServerGib>();
        ((Component) component2).get_transform().set_position(pos);
        ((Component) component2).get_transform().set_rotation(rot);
        component2._gibName = ((Object) componentsInChild).get_name();
        component2.PhysicsInit(component1.get_sharedMesh());
        Vector3 localPosition2 = ((Component) componentsInChild).get_transform().get_localPosition();
        Vector3 vector3_1 = Vector3.op_Multiply(((Vector3) ref localPosition2).get_normalized(), spreadVelocity);
        component2.rigidBody.set_velocity(Vector3.op_Addition(inheritVelocity, vector3_1));
        Rigidbody rigidBody = component2.rigidBody;
        Vector3 vector3_2 = Vector3Ex.Range(-1f, 1f);
        Vector3 vector3_3 = Vector3.op_Multiply(((Vector3) ref vector3_2).get_normalized(), 1f);
        rigidBody.set_angularVelocity(vector3_3);
        component2.rigidBody.WakeUp();
        component2.Spawn();
        serverGibList.Add(component2);
      }
    }
    foreach (ServerGib serverGib1 in serverGibList)
    {
      foreach (ServerGib serverGib2 in serverGibList)
      {
        if (!Object.op_Equality((Object) serverGib1, (Object) serverGib2))
          Physics.IgnoreCollision((Collider) serverGib2.GetCollider(), (Collider) serverGib1.GetCollider(), true);
      }
    }
    return serverGibList;
  }

  public override void Save(BaseNetworkable.SaveInfo info)
  {
    base.Save(info);
    if (info.forDisk || !(this._gibName != ""))
      return;
    info.msg.servergib = (__Null) new ServerGib();
    ((ServerGib) info.msg.servergib).gibName = (__Null) this._gibName;
  }

  public MeshCollider GetCollider()
  {
    return this.meshCollider;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.Invoke(new Action(this.RemoveMe), 1800f);
  }

  public void RemoveMe()
  {
    this.Kill(BaseNetworkable.DestroyMode.None);
  }

  public virtual void PhysicsInit(Mesh mesh)
  {
    this.meshCollider = (MeshCollider) ((Component) this).get_gameObject().AddComponent<MeshCollider>();
    this.meshCollider.set_sharedMesh(mesh);
    this.meshCollider.set_convex(true);
    ((Collider) this.meshCollider).set_material(this.physicsMaterial);
    Rigidbody rigidbody1 = (Rigidbody) ((Component) this).get_gameObject().AddComponent<Rigidbody>();
    rigidbody1.set_useGravity(true);
    Rigidbody rigidbody2 = rigidbody1;
    Bounds bounds1 = ((Collider) this.meshCollider).get_bounds();
    Vector3 size = ((Bounds) ref bounds1).get_size();
    double magnitude1 = (double) ((Vector3) ref size).get_magnitude();
    Bounds bounds2 = ((Collider) this.meshCollider).get_bounds();
    size = ((Bounds) ref bounds2).get_size();
    double magnitude2 = (double) ((Vector3) ref size).get_magnitude();
    double num = (double) Mathf.Clamp((float) (magnitude1 * magnitude2 * 20.0), 10f, 2000f);
    rigidbody2.set_mass((float) num);
    rigidbody1.set_interpolation((RigidbodyInterpolation) 1);
    if (this.isServer)
    {
      rigidbody1.set_drag(0.1f);
      rigidbody1.set_angularDrag(0.1f);
    }
    this.rigidBody = rigidbody1;
    ((Component) this).get_gameObject().set_layer(LayerMask.NameToLayer("Default"));
    if (!this.isClient)
      return;
    rigidbody1.set_isKinematic(true);
  }
}
