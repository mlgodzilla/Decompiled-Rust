// Decompiled with JetBrains decompiler
// Type: MonumentMarker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class MonumentMarker : MonoBehaviour
{
  public Text text;

  public void Setup(MonumentInfo info)
  {
    string translated = info.displayPhrase.translated;
    this.text.set_text(string.IsNullOrEmpty(translated) ? "Monument" : translated);
  }

  public MonumentMarker()
  {
    base.\u002Ector();
  }
}
