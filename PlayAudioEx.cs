// Decompiled with JetBrains decompiler
// Type: PlayAudioEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PlayAudioEx : MonoBehaviour
{
  public float delay;

  private void Start()
  {
  }

  private void OnEnable()
  {
    AudioSource component = (AudioSource) ((Component) this).GetComponent<AudioSource>();
    if (!Object.op_Implicit((Object) component))
      return;
    component.PlayDelayed(this.delay);
  }

  public PlayAudioEx()
  {
    base.\u002Ector();
  }
}
