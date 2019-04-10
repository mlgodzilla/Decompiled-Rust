// Decompiled with JetBrains decompiler
// Type: ConVar.Time
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("time")]
  public class Time : ConsoleSystem
  {
    [ServerVar]
    [Help("Pause time while loading")]
    public static bool pausewhileloading = true;

    [Help("Fixed delta time in seconds")]
    [ServerVar]
    public static float fixeddelta
    {
      get
      {
        return Time.get_fixedDeltaTime();
      }
      set
      {
        Time.set_fixedDeltaTime(value);
      }
    }

    [Help("The minimum amount of times to tick per frame")]
    [ServerVar]
    public static float maxdelta
    {
      get
      {
        return Time.get_maximumDeltaTime();
      }
      set
      {
        Time.set_maximumDeltaTime(value);
      }
    }

    [Help("The time scale")]
    [ServerVar]
    public static float timescale
    {
      get
      {
        return Time.get_timeScale();
      }
      set
      {
        Time.set_timeScale(value);
      }
    }

    public Time()
    {
      base.\u002Ector();
    }
  }
}
