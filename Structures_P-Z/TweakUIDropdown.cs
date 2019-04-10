// Decompiled with JetBrains decompiler
// Type: TweakUIDropdown
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class TweakUIDropdown : MonoBehaviour
{
  public Button Left;
  public Button Right;
  public Text Current;
  public Image BackgroundImage;
  public TweakUIDropdown.NameValue[] nameValues;
  public string convarName;
  public bool assignImageColor;
  internal ConsoleSystem.Command conVar;
  public int currentValue;

  protected void Awake()
  {
    this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
    if (this.conVar == null)
      Debug.LogWarning((object) ("TweakUIDropDown Convar Missing: " + this.convarName));
    else
      this.UpdateState();
  }

  protected void OnEnable()
  {
    this.UpdateState();
  }

  public void OnValueChanged()
  {
    this.UpdateConVar();
  }

  public void ChangeValue(int change)
  {
    this.currentValue += change;
    if (this.currentValue < 0)
      this.currentValue = 0;
    if (this.currentValue > this.nameValues.Length - 1)
      this.currentValue = this.nameValues.Length - 1;
    ((Selectable) this.Left).set_interactable(this.currentValue > 0);
    ((Selectable) this.Right).set_interactable(this.currentValue < this.nameValues.Length - 1);
    this.UpdateConVar();
  }

  private void UpdateConVar()
  {
    TweakUIDropdown.NameValue nameValue = this.nameValues[this.currentValue];
    if (this.conVar == null || this.conVar.get_String() == nameValue.value)
      return;
    this.conVar.Set(nameValue.value);
    this.UpdateState();
  }

  private void UpdateState()
  {
    if (this.conVar == null)
      return;
    string str = this.conVar.get_String();
    for (int index = 0; index < this.nameValues.Length; ++index)
    {
      if (!(this.nameValues[index].value != str))
      {
        this.Current.set_text(this.nameValues[index].label.translated);
        this.currentValue = index;
        if (this.assignImageColor)
          ((Graphic) this.BackgroundImage).set_color(this.nameValues[index].imageColor);
      }
    }
  }

  public TweakUIDropdown()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class NameValue
  {
    public string value;
    public Color imageColor;
    public Translate.Phrase label;
  }
}
