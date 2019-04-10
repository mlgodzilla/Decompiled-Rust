// Decompiled with JetBrains decompiler
// Type: KeyCodeEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine.UI;

public class KeyCodeEntry : UIDialog
{
  public Text textDisplay;
  public Action<string> onCodeEntered;
  public Text typeDisplay;
  public Translate.Phrase masterCodePhrase;
  public Translate.Phrase guestCodePhrase;
}
