// Decompiled with JetBrains decompiler
// Type: TweakUIMultiSelect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TweakUIMultiSelect : MonoBehaviour
{
  public ToggleGroup toggleGroup;
  public string convarName;
  internal ConsoleSystem.Command conVar;

  protected void Awake()
  {
    this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
    if (this.conVar != null)
      this.UpdateToggleGroup();
    else
      Debug.LogWarning((object) ("Tweak Slider Convar Missing: " + this.convarName));
  }

  protected void OnEnable()
  {
    this.UpdateToggleGroup();
  }

  public void OnChanged()
  {
    this.UpdateConVar();
  }

  private void UpdateToggleGroup()
  {
    if (this.conVar == null)
      return;
    string str = this.conVar.get_String();
    foreach (Toggle componentsInChild in (Toggle[]) ((Component) this.toggleGroup).GetComponentsInChildren<Toggle>())
      componentsInChild.set_isOn(((Object) componentsInChild).get_name() == str);
  }

  private void UpdateConVar()
  {
    if (this.conVar == null)
      return;
    Toggle toggle = ((IEnumerable<Toggle>) ((Component) this.toggleGroup).GetComponentsInChildren<Toggle>()).Where<Toggle>((Func<Toggle, bool>) (x => x.get_isOn())).FirstOrDefault<Toggle>();
    if (Object.op_Equality((Object) toggle, (Object) null) || this.conVar.get_String() == ((Object) toggle).get_name())
      return;
    this.conVar.Set(((Object) toggle).get_name());
  }

  public TweakUIMultiSelect()
  {
    base.\u002Ector();
  }
}
