// Decompiled with JetBrains decompiler
// Type: ConVar.Admin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Facepunch.Extend;
using Network;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("global")]
  public class Admin : ConsoleSystem
  {
    [ServerVar(Help = "Print out currently connected clients")]
    public static void status(ConsoleSystem.Arg arg)
    {
      string str1 = arg.GetString(0, "");
      string str2 = string.Empty;
      if (str1.Length == 0)
        str2 = str2 + "hostname: " + Server.hostname + "\n" + "version : " + 2161.ToString() + " secure (secure mode enabled, connected to Steam3)\n" + "map     : " + Server.level + "\n" + "players : " + (object) BasePlayer.activePlayerList.Count<BasePlayer>() + " (" + (object) Server.maxplayers + " max) (" + (object) ((ServerMgr) SingletonComponent<ServerMgr>.Instance).connectionQueue.Queued + " queued) (" + (object) ((ServerMgr) SingletonComponent<ServerMgr>.Instance).connectionQueue.Joining + " joining)\n\n";
      TextTable textTable = new TextTable();
      textTable.AddColumn("id");
      textTable.AddColumn("name");
      textTable.AddColumn("ping");
      textTable.AddColumn("connected");
      textTable.AddColumn("addr");
      textTable.AddColumn("owner");
      textTable.AddColumn("violation");
      textTable.AddColumn("kicks");
      foreach (BasePlayer activePlayer in BasePlayer.activePlayerList)
      {
        try
        {
          if (activePlayer.IsValid())
          {
            string userIdString = activePlayer.UserIDString;
            if (activePlayer.net.get_connection() == null)
            {
              textTable.AddRow(new string[2]
              {
                userIdString,
                "NO CONNECTION"
              });
            }
            else
            {
              // ISSUE: explicit non-virtual call
              string str3 = __nonvirtual (activePlayer.net.get_connection().ownerid.ToString());
              string str4 = StringExtensions.QuoteSafe(activePlayer.GetSubName(32));
              string str5 = ((Server) Net.sv).GetAveragePing(activePlayer.net.get_connection()).ToString();
              string str6 = (string) activePlayer.net.get_connection().ipaddress;
              string str7 = activePlayer.violationLevel.ToString("0.0");
              string str8 = activePlayer.GetAntiHackKicks().ToString();
              if (!arg.get_IsAdmin() && !arg.get_IsRcon())
                str6 = "xx.xxx.xx.xxx";
              string str9 = activePlayer.net.get_connection().GetSecondsConnected().ToString() + "s";
              if (str1.Length > 0 && !StringEx.Contains(str4, str1, CompareOptions.IgnoreCase) && (!userIdString.Contains(str1) && !str3.Contains(str1)))
              {
                if (!str6.Contains(str1))
                  continue;
              }
              textTable.AddRow(new string[8]
              {
                userIdString,
                str4,
                str5,
                str9,
                str6,
                str3 == userIdString ? string.Empty : str3,
                str7,
                str8
              });
            }
          }
        }
        catch (Exception ex)
        {
          textTable.AddRow(new string[2]
          {
            activePlayer.UserIDString,
            StringExtensions.QuoteSafe(ex.Message)
          });
        }
      }
      arg.ReplyWith(str2 + ((object) textTable).ToString());
    }

    [ServerVar(Help = "Print out stats of currently connected clients")]
    public static void stats(ConsoleSystem.Arg arg)
    {
      TextTable table = new TextTable();
      table.AddColumn("id");
      table.AddColumn("name");
      table.AddColumn("time");
      table.AddColumn("kills");
      table.AddColumn("deaths");
      table.AddColumn("suicides");
      table.AddColumn("player");
      table.AddColumn("building");
      table.AddColumn("entity");
      Action<ulong, string> action = (Action<ulong, string>) ((id, name) =>
      {
        ServerStatistics.Storage storage = ServerStatistics.Get(id);
        string shortString = TimeSpan.FromSeconds((double) storage.Get("time")).ToShortString();
        string str1 = storage.Get("kill_player").ToString();
        string str2 = (storage.Get("deaths") - storage.Get("death_suicide")).ToString();
        string str3 = storage.Get("death_suicide").ToString();
        string str4 = storage.Get("hit_player_direct_los").ToString();
        string str5 = storage.Get("hit_player_indirect_los").ToString();
        string str6 = storage.Get("hit_building_direct_los").ToString();
        string str7 = storage.Get("hit_building_indirect_los").ToString();
        string str8 = storage.Get("hit_entity_direct_los").ToString();
        string str9 = storage.Get("hit_entity_indirect_los").ToString();
        table.AddRow(new string[9]
        {
          id.ToString(),
          name,
          shortString,
          str1,
          str2,
          str3,
          str4 + " / " + str5,
          str6 + " / " + str7,
          str8 + " / " + str9
        });
      });
      ulong filterID = arg.GetUInt64(0, 0UL);
      if (filterID == 0UL)
      {
        string str1 = arg.GetString(0, "");
        foreach (BasePlayer activePlayer in BasePlayer.activePlayerList)
        {
          try
          {
            if (activePlayer.IsValid())
            {
              string str2 = StringExtensions.QuoteSafe(activePlayer.GetSubName(32));
              if (str1.Length > 0)
              {
                if (!StringEx.Contains(str2, str1, CompareOptions.IgnoreCase))
                  continue;
              }
              action(activePlayer.userID, str2);
            }
          }
          catch (Exception ex)
          {
            table.AddRow(new string[2]
            {
              activePlayer.UserIDString,
              StringExtensions.QuoteSafe(ex.Message)
            });
          }
        }
      }
      else
      {
        string str = "N/A";
        BasePlayer basePlayer = BasePlayer.activePlayerList.Find((Predicate<BasePlayer>) (p => (long) p.userID == (long) filterID));
        if (Object.op_Implicit((Object) basePlayer))
          str = StringExtensions.QuoteSafe(basePlayer.GetSubName(32));
        action(filterID, str);
      }
      arg.ReplyWith(((object) table).ToString());
    }

    [ServerVar]
    public static void kick(ConsoleSystem.Arg arg)
    {
      BasePlayer player = arg.GetPlayer(0);
      if (!Object.op_Implicit((Object) player) || player.net == null || player.net.get_connection() == null)
      {
        arg.ReplyWith("Player not found");
      }
      else
      {
        string str = arg.GetString(1, "no reason given");
        arg.ReplyWith("Kicked: " + player.displayName);
        Chat.Broadcast("Kicking " + player.displayName + " (" + str + ")", "SERVER", "#eee", 0UL);
        player.Kick("Kicked: " + arg.GetString(1, "No Reason Given"));
      }
    }

    [ServerVar]
    public static void kickall(ConsoleSystem.Arg arg)
    {
      foreach (BasePlayer basePlayer in BasePlayer.activePlayerList.ToArray())
        basePlayer.Kick("Kicked: " + arg.GetString(1, "No Reason Given"));
    }

    [ServerVar]
    public static void ban(ConsoleSystem.Arg arg)
    {
      BasePlayer player = arg.GetPlayer(0);
      if (!Object.op_Implicit((Object) player) || player.net == null || player.net.get_connection() == null)
      {
        arg.ReplyWith("Player not found");
      }
      else
      {
        ServerUsers.User user = ServerUsers.Get(player.userID);
        if (user != null && user.group == ServerUsers.UserGroup.Banned)
        {
          arg.ReplyWith("User " + (object) player.userID + " is already banned");
        }
        else
        {
          string notes = arg.GetString(1, "No Reason Given");
          ServerUsers.Set(player.userID, ServerUsers.UserGroup.Banned, player.displayName, notes);
          string str = "";
          if (player.IsConnected && player.net.get_connection().ownerid != player.net.get_connection().userid)
          {
            str = str + " and also banned ownerid " + (object) (ulong) player.net.get_connection().ownerid;
            ServerUsers.Set((ulong) player.net.get_connection().ownerid, ServerUsers.UserGroup.Banned, player.displayName, arg.GetString(1, "Family share owner of " + (object) (ulong) player.net.get_connection().userid));
          }
          ServerUsers.Save();
          arg.ReplyWith("Kickbanned User: " + (object) player.userID + " - " + player.displayName + str);
          Chat.Broadcast("Kickbanning " + player.displayName + " (" + notes + ")", "SERVER", "#eee", 0UL);
          ((Server) Net.sv).Kick(player.net.get_connection(), "Banned: " + notes);
        }
      }
    }

    [ServerVar]
    public static void moderatorid(ConsoleSystem.Arg arg)
    {
      ulong uint64 = arg.GetUInt64(0, 0UL);
      string username = arg.GetString(1, "unnamed");
      string notes = arg.GetString(2, "no reason");
      if (uint64 < 70000000000000000UL)
      {
        arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + (object) uint64);
      }
      else
      {
        ServerUsers.User user = ServerUsers.Get(uint64);
        if (user != null && user.group == ServerUsers.UserGroup.Moderator)
        {
          arg.ReplyWith("User " + (object) uint64 + " is already a Moderator");
        }
        else
        {
          ServerUsers.Set(uint64, ServerUsers.UserGroup.Moderator, username, notes);
          arg.ReplyWith("Added moderator " + username + ", steamid " + (object) uint64);
        }
      }
    }

    [ServerVar]
    public static void ownerid(ConsoleSystem.Arg arg)
    {
      ulong uint64 = arg.GetUInt64(0, 0UL);
      string username = arg.GetString(1, "unnamed");
      string notes = arg.GetString(2, "no reason");
      if (uint64 < 70000000000000000UL)
      {
        arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + (object) uint64);
      }
      else
      {
        ServerUsers.User user = ServerUsers.Get(uint64);
        if (user != null && user.group == ServerUsers.UserGroup.Owner)
        {
          arg.ReplyWith("User " + (object) uint64 + " is already an Owner");
        }
        else
        {
          ServerUsers.Set(uint64, ServerUsers.UserGroup.Owner, username, notes);
          arg.ReplyWith("Added owner " + username + ", steamid " + (object) uint64);
        }
      }
    }

    [ServerVar]
    public static void removemoderator(ConsoleSystem.Arg arg)
    {
      ulong uint64 = arg.GetUInt64(0, 0UL);
      if (uint64 < 70000000000000000UL)
      {
        arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + (object) uint64);
      }
      else
      {
        ServerUsers.User user = ServerUsers.Get(uint64);
        if (user == null || user.group != ServerUsers.UserGroup.Moderator)
        {
          arg.ReplyWith("User " + (object) uint64 + " isn't a moderator");
        }
        else
        {
          ServerUsers.Remove(uint64);
          arg.ReplyWith("Removed Moderator: " + (object) uint64);
        }
      }
    }

    [ServerVar]
    public static void removeowner(ConsoleSystem.Arg arg)
    {
      ulong uint64 = arg.GetUInt64(0, 0UL);
      if (uint64 < 70000000000000000UL)
      {
        arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + (object) uint64);
      }
      else
      {
        ServerUsers.User user = ServerUsers.Get(uint64);
        if (user == null || user.group != ServerUsers.UserGroup.Owner)
        {
          arg.ReplyWith("User " + (object) uint64 + " isn't an owner");
        }
        else
        {
          ServerUsers.Remove(uint64);
          arg.ReplyWith("Removed Owner: " + (object) uint64);
        }
      }
    }

    [ServerVar]
    public static void banid(ConsoleSystem.Arg arg)
    {
      ulong uint64 = arg.GetUInt64(0, 0UL);
      string username = arg.GetString(1, "unnamed");
      string notes = arg.GetString(2, "no reason");
      if (uint64 < 70000000000000000UL)
      {
        arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + (object) uint64);
      }
      else
      {
        ServerUsers.User user = ServerUsers.Get(uint64);
        if (user != null && user.group == ServerUsers.UserGroup.Banned)
        {
          arg.ReplyWith("User " + (object) uint64 + " is already banned");
        }
        else
        {
          ServerUsers.Set(uint64, ServerUsers.UserGroup.Banned, username, notes);
          arg.ReplyWith("Banned User: " + (object) uint64 + " - " + username);
        }
      }
    }

    [ServerVar]
    public static void unban(ConsoleSystem.Arg arg)
    {
      ulong uint64 = arg.GetUInt64(0, 0UL);
      if (uint64 < 70000000000000000UL)
      {
        arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + (object) uint64);
      }
      else
      {
        ServerUsers.User user = ServerUsers.Get(uint64);
        if (user == null || user.group != ServerUsers.UserGroup.Banned)
        {
          arg.ReplyWith("User " + (object) uint64 + " isn't banned");
        }
        else
        {
          ServerUsers.Remove(uint64);
          arg.ReplyWith("Unbanned User: " + (object) uint64);
        }
      }
    }

    [ServerVar]
    public static void skipqueue(ConsoleSystem.Arg arg)
    {
      ulong uint64 = arg.GetUInt64(0, 0UL);
      if (uint64 < 70000000000000000UL)
        arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + (object) uint64);
      else
        ((ServerMgr) SingletonComponent<ServerMgr>.Instance).connectionQueue.SkipQueue(uint64);
    }

    [ServerVar(Help = "Print out currently connected clients etc")]
    public static void players(ConsoleSystem.Arg arg)
    {
      TextTable textTable = new TextTable();
      textTable.AddColumn("id");
      textTable.AddColumn("name");
      textTable.AddColumn("ping");
      textTable.AddColumn("snap");
      textTable.AddColumn("updt");
      textTable.AddColumn("posi");
      textTable.AddColumn("dist");
      foreach (BasePlayer activePlayer in BasePlayer.activePlayerList)
      {
        string userIdString = activePlayer.UserIDString;
        string str1 = activePlayer.displayName.ToString();
        if (str1.Length >= 14)
          str1 = str1.Substring(0, 14) + "..";
        string str2 = str1;
        int num = ((Server) Net.sv).GetAveragePing(activePlayer.net.get_connection());
        string str3 = num.ToString();
        num = activePlayer.GetQueuedUpdateCount(BasePlayer.NetworkQueue.Update);
        string str4 = num.ToString();
        num = activePlayer.GetQueuedUpdateCount(BasePlayer.NetworkQueue.UpdateDistance);
        string str5 = num.ToString();
        textTable.AddRow(new string[7]
        {
          userIdString,
          str2,
          str3,
          string.Empty,
          str4,
          string.Empty,
          str5
        });
      }
      arg.ReplyWith(((object) textTable).ToString());
    }

    [ServerVar(Help = "Sends a message in chat")]
    public static void say(ConsoleSystem.Arg arg)
    {
      Chat.Broadcast((string) arg.FullString, "SERVER", "#eee", 0UL);
    }

    [ServerVar(Help = "Show user info for players on server.")]
    public static void users(ConsoleSystem.Arg arg)
    {
      string str1 = "<slot:userid:\"name\">\n";
      int num = 0;
      foreach (BasePlayer activePlayer in BasePlayer.activePlayerList)
      {
        str1 = str1 + (object) activePlayer.userID + ":\"" + activePlayer.displayName + "\"\n";
        ++num;
      }
      string str2 = str1 + num.ToString() + "users\n";
      arg.ReplyWith(str2);
    }

    [ServerVar(Help = "List of banned users (sourceds compat)")]
    public static void banlist(ConsoleSystem.Arg arg)
    {
      arg.ReplyWith(ServerUsers.BanListString(false));
    }

    [ServerVar(Help = "List of banned users - shows reasons and usernames")]
    public static void banlistex(ConsoleSystem.Arg arg)
    {
      arg.ReplyWith(ServerUsers.BanListStringEx());
    }

    [ServerVar(Help = "List of banned users, by ID (sourceds compat)")]
    public static void listid(ConsoleSystem.Arg arg)
    {
      arg.ReplyWith(ServerUsers.BanListString(true));
    }

    [ServerVar]
    public static void mutevoice(ConsoleSystem.Arg arg)
    {
      BasePlayer player = arg.GetPlayer(0);
      if (!Object.op_Implicit((Object) player) || player.net == null || player.net.get_connection() == null)
        arg.ReplyWith("Player not found");
      else
        player.SetPlayerFlag(BasePlayer.PlayerFlags.VoiceMuted, true);
    }

    [ServerVar]
    public static void unmutevoice(ConsoleSystem.Arg arg)
    {
      BasePlayer player = arg.GetPlayer(0);
      if (!Object.op_Implicit((Object) player) || player.net == null || player.net.get_connection() == null)
        arg.ReplyWith("Player not found");
      else
        player.SetPlayerFlag(BasePlayer.PlayerFlags.VoiceMuted, false);
    }

    [ServerVar]
    public static void mutechat(ConsoleSystem.Arg arg)
    {
      BasePlayer player = arg.GetPlayer(0);
      if (!Object.op_Implicit((Object) player) || player.net == null || player.net.get_connection() == null)
        arg.ReplyWith("Player not found");
      else
        player.SetPlayerFlag(BasePlayer.PlayerFlags.ChatMute, true);
    }

    [ServerVar]
    public static void unmutechat(ConsoleSystem.Arg arg)
    {
      BasePlayer player = arg.GetPlayer(0);
      if (!Object.op_Implicit((Object) player) || player.net == null || player.net.get_connection() == null)
        arg.ReplyWith("Player not found");
      else
        player.SetPlayerFlag(BasePlayer.PlayerFlags.ChatMute, false);
    }

    [ServerVar]
    public static void clientperf(ConsoleSystem.Arg arg)
    {
      foreach (BasePlayer activePlayer in BasePlayer.activePlayerList)
        activePlayer.ClientRPCPlayer((Connection) null, activePlayer, "GetPerformanceReport");
    }

    [ServerVar]
    public static void entid(ConsoleSystem.Arg arg)
    {
      BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(arg.GetUInt(1, 0U)) as BaseEntity;
      if (Object.op_Equality((Object) baseEntity, (Object) null) || baseEntity is BasePlayer)
        return;
      string str = arg.GetString(0, "");
      if (Object.op_Inequality((Object) arg.Player(), (Object) null))
        Debug.Log((object) ("[ENTCMD] " + arg.Player().displayName + "/" + (object) arg.Player().userID + " used *" + str + "* on ent: " + ((Object) baseEntity).get_name()));
      if (!(str == "kill"))
      {
        if (!(str == "lock"))
        {
          if (!(str == "unlock"))
          {
            if (!(str == "debug"))
            {
              if (!(str == "undebug"))
              {
                if (str == "who")
                  arg.ReplyWith("Owner ID: " + (object) baseEntity.OwnerID);
                else
                  arg.ReplyWith("Unknown command");
              }
              else
                baseEntity.SetFlag(BaseEntity.Flags.Debugging, false, false, true);
            }
            else
              baseEntity.SetFlag(BaseEntity.Flags.Debugging, true, false, true);
          }
          else
            baseEntity.SetFlag(BaseEntity.Flags.Locked, false, false, true);
        }
        else
          baseEntity.SetFlag(BaseEntity.Flags.Locked, true, false, true);
      }
      else
        baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);
    }

    [ServerVar(Help = "Get a list of players")]
    public static Admin.PlayerInfo[] playerlist()
    {
      return BasePlayer.activePlayerList.Select<BasePlayer, Admin.PlayerInfo>((Func<BasePlayer, Admin.PlayerInfo>) (x => new Admin.PlayerInfo()
      {
        SteamID = x.userID.ToString(),
        OwnerSteamID = x.OwnerID.ToString(),
        DisplayName = x.displayName,
        Ping = ((Server) Net.sv).GetAveragePing(x.net.get_connection()),
        Address = (string) x.net.get_connection().ipaddress,
        ConnectedSeconds = (int) x.net.get_connection().GetSecondsConnected(),
        VoiationLevel = x.violationLevel,
        Health = x.Health()
      })).ToArray<Admin.PlayerInfo>();
    }

    [ServerVar(Help = "List of banned users")]
    public static ServerUsers.User[] Bans()
    {
      return ServerUsers.GetAll(ServerUsers.UserGroup.Banned).ToArray<ServerUsers.User>();
    }

    [ServerVar(Help = "Get a list of information about the server")]
    public static Admin.ServerInfoOutput ServerInfo()
    {
      return new Admin.ServerInfoOutput()
      {
        Hostname = Server.hostname,
        MaxPlayers = Server.maxplayers,
        Players = BasePlayer.activePlayerList.Count,
        Queued = ((ServerMgr) SingletonComponent<ServerMgr>.Instance).connectionQueue.Queued,
        Joining = ((ServerMgr) SingletonComponent<ServerMgr>.Instance).connectionQueue.Joining,
        EntityCount = BaseNetworkable.serverEntities.Count,
        GameTime = Object.op_Inequality((Object) TOD_Sky.get_Instance(), (Object) null) ? ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).get_DateTime().ToString() : DateTime.UtcNow.ToString(),
        Uptime = (int) Time.get_realtimeSinceStartup(),
        Map = Server.level,
        Framerate = (float) Performance.report.frameRate,
        Memory = (int) Performance.report.memoryAllocations,
        Collections = (int) Performance.report.memoryCollections,
        NetworkIn = Net.sv == null ? 0 : (int) ((NetworkPeer) Net.sv).GetStat((Connection) null, (NetworkPeer.StatTypeLong) 3),
        NetworkOut = Net.sv == null ? 0 : (int) ((NetworkPeer) Net.sv).GetStat((Connection) null, (NetworkPeer.StatTypeLong) 1),
        Restarting = ((ServerMgr) SingletonComponent<ServerMgr>.Instance).Restarting,
        SaveCreatedTime = SaveRestore.SaveCreatedTime.ToString()
      };
    }

    [ServerVar(Help = "Get information about this build")]
    public static BuildInfo BuildInfo()
    {
      return BuildInfo.get_Current();
    }

    public Admin()
    {
      base.\u002Ector();
    }

    public struct PlayerInfo
    {
      public string SteamID;
      public string OwnerSteamID;
      public string DisplayName;
      public int Ping;
      public string Address;
      public int ConnectedSeconds;
      public float VoiationLevel;
      public float CurrentLevel;
      public float UnspentXp;
      public float Health;
    }

    public struct ServerInfoOutput
    {
      public string Hostname;
      public int MaxPlayers;
      public int Players;
      public int Queued;
      public int Joining;
      public int EntityCount;
      public string GameTime;
      public int Uptime;
      public string Map;
      public float Framerate;
      public int Memory;
      public int Collections;
      public int NetworkIn;
      public int NetworkOut;
      public bool Restarting;
      public string SaveCreatedTime;
    }
  }
}
