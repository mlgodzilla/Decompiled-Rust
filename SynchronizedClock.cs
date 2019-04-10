// Decompiled with JetBrains decompiler
// Type: SynchronizedClock
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class SynchronizedClock
{
  public List<SynchronizedClock.TimedEvent> events = new List<SynchronizedClock.TimedEvent>();

  private static long Ticks
  {
    get
    {
      if (!Object.op_Implicit((Object) TOD_Sky.get_Instance()))
        return DateTime.Now.Ticks;
      return ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).get_Ticks();
    }
  }

  private static float DayLengthInMinutes
  {
    get
    {
      if (!Object.op_Implicit((Object) TOD_Sky.get_Instance()))
        return 30f;
      return (float) TOD_Sky.get_Instance().get_Components().get_Time().DayLengthInMinutes;
    }
  }

  public void Add(float delta, float variance, Action<uint> action)
  {
    this.events.Add(new SynchronizedClock.TimedEvent()
    {
      ticks = SynchronizedClock.Ticks,
      delta = delta,
      variance = variance,
      action = action
    });
  }

  public void Tick()
  {
    double num1 = 10000000.0 * (1440.0 / (double) SynchronizedClock.DayLengthInMinutes);
    for (int index = 0; index < this.events.Count; ++index)
    {
      SynchronizedClock.TimedEvent timedEvent = this.events[index];
      long ticks1 = timedEvent.ticks;
      long ticks2 = SynchronizedClock.Ticks;
      long num2 = (long) ((double) timedEvent.delta * num1);
      long num3 = ticks1 / num2 * num2;
      uint num4 = (uint) ((ulong) num3 % (ulong) uint.MaxValue);
      int num5 = (int) SeedRandom.Wanghash(ref num4);
      long num6 = (long) ((double) SeedRandom.Range(ref num4, -timedEvent.variance, timedEvent.variance) * num1);
      long num7 = num3 + num2 + num6;
      if (ticks1 < num7 && ticks2 >= num7)
      {
        timedEvent.action(num4);
        timedEvent.ticks = ticks2;
      }
      else if (ticks2 > ticks1 || ticks2 < num3)
        timedEvent.ticks = ticks2;
      this.events[index] = timedEvent;
    }
  }

  public struct TimedEvent
  {
    public long ticks;
    public float delta;
    public float variance;
    public Action<uint> action;
  }
}
