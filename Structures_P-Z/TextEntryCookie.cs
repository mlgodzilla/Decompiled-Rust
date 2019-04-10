// Decompiled with JetBrains decompiler
// Type: TextEntryCookie
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextEntryCookie : MonoBehaviour
{
  public InputField control
  {
    get
    {
      return (InputField) ((Component) this).GetComponent<InputField>();
    }
  }

  private void OnEnable()
  {
    string str = PlayerPrefs.GetString("TextEntryCookie_" + ((Object) this).get_name());
    if (!string.IsNullOrEmpty(str))
      this.control.set_text(str);
    ((UnityEvent<string>) this.control.get_onValueChanged()).Invoke(this.control.get_text());
  }

  private void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    PlayerPrefs.SetString("TextEntryCookie_" + ((Object) this).get_name(), this.control.get_text());
  }

  public TextEntryCookie()
  {
    base.\u002Ector();
  }
}
