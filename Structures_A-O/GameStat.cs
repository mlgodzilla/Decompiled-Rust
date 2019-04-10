// Decompiled with JetBrains decompiler
// Type: GameStat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class GameStat : MonoBehaviour
{
  public float refreshTime;
  public Text title;
  public Text globalStat;
  public Text localStat;
  private long globalValue;
  private long localValue;
  private long oldGlobalValue;
  private long oldLocalValue;
  private float secondsSinceRefresh;
  private float secondsUntilUpdate;
  private float secondsUntilChange;
  public GameStat.Stat[] stats;

  public GameStat()
  {
    base.\u002Ector();
  }

  [Serializable]
  public struct Stat
  {
    public string statName;
    public string statTitle;
  }
}
