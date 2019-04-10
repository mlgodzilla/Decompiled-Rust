// Decompiled with JetBrains decompiler
// Type: LocalClock
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalClock
{
  public List<LocalClock.TimedEvent> events = new List<LocalClock.TimedEvent>();

  public void Add(float delta, float variance, Action action)
  {
    this.events.Add(new LocalClock.TimedEvent()
    {
      time = Time.get_time() + delta + Random.Range(-variance, variance),
      delta = delta,
      variance = variance,
      action = action
    });
  }

  public void Tick()
  {
    for (int index = 0; index < this.events.Count; ++index)
    {
      LocalClock.TimedEvent timedEvent = this.events[index];
      if ((double) Time.get_time() > (double) timedEvent.time)
      {
        float delta = timedEvent.delta;
        float variance = timedEvent.variance;
        timedEvent.action();
        timedEvent.time = Time.get_time() + delta + Random.Range(-variance, variance);
        this.events[index] = timedEvent;
      }
    }
  }

  public struct TimedEvent
  {
    public float time;
    public float delta;
    public float variance;
    public Action action;
  }
}
