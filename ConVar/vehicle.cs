// Decompiled with JetBrains decompiler
// Type: ConVar.vehicle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("vehicle")]
  public class vehicle : ConsoleSystem
  {
    [ServerVar]
    [Help("how long until boat corpses despawn")]
    public static float boat_corpse_seconds = 300f;

    [ServerUserVar]
    public static void swapseats(ConsoleSystem.Arg arg)
    {
      int targetSeat = 0;
      BasePlayer player = arg.Player();
      if (Object.op_Equality((Object) player, (Object) null) || player.SwapSeatCooldown())
        return;
      BaseMountable mounted = player.GetMounted();
      if (Object.op_Equality((Object) mounted, (Object) null))
        return;
      BaseVehicle baseVehicle = (BaseVehicle) ((Component) mounted).GetComponent<BaseVehicle>();
      if (Object.op_Equality((Object) baseVehicle, (Object) null))
        baseVehicle = mounted.VehicleParent();
      if (Object.op_Equality((Object) baseVehicle, (Object) null))
        return;
      baseVehicle.SwapSeats(player, targetSeat);
    }

    public vehicle()
    {
      base.\u002Ector();
    }
  }
}
