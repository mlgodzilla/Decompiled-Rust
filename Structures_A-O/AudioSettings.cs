// Decompiled with JetBrains decompiler
// Type: AudioSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
  public AudioMixer mixer;

  private void Update()
  {
    if (Object.op_Equality((Object) this.mixer, (Object) null))
      return;
    this.mixer.SetFloat("MasterVol", this.LinearToDecibel(ConVar.Audio.master));
    float num;
    this.mixer.GetFloat("MusicVol", ref num);
    if (!LevelManager.isLoaded || !MainCamera.isValid)
      this.mixer.SetFloat("MusicVol", Mathf.Lerp(num, this.LinearToDecibel(ConVar.Audio.musicvolumemenu), Time.get_deltaTime()));
    else
      this.mixer.SetFloat("MusicVol", Mathf.Lerp(num, this.LinearToDecibel(ConVar.Audio.musicvolume), Time.get_deltaTime()));
    this.mixer.SetFloat("WorldVol", this.LinearToDecibel(ConVar.Audio.game));
    this.mixer.SetFloat("VoiceVol", this.LinearToDecibel(ConVar.Audio.voices));
  }

  private float LinearToDecibel(float linear)
  {
    return (double) linear <= 0.0 ? -144f : 20f * Mathf.Log10(linear);
  }

  public AudioSettings()
  {
    base.\u002Ector();
  }
}
