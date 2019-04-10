// Decompiled with JetBrains decompiler
// Type: LocalizeText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LocalizeText : MonoBehaviour, IClientComponent, ILanguageChanged
{
  public string token;
  [TextArea]
  public string english;
  public string append;
  public LocalizeText.SpecialMode specialMode;

  public LocalizeText()
  {
    base.\u002Ector();
  }

  public enum SpecialMode
  {
    None,
    AllUppercase,
    AllLowercase,
  }
}
