// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Setup.Settings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAnalyticsSDK.Setup
{
  public class Settings : ScriptableObject
  {
    [HideInInspector]
    public static string VERSION;
    [HideInInspector]
    public static bool CheckingForUpdates;
    public int TotalMessagesSubmitted;
    public int TotalMessagesFailed;
    public int DesignMessagesSubmitted;
    public int DesignMessagesFailed;
    public int QualityMessagesSubmitted;
    public int QualityMessagesFailed;
    public int ErrorMessagesSubmitted;
    public int ErrorMessagesFailed;
    public int BusinessMessagesSubmitted;
    public int BusinessMessagesFailed;
    public int UserMessagesSubmitted;
    public int UserMessagesFailed;
    public string CustomArea;
    [SerializeField]
    private List<string> gameKey;
    [SerializeField]
    private List<string> secretKey;
    [SerializeField]
    public List<string> Build;
    [SerializeField]
    public List<string> SelectedPlatformStudio;
    [SerializeField]
    public List<string> SelectedPlatformGame;
    [SerializeField]
    public List<int> SelectedPlatformGameID;
    [SerializeField]
    public List<int> SelectedStudio;
    [SerializeField]
    public List<int> SelectedGame;
    public string NewVersion;
    public string Changes;
    public bool SignUpOpen;
    public string StudioName;
    public string GameName;
    public string EmailGA;
    [NonSerialized]
    public string PasswordGA;
    [NonSerialized]
    public string TokenGA;
    [NonSerialized]
    public string ExpireTime;
    [NonSerialized]
    public string LoginStatus;
    [NonSerialized]
    public bool JustSignedUp;
    [NonSerialized]
    public bool HideSignupWarning;
    public bool IntroScreen;
    [NonSerialized]
    public List<Studio> Studios;
    public bool InfoLogEditor;
    public bool InfoLogBuild;
    public bool VerboseLogBuild;
    public bool UseManualSessionHandling;
    public bool SendExampleGameDataToMyGame;
    public bool InternetConnectivity;
    public List<string> CustomDimensions01;
    public List<string> CustomDimensions02;
    public List<string> CustomDimensions03;
    public List<string> ResourceItemTypes;
    public List<string> ResourceCurrencies;
    public RuntimePlatform LastCreatedGamePlatform;
    public List<RuntimePlatform> Platforms;
    public Settings.InspectorStates CurrentInspectorState;
    public List<Settings.HelpTypes> ClosedHints;
    public bool DisplayHints;
    public Vector2 DisplayHintsScrollState;
    public Texture2D Logo;
    public Texture2D UpdateIcon;
    public Texture2D InfoIcon;
    public Texture2D DeleteIcon;
    public Texture2D GameIcon;
    public Texture2D HomeIcon;
    public Texture2D InstrumentIcon;
    public Texture2D QuestionIcon;
    public Texture2D UserIcon;
    public Texture2D AmazonIcon;
    public Texture2D GooglePlayIcon;
    public Texture2D iosIcon;
    public Texture2D macIcon;
    public Texture2D windowsPhoneIcon;
    [NonSerialized]
    public GUIStyle SignupButton;
    public bool UsePlayerSettingsBuildNumber;
    public bool SubmitErrors;
    public int MaxErrorCount;
    public bool SubmitFpsAverage;
    public bool SubmitFpsCritical;
    public bool IncludeGooglePlay;
    public int FpsCriticalThreshold;
    public int FpsCirticalSubmitInterval;
    public List<bool> PlatformFoldOut;
    public bool CustomDimensions01FoldOut;
    public bool CustomDimensions02FoldOut;
    public bool CustomDimensions03FoldOut;
    public bool ResourceItemTypesFoldOut;
    public bool ResourceCurrenciesFoldOut;
    public static readonly RuntimePlatform[] AvailablePlatforms;

    public void SetCustomUserID(string customID)
    {
      int num = customID != string.Empty ? 1 : 0;
    }

    public void RemovePlatformAtIndex(int index)
    {
      if (index < 0 || index >= this.Platforms.Count)
        return;
      this.gameKey.RemoveAt(index);
      this.secretKey.RemoveAt(index);
      this.Build.RemoveAt(index);
      this.SelectedPlatformStudio.RemoveAt(index);
      this.SelectedPlatformGame.RemoveAt(index);
      this.SelectedPlatformGameID.RemoveAt(index);
      this.SelectedStudio.RemoveAt(index);
      this.SelectedGame.RemoveAt(index);
      this.PlatformFoldOut.RemoveAt(index);
      this.Platforms.RemoveAt(index);
    }

    public void AddPlatform(RuntimePlatform platform)
    {
      this.gameKey.Add("");
      this.secretKey.Add("");
      this.Build.Add("0.1");
      this.SelectedPlatformStudio.Add("");
      this.SelectedPlatformGame.Add("");
      this.SelectedPlatformGameID.Add(-1);
      this.SelectedStudio.Add(0);
      this.SelectedGame.Add(0);
      this.PlatformFoldOut.Add(true);
      this.Platforms.Add(platform);
    }

    public string[] GetAvailablePlatforms()
    {
      List<string> stringList = new List<string>();
      for (int index = 0; index < Settings.AvailablePlatforms.Length; ++index)
      {
        RuntimePlatform availablePlatform = (RuntimePlatform) (int) Settings.AvailablePlatforms[index];
        if (availablePlatform == 8)
        {
          if (!this.Platforms.Contains((RuntimePlatform) 31) && !this.Platforms.Contains(availablePlatform))
            stringList.Add(availablePlatform.ToString());
          else if (!this.Platforms.Contains(availablePlatform))
            stringList.Add(availablePlatform.ToString());
        }
        else if (availablePlatform == 31)
        {
          if (!this.Platforms.Contains((RuntimePlatform) 8) && !this.Platforms.Contains(availablePlatform))
            stringList.Add(availablePlatform.ToString());
          else if (!this.Platforms.Contains(availablePlatform))
            stringList.Add(availablePlatform.ToString());
        }
        else if (availablePlatform == 20)
        {
          if (!this.Platforms.Contains(availablePlatform))
            stringList.Add("WSA");
        }
        else if (!this.Platforms.Contains(availablePlatform))
          stringList.Add(availablePlatform.ToString());
      }
      return stringList.ToArray();
    }

    public bool IsGameKeyValid(int index, string value)
    {
      bool flag = true;
      for (int index1 = 0; index1 < this.Platforms.Count; ++index1)
      {
        if (index != index1 && value.Equals(this.gameKey[index1]))
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    public bool IsSecretKeyValid(int index, string value)
    {
      bool flag = true;
      for (int index1 = 0; index1 < this.Platforms.Count; ++index1)
      {
        if (index != index1 && value.Equals(this.secretKey[index1]))
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    public void UpdateGameKey(int index, string value)
    {
      if (!string.IsNullOrEmpty(value))
      {
        if (this.IsGameKeyValid(index, value))
        {
          this.gameKey[index] = value;
        }
        else
        {
          if (!this.gameKey[index].Equals(value))
            return;
          this.gameKey[index] = "";
        }
      }
      else
        this.gameKey[index] = value;
    }

    public void UpdateSecretKey(int index, string value)
    {
      if (!string.IsNullOrEmpty(value))
      {
        if (this.IsSecretKeyValid(index, value))
        {
          this.secretKey[index] = value;
        }
        else
        {
          if (!this.secretKey[index].Equals(value))
            return;
          this.secretKey[index] = "";
        }
      }
      else
        this.secretKey[index] = value;
    }

    public string GetGameKey(int index)
    {
      return this.gameKey[index];
    }

    public string GetSecretKey(int index)
    {
      return this.secretKey[index];
    }

    public void SetCustomArea(string customArea)
    {
    }

    public void SetKeys(string gamekey, string secretkey)
    {
    }

    public Settings()
    {
      base.\u002Ector();
    }

    static Settings()
    {
      // ISSUE: unable to decompile the method.
    }

    public enum HelpTypes
    {
      None,
      IncludeSystemSpecsHelp,
      ProvideCustomUserID,
    }

    public enum MessageTypes
    {
      None,
      Error,
      Info,
      Warning,
    }

    public struct HelpInfo
    {
      public string Message;
      public Settings.MessageTypes MsgType;
      public Settings.HelpTypes HelpType;
    }

    public enum InspectorStates
    {
      Account,
      Basic,
      Debugging,
      Pref,
    }
  }
}
