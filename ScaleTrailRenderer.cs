// Decompiled with JetBrains decompiler
// Type: ScaleTrailRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ScaleTrailRenderer : ScaleRenderer
{
  private TrailRenderer trailRenderer;
  [NonSerialized]
  private float startWidth;
  [NonSerialized]
  private float endWidth;
  [NonSerialized]
  private float duration;
  [NonSerialized]
  private float startMultiplier;

  public override void GatherInitialValues()
  {
    base.GatherInitialValues();
    this.trailRenderer = !Object.op_Implicit((Object) this.myRenderer) ? (TrailRenderer) ((Component) this).GetComponentInChildren<TrailRenderer>() : (TrailRenderer) ((Component) this.myRenderer).GetComponent<TrailRenderer>();
    this.startWidth = this.trailRenderer.get_startWidth();
    this.endWidth = this.trailRenderer.get_endWidth();
    this.duration = this.trailRenderer.get_time();
    this.startMultiplier = this.trailRenderer.get_widthMultiplier();
  }

  public override void SetScale_Internal(float scale)
  {
    if ((double) scale == 0.0)
    {
      this.trailRenderer.set_emitting(false);
      ((Renderer) this.trailRenderer).set_enabled(false);
      this.trailRenderer.set_time(0.0f);
      this.trailRenderer.Clear();
    }
    else
    {
      if (!this.trailRenderer.get_emitting())
        this.trailRenderer.Clear();
      this.trailRenderer.set_emitting(true);
      ((Renderer) this.trailRenderer).set_enabled(true);
      base.SetScale_Internal(scale);
      this.trailRenderer.set_widthMultiplier(this.startMultiplier * scale);
      this.trailRenderer.set_time(this.duration * scale);
    }
  }
}
