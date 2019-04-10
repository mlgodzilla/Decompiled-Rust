// Decompiled with JetBrains decompiler
// Type: HitNumber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class HitNumber : MonoBehaviour
{
  public HitNumber.HitType hitType;

  public int ColorToMultiplier(HitNumber.HitType type)
  {
    switch (type)
    {
      case HitNumber.HitType.Yellow:
        return 1;
      case HitNumber.HitType.Green:
        return 3;
      case HitNumber.HitType.Blue:
        return 5;
      case HitNumber.HitType.Purple:
        return 10;
      case HitNumber.HitType.Red:
        return 20;
      default:
        return 0;
    }
  }

  public void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_white());
    Gizmos.DrawSphere(((Component) this).get_transform().get_position(), 0.025f);
  }

  public HitNumber()
  {
    base.\u002Ector();
  }

  public enum HitType
  {
    Yellow,
    Green,
    Blue,
    Purple,
    Red,
  }
}
