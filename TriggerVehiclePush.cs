// Decompiled with JetBrains decompiler
// Type: TriggerVehiclePush
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Linq;
using UnityEngine;

public class TriggerVehiclePush : TriggerBase, IServerComponent
{
  public float maxPushVelocity = 10f;
  public BaseEntity thisEntity;
  public float minRadius;
  public float maxRadius;

  internal override GameObject InterestedInObject(GameObject obj)
  {
    obj = base.InterestedInObject(obj);
    if (Object.op_Equality((Object) obj, (Object) null))
      return (GameObject) null;
    BaseEntity baseEntity = obj.ToBaseEntity();
    if (Object.op_Equality((Object) baseEntity, (Object) null))
      return (GameObject) null;
    if (baseEntity.isClient)
      return (GameObject) null;
    return ((Component) baseEntity).get_gameObject();
  }

  public void FixedUpdate()
  {
    if (Object.op_Equality((Object) this.thisEntity, (Object) null) || this.entityContents == null)
      return;
    foreach (BaseEntity ent in this.entityContents.ToArray<BaseEntity>())
    {
      if (ent.IsValid() && !ent.EqualNetID((BaseNetworkable) this.thisEntity))
      {
        Rigidbody component = (Rigidbody) ((Component) ent).GetComponent<Rigidbody>();
        if (Object.op_Implicit((Object) component) && !component.get_isKinematic())
        {
          float num1 = Vector3Ex.Distance2D(((Component) ent).get_transform().get_position(), ((Component) this).get_transform().get_position());
          float num2 = 1f - Mathf.InverseLerp(this.minRadius, this.maxRadius, num1);
          float num3 = 1f - Mathf.InverseLerp(this.minRadius - 1f, this.minRadius, num1);
          Vector3 vector3_1 = ent.ClosestPoint(((Component) this).get_transform().get_position());
          Vector3 vector3_2 = Vector3Ex.Direction2D(vector3_1, ((Component) this).get_transform().get_position());
          component.AddForceAtPosition(Vector3.op_Multiply(Vector3.op_Multiply(vector3_2, this.maxPushVelocity), num2), vector3_1, (ForceMode) 5);
          if ((double) num3 > 0.0)
            component.AddForceAtPosition(Vector3.op_Multiply(Vector3.op_Multiply(vector3_2, 1f), num3), vector3_1, (ForceMode) 2);
        }
      }
    }
  }

  public void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_red());
    Gizmos.DrawWireSphere(((Component) this).get_transform().get_position(), this.minRadius);
    Gizmos.set_color(new Color(0.5f, 0.0f, 0.0f, 1f));
    Gizmos.DrawWireSphere(((Component) this).get_transform().get_position(), this.maxRadius);
  }
}
