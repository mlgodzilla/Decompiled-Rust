// Decompiled with JetBrains decompiler
// Type: Sound
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Sound : MonoBehaviour, IClientComponent
{
  public static float volumeExponent = Mathf.Log(Mathf.Sqrt(10f), 2f);
  public SoundDefinition definition;
  public SoundModifier[] modifiers;
  public SoundSource soundSource;
  public AudioSource[] audioSources;
  [SerializeField]
  private SoundFade _fade;
  [SerializeField]
  private SoundModulation _modulation;
  [SerializeField]
  private SoundOcclusion _occlusion;

  public SoundFade fade
  {
    get
    {
      return this._fade;
    }
  }

  public SoundModulation modulation
  {
    get
    {
      return this._modulation;
    }
  }

  public SoundOcclusion occlusion
  {
    get
    {
      return this._occlusion;
    }
  }

  public Sound()
  {
    base.\u002Ector();
  }
}
