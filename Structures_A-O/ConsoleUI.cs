// Decompiled with JetBrains decompiler
// Type: ConsoleUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class ConsoleUI : SingletonComponent<ConsoleUI>
{
  public Text text;
  public InputField outputField;
  public InputField inputField;
  public GameObject AutocompleteDropDown;
  public GameObject ItemTemplate;
  public Color errorColor;
  public Color warningColor;
  public Color inputColor;

  public ConsoleUI()
  {
    base.\u002Ector();
  }
}
