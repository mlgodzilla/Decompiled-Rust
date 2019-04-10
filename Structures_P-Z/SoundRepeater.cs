// Decompiled with JetBrains decompiler
// Type: SoundRepeater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (SoundPlayer))]
public class SoundRepeater : MonoBehaviour
{
  public float interval;
  public SoundPlayer player;

  public SoundRepeater()
  {
    base.\u002Ector();
  }
}
