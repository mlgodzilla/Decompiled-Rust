// Decompiled with JetBrains decompiler
// Type: MusicClipLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

public class MusicClipLoader
{
  public List<MusicClipLoader.LoadedAudioClip> loadedClips = new List<MusicClipLoader.LoadedAudioClip>();
  public Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip> loadedClipDict = new Dictionary<AudioClip, MusicClipLoader.LoadedAudioClip>();
  public List<AudioClip> clipsToLoad = new List<AudioClip>();
  public List<AudioClip> clipsToUnload = new List<AudioClip>();

  public void Update()
  {
    for (int index = this.clipsToLoad.Count - 1; index >= 0; --index)
    {
      AudioClip audioClip = this.clipsToLoad[index];
      if (audioClip.get_loadState() != 2 && audioClip.get_loadState() != 1)
      {
        audioClip.LoadAudioData();
        this.clipsToLoad.RemoveAt(index);
        return;
      }
    }
    for (int index = this.clipsToUnload.Count - 1; index >= 0; --index)
    {
      AudioClip audioClip = this.clipsToUnload[index];
      if (audioClip.get_loadState() == 2)
      {
        audioClip.UnloadAudioData();
        this.clipsToUnload.RemoveAt(index);
        break;
      }
    }
  }

  public void Refresh()
  {
    for (int index = 0; index < ((MusicManager) SingletonComponent<MusicManager>.Instance).activeMusicClips.Count; ++index)
    {
      MusicTheme.PositionedClip activeMusicClip = ((MusicManager) SingletonComponent<MusicManager>.Instance).activeMusicClips[index];
      MusicClipLoader.LoadedAudioClip loadedClip = this.FindLoadedClip(activeMusicClip.musicClip.audioClip);
      if (loadedClip == null)
      {
        MusicClipLoader.LoadedAudioClip loadedAudioClip = (MusicClipLoader.LoadedAudioClip) Pool.Get<MusicClipLoader.LoadedAudioClip>();
        loadedAudioClip.clip = activeMusicClip.musicClip.audioClip;
        loadedAudioClip.unloadTime = (float) (AudioSettings.get_dspTime() + (double) loadedAudioClip.clip.get_length() + 1.0);
        this.loadedClips.Add(loadedAudioClip);
        this.loadedClipDict.Add(loadedAudioClip.clip, loadedAudioClip);
        this.clipsToLoad.Add(loadedAudioClip.clip);
      }
      else
      {
        loadedClip.unloadTime = (float) (AudioSettings.get_dspTime() + (double) loadedClip.clip.get_length() + 1.0);
        this.clipsToUnload.Remove(loadedClip.clip);
      }
    }
    for (int index = this.loadedClips.Count - 1; index >= 0; --index)
    {
      MusicClipLoader.LoadedAudioClip loadedClip = this.loadedClips[index];
      if (AudioSettings.get_dspTime() > (double) loadedClip.unloadTime)
      {
        this.clipsToUnload.Add(loadedClip.clip);
        this.loadedClips.Remove(loadedClip);
        this.loadedClipDict.Remove(loadedClip.clip);
        // ISSUE: cast to a reference type
        Pool.Free<MusicClipLoader.LoadedAudioClip>((M0&) ref loadedClip);
      }
    }
  }

  private MusicClipLoader.LoadedAudioClip FindLoadedClip(AudioClip clip)
  {
    if (this.loadedClipDict.ContainsKey(clip))
      return this.loadedClipDict[clip];
    return (MusicClipLoader.LoadedAudioClip) null;
  }

  public class LoadedAudioClip
  {
    public AudioClip clip;
    public float unloadTime;
  }
}
