// Decompiled with JetBrains decompiler
// Type: MusicZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class MusicZone : MonoBehaviour, IClientComponent
{
  public List<MusicTheme> themes;
  public float priority;
  public bool suppressAutomaticMusic;

  public MusicZone()
  {
    base.\u002Ector();
  }
}
