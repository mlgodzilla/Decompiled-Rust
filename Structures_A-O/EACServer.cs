// Decompiled with JetBrains decompiler
// Type: EACServer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using EasyAntiCheat.Server;
using EasyAntiCheat.Server.Cerberus;
using EasyAntiCheat.Server.Hydra;
using Network;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EACServer
{
  private static Dictionary<EasyAntiCheat.Server.Hydra.Client, Connection> client2connection = new Dictionary<EasyAntiCheat.Server.Hydra.Client, Connection>();
  private static Dictionary<Connection, EasyAntiCheat.Server.Hydra.Client> connection2client = new Dictionary<Connection, EasyAntiCheat.Server.Hydra.Client>();
  private static Dictionary<Connection, ClientStatus> connection2status = new Dictionary<Connection, ClientStatus>();
  private static EasyAntiCheatServer<EasyAntiCheat.Server.Hydra.Client> easyAntiCheat = (EasyAntiCheatServer<EasyAntiCheat.Server.Hydra.Client>) null;
  public static ICerberus<EasyAntiCheat.Server.Hydra.Client> playerTracker;
  public static EasyAntiCheat.Server.Scout.Scout eacScout;

  public static void Encrypt(
    Connection connection,
    MemoryStream src,
    int srcOffset,
    MemoryStream dst,
    int dstOffset)
  {
    EACServer.easyAntiCheat.get_NetProtect().ProtectMessage(EACServer.GetClient(connection), src, (long) srcOffset, dst, (long) dstOffset);
  }

  public static void Decrypt(
    Connection connection,
    MemoryStream src,
    int srcOffset,
    MemoryStream dst,
    int dstOffset)
  {
    EACServer.easyAntiCheat.get_NetProtect().UnprotectMessage(EACServer.GetClient(connection), src, (long) srcOffset, dst, (long) dstOffset);
  }

  public static EasyAntiCheat.Server.Hydra.Client GetClient(Connection connection)
  {
    EasyAntiCheat.Server.Hydra.Client client;
    EACServer.connection2client.TryGetValue(connection, out client);
    return client;
  }

  public static Connection GetConnection(EasyAntiCheat.Server.Hydra.Client client)
  {
    Connection connection;
    EACServer.client2connection.TryGetValue(client, out connection);
    return connection;
  }

  public static bool IsAuthenticated(Connection connection)
  {
    ClientStatus clientStatus;
    EACServer.connection2status.TryGetValue(connection, out clientStatus);
    return clientStatus == 5;
  }

  private static void OnAuthenticatedLocal(Connection connection)
  {
    if ((string) connection.authStatus == string.Empty)
      connection.authStatus = (__Null) "ok";
    EACServer.connection2status[connection] = (ClientStatus) 2;
  }

  private static void OnAuthenticatedRemote(Connection connection)
  {
    EACServer.connection2status[connection] = (ClientStatus) 5;
  }

  public static bool ShouldIgnore(Connection connection)
  {
    return !ConVar.Server.secure || connection.authLevel >= 3;
  }

  private static void HandleClientUpdate(ClientStatusUpdate<EasyAntiCheat.Server.Hydra.Client> clientStatus)
  {
    using (TimeWarning.New("AntiCheatKickPlayer", 10L))
    {
      EasyAntiCheat.Server.Hydra.Client client = clientStatus.get_Client();
      Connection connection = EACServer.GetConnection(client);
      if (connection == null)
      {
        Debug.LogError((object) ("EAC status update for invalid client: " + (object) ((EasyAntiCheat.Server.Hydra.Client) ref client).get_ClientID()));
      }
      else
      {
        if (EACServer.ShouldIgnore(connection))
          return;
        if (clientStatus.get_RequiresKick())
        {
          string message = clientStatus.get_Message();
          if (string.IsNullOrEmpty(message))
            message = clientStatus.get_Status().ToString();
          Debug.Log((object) ("[EAC] Kicking " + (object) (ulong) connection.userid + " (" + message + ")"));
          connection.authStatus = (__Null) "eac";
          ((Network.Server) Net.sv).Kick(connection, "EAC: " + message);
          DateTime? nullable = new DateTime?();
          if (clientStatus.IsBanned(ref nullable))
          {
            connection.authStatus = (__Null) "eacbanned";
            Interface.CallHook("OnPlayerBanned", (object) connection, (object) connection.authStatus);
            ConsoleNetwork.BroadcastToAllClients("chat.add", (object) 0, (object) ("<color=#fff>SERVER</color> Kicking " + (string) connection.username + " (banned by anticheat)"));
            if (!nullable.HasValue)
              Entity.DeleteBy((ulong) connection.userid);
          }
          EACServer.easyAntiCheat.UnregisterClient(client);
          EACServer.client2connection.Remove(client);
          EACServer.connection2client.Remove(connection);
          EACServer.connection2status.Remove(connection);
        }
        else if (clientStatus.get_Status() == 2)
        {
          EACServer.OnAuthenticatedLocal(connection);
          EACServer.easyAntiCheat.SetClientNetworkState(client, false);
        }
        else
        {
          if (clientStatus.get_Status() != 5)
            return;
          EACServer.OnAuthenticatedRemote(connection);
        }
      }
    }
  }

  private static void SendToClient(EasyAntiCheat.Server.Hydra.Client client, byte[] message, int messageLength)
  {
    Connection connection = EACServer.GetConnection(client);
    if (connection == null)
    {
      Debug.LogError((object) ("EAC network packet for invalid client: " + (object) ((EasyAntiCheat.Server.Hydra.Client) ref client).get_ClientID()));
    }
    else
    {
      if (!((Write) ((NetworkPeer) Net.sv).write).Start())
        return;
      ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 22);
      ((Write) ((NetworkPeer) Net.sv).write).UInt32((uint) messageLength);
      ((Stream) ((NetworkPeer) Net.sv).write).Write(message, 0, messageLength);
      ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo(connection));
    }
  }

  public static void DoStartup()
  {
    EACServer.client2connection.Clear();
    EACServer.connection2client.Clear();
    EACServer.connection2status.Clear();
    Log.SetOut((TextWriter) new StreamWriter(ConVar.Server.rootFolder + "/Log.EAC.txt", false)
    {
      AutoFlush = true
    });
    Log.set_Prefix("");
    Log.set_Level((LogLevel) 3);
    // ISSUE: method pointer
    EACServer.easyAntiCheat = new EasyAntiCheatServer<EasyAntiCheat.Server.Hydra.Client>(new EasyAntiCheatServer<EasyAntiCheat.Server.Hydra.Client>.ClientStatusHandler((object) null, __methodptr(HandleClientUpdate)), 20, ConVar.Server.hostname);
    EACServer.playerTracker = EACServer.easyAntiCheat.get_Cerberus();
    EACServer.playerTracker.LogGameRoundStart(World.Name, string.Empty, 0);
    EACServer.eacScout = new EasyAntiCheat.Server.Scout.Scout();
  }

  public static void DoUpdate()
  {
    if (EACServer.easyAntiCheat == null)
      return;
    EACServer.easyAntiCheat.HandleClientUpdates();
    if (Net.sv == null || !((Network.Server) Net.sv).IsConnected())
      return;
    EasyAntiCheat.Server.Hydra.Client client;
    byte[] message;
    int messageLength;
    while (EACServer.easyAntiCheat.PopNetworkMessage(ref client, ref message, ref messageLength))
      EACServer.SendToClient(client, message, messageLength);
  }

  public static void DoShutdown()
  {
    EACServer.client2connection.Clear();
    EACServer.connection2client.Clear();
    EACServer.connection2status.Clear();
    if (EACServer.eacScout != null)
    {
      Debug.Log((object) "EasyAntiCheat Scout Shutting Down");
      EACServer.eacScout.Dispose();
      EACServer.eacScout = (EasyAntiCheat.Server.Scout.Scout) null;
    }
    if (EACServer.easyAntiCheat == null)
      return;
    Debug.Log((object) "EasyAntiCheat Server Shutting Down");
    EACServer.easyAntiCheat.Dispose();
    EACServer.easyAntiCheat = (EasyAntiCheatServer<EasyAntiCheat.Server.Hydra.Client>) null;
  }

  public static void OnLeaveGame(Connection connection)
  {
    if (EACServer.easyAntiCheat == null)
      return;
    EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(connection);
    EACServer.easyAntiCheat.UnregisterClient(client);
    EACServer.client2connection.Remove(client);
    EACServer.connection2client.Remove(connection);
    EACServer.connection2status.Remove(connection);
  }

  public static void OnJoinGame(Connection connection)
  {
    if (EACServer.easyAntiCheat != null)
    {
      EasyAntiCheat.Server.Hydra.Client compatibilityClient = EACServer.easyAntiCheat.GenerateCompatibilityClient();
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      EACServer.easyAntiCheat.RegisterClient(compatibilityClient, __nonvirtual (connection.userid.ToString()), (string) connection.ipaddress, __nonvirtual (connection.ownerid.ToString()), (string) connection.username, connection.authLevel > 0 ? (PlayerRegisterFlags) 1 : (PlayerRegisterFlags) 0);
      EACServer.client2connection.Add(compatibilityClient, connection);
      EACServer.connection2client.Add(connection, compatibilityClient);
      EACServer.connection2status.Add(connection, (ClientStatus) 0);
      if (!EACServer.ShouldIgnore(connection))
        return;
      EACServer.OnAuthenticatedLocal(connection);
      EACServer.OnAuthenticatedRemote(connection);
    }
    else
    {
      EACServer.OnAuthenticatedLocal(connection);
      EACServer.OnAuthenticatedRemote(connection);
    }
  }

  public static void OnStartLoading(Connection connection)
  {
    if (EACServer.easyAntiCheat == null)
      return;
    EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(connection);
    EACServer.easyAntiCheat.SetClientNetworkState(client, false);
  }

  public static void OnFinishLoading(Connection connection)
  {
    if (EACServer.easyAntiCheat == null)
      return;
    EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(connection);
    EACServer.easyAntiCheat.SetClientNetworkState(client, true);
  }

  public static void OnMessageReceived(Message message)
  {
    if (!EACServer.connection2client.ContainsKey((Connection) message.connection))
    {
      Debug.LogError((object) ("EAC network packet from invalid connection: " + (object) (ulong) ((Connection) message.connection).userid));
    }
    else
    {
      EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient((Connection) message.connection);
      MemoryStream memoryStream = message.get_read().MemoryStreamWithSize();
      EACServer.easyAntiCheat.PushNetworkMessage(client, memoryStream.GetBuffer(), (int) memoryStream.Length);
    }
  }
}
