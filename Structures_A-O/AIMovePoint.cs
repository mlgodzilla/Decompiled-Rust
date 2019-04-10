// Decompiled with JetBrains decompiler
// Type: AIMovePoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class AIMovePoint : MonoBehaviour
{
  public float radius;
  public float nextAvailableRoamTime;
  public float nextAvailableEngagementTime;
  public BaseEntity lastUser;

  public void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_green());
    GizmosUtil.DrawWireCircleY(((Component) this).get_transform().get_position(), this.radius);
  }

  public bool CanBeUsedBy(BaseEntity user)
  {
    if (Object.op_Inequality((Object) user, (Object) null) && Object.op_Equality((Object) this.lastUser, (Object) user))
      return true;
    return this.IsUsed();
  }

  public bool IsUsed()
  {
    if (!this.IsUsedForRoaming())
      return this.IsUsedForEngagement();
    return true;
  }

  public void MarkUsedForRoam(float dur = 10f, BaseEntity user = null)
  {
    this.nextAvailableRoamTime = Time.get_time() + dur;
    this.lastUser = user;
  }

  public void MarkUsedForEngagement(float dur = 5f, BaseEntity user = null)
  {
    this.nextAvailableEngagementTime = Time.get_time() + dur;
    this.lastUser = user;
  }

  public bool IsUsedForRoaming()
  {
    return (double) Time.get_time() < (double) this.nextAvailableRoamTime;
  }

  public bool IsUsedForEngagement()
  {
    return (double) Time.get_time() < (double) this.nextAvailableEngagementTime;
  }

  public AIMovePoint()
  {
    base.\u002Ector();
  }
}
