// Decompiled with JetBrains decompiler
// Type: DragMe
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragMe : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
  public static DragMe dragging;
  public static GameObject dragIcon;
  public static object data;
  [NonSerialized]
  public string dragType;

  public virtual void OnBeginDrag(PointerEventData eventData)
  {
  }

  public virtual void OnDrag(PointerEventData eventData)
  {
  }

  public virtual void OnEndDrag(PointerEventData eventData)
  {
  }

  public DragMe()
  {
    base.\u002Ector();
  }
}
