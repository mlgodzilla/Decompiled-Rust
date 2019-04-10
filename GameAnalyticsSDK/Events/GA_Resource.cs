// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Events.GA_Resource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using GameAnalyticsSDK.Wrapper;
using System.Collections.Generic;

namespace GameAnalyticsSDK.Events
{
  public static class GA_Resource
  {
    public static void NewEvent(
      GAResourceFlowType flowType,
      string currency,
      float amount,
      string itemType,
      string itemId,
      IDictionary<string, object> fields)
    {
      GA_Wrapper.AddResourceEvent(flowType, currency, amount, itemType, itemId, fields);
    }
  }
}
