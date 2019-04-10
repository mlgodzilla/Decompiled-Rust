// Decompiled with JetBrains decompiler
// Type: SocketMod_AreaCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SocketMod_AreaCheck : SocketMod
{
  public Bounds bounds = new Bounds(Vector3.get_zero(), Vector3.op_Multiply(Vector3.get_one(), 0.1f));
  public bool wantsInside = true;
  public LayerMask layerMask;

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    bool flag = true;
    if (!this.wantsInside)
      flag = !flag;
    Gizmos.set_color(flag ? Color.get_green() : Color.get_red());
    Gizmos.DrawCube(((Bounds) ref this.bounds).get_center(), ((Bounds) ref this.bounds).get_size());
  }

  public static bool IsInArea(
    Vector3 position,
    Quaternion rotation,
    Bounds bounds,
    LayerMask layerMask)
  {
    return GamePhysics.CheckOBB(new OBB(position, rotation, bounds), ((LayerMask) ref layerMask).get_value(), (QueryTriggerInteraction) 0);
  }

  public bool DoCheck(Vector3 position, Quaternion rotation)
  {
    return SocketMod_AreaCheck.IsInArea(Vector3.op_Addition(position, Quaternion.op_Multiply(rotation, this.worldPosition)), Quaternion.op_Multiply(rotation, this.worldRotation), this.bounds, this.layerMask) == this.wantsInside;
  }

  public override bool DoCheck(Construction.Placement place)
  {
    if (this.DoCheck(place.position, place.rotation))
      return true;
    Construction.lastPlacementError = "Failed Check: IsInArea (" + this.hierachyName + ")";
    return false;
  }
}
