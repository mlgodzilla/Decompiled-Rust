// Decompiled with JetBrains decompiler
// Type: ToggleTerrainRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class ToggleTerrainRenderer : MonoBehaviour
{
  public Toggle toggleControl;
  public Text textControl;

  protected void OnEnable()
  {
    if (!Object.op_Implicit((Object) Terrain.get_activeTerrain()))
      return;
    this.toggleControl.set_isOn(Terrain.get_activeTerrain().get_drawHeightmap());
  }

  public void OnToggleChanged()
  {
    if (!Object.op_Implicit((Object) Terrain.get_activeTerrain()))
      return;
    Terrain.get_activeTerrain().set_drawHeightmap(this.toggleControl.get_isOn());
  }

  protected void OnValidate()
  {
    if (!Object.op_Implicit((Object) this.textControl))
      return;
    this.textControl.set_text("Terrain Renderer");
  }

  public ToggleTerrainRenderer()
  {
    base.\u002Ector();
  }
}
