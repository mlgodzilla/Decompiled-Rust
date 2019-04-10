// Decompiled with JetBrains decompiler
// Type: Bootstrap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using Rust;
using Rust.Ai;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Bootstrap : SingletonComponent<Bootstrap>
{
  internal static bool bootstrapInitRun;
  public static bool isErrored;
  public string messageString;
  public GameObject errorPanel;
  public Text errorText;
  public Text statusText;

  public static bool needsSetup
  {
    get
    {
      return !Bootstrap.bootstrapInitRun;
    }
  }

  public static bool isPresent
  {
    get
    {
      return Bootstrap.bootstrapInitRun || ((IEnumerable<GameSetup>) Object.FindObjectsOfType<GameSetup>()).Count<GameSetup>() > 0;
    }
  }

  public static void RunDefaults()
  {
    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    Application.set_targetFrameRate(256);
    Time.set_fixedDeltaTime(1f / 16f);
    Time.set_maximumDeltaTime(0.5f);
  }

  public static void Init_Tier0()
  {
    Bootstrap.RunDefaults();
    GameSetup.RunOnce = true;
    Bootstrap.bootstrapInitRun = true;
    ConsoleSystem.Index.Initialize(ConsoleGen.All);
    UnityButtons.Register();
    Output.Install();
    Pool.ResizeBuffer<Networkable>(65536);
    Pool.ResizeBuffer<EntityLink>(65536);
    Pool.FillBuffer<Networkable>(int.MaxValue);
    Pool.FillBuffer<EntityLink>(int.MaxValue);
    Bootstrap.NetworkInit();
    Bootstrap.WriteToLog("Command Line: " + CommandLine.get_Full().Replace(CommandLine.GetSwitch("-rcon.password", CommandLine.GetSwitch("+rcon.password", "RCONPASSWORD")), "******"));
    Interface.Initialize();
  }

  public static void Init_Systems()
  {
    Application.Initialize((BaseIntegration) new Integration());
    Performance.GetMemoryUsage = (__Null) (() => SystemInfoEx.systemMemoryUsed);
  }

  public static void Init_Config()
  {
    ConsoleNetwork.Init();
    ConsoleSystem.UpdateValuesFromCommandLine();
    ConsoleSystem.Run(ConsoleSystem.Option.get_Server(), "server.readcfg", (object[]) Array.Empty<object>());
    ServerUsers.Load();
  }

  public static void NetworkInit()
  {
    Net.sv = (__Null) new Facepunch.Network.Raknet.Server();
  }

  private IEnumerator Start()
  {
    Bootstrap bootstrap = this;
    Bootstrap.WriteToLog("Bootstrap Startup");
    Bootstrap.WriteToLog(SystemInfoGeneralText.currentInfo);
    Texture.SetGlobalAnisotropicFilteringLimits(1, 16);
    yield return (object) ((MonoBehaviour) bootstrap).StartCoroutine(Bootstrap.LoadingUpdate("Loading Bundles"));
    FileSystem.Backend = (FileSystemBackend) new AssetBundleBackend("Bundles/Bundles");
    if (FileSystem.Backend.isError != null)
      bootstrap.ThrowError((string) FileSystem.Backend.loadingError);
    if (!Bootstrap.isErrored)
    {
      yield return (object) ((MonoBehaviour) bootstrap).StartCoroutine(Bootstrap.LoadingUpdate("Loading Game Manifest"));
      GameManifest.Load();
      yield return (object) ((MonoBehaviour) bootstrap).StartCoroutine(Bootstrap.LoadingUpdate("DONE!"));
      yield return (object) ((MonoBehaviour) bootstrap).StartCoroutine(Bootstrap.LoadingUpdate("Running Self Check"));
      SelfCheck.Run();
      if (!Bootstrap.isErrored)
      {
        yield return (object) ((MonoBehaviour) bootstrap).StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Tier0"));
        Bootstrap.Init_Tier0();
        ConsoleSystem.UpdateValuesFromCommandLine();
        yield return (object) ((MonoBehaviour) bootstrap).StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Systems"));
        Bootstrap.Init_Systems();
        yield return (object) ((MonoBehaviour) bootstrap).StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Config"));
        Bootstrap.Init_Config();
        if (!Bootstrap.isErrored)
        {
          yield return (object) ((MonoBehaviour) bootstrap).StartCoroutine(Bootstrap.LoadingUpdate("Loading Items"));
          ItemManager.Initialize();
          if (!Bootstrap.isErrored)
          {
            yield return (object) ((MonoBehaviour) bootstrap).StartCoroutine(bootstrap.DedicatedServerStartup());
            GameManager.Destroy(((Component) bootstrap).get_gameObject(), 0.0f);
          }
        }
      }
    }
  }

  private IEnumerator DedicatedServerStartup()
  {
    Bootstrap bootstrap = this;
    Application.isLoading = (__Null) 1;
    Bootstrap.WriteToLog("Skinnable Warmup");
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) CoroutineEx.waitForEndOfFrame;
    GameManifest.LoadAssets();
    Bootstrap.WriteToLog("Loading Scene");
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) CoroutineEx.waitForEndOfFrame;
    Physics.set_solverIterationCount(3);
    QualitySettings.SetQualityLevel(0);
    Object.DontDestroyOnLoad((Object) ((Component) bootstrap).get_gameObject());
    Object.DontDestroyOnLoad((Object) GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server_console.prefab", true));
    bootstrap.StartupShared();
    World.InitSize(ConVar.Server.worldsize);
    World.InitSeed(ConVar.Server.seed);
    World.InitSalt(ConVar.Server.salt);
    World.Url = ConVar.Server.levelurl;
    LevelManager.LoadLevel(ConVar.Server.level, true);
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) ((MonoBehaviour) bootstrap).StartCoroutine(FileSystem_Warmup.Run(0.2f, new Action<string>(Bootstrap.WriteToLog), "Asset Warmup ({0}/{1})"));
    yield return (object) ((MonoBehaviour) bootstrap).StartCoroutine(Bootstrap.StartServer(!CommandLine.HasSwitch("-skipload"), "", false));
    if (!Object.op_Implicit((Object) Object.FindObjectOfType<Performance>()))
      Object.DontDestroyOnLoad((Object) GameManager.server.CreatePrefab("assets/bundled/prefabs/system/performance.prefab", true));
    Pool.Clear();
    Rust.GC.Collect();
    Application.isLoading = (__Null) 0;
  }

  public static IEnumerator StartServer(
    bool doLoad,
    string saveFileOverride,
    bool allowOutOfDateSaves)
  {
    float timeScale = Time.get_timeScale();
    if (Time.pausewhileloading)
      Time.set_timeScale(0.0f);
    RCon.Initialize();
    BaseEntity.Query.Server = new BaseEntity.Query.EntityTree(8096f);
    if (Object.op_Implicit((Object) SingletonComponent<WorldSetup>.Instance))
      yield return (object) ((MonoBehaviour) SingletonComponent<WorldSetup>.Instance).StartCoroutine(((WorldSetup) SingletonComponent<WorldSetup>.Instance).InitCoroutine());
    if (Object.op_Implicit((Object) SingletonComponent<DynamicNavMesh>.Instance) && ((Behaviour) SingletonComponent<DynamicNavMesh>.Instance).get_enabled() && !AiManager.nav_disable)
      yield return (object) ((MonoBehaviour) SingletonComponent<DynamicNavMesh>.Instance).StartCoroutine(((DynamicNavMesh) SingletonComponent<DynamicNavMesh>.Instance).UpdateNavMeshAndWait());
    if (Object.op_Implicit((Object) SingletonComponent<AiManager>.Instance) && ((Behaviour) SingletonComponent<AiManager>.Instance).get_enabled())
    {
      ((AiManager) SingletonComponent<AiManager>.Instance).Initialize();
      if (!AiManager.nav_disable && AI.npc_enable && Object.op_Inequality((Object) TerrainMeta.Path, (Object) null))
      {
        foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
        {
          if (monument.HasNavmesh)
            yield return (object) monument.StartCoroutine(monument.GetMonumentNavMesh().UpdateNavMeshAndWait());
        }
      }
    }
    GameObject prefab = GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab", true);
    Object.DontDestroyOnLoad((Object) prefab);
    ServerMgr serverMgr = (ServerMgr) prefab.GetComponent<ServerMgr>();
    serverMgr.Initialize(doLoad, saveFileOverride, allowOutOfDateSaves, false);
    yield return (object) CoroutineEx.waitForSecondsRealtime(0.1f);
    ColliderGrid.RefreshAll();
    yield return (object) CoroutineEx.waitForSecondsRealtime(0.1f);
    SaveRestore.InitializeEntityLinks();
    yield return (object) CoroutineEx.waitForSecondsRealtime(0.1f);
    SaveRestore.InitializeEntitySupports();
    yield return (object) CoroutineEx.waitForSecondsRealtime(0.1f);
    SaveRestore.InitializeEntityConditionals();
    yield return (object) CoroutineEx.waitForSecondsRealtime(0.1f);
    ColliderGrid.RefreshAll();
    yield return (object) CoroutineEx.waitForSecondsRealtime(0.1f);
    SaveRestore.GetSaveCache();
    yield return (object) CoroutineEx.waitForSecondsRealtime(0.1f);
    serverMgr.OpenConnection();
    if (Time.pausewhileloading)
      Time.set_timeScale(timeScale);
    Bootstrap.WriteToLog("Server startup complete");
  }

  private void StartupShared()
  {
    Interface.CallHook("InitLogging");
    ItemManager.Initialize();
  }

  public void ThrowError(string error)
  {
    Debug.Log((object) ("ThrowError: " + error));
    this.errorPanel.SetActive(true);
    this.errorText.set_text(error);
    Bootstrap.isErrored = true;
  }

  public void ExitGame()
  {
    Debug.Log((object) "Exiting due to Exit Game button on bootstrap error panel");
    Application.Quit();
  }

  public static IEnumerator LoadingUpdate(string str)
  {
    if (Object.op_Implicit((Object) SingletonComponent<Bootstrap>.Instance))
    {
      ((Bootstrap) SingletonComponent<Bootstrap>.Instance).messageString = str;
      yield return (object) CoroutineEx.waitForEndOfFrame;
      yield return (object) CoroutineEx.waitForEndOfFrame;
    }
  }

  public static void WriteToLog(string str)
  {
    DebugEx.Log((object) str, (StackTraceLogType) 0);
  }

  public Bootstrap()
  {
    base.\u002Ector();
  }
}
