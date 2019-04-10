// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Events.GA_Debug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GameAnalyticsSDK.Events
{
  public static class GA_Debug
  {
    public static int MaxErrorCount = 10;
    private static int _errorCount = 0;
    private static bool _showLogOnGUI = false;
    public static List<string> Messages;

    public static void HandleLog(string logString, string stackTrace, LogType type)
    {
      if (logString.StartsWith("[Physics.PhysX] RigidBody::setRigidBodyFlag"))
        return;
      if (GA_Debug._showLogOnGUI)
      {
        if (GA_Debug.Messages == null)
          GA_Debug.Messages = new List<string>();
        GA_Debug.Messages.Add(logString);
      }
      if (!GameAnalytics.SettingsGA.SubmitErrors || GA_Debug._errorCount >= GA_Debug.MaxErrorCount || type == 3)
        return;
      if (string.IsNullOrEmpty(stackTrace))
        stackTrace = new StackTrace().ToString();
      ++GA_Debug._errorCount;
      string message = logString.Replace('"', '\'').Replace('\n', ' ').Replace('\r', ' ') + " " + stackTrace.Replace('"', '\'').Replace('\n', ' ').Replace('\r', ' ');
      if (message.Length > 8192)
        message = message.Substring(8192);
      GA_Debug.SubmitError(message, type);
    }

    private static void SubmitError(string message, LogType type)
    {
      GAErrorSeverity severity = GAErrorSeverity.Info;
      switch ((int) type)
      {
        case 0:
          severity = GAErrorSeverity.Error;
          break;
        case 1:
          severity = GAErrorSeverity.Info;
          break;
        case 2:
          severity = GAErrorSeverity.Warning;
          break;
        case 3:
          severity = GAErrorSeverity.Debug;
          break;
        case 4:
          severity = GAErrorSeverity.Critical;
          break;
      }
      GA_Error.NewEvent(severity, message, (IDictionary<string, object>) null);
    }

    public static void EnabledLog()
    {
      GA_Debug._showLogOnGUI = true;
    }
  }
}
