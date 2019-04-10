// Decompiled with JetBrains decompiler
// Type: ItemTextValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class ItemTextValue : MonoBehaviour
{
  public Text text;
  public Color bad;
  public Color good;
  public bool negativestat;
  public bool asPercentage;
  public bool useColors;
  public bool signed;
  public string suffix;

  public void SetValue(float val, int numDecimals = 0, string overrideText = "")
  {
    this.text.set_text(overrideText == "" ? string.Format("{0}{1:n" + (object) numDecimals + "}", (double) val <= 0.0 || !this.signed ? (object) "" : (object) "+", (object) val) : overrideText);
    if (this.asPercentage)
    {
      Text text = this.text;
      text.set_text(text.get_text() + " %");
    }
    if (this.suffix != "")
    {
      Text text = this.text;
      text.set_text(text.get_text() + this.suffix);
    }
    bool flag = (double) val > 0.0;
    if (this.negativestat)
      flag = !flag;
    if (!this.useColors)
      return;
    ((Graphic) this.text).set_color(flag ? this.good : this.bad);
  }

  public ItemTextValue()
  {
    base.\u002Ector();
  }
}
