// Decompiled with JetBrains decompiler
// Type: MixerSnapshotManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Audio;

public class MixerSnapshotManager : MonoBehaviour
{
  public AudioMixerSnapshot defaultSnapshot;
  public AudioMixerSnapshot underwaterSnapshot;
  public AudioMixerSnapshot loadingSnapshot;
  public AudioMixerSnapshot woundedSnapshot;
  public SoundDefinition underwaterInSound;
  public SoundDefinition underwaterOutSound;
  public SoundDefinition woundedLoop;
  private Sound woundedLoopSound;

  public MixerSnapshotManager()
  {
    base.\u002Ector();
  }
}
