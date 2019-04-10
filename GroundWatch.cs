// Decompiled with JetBrains decompiler
// Type: GroundWatch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class GroundWatch : MonoBehaviour, IServerComponent
{
  public Vector3 groundPosition;
  public LayerMask layers;
  public float radius;

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(Color.get_green());
    Gizmos.DrawSphere(this.groundPosition, this.radius);
  }

  public static void PhysicsChanged(GameObject obj)
  {
    Collider component = (Collider) obj.GetComponent<Collider>();
    if (!Object.op_Implicit((Object) component))
      return;
    Bounds bounds = component.get_bounds();
    List<BaseEntity> list1 = (List<BaseEntity>) Pool.GetList<BaseEntity>();
    Vector3 center = ((Bounds) ref bounds).get_center();
    Vector3 extents = ((Bounds) ref bounds).get_extents();
    double num = (double) ((Vector3) ref extents).get_magnitude() + 1.0;
    List<BaseEntity> list2 = list1;
    Vis.Entities<BaseEntity>(center, (float) num, list2, 2228480, (QueryTriggerInteraction) 2);
    foreach (BaseEntity baseEntity in list1)
    {
      if (!baseEntity.IsDestroyed && !baseEntity.isClient && !(baseEntity is BuildingBlock))
        ((Component) baseEntity).BroadcastMessage("OnPhysicsNeighbourChanged", (SendMessageOptions) 1);
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BaseEntity>((List<M0>&) ref list1);
  }

  private void OnPhysicsNeighbourChanged()
  {
    if (this.OnGround())
      return;
    ((Component) ((Component) this).get_transform().get_root()).BroadcastMessage("OnGroundMissing", (SendMessageOptions) 1);
  }

  private bool OnGround()
  {
    BaseEntity component = (BaseEntity) ((Component) this).GetComponent<BaseEntity>();
    if (Object.op_Implicit((Object) component))
    {
      Construction construction = PrefabAttribute.server.Find<Construction>(component.prefabID);
      if ((bool) ((PrefabAttribute) construction))
      {
        foreach (Socket_Base allSocket in construction.allSockets)
        {
          foreach (SocketMod socketMod in allSocket.socketMods)
          {
            SocketMod_AreaCheck socketModAreaCheck = socketMod as SocketMod_AreaCheck;
            if ((bool) ((PrefabAttribute) socketModAreaCheck) && socketModAreaCheck.wantsInside && !socketModAreaCheck.DoCheck(((Component) component).get_transform().get_position(), ((Component) component).get_transform().get_rotation()))
              return false;
          }
        }
      }
    }
    List<Collider> list = (List<Collider>) Pool.GetList<Collider>();
    Vis.Colliders<Collider>(((Component) this).get_transform().TransformPoint(this.groundPosition), this.radius, list, LayerMask.op_Implicit(this.layers), (QueryTriggerInteraction) 2);
    using (List<Collider>.Enumerator enumerator = list.GetEnumerator())
    {
      while (enumerator.MoveNext())
      {
        BaseEntity baseEntity = ((Component) enumerator.Current).get_gameObject().ToBaseEntity();
        if (!Object.op_Implicit((Object) baseEntity) || !Object.op_Equality((Object) baseEntity, (Object) component) && !baseEntity.IsDestroyed && !baseEntity.isClient)
        {
          // ISSUE: cast to a reference type
          Pool.FreeList<Collider>((List<M0>&) ref list);
          return true;
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<Collider>((List<M0>&) ref list);
    return false;
  }

  public GroundWatch()
  {
    base.\u002Ector();
  }
}
