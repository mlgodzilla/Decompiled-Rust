// Decompiled with JetBrains decompiler
// Type: LevelManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelManager
{
  public static string CurrentLevelName;

  public static bool isLoaded
  {
    get
    {
      return LevelManager.CurrentLevelName != null && !(LevelManager.CurrentLevelName == "") && (!(LevelManager.CurrentLevelName == "UIScene") && !(LevelManager.CurrentLevelName == "Empty")) && (!(LevelManager.CurrentLevelName == "MenuBackground") && !(LevelManager.CurrentLevelName == "UIWorkshop"));
    }
  }

  public static bool IsValid(string strName)
  {
    return Application.CanStreamedLevelBeLoaded(strName);
  }

  public static void LoadLevel(string strName, bool keepLoadingScreenOpen = true)
  {
    if (strName == "proceduralmap")
      strName = "Procedural Map";
    LevelManager.CurrentLevelName = strName;
    ((Server) Net.sv).Reset();
    SceneManager.LoadScene(strName, (LoadSceneMode) 0);
  }

  public static IEnumerator LoadLevelAsync(string strName, bool keepLoadingScreenOpen = true)
  {
    LevelManager.CurrentLevelName = strName;
    ((Server) Net.sv).Reset();
    yield return (object) null;
    yield return (object) SceneManager.LoadSceneAsync(strName, (LoadSceneMode) 0);
    yield return (object) null;
    yield return (object) null;
  }

  public static void UnloadLevel()
  {
    LevelManager.CurrentLevelName = (string) null;
    Application.LoadLevel("Empty");
  }
}
