// Decompiled with JetBrains decompiler
// Type: SocketMod_SphereCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SocketMod_SphereCheck : SocketMod
{
  public float sphereRadius = 1f;
  public LayerMask layerMask;
  public bool wantsCollide;

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(this.wantsCollide ? new Color(0.0f, 1f, 0.0f, 0.7f) : new Color(1f, 0.0f, 0.0f, 0.7f));
    Gizmos.DrawSphere(Vector3.get_zero(), this.sphereRadius);
  }

  public override bool DoCheck(Construction.Placement place)
  {
    if (this.wantsCollide == GamePhysics.CheckSphere(Vector3.op_Addition(place.position, Quaternion.op_Multiply(place.rotation, this.worldPosition)), this.sphereRadius, ((LayerMask) ref this.layerMask).get_value(), (QueryTriggerInteraction) 0))
      return true;
    Construction.lastPlacementError = "Failed Check: Sphere Test (" + this.hierachyName + ")";
    return false;
  }
}
