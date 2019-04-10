// Decompiled with JetBrains decompiler
// Type: DeferredAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class DeferredAction
{
  private ActionPriority priority = ActionPriority.Medium;
  private Object sender;
  private Action action;

  public bool Idle { get; private set; }

  public int Index
  {
    get
    {
      return (int) this.priority;
    }
  }

  public DeferredAction(Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
  {
    this.sender = sender;
    this.action = action;
    this.priority = priority;
    this.Idle = true;
  }

  public void Action()
  {
    if (this.Idle)
      throw new Exception("Double invocation of a deferred action.");
    this.Idle = true;
    if (!Object.op_Implicit(this.sender))
      return;
    this.action();
  }

  public void Invoke()
  {
    if (!this.Idle)
      throw new Exception("Double invocation of a deferred action.");
    LoadBalancer.Enqueue(this);
    this.Idle = false;
  }

  public static implicit operator bool(DeferredAction obj)
  {
    return obj != null;
  }

  public static void Invoke(Object sender, Action action, ActionPriority priority = ActionPriority.Medium)
  {
    new DeferredAction(sender, action, priority).Invoke();
  }
}
