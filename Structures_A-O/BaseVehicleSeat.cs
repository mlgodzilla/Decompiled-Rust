// Decompiled with JetBrains decompiler
// Type: BaseVehicleSeat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BaseVehicleSeat : BaseVehicleMountPoint
{
  public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
  {
    BaseVehicle vehicleParent = this.GetVehicleParent();
    if (Object.op_Equality((Object) vehicleParent, (Object) null))
      return;
    vehicleParent.MounteeTookDamage(mountee, info);
  }

  public override void PlayerServerInput(InputState inputState, BasePlayer player)
  {
    BaseVehicle vehicleParent = this.GetVehicleParent();
    if (Object.op_Inequality((Object) vehicleParent, (Object) null))
      vehicleParent.PlayerServerInput(inputState, player);
    base.PlayerServerInput(inputState, player);
  }

  public override void LightToggle(BasePlayer player)
  {
    BaseVehicle vehicleParent = this.GetVehicleParent();
    if (Object.op_Equality((Object) vehicleParent, (Object) null))
      return;
    vehicleParent.LightToggle(player);
  }

  public override float GetSteering(BasePlayer player)
  {
    return this.GetVehicleParent().GetSteering(player);
  }
}
