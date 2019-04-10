// Decompiled with JetBrains decompiler
// Type: CanvasOrderHack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class CanvasOrderHack : MonoBehaviour
{
  private void OnEnable()
  {
    foreach (Canvas componentsInChild in (Canvas[]) ((Component) this).GetComponentsInChildren<Canvas>(true))
    {
      if (componentsInChild.get_overrideSorting())
      {
        Canvas canvas = componentsInChild;
        canvas.set_sortingOrder(canvas.get_sortingOrder() + 1);
      }
    }
    foreach (Canvas componentsInChild in (Canvas[]) ((Component) this).GetComponentsInChildren<Canvas>(true))
    {
      if (componentsInChild.get_overrideSorting())
      {
        Canvas canvas = componentsInChild;
        canvas.set_sortingOrder(canvas.get_sortingOrder() - 1);
      }
    }
  }

  public CanvasOrderHack()
  {
    base.\u002Ector();
  }
}
