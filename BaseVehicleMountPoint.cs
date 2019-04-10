// Decompiled with JetBrains decompiler
// Type: BaseVehicleMountPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BaseVehicleMountPoint : BaseMountable
{
  public override bool DirectlyMountable()
  {
    return false;
  }

  public BaseVehicle GetVehicleParent()
  {
    return this.GetParentEntity() as BaseVehicle;
  }

  public override float WaterFactorForPlayer(BasePlayer player)
  {
    BaseVehicle vehicleParent = this.GetVehicleParent();
    if (Object.op_Equality((Object) vehicleParent, (Object) null))
      return 0.0f;
    return vehicleParent.WaterFactorForPlayer(player);
  }
}
