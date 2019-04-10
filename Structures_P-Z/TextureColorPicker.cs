// Decompiled with JetBrains decompiler
// Type: TextureColorPicker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TextureColorPicker : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
{
  public Texture2D texture;
  public TextureColorPicker.onColorSelectedEvent onColorSelected;

  public virtual void OnPointerDown(PointerEventData eventData)
  {
    this.OnDrag(eventData);
  }

  public virtual void OnDrag(PointerEventData eventData)
  {
    RectTransform transform = ((Component) this).get_transform() as RectTransform;
    Vector2 vector2 = (Vector2) null;
    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform, eventData.get_position(), eventData.get_pressEventCamera(), ref vector2))
      return;
    ref __Null local1 = ref vector2.x;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    double num1 = (double) ^(float&) ref local1;
    Rect rect1 = transform.get_rect();
    double num2 = (double) ((Rect) ref rect1).get_width() * 0.5;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local1 = (float) (num1 + num2);
    ref __Null local2 = ref vector2.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    double num3 = (double) ^(float&) ref local2;
    Rect rect2 = transform.get_rect();
    double num4 = (double) ((Rect) ref rect2).get_height() * 0.5;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local2 = (float) (num3 + num4);
    ref __Null local3 = ref vector2.x;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    double num5 = (double) ^(float&) ref local3;
    Rect rect3 = transform.get_rect();
    double width = (double) ((Rect) ref rect3).get_width();
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local3 = (float) (num5 / width);
    ref __Null local4 = ref vector2.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    double num6 = (double) ^(float&) ref local4;
    Rect rect4 = transform.get_rect();
    double height = (double) ((Rect) ref rect4).get_height();
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local4 = (float) (num6 / height);
    this.onColorSelected.Invoke(this.texture.GetPixel((int) (vector2.x * (double) ((Texture) this.texture).get_width()), (int) (vector2.y * (double) ((Texture) this.texture).get_height())));
  }

  public TextureColorPicker()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class onColorSelectedEvent : UnityEvent<Color>
  {
    public onColorSelectedEvent()
    {
      base.\u002Ector();
    }
  }
}
