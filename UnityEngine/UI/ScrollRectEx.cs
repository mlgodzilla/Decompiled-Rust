// Decompiled with JetBrains decompiler
// Type: UnityEngine.UI.ScrollRectEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (RectTransform))]
  [AddComponentMenu("UI/Scroll Rect Ex", 37)]
  [SelectionBase]
  public class ScrollRectEx : UIBehaviour, IInitializePotentialDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutGroup, ILayoutController
  {
    public PointerEventData.InputButton scrollButton;
    public PointerEventData.InputButton altScrollButton;
    [SerializeField]
    private RectTransform m_Content;
    [SerializeField]
    private bool m_Horizontal;
    [SerializeField]
    private bool m_Vertical;
    [SerializeField]
    private ScrollRectEx.MovementType m_MovementType;
    [SerializeField]
    private float m_Elasticity;
    [SerializeField]
    private bool m_Inertia;
    [SerializeField]
    private float m_DecelerationRate;
    [SerializeField]
    private float m_ScrollSensitivity;
    [SerializeField]
    private RectTransform m_Viewport;
    [SerializeField]
    private Scrollbar m_HorizontalScrollbar;
    [SerializeField]
    private Scrollbar m_VerticalScrollbar;
    [SerializeField]
    private ScrollRectEx.ScrollbarVisibility m_HorizontalScrollbarVisibility;
    [SerializeField]
    private ScrollRectEx.ScrollbarVisibility m_VerticalScrollbarVisibility;
    [SerializeField]
    private float m_HorizontalScrollbarSpacing;
    [SerializeField]
    private float m_VerticalScrollbarSpacing;
    [SerializeField]
    private ScrollRectEx.ScrollRectEvent m_OnValueChanged;
    private Vector2 m_PointerStartLocalCursor;
    private Vector2 m_ContentStartPosition;
    private RectTransform m_ViewRect;
    private Bounds m_ContentBounds;
    private Bounds m_ViewBounds;
    private Vector2 m_Velocity;
    private bool m_Dragging;
    private Vector2 m_PrevPosition;
    private Bounds m_PrevContentBounds;
    private Bounds m_PrevViewBounds;
    [NonSerialized]
    private bool m_HasRebuiltLayout;
    private bool m_HSliderExpand;
    private bool m_VSliderExpand;
    private float m_HSliderHeight;
    private float m_VSliderWidth;
    [NonSerialized]
    private RectTransform m_Rect;
    private RectTransform m_HorizontalScrollbarRect;
    private RectTransform m_VerticalScrollbarRect;
    private DrivenRectTransformTracker m_Tracker;
    private readonly Vector3[] m_Corners;

    public RectTransform content
    {
      get
      {
        return this.m_Content;
      }
      set
      {
        this.m_Content = value;
      }
    }

    public bool horizontal
    {
      get
      {
        return this.m_Horizontal;
      }
      set
      {
        this.m_Horizontal = value;
      }
    }

    public bool vertical
    {
      get
      {
        return this.m_Vertical;
      }
      set
      {
        this.m_Vertical = value;
      }
    }

    public ScrollRectEx.MovementType movementType
    {
      get
      {
        return this.m_MovementType;
      }
      set
      {
        this.m_MovementType = value;
      }
    }

    public float elasticity
    {
      get
      {
        return this.m_Elasticity;
      }
      set
      {
        this.m_Elasticity = value;
      }
    }

    public bool inertia
    {
      get
      {
        return this.m_Inertia;
      }
      set
      {
        this.m_Inertia = value;
      }
    }

    public float decelerationRate
    {
      get
      {
        return this.m_DecelerationRate;
      }
      set
      {
        this.m_DecelerationRate = value;
      }
    }

    public float scrollSensitivity
    {
      get
      {
        return this.m_ScrollSensitivity;
      }
      set
      {
        this.m_ScrollSensitivity = value;
      }
    }

    public RectTransform viewport
    {
      get
      {
        return this.m_Viewport;
      }
      set
      {
        this.m_Viewport = value;
        this.SetDirtyCaching();
      }
    }

    public Scrollbar horizontalScrollbar
    {
      get
      {
        return this.m_HorizontalScrollbar;
      }
      set
      {
        if (Object.op_Implicit((Object) this.m_HorizontalScrollbar))
          ((UnityEvent<float>) this.m_HorizontalScrollbar.get_onValueChanged()).RemoveListener(new UnityAction<float>((object) this, __methodptr(SetHorizontalNormalizedPosition)));
        this.m_HorizontalScrollbar = value;
        if (Object.op_Implicit((Object) this.m_HorizontalScrollbar))
          ((UnityEvent<float>) this.m_HorizontalScrollbar.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(SetHorizontalNormalizedPosition)));
        this.SetDirtyCaching();
      }
    }

    public Scrollbar verticalScrollbar
    {
      get
      {
        return this.m_VerticalScrollbar;
      }
      set
      {
        if (Object.op_Implicit((Object) this.m_VerticalScrollbar))
          ((UnityEvent<float>) this.m_VerticalScrollbar.get_onValueChanged()).RemoveListener(new UnityAction<float>((object) this, __methodptr(SetVerticalNormalizedPosition)));
        this.m_VerticalScrollbar = value;
        if (Object.op_Implicit((Object) this.m_VerticalScrollbar))
          ((UnityEvent<float>) this.m_VerticalScrollbar.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(SetVerticalNormalizedPosition)));
        this.SetDirtyCaching();
      }
    }

    public ScrollRectEx.ScrollbarVisibility horizontalScrollbarVisibility
    {
      get
      {
        return this.m_HorizontalScrollbarVisibility;
      }
      set
      {
        this.m_HorizontalScrollbarVisibility = value;
        this.SetDirtyCaching();
      }
    }

    public ScrollRectEx.ScrollbarVisibility verticalScrollbarVisibility
    {
      get
      {
        return this.m_VerticalScrollbarVisibility;
      }
      set
      {
        this.m_VerticalScrollbarVisibility = value;
        this.SetDirtyCaching();
      }
    }

    public float horizontalScrollbarSpacing
    {
      get
      {
        return this.m_HorizontalScrollbarSpacing;
      }
      set
      {
        this.m_HorizontalScrollbarSpacing = value;
        this.SetDirty();
      }
    }

    public float verticalScrollbarSpacing
    {
      get
      {
        return this.m_VerticalScrollbarSpacing;
      }
      set
      {
        this.m_VerticalScrollbarSpacing = value;
        this.SetDirty();
      }
    }

    public ScrollRectEx.ScrollRectEvent onValueChanged
    {
      get
      {
        return this.m_OnValueChanged;
      }
      set
      {
        this.m_OnValueChanged = value;
      }
    }

    protected RectTransform viewRect
    {
      get
      {
        if (Object.op_Equality((Object) this.m_ViewRect, (Object) null))
          this.m_ViewRect = this.m_Viewport;
        if (Object.op_Equality((Object) this.m_ViewRect, (Object) null))
          this.m_ViewRect = (RectTransform) ((Component) this).get_transform();
        return this.m_ViewRect;
      }
    }

    public Vector2 velocity
    {
      get
      {
        return this.m_Velocity;
      }
      set
      {
        this.m_Velocity = value;
      }
    }

    private RectTransform rectTransform
    {
      get
      {
        if (Object.op_Equality((Object) this.m_Rect, (Object) null))
          this.m_Rect = (RectTransform) ((Component) this).GetComponent<RectTransform>();
        return this.m_Rect;
      }
    }

    protected ScrollRectEx()
    {
      base.\u002Ector();
    }

    public virtual void Rebuild(CanvasUpdate executing)
    {
      if (executing == null)
        this.UpdateCachedData();
      if (executing != 2)
        return;
      this.UpdateBounds();
      this.UpdateScrollbars(Vector2.get_zero());
      this.UpdatePrevData();
      this.m_HasRebuiltLayout = true;
    }

    private void UpdateCachedData()
    {
      Transform transform = ((Component) this).get_transform();
      this.m_HorizontalScrollbarRect = Object.op_Equality((Object) this.m_HorizontalScrollbar, (Object) null) ? (RectTransform) null : ((Component) this.m_HorizontalScrollbar).get_transform() as RectTransform;
      this.m_VerticalScrollbarRect = Object.op_Equality((Object) this.m_VerticalScrollbar, (Object) null) ? (RectTransform) null : ((Component) this.m_VerticalScrollbar).get_transform() as RectTransform;
      int num1 = Object.op_Equality((Object) ((Transform) this.viewRect).get_parent(), (Object) transform) ? 1 : 0;
      bool flag1 = !Object.op_Implicit((Object) this.m_HorizontalScrollbarRect) || Object.op_Equality((Object) ((Transform) this.m_HorizontalScrollbarRect).get_parent(), (Object) transform);
      bool flag2 = !Object.op_Implicit((Object) this.m_VerticalScrollbarRect) || Object.op_Equality((Object) ((Transform) this.m_VerticalScrollbarRect).get_parent(), (Object) transform);
      int num2 = flag1 ? 1 : 0;
      bool flag3 = (num1 & num2 & (flag2 ? 1 : 0)) != 0;
      this.m_HSliderExpand = flag3 && Object.op_Implicit((Object) this.m_HorizontalScrollbarRect) && this.horizontalScrollbarVisibility == ScrollRectEx.ScrollbarVisibility.AutoHideAndExpandViewport;
      this.m_VSliderExpand = flag3 && Object.op_Implicit((Object) this.m_VerticalScrollbarRect) && this.verticalScrollbarVisibility == ScrollRectEx.ScrollbarVisibility.AutoHideAndExpandViewport;
      Rect rect;
      double num3;
      if (!Object.op_Equality((Object) this.m_HorizontalScrollbarRect, (Object) null))
      {
        rect = this.m_HorizontalScrollbarRect.get_rect();
        num3 = (double) ((Rect) ref rect).get_height();
      }
      else
        num3 = 0.0;
      this.m_HSliderHeight = (float) num3;
      double num4;
      if (!Object.op_Equality((Object) this.m_VerticalScrollbarRect, (Object) null))
      {
        rect = this.m_VerticalScrollbarRect.get_rect();
        num4 = (double) ((Rect) ref rect).get_width();
      }
      else
        num4 = 0.0;
      this.m_VSliderWidth = (float) num4;
    }

    protected virtual void OnEnable()
    {
      base.OnEnable();
      if (Object.op_Implicit((Object) this.m_HorizontalScrollbar))
      {
        // ISSUE: method pointer
        ((UnityEvent<float>) this.m_HorizontalScrollbar.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(SetHorizontalNormalizedPosition)));
      }
      if (Object.op_Implicit((Object) this.m_VerticalScrollbar))
      {
        // ISSUE: method pointer
        ((UnityEvent<float>) this.m_VerticalScrollbar.get_onValueChanged()).AddListener(new UnityAction<float>((object) this, __methodptr(SetVerticalNormalizedPosition)));
      }
      CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild((ICanvasElement) this);
    }

    protected virtual void OnDisable()
    {
      if (Application.isQuitting != null)
        return;
      CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild((ICanvasElement) this);
      if (Object.op_Implicit((Object) this.m_HorizontalScrollbar))
      {
        // ISSUE: method pointer
        ((UnityEvent<float>) this.m_HorizontalScrollbar.get_onValueChanged()).RemoveListener(new UnityAction<float>((object) this, __methodptr(SetHorizontalNormalizedPosition)));
      }
      if (Object.op_Implicit((Object) this.m_VerticalScrollbar))
      {
        // ISSUE: method pointer
        ((UnityEvent<float>) this.m_VerticalScrollbar.get_onValueChanged()).RemoveListener(new UnityAction<float>((object) this, __methodptr(SetVerticalNormalizedPosition)));
      }
      this.m_HasRebuiltLayout = false;
      ((DrivenRectTransformTracker) ref this.m_Tracker).Clear();
      LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
      base.OnDisable();
    }

    public virtual bool IsActive()
    {
      if (base.IsActive())
        return Object.op_Inequality((Object) this.m_Content, (Object) null);
      return false;
    }

    private void EnsureLayoutHasRebuilt()
    {
      if (this.m_HasRebuiltLayout || CanvasUpdateRegistry.IsRebuildingLayout())
        return;
      Canvas.ForceUpdateCanvases();
    }

    public virtual void StopMovement()
    {
      this.m_Velocity = Vector2.get_zero();
    }

    public virtual void OnScroll(PointerEventData data)
    {
      if (!base.IsActive())
        return;
      this.EnsureLayoutHasRebuilt();
      this.UpdateBounds();
      Vector2 scrollDelta = data.get_scrollDelta();
      ref __Null local = ref scrollDelta.y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local = ^(float&) ref local * -1f;
      if (this.vertical && !this.horizontal)
      {
        if ((double) Mathf.Abs((float) scrollDelta.x) > (double) Mathf.Abs((float) scrollDelta.y))
          scrollDelta.y = scrollDelta.x;
        scrollDelta.x = (__Null) 0.0;
      }
      if (this.horizontal && !this.vertical)
      {
        if ((double) Mathf.Abs((float) scrollDelta.y) > (double) Mathf.Abs((float) scrollDelta.x))
          scrollDelta.x = scrollDelta.y;
        scrollDelta.y = (__Null) 0.0;
      }
      Vector2 position = Vector2.op_Addition(this.m_Content.get_anchoredPosition(), Vector2.op_Multiply(scrollDelta, this.m_ScrollSensitivity));
      if (this.m_MovementType == ScrollRectEx.MovementType.Clamped)
        position = Vector2.op_Addition(position, this.CalculateOffset(Vector2.op_Subtraction(position, this.m_Content.get_anchoredPosition())));
      this.SetContentAnchoredPosition(position);
      this.UpdateBounds();
    }

    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {
      if (eventData.get_button() != this.scrollButton && eventData.get_button() != this.altScrollButton)
        return;
      this.m_Velocity = Vector2.get_zero();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
      if (eventData.get_button() != this.scrollButton && eventData.get_button() != this.altScrollButton || !base.IsActive())
        return;
      this.UpdateBounds();
      this.m_PointerStartLocalCursor = Vector2.get_zero();
      RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.get_position(), eventData.get_pressEventCamera(), ref this.m_PointerStartLocalCursor);
      this.m_ContentStartPosition = this.m_Content.get_anchoredPosition();
      this.m_Dragging = true;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
      if (eventData.get_button() != this.scrollButton && eventData.get_button() != this.altScrollButton)
        return;
      this.m_Dragging = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
      Vector2 vector2_1;
      if (eventData.get_button() != this.scrollButton && eventData.get_button() != this.altScrollButton || (!base.IsActive() || !RectTransformUtility.ScreenPointToLocalPointInRectangle(this.viewRect, eventData.get_position(), eventData.get_pressEventCamera(), ref vector2_1)))
        return;
      this.UpdateBounds();
      Vector2 vector2_2 = Vector2.op_Addition(this.m_ContentStartPosition, Vector2.op_Subtraction(vector2_1, this.m_PointerStartLocalCursor));
      Vector2 offset = this.CalculateOffset(Vector2.op_Subtraction(vector2_2, this.m_Content.get_anchoredPosition()));
      Vector2 position = Vector2.op_Addition(vector2_2, offset);
      if (this.m_MovementType == ScrollRectEx.MovementType.Elastic)
      {
        if (offset.x != 0.0)
          position.x = (__Null) (position.x - (double) ScrollRectEx.RubberDelta((float) offset.x, (float) ((Bounds) ref this.m_ViewBounds).get_size().x));
        if (offset.y != 0.0)
          position.y = (__Null) (position.y - (double) ScrollRectEx.RubberDelta((float) offset.y, (float) ((Bounds) ref this.m_ViewBounds).get_size().y));
      }
      this.SetContentAnchoredPosition(position);
    }

    protected virtual void SetContentAnchoredPosition(Vector2 position)
    {
      if (!this.m_Horizontal)
        position.x = this.m_Content.get_anchoredPosition().x;
      if (!this.m_Vertical)
        position.y = this.m_Content.get_anchoredPosition().y;
      if (!Vector2.op_Inequality(position, this.m_Content.get_anchoredPosition()))
        return;
      this.m_Content.set_anchoredPosition(position);
      this.UpdateBounds();
    }

    protected virtual void LateUpdate()
    {
      if (!Object.op_Implicit((Object) this.m_Content))
        return;
      this.EnsureLayoutHasRebuilt();
      this.UpdateScrollbarVisibility();
      this.UpdateBounds();
      float unscaledDeltaTime = Time.get_unscaledDeltaTime();
      Vector2 offset = this.CalculateOffset(Vector2.get_zero());
      if (!this.m_Dragging && (Vector2.op_Inequality(offset, Vector2.get_zero()) || Vector2.op_Inequality(this.m_Velocity, Vector2.get_zero())))
      {
        Vector2 position = this.m_Content.get_anchoredPosition();
        for (int index = 0; index < 2; ++index)
        {
          if (this.m_MovementType == ScrollRectEx.MovementType.Elastic && (double) ((Vector2) ref offset).get_Item(index) != 0.0)
          {
            float num1 = ((Vector2) ref this.m_Velocity).get_Item(index);
            ref Vector2 local1 = ref position;
            int num2 = index;
            Vector2 anchoredPosition1 = this.m_Content.get_anchoredPosition();
            double num3 = (double) ((Vector2) ref anchoredPosition1).get_Item(index);
            Vector2 anchoredPosition2 = this.m_Content.get_anchoredPosition();
            double num4 = (double) ((Vector2) ref anchoredPosition2).get_Item(index) + (double) ((Vector2) ref offset).get_Item(index);
            ref float local2 = ref num1;
            double elasticity = (double) this.m_Elasticity;
            double num5 = (double) unscaledDeltaTime;
            double num6 = (double) Mathf.SmoothDamp((float) num3, (float) num4, ref local2, (float) elasticity, float.PositiveInfinity, (float) num5);
            ((Vector2) ref local1).set_Item(num2, (float) num6);
            ((Vector2) ref this.m_Velocity).set_Item(index, num1);
          }
          else if (this.m_Inertia)
          {
            ref Vector2 local1 = ref this.m_Velocity;
            int num1 = index;
            ((Vector2) ref local1).set_Item(num1, ((Vector2) ref local1).get_Item(num1) * Mathf.Pow(this.m_DecelerationRate, unscaledDeltaTime));
            if ((double) Mathf.Abs(((Vector2) ref this.m_Velocity).get_Item(index)) < 1.0)
              ((Vector2) ref this.m_Velocity).set_Item(index, 0.0f);
            ref Vector2 local2 = ref position;
            int num2 = index;
            ((Vector2) ref local2).set_Item(num2, ((Vector2) ref local2).get_Item(num2) + ((Vector2) ref this.m_Velocity).get_Item(index) * unscaledDeltaTime);
          }
          else
            ((Vector2) ref this.m_Velocity).set_Item(index, 0.0f);
        }
        if (Vector2.op_Inequality(this.m_Velocity, Vector2.get_zero()))
        {
          if (this.m_MovementType == ScrollRectEx.MovementType.Clamped)
          {
            offset = this.CalculateOffset(Vector2.op_Subtraction(position, this.m_Content.get_anchoredPosition()));
            position = Vector2.op_Addition(position, offset);
          }
          this.SetContentAnchoredPosition(position);
        }
      }
      if (this.m_Dragging && this.m_Inertia)
        this.m_Velocity = Vector2.op_Implicit(Vector3.Lerp(Vector2.op_Implicit(this.m_Velocity), Vector2.op_Implicit(Vector2.op_Division(Vector2.op_Subtraction(this.m_Content.get_anchoredPosition(), this.m_PrevPosition), unscaledDeltaTime)), unscaledDeltaTime * 10f));
      if (!Bounds.op_Inequality(this.m_ViewBounds, this.m_PrevViewBounds) && !Bounds.op_Inequality(this.m_ContentBounds, this.m_PrevContentBounds) && !Vector2.op_Inequality(this.m_Content.get_anchoredPosition(), this.m_PrevPosition))
        return;
      this.UpdateScrollbars(offset);
      this.m_OnValueChanged.Invoke(this.normalizedPosition);
      this.UpdatePrevData();
    }

    private void UpdatePrevData()
    {
      this.m_PrevPosition = !Object.op_Equality((Object) this.m_Content, (Object) null) ? this.m_Content.get_anchoredPosition() : Vector2.get_zero();
      this.m_PrevViewBounds = this.m_ViewBounds;
      this.m_PrevContentBounds = this.m_ContentBounds;
    }

    private void UpdateScrollbars(Vector2 offset)
    {
      if (Object.op_Implicit((Object) this.m_HorizontalScrollbar))
      {
        if (((Bounds) ref this.m_ContentBounds).get_size().x > 0.0)
          this.m_HorizontalScrollbar.set_size(Mathf.Clamp01((float) ((((Bounds) ref this.m_ViewBounds).get_size().x - (double) Mathf.Abs((float) offset.x)) / ((Bounds) ref this.m_ContentBounds).get_size().x)));
        else
          this.m_HorizontalScrollbar.set_size(1f);
        this.m_HorizontalScrollbar.set_value(this.horizontalNormalizedPosition);
      }
      if (!Object.op_Implicit((Object) this.m_VerticalScrollbar))
        return;
      if (((Bounds) ref this.m_ContentBounds).get_size().y > 0.0)
        this.m_VerticalScrollbar.set_size(Mathf.Clamp01((float) ((((Bounds) ref this.m_ViewBounds).get_size().y - (double) Mathf.Abs((float) offset.y)) / ((Bounds) ref this.m_ContentBounds).get_size().y)));
      else
        this.m_VerticalScrollbar.set_size(1f);
      this.m_VerticalScrollbar.set_value(this.verticalNormalizedPosition);
    }

    public Vector2 normalizedPosition
    {
      get
      {
        return new Vector2(this.horizontalNormalizedPosition, this.verticalNormalizedPosition);
      }
      set
      {
        this.SetNormalizedPosition((float) value.x, 0);
        this.SetNormalizedPosition((float) value.y, 1);
      }
    }

    public float horizontalNormalizedPosition
    {
      get
      {
        this.UpdateBounds();
        if (((Bounds) ref this.m_ContentBounds).get_size().x <= ((Bounds) ref this.m_ViewBounds).get_size().x)
          return ((Bounds) ref this.m_ViewBounds).get_min().x > ((Bounds) ref this.m_ContentBounds).get_min().x ? 1f : 0.0f;
        return (float) ((((Bounds) ref this.m_ViewBounds).get_min().x - ((Bounds) ref this.m_ContentBounds).get_min().x) / (((Bounds) ref this.m_ContentBounds).get_size().x - ((Bounds) ref this.m_ViewBounds).get_size().x));
      }
      set
      {
        this.SetNormalizedPosition(value, 0);
      }
    }

    public float verticalNormalizedPosition
    {
      get
      {
        this.UpdateBounds();
        if (((Bounds) ref this.m_ContentBounds).get_size().y <= ((Bounds) ref this.m_ViewBounds).get_size().y)
          return ((Bounds) ref this.m_ViewBounds).get_min().y > ((Bounds) ref this.m_ContentBounds).get_min().y ? 1f : 0.0f;
        return (float) ((((Bounds) ref this.m_ViewBounds).get_min().y - ((Bounds) ref this.m_ContentBounds).get_min().y) / (((Bounds) ref this.m_ContentBounds).get_size().y - ((Bounds) ref this.m_ViewBounds).get_size().y));
      }
      set
      {
        this.SetNormalizedPosition(value, 1);
      }
    }

    private void SetHorizontalNormalizedPosition(float value)
    {
      this.SetNormalizedPosition(value, 0);
    }

    private void SetVerticalNormalizedPosition(float value)
    {
      this.SetNormalizedPosition(value, 1);
    }

    private void SetNormalizedPosition(float value, int axis)
    {
      this.EnsureLayoutHasRebuilt();
      this.UpdateBounds();
      Vector3 size = ((Bounds) ref this.m_ContentBounds).get_size();
      double num1 = (double) ((Vector3) ref size).get_Item(axis);
      size = ((Bounds) ref this.m_ViewBounds).get_size();
      double num2 = (double) ((Vector3) ref size).get_Item(axis);
      float num3 = (float) (num1 - num2);
      Vector3 min = ((Bounds) ref this.m_ViewBounds).get_min();
      float num4 = ((Vector3) ref min).get_Item(axis) - value * num3;
      Vector3 vector3 = ((Transform) this.m_Content).get_localPosition();
      double num5 = (double) ((Vector3) ref vector3).get_Item(axis) + (double) num4;
      vector3 = ((Bounds) ref this.m_ContentBounds).get_min();
      double num6 = (double) ((Vector3) ref vector3).get_Item(axis);
      float num7 = (float) (num5 - num6);
      Vector3 localPosition = ((Transform) this.m_Content).get_localPosition();
      if ((double) Mathf.Abs(((Vector3) ref localPosition).get_Item(axis) - num7) <= 0.00999999977648258)
        return;
      ((Vector3) ref localPosition).set_Item(axis, num7);
      ((Transform) this.m_Content).set_localPosition(localPosition);
      ((Vector2) ref this.m_Velocity).set_Item(axis, 0.0f);
      this.UpdateBounds();
    }

    private static float RubberDelta(float overStretching, float viewSize)
    {
      return (float) (1.0 - 1.0 / ((double) Mathf.Abs(overStretching) * 0.550000011920929 / (double) viewSize + 1.0)) * viewSize * Mathf.Sign(overStretching);
    }

    protected virtual void OnRectTransformDimensionsChange()
    {
      this.SetDirty();
    }

    private bool hScrollingNeeded
    {
      get
      {
        if (Application.get_isPlaying())
          return ((Bounds) ref this.m_ContentBounds).get_size().x > ((Bounds) ref this.m_ViewBounds).get_size().x + 0.00999999977648258;
        return true;
      }
    }

    private bool vScrollingNeeded
    {
      get
      {
        if (Application.get_isPlaying())
          return ((Bounds) ref this.m_ContentBounds).get_size().y > ((Bounds) ref this.m_ViewBounds).get_size().y + 0.00999999977648258;
        return true;
      }
    }

    public virtual void SetLayoutHorizontal()
    {
      ((DrivenRectTransformTracker) ref this.m_Tracker).Clear();
      Rect rect;
      if (this.m_HSliderExpand || this.m_VSliderExpand)
      {
        ((DrivenRectTransformTracker) ref this.m_Tracker).Add((Object) this, this.viewRect, (DrivenTransformProperties) 16134);
        this.viewRect.set_anchorMin(Vector2.get_zero());
        this.viewRect.set_anchorMax(Vector2.get_one());
        this.viewRect.set_sizeDelta(Vector2.get_zero());
        this.viewRect.set_anchoredPosition(Vector2.get_zero());
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
        rect = this.viewRect.get_rect();
        Vector3 vector3_1 = Vector2.op_Implicit(((Rect) ref rect).get_center());
        rect = this.viewRect.get_rect();
        Vector3 vector3_2 = Vector2.op_Implicit(((Rect) ref rect).get_size());
        this.m_ViewBounds = new Bounds(vector3_1, vector3_2);
        this.m_ContentBounds = this.GetBounds();
      }
      if (this.m_VSliderExpand && this.vScrollingNeeded)
      {
        this.viewRect.set_sizeDelta(new Vector2((float) -((double) this.m_VSliderWidth + (double) this.m_VerticalScrollbarSpacing), (float) this.viewRect.get_sizeDelta().y));
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.content);
        rect = this.viewRect.get_rect();
        Vector3 vector3_1 = Vector2.op_Implicit(((Rect) ref rect).get_center());
        rect = this.viewRect.get_rect();
        Vector3 vector3_2 = Vector2.op_Implicit(((Rect) ref rect).get_size());
        this.m_ViewBounds = new Bounds(vector3_1, vector3_2);
        this.m_ContentBounds = this.GetBounds();
      }
      if (this.m_HSliderExpand && this.hScrollingNeeded)
      {
        this.viewRect.set_sizeDelta(new Vector2((float) this.viewRect.get_sizeDelta().x, (float) -((double) this.m_HSliderHeight + (double) this.m_HorizontalScrollbarSpacing)));
        rect = this.viewRect.get_rect();
        Vector3 vector3_1 = Vector2.op_Implicit(((Rect) ref rect).get_center());
        rect = this.viewRect.get_rect();
        Vector3 vector3_2 = Vector2.op_Implicit(((Rect) ref rect).get_size());
        this.m_ViewBounds = new Bounds(vector3_1, vector3_2);
        this.m_ContentBounds = this.GetBounds();
      }
      if (!this.m_VSliderExpand || !this.vScrollingNeeded || (this.viewRect.get_sizeDelta().x != 0.0 || this.viewRect.get_sizeDelta().y >= 0.0))
        return;
      this.viewRect.set_sizeDelta(new Vector2((float) -((double) this.m_VSliderWidth + (double) this.m_VerticalScrollbarSpacing), (float) this.viewRect.get_sizeDelta().y));
    }

    public virtual void SetLayoutVertical()
    {
      this.UpdateScrollbarLayout();
      Rect rect = this.viewRect.get_rect();
      Vector3 vector3_1 = Vector2.op_Implicit(((Rect) ref rect).get_center());
      rect = this.viewRect.get_rect();
      Vector3 vector3_2 = Vector2.op_Implicit(((Rect) ref rect).get_size());
      this.m_ViewBounds = new Bounds(vector3_1, vector3_2);
      this.m_ContentBounds = this.GetBounds();
    }

    private void UpdateScrollbarVisibility()
    {
      if (Object.op_Implicit((Object) this.m_VerticalScrollbar) && this.m_VerticalScrollbarVisibility != ScrollRectEx.ScrollbarVisibility.Permanent && ((Component) this.m_VerticalScrollbar).get_gameObject().get_activeSelf() != this.vScrollingNeeded)
        ((Component) this.m_VerticalScrollbar).get_gameObject().SetActive(this.vScrollingNeeded);
      if (!Object.op_Implicit((Object) this.m_HorizontalScrollbar) || this.m_HorizontalScrollbarVisibility == ScrollRectEx.ScrollbarVisibility.Permanent || ((Component) this.m_HorizontalScrollbar).get_gameObject().get_activeSelf() == this.hScrollingNeeded)
        return;
      ((Component) this.m_HorizontalScrollbar).get_gameObject().SetActive(this.hScrollingNeeded);
    }

    private void UpdateScrollbarLayout()
    {
      if (this.m_VSliderExpand && Object.op_Implicit((Object) this.m_HorizontalScrollbar))
      {
        ((DrivenRectTransformTracker) ref this.m_Tracker).Add((Object) this, this.m_HorizontalScrollbarRect, (DrivenTransformProperties) 5378);
        this.m_HorizontalScrollbarRect.set_anchorMin(new Vector2(0.0f, (float) this.m_HorizontalScrollbarRect.get_anchorMin().y));
        this.m_HorizontalScrollbarRect.set_anchorMax(new Vector2(1f, (float) this.m_HorizontalScrollbarRect.get_anchorMax().y));
        this.m_HorizontalScrollbarRect.set_anchoredPosition(new Vector2(0.0f, (float) this.m_HorizontalScrollbarRect.get_anchoredPosition().y));
        if (this.vScrollingNeeded)
          this.m_HorizontalScrollbarRect.set_sizeDelta(new Vector2((float) -((double) this.m_VSliderWidth + (double) this.m_VerticalScrollbarSpacing), (float) this.m_HorizontalScrollbarRect.get_sizeDelta().y));
        else
          this.m_HorizontalScrollbarRect.set_sizeDelta(new Vector2(0.0f, (float) this.m_HorizontalScrollbarRect.get_sizeDelta().y));
      }
      if (!this.m_HSliderExpand || !Object.op_Implicit((Object) this.m_VerticalScrollbar))
        return;
      ((DrivenRectTransformTracker) ref this.m_Tracker).Add((Object) this, this.m_VerticalScrollbarRect, (DrivenTransformProperties) 10756);
      this.m_VerticalScrollbarRect.set_anchorMin(new Vector2((float) this.m_VerticalScrollbarRect.get_anchorMin().x, 0.0f));
      this.m_VerticalScrollbarRect.set_anchorMax(new Vector2((float) this.m_VerticalScrollbarRect.get_anchorMax().x, 1f));
      this.m_VerticalScrollbarRect.set_anchoredPosition(new Vector2((float) this.m_VerticalScrollbarRect.get_anchoredPosition().x, 0.0f));
      if (this.hScrollingNeeded)
        this.m_VerticalScrollbarRect.set_sizeDelta(new Vector2((float) this.m_VerticalScrollbarRect.get_sizeDelta().x, (float) -((double) this.m_HSliderHeight + (double) this.m_HorizontalScrollbarSpacing)));
      else
        this.m_VerticalScrollbarRect.set_sizeDelta(new Vector2((float) this.m_VerticalScrollbarRect.get_sizeDelta().x, 0.0f));
    }

    private void UpdateBounds()
    {
      Rect rect = this.viewRect.get_rect();
      Vector3 vector3_1 = Vector2.op_Implicit(((Rect) ref rect).get_center());
      rect = this.viewRect.get_rect();
      Vector3 vector3_2 = Vector2.op_Implicit(((Rect) ref rect).get_size());
      this.m_ViewBounds = new Bounds(vector3_1, vector3_2);
      this.m_ContentBounds = this.GetBounds();
      if (Object.op_Equality((Object) this.m_Content, (Object) null))
        return;
      Vector3 size = ((Bounds) ref this.m_ContentBounds).get_size();
      Vector3 center = ((Bounds) ref this.m_ContentBounds).get_center();
      Vector3 vector3_3 = Vector3.op_Subtraction(((Bounds) ref this.m_ViewBounds).get_size(), size);
      if (vector3_3.x > 0.0)
      {
        ref __Null local = ref center.x;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local - (float) (vector3_3.x * (this.m_Content.get_pivot().x - 0.5));
        size.x = ((Bounds) ref this.m_ViewBounds).get_size().x;
      }
      if (vector3_3.y > 0.0)
      {
        ref __Null local = ref center.y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local = ^(float&) ref local - (float) (vector3_3.y * (this.m_Content.get_pivot().y - 0.5));
        size.y = ((Bounds) ref this.m_ViewBounds).get_size().y;
      }
      ((Bounds) ref this.m_ContentBounds).set_size(size);
      ((Bounds) ref this.m_ContentBounds).set_center(center);
    }

    private Bounds GetBounds()
    {
      if (Object.op_Equality((Object) this.m_Content, (Object) null))
        return (Bounds) null;
      Vector3 vector3_1;
      ((Vector3) ref vector3_1).\u002Ector(float.MaxValue, float.MaxValue, float.MaxValue);
      Vector3 vector3_2;
      ((Vector3) ref vector3_2).\u002Ector(float.MinValue, float.MinValue, float.MinValue);
      Matrix4x4 worldToLocalMatrix = ((Transform) this.viewRect).get_worldToLocalMatrix();
      this.m_Content.GetWorldCorners(this.m_Corners);
      for (int index = 0; index < 4; ++index)
      {
        Vector3 vector3_3 = ((Matrix4x4) ref worldToLocalMatrix).MultiplyPoint3x4(this.m_Corners[index]);
        vector3_1 = Vector3.Min(vector3_3, vector3_1);
        vector3_2 = Vector3.Max(vector3_3, vector3_2);
      }
      Bounds bounds;
      ((Bounds) ref bounds).\u002Ector(vector3_1, Vector3.get_zero());
      ((Bounds) ref bounds).Encapsulate(vector3_2);
      return bounds;
    }

    private Vector2 CalculateOffset(Vector2 delta)
    {
      Vector2 zero = Vector2.get_zero();
      if (this.m_MovementType == ScrollRectEx.MovementType.Unrestricted)
        return zero;
      Vector2 vector2_1 = Vector2.op_Implicit(((Bounds) ref this.m_ContentBounds).get_min());
      Vector2 vector2_2 = Vector2.op_Implicit(((Bounds) ref this.m_ContentBounds).get_max());
      if (this.m_Horizontal)
      {
        ref __Null local1 = ref vector2_1.x;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 + (float) delta.x;
        ref __Null local2 = ref vector2_2.x;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 + (float) delta.x;
        if (vector2_1.x > ((Bounds) ref this.m_ViewBounds).get_min().x)
          zero.x = ((Bounds) ref this.m_ViewBounds).get_min().x - vector2_1.x;
        else if (vector2_2.x < ((Bounds) ref this.m_ViewBounds).get_max().x)
          zero.x = ((Bounds) ref this.m_ViewBounds).get_max().x - vector2_2.x;
      }
      if (this.m_Vertical)
      {
        ref __Null local1 = ref vector2_1.y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local1 = ^(float&) ref local1 + (float) delta.y;
        ref __Null local2 = ref vector2_2.y;
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        // ISSUE: cast to a reference type
        // ISSUE: explicit reference operation
        ^(float&) ref local2 = ^(float&) ref local2 + (float) delta.y;
        if (vector2_2.y < ((Bounds) ref this.m_ViewBounds).get_max().y)
          zero.y = ((Bounds) ref this.m_ViewBounds).get_max().y - vector2_2.y;
        else if (vector2_1.y > ((Bounds) ref this.m_ViewBounds).get_min().y)
          zero.y = ((Bounds) ref this.m_ViewBounds).get_min().y - vector2_1.y;
      }
      return zero;
    }

    protected void SetDirty()
    {
      if (!base.IsActive())
        return;
      LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
    }

    protected void SetDirtyCaching()
    {
      if (!base.IsActive())
        return;
      CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild((ICanvasElement) this);
      LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
    }

    public void CenterOnPosition(Vector2 pos)
    {
      RectTransform transform = ((Component) this).get_transform() as RectTransform;
      Vector2 vector2_1;
      ((Vector2) ref vector2_1).\u002Ector((float) ((Transform) this.content).get_localScale().x, (float) ((Transform) this.content).get_localScale().y);
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
      Rect rect = this.content.get_rect();
      double num1 = (double) ((Rect) ref rect).get_width() * vector2_1.x;
      rect = transform.get_rect();
      double width = (double) ((Rect) ref rect).get_width();
      double num2 = num1 - width;
      rect = this.content.get_rect();
      double num3 = (double) ((Rect) ref rect).get_height() * vector2_1.y;
      rect = transform.get_rect();
      double height = (double) ((Rect) ref rect).get_height();
      double num4 = num3 - height;
      ((Vector2) ref local3).\u002Ector((float) num2, (float) num4);
      pos.x = pos.x / vector2_2.x + this.content.get_pivot().x;
      pos.y = pos.y / vector2_2.y + this.content.get_pivot().y;
      if (this.movementType != ScrollRectEx.MovementType.Unrestricted)
      {
        pos.x = (__Null) (double) Mathf.Clamp((float) pos.x, 0.0f, 1f);
        pos.y = (__Null) (double) Mathf.Clamp((float) pos.y, 0.0f, 1f);
      }
      this.normalizedPosition = pos;
    }

    public void LayoutComplete()
    {
    }

    public void GraphicUpdateComplete()
    {
    }

    [SpecialName]
    Transform ICanvasElement.get_transform()
    {
      return ((Component) this).get_transform();
    }

    public enum MovementType
    {
      Unrestricted,
      Elastic,
      Clamped,
    }

    public enum ScrollbarVisibility
    {
      Permanent,
      AutoHide,
      AutoHideAndExpandViewport,
    }

    [Serializable]
    public class ScrollRectEvent : UnityEvent<Vector2>
    {
      public ScrollRectEvent()
      {
        base.\u002Ector();
      }
    }
  }
}
