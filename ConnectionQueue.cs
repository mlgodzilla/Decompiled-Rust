// Decompiled with JetBrains decompiler
// Type: ConnectionQueue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Oxide.Core;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionQueue
{
  public List<Connection> queue = new List<Connection>();
  public List<Connection> joining = new List<Connection>();
  public float nextMessageTime;

  public int Queued
  {
    get
    {
      return this.queue.Count;
    }
  }

  public int Joining
  {
    get
    {
      return this.joining.Count;
    }
  }

  public void SkipQueue(ulong userid)
  {
    for (int index = 0; index < this.queue.Count; ++index)
    {
      Connection connection = this.queue[index];
      if (connection.userid == (long) userid)
      {
        this.JoinGame(connection);
        break;
      }
    }
  }

  internal void Join(Connection connection)
  {
    connection.state = (__Null) 2;
    this.queue.Add(connection);
    this.nextMessageTime = 0.0f;
    if (!this.CanJumpQueue(connection))
      return;
    this.JoinGame(connection);
  }

  public void Cycle(int availableSlots)
  {
    if (this.queue.Count == 0)
      return;
    if (availableSlots - this.Joining > 0)
      this.JoinGame(this.queue[0]);
    this.SendMessages();
  }

  private void SendMessages()
  {
    if ((double) this.nextMessageTime > (double) Time.get_realtimeSinceStartup())
      return;
    this.nextMessageTime = Time.get_realtimeSinceStartup() + 10f;
    for (int position = 0; position < this.queue.Count; ++position)
      this.SendMessage(this.queue[position], position);
  }

  private void SendMessage(Connection c, int position)
  {
    string empty = string.Empty;
    string str = position <= 0 ? string.Format("YOU'RE NEXT - {1:N0} PLAYERS BEHIND YOU", (object) position, (object) (this.queue.Count - position - 1)) : string.Format("{0:N0} PLAYERS AHEAD OF YOU, {1:N0} PLAYERS BEHIND", (object) position, (object) (this.queue.Count - position - 1));
    if (!((Write) ((NetworkPeer) Net.sv).write).Start())
      return;
    ((Write) ((NetworkPeer) Net.sv).write).PacketID((Message.Type) 16);
    ((Write) ((NetworkPeer) Net.sv).write).String("QUEUE");
    ((Write) ((NetworkPeer) Net.sv).write).String(str);
    ((Write) ((NetworkPeer) Net.sv).write).Send(new SendInfo(c));
  }

  public void RemoveConnection(Connection connection)
  {
    if (this.queue.Remove(connection))
      this.nextMessageTime = 0.0f;
    this.joining.Remove(connection);
  }

  private void JoinGame(Connection connection)
  {
    this.queue.Remove(connection);
    connection.state = (__Null) 3;
    this.nextMessageTime = 0.0f;
    this.joining.Add(connection);
    ((ServerMgr) SingletonComponent<ServerMgr>.Instance).JoinGame(connection);
  }

  public void JoinedGame(Connection connection)
  {
    this.RemoveConnection(connection);
  }

  private bool CanJumpQueue(Connection connection)
  {
    object obj = Interface.CallHook("CanBypassQueue", (object) connection);
    if (obj is bool)
      return (bool) obj;
    if (DeveloperList.Contains((ulong) connection.userid))
      return true;
    ServerUsers.User user = ServerUsers.Get((ulong) connection.userid);
    return user != null && user.group == ServerUsers.UserGroup.Moderator || user != null && user.group == ServerUsers.UserGroup.Owner;
  }
}
