// Decompiled with JetBrains decompiler
// Type: ToggleTerrainTrees
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class ToggleTerrainTrees : MonoBehaviour
{
  public Toggle toggleControl;
  public Text textControl;

  protected void OnEnable()
  {
    if (!Object.op_Implicit((Object) Terrain.get_activeTerrain()))
      return;
    this.toggleControl.set_isOn(Terrain.get_activeTerrain().get_drawTreesAndFoliage());
  }

  public void OnToggleChanged()
  {
    if (!Object.op_Implicit((Object) Terrain.get_activeTerrain()))
      return;
    Terrain.get_activeTerrain().set_drawTreesAndFoliage(this.toggleControl.get_isOn());
  }

  protected void OnValidate()
  {
    if (!Object.op_Implicit((Object) this.textControl))
      return;
    this.textControl.set_text("Terrain Trees");
  }

  public ToggleTerrainTrees()
  {
    base.\u002Ector();
  }
}
