// Decompiled with JetBrains decompiler
// Type: EventSchedule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System;
using UnityEngine;

public class EventSchedule : BaseMonoBehaviour
{
  [Tooltip("The minimum amount of hours between events")]
  public float minimumHoursBetween = 12f;
  [Tooltip("The maximum amount of hours between events")]
  public float maxmumHoursBetween = 24f;
  private float hoursRemaining;
  private long lastRun;

  private void OnEnable()
  {
    this.hoursRemaining = Random.Range(this.minimumHoursBetween, this.maxmumHoursBetween);
    this.InvokeRepeating(new Action(this.RunSchedule), 1f, 1f);
  }

  private void OnDisable()
  {
    if (Application.isQuitting != null)
      return;
    this.CancelInvoke(new Action(this.RunSchedule));
  }

  private void RunSchedule()
  {
    if (Application.isLoading != null || !ConVar.Server.events)
      return;
    this.CountHours();
    if ((double) this.hoursRemaining > 0.0)
      return;
    this.Trigger();
  }

  private void Trigger()
  {
    this.hoursRemaining = Random.Range(this.minimumHoursBetween, this.maxmumHoursBetween);
    TriggeredEvent[] components = (TriggeredEvent[]) ((Component) this).GetComponents<TriggeredEvent>();
    if (components.Length == 0)
      return;
    TriggeredEvent triggeredEvent = components[Random.Range(0, components.Length)];
    if (Object.op_Equality((Object) triggeredEvent, (Object) null))
      return;
    ((Component) triggeredEvent).SendMessage("RunEvent", (SendMessageOptions) 1);
  }

  private void CountHours()
  {
    if (!Object.op_Implicit((Object) TOD_Sky.get_Instance()))
      return;
    if (this.lastRun != 0L)
      this.hoursRemaining -= (float) (((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).get_DateTime().Subtract(DateTime.FromBinary(this.lastRun)).TotalSeconds / 60.0 / 60.0);
    this.lastRun = ((TOD_CycleParameters) TOD_Sky.get_Instance().Cycle).get_DateTime().ToBinary();
  }
}
