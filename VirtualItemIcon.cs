// Decompiled with JetBrains decompiler
// Type: VirtualItemIcon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class VirtualItemIcon : MonoBehaviour
{
  public ItemDefinition itemDef;
  public int itemAmount;
  public bool asBlueprint;
  public Image iconImage;
  public Image bpUnderlay;
  public Text amountText;
  public CanvasGroup iconContents;
  public CanvasGroup conditionObject;
  public Image conditionFill;
  public Image maxConditionFill;
  public Image cornerIcon;

  public VirtualItemIcon()
  {
    base.\u002Ector();
  }
}
