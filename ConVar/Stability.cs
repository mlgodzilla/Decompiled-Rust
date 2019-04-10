// Decompiled with JetBrains decompiler
// Type: ConVar.Stability
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Linq;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("stability")]
  public class Stability : ConsoleSystem
  {
    [ServerVar]
    public static int verbose = 0;
    [ServerVar]
    public static int strikes = 10;
    [ServerVar]
    public static float collapse = 0.05f;
    [ServerVar]
    public static float accuracy = 1f / 1000f;
    [ServerVar]
    public static float stabilityqueue = 9f;
    [ServerVar]
    public static float surroundingsqueue = 3f;

    [ServerVar]
    public static void refresh_stability(ConsoleSystem.Arg args)
    {
      StabilityEntity[] array = BaseNetworkable.serverEntities.OfType<StabilityEntity>().ToArray<StabilityEntity>();
      Debug.Log((object) ("Refreshing stability on " + (object) array.Length + " entities..."));
      for (int index = 0; index < array.Length; ++index)
        array[index].UpdateStability();
    }

    public Stability()
    {
      base.\u002Ector();
    }
  }
}
