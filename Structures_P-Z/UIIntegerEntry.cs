// Decompiled with JetBrains decompiler
// Type: UIIntegerEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class UIIntegerEntry : MonoBehaviour
{
  public InputField textEntry;

  public event Action textChanged;

  public void OnAmountTextChanged()
  {
    this.textChanged();
  }

  public void SetAmount(int amount)
  {
    if (amount == this.GetIntAmount())
      return;
    this.textEntry.set_text(amount.ToString());
  }

  public int GetIntAmount()
  {
    int result = 0;
    int.TryParse(this.textEntry.get_text(), out result);
    return result;
  }

  public void PlusMinus(int delta)
  {
    this.SetAmount(this.GetIntAmount() + delta);
  }

  public UIIntegerEntry()
  {
    base.\u002Ector();
  }
}
