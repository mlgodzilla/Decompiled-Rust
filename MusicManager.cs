// Decompiled with JetBrains decompiler
// Type: MusicManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : SingletonComponent<MusicManager>, IClientComponent
{
  public AudioMixerGroup mixerGroup;
  public List<MusicTheme> themes;
  public MusicTheme currentTheme;
  public List<AudioSource> sources;
  public double nextMusic;
  public double nextMusicFromIntensityRaise;
  [Range(0.0f, 1f)]
  public float intensity;
  public Dictionary<MusicTheme.PositionedClip, MusicManager.ClipPlaybackData> clipPlaybackData;
  public int holdIntensityUntilBar;
  public bool musicPlaying;
  public bool loadingFirstClips;
  public MusicTheme nextTheme;
  public double lastClipUpdate;
  public float clipUpdateInterval;
  public double themeStartTime;
  public int lastActiveClipRefresh;
  public int activeClipRefreshInterval;
  public bool forceThemeChange;
  public float randomIntensityJumpChance;
  public int clipScheduleBarsEarly;
  public List<MusicTheme.PositionedClip> activeClips;
  public List<MusicTheme.PositionedClip> activeMusicClips;
  public List<MusicTheme.PositionedClip> activeControlClips;
  public List<MusicZone> currentMusicZones;
  public int currentBar;
  public int barOffset;

  public double currentThemeTime
  {
    get
    {
      return AudioSettings.get_dspTime() - this.themeStartTime;
    }
  }

  public int themeBar
  {
    get
    {
      return this.currentBar + this.barOffset;
    }
  }

  public static void RaiseIntensityTo(float amount, int holdLengthBars = 0)
  {
  }

  public void StopMusic()
  {
  }

  public MusicManager()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class ClipPlaybackData
  {
    public AudioSource source;
    public MusicTheme.PositionedClip positionedClip;
    public bool isActive;
    public bool fadingIn;
    public bool fadingOut;
    public double fadeStarted;
    public bool needsSync;
  }
}
