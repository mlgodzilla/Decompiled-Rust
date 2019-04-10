// Decompiled with JetBrains decompiler
// Type: ScrollRectZoom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectZoom : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
  public ScrollRectEx scrollRect;
  public float zoom;
  public bool smooth;
  public float max;
  public float min;
  public float velocity;

  public RectTransform rectTransform
  {
    get
    {
      return ((Component) this.scrollRect).get_transform() as RectTransform;
    }
  }

  private void OnEnable()
  {
    this.SetZoom(this.zoom);
  }

  public void OnScroll(PointerEventData data)
  {
    this.velocity += (float) (data.get_scrollDelta().y * (1.0 / 1000.0));
    this.velocity = Mathf.Clamp(this.velocity, -1f, 1f);
  }

  private void Update()
  {
    this.velocity = Mathf.Lerp(this.velocity, 0.0f, Time.get_deltaTime() * 10f);
    this.SetZoom(this.zoom + this.velocity);
  }

  private void SetZoom(float z)
  {
    z = Mathf.Clamp(z, this.min, this.max);
    if ((double) z == (double) this.zoom)
      return;
    this.zoom = z;
    Rect rect1 = (((Component) this.scrollRect).get_transform() as RectTransform).get_rect();
    Rect rect2 = this.scrollRect.content.get_rect();
    Vector2 vector2 = Vector2.op_Multiply(((Rect) ref rect2).get_size(), this.zoom);
    Vector2 normalizedPosition = this.scrollRect.normalizedPosition;
    if (vector2.x < (double) ((Rect) ref rect1).get_width())
    {
      double width = (double) ((Rect) ref rect1).get_width();
      rect2 = this.scrollRect.content.get_rect();
      // ISSUE: variable of the null type
      __Null x = ((Rect) ref rect2).get_size().x;
      this.zoom = (float) (width / x);
    }
    if (vector2.y < (double) ((Rect) ref rect1).get_height())
    {
      double height = (double) ((Rect) ref rect1).get_height();
      rect2 = this.scrollRect.content.get_rect();
      // ISSUE: variable of the null type
      __Null y = ((Rect) ref rect2).get_size().y;
      this.zoom = (float) (height / y);
    }
    ((Transform) this.scrollRect.content).set_localScale(Vector3.op_Multiply(Vector3.get_one(), this.zoom));
    this.scrollRect.normalizedPosition = normalizedPosition;
  }

  public ScrollRectZoom()
  {
    base.\u002Ector();
  }
}
