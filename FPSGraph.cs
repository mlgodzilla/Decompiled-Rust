// Decompiled with JetBrains decompiler
// Type: FPSGraph
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using UnityEngine;

public class FPSGraph : Graph
{
  public void Refresh()
  {
    ((Behaviour) this).set_enabled(FPS.graph > 0);
    ((Rect) ref this.Area).set_width((float) (this.Resolution = Mathf.Clamp(FPS.graph, 0, Screen.get_width())));
  }

  protected void OnEnable()
  {
    this.Refresh();
  }

  protected override float GetValue()
  {
    return 1f / Time.get_deltaTime();
  }

  protected override Color GetColor(float value)
  {
    if ((double) value < 10.0)
      return Color.get_red();
    if ((double) value >= 30.0)
      return Color.get_green();
    return Color.get_yellow();
  }
}
