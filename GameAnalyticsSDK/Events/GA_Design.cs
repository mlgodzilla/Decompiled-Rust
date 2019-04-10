// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Events.GA_Design
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using GameAnalyticsSDK.Wrapper;
using System.Collections.Generic;

namespace GameAnalyticsSDK.Events
{
  public static class GA_Design
  {
    public static void NewEvent(
      string eventName,
      float eventValue,
      IDictionary<string, object> fields)
    {
      GA_Design.CreateNewEvent(eventName, new float?(eventValue), fields);
    }

    public static void NewEvent(string eventName, IDictionary<string, object> fields)
    {
      GA_Design.CreateNewEvent(eventName, new float?(), fields);
    }

    private static void CreateNewEvent(
      string eventName,
      float? eventValue,
      IDictionary<string, object> fields)
    {
      if (eventValue.HasValue)
        GA_Wrapper.AddDesignEvent(eventName, eventValue.Value, fields);
      else
        GA_Wrapper.AddDesignEvent(eventName, fields);
    }
  }
}
