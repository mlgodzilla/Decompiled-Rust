// Decompiled with JetBrains decompiler
// Type: ConVar.Bradley
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("bradley")]
  public class Bradley : ConsoleSystem
  {
    [ServerVar]
    public static float respawnDelayMinutes = 60f;
    [ServerVar]
    public static float respawnDelayVariance = 1f;
    [ServerVar]
    public static bool enabled = true;

    [ServerVar]
    public static void quickrespawn(ConsoleSystem.Arg arg)
    {
      if (!Object.op_Implicit((Object) arg.Player()))
        return;
      BradleySpawner singleton = BradleySpawner.singleton;
      if (Object.op_Equality((Object) singleton, (Object) null))
      {
        Debug.LogWarning((object) "No Spawner");
      }
      else
      {
        if (Object.op_Implicit((Object) singleton.spawned))
          singleton.spawned.Kill(BaseNetworkable.DestroyMode.None);
        singleton.spawned = (BradleyAPC) null;
        singleton.DoRespawn();
      }
    }

    public Bradley()
    {
      base.\u002Ector();
    }
  }
}
