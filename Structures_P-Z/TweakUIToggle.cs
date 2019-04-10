// Decompiled with JetBrains decompiler
// Type: TweakUIToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class TweakUIToggle : MonoBehaviour
{
  public Toggle toggleControl;
  public string convarName;
  public bool inverse;
  internal ConsoleSystem.Command conVar;

  protected void Awake()
  {
    this.conVar = ConsoleSystem.Index.Client.Find(this.convarName);
    if (this.conVar != null)
      this.UpdateToggleState();
    else
      Debug.LogWarning((object) ("Tweak Toggle Convar Missing: " + this.convarName));
  }

  protected void OnEnable()
  {
    this.UpdateToggleState();
  }

  public void OnToggleChanged()
  {
    this.UpdateConVar();
  }

  private void UpdateConVar()
  {
    if (this.conVar == null)
      return;
    bool flag = this.toggleControl.get_isOn();
    if (this.inverse)
      flag = !flag;
    if (this.conVar.get_AsBool() == flag)
      return;
    this.conVar.Set(flag);
  }

  private void UpdateToggleState()
  {
    if (this.conVar == null)
      return;
    bool flag = this.conVar.get_AsBool();
    if (this.inverse)
      flag = !flag;
    this.toggleControl.set_isOn(flag);
  }

  public TweakUIToggle()
  {
    base.\u002Ector();
  }
}
