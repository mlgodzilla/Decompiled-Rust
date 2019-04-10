// Decompiled with JetBrains decompiler
// Type: UIMapVendingMachineMarker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ProtoBuf;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapVendingMachineMarker : MonoBehaviour
{
  public Color inStock;
  public Color outOfStock;
  public Image colorBackground;
  public string displayName;
  public Tooltip toolTip;
  private bool isInStock;

  public void SetOutOfStock(bool stock)
  {
    ((Graphic) this.colorBackground).set_color(stock ? this.inStock : this.outOfStock);
    this.isInStock = stock;
  }

  public void UpdateDisplayName(
    string newName,
    VendingMachine.SellOrderContainer sellOrderContainer)
  {
    this.displayName = newName;
    this.toolTip.Text = this.displayName;
    if (this.isInStock && sellOrderContainer != null && (sellOrderContainer.sellOrders != null && ((List<VendingMachine.SellOrder>) sellOrderContainer.sellOrders).Count > 0))
    {
      this.toolTip.Text += "\n";
      using (List<VendingMachine.SellOrder>.Enumerator enumerator = ((List<VendingMachine.SellOrder>) sellOrderContainer.sellOrders).GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          VendingMachine.SellOrder current = enumerator.Current;
          if (current.inStock > 0)
          {
            string str1 = ItemManager.FindItemDefinition((int) current.itemToSellID).displayName.translated + (current.itemToSellIsBP != null ? " (BP)" : "");
            string str2 = ItemManager.FindItemDefinition((int) current.currencyID).displayName.translated + (current.currencyIsBP != null ? " (BP)" : "");
            Tooltip toolTip1 = this.toolTip;
            toolTip1.Text = toolTip1.Text + "\n" + (object) (int) current.itemToSellAmount + " " + str1 + " | " + (object) (int) current.currencyAmountPerItem + " " + str2;
            Tooltip toolTip2 = this.toolTip;
            toolTip2.Text = toolTip2.Text + " (" + (object) (int) current.inStock + " Left)";
          }
        }
      }
    }
    ((Behaviour) this.toolTip).set_enabled(this.toolTip.Text != "");
  }

  public UIMapVendingMachineMarker()
  {
    base.\u002Ector();
  }
}
