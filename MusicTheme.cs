// Decompiled with JetBrains decompiler
// Type: MusicTheme
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/MusicTheme")]
public class MusicTheme : ScriptableObject
{
  [Header("Basic info")]
  public float tempo;
  public int intensityHoldBars;
  public int lengthInBars;
  [Header("Playback restrictions")]
  public bool canPlayInMenus;
  [Horizontal(2, -1)]
  public MusicTheme.ValueRange rain;
  [Horizontal(2, -1)]
  public MusicTheme.ValueRange wind;
  [Horizontal(2, -1)]
  public MusicTheme.ValueRange snow;
  [InspectorFlags]
  public TerrainBiome.Enum biomes;
  [InspectorFlags]
  public TerrainTopology.Enum topologies;
  public AnimationCurve time;
  [Header("Clip data")]
  public List<MusicTheme.PositionedClip> clips;
  public List<MusicTheme.Layer> layers;
  private Dictionary<int, List<MusicTheme.PositionedClip>> activeClips;
  private List<AudioClip> firstAudioClips;
  private Dictionary<AudioClip, bool> audioClipDict;

  public int layerCount
  {
    get
    {
      return this.layers.Count;
    }
  }

  public int samplesPerBar
  {
    get
    {
      return MusicUtil.BarsToSamples(this.tempo, 1f, 44100);
    }
  }

  private void OnValidate()
  {
    this.audioClipDict.Clear();
    this.activeClips.Clear();
    this.UpdateLengthInBars();
    for (int index = 0; index < this.clips.Count; ++index)
    {
      MusicTheme.PositionedClip clip = this.clips[index];
      int num1 = this.ActiveClipCollectionID(clip.startingBar - 8);
      int num2 = this.ActiveClipCollectionID(clip.endingBar);
      for (int key = num1; key <= num2; ++key)
      {
        if (!this.activeClips.ContainsKey(key))
          this.activeClips.Add(key, new List<MusicTheme.PositionedClip>());
        if (!this.activeClips[key].Contains(clip))
          this.activeClips[key].Add(clip);
      }
      if (Object.op_Inequality((Object) clip.musicClip, (Object) null))
      {
        AudioClip audioClip = clip.musicClip.audioClip;
        if (!this.audioClipDict.ContainsKey(audioClip))
          this.audioClipDict.Add(audioClip, true);
        if (clip.startingBar < 8 && !this.firstAudioClips.Contains(audioClip))
          this.firstAudioClips.Add(audioClip);
        clip.musicClip.lengthInBarsWithTail = Mathf.CeilToInt(MusicUtil.SecondsToBars(this.tempo, (double) clip.musicClip.audioClip.get_length()));
      }
    }
  }

  public List<MusicTheme.PositionedClip> GetActiveClipsForBar(int bar)
  {
    int key = this.ActiveClipCollectionID(bar);
    if (!this.activeClips.ContainsKey(key))
      return (List<MusicTheme.PositionedClip>) null;
    return this.activeClips[key];
  }

  private int ActiveClipCollectionID(int bar)
  {
    return Mathf.FloorToInt(Mathf.Max((float) (bar / 4), 0.0f));
  }

  public MusicTheme.Layer LayerById(int id)
  {
    if (this.layers.Count <= id)
      return (MusicTheme.Layer) null;
    return this.layers[id];
  }

  public void AddLayer()
  {
    this.layers.Add(new MusicTheme.Layer()
    {
      name = "layer " + (object) this.layers.Count
    });
  }

  private void UpdateLengthInBars()
  {
    int num1 = 0;
    for (int index = 0; index < this.clips.Count; ++index)
    {
      MusicTheme.PositionedClip clip = this.clips[index];
      if (!Object.op_Equality((Object) clip.musicClip, (Object) null))
      {
        int num2 = clip.startingBar + clip.musicClip.lengthInBars;
        if (num2 > num1)
          num1 = num2;
      }
    }
    this.lengthInBars = num1;
  }

  public bool CanPlayInEnvironment(
    int currentBiome,
    int currentTopology,
    float currentRain,
    float currentSnow,
    float currentWind)
  {
    return (!Object.op_Implicit((Object) TOD_Sky.get_Instance()) || (double) this.time.Evaluate((float) ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).Hour) >= 0.0) && (this.biomes == -1 || (this.biomes & currentBiome) != null) && (this.topologies == -1 || (this.topologies & currentTopology) == null) && (((double) this.rain.min <= 0.0 && (double) this.rain.max >= 1.0 || (double) currentRain >= (double) this.rain.min) && (double) currentRain <= (double) this.rain.max) && (((double) this.snow.min <= 0.0 && (double) this.snow.max >= 1.0 || (double) currentSnow >= (double) this.snow.min) && (double) currentSnow <= (double) this.snow.max && (((double) this.wind.min <= 0.0 && (double) this.wind.max >= 1.0 || (double) currentWind >= (double) this.wind.min) && (double) currentWind <= (double) this.wind.max));
  }

  public bool FirstClipsLoaded()
  {
    for (int index = 0; index < this.firstAudioClips.Count; ++index)
    {
      if (this.firstAudioClips[index].get_loadState() != 2)
        return false;
    }
    return true;
  }

  public bool ContainsAudioClip(AudioClip clip)
  {
    return this.audioClipDict.ContainsKey(clip);
  }

  public MusicTheme()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class Layer
  {
    public string name = "layer";
  }

  [Serializable]
  public class PositionedClip
  {
    public float maxIntensity = 1f;
    public bool allowFadeIn = true;
    public bool allowFadeOut = true;
    public float fadeInTime = 1f;
    public float fadeOutTime = 0.5f;
    public float jumpMinimumIntensity = 0.5f;
    public float jumpMaximumIntensity = 0.5f;
    public MusicTheme theme;
    public MusicClip musicClip;
    public int startingBar;
    public int layerId;
    public float minIntensity;
    public float intensityReduction;
    public int jumpBarCount;

    public int endingBar
    {
      get
      {
        if (!Object.op_Equality((Object) this.musicClip, (Object) null))
          return this.startingBar + this.musicClip.lengthInBarsWithTail;
        return this.startingBar;
      }
    }

    public bool CanPlay(float intensity)
    {
      if ((double) intensity > (double) this.minIntensity || (double) this.minIntensity == 0.0 && (double) intensity == 0.0)
        return (double) intensity <= (double) this.maxIntensity;
      return false;
    }

    public bool isControlClip
    {
      get
      {
        return Object.op_Equality((Object) this.musicClip, (Object) null);
      }
    }

    public void CopySettingsFrom(MusicTheme.PositionedClip otherClip)
    {
      if (this.isControlClip != otherClip.isControlClip || otherClip == this)
        return;
      this.allowFadeIn = otherClip.allowFadeIn;
      this.fadeInTime = otherClip.fadeInTime;
      this.allowFadeOut = otherClip.allowFadeOut;
      this.fadeOutTime = otherClip.fadeOutTime;
      this.maxIntensity = otherClip.maxIntensity;
      this.minIntensity = otherClip.minIntensity;
      this.intensityReduction = otherClip.intensityReduction;
    }
  }

  [Serializable]
  public class ValueRange
  {
    public float min;
    public float max;

    public ValueRange(float min, float max)
    {
      this.min = min;
      this.max = max;
    }
  }
}
