// Decompiled with JetBrains decompiler
// Type: CH47LandingZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class CH47LandingZone : MonoBehaviour
{
  private static List<CH47LandingZone> landingZones = new List<CH47LandingZone>();
  public float lastDropTime;
  public float dropoffScale;

  public void Awake()
  {
    if (CH47LandingZone.landingZones.Contains(this))
      return;
    CH47LandingZone.landingZones.Add(this);
  }

  public static CH47LandingZone GetClosest(Vector3 pos)
  {
    float num1 = float.PositiveInfinity;
    CH47LandingZone ch47LandingZone = (CH47LandingZone) null;
    foreach (CH47LandingZone landingZone in CH47LandingZone.landingZones)
    {
      float num2 = Vector3Ex.Distance2D(pos, ((Component) landingZone).get_transform().get_position());
      if ((double) num2 < (double) num1)
      {
        num1 = num2;
        ch47LandingZone = landingZone;
      }
    }
    return ch47LandingZone;
  }

  public void OnDestroy()
  {
    if (!CH47LandingZone.landingZones.Contains(this))
      return;
    CH47LandingZone.landingZones.Remove(this);
  }

  public float TimeSinceLastDrop()
  {
    return Time.get_time() - this.lastDropTime;
  }

  public void Used()
  {
    this.lastDropTime = Time.get_time();
  }

  public void OnDrawGizmos()
  {
    Color magenta = Color.get_magenta();
    magenta.a = (__Null) 0.25;
    Gizmos.set_color(magenta);
    GizmosUtil.DrawCircleY(((Component) this).get_transform().get_position(), 6f);
    magenta.a = (__Null) 1.0;
    Gizmos.set_color(magenta);
    GizmosUtil.DrawWireCircleY(((Component) this).get_transform().get_position(), 6f);
  }

  public CH47LandingZone()
  {
    base.\u002Ector();
  }
}
