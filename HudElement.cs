// Decompiled with JetBrains decompiler
// Type: HudElement
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class HudElement : MonoBehaviour
{
  public Text[] ValueText;
  public Image[] FilledImage;
  private float LastValue;

  public void SetValue(float value, float max = 1f)
  {
    using (TimeWarning.New("HudElement.SetValue", 0.1f))
    {
      float f = value / max;
      if ((double) f == (double) this.LastValue)
        return;
      this.LastValue = f;
      this.SetText(value.ToString("0"));
      this.SetImage(f);
    }
  }

  private void SetText(string v)
  {
    for (int index = 0; index < this.ValueText.Length; ++index)
      this.ValueText[index].set_text(v);
  }

  private void SetImage(float f)
  {
    for (int index = 0; index < this.FilledImage.Length; ++index)
      this.FilledImage[index].set_fillAmount(f);
  }

  public HudElement()
  {
    base.\u002Ector();
  }
}
