// Decompiled with JetBrains decompiler
// Type: ToggleLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class ToggleLayer : MonoBehaviour, IClientComponent
{
  public Toggle toggleControl;
  public Text textControl;
  public LayerSelect layer;

  protected void OnEnable()
  {
    if (!Object.op_Implicit((Object) MainCamera.mainCamera))
      return;
    this.toggleControl.set_isOn((uint) (MainCamera.mainCamera.get_cullingMask() & this.layer.Mask) > 0U);
  }

  public void OnToggleChanged()
  {
    if (!Object.op_Implicit((Object) MainCamera.mainCamera))
      return;
    if (this.toggleControl.get_isOn())
    {
      Camera mainCamera = MainCamera.mainCamera;
      mainCamera.set_cullingMask(mainCamera.get_cullingMask() | this.layer.Mask);
    }
    else
    {
      Camera mainCamera = MainCamera.mainCamera;
      mainCamera.set_cullingMask(mainCamera.get_cullingMask() & ~this.layer.Mask);
    }
  }

  protected void OnValidate()
  {
    if (!Object.op_Implicit((Object) this.textControl))
      return;
    this.textControl.set_text(this.layer.Name);
  }

  public ToggleLayer()
  {
    base.\u002Ector();
  }
}
