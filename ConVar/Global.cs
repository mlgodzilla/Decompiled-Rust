// Decompiled with JetBrains decompiler
// Type: ConVar.Global
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Facepunch.Extend;
using Network;
using Network.Visibility;
using Rust;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace ConVar
{
  [ConsoleSystem.Factory("global")]
  public class Global : ConsoleSystem
  {
    [ServerVar]
    [ClientVar]
    public static int maxthreads = 8;
    [ServerVar(Saved = true)]
    [ClientVar(Saved = true)]
    public static int perf = 0;
    [ClientVar(ClientInfo = true, Help = "If you're an admin this will enable god mode", Saved = true)]
    public static bool god = false;
    [ClientVar(ClientInfo = true, Help = "If enabled you will be networked when you're spectating. This means that you will hear audio chat, but also means that cheaters will potentially be able to detect you watching them.", Saved = true)]
    public static bool specnet = false;
    private static int _developer;

    [ServerVar]
    [ClientVar]
    public static bool timewarning
    {
      get
      {
        return (bool) TimeWarning.Enabled;
      }
      set
      {
        TimeWarning.Enabled = (__Null) (value ? 1 : 0);
      }
    }

    [ClientVar]
    [ServerVar]
    public static int developer
    {
      set
      {
        Global._developer = value;
      }
      get
      {
        return Global._developer;
      }
    }

    [ServerVar]
    public static void restart(ConsoleSystem.Arg args)
    {
      ServerMgr.RestartServer(args.GetString(1, string.Empty), args.GetInt(0, 300));
    }

    [ClientVar]
    [ServerVar]
    public static void quit(ConsoleSystem.Arg args)
    {
      ((ServerMgr) SingletonComponent<ServerMgr>.Instance).Shutdown();
      Application.isQuitting = (__Null) 1;
      ((Network.Server) Net.sv).Stop(nameof (quit));
      Process.GetCurrentProcess().Kill();
      Debug.Log((object) "Quitting");
      Application.Quit();
    }

    [ServerVar]
    public static void report(ConsoleSystem.Arg args)
    {
      ServerPerformance.DoReport();
    }

    [ServerVar]
    [ClientVar]
    public static void objects(ConsoleSystem.Arg args)
    {
      M0[] objectsOfType = Object.FindObjectsOfType<Object>();
      string str = "";
      Dictionary<System.Type, int> dictionary = new Dictionary<System.Type, int>();
      Dictionary<System.Type, long> source = new Dictionary<System.Type, long>();
      foreach (Object @object in (Object[]) objectsOfType)
      {
        int runtimeMemorySize = Profiler.GetRuntimeMemorySize(@object);
        if (dictionary.ContainsKey(((object) @object).GetType()))
          dictionary[((object) @object).GetType()]++;
        else
          dictionary.Add(((object) @object).GetType(), 1);
        if (source.ContainsKey(((object) @object).GetType()))
          source[((object) @object).GetType()] += (long) runtimeMemorySize;
        else
          source.Add(((object) @object).GetType(), (long) runtimeMemorySize);
      }
      foreach (KeyValuePair<System.Type, long> keyValuePair in (IEnumerable<KeyValuePair<System.Type, long>>) source.OrderByDescending<KeyValuePair<System.Type, long>, long>((Func<KeyValuePair<System.Type, long>, long>) (x => x.Value)))
        str = str + dictionary[keyValuePair.Key].ToString().PadLeft(10) + " " + NumberExtensions.FormatBytes<long>((M0) keyValuePair.Value, false).PadLeft(15) + "\t" + (object) keyValuePair.Key + "\n";
      args.ReplyWith(str);
    }

    [ServerVar]
    [ClientVar]
    public static void textures(ConsoleSystem.Arg args)
    {
      M0[] objectsOfType = Object.FindObjectsOfType<Texture>();
      string str1 = "";
      foreach (Texture texture in (Texture[]) objectsOfType)
      {
        string str2 = NumberExtensions.FormatBytes<int>((M0) Profiler.GetRuntimeMemorySize((Object) texture), false);
        str1 = str1 + ((object) texture).ToString().PadRight(30) + ((Object) texture).get_name().PadRight(30) + str2 + "\n";
      }
      args.ReplyWith(str1);
    }

    [ServerVar]
    [ClientVar]
    public static void colliders(ConsoleSystem.Arg args)
    {
      string str = ((IEnumerable<Collider>) Object.FindObjectsOfType<Collider>()).Where<Collider>((Func<Collider, bool>) (x => x.get_enabled())).Count<Collider>().ToString() + " colliders enabled, " + (object) ((IEnumerable<Collider>) Object.FindObjectsOfType<Collider>()).Where<Collider>((Func<Collider, bool>) (x => !x.get_enabled())).Count<Collider>() + " disabled";
      args.ReplyWith(str);
    }

    [ServerVar]
    [ClientVar]
    public static void error(ConsoleSystem.Arg args)
    {
      ((GameObject) null).get_transform().set_position(Vector3.get_zero());
    }

    [ServerVar]
    [ClientVar]
    public static void queue(ConsoleSystem.Arg args)
    {
      string str = "" + "stabilityCheckQueue:\t\t" + StabilityEntity.stabilityCheckQueue.Info() + "\n" + "updateSurroundingsQueue:\t" + StabilityEntity.updateSurroundingsQueue.Info() + "\n";
      args.ReplyWith(str);
    }

    [ServerUserVar]
    public static void setinfo(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      string key = args.GetString(0, (string) null);
      string val = args.GetString(1, (string) null);
      if (key == null || val == null)
        return;
      basePlayer.SetInfo(key, val);
    }

    [ServerVar]
    public static void sleep(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer) || basePlayer.IsSleeping() || (basePlayer.IsSpectating() || basePlayer.IsDead()))
        return;
      basePlayer.StartSleeping();
    }

    [ServerUserVar]
    public static void kill(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer) || basePlayer.IsSpectating() || basePlayer.IsDead())
        return;
      if (basePlayer.CanSuicide())
      {
        basePlayer.MarkSuicide();
        basePlayer.Hurt(1000f, DamageType.Suicide, (BaseEntity) basePlayer, false);
      }
      else
        basePlayer.ConsoleMessage("You can't suicide again so quickly, wait a while");
    }

    [ServerUserVar]
    public static void respawn(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      if (!basePlayer.IsDead() && !basePlayer.IsSpectating())
      {
        if (Global.developer > 0)
          Debug.LogWarning((object) (basePlayer.ToString() + " wanted to respawn but isn't dead or spectating"));
        basePlayer.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
      }
      else
        basePlayer.Respawn();
    }

    [ServerVar]
    public static void injure(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer) || basePlayer.IsDead())
        return;
      basePlayer.StartWounded();
    }

    [ServerVar]
    public static void spectate(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      if (!basePlayer.IsDead())
        basePlayer.DieInstantly();
      string strName = args.GetString(0, "");
      if (!basePlayer.IsDead())
        return;
      basePlayer.StartSpectating();
      basePlayer.UpdateSpectateTarget(strName);
    }

    [ServerUserVar]
    public static void respawn_sleepingbag(ConsoleSystem.Arg args)
    {
      BasePlayer player = args.Player();
      if (!Object.op_Implicit((Object) player) || !player.IsDead())
        return;
      uint sleepingBag = args.GetUInt(0, 0U);
      if (sleepingBag == 0U)
      {
        args.ReplyWith("Missing sleeping bag ID");
      }
      else
      {
        if (SleepingBag.SpawnPlayer(player, sleepingBag))
          return;
        args.ReplyWith("Couldn't spawn in sleeping bag!");
      }
    }

    [ServerUserVar]
    public static void respawn_sleepingbag_remove(ConsoleSystem.Arg args)
    {
      BasePlayer player = args.Player();
      if (!Object.op_Implicit((Object) player))
        return;
      uint sleepingBag = args.GetUInt(0, 0U);
      if (sleepingBag == 0U)
        args.ReplyWith("Missing sleeping bag ID");
      else
        SleepingBag.DestroyBag(player, sleepingBag);
    }

    [ServerUserVar]
    public static void status_sv(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      args.ReplyWith(basePlayer.GetDebugStatus());
    }

    [ClientVar]
    public static void status_cl(ConsoleSystem.Arg args)
    {
    }

    [ServerVar]
    public static void teleport(ConsoleSystem.Arg args)
    {
      if (args.HasArgs(2))
      {
        BasePlayer player = args.GetPlayer(0);
        if (!Object.op_Implicit((Object) player) || !player.IsAlive())
          return;
        BasePlayer playerOrSleeper = args.GetPlayerOrSleeper(1);
        if (!Object.op_Implicit((Object) playerOrSleeper) || !playerOrSleeper.IsAlive())
          return;
        player.Teleport(playerOrSleeper);
      }
      else
      {
        BasePlayer basePlayer = args.Player();
        if (!Object.op_Implicit((Object) basePlayer) || !basePlayer.IsAlive())
          return;
        BasePlayer playerOrSleeper = args.GetPlayerOrSleeper(0);
        if (!Object.op_Implicit((Object) playerOrSleeper) || !playerOrSleeper.IsAlive())
          return;
        basePlayer.Teleport(playerOrSleeper);
      }
    }

    [ServerVar]
    public static void teleport2me(ConsoleSystem.Arg args)
    {
      BasePlayer player1 = args.GetPlayer(0);
      if (!Object.op_Implicit((Object) player1) || !player1.IsAlive())
        return;
      BasePlayer player2 = args.Player();
      if (!Object.op_Implicit((Object) player2) || !player2.IsAlive())
        return;
      player1.Teleport(player2);
    }

    [ServerVar]
    public static void teleportany(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer) || !basePlayer.IsAlive())
        return;
      basePlayer.Teleport(args.GetString(0, ""), false);
    }

    [ServerVar]
    public static void teleportpos(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer) || !basePlayer.IsAlive())
        return;
      basePlayer.Teleport(args.GetVector3(0, Vector3.get_zero()));
    }

    [ClientVar]
    [ServerVar]
    public static void free(ConsoleSystem.Arg args)
    {
      Pool.clear_prefabs(args);
      Pool.clear_assets(args);
      Pool.clear_memory(args);
      GC.collect();
      GC.unload();
    }

    [ServerVar(ServerUser = true)]
    [ClientVar]
    public static void version(ConsoleSystem.Arg arg)
    {
      arg.ReplyWith(string.Format("Protocol: {0}\nBuild Date: {1}\nUnity Version: {2}\nChangeset: {3}\nBranch: {4}", (object) Protocol.printable, (object) BuildInfo.get_Current().get_BuildDate(), (object) Application.get_unityVersion(), (object) BuildInfo.get_Current().get_Scm().get_ChangeId(), (object) BuildInfo.get_Current().get_Scm().get_Branch()));
    }

    [ServerVar]
    [ClientVar]
    public static void sysinfo(ConsoleSystem.Arg arg)
    {
      arg.ReplyWith(SystemInfoGeneralText.currentInfo);
    }

    [ServerVar]
    [ClientVar]
    public static void sysuid(ConsoleSystem.Arg arg)
    {
      arg.ReplyWith(SystemInfo.get_deviceUniqueIdentifier());
    }

    [ServerVar]
    public static void breakitem(ConsoleSystem.Arg args)
    {
      BasePlayer basePlayer = args.Player();
      if (!Object.op_Implicit((Object) basePlayer))
        return;
      Item activeItem = basePlayer.GetActiveItem();
      activeItem?.LoseCondition(activeItem.condition);
    }

    [ServerVar]
    [ClientVar]
    public static void subscriptions(ConsoleSystem.Arg arg)
    {
      TextTable textTable = new TextTable();
      textTable.AddColumn("realm");
      textTable.AddColumn("group");
      BasePlayer basePlayer = arg.Player();
      if (Object.op_Implicit((Object) basePlayer))
      {
        using (List<Group>.Enumerator enumerator = ((List<Group>) ((Subscriber) basePlayer.net.subscriber).subscribed).GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            Group current = enumerator.Current;
            // ISSUE: explicit non-virtual call
            textTable.AddRow(new string[2]
            {
              "sv",
              __nonvirtual (current.ID.ToString())
            });
          }
        }
      }
      arg.ReplyWith(((object) textTable).ToString());
    }

    public Global()
    {
      base.\u002Ector();
    }
  }
}
