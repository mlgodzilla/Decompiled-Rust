// Decompiled with JetBrains decompiler
// Type: PathInterestNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PathInterestNode : MonoBehaviour
{
  public float NextVisitTime { get; set; }

  public void OnDrawGizmos()
  {
    Gizmos.set_color(new Color(0.0f, 1f, 1f, 0.5f));
    Gizmos.DrawSphere(((Component) this).get_transform().get_position(), 0.5f);
  }

  public PathInterestNode()
  {
    base.\u002Ector();
  }
}
