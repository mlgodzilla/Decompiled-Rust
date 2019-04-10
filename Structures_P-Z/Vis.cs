// Decompiled with JetBrains decompiler
// Type: Vis
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public static class Vis
{
  public static Collider[] colBuffer = new Collider[8192];

  public static void Colliders<T>(
    Vector3 position,
    float radius,
    List<T> list,
    int layerMask = -1,
    QueryTriggerInteraction triggerInteraction = 2)
    where T : Collider
  {
    layerMask = GamePhysics.HandleTerrainCollision(position, layerMask);
    int num = Physics.OverlapSphereNonAlloc(position, radius, Vis.colBuffer, layerMask, triggerInteraction);
    if (num >= Vis.colBuffer.Length)
      Debug.LogWarning((object) "Vis query is exceeding collider buffer length.");
    for (int index = 0; index < num; ++index)
    {
      T obj = Vis.colBuffer[index] as T;
      Vis.colBuffer[index] = (Collider) null;
      if (!Object.op_Equality((Object) (object) obj, (Object) null) && ((Collider) (object) obj).get_enabled())
      {
        if (((Component) ((Component) (object) obj).get_transform()).CompareTag("MeshColliderBatch"))
          ((MeshColliderBatch) ((Component) ((Component) (object) obj).get_transform()).GetComponent<MeshColliderBatch>()).LookupColliders<T>(position, radius, list);
        else
          list.Add(obj);
      }
    }
  }

  public static void Components<T>(
    Vector3 position,
    float radius,
    List<T> list,
    int layerMask = -1,
    QueryTriggerInteraction triggerInteraction = 2)
    where T : Component
  {
    List<Collider> list1 = (List<Collider>) Pool.GetList<Collider>();
    Vis.Colliders<Collider>(position, radius, list1, layerMask, triggerInteraction);
    for (int index = 0; index < list1.Count; ++index)
    {
      T component = ((Component) list1[index]).get_gameObject().GetComponent<T>();
      if (!Object.op_Equality((Object) (object) component, (Object) null))
        list.Add(component);
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list1);
  }

  public static void Entities<T>(
    Vector3 position,
    float radius,
    List<T> list,
    int layerMask = -1,
    QueryTriggerInteraction triggerInteraction = 2)
    where T : BaseEntity
  {
    List<Collider> list1 = (List<Collider>) Pool.GetList<Collider>();
    Vis.Colliders<Collider>(position, radius, list1, layerMask, triggerInteraction);
    for (int index = 0; index < list1.Count; ++index)
    {
      T baseEntity = ((Component) list1[index]).get_gameObject().ToBaseEntity() as T;
      if (!Object.op_Equality((Object) (object) baseEntity, (Object) null))
        list.Add(baseEntity);
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list1);
  }

  public static void EntityComponents<T>(
    Vector3 position,
    float radius,
    List<T> list,
    int layerMask = -1,
    QueryTriggerInteraction triggerInteraction = 2)
    where T : EntityComponentBase
  {
    List<Collider> list1 = (List<Collider>) Pool.GetList<Collider>();
    Vis.Colliders<Collider>(position, radius, list1, layerMask, triggerInteraction);
    for (int index = 0; index < list1.Count; ++index)
    {
      BaseEntity baseEntity = ((Component) list1[index]).get_gameObject().ToBaseEntity();
      if (!Object.op_Equality((Object) baseEntity, (Object) null))
      {
        T component = ((Component) baseEntity).get_gameObject().GetComponent<T>();
        if (!Object.op_Equality((Object) (object) component, (Object) null))
          list.Add(component);
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list1);
  }

  public static void Colliders<T>(
    OBB bounds,
    List<T> list,
    int layerMask = -1,
    QueryTriggerInteraction triggerInteraction = 2)
    where T : Collider
  {
    layerMask = GamePhysics.HandleTerrainCollision((Vector3) bounds.position, layerMask);
    int num = Physics.OverlapBoxNonAlloc((Vector3) bounds.position, (Vector3) bounds.extents, Vis.colBuffer, (Quaternion) bounds.rotation, layerMask, triggerInteraction);
    if (num >= Vis.colBuffer.Length)
      Debug.LogWarning((object) "Vis query is exceeding collider buffer length.");
    for (int index = 0; index < num; ++index)
    {
      T obj = Vis.colBuffer[index] as T;
      Vis.colBuffer[index] = (Collider) null;
      if (!Object.op_Equality((Object) (object) obj, (Object) null) && ((Collider) (object) obj).get_enabled())
      {
        if (((Component) ((Component) (object) obj).get_transform()).CompareTag("MeshColliderBatch"))
          ((MeshColliderBatch) ((Component) ((Component) (object) obj).get_transform()).GetComponent<MeshColliderBatch>()).LookupColliders<T>(bounds, list);
        else
          list.Add(obj);
      }
    }
  }

  public static void Components<T>(
    OBB bounds,
    List<T> list,
    int layerMask = -1,
    QueryTriggerInteraction triggerInteraction = 2)
    where T : Component
  {
    List<Collider> list1 = (List<Collider>) Pool.GetList<Collider>();
    Vis.Colliders<Collider>(bounds, list1, layerMask, triggerInteraction);
    for (int index = 0; index < list1.Count; ++index)
    {
      T component = ((Component) list1[index]).get_gameObject().GetComponent<T>();
      if (!Object.op_Equality((Object) (object) component, (Object) null))
        list.Add(component);
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list1);
  }

  public static void Entities<T>(
    OBB bounds,
    List<T> list,
    int layerMask = -1,
    QueryTriggerInteraction triggerInteraction = 2)
    where T : BaseEntity
  {
    List<Collider> list1 = (List<Collider>) Pool.GetList<Collider>();
    Vis.Colliders<Collider>(bounds, list1, layerMask, triggerInteraction);
    for (int index = 0; index < list1.Count; ++index)
    {
      T baseEntity = ((Component) list1[index]).get_gameObject().ToBaseEntity() as T;
      if (!Object.op_Equality((Object) (object) baseEntity, (Object) null))
        list.Add(baseEntity);
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list1);
  }

  public static void EntityComponents<T>(
    OBB bounds,
    List<T> list,
    int layerMask = -1,
    QueryTriggerInteraction triggerInteraction = 2)
    where T : EntityComponentBase
  {
    List<Collider> list1 = (List<Collider>) Pool.GetList<Collider>();
    Vis.Colliders<Collider>(bounds, list1, layerMask, triggerInteraction);
    for (int index = 0; index < list1.Count; ++index)
    {
      BaseEntity baseEntity = ((Component) list1[index]).get_gameObject().ToBaseEntity();
      if (!Object.op_Equality((Object) baseEntity, (Object) null))
      {
        T component = ((Component) baseEntity).get_gameObject().GetComponent<T>();
        if (!Object.op_Equality((Object) (object) component, (Object) null))
          list.Add(component);
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list1);
  }
}
