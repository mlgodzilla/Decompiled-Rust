// Decompiled with JetBrains decompiler
// Type: PathSpeedZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PathSpeedZone : MonoBehaviour
{
  public Bounds bounds;
  public OBB obbBounds;
  public float maxVelocityPerSec;

  public OBB WorldSpaceBounds()
  {
    return new OBB(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_lossyScale(), ((Component) this).get_transform().get_rotation(), this.bounds);
  }

  public float GetMaxSpeed()
  {
    return this.maxVelocityPerSec;
  }

  public virtual void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(new Color(1f, 0.5f, 0.0f, 0.5f));
    Gizmos.DrawCube(((Bounds) ref this.bounds).get_center(), ((Bounds) ref this.bounds).get_size());
    Gizmos.set_color(new Color(1f, 0.7f, 0.0f, 1f));
    Gizmos.DrawWireCube(((Bounds) ref this.bounds).get_center(), ((Bounds) ref this.bounds).get_size());
  }

  public PathSpeedZone()
  {
    base.\u002Ector();
  }
}
