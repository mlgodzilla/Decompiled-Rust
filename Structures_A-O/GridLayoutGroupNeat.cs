// Decompiled with JetBrains decompiler
// Type: GridLayoutGroupNeat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class GridLayoutGroupNeat : GridLayoutGroup
{
  private float IdealCellWidth(float cellSize)
  {
    Rect rect = ((LayoutGroup) this).get_rectTransform().get_rect();
    double num = (double) ((Rect) ref rect).get_x() + (double) (((LayoutGroup) this).get_padding().get_left() + ((LayoutGroup) this).get_padding().get_right()) * 0.5;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    return (float) (num / (double) Mathf.Floor((float) num / cellSize) - (^(Vector2&) ref this.m_Spacing).x);
  }

  public virtual void SetLayoutHorizontal()
  {
    Vector2 cellSize = (Vector2) this.m_CellSize;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    (^(Vector2&) ref this.m_CellSize).x = (__Null) (double) this.IdealCellWidth((float) cellSize.x);
    base.SetLayoutHorizontal();
    this.m_CellSize = (__Null) cellSize;
  }

  public virtual void SetLayoutVertical()
  {
    Vector2 cellSize = (Vector2) this.m_CellSize;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    (^(Vector2&) ref this.m_CellSize).x = (__Null) (double) this.IdealCellWidth((float) cellSize.x);
    base.SetLayoutVertical();
    this.m_CellSize = (__Null) cellSize;
  }

  public GridLayoutGroupNeat()
  {
    base.\u002Ector();
  }
}
