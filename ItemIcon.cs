// Decompiled with JetBrains decompiler
// Type: ItemIcon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemIcon : BaseMonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler, IDraggable, IInventoryChanged, IItemAmountChanged, IItemIconChanged
{
  public static Color defaultBackgroundColor = new Color(0.9686275f, 0.9215686f, 0.8823529f, 0.03529412f);
  public static Color selectedBackgroundColor = new Color(0.1215686f, 0.4196078f, 0.627451f, 0.7843137f);
  public bool setSlotFromSiblingIndex = true;
  public bool allowSelection = true;
  public bool allowDropping = true;
  public ItemContainerSource containerSource;
  public int slotOffset;
  [Range(0.0f, 64f)]
  public int slot;
  public GameObject slots;
  public CanvasGroup iconContents;
  public Image iconImage;
  public Image underlayImage;
  public Text amountText;
  public Image hoverOutline;
  public Image cornerIcon;
  public Image lockedImage;
  public Image progressImage;
  public Image backgroundImage;
  public CanvasGroup conditionObject;
  public Image conditionFill;
  public Image maxConditionFill;
  [NonSerialized]
  public Item item;
  [NonSerialized]
  public bool invalidSlot;
  public SoundDefinition hoverSound;

  public virtual void OnPointerClick(PointerEventData eventData)
  {
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
  }

  public void OnPointerExit(PointerEventData eventData)
  {
  }
}
