// Decompiled with JetBrains decompiler
// Type: SocketMod_AngleCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SocketMod_AngleCheck : SocketMod
{
  public bool wantsAngle = true;
  public Vector3 worldNormal = Vector3.get_up();
  public float withinDegrees = 45f;

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(Color.get_yellow());
    Gizmos.DrawFrustum(Vector3.get_zero(), this.withinDegrees, 1f, 0.0f, 1f);
  }

  public override bool DoCheck(Construction.Placement place)
  {
    if ((double) Vector3Ex.DotDegrees(this.worldNormal, Quaternion.op_Multiply(place.rotation, Vector3.get_up())) < (double) this.withinDegrees)
      return true;
    Construction.lastPlacementError = "Failed Check: AngleCheck (" + this.hierachyName + ")";
    return false;
  }
}
