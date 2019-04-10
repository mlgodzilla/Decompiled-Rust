// Decompiled with JetBrains decompiler
// Type: SocketMod_InWater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SocketMod_InWater : SocketMod
{
  public bool wantsInWater = true;

  private void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(Color.get_cyan());
    Gizmos.DrawSphere(Vector3.get_zero(), 0.1f);
  }

  public override bool DoCheck(Construction.Placement place)
  {
    if (WaterLevel.Test(Vector3.op_Addition(place.position, Quaternion.op_Multiply(place.rotation, this.worldPosition))) == this.wantsInWater)
      return true;
    Construction.lastPlacementError = "Failed Check: InWater (" + this.hierachyName + ")";
    return false;
  }
}
