// Decompiled with JetBrains decompiler
// Type: ToggleGroupCookie
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleGroupCookie : MonoBehaviour
{
  public ToggleGroup group
  {
    get
    {
      return (ToggleGroup) ((Component) this).GetComponent<ToggleGroup>();
    }
  }

  private void OnEnable()
  {
    string str = PlayerPrefs.GetString("ToggleGroupCookie_" + ((Object) this).get_name());
    if (!string.IsNullOrEmpty(str))
    {
      Transform transform = ((Component) this).get_transform().Find(str);
      if (Object.op_Implicit((Object) transform))
      {
        Toggle component = (Toggle) ((Component) transform).GetComponent<Toggle>();
        if (Object.op_Implicit((Object) component))
        {
          foreach (Toggle componentsInChild in (Toggle[]) ((Component) this).GetComponentsInChildren<Toggle>(true))
            componentsInChild.set_isOn(false);
          component.set_isOn(false);
          component.set_isOn(true);
          this.SetupListeners();
          return;
        }
      }
    }
    Toggle toggle = this.group.ActiveToggles().FirstOrDefault<Toggle>((Func<Toggle, bool>) (x => x.get_isOn()));
    if (Object.op_Implicit((Object) toggle))
    {
      toggle.set_isOn(false);
      toggle.set_isOn(true);
    }
    this.SetupListeners();
  }

  private void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    foreach (Toggle componentsInChild in (Toggle[]) ((Component) this).GetComponentsInChildren<Toggle>(true))
    {
      // ISSUE: method pointer
      ((UnityEvent<bool>) componentsInChild.onValueChanged).RemoveListener(new UnityAction<bool>((object) this, __methodptr(OnToggleChanged)));
    }
  }

  private void SetupListeners()
  {
    foreach (Toggle componentsInChild in (Toggle[]) ((Component) this).GetComponentsInChildren<Toggle>(true))
    {
      // ISSUE: method pointer
      ((UnityEvent<bool>) componentsInChild.onValueChanged).AddListener(new UnityAction<bool>((object) this, __methodptr(OnToggleChanged)));
    }
  }

  private void OnToggleChanged(bool b)
  {
    Toggle toggle = ((IEnumerable<Toggle>) ((Component) this).GetComponentsInChildren<Toggle>()).FirstOrDefault<Toggle>((Func<Toggle, bool>) (x => x.get_isOn()));
    if (!Object.op_Implicit((Object) toggle))
      return;
    PlayerPrefs.SetString("ToggleGroupCookie_" + ((Object) this).get_name(), ((Object) ((Component) toggle).get_gameObject()).get_name());
  }

  public ToggleGroupCookie()
  {
    base.\u002Ector();
  }
}
