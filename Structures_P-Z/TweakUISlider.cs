// Decompiled with JetBrains decompiler
// Type: TweakUISlider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class TweakUISlider : MonoBehaviour
{
  public Slider sliderControl;
  public Text textControl;
  public string convarName;
  internal ConsoleSystem.Command conVar;

  protected void Awake()
  {
    this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
    if (this.conVar != null)
    {
      this.UpdateSliderValue();
      this.UpdateTextValue();
    }
    else
      Debug.LogWarning((object) ("Tweak Slider Convar Missing: " + this.convarName));
  }

  protected void OnEnable()
  {
    this.UpdateSliderValue();
    this.UpdateTextValue();
  }

  public void OnChanged()
  {
    this.UpdateConVar();
    this.UpdateTextValue();
    this.UpdateSliderValue();
  }

  private void UpdateConVar()
  {
    if (this.conVar == null)
      return;
    float num = this.sliderControl.get_value();
    if ((double) this.conVar.get_AsFloat() == (double) num)
      return;
    this.conVar.Set(num);
  }

  private void UpdateSliderValue()
  {
    if (this.conVar == null)
      return;
    float asFloat = this.conVar.get_AsFloat();
    if ((double) this.sliderControl.get_value() == (double) asFloat)
      return;
    this.sliderControl.set_value(asFloat);
  }

  private void UpdateTextValue()
  {
    if (this.sliderControl.get_wholeNumbers())
      this.textControl.set_text(this.sliderControl.get_value().ToString("N0"));
    else
      this.textControl.set_text(this.sliderControl.get_value().ToString("0.0"));
  }

  public TweakUISlider()
  {
    base.\u002Ector();
  }
}
