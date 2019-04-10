// Decompiled with JetBrains decompiler
// Type: SoundDefinition
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundDefinition : ScriptableObject
{
  public GameObjectRef template;
  [Horizontal(2, -1)]
  public List<WeightedAudioClip> weightedAudioClips;
  public List<SoundDefinition.DistanceAudioClipList> distanceAudioClips;
  public SoundClass soundClass;
  public bool defaultToFirstPerson;
  public bool loop;
  public bool randomizeStartPosition;
  [Range(0.0f, 1f)]
  public float volume;
  [Range(0.0f, 1f)]
  public float volumeVariation;
  [Range(-3f, 3f)]
  public float pitch;
  [Range(0.0f, 1f)]
  public float pitchVariation;
  [Header("Voice limiting")]
  public bool dontVoiceLimit;
  public int globalVoiceMaxCount;
  public int localVoiceMaxCount;
  public float localVoiceRange;
  public float voiceLimitFadeOutTime;
  public float localVoiceDebounceTime;
  [Header("Occlusion Settings")]
  public bool forceOccludedPlayback;
  [Header("Custom curves")]
  public AnimationCurve falloffCurve;
  public bool useCustomFalloffCurve;
  public AnimationCurve spatialBlendCurve;
  public bool useCustomSpatialBlendCurve;
  public AnimationCurve spreadCurve;
  public bool useCustomSpreadCurve;

  public float maxDistance
  {
    get
    {
      if (this.template == null)
        return 0.0f;
      AudioSource component = (AudioSource) this.template.Get().GetComponent<AudioSource>();
      if (Object.op_Equality((Object) component, (Object) null))
        return 0.0f;
      return component.get_maxDistance();
    }
  }

  public float GetLength()
  {
    float num = 0.0f;
    for (int index = 0; index < this.weightedAudioClips.Count; ++index)
    {
      AudioClip audioClip = this.weightedAudioClips[index].audioClip;
      if (Object.op_Implicit((Object) audioClip))
        num = Mathf.Max(audioClip.get_length(), num);
    }
    for (int index1 = 0; index1 < this.distanceAudioClips.Count; ++index1)
    {
      List<WeightedAudioClip> audioClips = this.distanceAudioClips[index1].audioClips;
      for (int index2 = 0; index2 < audioClips.Count; ++index2)
      {
        AudioClip audioClip = audioClips[index2].audioClip;
        if (Object.op_Implicit((Object) audioClip))
          num = Mathf.Max(audioClip.get_length(), num);
      }
    }
    return num;
  }

  public SoundDefinition()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class DistanceAudioClipList
  {
    public int distance;
    [Horizontal(2, -1)]
    public List<WeightedAudioClip> audioClips;
  }
}
