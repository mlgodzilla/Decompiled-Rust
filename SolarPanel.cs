// Decompiled with JetBrains decompiler
// Type: SolarPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SolarPanel : IOEntity
{
  public int maximalPowerOutput = 10;
  public float dot_minimum = 0.1f;
  public float dot_maximum = 0.6f;
  public Transform sunSampler;
  private const int tickrateSeconds = 60;

  public override bool IsRootEntity()
  {
    return true;
  }

  public override void ServerInit()
  {
    base.ServerInit();
    this.InvokeRandomized(new Action(this.SunUpdate), 1f, 5f, 2f);
  }

  public void SunUpdate()
  {
    int currentEnergy = this.currentEnergy;
    int num1;
    if (TOD_Sky.get_Instance().get_IsNight())
    {
      num1 = 0;
    }
    else
    {
      Vector3 vector3 = Vector3.op_Subtraction(((GameObject) TOD_Sky.get_Instance().get_Components().Sun).get_transform().get_position(), ((Component) this.sunSampler).get_transform().get_position());
      Vector3 normalized = ((Vector3) ref vector3).get_normalized();
      float num2 = Mathf.InverseLerp(this.dot_minimum, this.dot_maximum, Vector3.Dot(((Component) this.sunSampler).get_transform().get_forward(), normalized));
      if ((double) num2 > 0.0 && !this.IsVisible(Vector3.op_Addition(((Component) this.sunSampler).get_transform().get_position(), Vector3.op_Multiply(normalized, 100f)), 101f))
        num2 = 0.0f;
      num1 = Mathf.FloorToInt((float) this.maximalPowerOutput * num2 * this.healthFraction);
    }
    int num3 = this.currentEnergy != num1 ? 1 : 0;
    this.currentEnergy = num1;
    if (num3 == 0)
      return;
    this.MarkDirty();
  }

  public override int ConsumptionAmount()
  {
    return 0;
  }
}
