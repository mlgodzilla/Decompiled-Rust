// Decompiled with JetBrains decompiler
// Type: SocketMod_Attraction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class SocketMod_Attraction : SocketMod
{
  public float outerRadius = 1f;
  public float innerRadius = 0.1f;
  public string groupName = "wallbottom";

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(new Color(1f, 1f, 0.0f, 0.3f));
    Gizmos.DrawSphere(Vector3.get_zero(), this.outerRadius);
    Gizmos.set_color(new Color(0.0f, 1f, 0.0f, 0.6f));
    Gizmos.DrawSphere(Vector3.get_zero(), this.innerRadius);
  }

  public override bool DoCheck(Construction.Placement place)
  {
    return true;
  }

  public override void ModifyPlacement(Construction.Placement place)
  {
    Vector3 position = Vector3.op_Addition(place.position, Quaternion.op_Multiply(place.rotation, this.worldPosition));
    List<BaseEntity> list = (List<BaseEntity>) Pool.GetList<BaseEntity>();
    Vis.Entities<BaseEntity>(position, this.outerRadius * 2f, list, -1, (QueryTriggerInteraction) 2);
    foreach (BaseEntity baseEntity in list)
    {
      if (baseEntity.isServer == this.isServer)
      {
        AttractionPoint[] all = this.prefabAttribute.FindAll<AttractionPoint>(baseEntity.prefabID);
        if (all != null)
        {
          foreach (AttractionPoint attractionPoint in all)
          {
            if (!(attractionPoint.groupName != this.groupName))
            {
              Vector3 vector3_1 = Vector3.op_Addition(((Component) baseEntity).get_transform().get_position(), Quaternion.op_Multiply(((Component) baseEntity).get_transform().get_rotation(), attractionPoint.worldPosition));
              Vector3 vector3_2 = Vector3.op_Subtraction(vector3_1, position);
              float magnitude = ((Vector3) ref vector3_2).get_magnitude();
              if ((double) magnitude <= (double) this.outerRadius)
              {
                Quaternion quaternion = QuaternionEx.LookRotationWithOffset(this.worldPosition, Vector3.op_Subtraction(vector3_1, place.position), Vector3.get_up());
                float num = Mathf.InverseLerp(this.outerRadius, this.innerRadius, magnitude);
                place.rotation = Quaternion.Lerp(place.rotation, quaternion, num);
                position = Vector3.op_Addition(place.position, Quaternion.op_Multiply(place.rotation, this.worldPosition));
                Vector3 vector3_3 = Vector3.op_Subtraction(vector3_1, position);
                place.position = Vector3.op_Addition(place.position, Vector3.op_Multiply(vector3_3, num));
              }
            }
          }
        }
      }
    }
    // ISSUE: cast to a reference type
    Pool.FreeList<BaseEntity>((List<M0>&) ref list);
  }
}
