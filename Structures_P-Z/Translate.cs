// Decompiled with JetBrains decompiler
// Type: Translate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using UnityEngine;

public static class Translate
{
  public static string TranslateMouseButton(string mouseButton)
  {
    if (mouseButton == "mouse0")
      return "Left Mouse";
    if (mouseButton == "mouse1")
      return "Right Mouse";
    if (mouseButton == "mouse2")
      return "Center Mouse";
    return mouseButton;
  }

  [Serializable]
  public class Phrase
  {
    public string token;
    [TextArea]
    public string english;

    public virtual string translated
    {
      get
      {
        string.IsNullOrEmpty(this.token);
        return this.english;
      }
    }

    public bool IsValid()
    {
      return !string.IsNullOrEmpty(this.token);
    }

    public Phrase(string t = "", string eng = "")
    {
      this.token = t;
      this.english = eng;
    }
  }

  [Serializable]
  public class TokenisedPhrase : Translate.Phrase
  {
    public override string translated
    {
      get
      {
        return base.translated.Replace("[inventory.toggle]", string.Format("[{0}]", (object) Input.GetButtonWithBind("inventory.toggle").ToUpper())).Replace("[inventory.togglecrafting]", string.Format("[{0}]", (object) Input.GetButtonWithBind("inventory.togglecrafting").ToUpper())).Replace("[+map]", string.Format("[{0}]", (object) Input.GetButtonWithBind("+map").ToUpper())).Replace("[inventory.examineheld]", string.Format("[{0}]", (object) Input.GetButtonWithBind("inventory.examineheld").ToUpper())).Replace("[slot2]", string.Format("[{0}]", (object) Input.GetButtonWithBind("+slot2").ToUpper())).Replace("[attack]", string.Format("[{0}]", (object) Translate.TranslateMouseButton(Input.GetButtonWithBind("+attack")).ToUpper())).Replace("[attack2]", string.Format("[{0}]", (object) Translate.TranslateMouseButton(Input.GetButtonWithBind("+attack2")).ToUpper())).Replace("[+use]", string.Format("[{0}]", (object) Translate.TranslateMouseButton(Input.GetButtonWithBind("+use")).ToUpper())).Replace("[+altlook]", string.Format("[{0}]", (object) Translate.TranslateMouseButton(Input.GetButtonWithBind("+altlook")).ToUpper())).Replace("[+reload]", string.Format("[{0}]", (object) Translate.TranslateMouseButton(Input.GetButtonWithBind("+reload")).ToUpper())).Replace("[+voice]", string.Format("[{0}]", (object) Translate.TranslateMouseButton(Input.GetButtonWithBind("+voice")).ToUpper()));
      }
    }

    public TokenisedPhrase(string t = "", string eng = "")
      : base(t, eng)
    {
    }
  }
}
