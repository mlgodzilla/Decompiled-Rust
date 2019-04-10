// Decompiled with JetBrains decompiler
// Type: SocketMod_HotSpot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SocketMod_HotSpot : SocketMod
{
  public float spotSize = 0.1f;

  private void OnDrawGizmos()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(new Color(1f, 1f, 0.0f, 0.5f));
    Gizmos.DrawSphere(Vector3.get_zero(), this.spotSize);
  }

  public override void ModifyPlacement(Construction.Placement place)
  {
    Vector3 vector3 = Vector3.op_Addition(place.position, Quaternion.op_Multiply(place.rotation, this.worldPosition));
    place.position = vector3;
  }
}
