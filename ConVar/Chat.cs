// Decompiled with JetBrains decompiler
// Type: ConVar.Chat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using Facepunch.Math;
using Oxide.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("chat")]
  public class Chat : ConsoleSystem
  {
    [ClientVar]
    [ServerVar]
    public static bool enabled = true;
    private static List<Chat.ChatEntry> History = new List<Chat.ChatEntry>();
    [ServerVar]
    public static bool serverlog = true;
    private const float textRange = 50f;
    private const float textVolumeBoost = 0.2f;

    public static void Broadcast(string message, string username = "SERVER", string color = "#eee", ulong userid = 0)
    {
      if (Interface.CallHook("OnServerMessage", (object) message, (object) username, (object) color, (object) userid) != null)
        return;
      string str = StringEx.EscapeRichText(username);
      ConsoleNetwork.BroadcastToAllClients("chat.add", (object) 0, (object) ("<color=" + color + ">" + str + "</color> " + message));
      Chat.ChatEntry chatEntry = new Chat.ChatEntry()
      {
        Message = message,
        UserId = userid,
        Username = username,
        Color = color,
        Time = Epoch.get_Current()
      };
      Chat.History.Add(chatEntry);
      RCon.Broadcast(RCon.LogType.Chat, (object) chatEntry);
    }

    [ServerUserVar]
    public static void say(ConsoleSystem.Arg arg)
    {
      if (!Chat.enabled)
      {
        arg.ReplyWith("Chat is disabled.");
      }
      else
      {
        BasePlayer basePlayer = arg.Player();
        if (!Object.op_Implicit((Object) basePlayer) || basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
          return;
        if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper)
        {
          if ((double) basePlayer.NextChatTime == 0.0)
            basePlayer.NextChatTime = Time.get_realtimeSinceStartup() - 30f;
          if ((double) basePlayer.NextChatTime > (double) Time.get_realtimeSinceStartup())
          {
            basePlayer.NextChatTime += 2f;
            float num = basePlayer.NextChatTime - Time.get_realtimeSinceStartup();
            ConsoleNetwork.SendClientCommand(basePlayer.net.get_connection(), "chat.add", (object) 0, (object) ("You're chatting too fast - try again in " + (num + 0.5f).ToString("0") + " seconds"));
            if ((double) num <= 120.0)
              return;
            basePlayer.Kick("Chatting too fast");
            return;
          }
        }
        string str1 = arg.GetString(0, "text").Trim();
        if (str1.Length > 128)
          str1 = str1.Substring(0, 128);
        if (str1.Length <= 0)
          return;
        if (str1.StartsWith("/") || str1.StartsWith("\\"))
        {
          if (Interface.CallHook("IOnPlayerCommand", (object) arg) != null)
            ;
        }
        else
        {
          string str2 = StringEx.EscapeRichText(str1);
          if (Interface.CallHook("IOnPlayerChat", (object) arg) != null)
            return;
          if (Chat.serverlog)
          {
            ServerConsole.PrintColoured((object) ConsoleColor.DarkYellow, (object) (basePlayer.displayName + ": "), (object) ConsoleColor.DarkGreen, (object) str2);
            DebugEx.Log((object) string.Format("[CHAT] {0} : {1}", (object) ((object) basePlayer).ToString(), (object) str2), (StackTraceLogType) 0);
          }
          string str3 = "#5af";
          if (basePlayer.IsAdmin)
            str3 = "#af5";
          if (basePlayer.IsDeveloper)
            str3 = "#fa5";
          string str4 = StringEx.EscapeRichText(basePlayer.displayName);
          basePlayer.NextChatTime = Time.get_realtimeSinceStartup() + 1.5f;
          Chat.ChatEntry chatEntry = new Chat.ChatEntry()
          {
            Message = str2,
            UserId = basePlayer.userID,
            Username = basePlayer.displayName,
            Color = str3,
            Time = Epoch.get_Current()
          };
          Chat.History.Add(chatEntry);
          RCon.Broadcast(RCon.LogType.Chat, (object) chatEntry);
          if (Server.globalchat)
          {
            ConsoleNetwork.BroadcastToAllClients("chat.add2", (object) basePlayer.userID, (object) str2, (object) str4, (object) str3, (object) 1f);
            arg.ReplyWith("");
          }
          else
          {
            float num = 2500f;
            foreach (BasePlayer activePlayer in BasePlayer.activePlayerList)
            {
              Vector3 vector3 = Vector3.op_Subtraction(((Component) activePlayer).get_transform().get_position(), ((Component) basePlayer).get_transform().get_position());
              float sqrMagnitude = ((Vector3) ref vector3).get_sqrMagnitude();
              if ((double) sqrMagnitude <= (double) num)
                ConsoleNetwork.SendClientCommand(activePlayer.net.get_connection(), "chat.add2", (object) basePlayer.userID, (object) str2, (object) str4, (object) str3, (object) Mathf.Clamp01((float) ((double) num - (double) sqrMagnitude + 0.200000002980232)));
            }
            arg.ReplyWith("");
          }
        }
      }
    }

    [ServerVar]
    [Help("Return the last x lines of the console. Default is 200")]
    public static IEnumerable<Chat.ChatEntry> tail(ConsoleSystem.Arg arg)
    {
      int num = arg.GetInt(0, 200);
      int count = Chat.History.Count - num;
      if (count < 0)
        count = 0;
      return Chat.History.Skip<Chat.ChatEntry>(count);
    }

    [Help("Search the console for a particular string")]
    [ServerVar]
    public static IEnumerable<Chat.ChatEntry> search(ConsoleSystem.Arg arg)
    {
      string search = arg.GetString(0, (string) null);
      if (search == null)
        return Enumerable.Empty<Chat.ChatEntry>();
      return Chat.History.Where<Chat.ChatEntry>((Func<Chat.ChatEntry, bool>) (x =>
      {
        if (x.Message.Length < 4096)
          return StringEx.Contains(x.Message, search, CompareOptions.IgnoreCase);
        return false;
      }));
    }

    public Chat()
    {
      base.\u002Ector();
    }

    public struct ChatEntry
    {
      public string Message { get; set; }

      public ulong UserId { get; set; }

      public string Username { get; set; }

      public string Color { get; set; }

      public int Time { get; set; }
    }
  }
}
