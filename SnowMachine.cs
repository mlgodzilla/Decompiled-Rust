// Decompiled with JetBrains decompiler
// Type: SnowMachine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SnowMachine : FogMachine
{
  public AdaptMeshToTerrain snowMesh;
  public TriggerTemperature tempTrigger;

  public override bool MotionModeEnabled()
  {
    return false;
  }

  public override void EnableFogField()
  {
    base.EnableFogField();
    ((Component) this.tempTrigger).get_gameObject().SetActive(true);
  }

  public override void FinishFogging()
  {
    base.FinishFogging();
    ((Component) this.tempTrigger).get_gameObject().SetActive(false);
  }
}
