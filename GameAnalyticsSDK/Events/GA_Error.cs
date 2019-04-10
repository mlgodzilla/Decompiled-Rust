// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Events.GA_Error
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using GameAnalyticsSDK.Wrapper;
using System.Collections.Generic;

namespace GameAnalyticsSDK.Events
{
  public static class GA_Error
  {
    public static void NewEvent(
      GAErrorSeverity severity,
      string message,
      IDictionary<string, object> fields)
    {
      GA_Error.CreateNewEvent(severity, message, fields);
    }

    private static void CreateNewEvent(
      GAErrorSeverity severity,
      string message,
      IDictionary<string, object> fields)
    {
      GA_Wrapper.AddErrorEvent(severity, message, fields);
    }
  }
}
