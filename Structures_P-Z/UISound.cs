// Decompiled with JetBrains decompiler
// Type: UISound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using UnityEngine;

public static class UISound
{
  private static AudioSource source;

  private static AudioSource GetAudioSource()
  {
    if (Object.op_Inequality((Object) UISound.source, (Object) null))
      return UISound.source;
    UISound.source = (AudioSource) new GameObject(nameof (UISound)).AddComponent<AudioSource>();
    UISound.source.set_spatialBlend(0.0f);
    UISound.source.set_volume(1f);
    return UISound.source;
  }

  public static void Play(AudioClip clip, float volume = 1f)
  {
    if (Object.op_Equality((Object) clip, (Object) null))
      return;
    UISound.GetAudioSource().set_volume((float) ((double) volume * (double) Audio.master * 0.400000005960464));
    UISound.GetAudioSource().PlayOneShot(clip);
  }
}
