// Decompiled with JetBrains decompiler
// Type: ConVar.Server
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using EasyAntiCheat.Server.Scout;
using Network;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("server")]
  public class Server : ConsoleSystem
  {
    [ServerVar]
    public static string ip = "";
    [ServerVar]
    public static int port = 28015;
    [ServerVar]
    public static int queryport = 0;
    [ServerVar]
    public static int maxplayers = 500;
    [ServerVar]
    public static string hostname = "My Untitled Rust Server";
    [ServerVar]
    public static string identity = "my_server_identity";
    [ServerVar]
    public static string level = "Procedural Map";
    [ServerVar]
    public static string levelurl = "";
    [ServerVar]
    public static int seed = 0;
    [ServerVar]
    public static int salt = 0;
    [ServerVar]
    public static int worldsize = 0;
    [ServerVar]
    public static int saveinterval = 600;
    [ServerVar]
    public static bool secure = true;
    [ServerVar]
    public static int encryption = 2;
    [ServerVar]
    public static int tickrate = 10;
    [ServerVar]
    public static int entityrate = 16;
    [ServerVar]
    public static float schematime = 1800f;
    [ServerVar]
    public static float cycletime = 500f;
    [ServerVar]
    public static bool official = false;
    [ServerVar]
    public static bool stats = false;
    [ServerVar]
    public static bool globalchat = true;
    [ServerVar]
    public static bool stability = true;
    [ServerVar]
    public static bool radiation = true;
    [ServerVar]
    public static float itemdespawn = 300f;
    [ServerVar]
    public static float corpsedespawn = 300f;
    [ServerVar]
    public static float debrisdespawn = 30f;
    [ServerVar]
    public static bool pve = false;
    [ServerVar]
    public static string description = "No server description has been provided.";
    [ServerVar]
    public static string headerimage = "";
    [ServerVar]
    public static string url = "";
    [ServerVar]
    public static string branch = "";
    [ServerVar]
    public static int queriesPerSecond = 2000;
    [ServerVar]
    public static int ipQueriesPerMin = 30;
    [ServerVar(Saved = true)]
    public static float meleedamage = 1f;
    [ServerVar(Saved = true)]
    public static float arrowdamage = 1f;
    [ServerVar(Saved = true)]
    public static float bulletdamage = 1f;
    [ServerVar(Saved = true)]
    public static float bleedingdamage = 1f;
    [ServerVar(Saved = true)]
    public static float meleearmor = 1f;
    [ServerVar(Saved = true)]
    public static float arrowarmor = 1f;
    [ServerVar(Saved = true)]
    public static float bulletarmor = 1f;
    [ServerVar(Saved = true)]
    public static float bleedingarmor = 1f;
    [ServerVar]
    public static int updatebatch = 512;
    [ServerVar]
    public static int updatebatchspawn = 1024;
    [ServerVar]
    public static int entitybatchsize = 100;
    [ServerVar]
    public static float entitybatchtime = 1f;
    [ServerVar]
    public static float planttick = 60f;
    [ServerVar]
    public static float planttickscale = 1f;
    [ServerVar]
    public static float metabolismtick = 1f;
    [ServerVar(Saved = true)]
    public static bool woundingenabled = true;
    [ServerVar(Saved = true)]
    public static bool playerserverfall = true;
    [ServerVar]
    public static bool plantlightdetection = true;
    [ServerVar]
    public static float respawnresetrange = 50f;
    [ServerVar]
    public static int maxunack = 4;
    [ServerVar]
    public static bool netcache = true;
    [ServerVar]
    public static bool corpses = true;
    [ServerVar]
    public static bool events = true;
    [ServerVar]
    public static bool dropitems = true;
    [ServerVar]
    public static int netcachesize = 0;
    [ServerVar]
    public static int savecachesize = 0;
    [ServerVar]
    public static int combatlogsize = 30;
    [ServerVar]
    public static int combatlogdelay = 10;
    [ServerVar]
    public static int authtimeout = 60;
    [ServerVar]
    public static int playertimeout = 60;
    [ServerVar]
    public static int idlekick = 30;
    [ServerVar]
    public static int idlekickmode = 1;
    [ServerVar]
    public static int idlekickadmins = 0;
    [ServerVar(Saved = true)]
    public static bool showHolsteredItems = true;
    [ServerVar]
    public static int maxrpcspersecond = 200;
    [ServerVar]
    public static int maxcommandspersecond = 100;
    [ServerVar]
    public static int maxcommandpacketsize = 1000000;
    [ServerVar]
    public static int maxtickspersecond = 300;

    public static float TickDelta()
    {
      return 1f / (float) ConVar.Server.tickrate;
    }

    public static float TickTime(uint tick)
    {
      return ConVar.Server.TickDelta() * (float) tick;
    }

    [ServerVar(Help = "Show holstered items on player bodies")]
    public static void setshowholstereditems(ConsoleSystem.Arg arg)
    {
      ConVar.Server.showHolsteredItems = arg.GetBool(0, ConVar.Server.showHolsteredItems);
      foreach (BasePlayer activePlayer in BasePlayer.activePlayerList)
        activePlayer.inventory.UpdatedVisibleHolsteredItems();
      foreach (BasePlayer sleepingPlayer in BasePlayer.sleepingPlayerList)
        sleepingPlayer.inventory.UpdatedVisibleHolsteredItems();
    }

    [ServerVar]
    public static float maxreceivetime
    {
      get
      {
        return (float) Facepunch.Network.Raknet.Server.MaxReceiveTime;
      }
      set
      {
        Facepunch.Network.Raknet.Server.MaxReceiveTime = (__Null) (double) Mathf.Clamp(value, 1f, 1000f);
      }
    }

    [ServerVar]
    public static int maxpacketspersecond
    {
      get
      {
        return (int) Facepunch.Network.Raknet.Server.MaxPacketsPerSecond;
      }
      set
      {
        Facepunch.Network.Raknet.Server.MaxPacketsPerSecond = (__Null) (long) Mathf.Clamp(value, 1, 1000000);
      }
    }

    [ServerVar]
    public static int maxpacketsize
    {
      get
      {
        return (int) Facepunch.Network.Raknet.Server.MaxPacketSize;
      }
      set
      {
        Facepunch.Network.Raknet.Server.MaxPacketSize = (__Null) Mathf.Clamp(value, 1, 1000000000);
      }
    }

    [ServerVar(Help = "Starts a server")]
    public static void start(ConsoleSystem.Arg arg)
    {
      if (((Network.Server) Net.sv).IsConnected())
      {
        arg.ReplyWith("There is already a server running!");
      }
      else
      {
        string strName = arg.GetString(0, ConVar.Server.level);
        if (!LevelManager.IsValid(strName))
          arg.ReplyWith("Level '" + strName + "' isn't valid!");
        else if (Object.op_Implicit((Object) Object.FindObjectOfType<ServerMgr>()))
        {
          arg.ReplyWith("There is already a server running!");
        }
        else
        {
          Object.DontDestroyOnLoad((Object) GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab", true));
          LevelManager.LoadLevel(strName, true);
        }
      }
    }

    [ServerVar(Help = "Stops a server")]
    public static void stop(ConsoleSystem.Arg arg)
    {
      if (!((Network.Server) Net.sv).IsConnected())
        arg.ReplyWith("There isn't a server running!");
      else
        ((Network.Server) Net.sv).Stop(arg.GetString(0, "Stopping Server"));
    }

    public static string rootFolder
    {
      get
      {
        return "server/" + ConVar.Server.identity;
      }
    }

    public static string backupFolder
    {
      get
      {
        return "backup/0/" + ConVar.Server.identity;
      }
    }

    public static string backupFolder1
    {
      get
      {
        return "backup/1/" + ConVar.Server.identity;
      }
    }

    public static string backupFolder2
    {
      get
      {
        return "backup/2/" + ConVar.Server.identity;
      }
    }

    public static string backupFolder3
    {
      get
      {
        return "backup/3/" + ConVar.Server.identity;
      }
    }

    [ServerVar(Help = "Backup server folder")]
    public static void backup()
    {
      DirectoryEx.Backup(ConVar.Server.backupFolder, ConVar.Server.backupFolder1, ConVar.Server.backupFolder2, ConVar.Server.backupFolder3);
      DirectoryEx.CopyAll(ConVar.Server.rootFolder, ConVar.Server.backupFolder);
    }

    public static string GetServerFolder(string folder)
    {
      string path = ConVar.Server.rootFolder + "/" + folder;
      if (Directory.Exists(path))
        return path;
      Directory.CreateDirectory(path);
      return path;
    }

    [ServerVar(Help = "Writes config files")]
    public static void writecfg(ConsoleSystem.Arg arg)
    {
      File.WriteAllText(ConVar.Server.GetServerFolder("cfg") + "/serverauto.cfg", ConsoleSystem.SaveToConfigString(true));
      ServerUsers.Save();
      arg.ReplyWith("Config Saved");
    }

    [ServerVar]
    public static void fps(ConsoleSystem.Arg arg)
    {
      arg.ReplyWith(Performance.report.frameRate.ToString() + " FPS");
    }

    [ServerVar(Help = "Force save the current game")]
    public static void save(ConsoleSystem.Arg arg)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      foreach (BaseNetworkable save in BaseEntity.saveList)
        save.InvalidateNetworkCache();
      Debug.Log((object) ("Invalidate Network Cache took " + stopwatch.Elapsed.TotalSeconds.ToString("0.00") + " seconds"));
      SaveRestore.Save(true);
    }

    [ServerVar]
    public static string readcfg(ConsoleSystem.Arg arg)
    {
      string serverFolder = ConVar.Server.GetServerFolder("cfg");
      ConsoleSystem.Option server;
      if (File.Exists(serverFolder + "/serverauto.cfg"))
      {
        string str = File.ReadAllText(serverFolder + "/serverauto.cfg");
        server = ConsoleSystem.Option.get_Server();
        ConsoleSystem.RunFile(((ConsoleSystem.Option) ref server).Quiet(), str);
      }
      if (File.Exists(serverFolder + "/server.cfg"))
      {
        string str = File.ReadAllText(serverFolder + "/server.cfg");
        server = ConsoleSystem.Option.get_Server();
        ConsoleSystem.RunFile(((ConsoleSystem.Option) ref server).Quiet(), str);
      }
      return "Server Config Loaded";
    }

    [ServerVar]
    public static bool compression
    {
      get
      {
        if (Net.sv == null)
          return false;
        return (bool) ((Network.Server) Net.sv).compressionEnabled;
      }
      set
      {
        ((Network.Server) Net.sv).compressionEnabled = (__Null) (value ? 1 : 0);
      }
    }

    [ServerVar]
    public static bool netlog
    {
      get
      {
        if (Net.sv == null)
          return false;
        return (bool) ((Network.Server) Net.sv).logging;
      }
      set
      {
        ((Network.Server) Net.sv).logging = (__Null) (value ? 1 : 0);
      }
    }

    [ServerUserVar]
    public static void cheatreport(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (Object.op_Equality((Object) basePlayer, (Object) null))
        return;
      ulong uint64 = arg.GetUInt64(0, 0UL);
      string str = arg.GetString(1, "");
      Debug.LogWarning((object) (basePlayer.ToString() + " reported " + (object) uint64 + ": " + StringEx.ToPrintable(str, 140)));
      // ISSUE: explicit non-virtual call
      EACServer.eacScout.SendPlayerReport(uint64.ToString(), __nonvirtual (basePlayer.net.get_connection().userid.ToString()), (PlayerReportCategory) 1, str);
    }

    [ServerUserVar(Help = "Get the player combat log")]
    public static string combatlog(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (arg.HasArgs(1) && Object.op_Inequality((Object) basePlayer, (Object) null) && basePlayer.IsAdmin)
        basePlayer = arg.GetPlayerOrSleeper(0);
      if (Object.op_Equality((Object) basePlayer, (Object) null))
        return "invalid player";
      return basePlayer.stats.combat.Get(ConVar.Server.combatlogsize);
    }

    [ServerVar(Help = "Print the current player position.")]
    public static string printpos(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (arg.HasArgs(1))
        basePlayer = arg.GetPlayerOrSleeper(0);
      if (!Object.op_Equality((Object) basePlayer, (Object) null))
        return ((Component) basePlayer).get_transform().get_position().ToString();
      return "invalid player";
    }

    [ServerVar(Help = "Print the current player rotation.")]
    public static string printrot(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (arg.HasArgs(1))
        basePlayer = arg.GetPlayerOrSleeper(0);
      if (Object.op_Equality((Object) basePlayer, (Object) null))
        return "invalid player";
      Quaternion rotation = ((Component) basePlayer).get_transform().get_rotation();
      return ((Quaternion) ref rotation).get_eulerAngles().ToString();
    }

    [ServerVar(Help = "Print the current player eyes.")]
    public static string printeyes(ConsoleSystem.Arg arg)
    {
      BasePlayer basePlayer = arg.Player();
      if (arg.HasArgs(1))
        basePlayer = arg.GetPlayerOrSleeper(0);
      if (Object.op_Equality((Object) basePlayer, (Object) null))
        return "invalid player";
      Quaternion rotation = basePlayer.eyes.rotation;
      return ((Quaternion) ref rotation).get_eulerAngles().ToString();
    }

    [ServerVar(Help = "This sends a snapshot of all the entities in the client's pvs. This is mostly redundant, but we request this when the client starts recording a demo.. so they get all the information.", ServerAdmin = false)]
    public static void snapshot(ConsoleSystem.Arg arg)
    {
      if (Object.op_Equality((Object) arg.Player(), (Object) null))
        return;
      Debug.Log((object) ("Sending full snapshot to " + (object) arg.Player()));
      arg.Player().SendNetworkUpdateImmediate(false);
      arg.Player().SendGlobalSnapshot();
      arg.Player().SendFullSnapshot();
    }

    [ServerVar(Help = "Send network update for all players")]
    public static void sendnetworkupdate(ConsoleSystem.Arg arg)
    {
      foreach (BaseNetworkable activePlayer in BasePlayer.activePlayerList)
        activePlayer.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
    }

    public Server()
    {
      base.\u002Ector();
    }
  }
}
