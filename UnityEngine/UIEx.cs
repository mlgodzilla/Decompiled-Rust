// Decompiled with JetBrains decompiler
// Type: UnityEngine.UIEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine.UI;

namespace UnityEngine
{
  public static class UIEx
  {
    public static Vector2 Unpivot(this RectTransform rect, Vector2 localPos)
    {
      ref __Null local1 = ref localPos.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      double num1 = (double) ^(float&) ref local1;
      // ISSUE: variable of the null type
      __Null x = rect.get_pivot().x;
      Rect rect1 = rect.get_rect();
      double width = (double) ((Rect) ref rect1).get_width();
      double num2 = x * width;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = (float) (num1 + num2);
      ref __Null local2 = ref localPos.y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      double num3 = (double) ^(float&) ref local2;
      // ISSUE: variable of the null type
      __Null y = rect.get_pivot().y;
      Rect rect2 = rect.get_rect();
      double height = (double) ((Rect) ref rect2).get_height();
      double num4 = y * height;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = (float) (num3 + num4);
      return localPos;
    }

    public static void CenterOnPosition(this ScrollRect scrollrect, Vector2 pos)
    {
      RectTransform transform = ((Component) scrollrect).get_transform() as RectTransform;
      Vector2 vector2_1;
      ((Vector2) ref vector2_1).\u002Ector((float) ((Transform) scrollrect.get_content()).get_localScale().x, (float) ((Transform) scrollrect.get_content()).get_localScale().y);
      ref __Null local1 = ref pos.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 * (float) vector2_1.x;
      ref __Null local2 = ref pos.y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 * (float) vector2_1.y;
      Vector2 vector2_2;
      ref Vector2 local3 = ref vector2_2;
      Rect rect = scrollrect.get_content().get_rect();
      double num1 = (double) ((Rect) ref rect).get_width() * vector2_1.x;
      rect = transform.get_rect();
      double width = (double) ((Rect) ref rect).get_width();
      double num2 = num1 - width;
      rect = scrollrect.get_content().get_rect();
      double num3 = (double) ((Rect) ref rect).get_height() * vector2_1.y;
      rect = transform.get_rect();
      double height = (double) ((Rect) ref rect).get_height();
      double num4 = num3 - height;
      ((Vector2) ref local3).\u002Ector((float) num2, (float) num4);
      pos.x = pos.x / vector2_2.x + scrollrect.get_content().get_pivot().x;
      pos.y = pos.y / vector2_2.y + scrollrect.get_content().get_pivot().y;
      if (scrollrect.get_movementType() != null)
      {
        pos.x = (__Null) (double) Mathf.Clamp((float) pos.x, 0.0f, 1f);
        pos.y = (__Null) (double) Mathf.Clamp((float) pos.y, 0.0f, 1f);
      }
      scrollrect.set_normalizedPosition(pos);
    }
  }
}
