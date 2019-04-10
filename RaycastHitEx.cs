// Decompiled with JetBrains decompiler
// Type: RaycastHitEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public static class RaycastHitEx
{
  public static Transform GetTransform(this RaycastHit hit)
  {
    if (((RaycastHit) ref hit).get_triangleIndex() < 0)
      return ((RaycastHit) ref hit).get_transform();
    if (!((Component) ((RaycastHit) ref hit).get_transform()).CompareTag("MeshColliderBatch"))
      return ((RaycastHit) ref hit).get_transform();
    MeshColliderBatch component = (MeshColliderBatch) ((Component) ((RaycastHit) ref hit).get_transform()).GetComponent<MeshColliderBatch>();
    if (!Object.op_Implicit((Object) component))
      return ((RaycastHit) ref hit).get_transform();
    Transform transform = component.LookupTransform(((RaycastHit) ref hit).get_triangleIndex());
    if (!Object.op_Implicit((Object) transform))
      return ((RaycastHit) ref hit).get_transform();
    return transform;
  }

  public static Rigidbody GetRigidbody(this RaycastHit hit)
  {
    if (((RaycastHit) ref hit).get_triangleIndex() < 0)
      return ((RaycastHit) ref hit).get_rigidbody();
    if (!((Component) ((RaycastHit) ref hit).get_transform()).CompareTag("MeshColliderBatch"))
      return ((RaycastHit) ref hit).get_rigidbody();
    MeshColliderBatch component = (MeshColliderBatch) ((Component) ((RaycastHit) ref hit).get_transform()).GetComponent<MeshColliderBatch>();
    if (!Object.op_Implicit((Object) component))
      return ((RaycastHit) ref hit).get_rigidbody();
    Rigidbody rigidbody = component.LookupRigidbody(((RaycastHit) ref hit).get_triangleIndex());
    if (!Object.op_Implicit((Object) rigidbody))
      return ((RaycastHit) ref hit).get_rigidbody();
    return rigidbody;
  }

  public static Collider GetCollider(this RaycastHit hit)
  {
    if (((RaycastHit) ref hit).get_triangleIndex() < 0)
      return ((RaycastHit) ref hit).get_collider();
    if (!((Component) ((RaycastHit) ref hit).get_transform()).CompareTag("MeshColliderBatch"))
      return ((RaycastHit) ref hit).get_collider();
    MeshColliderBatch component = (MeshColliderBatch) ((Component) ((RaycastHit) ref hit).get_transform()).GetComponent<MeshColliderBatch>();
    if (!Object.op_Implicit((Object) component))
      return ((RaycastHit) ref hit).get_collider();
    Collider collider = component.LookupCollider(((RaycastHit) ref hit).get_triangleIndex());
    if (!Object.op_Implicit((Object) collider))
      return ((RaycastHit) ref hit).get_collider();
    return collider;
  }

  public static BaseEntity GetEntity(this RaycastHit hit)
  {
    return ((Component) hit.GetTransform()).get_gameObject().ToBaseEntity();
  }
}
