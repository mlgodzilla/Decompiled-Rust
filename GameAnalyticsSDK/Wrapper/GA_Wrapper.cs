// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Wrapper.GA_Wrapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using GameAnalyticsSDK.Net;
using GameAnalyticsSDK.State;
using GameAnalyticsSDK.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAnalyticsSDK.Wrapper
{
  public class GA_Wrapper
  {
    private static readonly GA_Wrapper.UnityCommandCenterListener unityCommandCenterListener = new GA_Wrapper.UnityCommandCenterListener();

    private static void configureAvailableCustomDimensions01(string list)
    {
      IList<object> objectList = GA_MiniJSON.Deserialize(list) as IList<object>;
      ArrayList arrayList = new ArrayList();
      foreach (object obj in (IEnumerable<object>) objectList)
        arrayList.Add(obj);
      GameAnalytics.ConfigureAvailableCustomDimensions01((string[]) arrayList.ToArray(typeof (string)));
    }

    private static void configureAvailableCustomDimensions02(string list)
    {
      IList<object> objectList = GA_MiniJSON.Deserialize(list) as IList<object>;
      ArrayList arrayList = new ArrayList();
      foreach (object obj in (IEnumerable<object>) objectList)
        arrayList.Add(obj);
      GameAnalytics.ConfigureAvailableCustomDimensions02((string[]) arrayList.ToArray(typeof (string)));
    }

    private static void configureAvailableCustomDimensions03(string list)
    {
      IList<object> objectList = GA_MiniJSON.Deserialize(list) as IList<object>;
      ArrayList arrayList = new ArrayList();
      foreach (object obj in (IEnumerable<object>) objectList)
        arrayList.Add(obj);
      GameAnalytics.ConfigureAvailableCustomDimensions03((string[]) arrayList.ToArray(typeof (string)));
    }

    private static void configureAvailableResourceCurrencies(string list)
    {
      IList<object> objectList = GA_MiniJSON.Deserialize(list) as IList<object>;
      ArrayList arrayList = new ArrayList();
      foreach (object obj in (IEnumerable<object>) objectList)
        arrayList.Add(obj);
      GameAnalytics.ConfigureAvailableResourceCurrencies((string[]) arrayList.ToArray(typeof (string)));
    }

    private static void configureAvailableResourceItemTypes(string list)
    {
      IList<object> objectList = GA_MiniJSON.Deserialize(list) as IList<object>;
      ArrayList arrayList = new ArrayList();
      foreach (object obj in (IEnumerable<object>) objectList)
        arrayList.Add(obj);
      GameAnalytics.ConfigureAvailableResourceItemTypes((string[]) arrayList.ToArray(typeof (string)));
    }

    private static void configureSdkGameEngineVersion(string unitySdkVersion)
    {
      GameAnalytics.ConfigureSdkGameEngineVersion(unitySdkVersion);
    }

    private static void configureGameEngineVersion(string unityEngineVersion)
    {
      GameAnalytics.ConfigureGameEngineVersion(unityEngineVersion);
    }

    private static void configureBuild(string build)
    {
      GameAnalytics.ConfigureBuild(build);
    }

    private static void configureUserId(string userId)
    {
      GameAnalytics.ConfigureUserId(userId);
    }

    private static void initialize(string gamekey, string gamesecret)
    {
      GameAnalytics.AddCommandCenterListener((ICommandCenterListener) GA_Wrapper.unityCommandCenterListener);
      GameAnalytics.Initialize(gamekey, gamesecret);
    }

    private static void setCustomDimension01(string customDimension)
    {
      GameAnalytics.SetCustomDimension01(customDimension);
    }

    private static void setCustomDimension02(string customDimension)
    {
      GameAnalytics.SetCustomDimension02(customDimension);
    }

    private static void setCustomDimension03(string customDimension)
    {
      GameAnalytics.SetCustomDimension03(customDimension);
    }

    private static void addBusinessEvent(
      string currency,
      int amount,
      string itemType,
      string itemId,
      string cartType,
      string fields)
    {
      GameAnalytics.AddBusinessEvent(currency, amount, itemType, itemId, cartType);
    }

    private static void addResourceEvent(
      int flowType,
      string currency,
      float amount,
      string itemType,
      string itemId,
      string fields)
    {
      GameAnalytics.AddResourceEvent((EGAResourceFlowType) flowType, currency, amount, itemType, itemId);
    }

    private static void addProgressionEvent(
      int progressionStatus,
      string progression01,
      string progression02,
      string progression03,
      string fields)
    {
      GameAnalytics.AddProgressionEvent((EGAProgressionStatus) progressionStatus, progression01, progression02, progression03);
    }

    private static void addProgressionEventWithScore(
      int progressionStatus,
      string progression01,
      string progression02,
      string progression03,
      int score,
      string fields)
    {
      GameAnalytics.AddProgressionEvent((EGAProgressionStatus) progressionStatus, progression01, progression02, progression03, (double) score);
    }

    private static void addDesignEvent(string eventId, string fields)
    {
      GameAnalytics.AddDesignEvent(eventId, (IDictionary<string, object>) null);
    }

    private static void addDesignEventWithValue(string eventId, float value, string fields)
    {
      GameAnalytics.AddDesignEvent(eventId, (double) value);
    }

    private static void addErrorEvent(int severity, string message, string fields)
    {
      GameAnalytics.AddErrorEvent((EGAErrorSeverity) severity, message);
    }

    private static void setEnabledInfoLog(bool enabled)
    {
      GameAnalytics.SetEnabledInfoLog(enabled);
    }

    private static void setEnabledVerboseLog(bool enabled)
    {
      GameAnalytics.SetEnabledVerboseLog(enabled);
    }

    private static void setManualSessionHandling(bool enabled)
    {
      GameAnalytics.SetEnabledManualSessionHandling(enabled);
    }

    private static void gameAnalyticsStartSession()
    {
      GameAnalytics.StartSession();
    }

    private static void gameAnalyticsEndSession()
    {
      GameAnalytics.EndSession();
    }

    private static void setFacebookId(string facebookId)
    {
      GameAnalytics.SetFacebookId(facebookId);
    }

    private static void setGender(string gender)
    {
      if (!(gender == "male"))
      {
        if (!(gender == "female"))
          return;
        GameAnalytics.SetGender((EGAGender) 2);
      }
      else
        GameAnalytics.SetGender((EGAGender) 1);
    }

    private static void setBirthYear(int birthYear)
    {
      GameAnalytics.SetBirthYear(birthYear);
    }

    private static string getCommandCenterValueAsString(string key, string defaultValue)
    {
      return GameAnalytics.GetCommandCenterValueAsString(key, defaultValue);
    }

    private static bool isCommandCenterReady()
    {
      return GameAnalytics.IsCommandCenterReady();
    }

    private static string getConfigurationsContentAsString()
    {
      return GameAnalytics.GetConfigurationsAsString();
    }

    public static void SetAvailableCustomDimensions01(string list)
    {
      GA_Wrapper.configureAvailableCustomDimensions01(list);
    }

    public static void SetAvailableCustomDimensions02(string list)
    {
      GA_Wrapper.configureAvailableCustomDimensions02(list);
    }

    public static void SetAvailableCustomDimensions03(string list)
    {
      GA_Wrapper.configureAvailableCustomDimensions03(list);
    }

    public static void SetAvailableResourceCurrencies(string list)
    {
      GA_Wrapper.configureAvailableResourceCurrencies(list);
    }

    public static void SetAvailableResourceItemTypes(string list)
    {
      GA_Wrapper.configureAvailableResourceItemTypes(list);
    }

    public static void SetUnitySdkVersion(string unitySdkVersion)
    {
      GA_Wrapper.configureSdkGameEngineVersion(unitySdkVersion);
    }

    public static void SetUnityEngineVersion(string unityEngineVersion)
    {
      GA_Wrapper.configureGameEngineVersion(unityEngineVersion);
    }

    public static void SetBuild(string build)
    {
      GA_Wrapper.configureBuild(build);
    }

    public static void SetCustomUserId(string userId)
    {
      GA_Wrapper.configureUserId(userId);
    }

    public static void SetEnabledManualSessionHandling(bool enabled)
    {
      GA_Wrapper.setManualSessionHandling(enabled);
    }

    public static void StartSession()
    {
      if (GAState.IsManualSessionHandlingEnabled())
        GA_Wrapper.gameAnalyticsStartSession();
      else
        Debug.Log((object) "Manual session handling is not enabled. \nPlease check the \"Use manual session handling\" option in the \"Advanced\" section of the Settings object.");
    }

    public static void EndSession()
    {
      if (GAState.IsManualSessionHandlingEnabled())
        GA_Wrapper.gameAnalyticsEndSession();
      else
        Debug.Log((object) "Manual session handling is not enabled. \nPlease check the \"Use manual session handling\" option in the \"Advanced\" section of the Settings object.");
    }

    public static void Initialize(string gamekey, string gamesecret)
    {
      GA_Wrapper.initialize(gamekey, gamesecret);
    }

    public static void SetCustomDimension01(string customDimension)
    {
      GA_Wrapper.setCustomDimension01(customDimension);
    }

    public static void SetCustomDimension02(string customDimension)
    {
      GA_Wrapper.setCustomDimension02(customDimension);
    }

    public static void SetCustomDimension03(string customDimension)
    {
      GA_Wrapper.setCustomDimension03(customDimension);
    }

    public static void AddBusinessEvent(
      string currency,
      int amount,
      string itemType,
      string itemId,
      string cartType,
      IDictionary<string, object> fields)
    {
      string jsonString = GA_Wrapper.DictionaryToJsonString(fields);
      GA_Wrapper.addBusinessEvent(currency, amount, itemType, itemId, cartType, jsonString);
    }

    public static void AddResourceEvent(
      GAResourceFlowType flowType,
      string currency,
      float amount,
      string itemType,
      string itemId,
      IDictionary<string, object> fields)
    {
      string jsonString = GA_Wrapper.DictionaryToJsonString(fields);
      GA_Wrapper.addResourceEvent((int) flowType, currency, amount, itemType, itemId, jsonString);
    }

    public static void AddProgressionEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02,
      string progression03,
      IDictionary<string, object> fields)
    {
      string jsonString = GA_Wrapper.DictionaryToJsonString(fields);
      GA_Wrapper.addProgressionEvent((int) progressionStatus, progression01, progression02, progression03, jsonString);
    }

    public static void AddProgressionEventWithScore(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02,
      string progression03,
      int score,
      IDictionary<string, object> fields)
    {
      string jsonString = GA_Wrapper.DictionaryToJsonString(fields);
      GA_Wrapper.addProgressionEventWithScore((int) progressionStatus, progression01, progression02, progression03, score, jsonString);
    }

    public static void AddDesignEvent(
      string eventID,
      float eventValue,
      IDictionary<string, object> fields)
    {
      string jsonString = GA_Wrapper.DictionaryToJsonString(fields);
      GA_Wrapper.addDesignEventWithValue(eventID, eventValue, jsonString);
    }

    public static void AddDesignEvent(string eventID, IDictionary<string, object> fields)
    {
      string jsonString = GA_Wrapper.DictionaryToJsonString(fields);
      GA_Wrapper.addDesignEvent(eventID, jsonString);
    }

    public static void AddErrorEvent(
      GAErrorSeverity severity,
      string message,
      IDictionary<string, object> fields)
    {
      string jsonString = GA_Wrapper.DictionaryToJsonString(fields);
      GA_Wrapper.addErrorEvent((int) severity, message, jsonString);
    }

    public static void SetInfoLog(bool enabled)
    {
      GA_Wrapper.setEnabledInfoLog(enabled);
    }

    public static void SetVerboseLog(bool enabled)
    {
      GA_Wrapper.setEnabledVerboseLog(enabled);
    }

    public static void SetFacebookId(string facebookId)
    {
      GA_Wrapper.setFacebookId(facebookId);
    }

    public static void SetGender(string gender)
    {
      GA_Wrapper.setGender(gender);
    }

    public static void SetBirthYear(int birthYear)
    {
      GA_Wrapper.setBirthYear(birthYear);
    }

    public static string GetCommandCenterValueAsString(string key, string defaultValue)
    {
      return GA_Wrapper.getCommandCenterValueAsString(key, defaultValue);
    }

    public static bool IsCommandCenterReady()
    {
      return GA_Wrapper.isCommandCenterReady();
    }

    public static string GetConfigurationsContentAsString()
    {
      return GA_Wrapper.getConfigurationsContentAsString();
    }

    private static string DictionaryToJsonString(IDictionary<string, object> dict)
    {
      Hashtable hashtable = new Hashtable();
      if (dict != null)
      {
        foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) dict)
          hashtable.Add((object) keyValuePair.Key, keyValuePair.Value);
      }
      return GA_MiniJSON.Serialize((object) hashtable);
    }

    private class UnityCommandCenterListener : ICommandCenterListener
    {
      public void OnCommandCenterUpdated()
      {
        GameAnalytics.CommandCenterUpdated();
      }
    }
  }
}
