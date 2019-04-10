// Decompiled with JetBrains decompiler
// Type: ConVar.Audio
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("audio")]
  public class Audio : ConsoleSystem
  {
    [ClientVar(Help = "Volume", Saved = true)]
    public static float master = 1f;
    [ClientVar(Help = "Volume", Saved = true)]
    public static float musicvolume = 1f;
    [ClientVar(Help = "Volume", Saved = true)]
    public static float musicvolumemenu = 1f;
    [ClientVar(Help = "Volume", Saved = true)]
    public static float game = 1f;
    [ClientVar(Help = "Volume", Saved = true)]
    public static float voices = 1f;
    [ClientVar(Help = "Ambience System")]
    public static bool ambience = true;
    [ClientVar(Help = "Max ms per frame to spend updating sounds")]
    public static float framebudget = 0.3f;
    [ClientVar(Help = "Use more advanced sound occlusion", Saved = true)]
    public static bool advancedocclusion = false;

    [ClientVar(Help = "Volume", Saved = true)]
    public static int speakers
    {
      get
      {
        return (int) AudioSettings.get_speakerMode();
      }
      set
      {
        value = Mathf.Clamp(value, 2, 7);
        AudioConfiguration configuration = AudioSettings.GetConfiguration();
        configuration.speakerMode = (__Null) value;
        using (TimeWarning.New("Audio Settings Reset", 0.25f))
          AudioSettings.Reset(configuration);
      }
    }

    public Audio()
    {
      base.\u002Ector();
    }
  }
}
