// Decompiled with JetBrains decompiler
// Type: SoundPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class SoundPlayer : BaseMonoBehaviour, IClientComponent
{
  public bool playImmediately = true;
  public Vector3 soundOffset = Vector3.get_zero();
  public SoundDefinition soundDefinition;
  public bool debugRepeat;
  public bool pending;
}
