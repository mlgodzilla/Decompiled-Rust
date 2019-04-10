// Decompiled with JetBrains decompiler
// Type: NeighbourSocket
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class NeighbourSocket : Socket_Base
{
  private void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
  }

  public override bool TestTarget(Construction.Target target)
  {
    return false;
  }

  public override bool CanConnect(
    Vector3 position,
    Quaternion rotation,
    Socket_Base socket,
    Vector3 socketPosition,
    Quaternion socketRotation)
  {
    if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
      return false;
    OBB selectBounds1 = this.GetSelectBounds(position, rotation);
    OBB selectBounds2 = socket.GetSelectBounds(socketPosition, socketRotation);
    return ((OBB) ref selectBounds1).Intersects(selectBounds2);
  }
}
