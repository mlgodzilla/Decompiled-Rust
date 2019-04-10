// Decompiled with JetBrains decompiler
// Type: ServerUsers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch.Extend;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class ServerUsers
{
  private static Dictionary<ulong, ServerUsers.User> users = new Dictionary<ulong, ServerUsers.User>();

  public static void Remove(ulong uid)
  {
    if (!ServerUsers.users.ContainsKey(uid))
      return;
    Interface.CallHook("IOnServerUsersRemove", (object) uid);
    ServerUsers.users.Remove(uid);
  }

  public static void Set(ulong uid, ServerUsers.UserGroup group, string username, string notes)
  {
    ServerUsers.Remove(uid);
    ServerUsers.User user = new ServerUsers.User()
    {
      steamid = uid,
      group = group,
      username = username,
      notes = notes
    };
    Interface.CallHook("IOnServerUsersSet", (object) uid, (object) group, (object) username, (object) notes);
    ServerUsers.users.Add(uid, user);
  }

  public static ServerUsers.User Get(ulong uid)
  {
    ServerUsers.User user = (ServerUsers.User) null;
    if (ServerUsers.users.TryGetValue(uid, out user))
      return user;
    return (ServerUsers.User) null;
  }

  public static bool Is(ulong uid, ServerUsers.UserGroup group)
  {
    ServerUsers.User user = ServerUsers.Get(uid);
    if (user == null)
      return false;
    return user.group == group;
  }

  public static IEnumerable<ServerUsers.User> GetAll(
    ServerUsers.UserGroup group)
  {
    return ServerUsers.users.Where<KeyValuePair<ulong, ServerUsers.User>>((Func<KeyValuePair<ulong, ServerUsers.User>, bool>) (x => x.Value.group == group)).Select<KeyValuePair<ulong, ServerUsers.User>, ServerUsers.User>((Func<KeyValuePair<ulong, ServerUsers.User>, ServerUsers.User>) (x => x.Value));
  }

  public static void Clear()
  {
    ServerUsers.users.Clear();
  }

  public static void Load()
  {
    ServerUsers.Clear();
    string serverFolder = Server.GetServerFolder("cfg");
    if (File.Exists(serverFolder + "/bans.cfg"))
    {
      string str = File.ReadAllText(serverFolder + "/bans.cfg");
      if (!string.IsNullOrEmpty(str))
      {
        Debug.Log((object) ("Running " + serverFolder + "/bans.cfg"));
        ConsoleSystem.Option server = ConsoleSystem.Option.get_Server();
        ConsoleSystem.RunFile(((ConsoleSystem.Option) ref server).Quiet(), str);
      }
    }
    if (!File.Exists(serverFolder + "/users.cfg"))
      return;
    string str1 = File.ReadAllText(serverFolder + "/users.cfg");
    if (string.IsNullOrEmpty(str1))
      return;
    Debug.Log((object) ("Running " + serverFolder + "/users.cfg"));
    ConsoleSystem.Option server1 = ConsoleSystem.Option.get_Server();
    ConsoleSystem.RunFile(((ConsoleSystem.Option) ref server1).Quiet(), str1);
  }

  public static void Save()
  {
    string serverFolder = Server.GetServerFolder("cfg");
    string contents1 = "";
    foreach (ServerUsers.User user in ServerUsers.GetAll(ServerUsers.UserGroup.Banned))
    {
      if (!(user.notes == "EAC"))
        contents1 = contents1 + "banid " + user.steamid.ToString() + " " + StringExtensions.QuoteSafe(user.username) + " " + StringExtensions.QuoteSafe(user.notes) + "\r\n";
    }
    File.WriteAllText(serverFolder + "/bans.cfg", contents1);
    string contents2 = "";
    foreach (ServerUsers.User user in ServerUsers.GetAll(ServerUsers.UserGroup.Owner))
      contents2 = contents2 + "ownerid " + user.steamid.ToString() + " " + StringExtensions.QuoteSafe(user.username) + " " + StringExtensions.QuoteSafe(user.notes) + "\r\n";
    foreach (ServerUsers.User user in ServerUsers.GetAll(ServerUsers.UserGroup.Moderator))
      contents2 = contents2 + "moderatorid " + user.steamid.ToString() + " " + StringExtensions.QuoteSafe(user.username) + " " + StringExtensions.QuoteSafe(user.notes) + "\r\n";
    File.WriteAllText(serverFolder + "/users.cfg", contents2);
  }

  public static string BanListString(bool bHeader = false)
  {
    IEnumerable<ServerUsers.User> all = ServerUsers.GetAll(ServerUsers.UserGroup.Banned);
    string str = "";
    if (bHeader)
    {
      if (all.Count<ServerUsers.User>() == 0)
        return "ID filter list: empty\n";
      str = all.Count<ServerUsers.User>() != 1 ? "ID filter list: " + all.Count<ServerUsers.User>().ToString() + " entries\n" : "ID filter list: 1 entry\n";
    }
    int num = 1;
    foreach (ServerUsers.User user in all)
    {
      str = str + num.ToString() + " " + user.steamid.ToString() + " : permanent\n";
      ++num;
    }
    return str;
  }

  public static string BanListStringEx()
  {
    IEnumerable<ServerUsers.User> all = ServerUsers.GetAll(ServerUsers.UserGroup.Banned);
    string str = "";
    int num = 1;
    foreach (ServerUsers.User user in all)
    {
      str = str + num.ToString() + " " + user.steamid.ToString() + " " + StringExtensions.QuoteSafe(user.username) + " " + StringExtensions.QuoteSafe(user.notes) + "\n";
      ++num;
    }
    return str;
  }

  public enum UserGroup
  {
    None,
    Owner,
    Moderator,
    Banned,
  }

  public class User
  {
    public ulong steamid;
    [JsonConverter(typeof (StringEnumConverter))]
    public ServerUsers.UserGroup group;
    public string username;
    public string notes;
  }
}
