// Decompiled with JetBrains decompiler
// Type: ConVar.Spawn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("spawn")]
  public class Spawn : ConsoleSystem
  {
    [ServerVar]
    public static float min_rate = 0.5f;
    [ServerVar]
    public static float max_rate = 1f;
    [ServerVar]
    public static float min_density = 0.5f;
    [ServerVar]
    public static float max_density = 1f;
    [ServerVar]
    public static float player_base = 100f;
    [ServerVar]
    public static float player_scale = 2f;
    [ServerVar]
    public static bool respawn_populations = true;
    [ServerVar]
    public static bool respawn_groups = true;
    [ServerVar]
    public static bool respawn_individuals = true;
    [ServerVar]
    public static float tick_populations = 60f;
    [ServerVar]
    public static float tick_individuals = 300f;

    [ServerVar]
    public static void fill_populations(ConsoleSystem.Arg args)
    {
      if (!Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance))
        return;
      ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).FillPopulations();
    }

    [ServerVar]
    public static void fill_groups(ConsoleSystem.Arg args)
    {
      if (!Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance))
        return;
      ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).FillGroups();
    }

    [ServerVar]
    public static void fill_individuals(ConsoleSystem.Arg args)
    {
      if (!Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance))
        return;
      ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).FillIndividuals();
    }

    [ServerVar]
    public static void report(ConsoleSystem.Arg args)
    {
      if (Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance))
        args.ReplyWith(((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).GetReport(false));
      else
        args.ReplyWith("No spawn handler found.");
    }

    [ServerVar]
    public static void scalars(ConsoleSystem.Arg args)
    {
      TextTable textTable = new TextTable();
      textTable.AddColumn("Type");
      textTable.AddColumn("Value");
      textTable.AddRow(new string[2]
      {
        "Player Fraction",
        SpawnHandler.PlayerFraction().ToString()
      });
      textTable.AddRow(new string[2]
      {
        "Player Excess",
        SpawnHandler.PlayerExcess().ToString()
      });
      textTable.AddRow(new string[2]
      {
        "Population Rate",
        SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate).ToString()
      });
      textTable.AddRow(new string[2]
      {
        "Population Density",
        SpawnHandler.PlayerLerp(Spawn.min_density, Spawn.max_density).ToString()
      });
      textTable.AddRow(new string[2]
      {
        "Group Rate",
        SpawnHandler.PlayerScale(Spawn.player_scale).ToString()
      });
      args.ReplyWith(((object) textTable).ToString());
    }

    public Spawn()
    {
      base.\u002Ector();
    }
  }
}
