// Decompiled with JetBrains decompiler
// Type: ConnectionAuth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Oxide.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectionAuth : MonoBehaviour
{
  [NonSerialized]
  public static List<Connection> m_AuthConnection = new List<Connection>();

  public bool IsConnected(ulong iSteamID)
  {
    if (Object.op_Implicit((Object) BasePlayer.FindByID(iSteamID)))
      return true;
    return ((IEnumerable<Connection>) ConnectionAuth.m_AuthConnection).Any<Connection>((Func<Connection, bool>) (item => item.userid == (long) iSteamID));
  }

  public static void Reject(Connection connection, string strReason)
  {
    DebugEx.Log((object) (((object) connection).ToString() + " Rejecting connection - " + strReason), (StackTraceLogType) 0);
    ((Server) Net.sv).Kick(connection, strReason);
    ConnectionAuth.m_AuthConnection.Remove(connection);
  }

  public static void OnDisconnect(Connection connection)
  {
    ConnectionAuth.m_AuthConnection.Remove(connection);
  }

  public void Approve(Connection connection)
  {
    ConnectionAuth.m_AuthConnection.Remove(connection);
    ((ServerMgr) SingletonComponent<ServerMgr>.Instance).connectionQueue.Join(connection);
  }

  public void OnNewConnection(Connection connection)
  {
    connection.connected = (__Null) 0;
    if (connection.token == null || connection.token.Length < 32)
      ConnectionAuth.Reject(connection, "Invalid Token");
    else if (connection.userid == null)
    {
      ConnectionAuth.Reject(connection, "Invalid SteamID");
    }
    else
    {
      if (connection.protocol != 2161)
      {
        if (DeveloperList.Contains((ulong) connection.userid))
        {
          DebugEx.Log((object) ("Not kicking " + (object) (ulong) connection.userid + " for incompatible protocol (is a developer)"), (StackTraceLogType) 0);
        }
        else
        {
          ConnectionAuth.Reject(connection, "Incompatible Version");
          return;
        }
      }
      if (ServerUsers.Is((ulong) connection.userid, ServerUsers.UserGroup.Banned))
      {
        ConnectionAuth.Reject(connection, "You are banned from this server");
      }
      else
      {
        if (ServerUsers.Is((ulong) connection.userid, ServerUsers.UserGroup.Moderator))
        {
          DebugEx.Log((object) (((object) connection).ToString() + " has auth level 1"), (StackTraceLogType) 0);
          connection.authLevel = (__Null) 1;
        }
        if (ServerUsers.Is((ulong) connection.userid, ServerUsers.UserGroup.Owner))
        {
          DebugEx.Log((object) (((object) connection).ToString() + " has auth level 2"), (StackTraceLogType) 0);
          connection.authLevel = (__Null) 2;
        }
        if (DeveloperList.Contains((ulong) connection.userid))
        {
          DebugEx.Log((object) (((object) connection).ToString() + " is a developer"), (StackTraceLogType) 0);
          connection.authLevel = (__Null) 3;
        }
        if (this.IsConnected((ulong) connection.userid))
        {
          ConnectionAuth.Reject(connection, "You are already connected!");
        }
        else
        {
          if (Interface.CallHook("IOnUserApprove", (object) connection) != null)
            return;
          ConnectionAuth.m_AuthConnection.Add(connection);
          this.StartCoroutine(this.AuthorisationRoutine(connection));
        }
      }
    }
  }

  public IEnumerator AuthorisationRoutine(Connection connection)
  {
    ConnectionAuth connectionAuth = this;
    yield return (object) connectionAuth.StartCoroutine(Auth_Steam.Run(connection));
    yield return (object) connectionAuth.StartCoroutine(Auth_EAC.Run(connection));
    if (connection.rejected == null && connection.active != null)
    {
      BasePlayer byId = BasePlayer.FindByID((ulong) connection.userid);
      if (Object.op_Implicit((Object) byId) && byId.net.get_connection() != null)
        ConnectionAuth.Reject(connection, "You are already connected as a player!");
      else
        connectionAuth.Approve(connection);
    }
  }

  public ConnectionAuth()
  {
    base.\u002Ector();
  }
}
