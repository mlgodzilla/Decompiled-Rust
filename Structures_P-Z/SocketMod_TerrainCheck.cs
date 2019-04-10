// Decompiled with JetBrains decompiler
// Type: SocketMod_TerrainCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SocketMod_TerrainCheck : SocketMod
{
  public bool wantsInTerrain = true;

  private void OnDrawGizmos()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    bool flag = SocketMod_TerrainCheck.IsInTerrain(((Component) this).get_transform().get_position());
    if (!this.wantsInTerrain)
      flag = !flag;
    Gizmos.set_color(flag ? Color.get_green() : Color.get_red());
    Gizmos.DrawSphere(Vector3.get_zero(), 0.1f);
  }

  public static bool IsInTerrain(Vector3 vPoint)
  {
    if (TerrainMeta.OutOfBounds(vPoint))
      return false;
    if (!Object.op_Implicit((Object) TerrainMeta.Collision) || !TerrainMeta.Collision.GetIgnore(vPoint, 0.01f))
    {
      foreach (Terrain activeTerrain in Terrain.get_activeTerrains())
      {
        if ((double) activeTerrain.SampleHeight(vPoint) + ((Component) activeTerrain).get_transform().get_position().y > vPoint.y)
          return true;
      }
    }
    return Physics.Raycast(new Ray(Vector3.op_Addition(vPoint, Vector3.op_Multiply(Vector3.get_up(), 3f)), Vector3.get_down()), 3f, 65536);
  }

  public override bool DoCheck(Construction.Placement place)
  {
    if (SocketMod_TerrainCheck.IsInTerrain(Vector3.op_Addition(place.position, Quaternion.op_Multiply(place.rotation, this.worldPosition))) == this.wantsInTerrain)
      return true;
    Construction.lastPlacementError = this.fullName + ": not in terrain";
    return false;
  }
}
