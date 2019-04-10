// Decompiled with JetBrains decompiler
// Type: Auth_Steam
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch.Steamworks;
using Network;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public static class Auth_Steam
{
  internal static List<Connection> waitingList = new List<Connection>();

  public static IEnumerator Run(Connection connection)
  {
    connection.authStatus = (__Null) "";
    if (!Global.get_SteamServer().get_Auth().StartSession((byte[]) connection.token, (ulong) connection.userid))
    {
      ConnectionAuth.Reject(connection, "Steam Auth Failed");
    }
    else
    {
      Auth_Steam.waitingList.Add(connection);
      Stopwatch timeout = Stopwatch.StartNew();
      while (timeout.Elapsed.TotalSeconds < 30.0 && connection.active != null && !((string) connection.authStatus != ""))
        yield return (object) null;
      Auth_Steam.waitingList.Remove(connection);
      if (connection.active != null)
      {
        if (((string) connection.authStatus).Length == 0)
        {
          ConnectionAuth.Reject(connection, "Steam Auth Timeout");
          Global.get_SteamServer().get_Auth().EndSession((ulong) connection.userid);
        }
        else if ((string) connection.authStatus == "banned")
        {
          ConnectionAuth.Reject(connection, "Auth: " + (string) connection.authStatus);
          Global.get_SteamServer().get_Auth().EndSession((ulong) connection.userid);
        }
        else if ((string) connection.authStatus == "gamebanned")
        {
          ConnectionAuth.Reject(connection, "Steam Auth: " + (string) connection.authStatus);
          Global.get_SteamServer().get_Auth().EndSession((ulong) connection.userid);
        }
        else if ((string) connection.authStatus == "vacbanned")
        {
          ConnectionAuth.Reject(connection, "Steam Auth: " + (string) connection.authStatus);
          Global.get_SteamServer().get_Auth().EndSession((ulong) connection.userid);
        }
        else if ((string) connection.authStatus != "ok")
        {
          ConnectionAuth.Reject(connection, "Steam Auth Error: " + (string) connection.authStatus);
          Global.get_SteamServer().get_Auth().EndSession((ulong) connection.userid);
        }
        else
          Global.get_SteamServer().UpdatePlayer((ulong) connection.userid, (string) connection.username, 0);
      }
    }
  }

  public static bool ValidateConnecting(
    ulong steamid,
    ulong ownerSteamID,
    ServerAuth.Status response)
  {
    Connection connection = Auth_Steam.waitingList.Find((Predicate<Connection>) (x => x.userid == (long) steamid));
    if (connection == null)
      return false;
    connection.ownerid = (__Null) (long) ownerSteamID;
    if (ServerUsers.Is(ownerSteamID, ServerUsers.UserGroup.Banned) || ServerUsers.Is(steamid, ServerUsers.UserGroup.Banned))
    {
      connection.authStatus = (__Null) "banned";
      return true;
    }
    if (response == null)
    {
      connection.authStatus = (__Null) "ok";
      return true;
    }
    if (response == 3)
    {
      connection.authStatus = (__Null) "vacbanned";
      return true;
    }
    if (response == 9)
    {
      connection.authStatus = (__Null) "gamebanned";
      return true;
    }
    if (response == 5)
    {
      connection.authStatus = (__Null) "ok";
      return true;
    }
    connection.authStatus = (__Null) response.ToString();
    return true;
  }
}
