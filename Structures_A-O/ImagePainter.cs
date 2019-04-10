// Decompiled with JetBrains decompiler
// Type: ImagePainter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Painting;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ImagePainter : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IInitializePotentialDragHandler
{
  public ImagePainter.OnDrawingEvent onDrawing;
  public MonoBehaviour redirectRightClick;
  [Tooltip("Spacing scale will depend on your texel size, tweak to what's right.")]
  public float spacingScale;
  internal Brush brush;
  internal ImagePainter.PointerState[] pointerState;

  public RectTransform rectTransform
  {
    get
    {
      return ((Component) this).get_transform() as RectTransform;
    }
  }

  public virtual void OnPointerDown(PointerEventData eventData)
  {
    if (eventData.get_button() == 1)
      return;
    Vector2 position;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.get_position(), eventData.get_pressEventCamera(), ref position);
    this.DrawAt(position, eventData.get_button());
    this.pointerState[eventData.get_button()].isDown = true;
  }

  public virtual void OnPointerUp(PointerEventData eventData)
  {
    this.pointerState[eventData.get_button()].isDown = false;
  }

  public virtual void OnDrag(PointerEventData eventData)
  {
    if (eventData.get_button() == 1)
    {
      if (!Object.op_Implicit((Object) this.redirectRightClick))
        return;
      ((Component) this.redirectRightClick).SendMessage(nameof (OnDrag), (object) eventData);
    }
    else
    {
      Vector2 position;
      RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.get_position(), eventData.get_pressEventCamera(), ref position);
      this.DrawAt(position, eventData.get_button());
    }
  }

  public virtual void OnBeginDrag(PointerEventData eventData)
  {
    if (eventData.get_button() != 1 || !Object.op_Implicit((Object) this.redirectRightClick))
      return;
    ((Component) this.redirectRightClick).SendMessage(nameof (OnBeginDrag), (object) eventData);
  }

  public virtual void OnEndDrag(PointerEventData eventData)
  {
    if (eventData.get_button() != 1 || !Object.op_Implicit((Object) this.redirectRightClick))
      return;
    ((Component) this.redirectRightClick).SendMessage(nameof (OnEndDrag), (object) eventData);
  }

  public virtual void OnInitializePotentialDrag(PointerEventData eventData)
  {
    if (eventData.get_button() != 1 || !Object.op_Implicit((Object) this.redirectRightClick))
      return;
    ((Component) this.redirectRightClick).SendMessage(nameof (OnInitializePotentialDrag), (object) eventData);
  }

  private void DrawAt(Vector2 position, PointerEventData.InputButton button)
  {
    if (this.brush == null)
      return;
    ImagePainter.PointerState pointerState = this.pointerState[button];
    Vector2 vector2_1 = this.rectTransform.Unpivot(position);
    if (pointerState.isDown)
    {
      Vector2 vector2_2 = Vector2.op_Subtraction(pointerState.lastPos, vector2_1);
      Vector2 normalized = ((Vector2) ref vector2_2).get_normalized();
      for (float num = 0.0f; (double) num < (double) ((Vector2) ref vector2_2).get_magnitude(); num += Mathf.Max(this.brush.spacing, 1f) * Mathf.Max(this.spacingScale, 0.1f))
        this.onDrawing.Invoke(Vector2.op_Addition(vector2_1, Vector2.op_Multiply(num, normalized)), this.brush);
      pointerState.lastPos = vector2_1;
    }
    else
    {
      this.onDrawing.Invoke(vector2_1, this.brush);
      pointerState.lastPos = vector2_1;
    }
  }

  private void Start()
  {
  }

  public void UpdateBrush(Brush brush)
  {
    this.brush = brush;
  }

  public ImagePainter()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class OnDrawingEvent : UnityEvent<Vector2, Brush>
  {
    public OnDrawingEvent()
    {
      base.\u002Ector();
    }
  }

  internal class PointerState
  {
    public Vector2 lastPos;
    public bool isDown;
  }
}
