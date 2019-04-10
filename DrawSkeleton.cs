// Decompiled with JetBrains decompiler
// Type: DrawSkeleton
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DrawSkeleton : MonoBehaviour
{
  private void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_white());
    DrawSkeleton.DrawTransform(((Component) this).get_transform());
  }

  private static void DrawTransform(Transform t)
  {
    for (int index = 0; index < t.get_childCount(); ++index)
    {
      Gizmos.DrawLine(t.get_position(), t.GetChild(index).get_position());
      DrawSkeleton.DrawTransform(t.GetChild(index));
    }
  }

  public DrawSkeleton()
  {
    base.\u002Ector();
  }
}
