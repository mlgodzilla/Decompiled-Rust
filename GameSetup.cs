// Decompiled with JetBrains decompiler
// Type: GameSetup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using Rust;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour
{
  public static bool RunOnce;
  public bool startServer;
  public string clientConnectCommand;
  public bool loadMenu;
  public bool loadLevel;
  public string loadLevelScene;
  public bool loadSave;
  public string loadSaveFile;

  protected void Awake()
  {
    if (GameSetup.RunOnce)
    {
      GameManager.Destroy(((Component) this).get_gameObject(), 0.0f);
    }
    else
    {
      GameManifest.Load();
      GameManifest.LoadAssets();
      GameSetup.RunOnce = true;
      if (Bootstrap.needsSetup)
      {
        Bootstrap.Init_Tier0();
        Bootstrap.Init_Systems();
        Bootstrap.Init_Config();
      }
      this.StartCoroutine(this.DoGameSetup());
    }
  }

  private IEnumerator DoGameSetup()
  {
    GameSetup gameSetup = this;
    Application.isLoading = (__Null) 1;
    TerrainMeta.InitNoTerrain();
    ItemManager.Initialize();
    Scene activeScene = SceneManager.GetActiveScene();
    LevelManager.CurrentLevelName = ((Scene) ref activeScene).get_name();
    if (gameSetup.loadLevel && !string.IsNullOrEmpty(gameSetup.loadLevelScene))
    {
      ((Network.Server) Net.sv).Reset();
      ConVar.Server.level = gameSetup.loadLevelScene;
      LoadingScreen.Update("LOADING SCENE");
      Application.LoadLevelAdditive(gameSetup.loadLevelScene);
      LoadingScreen.Update(gameSetup.loadLevelScene.ToUpper() + " LOADED");
    }
    if (gameSetup.startServer)
      yield return (object) gameSetup.StartCoroutine(gameSetup.StartServer());
    yield return (object) null;
    Application.isLoading = (__Null) 0;
  }

  private IEnumerator StartServer()
  {
    GameSetup gameSetup = this;
    ConVar.GC.collect();
    ConVar.GC.unload();
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) CoroutineEx.waitForEndOfFrame;
    yield return (object) gameSetup.StartCoroutine(Bootstrap.StartServer(gameSetup.loadSave, gameSetup.loadSaveFile, true));
  }

  public GameSetup()
  {
    base.\u002Ector();
  }
}
