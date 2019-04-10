// Decompiled with JetBrains decompiler
// Type: ServerMgr
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Facepunch.Math;
using Facepunch.Steamworks;
using Ionic.Crc;
using Network;
using Network.Visibility;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using UnityEngine;

public class ServerMgr : SingletonComponent<ServerMgr>, IServerCallback
{
  private ConnectionAuth auth;
  private bool runFrameUpdate;
  private bool useQueryPort;
  public UserPersistance persistance;
  private string _AssemblyHash;
  private IEnumerator restartCoroutine;
  public ConnectionQueue connectionQueue;
  private Stopwatch queryTimer;
  private Dictionary<uint, int> unconnectedQueries;
  private Stopwatch queriesPerSeconTimer;
  private int NumQueriesLastSecond;
  private MemoryStream queryBuffer;

  public void Initialize(
    bool loadSave = true,
    string saveFile = "",
    bool allowOutOfDateSaves = false,
    bool skipInitialSpawn = false)
  {
    if (!ConVar.Server.official)
      ExceptionReporter.set_Disabled(true);
    this.persistance = new UserPersistance(ConVar.Server.rootFolder);
    this.SpawnMapEntities();
    if (Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance))
    {
      using (TimeWarning.New("SpawnHandler.UpdateDistributions", 0.1f))
        ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).UpdateDistributions();
    }
    if (loadSave)
      skipInitialSpawn = SaveRestore.Load(saveFile, allowOutOfDateSaves);
    if (Object.op_Implicit((Object) SingletonComponent<SpawnHandler>.Instance))
    {
      if (!skipInitialSpawn)
      {
        using (TimeWarning.New("SpawnHandler.InitialSpawn", 200L))
          ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).InitialSpawn();
      }
      using (TimeWarning.New("SpawnHandler.StartSpawnTick", 200L))
        ((SpawnHandler) SingletonComponent<SpawnHandler>.Instance).StartSpawnTick();
    }
    this.CreateImportantEntities();
    this.auth = (ConnectionAuth) ((Component) this).GetComponent<ConnectionAuth>();
  }

  public void OpenConnection()
  {
    this.useQueryPort = ConVar.Server.queryport > 0 && ConVar.Server.queryport != ConVar.Server.port;
    ((Network.Server) Network.Net.sv).ip = (__Null) ConVar.Server.ip;
    ((Network.Server) Network.Net.sv).port = (__Null) ConVar.Server.port;
    if (!((Network.Server) Network.Net.sv).Start())
    {
      Debug.LogWarning((object) "Couldn't Start Server.");
    }
    else
    {
      this.StartSteamServer();
      ((Network.Server) Network.Net.sv).callbackHandler = (__Null) this;
      ((NetworkPeer) Network.Net.sv).cryptography = (__Null) new NetworkCryptographyServer();
      EACServer.DoStartup();
      ((MonoBehaviour) this).InvokeRepeating("EACUpdate", 1f, 1f);
      ((MonoBehaviour) this).InvokeRepeating("DoTick", 1f, 1f / (float) ConVar.Server.tickrate);
      ((MonoBehaviour) this).InvokeRepeating("DoHeartbeat", 1f, 1f);
      this.runFrameUpdate = true;
      Interface.CallHook("IOnServerInitialized");
    }
  }

  private void CloseConnection()
  {
    if (this.persistance != null)
    {
      this.persistance.Dispose();
      this.persistance = (UserPersistance) null;
    }
    EACServer.DoShutdown();
    ((Network.Server) Network.Net.sv).callbackHandler = null;
    using (TimeWarning.New("sv.Stop", 0.1f))
      ((Network.Server) Network.Net.sv).Stop("Shutting Down");
    using (TimeWarning.New("RCon.Shutdown", 0.1f))
      RCon.Shutdown();
    using (TimeWarning.New("Steamworks.GameServer.Shutdown", 0.1f))
    {
      if (Global.get_SteamServer() == null)
        return;
      Debug.Log((object) "Steamworks Shutting Down");
      ((BaseSteamworks) Global.get_SteamServer()).Dispose();
      Global.set_SteamServer((Facepunch.Steamworks.Server) null);
      Debug.Log((object) "Okay");
    }
  }

  private void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    this.CloseConnection();
  }

  private void OnApplicationQuit()
  {
    Application.isQuitting = (__Null) 1;
    this.CloseConnection();
  }

  private void CreateImportantEntities()
  {
    this.CreateImportantEntity<EnvSync>("assets/bundled/prefabs/system/net_env.prefab");
    this.CreateImportantEntity<CommunityEntity>("assets/bundled/prefabs/system/server/community.prefab");
    this.CreateImportantEntity<ResourceDepositManager>("assets/bundled/prefabs/system/server/resourcedepositmanager.prefab");
    this.CreateImportantEntity<RelationshipManager>("assets/bundled/prefabs/system/server/relationship_manager.prefab");
  }

  private void CreateImportantEntity<T>(string prefabName) where T : BaseEntity
  {
    if (BaseNetworkable.serverEntities.Any<BaseNetworkable>((Func<BaseNetworkable, bool>) (x => x is T)))
      return;
    Debug.LogWarning((object) ("Missing " + typeof (T).Name + " - creating"));
    BaseEntity entity = GameManager.server.CreateEntity(prefabName, (Vector3) null, (Quaternion) null, true);
    if (Object.op_Equality((Object) entity, (Object) null))
      Debug.LogWarning((object) "Couldn't create");
    else
      entity.Spawn();
  }

  private void StartSteamServer()
  {
    if (Global.get_SteamServer() != null)
      return;
    IPAddress ipAddress = (IPAddress) null;
    if (!string.IsNullOrEmpty(ConVar.Server.ip))
      ipAddress = IPAddress.Parse(ConVar.Server.ip);
    Config.ForUnity(Application.get_platform().ToString());
    ServerInit serverInit = new ServerInit("rust", "Rust");
    serverInit.IpAddress = (__Null) ipAddress;
    serverInit.GamePort = (__Null) (int) (ushort) ((Network.Server) Network.Net.sv).port;
    serverInit.Secure = (__Null) (ConVar.Server.secure ? 1 : 0);
    serverInit.VersionString = (__Null) 2161.ToString();
    if (this.useQueryPort)
      serverInit.QueryPort = (__Null) (int) (ushort) ConVar.Server.queryport;
    else
      serverInit.QueryShareGamePort();
    Global.set_SteamServer(new Facepunch.Steamworks.Server(Defines.appID, serverInit));
    if (!((BaseSteamworks) Global.get_SteamServer()).get_IsValid())
    {
      Debug.LogWarning((object) ("Couldn't initialize Steam Server (" + (object) ipAddress + ")"));
      ((BaseSteamworks) Global.get_SteamServer()).Dispose();
      Global.set_SteamServer((Facepunch.Steamworks.Server) null);
      Application.Quit();
    }
    else
    {
      if (CommandLine.HasSwitch("-debugsteamcallbacks"))
      {
        Facepunch.Steamworks.Server steamServer = Global.get_SteamServer();
        ((BaseSteamworks) steamServer).OnAnyCallback = (__Null) Delegate.Combine((Delegate) ((BaseSteamworks) steamServer).OnAnyCallback, (Delegate) new Action<object>(ServerMgr.DebugPrintSteamCallback));
      }
      Global.get_SteamServer().get_Auth().OnAuthChange = (__Null) new Action<ulong, ulong, ServerAuth.Status>(this.OnValidateAuthTicketResponse);
      ((BaseSteamworks) Global.get_SteamServer()).get_Inventory().add_OnDefinitionsUpdated(new Action(this.OnInventoryDefinitionsUpdated));
      Global.get_SteamServer().LogOnAnonymous();
      ((MonoBehaviour) this).InvokeRepeating("UpdateServerInformation", 1f, 10f);
      DebugEx.Log((object) "Connected to Steam", (StackTraceLogType) 0);
    }
  }

  private static void DebugPrintSteamCallback(object obj)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.AppendLine("<color=#88dd88>" + obj.GetType().Name + "</color>");
    foreach (FieldInfo field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      stringBuilder.AppendLine("  <color=#aaaaaa>" + field.Name + ":</color>\t <color=#fffff>" + field.GetValue(obj).ToString() + "</color>");
    Debug.Log((object) stringBuilder.ToString());
  }

  private void OnInventoryDefinitionsUpdated()
  {
    ItemManager.InvalidateWorkshopSkinCache();
  }

  private void OnValidateAuthTicketResponse(ulong SteamId, ulong OwnerId, ServerAuth.Status Status)
  {
    if (Auth_Steam.ValidateConnecting(SteamId, OwnerId, Status))
      return;
    Network.Connection connection = ((IEnumerable<Network.Connection>) ((Network.Server) Network.Net.sv).connections).FirstOrDefault<Network.Connection>((Func<Network.Connection, bool>) (x => x.userid == (long) SteamId));
    if (connection == null)
      Debug.LogWarning((object) ("Steam gave us a " + (object) Status + " ticket response for unconnected id " + (object) SteamId));
    else if (Status == null)
    {
      Debug.LogWarning((object) ("Steam gave us a 'ok' ticket response for already connected id " + (object) SteamId));
    }
    else
    {
      if (Status == 5)
        return;
      connection.authStatus = (__Null) Status.ToString();
      ((Network.Server) Network.Net.sv).Kick(connection, "Steam: " + Status.ToString());
    }
  }

  private void EACUpdate()
  {
    EACServer.DoUpdate();
  }

  public static int AvailableSlots
  {
    get
    {
      return ConVar.Server.maxplayers - BasePlayer.activePlayerList.Count<BasePlayer>();
    }
  }

  private void Update()
  {
    if (!this.runFrameUpdate)
      return;
    using (TimeWarning.New("ServerMgr.Update", 500L))
    {
      if (EACServer.playerTracker != null)
        EACServer.playerTracker.BeginFrame();
      try
      {
        using (TimeWarning.New("Net.sv.Cycle", 100L))
          ((Network.Server) Network.Net.sv).Cycle();
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) "Server Exception: Network Cycle");
        Debug.LogException(ex, (Object) this);
      }
      try
      {
        using (TimeWarning.New("ServerBuildingManager.Cycle", 0.1f))
          BuildingManager.server.Cycle();
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) "Server Exception: Building Manager");
        Debug.LogException(ex, (Object) this);
      }
      try
      {
        using (TimeWarning.New("BasePlayer.ServerCycle", 0.1f))
          BasePlayer.ServerCycle(Time.get_deltaTime());
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) "Server Exception: Player Update");
        Debug.LogException(ex, (Object) this);
      }
      try
      {
        using (TimeWarning.New("SteamQueryResponse", 0.1f))
          this.SteamQueryResponse();
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) "Server Exception: Steam Query");
        Debug.LogException(ex, (Object) this);
      }
      try
      {
        using (TimeWarning.New("connectionQueue.Cycle", 0.1f))
          this.connectionQueue.Cycle(ServerMgr.AvailableSlots);
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) "Server Exception: Connection Queue");
        Debug.LogException(ex, (Object) this);
      }
      try
      {
        using (TimeWarning.New("IOEntity.ProcessQueue", 0.1f))
          IOEntity.ProcessQueue();
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) "Server Exception: IOEntity.ProcessQueue");
        Debug.LogException(ex, (Object) this);
      }
      try
      {
        using (TimeWarning.New("AIThinkManager.ProcessQueue", 0.1f))
          AIThinkManager.ProcessQueue();
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) "Server Exception: AIThinkManager.ProcessQueue");
        Debug.LogException(ex, (Object) this);
      }
      if (EACServer.playerTracker == null)
        return;
      EACServer.playerTracker.EndFrame();
    }
  }

  private void SteamQueryResponse()
  {
    if (Global.get_SteamServer() == null)
      return;
    using (TimeWarning.New("SteamGameServer.GetNextOutgoingPacket", 0.1f))
    {
      ServerQuery.Packet packet;
      while (Global.get_SteamServer().get_Query().GetOutgoingPacket(ref packet))
        ((Network.Server) Network.Net.sv).SendUnconnected(((ServerQuery.Packet) ref packet).get_Address(), ((ServerQuery.Packet) ref packet).get_Port(), ((ServerQuery.Packet) ref packet).get_Data(), ((ServerQuery.Packet) ref packet).get_Size());
    }
  }

  private void DoTick()
  {
    if (Global.get_SteamServer() != null)
    {
      Interface.CallHook("OnTick");
      ((BaseSteamworks) Global.get_SteamServer()).Update();
    }
    RCon.Update();
    for (int index = 0; index < ((List<Network.Connection>) ((Network.Server) Network.Net.sv).connections).Count; ++index)
    {
      Network.Connection connection = ((List<Network.Connection>) ((Network.Server) Network.Net.sv).connections)[index];
      if (!connection.get_isAuthenticated() && (double) connection.GetSecondsConnected() >= (double) ConVar.Server.authtimeout)
        ((Network.Server) Network.Net.sv).Kick(connection, "Authentication Timed Out");
    }
  }

  private void DoHeartbeat()
  {
    ItemManager.Heartbeat();
  }

  private string AssemblyHash
  {
    get
    {
      if (this._AssemblyHash == null)
      {
        byte[] numArray = System.IO.File.ReadAllBytes(typeof (ServerMgr).Assembly.Location);
        CRC32 crC32 = new CRC32();
        crC32.SlurpBlock(numArray, 0, numArray.Length);
        this._AssemblyHash = crC32.get_Crc32Result().ToString("x");
      }
      return this._AssemblyHash;
    }
  }

  private void UpdateServerInformation()
  {
    if (Global.get_SteamServer() == null)
      return;
    using (TimeWarning.New(nameof (UpdateServerInformation), 0.1f))
    {
      Global.get_SteamServer().set_ServerName(ConVar.Server.hostname);
      Global.get_SteamServer().set_MaxPlayers(ConVar.Server.maxplayers);
      Global.get_SteamServer().set_Passworded(false);
      Global.get_SteamServer().set_MapName(World.Name);
      string str1 = "stok";
      if (this.Restarting)
        str1 = "strst";
      string str2 = string.Format("born{0}", (object) Epoch.FromDateTime(SaveRestore.SaveCreatedTime));
      string str3 = string.Format("mp{0},cp{1},qp{5},v{2}{3},h{4},{6},{7}", (object) ConVar.Server.maxplayers, (object) BasePlayer.activePlayerList.Count, (object) 2161, ConVar.Server.pve ? (object) ",pve" : (object) string.Empty, (object) this.AssemblyHash, (object) ((ServerMgr) SingletonComponent<ServerMgr>.Instance).connectionQueue.Queued, (object) str1, (object) str2) + ",oxide";
      if (Interface.get_Oxide().get_Config().get_Options().Modded != null)
        str3 += ",modded";
      Global.get_SteamServer().set_GameTags(str3);
      if (ConVar.Server.description != null && ConVar.Server.description.Length > 100)
      {
        string[] array = StringEx.SplitToChunks(ConVar.Server.description, 100).ToArray<string>();
        for (int index = 0; index < 16; ++index)
        {
          if (index < array.Length)
            Global.get_SteamServer().SetKey(string.Format("description_{0:00}", (object) index), array[index]);
          else
            Global.get_SteamServer().SetKey(string.Format("description_{0:00}", (object) index), string.Empty);
        }
      }
      else
      {
        Global.get_SteamServer().SetKey("description_0", ConVar.Server.description);
        for (int index = 1; index < 16; ++index)
          Global.get_SteamServer().SetKey(string.Format("description_{0:00}", (object) index), string.Empty);
      }
      Global.get_SteamServer().SetKey("hash", this.AssemblyHash);
      Facepunch.Steamworks.Server steamServer1 = Global.get_SteamServer();
      uint num = World.Seed;
      string str4 = num.ToString();
      steamServer1.SetKey("world.seed", str4);
      Facepunch.Steamworks.Server steamServer2 = Global.get_SteamServer();
      num = World.Size;
      string str5 = num.ToString();
      steamServer2.SetKey("world.size", str5);
      Global.get_SteamServer().SetKey("pve", ConVar.Server.pve.ToString());
      Global.get_SteamServer().SetKey("headerimage", ConVar.Server.headerimage);
      Global.get_SteamServer().SetKey("url", ConVar.Server.url);
      Global.get_SteamServer().SetKey("uptime", ((int) Time.get_realtimeSinceStartup()).ToString());
      Global.get_SteamServer().SetKey("gc_mb", Performance.report.memoryAllocations.ToString());
      Global.get_SteamServer().SetKey("gc_cl", Performance.report.memoryCollections.ToString());
      Global.get_SteamServer().SetKey("fps", Performance.report.frameRate.ToString());
      Global.get_SteamServer().SetKey("fps_avg", Performance.report.frameRateAverage.ToString("0.00"));
      Global.get_SteamServer().SetKey("ent_cnt", BaseNetworkable.serverEntities.Count.ToString());
      Global.get_SteamServer().SetKey("build", BuildInfo.get_Current().get_Scm().get_ChangeId());
    }
  }

  public void OnDisconnected(string strReason, Network.Connection connection)
  {
    this.connectionQueue.RemoveConnection(connection);
    ConnectionAuth.OnDisconnect(connection);
    Global.get_SteamServer().get_Auth().EndSession((ulong) connection.userid);
    EACServer.OnLeaveGame(connection);
    BasePlayer player = connection.player as BasePlayer;
    if (!Object.op_Implicit((Object) player))
      return;
    Interface.CallHook("OnPlayerDisconnected", (object) player, (object) strReason);
    player.OnDisconnected();
  }

  public static void OnEnterVisibility(Network.Connection connection, Group group)
  {
    if (!((Network.Server) Network.Net.sv).IsConnected() || !((Write) ((NetworkPeer) Network.Net.sv).write).Start())
      return;
    ((Write) ((NetworkPeer) Network.Net.sv).write).PacketID((Message.Type) 19);
    ((Write) ((NetworkPeer) Network.Net.sv).write).GroupID((uint) group.ID);
    ((Write) ((NetworkPeer) Network.Net.sv).write).Send(new SendInfo(connection));
  }

  public static void OnLeaveVisibility(Network.Connection connection, Group group)
  {
    if (!((Network.Server) Network.Net.sv).IsConnected())
      return;
    if (((Write) ((NetworkPeer) Network.Net.sv).write).Start())
    {
      ((Write) ((NetworkPeer) Network.Net.sv).write).PacketID((Message.Type) 20);
      ((Write) ((NetworkPeer) Network.Net.sv).write).GroupID((uint) group.ID);
      ((Write) ((NetworkPeer) Network.Net.sv).write).Send(new SendInfo(connection));
    }
    if (!((Write) ((NetworkPeer) Network.Net.sv).write).Start())
      return;
    ((Write) ((NetworkPeer) Network.Net.sv).write).PacketID((Message.Type) 8);
    ((Write) ((NetworkPeer) Network.Net.sv).write).GroupID((uint) group.ID);
    ((Write) ((NetworkPeer) Network.Net.sv).write).Send(new SendInfo(connection));
  }

  internal void SpawnMapEntities()
  {
    PrefabPreProcess prefabPreProcess = new PrefabPreProcess(false, true, false);
    BaseEntity[] objectsOfType = (BaseEntity[]) Object.FindObjectsOfType<BaseEntity>();
    foreach (BaseEntity baseEntity in objectsOfType)
    {
      if (prefabPreProcess.NeedsProcessing(((Component) baseEntity).get_gameObject()))
        prefabPreProcess.ProcessObject((string) null, ((Component) baseEntity).get_gameObject(), false);
      baseEntity.SpawnAsMapEntity();
    }
    DebugEx.Log((object) ("Map Spawned " + (object) objectsOfType.Length + " entities"), (StackTraceLogType) 0);
    foreach (BaseEntity baseEntity in objectsOfType)
    {
      if (Object.op_Inequality((Object) baseEntity, (Object) null))
        baseEntity.PostMapEntitySpawn();
    }
  }

  public static BasePlayer.SpawnPoint FindSpawnPoint()
  {
    if (Object.op_Inequality((Object) SingletonComponent<SpawnHandler>.Instance, (Object) null))
    {
      BasePlayer.SpawnPoint spawnPoint = SpawnHandler.GetSpawnPoint();
      if (spawnPoint != null)
        return spawnPoint;
    }
    BasePlayer.SpawnPoint spawnPoint1 = new BasePlayer.SpawnPoint();
    GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("spawnpoint");
    if (gameObjectsWithTag.Length != 0)
    {
      GameObject gameObject = gameObjectsWithTag[Random.Range(0, gameObjectsWithTag.Length)];
      spawnPoint1.pos = gameObject.get_transform().get_position();
      spawnPoint1.rot = gameObject.get_transform().get_rotation();
    }
    else
    {
      Debug.Log((object) "Couldn't find an appropriate spawnpoint for the player - so spawning at camera");
      if (Object.op_Inequality((Object) MainCamera.mainCamera, (Object) null))
      {
        spawnPoint1.pos = ((Component) MainCamera.mainCamera).get_transform().get_position();
        spawnPoint1.rot = ((Component) MainCamera.mainCamera).get_transform().get_rotation();
      }
    }
    RaycastHit raycastHit;
    if (Physics.Raycast(new Ray(spawnPoint1.pos, Vector3.get_down()), ref raycastHit, 32f, 1537286401))
      spawnPoint1.pos = ((RaycastHit) ref raycastHit).get_point();
    return spawnPoint1;
  }

  public void JoinGame(Network.Connection connection)
  {
    using (Approval approval = (Approval) Pool.Get<Approval>())
    {
      uint num = (uint) ConVar.Server.encryption;
      if (num > 1U && (string) connection.os == "editor" && DeveloperList.Contains((ulong) connection.ownerid))
        num = 1U;
      approval.level = (__Null) Application.get_loadedLevelName();
      approval.levelUrl = (__Null) World.Url;
      approval.levelSeed = (__Null) (int) World.Seed;
      approval.levelSize = (__Null) (int) World.Size;
      approval.checksum = (__Null) World.Checksum;
      approval.hostname = (__Null) ConVar.Server.hostname;
      approval.official = (__Null) (ConVar.Server.official ? 1 : 0);
      approval.encryption = (__Null) (int) num;
      if (((Write) ((NetworkPeer) Network.Net.sv).write).Start())
      {
        ((Write) ((NetworkPeer) Network.Net.sv).write).PacketID((Message.Type) 3);
        approval.WriteToStream((Stream) ((NetworkPeer) Network.Net.sv).write);
        ((Write) ((NetworkPeer) Network.Net.sv).write).Send(new SendInfo(connection));
      }
      connection.encryptionLevel = (__Null) (int) num;
      connection.encryptOutgoing = (__Null) 1;
    }
    connection.connected = (__Null) 1;
  }

  public bool Restarting
  {
    get
    {
      return this.restartCoroutine != null;
    }
  }

  internal void Shutdown()
  {
    Interface.CallHook("OnServerShutdown");
    foreach (BasePlayer basePlayer in BasePlayer.activePlayerList.ToArray())
      basePlayer.Kick("Server Shutting Down");
    ConsoleSystem.Run(ConsoleSystem.Option.get_Server(), "server.save", (object[]) Array.Empty<object>());
    ConsoleSystem.Run(ConsoleSystem.Option.get_Server(), "server.writecfg", (object[]) Array.Empty<object>());
  }

  private IEnumerator ServerRestartWarning(string info, int iSeconds)
  {
    if (iSeconds >= 0)
    {
      if (!string.IsNullOrEmpty(info))
        ConsoleNetwork.BroadcastToAllClients("chat.add", (object) 0, (object) ("<color=#fff>SERVER</color> Restarting: " + info));
      for (int i = iSeconds; i > 0; --i)
      {
        if (i == iSeconds || i % 60 == 0 || i < 300 && i % 30 == 0 || (i < 60 && i % 10 == 0 || i < 10))
        {
          ConsoleNetwork.BroadcastToAllClients("chat.add", (object) 0, (object) ("<color=#fff>SERVER</color> Restarting in " + (object) i + " seconds!"));
          Debug.Log((object) ("Restarting in " + (object) i + " seconds"));
        }
        yield return (object) CoroutineEx.waitForSeconds(1f);
      }
      ConsoleNetwork.BroadcastToAllClients("chat.add", (object) 0, (object) "<color=#fff>SERVER</color> Restarting");
      yield return (object) CoroutineEx.waitForSeconds(2f);
      foreach (BasePlayer basePlayer in BasePlayer.activePlayerList.ToArray())
        basePlayer.Kick("Server Restarting");
      yield return (object) CoroutineEx.waitForSeconds(1f);
      ConsoleSystem.Run(ConsoleSystem.Option.get_Server(), "quit", (object[]) Array.Empty<object>());
    }
  }

  public static void RestartServer(string strNotice, int iSeconds)
  {
    if (Object.op_Equality((Object) SingletonComponent<ServerMgr>.Instance, (Object) null))
      return;
    if (((ServerMgr) SingletonComponent<ServerMgr>.Instance).restartCoroutine != null)
    {
      ConsoleNetwork.BroadcastToAllClients("chat.add", (object) 0, (object) "<color=#fff>SERVER</color> Restart interrupted!");
      ((MonoBehaviour) SingletonComponent<ServerMgr>.Instance).StopCoroutine(((ServerMgr) SingletonComponent<ServerMgr>.Instance).restartCoroutine);
      ((ServerMgr) SingletonComponent<ServerMgr>.Instance).restartCoroutine = (IEnumerator) null;
    }
    ((ServerMgr) SingletonComponent<ServerMgr>.Instance).restartCoroutine = ((ServerMgr) SingletonComponent<ServerMgr>.Instance).ServerRestartWarning(strNotice, iSeconds);
    ((MonoBehaviour) SingletonComponent<ServerMgr>.Instance).StartCoroutine(((ServerMgr) SingletonComponent<ServerMgr>.Instance).restartCoroutine);
    ((ServerMgr) SingletonComponent<ServerMgr>.Instance).UpdateServerInformation();
  }

  private void Log(Exception e)
  {
    if (Global.developer <= 0)
      return;
    Debug.LogException(e);
  }

  public void OnNetworkMessage(Message packet)
  {
    Message.Type type = (Message.Type) packet.type;
    if (type != 4)
    {
      switch (type - 9)
      {
        case 0:
          if (!((Network.Connection) packet.connection).get_isAuthenticated())
            break;
          if (((Network.Connection) packet.connection).GetPacketsPerSecond((Message.Type) packet.type) > (ulong) ConVar.Server.maxrpcspersecond)
          {
            ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Paket Flooding: RPC Message");
            break;
          }
          using (TimeWarning.New("OnRPCMessage", 20L))
          {
            try
            {
              this.OnRPCMessage(packet);
            }
            catch (Exception ex)
            {
              this.Log(ex);
              ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Packet: RPC Message");
            }
          }
          ((Network.Connection) packet.connection).AddPacketsPerSecond((Message.Type) packet.type);
          break;
        case 1:
        case 2:
        case 4:
label_84:
          this.ProcessUnhandledPacket(packet);
          break;
        case 3:
          if (!((Network.Connection) packet.connection).get_isAuthenticated())
            break;
          if (((Network.Connection) packet.connection).GetPacketsPerSecond((Message.Type) packet.type) > (ulong) ConVar.Server.maxcommandspersecond)
          {
            ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Packet Flooding: Client Command");
            break;
          }
          using (TimeWarning.New("OnClientCommand", 20L))
          {
            try
            {
              ConsoleNetwork.OnClientCommand(packet);
            }
            catch (Exception ex)
            {
              this.Log(ex);
              ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Packet: Client Command");
            }
          }
          ((Network.Connection) packet.connection).AddPacketsPerSecond((Message.Type) packet.type);
          break;
        case 5:
          if (!((Network.Connection) packet.connection).get_isAuthenticated())
            break;
          if (((Network.Connection) packet.connection).GetPacketsPerSecond((Message.Type) packet.type) > 1UL)
          {
            ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Packet Flooding: Disconnect Reason");
            break;
          }
          using (TimeWarning.New("ReadDisconnectReason", 20L))
          {
            try
            {
              this.ReadDisconnectReason(packet);
            }
            catch (Exception ex)
            {
              this.Log(ex);
              ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Packet: Disconnect Reason");
            }
          }
          ((Network.Connection) packet.connection).AddPacketsPerSecond((Message.Type) packet.type);
          break;
        case 6:
          if (!((Network.Connection) packet.connection).get_isAuthenticated())
            break;
          if (((Network.Connection) packet.connection).GetPacketsPerSecond((Message.Type) packet.type) > (ulong) ConVar.Server.maxtickspersecond)
          {
            ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Packet Flooding: Player Tick");
            break;
          }
          using (TimeWarning.New("OnPlayerTick", 20L))
          {
            try
            {
              this.OnPlayerTick(packet);
            }
            catch (Exception ex)
            {
              this.Log(ex);
              ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Packet: Player Tick");
            }
          }
          ((Network.Connection) packet.connection).AddPacketsPerSecond((Message.Type) packet.type);
          break;
        default:
          switch (type - 18)
          {
            case 0:
              if (((Network.Connection) packet.connection).GetPacketsPerSecond((Message.Type) packet.type) > 1UL)
              {
                ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Packet Flooding: User Information");
                return;
              }
              using (TimeWarning.New("GiveUserInformation", 20L))
              {
                try
                {
                  this.OnGiveUserInformation(packet);
                }
                catch (Exception ex)
                {
                  this.Log(ex);
                  ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Packet: User Information");
                }
              }
              ((Network.Connection) packet.connection).AddPacketsPerSecond((Message.Type) packet.type);
              return;
            case 3:
              if (!((Network.Connection) packet.connection).get_isAuthenticated())
                return;
              if (((Network.Connection) packet.connection).GetPacketsPerSecond((Message.Type) packet.type) > 100UL)
              {
                ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Packet Flooding: Disconnect Reason");
                return;
              }
              using (TimeWarning.New("OnPlayerVoice", 20L))
              {
                try
                {
                  this.OnPlayerVoice(packet);
                }
                catch (Exception ex)
                {
                  this.Log(ex);
                  ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Packet: Player Voice");
                }
              }
              ((Network.Connection) packet.connection).AddPacketsPerSecond((Message.Type) packet.type);
              return;
            case 4:
              using (TimeWarning.New("OnEACMessage", 20L))
              {
                try
                {
                  EACServer.OnMessageReceived(packet);
                  return;
                }
                catch (Exception ex)
                {
                  this.Log(ex);
                  ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Packet: EAC");
                  return;
                }
              }
            default:
              goto label_84;
          }
      }
    }
    else
    {
      if (!((Network.Connection) packet.connection).get_isAuthenticated())
        return;
      if (((Network.Connection) packet.connection).GetPacketsPerSecond((Message.Type) packet.type) > 1UL)
      {
        ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Packet Flooding: Client Ready");
      }
      else
      {
        using (TimeWarning.New("ClientReady", 20L))
        {
          try
          {
            this.ClientReady(packet);
          }
          catch (Exception ex)
          {
            this.Log(ex);
            ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Packet: Client Ready");
          }
        }
        ((Network.Connection) packet.connection).AddPacketsPerSecond((Message.Type) packet.type);
      }
    }
  }

  public void ProcessUnhandledPacket(Message packet)
  {
    Debug.LogWarning((object) ("[SERVER][UNHANDLED] " + (object) (Message.Type) packet.type));
    ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Sent Unhandled Message");
  }

  public void ReadDisconnectReason(Message packet)
  {
    string str1 = packet.get_read().String();
    string str2 = ((object) packet.connection).ToString();
    if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
      return;
    DebugEx.Log((object) (str2 + " disconnecting: " + str1), (StackTraceLogType) 0);
  }

  private bool SpawnPlayerSleeping(Network.Connection connection)
  {
    BasePlayer sleeping = BasePlayer.FindSleeping((ulong) connection.userid);
    if (Object.op_Equality((Object) sleeping, (Object) null))
      return false;
    if (!sleeping.IsSleeping())
    {
      Debug.LogWarning((object) "Player spawning into sleeper that isn't sleeping!");
      sleeping.Kill(BaseNetworkable.DestroyMode.None);
      return false;
    }
    sleeping.PlayerInit(connection);
    sleeping.inventory.SendSnapshot();
    DebugEx.Log((object) (((object) sleeping.net.get_connection()).ToString() + " joined [" + (object) sleeping.net.get_connection().os + "/" + (object) (ulong) sleeping.net.get_connection().ownerid + "]"), (StackTraceLogType) 0);
    return true;
  }

  private void SpawnNewPlayer(Network.Connection connection)
  {
    BasePlayer.SpawnPoint spawnPoint = ServerMgr.FindSpawnPoint();
    BasePlayer player = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", spawnPoint.pos, spawnPoint.rot, true).ToPlayer();
    if (Interface.CallHook("OnPlayerSpawn", (object) player) != null)
      return;
    player.health = 0.0f;
    player.lifestate = BaseCombatEntity.LifeState.Dead;
    player.ResetLifeStateOnSpawn = false;
    player.limitNetworking = true;
    player.Spawn();
    player.limitNetworking = false;
    player.PlayerInit(connection);
    if (SleepingBag.FindForPlayer(player.userID, true).Length == 0 && !player.hasPreviousLife)
      player.Respawn();
    else
      player.SendRespawnOptions();
    DebugEx.Log((object) (((object) player.net.get_connection()).ToString() + " joined [" + (object) player.net.get_connection().os + "/" + (object) (ulong) player.net.get_connection().ownerid + "]"), (StackTraceLogType) 0);
  }

  private void ClientReady(Message packet)
  {
    ((Network.Connection) packet.connection).decryptIncoming = (__Null) 1;
    using (ClientReady clientReady = ClientReady.Deserialize((Stream) packet.get_read()))
    {
      using (List<ClientReady.ClientInfo>.Enumerator enumerator = ((List<ClientReady.ClientInfo>) clientReady.clientInfo).GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          ClientReady.ClientInfo current = enumerator.Current;
          ((Network.Connection.ClientInfo) ((Network.Connection) packet.connection).info).Set((string) current.name, (string) current.value);
        }
      }
      this.connectionQueue.JoinedGame((Network.Connection) packet.connection);
      Interface.CallHook("OnPlayerConnected", (object) packet);
      using (TimeWarning.New(nameof (ClientReady), 0.1f))
      {
        using (TimeWarning.New("SpawnPlayerSleeping", 0.1f))
        {
          if (this.SpawnPlayerSleeping((Network.Connection) packet.connection))
            return;
        }
        using (TimeWarning.New("SpawnNewPlayer", 0.1f))
          this.SpawnNewPlayer((Network.Connection) packet.connection);
      }
    }
  }

  private void OnRPCMessage(Message packet)
  {
    uint uid = packet.get_read().UInt32();
    uint nameID = packet.get_read().UInt32();
    BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(uid) as BaseEntity;
    if (Object.op_Equality((Object) baseEntity, (Object) null))
      return;
    baseEntity.SV_RPCMessage(nameID, packet);
  }

  private void OnPlayerTick(Message packet)
  {
    BasePlayer basePlayer = packet.Player();
    if (Object.op_Equality((Object) basePlayer, (Object) null))
      return;
    basePlayer.OnReceivedTick((Stream) packet.get_read());
  }

  private void OnPlayerVoice(Message packet)
  {
    BasePlayer basePlayer = packet.Player();
    if (Object.op_Equality((Object) basePlayer, (Object) null))
      return;
    basePlayer.OnReceivedVoice(packet.get_read().BytesWithSize());
  }

  private void OnGiveUserInformation(Message packet)
  {
    if (((Network.Connection) packet.connection).state != null)
    {
      ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid connection state");
    }
    else
    {
      ((Network.Connection) packet.connection).state = (__Null) 1;
      if (packet.get_read().UInt8() != (byte) 228)
      {
        ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Connection Protocol");
      }
      else
      {
        ((Network.Connection) packet.connection).userid = (__Null) (long) packet.get_read().UInt64();
        ((Network.Connection) packet.connection).protocol = (__Null) (int) packet.get_read().UInt32();
        ((Network.Connection) packet.connection).os = (__Null) packet.get_read().String();
        ((Network.Connection) packet.connection).username = (__Null) packet.get_read().String();
        Interface.CallHook("OnClientAuth", (object) packet.connection);
        if (string.IsNullOrEmpty((string) ((Network.Connection) packet.connection).os))
          throw new Exception("Invalid OS");
        if (string.IsNullOrEmpty((string) ((Network.Connection) packet.connection).username))
        {
          ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Username");
        }
        else
        {
          ((Network.Connection) packet.connection).username = (__Null) ((string) ((Network.Connection) packet.connection).username).Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ').Trim();
          if (string.IsNullOrEmpty((string) ((Network.Connection) packet.connection).username))
          {
            ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Username");
          }
          else
          {
            string str = string.Empty;
            string branch = ConVar.Server.branch;
            if (packet.get_read().get_unread() >= 4)
              str = packet.get_read().String();
            if (branch != string.Empty && branch != str)
            {
              DebugEx.Log((object) ("Kicking " + (object) packet.connection + " - their branch is '" + str + "' not '" + branch + "'"), (StackTraceLogType) 0);
              ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Wrong Steam Beta: Requires '" + branch + "' branch!");
            }
            else if (((Network.Connection) packet.connection).protocol > 2161)
            {
              DebugEx.Log((object) ("Kicking " + (object) packet.connection + " - their protocol is " + (object) (uint) ((Network.Connection) packet.connection).protocol + " not " + (object) 2161), (StackTraceLogType) 0);
              ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Wrong Connection Protocol: Server update required!");
            }
            else if (((Network.Connection) packet.connection).protocol < 2161)
            {
              DebugEx.Log((object) ("Kicking " + (object) packet.connection + " - their protocol is " + (object) (uint) ((Network.Connection) packet.connection).protocol + " not " + (object) 2161), (StackTraceLogType) 0);
              ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Wrong Connection Protocol: Client update required!");
            }
            else
            {
              ((Network.Connection) packet.connection).token = (__Null) packet.get_read().BytesWithSize();
              if (((Network.Connection) packet.connection).token == null || ((Network.Connection) packet.connection).token.Length < 1)
                ((Network.Server) Network.Net.sv).Kick((Network.Connection) packet.connection, "Invalid Token");
              else
                this.auth.OnNewConnection((Network.Connection) packet.connection);
            }
          }
        }
      }
    }
  }

  public bool OnUnconnectedMessage(int type, Read read, uint ip, int port)
  {
    if (this.useQueryPort || type != (int) byte.MaxValue)
      return false;
    TimeSpan elapsed = this.queriesPerSeconTimer.Elapsed;
    if (elapsed.TotalSeconds > 1.0)
    {
      this.queriesPerSeconTimer.Reset();
      this.queriesPerSeconTimer.Start();
      this.NumQueriesLastSecond = 0;
    }
    if (this.NumQueriesLastSecond > ConVar.Server.queriesPerSecond || read.UInt8() != byte.MaxValue || (read.UInt8() != byte.MaxValue || read.UInt8() != byte.MaxValue))
      return false;
    elapsed = this.queryTimer.Elapsed;
    if (elapsed.TotalSeconds > 60.0)
    {
      this.queryTimer.Reset();
      this.queryTimer.Start();
      this.unconnectedQueries.Clear();
    }
    if (!this.unconnectedQueries.ContainsKey(ip))
      this.unconnectedQueries.Add(ip, 0);
    int num = this.unconnectedQueries[ip] + 1;
    this.unconnectedQueries[ip] = num;
    if (num > ConVar.Server.ipQueriesPerMin)
      return true;
    ++this.NumQueriesLastSecond;
    ((Stream) read).Position = 0L;
    int unread = read.get_unread();
    if (unread > 4096)
      return true;
    if (this.queryBuffer.Capacity < unread)
      this.queryBuffer.Capacity = unread;
    Global.get_SteamServer().get_Query().Handle(this.queryBuffer.GetBuffer(), ((Stream) read).Read(this.queryBuffer.GetBuffer(), 0, unread), ip, (ushort) port);
    return true;
  }

  public ServerMgr()
  {
    base.\u002Ector();
  }
}
