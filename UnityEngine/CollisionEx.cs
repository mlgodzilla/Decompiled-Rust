// Decompiled with JetBrains decompiler
// Type: UnityEngine.CollisionEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

namespace UnityEngine
{
  public static class CollisionEx
  {
    public static BaseEntity GetEntity(this Collision col)
    {
      if (((Component) col.get_transform()).CompareTag("MeshColliderBatch") && Object.op_Implicit((Object) col.get_gameObject().GetComponent<MeshColliderBatch>()))
      {
        for (int index = 0; index < col.get_contacts().Length; ++index)
        {
          ContactPoint contact = col.get_contacts()[index];
          Ray ray;
          ((Ray) ref ray).\u002Ector(Vector3.op_Addition(((ContactPoint) ref contact).get_point(), Vector3.op_Multiply(((ContactPoint) ref contact).get_normal(), 0.01f)), Vector3.op_UnaryNegation(((ContactPoint) ref contact).get_normal()));
          RaycastHit hit;
          if (col.get_collider().Raycast(ray, ref hit, 1f))
            return hit.GetEntity();
        }
      }
      return col.get_gameObject().ToBaseEntity();
    }
  }
}
