// Decompiled with JetBrains decompiler
// Type: AddSellOrderManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class AddSellOrderManager : MonoBehaviour
{
  public VirtualItemIcon sellItemIcon;
  public VirtualItemIcon currencyItemIcon;
  public GameObject itemSearchParent;
  public ItemSearchEntry itemSearchEntryPrefab;
  public InputField sellItemInput;
  public InputField sellItemAmount;
  public InputField currencyItemInput;
  public InputField currencyItemAmount;
  public VendingPanelAdmin adminPanel;

  public AddSellOrderManager()
  {
    base.\u002Ector();
  }
}
