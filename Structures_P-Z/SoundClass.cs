// Decompiled with JetBrains decompiler
// Type: SoundClass
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Rust/Sound Class")]
public class SoundClass : ScriptableObject
{
  [Header("Mixer Settings")]
  public AudioMixerGroup output;
  public AudioMixerGroup firstPersonOutput;
  [Header("Occlusion Settings")]
  public bool enableOcclusion;
  public bool playIfOccluded;
  public float occlusionGain;
  [Tooltip("Use this mixer group when the sound is occluded to save DSP CPU usage. Only works for non-looping sounds.")]
  public AudioMixerGroup occludedOutput;
  [Header("Voice Limiting")]
  public int globalVoiceMaxCount;
  public List<SoundDefinition> definitions;

  public SoundClass()
  {
    base.\u002Ector();
  }
}
