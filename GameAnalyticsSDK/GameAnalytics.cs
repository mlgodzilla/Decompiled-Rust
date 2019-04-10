// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.GameAnalytics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using GameAnalyticsSDK.Events;
using GameAnalyticsSDK.Net;
using GameAnalyticsSDK.Setup;
using GameAnalyticsSDK.State;
using GameAnalyticsSDK.Wrapper;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameAnalyticsSDK
{
  [RequireComponent(typeof (GA_SpecialEvents))]
  [ExecuteInEditMode]
  public class GameAnalytics : MonoBehaviour
  {
    private static Settings _settings;
    private static GameAnalytics _instance;
    private static bool _hasInitializeBeenCalled;

    public static Settings SettingsGA
    {
      get
      {
        if (Object.op_Equality((Object) GameAnalytics._settings, (Object) null))
          GameAnalytics.InitAPI();
        return GameAnalytics._settings;
      }
      private set
      {
        GameAnalytics._settings = value;
      }
    }

    public void Awake()
    {
      if (!Application.get_isPlaying())
        return;
      if (Object.op_Inequality((Object) GameAnalytics._instance, (Object) null))
      {
        Debug.LogWarning((object) "Destroying duplicate GameAnalytics object - only one is allowed per scene!");
        Object.Destroy((Object) ((Component) this).get_gameObject());
      }
      else
      {
        GameAnalytics._instance = this;
        Object.DontDestroyOnLoad((Object) ((Component) this).get_gameObject());
        // ISSUE: method pointer
        Application.add_logMessageReceived(new Application.LogCallback((object) null, __methodptr(HandleLog)));
        GameAnalytics.InternalInitialize();
      }
    }

    private void OnDestroy()
    {
      if (!Application.get_isPlaying() || !Object.op_Equality((Object) GameAnalytics._instance, (Object) this))
        return;
      GameAnalytics._instance = (GameAnalytics) null;
    }

    private void OnApplicationQuit()
    {
      if (GameAnalytics.SettingsGA.UseManualSessionHandling)
        return;
      GameAnalytics.OnStop();
    }

    private static void InitAPI()
    {
      try
      {
        GameAnalytics._settings = (Settings) Resources.Load("GameAnalytics/Settings", typeof (Settings));
        GAState.Init();
      }
      catch (Exception ex)
      {
        Debug.Log((object) ("Error getting Settings in InitAPI: " + ex.Message));
      }
    }

    private static void InternalInitialize()
    {
      if (!Application.get_isPlaying())
        return;
      if (GameAnalytics.SettingsGA.InfoLogBuild)
        GA_Setup.SetInfoLog(true);
      if (GameAnalytics.SettingsGA.VerboseLogBuild)
        GA_Setup.SetVerboseLog(true);
      GA_Wrapper.SetUnitySdkVersion("unity " + Settings.VERSION);
      GA_Wrapper.SetUnityEngineVersion("unity " + GameAnalytics.GetUnityVersion());
      GA_Wrapper.SetBuild(BuildInfo.get_Current().get_Scm().get_ChangeId());
      if (GameAnalytics.SettingsGA.CustomDimensions01.Count > 0)
        GA_Setup.SetAvailableCustomDimensions01(GameAnalytics.SettingsGA.CustomDimensions01);
      if (GameAnalytics.SettingsGA.CustomDimensions02.Count > 0)
        GA_Setup.SetAvailableCustomDimensions02(GameAnalytics.SettingsGA.CustomDimensions02);
      if (GameAnalytics.SettingsGA.CustomDimensions03.Count > 0)
        GA_Setup.SetAvailableCustomDimensions03(GameAnalytics.SettingsGA.CustomDimensions03);
      if (GameAnalytics.SettingsGA.ResourceItemTypes.Count > 0)
        GA_Setup.SetAvailableResourceItemTypes(GameAnalytics.SettingsGA.ResourceItemTypes);
      if (GameAnalytics.SettingsGA.ResourceCurrencies.Count > 0)
        GA_Setup.SetAvailableResourceCurrencies(GameAnalytics.SettingsGA.ResourceCurrencies);
      if (!GameAnalytics.SettingsGA.UseManualSessionHandling)
        return;
      GameAnalytics.SetEnabledManualSessionHandling(true);
    }

    public static void Initialize()
    {
      if (Application.get_isEditor())
      {
        GameAnalytics._hasInitializeBeenCalled = true;
      }
      else
      {
        GA_Wrapper.Initialize("ab258be05882d64cb4e285ac8e8110f2", "e9e34daf4b7aff6505eccfc99eef811136b4c96c");
        GameAnalytics._hasInitializeBeenCalled = true;
      }
    }

    public static void NewBusinessEvent(
      string currency,
      int amount,
      string itemType,
      string itemId,
      string cartType)
    {
      if (!GameAnalytics._hasInitializeBeenCalled)
        return;
      GA_Business.NewEvent(currency, amount, itemType, itemId, cartType, (IDictionary<string, object>) null);
    }

    public static void NewDesignEvent(string eventName)
    {
      if (!GameAnalytics._hasInitializeBeenCalled)
        return;
      GA_Design.NewEvent(eventName, (IDictionary<string, object>) null);
    }

    public static void NewDesignEvent(string eventName, float eventValue)
    {
      if (!GameAnalytics._hasInitializeBeenCalled)
        return;
      GA_Design.NewEvent(eventName, eventValue, (IDictionary<string, object>) null);
    }

    public static void NewProgressionEvent(
      GAProgressionStatus progressionStatus,
      string progression01)
    {
      if (!GameAnalytics._hasInitializeBeenCalled)
        return;
      GA_Progression.NewEvent(progressionStatus, progression01, (IDictionary<string, object>) null);
    }

    public static void NewProgressionEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02)
    {
      if (!GameAnalytics._hasInitializeBeenCalled)
        return;
      GA_Progression.NewEvent(progressionStatus, progression01, progression02, (IDictionary<string, object>) null);
    }

    public static void NewProgressionEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02,
      string progression03)
    {
      if (!GameAnalytics._hasInitializeBeenCalled)
        return;
      GA_Progression.NewEvent(progressionStatus, progression01, progression02, progression03, (IDictionary<string, object>) null);
    }

    public static void NewProgressionEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      int score)
    {
      if (!GameAnalytics._hasInitializeBeenCalled)
        return;
      GA_Progression.NewEvent(progressionStatus, progression01, score, (IDictionary<string, object>) null);
    }

    public static void NewProgressionEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02,
      int score)
    {
      if (!GameAnalytics._hasInitializeBeenCalled)
        return;
      GA_Progression.NewEvent(progressionStatus, progression01, progression02, score, (IDictionary<string, object>) null);
    }

    public static void NewProgressionEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02,
      string progression03,
      int score)
    {
      if (!GameAnalytics._hasInitializeBeenCalled)
        return;
      GA_Progression.NewEvent(progressionStatus, progression01, progression02, progression03, score, (IDictionary<string, object>) null);
    }

    public static void NewResourceEvent(
      GAResourceFlowType flowType,
      string currency,
      float amount,
      string itemType,
      string itemId)
    {
      if (!GameAnalytics._hasInitializeBeenCalled)
        return;
      GA_Resource.NewEvent(flowType, currency, amount, itemType, itemId, (IDictionary<string, object>) null);
    }

    public static void NewErrorEvent(GAErrorSeverity severity, string message)
    {
      if (!GameAnalytics._hasInitializeBeenCalled)
        return;
      GA_Error.NewEvent(severity, message, (IDictionary<string, object>) null);
    }

    public static void SetFacebookId(string facebookId)
    {
      GA_Setup.SetFacebookId(facebookId);
    }

    public static void SetGender(GAGender gender)
    {
      GA_Setup.SetGender(gender);
    }

    public static void SetBirthYear(int birthYear)
    {
      GA_Setup.SetBirthYear(birthYear);
    }

    public static void SetCustomId(string userId)
    {
      GA_Wrapper.SetCustomUserId(userId);
    }

    public static void SetEnabledManualSessionHandling(bool enabled)
    {
      GA_Wrapper.SetEnabledManualSessionHandling(enabled);
    }

    public static void StartSession()
    {
      GA_Wrapper.StartSession();
    }

    public static void EndSession()
    {
      GA_Wrapper.EndSession();
    }

    public static void SetCustomDimension01(string customDimension)
    {
      GA_Setup.SetCustomDimension01(customDimension);
    }

    public static void SetCustomDimension02(string customDimension)
    {
      GA_Setup.SetCustomDimension02(customDimension);
    }

    public static void SetCustomDimension03(string customDimension)
    {
      GA_Setup.SetCustomDimension03(customDimension);
    }

    public static event Action OnCommandCenterUpdatedEvent;

    public void OnCommandCenterUpdated()
    {
      if (GameAnalytics.OnCommandCenterUpdatedEvent == null)
        return;
      GameAnalytics.OnCommandCenterUpdatedEvent();
    }

    public static void CommandCenterUpdated()
    {
      if (GameAnalytics.OnCommandCenterUpdatedEvent == null)
        return;
      GameAnalytics.OnCommandCenterUpdatedEvent();
    }

    public static string GetCommandCenterValueAsString(string key)
    {
      return GameAnalytics.GetCommandCenterValueAsString(key, (string) null);
    }

    public static string GetCommandCenterValueAsString(string key, string defaultValue)
    {
      return GA_Wrapper.GetCommandCenterValueAsString(key, defaultValue);
    }

    public static bool IsCommandCenterReady()
    {
      return GA_Wrapper.IsCommandCenterReady();
    }

    public static string GetConfigurationsContentAsString()
    {
      return GA_Wrapper.GetConfigurationsContentAsString();
    }

    private static string GetUnityVersion()
    {
      string str = "";
      string[] strArray1 = Application.get_unityVersion().Split('.');
      for (int index = 0; index < strArray1.Length; ++index)
      {
        int result;
        if (int.TryParse(strArray1[index], out result))
        {
          str = index != 0 ? str + "." + strArray1[index] : strArray1[index];
        }
        else
        {
          string[] strArray2 = Regex.Split(strArray1[index], "[^\\d]+");
          if (strArray2.Length != 0 && int.TryParse(strArray2[0], out result))
            str = str + "." + strArray2[0];
        }
      }
      return str;
    }

    private static int GetPlatformIndex()
    {
      RuntimePlatform platform = Application.get_platform();
      return platform != 8 ? (platform != 31 ? (platform == 20 || platform == 19 || (platform == 18 || platform == 20) || (platform == 19 || platform == 18) ? GameAnalytics.SettingsGA.Platforms.IndexOf((RuntimePlatform) 20) : GameAnalytics.SettingsGA.Platforms.IndexOf(platform)) : (GameAnalytics.SettingsGA.Platforms.Contains(platform) ? GameAnalytics.SettingsGA.Platforms.IndexOf(platform) : GameAnalytics.SettingsGA.Platforms.IndexOf((RuntimePlatform) 8))) : (GameAnalytics.SettingsGA.Platforms.Contains(platform) ? GameAnalytics.SettingsGA.Platforms.IndexOf(platform) : GameAnalytics.SettingsGA.Platforms.IndexOf((RuntimePlatform) 31));
    }

    public static void SetBuildAllPlatforms(string build)
    {
      for (int index = 0; index < GameAnalytics.SettingsGA.Build.Count; ++index)
        GameAnalytics.SettingsGA.Build[index] = build;
    }

    public GameAnalytics()
    {
      base.\u002Ector();
    }
  }
}
