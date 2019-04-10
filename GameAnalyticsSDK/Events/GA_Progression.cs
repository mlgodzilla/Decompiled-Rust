// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Events.GA_Progression
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using GameAnalyticsSDK.Wrapper;
using System.Collections.Generic;

namespace GameAnalyticsSDK.Events
{
  public static class GA_Progression
  {
    public static void NewEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      IDictionary<string, object> fields)
    {
      GA_Progression.CreateEvent(progressionStatus, progression01, (string) null, (string) null, new int?(), fields);
    }

    public static void NewEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02,
      IDictionary<string, object> fields)
    {
      GA_Progression.CreateEvent(progressionStatus, progression01, progression02, (string) null, new int?(), fields);
    }

    public static void NewEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02,
      string progression03,
      IDictionary<string, object> fields)
    {
      GA_Progression.CreateEvent(progressionStatus, progression01, progression02, progression03, new int?(), fields);
    }

    public static void NewEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      int score,
      IDictionary<string, object> fields)
    {
      GA_Progression.CreateEvent(progressionStatus, progression01, (string) null, (string) null, new int?(score), fields);
    }

    public static void NewEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02,
      int score,
      IDictionary<string, object> fields)
    {
      GA_Progression.CreateEvent(progressionStatus, progression01, progression02, (string) null, new int?(score), fields);
    }

    public static void NewEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02,
      string progression03,
      int score,
      IDictionary<string, object> fields)
    {
      GA_Progression.CreateEvent(progressionStatus, progression01, progression02, progression03, new int?(score), fields);
    }

    private static void CreateEvent(
      GAProgressionStatus progressionStatus,
      string progression01,
      string progression02,
      string progression03,
      int? score,
      IDictionary<string, object> fields)
    {
      if (score.HasValue)
        GA_Wrapper.AddProgressionEventWithScore(progressionStatus, progression01, progression02, progression03, score.Value, fields);
      else
        GA_Wrapper.AddProgressionEvent(progressionStatus, progression01, progression02, progression03, fields);
    }
  }
}
