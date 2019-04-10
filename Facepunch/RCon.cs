// Decompiled with JetBrains decompiler
// Type: Facepunch.RCon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch.Rcon;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Facepunch
{
  public class RCon
  {
    public static string Password = "";
    [ServerVar]
    public static int Port = 0;
    [ServerVar]
    public static string Ip = "";
    [ServerVar(Help = "If set to true, use websocket rcon. If set to false use legacy, source engine rcon.")]
    public static bool Web = true;
    [ServerVar(Help = "If true, rcon commands etc will be printed in the console")]
    public static bool Print = false;
    internal static RCon.RConListener listener = (RCon.RConListener) null;
    internal static Listener listenerNew = (Listener) null;
    private static Queue<RCon.Command> Commands = new Queue<RCon.Command>();
    private static float lastRunTime = 0.0f;
    internal static List<RCon.BannedAddresses> bannedAddresses = new List<RCon.BannedAddresses>();
    internal static int SERVERDATA_AUTH = 3;
    internal static int SERVERDATA_EXECCOMMAND = 2;
    internal static int SERVERDATA_AUTH_RESPONSE = 2;
    internal static int SERVERDATA_RESPONSE_VALUE = 0;
    internal static int SERVERDATA_CONSOLE_LOG = 4;
    internal static int SERVERDATA_SWITCH_UTF8 = 5;
    private static int responseIdentifier;
    private static string responseConnection;
    private static bool isInput;

    public static void Initialize()
    {
      if (Interface.CallHook("IOnRconInitialize") != null)
        return;
      if (RCon.Port == 0)
        RCon.Port = Server.port;
      RCon.Password = CommandLine.GetSwitch("-rcon.password", CommandLine.GetSwitch("+rcon.password", ""));
      if (RCon.Password == "password" || RCon.Password == "")
        return;
      Output.OnMessage += new Action<string, string, UnityEngine.LogType>(RCon.OnMessage);
      if (RCon.Web)
      {
        RCon.listenerNew = new Listener();
        if (!string.IsNullOrEmpty(RCon.Ip))
          RCon.listenerNew.Address = (__Null) RCon.Ip;
        RCon.listenerNew.Password = (__Null) RCon.Password;
        RCon.listenerNew.Port = (__Null) RCon.Port;
        RCon.listenerNew.SslCertificate = (__Null) CommandLine.GetSwitch("-rcon.ssl", (string) null);
        RCon.listenerNew.SslCertificatePassword = (__Null) CommandLine.GetSwitch("-rcon.sslpwd", (string) null);
        RCon.listenerNew.OnMessage = (__Null) ((ip, id, msg) =>
        {
          lock (RCon.Commands)
          {
            RCon.Command command = (RCon.Command) JsonConvert.DeserializeObject<RCon.Command>(msg);
            command.Ip = ip;
            command.ConnectionId = id;
            RCon.Commands.Enqueue(command);
          }
        });
        RCon.listenerNew.Start();
        Debug.Log((object) ("WebSocket RCon Started on " + (object) RCon.Port));
      }
      else
      {
        RCon.listener = new RCon.RConListener();
        Debug.Log((object) ("RCon Started on " + (object) RCon.Port));
        Debug.Log((object) "Source style TCP Rcon is deprecated. Please switch to Websocket Rcon before it goes away.");
      }
    }

    public static void Shutdown()
    {
      if (RCon.listenerNew != null)
      {
        RCon.listenerNew.Shutdown();
        RCon.listenerNew = (Listener) null;
      }
      if (RCon.listener == null)
        return;
      RCon.listener.Shutdown();
      RCon.listener = (RCon.RConListener) null;
    }

    public static void Broadcast(RCon.LogType type, object obj)
    {
      if (RCon.listenerNew == null)
        return;
      RCon.Response response = new RCon.Response();
      response.Identifier = -1;
      response.Message = JsonConvert.SerializeObject(obj, (Formatting) 1);
      response.Type = type;
      if (string.IsNullOrEmpty(RCon.responseConnection))
        RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject((object) response, (Formatting) 1));
      else
        RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject((object) response, (Formatting) 1));
    }

    public static void Update()
    {
      lock (RCon.Commands)
      {
        while (RCon.Commands.Count > 0)
          RCon.OnCommand(RCon.Commands.Dequeue());
      }
      if (RCon.listener == null || (double) RCon.lastRunTime + 0.0199999995529652 >= (double) Time.get_realtimeSinceStartup())
        return;
      RCon.lastRunTime = Time.get_realtimeSinceStartup();
      try
      {
        RCon.bannedAddresses.RemoveAll((Predicate<RCon.BannedAddresses>) (x => (double) x.banTime < (double) Time.get_realtimeSinceStartup()));
        RCon.listener.Cycle();
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) "Rcon Exception");
        Debug.LogException(ex);
      }
    }

    public static void BanIP(IPAddress addr, float seconds)
    {
      RCon.bannedAddresses.RemoveAll((Predicate<RCon.BannedAddresses>) (x => x.addr == addr));
      RCon.BannedAddresses bannedAddresses = new RCon.BannedAddresses()
      {
        addr = addr,
        banTime = Time.get_realtimeSinceStartup() + seconds
      };
    }

    public static bool IsBanned(IPAddress addr)
    {
      return RCon.bannedAddresses.Count<RCon.BannedAddresses>((Func<RCon.BannedAddresses, bool>) (x =>
      {
        if (x.addr == addr)
          return (double) x.banTime > (double) Time.get_realtimeSinceStartup();
        return false;
      })) > 0;
    }

    private static void OnCommand(RCon.Command cmd)
    {
      try
      {
        RCon.responseIdentifier = cmd.Identifier;
        RCon.responseConnection = cmd.ConnectionId;
        RCon.isInput = true;
        if (RCon.Print)
          Debug.Log((object) ("[rcon] " + (object) cmd.Ip + ": " + cmd.Message));
        RCon.isInput = false;
        ConsoleSystem.Option server = ConsoleSystem.Option.get_Server();
        string message = ConsoleSystem.Run(((ConsoleSystem.Option) ref server).Quiet(), cmd.Message, (object[]) Array.Empty<object>());
        if (message == null)
          return;
        RCon.OnMessage(message, string.Empty, (UnityEngine.LogType) 3);
      }
      finally
      {
        RCon.responseIdentifier = 0;
        RCon.responseConnection = string.Empty;
      }
    }

    private static void OnMessage(string message, string stacktrace, UnityEngine.LogType type)
    {
      if (RCon.isInput || RCon.listenerNew == null)
        return;
      RCon.Response response = new RCon.Response();
      response.Identifier = RCon.responseIdentifier;
      response.Message = message;
      response.Stacktrace = stacktrace;
      response.Type = RCon.LogType.Generic;
      if (type == null || type == 4)
        response.Type = RCon.LogType.Error;
      if (type == 1 || type == 2)
        response.Type = RCon.LogType.Warning;
      if (string.IsNullOrEmpty(RCon.responseConnection))
        RCon.listenerNew.BroadcastMessage(JsonConvert.SerializeObject((object) response, (Formatting) 1));
      else
        RCon.listenerNew.SendMessage(RCon.responseConnection, JsonConvert.SerializeObject((object) response, (Formatting) 1));
    }

    public struct Command
    {
      public IPEndPoint Ip;
      public string ConnectionId;
      public string Name;
      public string Message;
      public int Identifier;
    }

    public enum LogType
    {
      Generic,
      Error,
      Warning,
      Chat,
    }

    public struct Response
    {
      public string Message;
      public int Identifier;
      [JsonConverter(typeof (StringEnumConverter))]
      public RCon.LogType Type;
      public string Stacktrace;
    }

    internal struct BannedAddresses
    {
      public IPAddress addr;
      public float banTime;
    }

    internal class RConClient
    {
      private int lastMessageID = -1;
      private Socket socket;
      private bool isAuthorised;
      private string connectionName;
      private bool runningConsoleCommand;
      private bool utf8Mode;

      internal RConClient(Socket cl)
      {
        this.socket = cl;
        this.socket.NoDelay = true;
        this.connectionName = this.socket.RemoteEndPoint.ToString();
      }

      internal bool IsValid()
      {
        return this.socket != null;
      }

      internal void Update()
      {
        if (this.socket == null)
          return;
        if (!this.socket.Connected)
        {
          this.Close("Disconnected");
        }
        else
        {
          int available = this.socket.Available;
          if (available < 14)
            return;
          if (available > 4096)
          {
            this.Close("overflow");
          }
          else
          {
            byte[] buffer = new byte[available];
            this.socket.Receive(buffer);
            using (BinaryReader read = new BinaryReader((Stream) new MemoryStream(buffer, false), this.utf8Mode ? Encoding.UTF8 : Encoding.ASCII))
            {
              int num = read.ReadInt32();
              if (available < num)
              {
                this.Close("invalid packet");
              }
              else
              {
                this.lastMessageID = read.ReadInt32();
                int type = read.ReadInt32();
                string msg = this.ReadNullTerminatedString(read);
                this.ReadNullTerminatedString(read);
                if (!this.HandleMessage(type, msg))
                  this.Close("invalid packet");
                else
                  this.lastMessageID = -1;
              }
            }
          }
        }
      }

      internal bool HandleMessage(int type, string msg)
      {
        if (!this.isAuthorised)
          return this.HandleMessage_UnAuthed(type, msg);
        if (type == RCon.SERVERDATA_SWITCH_UTF8)
        {
          this.utf8Mode = true;
          return true;
        }
        if (type == RCon.SERVERDATA_EXECCOMMAND)
        {
          Debug.Log((object) ("[RCON][" + this.connectionName + "] " + msg));
          this.runningConsoleCommand = true;
          ConsoleSystem.Run(ConsoleSystem.Option.get_Server(), msg, (object[]) Array.Empty<object>());
          this.runningConsoleCommand = false;
          this.Reply(-1, RCon.SERVERDATA_RESPONSE_VALUE, "");
          return true;
        }
        if (type == RCon.SERVERDATA_RESPONSE_VALUE)
        {
          this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
          return true;
        }
        Debug.Log((object) ("[RCON][" + this.connectionName + "] Unhandled: " + (object) this.lastMessageID + " -> " + (object) type + " -> " + msg));
        return false;
      }

      internal bool HandleMessage_UnAuthed(int type, string msg)
      {
        if (type != RCon.SERVERDATA_AUTH)
        {
          RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
          this.Close("Invalid Command - Not Authed");
          return false;
        }
        this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, "");
        this.isAuthorised = RCon.Password == msg;
        if (!this.isAuthorised)
        {
          this.Reply(-1, RCon.SERVERDATA_AUTH_RESPONSE, "");
          RCon.BanIP((this.socket.RemoteEndPoint as IPEndPoint).Address, 60f);
          this.Close("Invalid Password");
          return true;
        }
        this.Reply(this.lastMessageID, RCon.SERVERDATA_AUTH_RESPONSE, "");
        Debug.Log((object) ("[RCON] Auth: " + this.connectionName));
        Output.OnMessage += new Action<string, string, UnityEngine.LogType>(this.Output_OnMessage);
        return true;
      }

      private void Output_OnMessage(string message, string stacktrace, UnityEngine.LogType type)
      {
        if (!this.isAuthorised || !this.IsValid())
          return;
        if (this.lastMessageID != -1 && this.runningConsoleCommand)
          this.Reply(this.lastMessageID, RCon.SERVERDATA_RESPONSE_VALUE, message);
        this.Reply(0, RCon.SERVERDATA_CONSOLE_LOG, message);
      }

      internal void Reply(int id, int type, string msg)
      {
        MemoryStream memoryStream = new MemoryStream(1024);
        using (BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream))
        {
          if (this.utf8Mode)
          {
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            int num = 10 + bytes.Length;
            binaryWriter.Write(num);
            binaryWriter.Write(id);
            binaryWriter.Write(type);
            binaryWriter.Write(bytes);
          }
          else
          {
            int num = 10 + msg.Length;
            binaryWriter.Write(num);
            binaryWriter.Write(id);
            binaryWriter.Write(type);
            foreach (char ch in msg)
              binaryWriter.Write((sbyte) ch);
          }
          binaryWriter.Write((sbyte) 0);
          binaryWriter.Write((sbyte) 0);
          binaryWriter.Flush();
          try
          {
            this.socket.Send(memoryStream.GetBuffer(), (int) memoryStream.Position, SocketFlags.None);
          }
          catch (Exception ex)
          {
            Debug.LogWarning((object) ("Error sending rcon reply: " + (object) ex));
            this.Close("Exception");
          }
        }
      }

      internal void Close(string strReasn)
      {
        Output.OnMessage -= new Action<string, string, UnityEngine.LogType>(this.Output_OnMessage);
        if (this.socket == null)
          return;
        Debug.Log((object) ("[RCON][" + this.connectionName + "] Disconnected: " + strReasn));
        this.socket.Close();
        this.socket = (Socket) null;
      }

      internal string ReadNullTerminatedString(BinaryReader read)
      {
        string str = "";
        while (read.BaseStream.Position != read.BaseStream.Length)
        {
          char ch = read.ReadChar();
          if (ch == char.MinValue)
            return str;
          str += ch.ToString();
          if (str.Length > 8192)
            return string.Empty;
        }
        return "";
      }
    }

    internal class RConListener
    {
      private List<RCon.RConClient> clients = new List<RCon.RConClient>();
      private TcpListener server;

      internal RConListener()
      {
        IPAddress address = IPAddress.Any;
        if (!IPAddress.TryParse(RCon.Ip, out address))
          address = IPAddress.Any;
        this.server = new TcpListener(address, RCon.Port);
        try
        {
          this.server.Start();
        }
        catch (Exception ex)
        {
          Debug.LogWarning((object) ("Couldn't start RCON Listener: " + ex.Message));
          this.server = (TcpListener) null;
        }
      }

      internal void Shutdown()
      {
        if (this.server == null)
          return;
        this.server.Stop();
        this.server = (TcpListener) null;
      }

      internal void Cycle()
      {
        if (this.server == null)
          return;
        this.ProcessConnections();
        this.RemoveDeadClients();
        this.UpdateClients();
      }

      private void ProcessConnections()
      {
        if (!this.server.Pending())
          return;
        Socket cl = this.server.AcceptSocket();
        if (cl == null)
          return;
        IPEndPoint remoteEndPoint = cl.RemoteEndPoint as IPEndPoint;
        if (Interface.CallHook("OnRconConnection", (object) remoteEndPoint) != null)
          cl.Close();
        else if (RCon.IsBanned(remoteEndPoint.Address))
        {
          Debug.Log((object) ("[RCON] Ignoring connection - banned. " + remoteEndPoint.Address.ToString()));
          cl.Close();
        }
        else
          this.clients.Add(new RCon.RConClient(cl));
      }

      private void UpdateClients()
      {
        foreach (RCon.RConClient client in this.clients)
          client.Update();
      }

      private void RemoveDeadClients()
      {
        this.clients.RemoveAll((Predicate<RCon.RConClient>) (x => !x.IsValid()));
      }
    }
  }
}
