// Decompiled with JetBrains decompiler
// Type: BaseWheeledVehicle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class BaseWheeledVehicle : BaseVehicle
{
  [Header("Wheels")]
  public BaseWheeledVehicle.VehicleWheel[] wheels;

  [Serializable]
  public class VehicleWheel
  {
    public bool brakeWheel = true;
    public bool powerWheel = true;
    public Transform shock;
    public WheelCollider wheelCollider;
    public Transform wheel;
    public Transform axle;
    public bool steerWheel;
  }
}
