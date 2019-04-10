// Decompiled with JetBrains decompiler
// Type: ConsoleNetwork
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Network;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class ConsoleNetwork
{
  internal static void Init()
  {
  }

  internal static void OnClientCommand(Message packet)
  {
    if (packet.get_read().get_unread() > Server.maxcommandpacketsize)
    {
      Debug.LogWarning((object) "Dropping client command due to size");
    }
    else
    {
      string str = packet.get_read().String();
      if (packet.connection == null || ((Connection) packet.connection).connected == null)
      {
        Debug.LogWarning((object) ("Client without connection tried to run command: " + str));
      }
      else
      {
        ConsoleSystem.Option server = ConsoleSystem.Option.get_Server();
        ConsoleSystem.Option option = ((ConsoleSystem.Option) ref server).FromConnection((Connection) packet.connection);
        string strCommand = ConsoleSystem.Run(((ConsoleSystem.Option) ref option).Quiet(), str, (object[]) Array.Empty<object>());
        if (string.IsNullOrEmpty(strCommand))
          return;
        ConsoleNetwork.SendClientReply((Connection) packet.connection, strCommand);
      }
    }
  }

  internal static void SendClientReply(Connection cn, string strCommand)
  {
    if (!((Server) Net.sv).IsConnected())
      return;
    ((Write) ((NetworkPeer) Net.sv).write).Start();
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 11);
    ((Write) ((NetworkPeer) Net.sv).write).String(strCommand);
    ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo(cn));
  }

  public static void SendClientCommand(Connection cn, string strCommand, params object[] args)
  {
    if (!((Server) Net.sv).IsConnected())
      return;
    ((Write) ((NetworkPeer) Net.sv).write).Start();
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 12);
    ((Write) ((NetworkPeer) Net.sv).write).String(ConsoleSystem.BuildCommand(strCommand, args));
    ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo(cn));
  }

  public static void SendClientCommand(
    List<Connection> cn,
    string strCommand,
    params object[] args)
  {
    if (!((Server) Net.sv).IsConnected())
      return;
    ((Write) ((NetworkPeer) Net.sv).write).Start();
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 12);
    ((Write) ((NetworkPeer) Net.sv).write).String(ConsoleSystem.BuildCommand(strCommand, args));
    ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo((IEnumerable<Connection>) cn));
  }

  public static void BroadcastToAllClients(string strCommand, params object[] args)
  {
    if (!((Server) Net.sv).IsConnected())
      return;
    ((Write) ((NetworkPeer) Net.sv).write).Start();
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 12);
    ((Write) ((NetworkPeer) Net.sv).write).String(ConsoleSystem.BuildCommand(strCommand, args));
    ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo((IEnumerable<Connection>) ((Server) Net.sv).connections));
  }
}
