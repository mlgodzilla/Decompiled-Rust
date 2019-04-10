// Decompiled with JetBrains decompiler
// Type: ServerConsole
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using System;
using UnityEngine;
using Windows;

public class ServerConsole : SingletonComponent<ServerConsole>
{
  private ConsoleWindow console;
  private ConsoleInput input;
  private float nextUpdate;

  public void OnEnable()
  {
    if (Environment.OSVersion.Platform == PlatformID.Unix)
      return;
    this.console.Initialize();
    this.input.OnInputText += new Action<string>(this.OnInputText);
    Output.OnMessage += new Action<string, string, LogType>(this.HandleLog);
    this.input.ClearLine(System.Console.WindowHeight);
    for (int index = 0; index < System.Console.WindowHeight; ++index)
      System.Console.WriteLine("");
  }

  private void OnDisable()
  {
    Output.OnMessage -= new Action<string, string, LogType>(this.HandleLog);
    if (Environment.OSVersion.Platform == PlatformID.Unix)
      return;
    this.input.OnInputText -= new Action<string>(this.OnInputText);
    this.console.Shutdown();
  }

  private void OnInputText(string obj)
  {
    ConsoleSystem.Run(ConsoleSystem.Option.get_Server(), obj, (object[]) Array.Empty<object>());
  }

  public static void PrintColoured(params object[] objects)
  {
    if (Environment.OSVersion.Platform == PlatformID.Unix || Object.op_Equality((Object) SingletonComponent<ServerConsole>.Instance, (Object) null))
      return;
    ((ServerConsole) SingletonComponent<ServerConsole>.Instance).input.ClearLine(((ServerConsole) SingletonComponent<ServerConsole>.Instance).input.statusText.Length);
    for (int index = 0; index < objects.Length; ++index)
    {
      if (index % 2 == 0)
        System.Console.ForegroundColor = (ConsoleColor) objects[index];
      else
        System.Console.Write((string) objects[index]);
    }
    if (System.Console.CursorLeft != 0)
      ++System.Console.CursorTop;
    ((ServerConsole) SingletonComponent<ServerConsole>.Instance).input.RedrawInputLine();
  }

  private void HandleLog(string message, string stackTrace, LogType type)
  {
    if (Environment.OSVersion.Platform == PlatformID.Unix || message.StartsWith("[CHAT]") || message.StartsWith("[Physics.PhysX] RigidBody::setRigidBodyFlag"))
      return;
    if (type == 2)
    {
      if (message.StartsWith("HDR RenderTexture format is not") || message.StartsWith("The image effect") || (message.StartsWith("Image Effects are not supported on this platform") || message.StartsWith("[AmplifyColor]")) || message.StartsWith("Skipping profile frame."))
        return;
      System.Console.ForegroundColor = ConsoleColor.Yellow;
    }
    else
      System.Console.ForegroundColor = type != null ? (type != 4 ? (type != 1 ? ConsoleColor.Gray : ConsoleColor.Red) : ConsoleColor.Red) : ConsoleColor.Red;
    this.input.ClearLine(this.input.statusText.Length);
    System.Console.WriteLine(message);
    this.input.RedrawInputLine();
  }

  private void Update()
  {
    if (Environment.OSVersion.Platform == PlatformID.Unix)
      return;
    this.UpdateStatus();
    this.input.Update();
  }

  private void UpdateStatus()
  {
    if ((double) this.nextUpdate > (double) Time.get_realtimeSinceStartup() || Net.sv == null || !((Server) Net.sv).IsConnected())
      return;
    this.nextUpdate = Time.get_realtimeSinceStartup() + 0.33f;
    if (!this.input.valid)
      return;
    string str1 = NumberExtensions.FormatSeconds((long) Time.get_realtimeSinceStartup());
    string str2 = " " + this.currentGameTime.ToString("[H:mm]") + " [" + (object) this.currentPlayerCount + "/" + (object) this.maxPlayerCount + "] " + Server.hostname + " [" + Server.level + "]";
    string str3 = Performance.current.frameRate.ToString() + "fps " + (object) Performance.current.memoryCollections + "gc " + str1 ?? "";
    string str4 = NumberExtensions.FormatBytes<ulong>((M0) (long) ((NetworkPeer) Net.sv).GetStat((Connection) null, (NetworkPeer.StatTypeLong) 3), true) + "/s in, " + NumberExtensions.FormatBytes<ulong>((M0) (long) ((NetworkPeer) Net.sv).GetStat((Connection) null, (NetworkPeer.StatTypeLong) 1), true) + "/s out";
    string str5 = str3.PadLeft(this.input.lineWidth - 1);
    string str6 = str2 + (str2.Length < str5.Length ? str5.Substring(str2.Length) : "");
    string str7 = " " + this.currentEntityCount.ToString("n0") + " ents, " + this.currentSleeperCount.ToString("n0") + " slprs";
    string str8 = str4.PadLeft(this.input.lineWidth - 1);
    string str9 = str7 + (str7.Length < str8.Length ? str8.Substring(str7.Length) : "");
    this.input.statusText[0] = "";
    this.input.statusText[1] = str6;
    this.input.statusText[2] = str9;
  }

  private DateTime currentGameTime
  {
    get
    {
      if (!Object.op_Implicit((Object) TOD_Sky.get_Instance()))
        return DateTime.Now;
      return ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).get_DateTime();
    }
  }

  private int currentPlayerCount
  {
    get
    {
      return BasePlayer.activePlayerList.Count;
    }
  }

  private int maxPlayerCount
  {
    get
    {
      return Server.maxplayers;
    }
  }

  private int currentEntityCount
  {
    get
    {
      return BaseNetworkable.serverEntities.Count;
    }
  }

  private int currentSleeperCount
  {
    get
    {
      return BasePlayer.sleepingPlayerList.Count;
    }
  }

  public ServerConsole()
  {
    if (Environment.OSVersion.Platform == PlatformID.Unix)
      return;
    this.console = new ConsoleWindow();
    this.input = new ConsoleInput();
    base.\u002Ector();
  }
}
