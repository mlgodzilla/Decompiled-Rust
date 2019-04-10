// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.State.GAState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using GameAnalyticsSDK.Setup;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAnalyticsSDK.State
{
  internal static class GAState
  {
    private static Settings _settings;

    public static void Init()
    {
      try
      {
        GAState._settings = (Settings) Resources.Load("GameAnalytics/Settings", typeof (Settings));
      }
      catch (Exception ex)
      {
        Debug.Log((object) ("Could not get Settings during event validation \n" + ex.ToString()));
      }
    }

    private static bool ListContainsString(List<string> _list, string _string)
    {
      return _list.Contains(_string);
    }

    public static bool IsManualSessionHandlingEnabled()
    {
      return GAState._settings.UseManualSessionHandling;
    }

    public static bool HasAvailableResourceCurrency(string _currency)
    {
      return GAState.ListContainsString(GAState._settings.ResourceCurrencies, _currency);
    }

    public static bool HasAvailableResourceItemType(string _itemType)
    {
      return GAState.ListContainsString(GAState._settings.ResourceItemTypes, _itemType);
    }

    public static bool HasAvailableCustomDimensions01(string _dimension01)
    {
      return GAState.ListContainsString(GAState._settings.CustomDimensions01, _dimension01);
    }

    public static bool HasAvailableCustomDimensions02(string _dimension02)
    {
      return GAState.ListContainsString(GAState._settings.CustomDimensions02, _dimension02);
    }

    public static bool HasAvailableCustomDimensions03(string _dimension03)
    {
      return GAState.ListContainsString(GAState._settings.CustomDimensions03, _dimension03);
    }
  }
}
