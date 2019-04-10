// Decompiled with JetBrains decompiler
// Type: WaterVisibilityTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System.Collections.Generic;
using UnityEngine;

public class WaterVisibilityTrigger : EnvironmentVolumeTrigger
{
  private static long ticks = 1;
  private static SortedList<long, WaterVisibilityTrigger> tracker = new SortedList<long, WaterVisibilityTrigger>();
  private long enteredTick;

  public static void Reset()
  {
    WaterVisibilityTrigger.ticks = 1L;
    WaterVisibilityTrigger.tracker.Clear();
  }

  protected void OnDestroy()
  {
    if (Application.isQuitting != null)
      return;
    WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
  }

  private int GetVisibilityMask()
  {
    return 0;
  }

  private void ToggleVisibility()
  {
  }

  private void ResetVisibility()
  {
  }

  private void ToggleCollision(Collider other)
  {
    if (!Object.op_Inequality((Object) WaterSystem.Collision, (Object) null))
      return;
    WaterSystem.Collision.SetIgnore(other, (Collider) this.volume.trigger, true);
  }

  private void ResetCollision(Collider other)
  {
    if (!Object.op_Inequality((Object) WaterSystem.Collision, (Object) null))
      return;
    WaterSystem.Collision.SetIgnore(other, (Collider) this.volume.trigger, false);
  }

  protected void OnTriggerEnter(Collider other)
  {
    int num1 = Object.op_Inequality((Object) ((Component) other).get_gameObject().GetComponent<PlayerWalkMovement>(), (Object) null) ? 1 : 0;
    bool flag = ((Component) other).get_gameObject().CompareTag("MainCamera");
    int num2 = flag ? 1 : 0;
    if ((num1 | num2) != 0 && !WaterVisibilityTrigger.tracker.ContainsValue(this))
    {
      this.enteredTick = WaterVisibilityTrigger.ticks++;
      WaterVisibilityTrigger.tracker.Add(this.enteredTick, this);
      this.ToggleVisibility();
    }
    if (flag || other.get_isTrigger())
      return;
    this.ToggleCollision(other);
  }

  protected void OnTriggerExit(Collider other)
  {
    int num1 = Object.op_Inequality((Object) ((Component) other).get_gameObject().GetComponent<PlayerWalkMovement>(), (Object) null) ? 1 : 0;
    bool flag = ((Component) other).get_gameObject().CompareTag("MainCamera");
    int num2 = flag ? 1 : 0;
    if ((num1 | num2) != 0 && WaterVisibilityTrigger.tracker.ContainsValue(this))
    {
      WaterVisibilityTrigger.tracker.Remove(this.enteredTick);
      if (WaterVisibilityTrigger.tracker.Count > 0)
        WaterVisibilityTrigger.tracker.Values[WaterVisibilityTrigger.tracker.Count - 1].ToggleVisibility();
      else
        this.ResetVisibility();
    }
    if (flag || other.get_isTrigger())
      return;
    this.ResetCollision(other);
  }
}
